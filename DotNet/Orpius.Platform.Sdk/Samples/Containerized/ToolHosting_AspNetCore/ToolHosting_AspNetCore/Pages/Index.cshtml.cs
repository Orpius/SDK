using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToolHosting_AspNetCore.Pages
{
	public class IndexModel : PageModel
	{
		readonly QuantumSimulatorClient quantumSimulatorClient;

		[BindProperty]
		public string OpenQasmProgram { get; set; } = defaultOpenQasmProgram;

		[BindProperty]
		public int Shots { get; set; } = 1000;

		public Dictionary<string, int>? ResultCounts { get; private set; }

		public string? ErrorMessage { get; private set; }

		public IndexModel(QuantumSimulatorClient quantumSimulatorClient)
		{
			this.quantumSimulatorClient = quantumSimulatorClient;
		}

		public void OnGet()
		{
			/* Default values already set via property initializers. */
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (string.IsNullOrWhiteSpace(OpenQasmProgram))
			{
				ErrorMessage = "Please paste an OpenQASM 3 program.";
				return Page();
			}

			if (Shots < 1)
			{
				ErrorMessage = "Shots must be at least 1.";
				return Page();
			}

			try
			{
				ResultCounts = await quantumSimulatorClient.ExecuteAsync(
								   OpenQasmProgram,
								   Shots,
								   token: HttpContext.RequestAborted);

				return Page();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				return Page();
			}
		}

		const string defaultOpenQasmProgram =
			@"OPENQASM 3;
include ""stdgates.inc"";

qubit q;
bit c;

h q;
measure q -> c;
";
	}
}