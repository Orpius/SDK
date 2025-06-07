using System;

namespace Orpius.Platform.ToolsModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public sealed class ToolAttribute : Attribute
	{
		/// <summary>
		/// If specified, this value overrides the default value,
		/// which is the class or interface name.
		/// </summary>
		public string? Name    { get; set; }
	}	
	
	//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	//public sealed class ToolContractAttribute : Attribute
	//{
	//	///// <summary>
	//	///// If specified, this value overrides the default value,
	//	///// which is the class or interface name.
	//	///// </summary>
	//	//public string? Name    { get; set; }
	//}

	[AttributeUsage(AttributeTargets.Property)]
	public class ToolPropertyAttribute : Attribute
	{
		/// <summary>
		/// If specified, this value overrides the default value,
		/// which is the method name.
		/// </summary>
		public string? Name { get;        set; }
		public bool    Required    { get; set; } = false;

		/// <summary>
		/// Provide a description to assist the agent
		/// in understanding the purpose of this property
		/// and what to populate it with.
		/// </summary>
		public string? Description { get; set; }

		public string? Format      { get; set; }
		public Type?   RepresentAs { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ToolStringPropertyAttribute : ToolPropertyAttribute
	{
		public string? Pattern { get; set; }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ToolMethodAttribute : Attribute
	{
		public string? Name { get; set; }

		public string? Description { get; set; }
	}
}
