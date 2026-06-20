using System;

using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public partial class ToolRegistry
	{
		static string GetContractStorageTypeName(ContractMessage contractMessage)
		{
			if (contractMessage == null)
			{
				throw new ArgumentNullException(nameof(contractMessage));
			}

			switch (contractMessage.OneOfKind)
			{
				case ContractMessage.OneOf.SimpleContract:
				{
					SimpleContractMessage simpleContract
						= contractMessage.SimpleContract
						  ?? throw new InvalidOperationException(
							  "Expected SimpleContract to not be null.");

					return RequireContractTypeName(simpleContract.TypeName);
				}

				case ContractMessage.OneOf.EnumContract:
				{
					EnumContractMessage enumContract
						= contractMessage.EnumContract
						  ?? throw new InvalidOperationException(
							  "Expected EnumContract to not be null.");

					return RequireContractTypeName(enumContract.TypeName);
				}

				case ContractMessage.OneOf.ComplexContract:
				{
					ComplexContractMessage complexContract
						= contractMessage.ComplexContract
						  ?? throw new InvalidOperationException(
							  "Expected ComplexContract to not be null.");

					return RequireContractTypeName(complexContract.TypeName);
				}

				case ContractMessage.OneOf.ListContract:
				{
					ListContractMessage listContract
						= contractMessage.ListContract
						  ?? throw new InvalidOperationException(
							  "Expected ListContract to not be null.");

					return GetListStorageTypeName(listContract.TypeName);
				}

				case ContractMessage.OneOf.DictionaryContract:
				{
					DictionaryContractMessage dictionaryContract
						= contractMessage.DictionaryContract
						  ?? throw new InvalidOperationException(
							  "Expected DictionaryContract to not be null.");

					return RequireContractTypeName(dictionaryContract.TypeName);
				}

				default:
				{
					throw new InvalidOperationException(
						$"Unknown contract message kind '{contractMessage.OneOfKind}'.");
				}
			}
		}

		static string GetListStorageTypeName(string itemTypeName)
		{
			string safeItemTypeName = RequireContractTypeName(itemTypeName);

			return safeItemTypeName.EndsWith("[]", StringComparison.Ordinal)
					   ? safeItemTypeName
					   : safeItemTypeName + "[]";
		}

		static string RequireContractTypeName(string? typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				throw new InvalidOperationException(
					"Contract type name cannot be null or whitespace.");
			}

			return typeName;
		}
	}
}