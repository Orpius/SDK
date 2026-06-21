// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;

using Orpius.Platform.Inferencing;
using Orpius.Platform.RpcServiceModel;

using ProtoBuf;

namespace Orpius.Platform.OperationsModel.RpcOperationsService
{
	partial class ChatRequest
	{
#if !NET7_0_OR_GREATER
		/// <param name="operationExternalId">
		/// The Orpius client application provides a unique identifier
		/// for the operation on the Operation pane.</param>
		/// <param name="userMessage">
		/// The message sent by the user of your application.</param>
		public ChatRequest(Guid operationExternalId, UserMessage userMessage)
		{
			OperationExternalId = AssertArg.IsNotEmpty(operationExternalId, nameof(operationExternalId));
			UserMessage         = AssertArg.IsNotNull(userMessage, nameof(userMessage));
		}
#endif
		/// <summary>
		/// The external ID of the Operation.
		/// </summary>
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid OperationExternalId { get; set; }
#else
		public Guid OperationExternalId { get; set; }
#endif

		/// <summary>
		/// This must be null if starting a new conversation.
		/// The ConversationId is returned
		/// in the response <see cref="ChatResponse.ConversationId"/>.
		/// </summary>
		[ProtoMember(2, IsRequired = false)]
#if NET7_0_OR_GREATER
		public required Guid? ConversationId { get; set; }
#else
		public Guid? ConversationId { get; set; }
#endif

		[ProtoMember(3, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required UserMessage UserMessage { get; set; }
#else
		public UserMessage UserMessage { get; set; }
#endif

		[ProtoMember(4, IsRequired = true)]
		public Guid? RequestId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// The raw JSON that is included in the conversation,
		/// which allows the Agent to use Tools.
		/// For example, if you have a ToolMember named `ScheduleViewing`,
		/// and your request class requires the customer's ID, you would include that in this JSON.
		/// </summary>
		[ProtoMember(5, IsRequired = false)]
#if NET7_0_OR_GREATER
		public required string? JsonProvidedToAgent { get; set; }
#else
		public string? JsonProvidedToAgent { get; set; }
#endif

		/// <summary>
		/// Use this dictionary to provide contextual information to ToolMembers.
		/// The values in this dictionary are not provided to the agent.
		/// </summary>
		[ProtoMember(6, IsRequired = false)]
#if NET7_0_OR_GREATER
		public required Dictionary<string, string> Context { get; set; } = new();
#else
		public Dictionary<string, string> Context { get; set; }
			= new Dictionary<string, string>();
#endif

		/// <summary>
		/// The list of tools available for this operation.
		/// </summary>
		[ProtoMember(7, IsRequired = false)]
		public IList<Tool>? Tools { get; set; }
	}

	[ProtoContract]
	public class Tool
	{
#if !NET7_0_OR_GREATER
		public Tool(string name)
		{
			Name = AssertArg.IsNotNullOrWhiteSpace(name, nameof(name));
		}
#endif
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string Name { get; set; }
#else
		public string Name { get; set; }
#endif

		/// <summary>
		/// If the specified tool is required and cannot be found
		/// via a registered tool provider, an RpcException is thrown.
		/// </summary>
		[ProtoMember(2, IsRequired = true)]
		public ToolPresence ToolPresence { get; set; } = ToolPresence.Required;
	}

	[ProtoContract]
	public enum ToolPresence
	{
		/// <summary>
		/// If the tool is not found,
		/// then the Operation call will not proceed.
		/// </summary>
		Required = 0,
		/// <summary>
		/// The tool does not need to be found
		/// for the Operation to proceed.
		/// </summary>
		NotRequired = 1
	}

	partial class ChatResponse
	{
		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid ConversationId { get; set; }
#else
		public Guid ConversationId { get; set; }
#endif

		[ProtoMember(2, IsRequired = true)]
		public DateTime CreationTimeUtc { get; set; } = DateTime.UtcNow;

		[ProtoMember(3, IsRequired = false)]
		public AssistantMessage? AssistantMessage { get; set; }

		[ProtoMember(4, IsRequired = false)]
		public SystemMessage? SystemMessage { get; set; }
	}
}
