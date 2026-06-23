namespace Orpius.Samples.RealEstate
{
	public class ListingDatabase
	{
		readonly object syncRoot = new();

		readonly Dictionary<Guid, ListingRecord> listingsById = new();

		public async Task<RegisterPropertyListingResult> RegisterAsync(
			RegisterPropertyListingRequest request)
		{
			ArgumentNullException.ThrowIfNull(request);

			ListingRecord listing = new(
				Guid.NewGuid(),
				request.PropertyType,
				request.Location,
				request.BedroomCount,
				request.BathroomCount,
				request.SwimmingPool,
				request.Garden,
				request.Parking,
				request.Balcony,
				request.Price,
				request.Description);

			lock (syncRoot)
			{
				listingsById.Add(listing.Id, listing);
			}

			return new RegisterPropertyListingResult(listing);
		}

		public bool TryGetById(Guid listingId, out ListingRecord? listing)
		{
			lock (syncRoot)
			{
				return listingsById.TryGetValue(listingId, out listing);
			}
		}

		public IReadOnlyList<ListingRecord> GetAll()
		{
			lock (syncRoot)
			{
				return listingsById
					   .Values
					   .OrderBy(listing => listing.Location)
					   .ThenBy(listing => listing.PropertyType)
					   .ToArray();
			}
		}
	}

	public sealed record ListingRecord(
		Guid Id,
		string PropertyType,
		string Location,
		int? BedroomCount,
		int? BathroomCount,
		bool SwimmingPool,
		bool Garden,
		bool Parking,
		bool Balcony,
		decimal? Price,
		string Description);

	public sealed record RegisterPropertyListingResult(
		ListingRecord Listing);
}