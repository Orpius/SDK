using Orpius.Platform.Tooling;

namespace Orpius.Samples.RealEstate
{
	[Tool]
	public class ApplicantRegistrar
	{
		readonly ApplicantDatabase applicantDatabase;

		public ApplicantRegistrar(ApplicantDatabase applicantDatabase)
		{
			this.applicantDatabase = applicantDatabase
									 ?? throw new ArgumentNullException(nameof(applicantDatabase));
		}

		[ToolMethod(Description
			= """
			  Add a new prospective buyer to the applicant registrar database.
			  The applicant's email address and the real estate agent identifier are supplied
			  securely through the tool context and are not part of the agent-visible request.
			  This allows known properties to be matched with applicants.
			  """)]
		public async Task<RegisterApplicantResponse> RegisterApplicant(
			RegisterApplicantRequest request,
			ICombinedContext context)
		{
			string emailAddress = context.GetRequiredContextValue(
				RealEstateContextKeys.ApplicantEmailAddress);

			Guid realEstateAgentId = context.GetRequiredGuidContextValue(
				RealEstateContextKeys.RealEstateAgentId);

			RegisterApplicantResult result = await applicantDatabase.RegisterAsync(
												 request,
												 emailAddress,
												 realEstateAgentId);

			RegisterApplicantResponse response = new()
			{
				Added       = result.Added,
				ApplicantId = result.Applicant.Id
			};

			return response;
		}

		[ToolMethod(Description
			= """
			  Gets privacy-preserving applicant profiles for matching against a property listing.
			  The result is keyed by applicant identifier.
			  Each profile includes the applicant's desired property features, a short applicant label,
			  and the real estate agent identifier to use when notifying the agent.
			  Do not use the built-in Notifier for real estate agents.
			  Use RealEstateAgentMessenger when notifying real estate agents.
			  """)]
		public async Task<GetApplicantProfilesForMatchingResponse> GetApplicantProfilesForMatching(
			GetApplicantProfilesForMatchingRequest request,
			ICombinedContext context)
		{
			Dictionary<Guid, ApplicantMatchingProfile> applicantProfilesById
				= await applicantDatabase.GetApplicantProfilesByIdAsync();

			GetApplicantProfilesForMatchingResponse response = new()
			{
				ApplicantProfilesById = applicantProfilesById
			};

			return response;
		}

		[ToolMethod(Description
			= """
			  Record the applicants that match a newly registered property listing.
			  Use this after registering a listing, retrieving applicant profiles,
			  and deciding which applicants are likely matches for that listing.
			  The response includes real estate agent notification targets.
			  After this method succeeds, call RealEstateAgentMessenger.SendNotification
			  once for each notification target.
			  Do not use the built-in Notifier for real estate agent notifications.
			  """)]
		public async Task<RecordListingMatchesResponse> RecordListingMatches(
			RecordListingMatchesRequest request,
			ICombinedContext context)
		{
			RecordListingMatchesResult result =
				await applicantDatabase.RecordListingMatchesAsync(
					request.ListingId,
					request.MatchedApplicants);

			RecordListingMatchesResponse response = new()
			{
				ListingId              = result.ListingId,
				RecordedApplicantCount = result.MatchedApplicants.Count,
				NotificationTargets    = result.NotificationTargets.ToList(),
				UnknownApplicantIds    = result.UnknownApplicantIds.ToArray()
			};

			return response;
		}
	}

	public class RegisterApplicantRequest
	{
		[ToolProperty(
			Description = "The first name of the person looking to buy a property.",
			Required = true)]
		public required string FirstName { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The surname of the person looking to buy a property.")]
		public required string Surname { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  The features of the property that the prospective buyer is seeking.
				  Include, for example, the number of bedrooms,
				  if the user specifies that detail, and any other 'must haves'
				  or 'must not haves'.
				  For example: 'Looking for a 4 bedroom house near Lausanne with a garden.'
				  """)]
		public required string IdealPropertyFeatures { get; set; }
	}

	public class RegisterApplicantResponse
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  If true, the applicant was successfully registered.
				  If false, the applicant is now registered, but was already registered.
				  """)]
		public bool Added { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The unique identifier for the applicant.")]
		public Guid ApplicantId { get; set; }
	}

	public class ApplicantMatchingProfile
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  The identifier of the real estate agent who registered this applicant.
				  Use this with RealEstateAgentMessenger if this applicant matches a listing.
				  """)]
		public Guid RealEstateAgentId { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  A privacy-preserving applicant label.
				  It contains the first letter of the applicant's first name and the applicant's surname.
				  For example: J Smith.
				  """)]
		public required string ApplicantLabel { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The features of the property that the applicant is seeking.")]
		public required string IdealPropertyFeatures { get; set; }
	}

	public class GetApplicantProfilesForMatchingRequest
	{
	}

	public class GetApplicantProfilesForMatchingResponse
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  Applicant profiles keyed by applicant identifier.
				  Email addresses are intentionally excluded.
				  Use the applicant identifier when recording listing matches.
				  Use the real estate agent identifier when notifying the agent through RealEstateAgentMessenger.
				  """)]
		public required Dictionary<Guid, ApplicantMatchingProfile> ApplicantProfilesById { get; set; }
	}

	public class RecordListingMatchesRequest
	{
		[ToolProperty(
			Required = true,
			Description = "The unique identifier of the property listing.")]
		public Guid ListingId { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  The applicants that appear to match the property listing.
				  Include one item for each matching applicant.
				  """)]
		public required List<MatchedApplicant> MatchedApplicants { get; set; }
	}

	public class MatchedApplicant
	{
		[ToolProperty(
			Required = true,
			Description = "The unique identifier of the matching applicant.")]
		public Guid ApplicantId { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  A short explanation of why this applicant matches
				  the property listing. Mention the applicant's relevant
				  requirements and the listing features.
				  """)]
		public required string MatchReason { get; set; }
	}

	public class RealEstateAgentNotificationTarget
	{
		[ToolProperty(
			Required = true,
			Description = "The real estate agent identifier to notify.")]
		public Guid RealEstateAgentId { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  The privacy-preserving applicant labels for matched applicants registered by this agent.
				  For example: J Smith, A Brown.
				  """)]
		public required List<string> ApplicantLabels { get; set; }
	}

	public class RecordListingMatchesResponse
	{
		[ToolProperty(
			Required = true,
			Description = "The unique identifier of the property listing.")]
		public Guid ListingId { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The number of applicant matches recorded for the listing.")]
		public int RecordedApplicantCount { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  The real estate agents who should be notified, grouped with the applicants they registered.
				  After receiving this response, call RealEstateAgentMessenger.SendNotification for each item.
				  Do not use the built-in Notifier for these notifications.
				  """)]
		public required List<RealEstateAgentNotificationTarget> NotificationTargets { get; set; }

		[ToolProperty(
			Required = true,
			Description
				= """
				  Applicant identifiers that were supplied but not found in the applicant database.
				  This should normally be empty.
				  """)]
		public required Guid[] UnknownApplicantIds { get; set; }
	}
}