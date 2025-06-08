namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IToolRegistryItem
	{
		IToolInvoker   ToolInvoker   { get; }
		IToolsMetadata ToolsMetadata { get; }
	}
}
