using System;
using System.Diagnostics.CodeAnalysis;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IToolResolver
	{
		//bool TryGetTool<T>(string toolName, [NotNullWhen(true)]out T tool);
		bool TryGetTool<T>([NotNullWhen(true)] out T? tool)
			where T : class;
	}

	//public class LambdaResolver : IToolResolver
	//{
	//	Func<TInterface, TClass> getItemFromContainer;

	//	public LambdaResolver(Func<TInterface, TClass> getItemFromContainer)
	//	{
	//		this.getItemFromContainer = getItemFromContainer;
	//	}
		
	//	public bool TryGetTool<T>([NotNullWhen(true)] out T tool)
	//	{
	//		getItemFromContainer<T>();
	//	}
	//}
}