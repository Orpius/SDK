using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Orpius.Samples.RealEstate.Pages.Shared;

namespace Orpius.Samples.RealEstate.Web.Pages.Listings
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
		public string ListingText { get; set; } = "";

		public async Task<IActionResult> OnPostStreamAsync(CancellationToken token)
		{
			await OperationMessageStreamWriter.PrepareResponseAsync(
				Response,
				token);

			if (string.IsNullOrWhiteSpace(ListingText))
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;

				await OperationMessageStreamWriter.WriteAsync(
					Response,
					new OperationMessageView
					{
						Role    = OperationMessageRole.System,
						Text    = "Enter the listing details first.",
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
					Text = ListingText
				},
				token);

			await foreach (OperationMessageView message in
						   conversationService.AddListingFromTextAsync(
							   ListingText,
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