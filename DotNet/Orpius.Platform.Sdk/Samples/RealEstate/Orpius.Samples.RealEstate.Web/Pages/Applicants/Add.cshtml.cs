using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Orpius.Samples.RealEstate.Pages.Shared;

namespace Orpius.Samples.RealEstate.Web.Pages.Applicants
{
	public class AddModel : PageModel
	{
		readonly ApplicantChat applicantChat;

		public AddModel(ApplicantChat applicantChat)
		{
			this.applicantChat = applicantChat
									   ?? throw new ArgumentNullException(nameof(applicantChat));
		}

		[BindProperty]
		public Guid? ConversationId { get; set; }

		[BindProperty]
		public string EmailAddress { get; set; } = "";

		[BindProperty]
		public string ApplicantText { get; set; } = "";

		public async Task<IActionResult> OnPostStreamAsync(CancellationToken token)
		{
			await OperationMessageStreamWriter.PrepareResponseAsync(
				Response,
				token);

			if (string.IsNullOrWhiteSpace(EmailAddress))
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;

				await OperationMessageStreamWriter.WriteAsync(
					Response,
					new OperationMessageView
					{
						Role = OperationMessageRole.System,
						Text = "Enter the applicant's email address.",
						Success = false
					},
					token);

				return new EmptyResult();
			}

			if (string.IsNullOrWhiteSpace(ApplicantText))
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;

				await OperationMessageStreamWriter.WriteAsync(
					Response,
					new OperationMessageView
					{
						Role = OperationMessageRole.System,
						Text = "Enter the applicant details first.",
						Success = false
					},
					token);

				return new EmptyResult();
			}

			await OperationMessageStreamWriter.WriteAsync(
				Response,
				new OperationMessageView
				{
					Role = OperationMessageRole.User,
					Text = ApplicantText
				},
				token);

			await foreach (OperationMessageView message in
						   applicantChat.AddApplicantFromTextAsync(
							   EmailAddress,
							   ApplicantText,
							   ConversationId,
							   token))
			{
				await OperationMessageStreamWriter.WriteAsync(
					Response,
					message,
					token);
			}

			return new EmptyResult();
		}
	}
}