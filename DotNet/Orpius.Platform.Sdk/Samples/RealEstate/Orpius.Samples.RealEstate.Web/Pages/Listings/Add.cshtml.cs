using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Orpius.Samples.RealEstate.Pages.Shared;

namespace Orpius.Samples.RealEstate.Web.Pages.Listings
{
	public class AddModel : PageModel
	{
		readonly ListingConversationService conversationService;

		public AddModel(ListingConversationService conversationService)
		{
			this.conversationService = conversationService
									   ?? throw new ArgumentNullException(nameof(conversationService));
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
						   conversationService.AddListingFromTextAsync(
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