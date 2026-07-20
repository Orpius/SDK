// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;

using ProtoBuf;

namespace Orpius.Platform.Tooling.RpcToolProviderService
{
	partial class UseToolRequest
	{
#if !NET7_0_OR_GREATER
		public UseToolRequest(Dictionary<string, string> context,
							  string toolName,
							  string toolMember,
							  string parameterAsJson,
							  Guid apiCallId)
		{
			Context         = AssertArg.IsNotNull(context, nameof(context));
			ToolName        = AssertArg.IsNotNullOrWhiteSpace(toolName,   nameof(toolName));
			ToolMember      = AssertArg.IsNotNullOrWhiteSpace(toolMember, nameof(toolMember));
			ParameterAsJson = parameterAsJson;
			ApiCallPublicId = apiCallId;
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
		public required string ParameterAsJson { get; set; }
#else
		public string ParameterAsJson { get; set; }
#endif

		[ProtoMember(5, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required Guid ApiCallPublicId { get; set; }
#else
		public Guid ApiCallPublicId { get; set; }
#endif
	}

	partial class UseToolResponse
	{
#if !NET7_0_OR_GREATER
		public UseToolResponse(string resultAsJson)
		{
			ResultAsJson = AssertArg.IsNotNullOrWhiteSpace(resultAsJson, nameof(resultAsJson));
		}
#endif
		//[ProtoMember(1, IsRequired = true)]
		//public ToolResult? ToolResult { get; set; }

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ResultAsJson { get; set; }
#else
		public string ResultAsJson { get; set; }
#endif

		/// <summary>
		/// Setting this property will see the replacement of the context dictionary
		/// in future calls. This allows you to share state across tools.
		/// </summary>
		[ProtoMember(2, IsRequired = false)]
		public Dictionary<string, string>? ReplacementContext { get; set; }
	}
}
