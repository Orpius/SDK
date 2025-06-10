//using Orpius.Platform.Tooling.ToolRegistration;

//namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
//{
//	class RegistrationUtility
//	{
//		readonly IToolRegistry toolRegistry;

//		public RegistrationUtility(IToolRegistry toolRegistry)
//		{
//			this.toolRegistry    = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
//		}

//		internal async Task RegisterAsProviderAsync(CancellationToken token = default)
//		{
//			await toolRegistry.RegisterWithServerAsync(token);
//		}
//	}
//}
