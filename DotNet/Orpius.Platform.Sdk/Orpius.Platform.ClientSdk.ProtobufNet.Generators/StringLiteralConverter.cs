using System.Text;

namespace Orpius.Platform.Generators
{
	static class StringLiteralConverter
	{
		public static string ToCSharpStringLiteral(string? value)
		{
			if (value == null)
			{
				return "null";
			}

			var builder = new StringBuilder(value.Length + 2);

			builder.Append('"');

			foreach (char character in value)
			{
				switch (character)
				{
					case '\\':
					{
						builder.Append(@"\\");
						break;
					}

					case '"':
					{
						builder.Append("\\\"");
						break;
					}

					case '\r':
					{
						builder.Append(@"\r");
						break;
					}

					case '\n':
					{
						builder.Append(@"\n");
						break;
					}

					case '\t':
					{
						builder.Append(@"\t");
						break;
					}

					case '\0':
					{
						builder.Append(@"\0");
						break;
					}

					default:
					{
						if (char.IsControl(character))
						{
							builder.Append(@"\u");
							builder.Append(((int)character).ToString("x4"));
						}
						else
						{
							builder.Append(character);
						}

						break;
					}
				}
			}

			builder.Append('"');

			return builder.ToString();
		}
	}
}
