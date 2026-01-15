import asyncio
import multiprocessing as mp
import re
import signal
import time
import traceback
from typing import Any

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

MAX_QASM_CHARS = 64_000
MAX_SHOTS = 10_000
MAX_CONCURRENCY = 4
TIMEOUT_SECONDS = 15

semaphore = asyncio.Semaphore(MAX_CONCURRENCY)

class ExecuteRequest(BaseModel):
	qasm: str
	shots: int = 1024

def apply_limits() -> None:
	# Linux-only hard limits. On Windows these will not work.
	try:
		import resource

		# CPU time limit in seconds.
		resource.setrlimit(resource.RLIMIT_CPU, (8, 10))

		# Address space limit (best-effort).
		memory_bytes = 800 * 1024 * 1024  # 800 MB
		resource.setrlimit(resource.RLIMIT_AS, (memory_bytes, memory_bytes))

		# Limit number of open files.
		resource.setrlimit(resource.RLIMIT_NOFILE, (256, 256))
	except Exception:
		# If limits cannot be applied, continue.
		pass

def validate_circuit(circuit) -> None:
	max_qubits = 20
	max_ops = 50_000

	qubit_count = circuit.num_qubits
	if qubit_count > max_qubits:
		raise ValueError(
			f"Too many qubits: {qubit_count} (max {max_qubits})"
		)

	op_count = circuit.size()
	if op_count > max_ops:
		raise ValueError(f"Too many operations: {op_count} (max {max_ops})")

def install_signal_handlers() -> None:
	def on_cpu_limit(signum, frame):
		raise TimeoutError("CPU time limit exceeded (SIGXCPU).")

	def on_terminate(signum, frame):
		raise TimeoutError("Simulation terminated (SIGTERM).")

	try:
		signal.signal(signal.SIGXCPU, on_cpu_limit)
	except Exception:
		pass

	try:
		signal.signal(signal.SIGTERM, on_terminate)
	except Exception:
		pass

QASM_LOCATION_PATTERN = re.compile(r"(?P<line>\d+)\s*,\s*(?P<col>\d+)\s*:\s*(?P<msg>.+)")

def build_error_payload(ex: Exception) -> dict[str, Any]:
	exception_type = type(ex).__name__

	# str(ex) is sometimes empty, so fall back to repr(ex)
	message = str(ex).strip()
	if not message:
		message = repr(ex)

	location = None
	match = QASM_LOCATION_PATTERN.match(message)
	if match:
		location = {
			"line": int(match.group("line")),
			"column": int(match.group("col")),
		}
		# keep only the human message part
		message = match.group("msg").strip()

	trace = "".join(traceback.format_exception(type(ex), ex, ex.__traceback__))

	# avoid returning huge traces
	max_trace_chars = 8_000
	if len(trace) > max_trace_chars:
		trace = trace[:max_trace_chars] + "\n... (truncated)"

	return {
		"type": exception_type,
		"message": message,
		"location": location,
		"traceback": trace,
	}

def classify_http_status(error: dict[str, Any]) -> int:
	# Map common failures to better HTTP status codes
	message = (error.get("message") or "").lower()
	exception_type = (error.get("type") or "").lower()

	if "sigxcpu" in message or "sigterm" in message:
		return 408

	if "cpu time limit exceeded" in message or "time limit" in message or "timeout" in message:
		return 408

	if "memory" in message or "out of memory" in message or "killed" in message:
		return 413

	# Treat QASM parse/import issues as "unprocessable entity"
	if "qasm" in exception_type or "import" in exception_type or "parse" in exception_type:
		return 422

	# Validation errors from validate_circuit are bad input
	if exception_type in ("valueerror",):
		return 422

	return 400

def get_signal_name_from_exit_code(exit_code: int | None) -> str | None:
	# Add signal name decoding for negative exit codes on POSIX
	if exit_code is None or exit_code >= 0:
		return None

	signal_number = -exit_code
	try:
		return signal.Signals(signal_number).name
	except Exception:
		return f"SIG{signal_number}"

def run_job(qasm_text: str, shots: int, queue: mp.Queue) -> None:
	install_signal_handlers()
	apply_limits()

	try:
		from qiskit import qasm3, transpile
		from qiskit_aer import Aer

		circuit = qasm3.loads(qasm_text)
		validate_circuit(circuit)
		simulator = Aer.get_backend("aer_simulator")

		# Keep optimisation low to avoid expensive transpilation.
		compiled = transpile(circuit, simulator, optimization_level=0)

		result = simulator.run(compiled, shots=shots).result()
		counts = result.get_counts()

		queue.put({"ok": True, "counts": counts})
	except Exception as ex:
		queue.put({"ok": False, "error": build_error_payload(ex)})


@app.post("/execute")
async def execute(request: ExecuteRequest) -> Any:
	if len(request.qasm) > MAX_QASM_CHARS:
		raise HTTPException(status_code=413, detail="QASM program is too large.")

	if request.shots < 1 or request.shots > MAX_SHOTS:
		raise HTTPException(
			status_code=400,
			detail=f"Shots must be between 1 and {MAX_SHOTS}."
		)

	async with semaphore:
		queue = mp.SimpleQueue()
		process = mp.Process(
			target=run_job,
			args=(request.qasm, request.shots, queue),
			daemon=True
		)

		start_time = time.monotonic()
		process.start()

		# Poll without blocking the event loop.
		while process.is_alive():
			elapsed = time.monotonic() - start_time
			if elapsed > TIMEOUT_SECONDS:
				process.terminate()
				process.join(timeout=1.0)
			  
				if process.is_alive():
					process.kill()
					process.join()
		  
				exit_code = process.exitcode
				signal_name = get_signal_name_from_exit_code(exit_code)

				raise HTTPException(
					status_code=408,
					detail={
						"type": "Timeout",
						"message": f"Simulation exceeded time limit ({TIMEOUT_SECONDS}s).",
						"elapsedSeconds": elapsed,
						"exitCode": exit_code,
						"signal": signal_name,
					}
				)

			await asyncio.sleep(0.05)

		process.join()

		if queue.empty():
			exit_code = process.exitcode
			signal_name = get_signal_name_from_exit_code(exit_code)

			raise HTTPException(
				status_code=500,
				detail={
					"type": "WorkerCrashed",
					"message": "Simulator worker exited without output."
							   "This usually indicates a hard kill "
							   "(CPU/memory limit) or a native crash.",
					"exitCode": exit_code,
					"signal": signal_name,
					"elapsedSeconds": time.monotonic() - start_time,
				}
			)

		payload = queue.get()

		if not payload.get("ok", False):
			error = payload.get("error") or {"type": "UnknownError", "message": "Unknown error."}

			status_code = classify_http_status(error)
			raise HTTPException(status_code=status_code, detail=error)

		return {
			"counts": payload["counts"],
			"elapsedSeconds": time.monotonic() - start_time
		}
