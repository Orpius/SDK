//using System.Diagnostics.CodeAnalysis;

//using Orpius.Platform.Tooling.ToolRegistration;

//using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

//namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
//{
//	class ToolRegistryForDevelopment
//	{
//		class LocalResolver : IToolResolver
//		{
//			public bool TryGetTool<T>([NotNullWhen(true)] out T? tool)
//				where T : class
//			{
//				if (typeof(T) == typeof(FlightStatusChecker))
//				{
//					tool = (T)(object)new FlightStatusChecker();
//					return true;
//				}

//				tool = default!;
//				return false;
//			}
//		}

//		public ToolRegistry GetRegistry()
//		{
//			var result = new ToolRegistry(new LocalResolver());
//			result.RegisterToolInvoker(new GeneratedToolInvoker());
//			return result;
//		}
//	}
//}
