namespace Orpius.Samples.RealEstate
{
	public interface IRealEstateAgentIdentityService
	{
		Guid GetCurrentRealEstateAgentId();
	}

	public class DemoIdentityService : IRealEstateAgentIdentityService
	{
		static readonly Guid demoRealEstateAgentId = new(
			"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

		public Guid GetCurrentRealEstateAgentId()
		{
			/* In a real application, this would come from the signed-in user. */
			return demoRealEstateAgentId;
		}
	}
}