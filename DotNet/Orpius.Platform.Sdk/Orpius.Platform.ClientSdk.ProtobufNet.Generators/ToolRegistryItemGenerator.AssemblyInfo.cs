using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Orpius.Platform.Generators
{
	partial class ToolRegistryItemGenerator
	{
		sealed partial record AssemblyInfo
		{
			public static readonly AssemblyInfo None
				= new(false, null, ImmutableHashSet<IAssemblySymbol>.Empty);
		}

		sealed partial record AssemblyInfo(
			bool Enabled,
			string? FullClassName,
			ImmutableHashSet<IAssemblySymbol> AllowedAssemblies)
		{
		}
	}
}