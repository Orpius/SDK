using System.Text.Json;

namespace Orpius.Samples.RealEstate
{
	public class ApplicantDatabase
	{
		readonly object syncRoot = new();

		readonly Dictionary<string, ApplicantRecord> applicantsByEmail
			= new(StringComparer.OrdinalIgnoreCase);

		readonly Dictionary<Guid, ApplicantRecord> applicantsById = new();

		readonly Dictionary<Guid, ListingApplicantMatchRecord> matchesByListingId = new();

		public async Task<RegisterApplicantResult> RegisterAsync(
			RegisterApplicantRequest request,
			string emailAddress,
			Guid realEstateAgentId)
		{
			ArgumentNullException.ThrowIfNull(request);

			string normalizedEmailAddress = NormalizeEmailAddress(emailAddress);

			lock (syncRoot)
			{
				if (applicantsByEmail.TryGetValue(
						normalizedEmailAddress,
						out ApplicantRecord? existingApplicant))
				{
					ApplicantRecord updatedApplicant = new(
						existingApplicant.Id,
						realEstateAgentId,
						request.FirstName,
						request.Surname,
						normalizedEmailAddress,
						request.IdealPropertyFeatures);

					applicantsByEmail[normalizedEmailAddress] = updatedApplicant;
					applicantsById[updatedApplicant.Id]       = updatedApplicant;

					return new RegisterApplicantResult(updatedApplicant, false);
				}

				ApplicantRecord applicant = new(
					Guid.NewGuid(),
					realEstateAgentId,
					request.FirstName,
					request.Surname,
					normalizedEmailAddress,
					request.IdealPropertyFeatures);

				applicantsByEmail.Add(normalizedEmailAddress, applicant);
				applicantsById.Add(applicant.Id, applicant);

				return new RegisterApplicantResult(applicant, true);
			}
		}

		public bool TryGetById(Guid applicantId, out ApplicantRecord? applicant)
		{
			lock (syncRoot)
			{
				return applicantsById.TryGetValue(applicantId, out applicant);
			}
		}

		public bool TryGetByEmailAddress(
			string emailAddress,
			out ApplicantRecord? applicant)
		{
			string normalizedEmailAddress = NormalizeEmailAddress(emailAddress);

			lock (syncRoot)
			{
				return applicantsByEmail.TryGetValue(
					normalizedEmailAddress,
					out applicant);
			}
		}

		public IReadOnlyList<ApplicantRecord> GetAll()
		{
			lock (syncRoot)
			{
				return applicantsById
					   .Values
					   .OrderBy(applicant => applicant.Surname)
					   .ThenBy(applicant => applicant.FirstName)
					   .ToArray();
			}
		}

		public IReadOnlyList<ApplicantMatchCandidate> GetMatchCandidates()
		{
			lock (syncRoot)
			{
				return applicantsById
					   .Values
					   .OrderBy(applicant => applicant.Surname)
					   .ThenBy(applicant => applicant.FirstName)
					   .Select(applicant => new ApplicantMatchCandidate
					   {
						   ApplicantId = applicant.Id,
						   //FirstName = applicant.FirstName,
						   //Surname = applicant.Surname,
						   IdealPropertyFeatures = applicant.IdealPropertyFeatures
					   })
					   .ToArray();
			}
		}

		public string GetMatchCandidatesJson()
		{
			IReadOnlyList<ApplicantMatchCandidate> candidates = GetMatchCandidates();

			JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
			{
				WriteIndented = true
			};

			return JsonSerializer.Serialize(candidates, options);
		}

		static string NormalizeEmailAddress(string emailAddress)
		{
			if (string.IsNullOrWhiteSpace(emailAddress))
			{
				throw new ArgumentException(
					"The email address must not be empty.",
					nameof(emailAddress));
			}

			return emailAddress.Trim();
		}

		public async Task<Dictionary<Guid, ApplicantMatchingProfile>> GetApplicantProfilesByIdAsync()
		{
			lock (syncRoot)
			{
				Dictionary<Guid, ApplicantMatchingProfile> applicantProfilesById =
					applicantsById.ToDictionary(
						pair => pair.Key,
						pair => new ApplicantMatchingProfile
						{
							RealEstateAgentId     = pair.Value.RealEstateAgentId,
							ApplicantLabel        = CreateApplicantLabel(pair.Value),
							IdealPropertyFeatures = pair.Value.IdealPropertyFeatures
						});

				return applicantProfilesById;
			}
		}

		static string CreateApplicantLabel(ApplicantRecord applicant)
		{
			string firstName = applicant.FirstName.Trim();
			string surname = applicant.Surname.Trim();

			if (firstName.Length == 0)
			{
				return surname;
			}

			char firstLetter = char.ToUpperInvariant(firstName[0]);

			return $"{firstLetter} {surname}";
		}

		public async Task<RecordListingMatchesResult> RecordListingMatchesAsync(
			Guid listingId,
			IEnumerable<MatchedApplicant> matchedApplicants)
		{
			ArgumentNullException.ThrowIfNull(matchedApplicants);

			List<MatchedApplicantRecord> validMatches = new();
			List<Guid> unknownApplicantIds = new();
			List<RealEstateAgentNotificationTarget> notificationTargets;

			lock (syncRoot)
			{
				foreach (MatchedApplicant matchedApplicant in matchedApplicants)
				{
					if (!applicantsById.TryGetValue(
							matchedApplicant.ApplicantId,
							out ApplicantRecord? applicant))
					{
						unknownApplicantIds.Add(matchedApplicant.ApplicantId);

						continue;
					}

					validMatches.Add(
						new MatchedApplicantRecord(
							matchedApplicant.ApplicantId,
							matchedApplicant.MatchReason));
				}

				ListingApplicantMatchRecord record = new(
					listingId,
					validMatches);

				matchesByListingId[listingId] = record;

				notificationTargets
					= validMatches
					  .Select(match => applicantsById[match.ApplicantId])
					  .GroupBy(applicant => applicant.RealEstateAgentId)
					  .Select(group => new RealEstateAgentNotificationTarget
					  {
						  RealEstateAgentId = group.Key,
						  ApplicantLabels = group
											.Select(CreateApplicantLabel)
											.OrderBy(label => label)
											.ToList()
					  })
					  .ToList();
			}

			return new RecordListingMatchesResult(
				listingId,
				validMatches,
				notificationTargets,
				unknownApplicantIds);
		}
	}

	public sealed record ApplicantRecord(
		Guid Id,
		Guid RealEstateAgentId,
		string FirstName,
		string Surname,
		string EmailAddress,
		string IdealPropertyFeatures);

	public sealed record RegisterApplicantResult(
		ApplicantRecord Applicant,
		bool Added);

	public class ApplicantMatchCandidate
	{
		public Guid ApplicantId { get; set; }

		public required string IdealPropertyFeatures { get; set; }
	}

	public sealed record ListingApplicantMatchRecord(
		Guid ListingId,
		IReadOnlyList<MatchedApplicantRecord> MatchedApplicants);

	public sealed record MatchedApplicantRecord(
		Guid ApplicantId,
		string MatchReason);

	public sealed record RecordListingMatchesResult(
		Guid ListingId,
		IReadOnlyList<MatchedApplicantRecord> MatchedApplicants,
		IReadOnlyList<RealEstateAgentNotificationTarget> NotificationTargets,
		IReadOnlyList<Guid> UnknownApplicantIds);
}