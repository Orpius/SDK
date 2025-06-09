using System;

namespace Orpius.Platform.Tooling
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
