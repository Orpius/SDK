using Markdig;

namespace Orpius.Samples.RealEstate
{
	public class MarkdownRenderer
	{
		readonly MarkdownPipeline pipeline;

		public MarkdownRenderer()
		{
			pipeline = new MarkdownPipelineBuilder()
					   .DisableHtml()
					   .UseAdvancedExtensions()
					   .Build();
		}

		public string ToHtml(string? markdown)
		{
			if (string.IsNullOrWhiteSpace(markdown))
			{
				return string.Empty;
			}

			return Markdown.ToHtml(markdown, pipeline);
		}
	}
}