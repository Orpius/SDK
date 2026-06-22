using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Orpius.Samples.RealEstate.Pages.Applicants
{
	public class IndexModel : PageModel
	{
		readonly ApplicantDatabase applicantDatabase;

		public IndexModel(ApplicantDatabase applicantDatabase)
		{
			this.applicantDatabase = applicantDatabase
									 ?? throw new ArgumentNullException(nameof(applicantDatabase));
		}

		public IReadOnlyList<ApplicantRecord> Applicants { get; private set; }
			= Array.Empty<ApplicantRecord>();

		public void OnGet()
		{
			Applicants = applicantDatabase.GetAll();
		}
	}
}