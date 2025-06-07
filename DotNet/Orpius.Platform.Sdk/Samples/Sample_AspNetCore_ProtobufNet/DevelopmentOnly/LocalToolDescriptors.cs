using Orpius.Platform.Generators;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;
using Sample_AspNetCore_ProtobufNet.DevelopmentOnly;

namespace Orpius.Platform.Generators
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class GenerateToolDescriptorsAttribute : Attribute
	{
	}
}

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	[GenerateToolDescriptors]
	partial class LocalToolDescriptors //: IToolCaller
	{
		public LocalToolDescriptors(IToolRegistry toolRegistry)
		{
			GeneratedToolInvoker invoker = new();
			toolRegistry.RegisterToolInvoker(invoker);
		}

		public partial IList<ToolMessage>     Tools     { get; protected set; }

		public partial IList<ContractMessage> Contracts { get; protected set; }
	}
}
