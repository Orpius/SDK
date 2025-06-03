#if NET7_0_OR_GREATER
#define USE_REQUIRED
#endif

namespace Orpius.Platform.ToolsModel.RpcToolsRegistrationService
{
	public interface IContractMessage
	{
		string TypeName { get; }
	}
}
