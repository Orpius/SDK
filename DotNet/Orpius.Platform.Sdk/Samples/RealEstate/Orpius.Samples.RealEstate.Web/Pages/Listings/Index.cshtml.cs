using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Orpius.Samples.RealEstate.Pages.Listings
{
	public class IndexModel : PageModel
	{
		readonly ListingDatabase listingDatabase;

		public IndexModel(ListingDatabase listingDatabase)
		{
			this.listingDatabase = listingDatabase
								   ?? throw new ArgumentNullException(nameof(listingDatabase));
		}

		public IReadOnlyList<ListingRecord> Listings { get; private set; }
			= Array.Empty<ListingRecord>();

		public void OnGet()
		{
			Listings = listingDatabase.GetAll();
		}
	}
}