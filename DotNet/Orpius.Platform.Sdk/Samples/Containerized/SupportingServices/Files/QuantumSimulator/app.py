import asyncio
import multiprocessing as mp
import signal
import time
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
		queue.put({"ok": False, "error": str(ex)})


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
				raise HTTPException(
					status_code=408,
					detail=f"Simulation exceeded time limit ({TIMEOUT_SECONDS}s). exitCode={exit_code}"
				)

			await asyncio.sleep(0.05)

		process.join()

		if queue.empty():
			exit_code = process.exitcode
			raise HTTPException(
				status_code=500,
				detail=f"Simulator worker exited without output (exitCode={exit_code}). "
					   f"This usually indicates a hard kill (CPU/memory limit) or native crash."
			)

		payload = queue.get()

		if not payload.get("ok", False):
			error_text = payload.get("error", "Unknown error.")
		
			if "SIGXCPU" in error_text or "SIGTERM" in error_text:
				raise HTTPException(status_code=408, detail=error_text)
		
			raise HTTPException(status_code=400, detail=error_text)

		return {
			"counts": payload["counts"],
			"elapsedSeconds": time.monotonic() - start_time
		}
