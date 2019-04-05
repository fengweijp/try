using System;
using System.IO;
using FluentAssertions;
using HtmlAgilityPack;
using Markdig;
using Microsoft.DotNet.Try.Markdown;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Dotnet.Try.Markdown.Tests
{
    public class RenderingTests
    {
        private readonly ITestOutputHelper _output;

        public RenderingTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Markdown_snippets_can_be_transformed_into_editor_HTML()
        {
            var packageName = "Microsoft.Dotnet.Try.Markdown";
            var packageVersion = "1.2.3";

            var pipeline = new MarkdownPipelineBuilder()
                           .UseAnnotatedCodeBlocks()
                           .Build();

            var markdown = @"

```cs --editable:true
Console.WriteLine(""Hello world!"");
```
";

            var writer = new StringWriter();

            var context = new MarkdownParserContext();

            context.AddDefaultCodeBlockAnnotations(defaults =>
            {
                defaults.PackageName = packageName;
                defaults.PackageVersion = packageVersion;
            });

           Markdig.Markdown.ToHtml(
                markdown,
                writer,
                pipeline,
                context);

            var html = writer.ToString();

            _output.WriteLine(html);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var codeNode = htmlDocument.DocumentNode
                                       .SelectSingleNode("//pre/code");

            codeNode.Attributes["data-trydotnet-package"]
                    .Value
                    .Should()
                    .Be(packageName);
            codeNode.Attributes["data-trydotnet-package"]
                    .Value
                    .Should()
                    .Be(packageName);
            codeNode.Attributes["data-trydotnet-package-version"]
                    .Value
                    .Should()
                    .Be(packageVersion);
        }
    }
}