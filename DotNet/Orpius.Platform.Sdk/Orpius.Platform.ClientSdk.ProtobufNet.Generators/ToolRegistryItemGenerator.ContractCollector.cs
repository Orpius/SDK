using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Orpius.Platform.Generators
{
	public sealed partial class ToolRegistryItemGenerator
	{
		sealed class ContractCollector
		{
			readonly Dictionary<string, ContractModel> map = new(StringComparer.Ordinal);

			public void Add(ITypeSymbol t)
			{
				Visit(t);
			}

			public IReadOnlyList<ContractModel> All() => map.Values.ToList();

			void Visit(ITypeSymbol t)
			{
				string contractTypeName = ContractTypeNameProvider.GetContractTypeName(t);

				if (map.ContainsKey(contractTypeName))
				{
					return;
				}

				if (ContractTypeInspector.TryGetNullableUnderlyingType(
						t,
						out ITypeSymbol underlyingType))
				{
					Visit(underlyingType);

					return;
				}

				/* enum */
				if (t is INamedTypeSymbol en && en.TypeKind == TypeKind.Enum)
				{
					map[contractTypeName] = new EnumContractModel(en);

					return;
				}

				/* primitive and known simple types */
				if (ContractTypeInspector.IsSimpleContractType(t))
				{
					map[contractTypeName] = new SimpleContractModel(t);

					return;
				}

				/* dictionary */
				if (ContractTypeInspector.TryGetDictionaryTypes(
						t,
						out ITypeSymbol keyType,
						out ITypeSymbol valueType))
				{
					Visit(keyType);
					Visit(valueType);

					map[contractTypeName] = new DictionaryContractModel(
						t,
						keyType,
						valueType);

					return;
				}

				/* collection */
				if (ContractTypeInspector.TryGetCollectionElementType(
						t,
						out ITypeSymbol elementType))
				{
					Visit(elementType);

					map[contractTypeName] = new ListContractModel(t, elementType);

					return;
				}

				/* complex */
				if (t is INamedTypeSymbol complex)
				{
					List<IPropertySymbol> propertySymbols
						= complex.GetMembers()
								 .OfType<IPropertySymbol>()
								 .Where(symbol => symbol.GetAttributes()
														.Any(data => data.AttributeClass?.Name
																		 is "ToolPropertyAttribute"
																			or "ToolStringPropertyAttribute"))
								 .ToList();

					foreach (IPropertySymbol p in propertySymbols)
					{
						Visit(p.Type);
					}

					map[contractTypeName] = new ComplexContractModel(complex, propertySymbols);

					return;
				}

				/* fallback */
				map[contractTypeName] = new SimpleContractModel(t);
			}
		}
	}
}