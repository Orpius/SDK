using Orpius.Platform.Tooling;

namespace Orpius.Samples.RealEstate
{
	static class CombinedContextExtensions
	{
		public static string GetRequiredContextValue(
			this ICombinedContext context,
			string key)
		{
			if (!context.SharedContext.TryGetValue(key, out string? value))
			{
				throw new InvalidOperationException(
					$"The required context value '{key}' was not provided.");
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new InvalidOperationException(
					$"The required context value '{key}' was empty.");
			}

			return value;
		}

		public static Guid GetRequiredGuidContextValue(
			this ICombinedContext context,
			string key)
		{
			string value = GetRequiredContextValue(context, key);

			if (!Guid.TryParse(value, out Guid guid))
			{
				throw new InvalidOperationException(
					$"The required context value '{key}' was not a valid Guid.");
			}

			return guid;
		}
	}
}