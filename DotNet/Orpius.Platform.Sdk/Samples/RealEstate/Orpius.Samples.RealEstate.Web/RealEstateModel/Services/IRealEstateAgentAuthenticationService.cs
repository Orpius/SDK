namespace Orpius.Samples.RealEstate
{
	public interface IRealEstateAgentAuthenticationService
	{
		Guid GetCurrentRealEstateAgentId();
	}

	public class DemoRealEstateAgentAuthenticationService
		: IRealEstateAgentAuthenticationService
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