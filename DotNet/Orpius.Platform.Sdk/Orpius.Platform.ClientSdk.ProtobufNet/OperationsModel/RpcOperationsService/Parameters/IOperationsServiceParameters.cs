using System;
using System.Threading.Tasks;

namespace Orpius.Platform.OperationsModel
{
	public interface IOperationsServiceParameters
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
		/// Gets the externally-assigned identifier (GUID) for this tool registration.
		/// </summary>
		/// <value>
		/// A <see cref="Guid"/> that the remote service uses
		/// to identify your tool instance.
		/// </value>
		Guid ExternalId { get; }
	}

	public class FuncOperationsParameters : IOperationsServiceParameters
	{
		readonly Func<Guid> getExternalId;
		readonly Func<Task<Guid>> getApiKey;

		public FuncOperationsParameters(Func<Guid> getExternalId,
										Func<Task<Guid>> getApiKey)
		{
			this.getExternalId = getExternalId
								 ?? throw new ArgumentNullException(nameof(getExternalId));
			this.getApiKey = getApiKey
							 ?? throw new ArgumentNullException(nameof(getApiKey));
		}

		public Task<Guid> GetApiKeyAsync()
		{
			return getApiKey();
		}

		public Guid ExternalId => getExternalId();
	}
}
