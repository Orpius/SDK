using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Orpius.Samples.RealEstate.Web.Pages.Applicants
{
	public class AddModel : PageModel
	{
		readonly RealEstateConversationService conversationService;

		public AddModel(RealEstateConversationService conversationService)
		{
			this.conversationService = conversationService
									   ?? throw new ArgumentNullException(nameof(conversationService));
		}

		[BindProperty]
		public string EmailAddress { get; set; } = "";

		[BindProperty]
		public string ApplicantText { get; set; } = "";

		public List<OperationMessageView> Messages { get; } = new();

		public async Task<IActionResult> OnPostAsync(CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(EmailAddress))
			{
				ModelState.AddModelError(
					nameof(EmailAddress),
					"Enter the applicant's email address.");
			}

			if (string.IsNullOrWhiteSpace(ApplicantText))
			{
				ModelState.AddModelError(
					nameof(ApplicantText),
					"Enter the applicant details first.");
			}

			if (!ModelState.IsValid)
			{
				return Page();
			}

			await foreach (OperationMessageView message in
						   conversationService.AddApplicantFromTextAsync(
							   EmailAddress,
							   ApplicantText,
							   token))
			{
				Messages.Add(message);
			}

			ApplicantText = "";

			return Page();
		}
	}
}