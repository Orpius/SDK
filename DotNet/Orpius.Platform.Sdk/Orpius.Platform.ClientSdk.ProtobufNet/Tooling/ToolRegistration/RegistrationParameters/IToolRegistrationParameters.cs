using System;
using System.Threading.Tasks;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	/// <summary>
	/// Represents the set of parameters your SDK needs to register and unregister a tool
	/// with the remote service.
	/// </summary>
	/// <remarks>
	/// Implement this interface to supply:
	/// 1. An <see cref="AccessKeyContainer"/> (zeroized by the SDK after use),
	/// 2. The local callback <see cref="LocalUrl"/>, and
	/// 3. The tool’s unique <see cref="ExternalId"/>.
	/// </remarks>
	public interface IToolRegistrationParameters
	{
		/// <summary>
		/// Asynchronously retrieves the <see cref="AccessKeyContainer"/> to authenticate
		/// with the remote service.
		/// <para>
		/// This method may perform I/O (e.g. calling a vault or secure store),
		/// so it follows the async pattern. The SDK will dispose the returned token
		/// (wiping its bytes) immediately after use.
		/// </para>
		/// </summary>
		/// <returns>
		/// A task that completes with the <see cref="AccessKeyContainer"/>.  
		/// The SDK is responsible for disposing it to zero out the secret.
		/// </returns>
		Task<Guid> GetApiKeyAsync();

		/// <summary>
		/// Gets the local URL where your tool is listening (for callbacks).
		/// </summary>
		Uri LocalUrl { get; }

		/// <summary>
		/// Gets the externally-assigned identifier (GUID) for this tool registration.
		/// </summary>
		/// <value>
		/// A <see cref="Guid"/> that the remote service uses
		/// to identify your tool instance.
		/// </value>
		Guid ExternalId { get; }
	}
}