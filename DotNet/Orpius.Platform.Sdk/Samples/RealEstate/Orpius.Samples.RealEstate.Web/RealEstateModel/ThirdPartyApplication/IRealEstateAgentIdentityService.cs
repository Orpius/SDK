namespace Orpius.Samples.RealEstate
{
	public interface IRealEstateAgentIdentityService
	{
		Guid GetCurrentRealEstateAgentId();
	}

	public static class DemoRealEstateAgents
	{
		public static readonly Guid CurrentAgentId = new(
			"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

		public static readonly Guid LausanneAgentId = new(
			"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

		public static readonly Guid RivieraAgentId = new(
			"cccccccc-cccc-cccc-cccc-cccccccccccc");
	}

	public class DemoIdentityService : IRealEstateAgentIdentityService
	{
		static readonly Guid demoRealEstateAgentId
			= DemoRealEstateAgents.CurrentAgentId;

		public Guid GetCurrentRealEstateAgentId()
		{
			/* In a real application, this would come from the signed-in user. */
			return demoRealEstateAgentId;
		}
	}
}