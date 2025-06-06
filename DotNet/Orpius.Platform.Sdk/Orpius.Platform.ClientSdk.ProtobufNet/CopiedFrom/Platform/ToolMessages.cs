//using ProtoBuf;

//using System;
//using System.Collections.Generic;

//namespace Orpius.Platform.Inferencing.ToolsModel
//{
//	[ProtoContract]
//	public class ToolResult
//	{
//		[ProtoMember(1, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required ToolOutcome Outcome { get; set; }
//#else
//		public ToolOutcome Outcome { get; set; }
//#endif

//		[ProtoMember(2, IsRequired = false)]
//		public string? PayloadJson { get; set; }

//		[ProtoMember(3, IsRequired = false)]
//		public IEnumerable<ToolError>? Errors { get; set; }

//		[ProtoMember(4, IsRequired = false)]
//		public IEnumerable<ToolWarning>? Warnings { get; set; }
//	}

//	[ProtoContract]
//	public enum ToolOutcome
//	{
//		Success = 0,
//		SuccessWithWarnings = 1,
//		Fail = 2
//	}

//	static class ToolResultValueExtensions
//	{
//		internal static bool IsSuccess(this ToolOutcome value)
//		{
//			return value    == ToolOutcome.Success
//				   || value == ToolOutcome.SuccessWithWarnings;
//		}
//	}

//	[ProtoContract]
//	public class ToolError
//	{
//		[ProtoMember(1, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required string Text { get; set; }
//#else
//		public string Text { get; set; }
//#endif

//		/// <summary>
//		/// Either the type short name of the exception,
//		/// or something that will enable the assistant
//		/// to differentiate between different errors.
//		/// </summary>
//		[ProtoMember(2, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required string ErrorType { get; set; }
//#else
//		public string ErrorType { get; set; }
//#endif

//		/// <summary>
//		/// An optional identifier to correlate the error across calls.
//		/// </summary>
//		[ProtoMember(3, IsRequired = false)]
//		public Guid ErrorId { get; set; }
//	}

//	[ProtoContract]
//	public class ToolWarning
//	{
//		[ProtoMember(1, IsRequired = true)]
//#if NET7_0_OR_GREATER
//		public required string Text { get; set; }
//#else
//		public string Text { get; set; }
//#endif

//		/// <summary>
//		/// An optional identifier to correlate the warning across calls.
//		/// </summary>
//		[ProtoMember(2, IsRequired = false)]
//		public Guid WarningId { get; set; }
//	}
//}