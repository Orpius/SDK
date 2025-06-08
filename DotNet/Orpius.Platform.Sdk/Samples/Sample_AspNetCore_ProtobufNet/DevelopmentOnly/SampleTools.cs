using Orpius.Platform.Generators;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.ToolRegistration;
using Sample_AspNetCore_ProtobufNet.DevelopmentOnly;

[assembly: GenerateToolRegistryItem("Sample_AspNetCore_ProtobufNet.ToolRegistration.SampleTools")]

namespace Orpius.Platform.Generators
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class GenerateToolRegistryItemAttribute : Attribute
	{
		public GenerateToolRegistryItemAttribute(string fullClassName)
		{
			FullClassName = fullClassName;
		}

		public string FullClassName { get; set; }
	}
}

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	//[GenerateToolDescriptors]
	partial class SampleTools : IToolRegistryItem
	{
		public SampleTools(IToolRegistry toolRegistry)
		{
			GeneratedToolInvoker invoker = new();
			toolRegistry.AddItem(this);
		}
		
		readonly ToolsMetadata messageBundle = GeneratedToolInfo.GetToolsAndContracts();

		#region Implementation of IToolRegistryItem

		public IToolInvoker   ToolInvoker   { get; } = new GeneratedToolInvoker();

		public IToolsMetadata ToolsMetadata => messageBundle;

		#endregion
	}
}
