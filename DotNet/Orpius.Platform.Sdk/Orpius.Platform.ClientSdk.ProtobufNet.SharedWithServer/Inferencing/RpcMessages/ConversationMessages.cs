// ReSharper disable RedundantUsingDirective
using System;

using ProtoBuf;

namespace Orpius.Platform.Inferencing
{
	/// <summary>
	/// A client-side interface for messages in the conversation.
	/// </summary>
	public interface IChatMessage
	{
		string? Text { get; }

		Guid PublicId { get; }
	}

	[ProtoContract]
	public partial class UserMessage : IChatMessage
	{
		[ProtoMember(1)]
#if NET7_0_OR_GREATER
		public required string? Text { get; set; }
#else
		public string? Text { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid PublicId { get; set; }
#else
		public Guid PublicId { get; set; }
#endif
	}

	[ProtoContract]
	public partial class AssistantMessage : IChatMessage
	{
		[ProtoMember(1)]
#if NET7_0_OR_GREATER
		public required string? Text { get; set; }
#else
		public string? Text { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid PublicId { get; set; }
#else
		public Guid PublicId { get; set; }
#endif
	}

	[ProtoContract]
	public partial class SystemMessage : IChatMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string? Text { get; set; }

		[ProtoMember(2, IsRequired = true)]
		public SystemMessageType? MessageType { get; set; }

		[ProtoMember(3, IsRequired = false)]
		public ApiCallInfo? ApiCallInfo { get; set; }

		[ProtoMember(4, IsRequired = true)]
		public Guid PublicId { get; set; }
	}

	[ProtoContract]
	public partial class ApiCallInfo
	{
		[ProtoMember(1, IsRequired = true)]
		public string? PluginName { get; set; }

		[ProtoMember(2, IsRequired = true)]
		public string? ApiEndpoint { get; set; }

		[ProtoMember(3, IsRequired = false)]
		public bool? Success { get; set; }

		[ProtoMember(4, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid PublicId { get; set; }
#else
		public Guid PublicId { get; set; }
#endif
	}

	[ProtoContract]
	public enum SystemMessageType
	{
		ApiCallStart       = 0,
		ApiCallEnd         = 1,
		UserVisibleMessage = 2,
		UserVisibleError   = 3,
		ApiCallDeferred    = 4,
	}

	[ProtoContract]
	public class ChatReply
	{
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid ConversationId { get; set; }
#else
		public Guid ConversationId { get; set; }
#endif

		//		[ProtoMember(2, IsRequired = true)]
		//		public DateTime CreationTimeUtc { get; set; } = DateTime.UtcNow;

		[ProtoMember(3, IsRequired = false)]
		public AssistantMessage? AssistantMessage { get; set; }

		[ProtoMember(4, IsRequired = false)]
		public SystemMessage? SystemMessage { get; set; }
	}
}
