using Orpius.Platform.Tooling;

namespace Orpius.Samples.RealEstate
{
	[Tool]
	public class PropertyLister
	{
		readonly ListingDatabase listingDatabase;

		public PropertyLister(ListingDatabase listingDatabase)
		{
			this.listingDatabase = listingDatabase
								   ?? throw new ArgumentNullException(nameof(listingDatabase));
		}

		[ToolMethod(Description
			= """
			  Register a new property listing.
			  Use this when an estate agent provides a description of a house,
			  flat, apartment, or other property that is available for sale.
			  The property details should be extracted from the estate agent's description.
			  """)]
		public async Task<RegisterPropertyListingResponse> RegisterPropertyListing(
			RegisterPropertyListingRequest request,
			ICombinedContext context)
		{
			RegisterPropertyListingResult result
				= await listingDatabase.RegisterAsync(request);

			RegisterPropertyListingResponse response = new()
			{
				ListingId = result.Listing.Id
			};

			return response;
		}
	}

	public class RegisterPropertyListingRequest
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  The type of property being listed.
				  For example: house, flat, apartment, villa, townhouse, chalet, studio, or land.
				  """)]
		public required string PropertyType { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  The location of the property.
				  Include the town, city, area, district, or region if provided.
				  """)]
		public required string Location { get; set; }

		[ToolProperty(
			Required = false,
			Description = "The number of bedrooms in the property, if specified.")]
		public int? BedroomCount { get; set; }

		[ToolProperty(
			Required = false,
			Description = "The number of bathrooms in the property, if specified.")]
		public int? BathroomCount { get; set; }

		[ToolProperty(
			Required = true,
			Description = "Indicates whether the property has a swimming pool.")]
		public bool SwimmingPool { get; set; }

		[ToolProperty(
			Required = true,
			Description = "Indicates whether the property has a garden.")]
		public bool Garden { get; set; }

		[ToolProperty(
			Required = true,
			Description = "Indicates whether the property has parking.")]
		public bool Parking { get; set; }

		[ToolProperty(
			Required = true,
			Description = "Indicates whether the property has a balcony.")]
		public bool Balcony { get; set; }

		[ToolProperty(
			Required = false,
			Description = "The asking price of the property, if specified.")]
		public decimal? Price { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  A concise description of the property.
				  Include relevant features, constraints, style, condition, location notes,
				  and anything else that may help match the property to an applicant.
				  """)]
		public required string Description { get; set; }
	}

	public class RegisterPropertyListingResponse
	{
		[ToolProperty(
			Required = true,
			Description = "The unique identifier for the registered property listing.")]
		public Guid ListingId { get; set; }
	}
}