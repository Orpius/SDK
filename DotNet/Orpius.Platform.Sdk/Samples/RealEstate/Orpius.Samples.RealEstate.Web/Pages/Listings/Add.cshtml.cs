using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Orpius.Samples.RealEstate;

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

		public List<OperationMessageView> Messages { get; } = new();

		public async Task<IActionResult> OnPostAsync(CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(ListingText))
			{
				ModelState.AddModelError(
					nameof(ListingText),
					"Enter the listing details first.");

				return Page();
			}

			await foreach (OperationMessageView message in
						   conversationService.AddListingFromTextAsync(
							   ListingText,
							   token))
			{
				Messages.Add(message);
			}

			ListingText = "";

			return Page();
		}
	}
}