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

		/// <summary>
		/// Optional marker types that reside in assemblies you want to scan for
		/// <c>[Tool]</c> classes. If <see langword="null"/> or empty, the generator scans
		/// the current assembly. If specified, then only the specified assemblies
		/// are scanned for tools.
		/// </summary>
		/// <example>
		/// <code>
		/// [assembly: GenerateToolRegistryItem(
		///     "MyApp.ToolRegistration.AllTools",
		///     ScanAssembliesContaining = new[] {
		///         typeof(ExternalLib.FlightStatusChecker),
		///         typeof(SharedTools.WeatherForecasterMarker) })]
		/// </code>
		/// </example>
		public Type[]? ScanAssembliesContaining { get; set; }
	}
}
