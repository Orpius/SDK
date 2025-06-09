/* Auto-generated from the content of ContractMessage.tt */

using System;

using ProtoBuf;

namespace Orpius.Platform.Tooling.RpcToolsRegistrationService
{
	[ProtoContract]
	public partial class ContractMessage
	{
		DiscriminatedUnionObject payload;

		public void Clear() => payload = default;

		[ProtoMember(1)]
		public ComplexContractMessage? ComplexContract
		{
			get => payload.Is(1) ? (ComplexContractMessage?)payload.Object : null;
			set => payload = new DiscriminatedUnionObject(1, value);
		}

		public bool ShouldSerializeComplexContract() => payload.Is(1);
		public void ResetComplexContract() => DiscriminatedUnionObject.Reset(ref payload, 1);
		[ProtoMember(2)]
		public SimpleContractMessage? SimpleContract
		{
			get => payload.Is(2) ? (SimpleContractMessage?)payload.Object : null;
			set => payload = new DiscriminatedUnionObject(2, value);
		}

		public bool ShouldSerializeSimpleContract() => payload.Is(2);
		public void ResetSimpleContract() => DiscriminatedUnionObject.Reset(ref payload, 2);
		[ProtoMember(3)]
		public EnumContractMessage? EnumContract
		{
			get => payload.Is(3) ? (EnumContractMessage?)payload.Object : null;
			set => payload = new DiscriminatedUnionObject(3, value);
		}

		public bool ShouldSerializeEnumContract() => payload.Is(3);
		public void ResetEnumContract() => DiscriminatedUnionObject.Reset(ref payload, 3);
		[ProtoMember(4)]
		public ListContractMessage? ListContract
		{
			get => payload.Is(4) ? (ListContractMessage?)payload.Object : null;
			set => payload = new DiscriminatedUnionObject(4, value);
		}

		public bool ShouldSerializeListContract() => payload.Is(4);
		public void ResetListContract() => DiscriminatedUnionObject.Reset(ref payload, 4);

		/* Convenience members, ignored by the serializer. */

		[ProtoIgnore]
		public IContractMessage Contract
		{
			get => (IContractMessage)payload.Object;
			set
			{
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				if (value == null)
				{
					payload = default;
					return;
				}

				switch (value)
				{
					case ComplexContractMessage tempComplexContractMessage:
						ComplexContract = tempComplexContractMessage;
						break;
					case SimpleContractMessage tempSimpleContractMessage:
						SimpleContract = tempSimpleContractMessage;
						break;
					case EnumContractMessage tempEnumContractMessage:
						EnumContract = tempEnumContractMessage;
						break;
					case ListContractMessage tempListContractMessage:
						ListContract = tempListContractMessage;
						break;
					default:
					{
						throw new ArgumentException(
							$"Unsupported IContractMessage type {value.GetType()}", nameof(value));
					}
				}
			}
		}

		public OneOf OneOfKind => (OneOf)payload.Discriminator;

		public enum OneOf
		{
			None = 0, 
			ComplexContract = 1, 
			SimpleContract = 2, 
			EnumContract = 3, 
			ListContract = 4, 
		}
	}
}