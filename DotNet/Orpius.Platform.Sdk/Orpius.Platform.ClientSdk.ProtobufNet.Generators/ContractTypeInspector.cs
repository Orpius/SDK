using Microsoft.CodeAnalysis;

namespace Orpius.Platform.Generators
{
	static class ContractTypeInspector
	{
		static readonly SymbolDisplayFormat fullyQualifiedFormat
			= SymbolDisplayFormat.FullyQualifiedFormat;

		public static bool IsSimpleContractType(ITypeSymbol type)
		{
			if (type.SpecialType != SpecialType.None)
			{
				return true;
			}

			string typeName = type.ToDisplayString(fullyQualifiedFormat);

			return typeName is
				"global::System.Guid" or
				"global::System.DateTimeOffset" or
				"global::System.TimeSpan" or
				"global::System.Uri";
		}

		public static bool TryGetNullableUnderlyingType(
			ITypeSymbol type,
			out ITypeSymbol underlyingType)
		{
			if (type is INamedTypeSymbol namedType
				&& namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
				&& namedType.TypeArguments.Length == 1)
			{
				underlyingType = namedType.TypeArguments[0];

				return true;
			}

			underlyingType = null!;

			return false;
		}

		public static bool TryGetCollectionElementType(
			ITypeSymbol type,
			out ITypeSymbol elementType)
		{
			if (type.SpecialType == SpecialType.System_String)
			{
				elementType = null!;

				return false;
			}

			if (type is IArrayTypeSymbol arrayType)
			{
				elementType = arrayType.ElementType;

				return true;
			}

			if (type is not INamedTypeSymbol namedType)
			{
				elementType = null!;

				return false;
			}

			if (TryGetDictionaryTypes(type, out _, out _))
			{
				elementType = null!;

				return false;
			}

			if (namedType.IsGenericType
				&& IsKnownCollectionType(namedType.ConstructedFrom))
			{
				elementType = namedType.TypeArguments[0];

				return true;
			}

			foreach (INamedTypeSymbol interfaceType in namedType.AllInterfaces)
			{
				if (interfaceType.IsGenericType
					&& IsKnownCollectionType(interfaceType.ConstructedFrom))
				{
					elementType = interfaceType.TypeArguments[0];

					return true;
				}
			}

			elementType = null!;

			return false;
		}

		static bool IsKnownCollectionType(INamedTypeSymbol type)
		{
			string typeName = type.ToDisplayString(fullyQualifiedFormat);

			return typeName is
				"global::System.Collections.Generic.IEnumerable<T>" or
				"global::System.Collections.Generic.ICollection<T>" or
				"global::System.Collections.Generic.IReadOnlyCollection<T>" or
				"global::System.Collections.Generic.IList<T>" or
				"global::System.Collections.Generic.IReadOnlyList<T>" or
				"global::System.Collections.Generic.List<T>";
		}

		public static bool TryGetDictionaryTypes(
			ITypeSymbol type,
			out ITypeSymbol keyType,
			out ITypeSymbol valueType)
		{
			if (type is INamedTypeSymbol namedType)
			{
				if (namedType.IsGenericType
					&& IsKnownDictionaryType(namedType.ConstructedFrom))
				{
					keyType   = namedType.TypeArguments[0];
					valueType = namedType.TypeArguments[1];

					return true;
				}

				foreach (INamedTypeSymbol interfaceType in namedType.AllInterfaces)
				{
					if (interfaceType.IsGenericType
						&& IsKnownDictionaryType(interfaceType.ConstructedFrom))
					{
						keyType   = interfaceType.TypeArguments[0];
						valueType = interfaceType.TypeArguments[1];

						return true;
					}
				}
			}

			keyType   = null!;
			valueType = null!;

			return false;
		}

		static bool IsKnownDictionaryType(INamedTypeSymbol type)
		{
			string typeName = type.ToDisplayString(fullyQualifiedFormat);

			return typeName is
					   "global::System.Collections.Generic.IDictionary<TKey, TValue>" or
					   "global::System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>" or
					   "global::System.Collections.Generic.Dictionary<TKey, TValue>";
		}
	}
}