using System;
using System.Collections.Generic;

namespace Orpius.Platform.Tooling.Utilities
{
	public class EnumToDictionaryConverter
	{
		public Dictionary<string, int> GetEnumDictionary<T>() where T : struct, Enum
		{
			var result = new Dictionary<string, int>();

			foreach (T value in Enum.GetValues(typeof(T)))
			{
				result[value.ToString()] = Convert.ToInt32(value);
			}

			return result;
		}
	}
}
