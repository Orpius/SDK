using System;
using System.Collections.Generic;

using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	/// <summary>
	/// Represents the set of parameters your SDK needs to register and unregister a tool
	/// with the remote service.
	/// </summary>
	public interface IToolRegistrationParameters
	{
		/// <summary>
		/// Retrieves the <c>Guid</c> to authenticate
		/// with the remote service.
		/// </summary>
		Guid ApiKey { get; }

		/// <summary>
		/// Gets the local URL where your tool is listening (for callbacks).
		/// </summary>
		Uri LocalUrl { get; }

		/// <summary>
		/// Gets the toolset ID available in the Orpius client on the custom tool pane.
		/// </summary>
		Guid ToolsetExternalId { get; }

		IList<HeaderMessage> CallBackHeaders { get; }
	}
}