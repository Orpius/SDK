using Microsoft.CodeAnalysis;

namespace Orpius.Platform.Generators
{
	static class ContractTypeNameProvider
	{
		static readonly SymbolDisplayFormat contractTypeNameFormat = new(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
			memberOptions: SymbolDisplayMemberOptions.None,
			parameterOptions: SymbolDisplayParameterOptions.None,
			propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
			localOptions: SymbolDisplayLocalOptions.None,
			kindOptions: SymbolDisplayKindOptions.None,
			miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

		public static string GetContractTypeName(ITypeSymbol type)
		{
			if (ContractTypeInspector.TryGetNullableUnderlyingType(
					type,
					out ITypeSymbol underlyingType))
			{
				return GetContractTypeName(underlyingType);
			}

			if (ContractTypeInspector.TryGetDictionaryTypes(
					type,
					out ITypeSymbol keyType,
					out ITypeSymbol valueType))
			{
				return $"System.Collections.Generic.Dictionary<{GetContractTypeName(keyType)}, {GetContractTypeName(valueType)}>";
			}

			if (ContractTypeInspector.TryGetCollectionElementType(
					type,
					out ITypeSymbol elementType))
			{
				return $"{GetContractTypeName(elementType)}[]";
			}

			return type.ToDisplayString(contractTypeNameFormat);
		}
	}
}