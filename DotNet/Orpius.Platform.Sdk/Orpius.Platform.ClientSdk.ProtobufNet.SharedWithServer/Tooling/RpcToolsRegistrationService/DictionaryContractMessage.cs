using ProtoBuf;

namespace Orpius.Platform.Tooling.RpcToolsRegistrationService
{
	[ProtoContract]
	public class DictionaryContractMessage : IContractMessage
	{
#if !NET7_0_OR_GREATER
		public DictionaryContractMessage(
			string typeName,
			string keyContractTypeName,
			string valueContractTypeName)
		{
			TypeName = typeName;
			KeyContractTypeName = keyContractTypeName;
			ValueContractTypeName = valueContractTypeName;
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
		public required string KeyContractTypeName { get; set; }
#else
		public string KeyContractTypeName { get; set; }
#endif

		[ProtoMember(3, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ValueContractTypeName { get; set; }
#else
		public string ValueContractTypeName { get; set; }
#endif
	}
}