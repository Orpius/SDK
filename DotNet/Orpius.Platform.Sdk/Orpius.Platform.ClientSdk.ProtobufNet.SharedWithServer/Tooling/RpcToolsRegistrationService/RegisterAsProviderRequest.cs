// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;

using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServiceModel;

using ProtoBuf;

namespace Orpius.Platform.Tooling.RpcToolsRegistrationService
{
	public interface IToolsetExternalIdProvider
	{
		/// <summary>
		/// The external ID of the toolset.
		/// </summary>
		Guid ToolsetExternalId { get; }
	}

	partial class RegisterAsProviderRequest : IToolsetExternalIdProvider
	{
#if !NET7_0_OR_GREATER
		public RegisterAsProviderRequest(Guid toolsetExternalId,
										 string providerUrl, 
										 IList<ToolMessage> tools,
										 IList<ContractMessage> contracts,
										 ProgrammingLanguageId programmingLanguageId)
		{
			ToolsetExternalId = AssertArg.IsNotEmpty(toolsetExternalId, nameof(toolsetExternalId));

			if (!Uri.TryCreate(providerUrl, UriKind.Absolute, out _))
			{
				throw new ArgumentException("Invalid URL." + providerUrl, nameof(providerUrl));
			}
			
			ProviderUrl           = providerUrl;
			ProgrammingLanguageId = programmingLanguageId;

			AssertArg.IsNotNullOrEmpty(tools, nameof(tools));
			Tools = tools;

			AssertArg.IsNotNullOrEmpty(contracts, nameof(contracts));
			Contracts = contracts;
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid ToolsetExternalId { get; set; }
#else
		public Guid ToolsetExternalId { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ProviderUrl { get; set; }
#else
		public string ProviderUrl { get; set; }
#endif

		[ProtoMember(3, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required IList<ToolMessage> Tools { get; set; } = new List<ToolMessage>();
#else
		public IList<ToolMessage> Tools { get; set; } = new List<ToolMessage>();
#endif

		[ProtoMember(4, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required IList<ContractMessage> Contracts { get; set; } = new List<ContractMessage>();
#else
		public IList<ContractMessage> Contracts { get; set; } = new List<ContractMessage>();
#endif

		[ProtoMember(5, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required ProgrammingLanguageId ProgrammingLanguageId { get; set; }
#else
		public ProgrammingLanguageId ProgrammingLanguageId { get; set; }
#endif

		/// <summary>
		/// Headers are sent to the provider when requesting
		/// the use of a tool. They are stored as encrypted items
		/// and may contain authentication information.
		/// </summary>
		[ProtoMember(6, IsRequired = false)]
#if NET7_0_OR_GREATER
		public IList<HeaderMessage> Headers { get; set; } = new List<HeaderMessage>();
#else
		public IList<HeaderMessage> Headers { get; set; } = new List<HeaderMessage>();
#endif
	}

	[ProtoContract]
	public class HeaderMessage
	{
#if !NET7_0_OR_GREATER
		public HeaderMessage(string key, string value)
		{
			Key = AssertArg.IsNotNullOrWhiteSpace(key, nameof(key));
			Value = AssertArg.IsNotNullOrWhiteSpace(value, nameof(value));
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string Key { get; set; }
#else
		public string Key { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string Value { get; set; }
#else
		public string Value { get; set; }
#endif
	}

	public enum ProgrammingLanguageId
	{
		CSharp = 1,
		Java = 2
	}

	[ProtoContract]
	public class ToolMessage
	{
#if !NET7_0_OR_GREATER
		public ToolMessage(string toolName, string typeName)
		{
			ToolName = AssertArg.IsNotNullOrWhiteSpace(toolName, nameof(toolName));
			TypeName = AssertArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ToolName { get; set; }
#else
		public string ToolName { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif

		//		[ProtoMember(2, IsRequired = true)]
		//#if NET7_0_OR_GREATER
		//		public required int Version { get; set; }
		//#else
		//		public int Version { get; set; }
		//#endif

		[ProtoMember(3, IsRequired = false)]
#if NET7_0_OR_GREATER
		public required string? Description { get; set; }
#else
		public string? Description { get; set; }
#endif

		[ProtoMember(4, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required IList<ToolMethodMessage> Methods { get; set; } = new List<ToolMethodMessage>();
#else
		public IList<ToolMethodMessage> Methods { get; set; } = new List<ToolMethodMessage>();
#endif
	}

//	[ProtoContract]
//	public class ToolContractMessage
//	{
//#if !NET7_0_OR_GREATER
//		public ToolContractMessage(string typeName)
//		{
//			TypeName = AssertArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
//		}
//#endif
//		[ProtoMember(1, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required string TypeName { get; set; }
//#else
//		public string TypeName { get; set; }
//#endif

//		[ProtoMember(2, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required IList<ContractPropertyMessage> Properties { get; set; } = new List<ContractPropertyMessage>();
//#else
//		public List<ContractPropertyMessage> Properties { get; set; } = new List<ContractPropertyMessage>();
//#endif
//	}

	[ProtoContract]
	public class ComplexContractMessage : IContractMessage
	{
#if !NET7_0_OR_GREATER
		public ComplexContractMessage(string typeName)
		{
			TypeName = AssertArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required IList<ContractPropertyMessage> Properties { get; set; } = new List<ContractPropertyMessage>();
#else
		public List<ContractPropertyMessage> Properties { get; set; } = new List<ContractPropertyMessage>();
#endif
	}

	[ProtoContract]
	public class SimpleContractMessage : IContractMessage
	{
#if !NET7_0_OR_GREATER
		public SimpleContractMessage(string typeName)
		{
			TypeName = AssertArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif
	}

	[ProtoContract]
	public class ListContractMessage : IContractMessage
	{
#if !NET7_0_OR_GREATER
		public ListContractMessage(string itemTypeName)
		{
			TypeName = AssertArg.IsNotNullOrWhiteSpace(itemTypeName, nameof(itemTypeName));
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif
	}

	[ProtoContract]
	public class EnumContractMessage : IContractMessage
	{
#if !NET7_0_OR_GREATER
		public EnumContractMessage(string typeName, Dictionary<string, int> enumValues)
		{
			TypeName   = AssertArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
			AssertArg.IsNotNullOrEmpty(enumValues, nameof(enumValues));
			EnumValues = enumValues;
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Dictionary<string, int> EnumValues { get; set; } = new();
#else
		public Dictionary<string, int> EnumValues { get; set; }
#endif

	}

	[ProtoContract]
	public class ContractPropertyMessage
	{
#if !NET7_0_OR_GREATER
		public ContractPropertyMessage(string propertyName, string typeName)
		{
			PropertyName = AssertArg.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));
			TypeName     = AssertArg.IsNotNullOrWhiteSpace(typeName,     nameof(typeName));
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string PropertyName { get; set; }
#else
		public string PropertyName { get; set; }
#endif

		[ProtoMember(2, IsRequired = false)]
		public string? Description { get; set; }

		[ProtoMember(3, IsRequired = false)]
		public bool Required { get; set; }

		[ProtoMember(4, IsRequired = false)]
		public string? OpenApiFormat { get; set; }

		/// <summary>
		/// Full type name of how this is to be represented.
		/// This property allows you to represent,
		/// for example, a <c>HashSet&lt;AnEnum&gt;</c> as an <c>int[]</c>,
		/// or a <c>DateTime</c> as a <c>string</c> with a custom format.
		/// </summary>
		[ProtoMember(5, IsRequired = false)]
		public string? RepresentAs { get; set; }

		[ProtoMember(6, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string TypeName { get; set; }
#else
		public string TypeName { get; set; }
#endif

		[ProtoMember(7, IsRequired = false)]
		public string? Pattern { get; set; }
	}

	[ProtoContract]
	public class ToolMethodMessage
	{
#if !NET7_0_OR_GREATER
		public ToolMethodMessage(string methodName,
								 string parameterContractTypeName,
								 string returnsContractTypeName,
								 string description)
		{
			MethodName = AssertArg.IsNotNullOrWhiteSpace(methodName, nameof(methodName));
			ParameterContractTypeName = AssertArg.IsNotNullOrWhiteSpace(
				parameterContractTypeName, 
				nameof(parameterContractTypeName));
			ReturnsContractTypeName = AssertArg.IsNotNullOrWhiteSpace(
				returnsContractTypeName, 
				nameof(returnsContractTypeName));
			Description = AssertArg.IsNotNullOrWhiteSpace(description, nameof(description));
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string MethodName { get; set; }
#else
		public string MethodName { get; set; }
#endif

		[ProtoMember(2, IsRequired = false)]
		public string? Description { get; set; }

		[ProtoMember(3, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ParameterContractTypeName { get; set; }
#else
		public string ParameterContractTypeName { get; set; }
#endif

		[ProtoMember(4, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ReturnsContractTypeName { get; set; }
#else
		public string ReturnsContractTypeName { get; set; }
#endif
	}
}
