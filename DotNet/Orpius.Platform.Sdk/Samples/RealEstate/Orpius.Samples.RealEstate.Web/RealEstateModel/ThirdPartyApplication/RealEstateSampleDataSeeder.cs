namespace Orpius.Samples.RealEstate
{
	public class RealEstateSampleDataSeeder
	{
		readonly ApplicantDatabase applicantDatabase;
		readonly ListingDatabase listingDatabase;
		readonly IRealEstateAgentIdentityService identityService;

		public RealEstateSampleDataSeeder(
			ApplicantDatabase applicantDatabase,
			ListingDatabase listingDatabase,
			IRealEstateAgentIdentityService identityService)
		{
			this.applicantDatabase = applicantDatabase
									 ?? throw new ArgumentNullException(nameof(applicantDatabase));

			this.listingDatabase = listingDatabase
								   ?? throw new ArgumentNullException(nameof(listingDatabase));

			this.identityService = identityService
								   ?? throw new ArgumentNullException(nameof(identityService));
		}

		public async Task SeedAsync()
		{
			await SeedApplicantsAsync();
			await SeedListingsAsync();
		}

		async Task SeedApplicantsAsync()
		{
			if (applicantDatabase.GetAll().Count > 0)
			{
				return;
			}

			await applicantDatabase.RegisterAsync(
				new RegisterApplicantRequest
				{
					FirstName = "Jane",
					Surname   = "Smith",
					IdealPropertyFeatures =
						"Looking for a 4 bedroom house near Lausanne with a garden and parking."
				},
				"jane.smith@example.com",
				DemoRealEstateAgents.CurrentAgentId);

			await applicantDatabase.RegisterAsync(
				new RegisterApplicantRequest
				{
					FirstName = "Adam",
					Surname   = "Brown",
					IdealPropertyFeatures =
						"Looking for a modern flat near public transport, preferably with a balcony."
				},
				"adam.brown@example.com",
				DemoRealEstateAgents.LausanneAgentId);

			await applicantDatabase.RegisterAsync(
				new RegisterApplicantRequest
				{
					FirstName = "Sophie",
					Surname   = "Martin",
					IdealPropertyFeatures =
						"Looking for a family house with at least 3 bedrooms, a garden, and a quiet location."
				},
				"sophie.martin@example.com",
				DemoRealEstateAgents.RivieraAgentId);

			await applicantDatabase.RegisterAsync(
				new RegisterApplicantRequest
				{
					FirstName = "Marc",
					Surname   = "Dubois",
					IdealPropertyFeatures =
						"Looking for a villa around Montreux with lake views, a swimming pool, garden, and parking."
				},
				"marc.dubois@example.com",
				DemoRealEstateAgents.RivieraAgentId);

			await applicantDatabase.RegisterAsync(
				new RegisterApplicantRequest
				{
					FirstName = "Emily",
					Surname   = "Jones",
					IdealPropertyFeatures =
						"Looking for a 2 bedroom flat in Vevey close to public transport with a balcony."
				},
				"emily.jones@example.com",
				DemoRealEstateAgents.LausanneAgentId);
		}

		async Task SeedListingsAsync()
		{
			if (listingDatabase.GetAll().Count > 0)
			{
				return;
			}

			await listingDatabase.RegisterAsync(
				new RegisterPropertyListingRequest
				{
					PropertyType = "House",
					Location = "Lausanne",
					BedroomCount = 4,
					BathroomCount = 2,
					SwimmingPool = false,
					Garden = true,
					Parking = true,
					Balcony = false,
					Price = 1450000m,
					Description =
						"Spacious 4 bedroom family house in Lausanne"
						+ " with a private garden, parking, "
						+ "and generous living areas."
				});

			await listingDatabase.RegisterAsync(
				new RegisterPropertyListingRequest
				{
					PropertyType = "Flat",
					Location = "Vevey",
					BedroomCount = 2,
					BathroomCount = 1,
					SwimmingPool = false,
					Garden = false,
					Parking = false,
					Balcony = true,
					Price = 780000m,
					Description =
						"Modern 2 bedroom flat in Vevey close "
						+ "to public transport, with a balcony "
						+ "and bright open-plan living space."
				});

			await listingDatabase.RegisterAsync(
				new RegisterPropertyListingRequest
				{
					PropertyType = "Villa",
					Location = "Montreux",
					BedroomCount = 5,
					BathroomCount = 3,
					SwimmingPool = true,
					Garden = true,
					Parking = true,
					Balcony = true,
					Price = 2600000m,
					Description =
						"Elegant villa in Montreux with lake views, "
						+ "swimming pool, landscaped garden, parking, "
						+ "and large entertaining spaces."
				});
		}
	}
}