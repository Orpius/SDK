// ReSharper disable RedundantUsingDirective
using System.Collections.Generic;

using Orpius.Platform.Inferencing.ToolsModel;

using ProtoBuf;

namespace Orpius.Platform.ToolsModel.RpcToolProviderService
{
	partial class UseToolRequest
	{
#if !NET7_0_OR_GREATER
		public UseToolRequest(Dictionary<string, string> context,
							  string toolName,
							  string toolMember,
							  string? requestBody)
		{
			Context     = AssertArg.IsNotNull(context, nameof(context));
			ToolName    = AssertArg.IsNotNullOrWhiteSpace(toolName,   nameof(toolName));
			ToolMember  = AssertArg.IsNotNullOrWhiteSpace(toolMember, nameof(toolMember));
			RequestBody = requestBody;
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Dictionary<string, string> Context { get; set; } = new();
#else
		public Dictionary<string, string> Context { get; set; } 
			= new Dictionary<string, string>();
#endif

		[ProtoMember(2, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ToolName { get; set; }
#else
		public string ToolName { get; set; }
#endif

		[ProtoMember(3, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ToolMember { get; set; }
#else
		public string ToolMember { get; set; }
#endif

		[ProtoMember(4, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string? RequestBody { get; set; }
#else
		public string? RequestBody { get; set; }
#endif
	}

	partial class UseToolResponse
	{
		[ProtoMember(1, IsRequired = true)]
		public ToolResult? ToolResult { get; set; }

		/// <summary>
		/// Setting this property will see the replacement of the context dictionary
		/// in future calls. This allows you to share state across tools.
		/// </summary>
		[ProtoMember(2, IsRequired = false)]
		public Dictionary<string, string>? ReplacementContext { get; set; }
	}
}
