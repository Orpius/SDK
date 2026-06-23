using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Orpius.Samples.RealEstate.Pages.Shared;

namespace Orpius.Samples.RealEstate.Web.Pages.Listings
{
	public class AddModel : PageModel
	{
		readonly ListingChat listingChat;

		public AddModel(ListingChat listingChat)
		{
			this.listingChat = listingChat
									   ?? throw new ArgumentNullException(nameof(listingChat));
		}

		[BindProperty]
		public Guid? ConversationId { get; set; }

		[BindProperty]
		public IFormFile? ListingImage { get; set; }

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

			string? jsonProvidedToAgent;

			try
			{
				jsonProvidedToAgent
					= await ListingImageForAgent.CreateJsonAsync(
										  ListingImage,
										  token);
			}
			catch (Exception ex)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;

				await OperationMessageStreamWriter.WriteAsync(
					Response,
					new OperationMessageView
					{
						Role    = OperationMessageRole.System,
						Text    = ex.Message,
						Success = false
					},
					token);

				return new EmptyResult();
			}

			await foreach (OperationMessageView message in
						   listingChat.AddListingFromTextAsync(
							   ListingText,
							   ConversationId,
							   // Disabled until file attachment in place.
							   /*jsonProvidedToAgent*/null,
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