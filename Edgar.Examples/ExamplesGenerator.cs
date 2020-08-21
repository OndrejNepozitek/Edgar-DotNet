using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Edgar.Examples.Grid2D;
using Edgar.Examples.MapDrawing;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.MapDrawing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Edgar.Examples
{
    public class ExamplesGenerator
    {
        private readonly string sourceFolder;
        private readonly string outputFolder;
        private SourceCodeParser sourceCodeParser;

        class RegionNodes
        {
            public SyntaxTrivia RegionDirective;
            public SyntaxTrivia EndRegionDirective;

            public TextSpan RegionSpan
            {
                get
                {
                    var start = RegionDirective.Span.Start;
                    var end = EndRegionDirective.Span.Start + EndRegionDirective.Span.Length;
                    return new TextSpan(start, end - start);
                }
            }

            public List<SyntaxNode> Nodes = new List<SyntaxNode>();

            public void AddNode(SyntaxNode node)
            {
                if (RegionSpan.Contains(node.Span))
                    Nodes.Add(node);
            }
        }

        public ExamplesGenerator(string sourceFolder, string outputFolder)
        {
            this.sourceFolder = sourceFolder;
            this.outputFolder = outputFolder;
        }

        public void Run()
        {
            //GenerateExample(new BasicsExample());
            //GenerateExample(new CorridorsExample());
            //GenerateExample(new MinimumRoomDistanceExample());
            GenerateExample(new RealLifeExample());
        }

        private void GenerateExample<TRoom>(IExampleGrid2D<TRoom> example)
        {
            var className = example.GetType().Name;
            sourceCodeParser = new SourceCodeParser(Path.Combine(sourceFolder, $"{className}.cs"));

            var sourceCode = sourceCodeParser.GetMethod("GetLevelDescription", false);
            var stringBuilder = new StringBuilder();
            var insideCodeBlock = false;


            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine($"title: {example.Name}");
            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("import { Gallery, GalleryImage } from \"@theme/Gallery\";");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine(
                $"> This documentation page was automatically generated from the source code that can be found [on github](https://github.com/OndrejNepozitek/Edgar-DotNet/blob/master/Edgar.Examples/Grid2D/{className}.cs).");
            stringBuilder.AppendLine();

            AddContent(sourceCode, stringBuilder);

            stringBuilder.AppendLine("## Results");
            stringBuilder.AppendLine();

            var resultsCounter = 0;
            foreach (var line in GetSourceCode(example, "GetResults"))
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("//md"))
                {
                    if (insideCodeBlock)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine("```");
                        stringBuilder.AppendLine();
                        insideCodeBlock = false;
                    }

                    if (trimmed.Length > 5)
                    {
                        trimmed = trimmed.Remove(0, 5);
                    }
                    else
                    {
                        trimmed = trimmed.Remove(0, 4);
                    }

                    stringBuilder.AppendLine(trimmed);
                }
                else if (trimmed.Contains("yield"))
                {
                    if (insideCodeBlock)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine("```");
                        stringBuilder.AppendLine();
                        insideCodeBlock = false;
                    }

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("<Gallery cols={2}>");
                    for (int i = 0; i < 4; i++)
                    {
                        stringBuilder.AppendLine(
                            $"<GalleryImage withoutLinks src={{require('!!url-loader!./{example.DocsFileName}/{resultsCounter}_{i}.jpg').default}} />");
                    }

                    stringBuilder.AppendLine("</Gallery>");
                    stringBuilder.AppendLine();
                    resultsCounter++;
                }
                else if (insideCodeBlock)
                {
                    stringBuilder.AppendLine(line);
                }
                else if (!string.IsNullOrEmpty(trimmed))
                {
                    insideCodeBlock = true;
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("```");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(line);
                }
                else
                {
                    stringBuilder.AppendLine();
                }
            }

            Console.WriteLine(stringBuilder.ToString());
            File.WriteAllText(Path.Combine(outputFolder, $"{example.DocsFileName}.md"), stringBuilder.ToString());

            resultsCounter = 0;
            foreach (var levelDescription in example.GetResults())
            {
                var generator = new GraphBasedGeneratorGrid2D<TRoom>(levelDescription);
                generator.InjectRandomGenerator(new Random(0));

                var layoutDrawer = new SVGLayoutDrawer<TRoom>();
                var oldMapDrawer = new OldMapDrawer<TRoom>();

                for (int i = 0; i < 4; i++)
                {
                    var level = generator.GenerateLayout();
                    var svg = layoutDrawer.DrawLayout(level, 800, forceSquare: true, flipY: true, fixedFontSize: 30);
                    Directory.CreateDirectory(Path.Combine(outputFolder, example.DocsFileName));
                    File.WriteAllText(Path.Combine(outputFolder, example.DocsFileName, $"{resultsCounter}_{i}.svg"),
                        svg);

                    var bitmap = oldMapDrawer.DrawLayout(level, 2000, 2000, fixedFontSize: 1.5f, withNames: true);
                    bitmap.Save(Path.Combine(outputFolder, example.DocsFileName, $"{resultsCounter}_{i}.jpg"));
                }

                resultsCounter++;
            }
        }

        private void AddContent(List<string> sourceCode, StringBuilder output)
        {
            var codeBlockHandler = new CodeBlockHandler(output);

            for (var i = 0; i < sourceCode.Count; i++)
            {
                var line = sourceCode[i];
                var trimmed = line.Trim();

                if (trimmed.StartsWith("//md"))
                {
                    codeBlockHandler.Exit();
                    trimmed = trimmed.Remove(0, trimmed.Length > 5 ? 5 : 4);
                    output.AppendLine(trimmed);
                }
                else if (trimmed.StartsWith("//sc"))
                {
                    codeBlockHandler.Exit();
                    var shortcode = trimmed.Replace("//sc ", "");
                    HandleShortcode(shortcode, output);
                }
                else if (trimmed.StartsWith("#region hidden:"))
                {
                    var nestLevel = 1;
                    i++;

                    var description = trimmed.Replace("#region hidden:", "");

                    if (description != "")
                    {
                        output.AppendLine();
                        output.AppendLine(line.Replace("#region hidden:", "/* ") + " */");
                        output.AppendLine();
                    }

                    while (i < sourceCode.Count)
                    {
                        var nextLine = sourceCode[i].Trim();

                        if (nextLine.StartsWith("#region"))
                        {
                            nestLevel++;
                        } 
                        else if (nextLine.StartsWith("#endregion"))
                        {
                            nestLevel--;
                        }

                        if (nestLevel == 0)
                        {
                            break;
                        }

                        i++;
                    }
                }
                else if (codeBlockHandler.IsInside)
                {
                    output.AppendLine(line);
                }
                else if (!string.IsNullOrEmpty(trimmed))
                {
                    codeBlockHandler.Enter();
                    output.AppendLine(line);
                }
                else
                {
                    output.AppendLine();
                }
            }

            codeBlockHandler.Exit();
        }

        private void HandleShortcode(string shortcode, StringBuilder output)
        {
            var parts = shortcode.Split(":");
            var type = parts[0];

            switch (type)
            {
                case "method":
                {
                    var methodName = parts[1];
                    var sourceCode = sourceCodeParser.GetMethod(methodName, true);
                    AddContent(sourceCode, output);
                    break;
                }

                case "enum":
                {
                    var enumName = parts[1];
                    var sourceCode = sourceCodeParser.GetEnum(enumName);
                    AddContent(sourceCode, output);
                    break;
                }

                case "class":
                {
                    var name = parts[1];
                    var sourceCode = sourceCodeParser.GetClass(name);
                    AddContent(sourceCode, output);
                    break;
                }
            }
        }

        private class CodeBlockHandler
        {
            private readonly StringBuilder output;
            private bool isInside;

            public bool IsInside => isInside;

            public CodeBlockHandler(StringBuilder output)
            {
                this.output = output;
            }

            public void Enter()
            {
                if (!isInside)
                {
                    isInside = true;
                    output.AppendLine();
                    output.AppendLine("```");
                    output.AppendLine();
                }
            }

            public void Exit()
            {
                if (isInside)
                {
                    isInside = false;
                    output.AppendLine();
                    output.AppendLine("```");
                    output.AppendLine();
                }
            }
        }

        private List<string> GetSourceCode(IExample example, string methodName)
        {
            var className = example.GetType().Name;
            var sourceCode = new List<string>();
            var allLines = File.ReadAllLines(Path.Combine(sourceFolder, $"{className}.cs"));
            var insideMainMethod = false;

            var syntaxTree =
                CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(sourceFolder, $"{className}.cs")));
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
            var namespaceSyntax = root.Members.OfType<NamespaceDeclarationSyntax>().First();
            var programClassSyntax = namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First();
            var mainMethodSyntax = programClassSyntax.Members.OfType<MethodDeclarationSyntax>().First();


            Console.WriteLine(mainMethodSyntax.ToString());

            //var regionNodesList = new List<RegionNodes>();
            //foreach (var regionDirective in root.DescendantTrivia()
            //    .Where(i => i.Kind() == SyntaxKind.RegionDirectiveTrivia))
            //    regionNodesList.Add(new RegionNodes {RegionDirective = regionDirective});
            //var count = regionNodesList.Count;
            //foreach (var endRegionDirective in root.DescendantTrivia()
            //    .Where(j => j.Kind() == SyntaxKind.EndRegionDirectiveTrivia))
            //    regionNodesList[--count].EndRegionDirective = endRegionDirective;
            //foreach (var node in root.DescendantNodes()
            //    .Where(i => i is MemberDeclarationSyntax || i is StatementSyntax))
            //foreach (var regionNodes in regionNodesList)
            //    regionNodes.AddNode(node);


            //foreach (var regionNode in regionNodesList)
            //{
            //    Console.WriteLine();

            //    var test = root.ToFullString();
            //    var test2 = root.GetText();
            //    var test3 = test2.GetSubText(regionNode.RegionSpan);

            //    Console.WriteLine(test3.ToString());
            //}

            Console.WriteLine();

            // get the region trivia
            var regions = root
                .DescendantNodes(descendIntoTrivia: true)
                .OfType<RegionDirectiveTriviaSyntax>()
                .ToList();

            foreach (var region in regions)
            {
                var regionText = region.ToString().Replace("#region ", "");
                Console.WriteLine(regionText);

                Console.WriteLine(region.ToFullString());

                var test = root.ToFullString();
                var test2 = root.GetText();
                var test3 = test2.GetSubText(region.Span);

                Console.WriteLine();
            }

            var sourceCodeParser = new SourceCodeParser(Path.Combine(sourceFolder, $"{className}.cs"));

            for (var i = 0; i < allLines.Length; i++)
            {
                var line = allLines[i];

                if (line.Contains(methodName) && line.Contains("public"))
                {
                    insideMainMethod = true;

                    if (!line.Contains("{"))
                    {
                        i++;
                    }
                }
                else if (line.StartsWith("        }") || line.StartsWith("            return"))
                {
                    insideMainMethod = false;
                }
                else if (insideMainMethod)
                {
                    if (line.StartsWith("            "))
                    {
                        line = line.Remove(0, "            ".Length);
                    }

                    sourceCode.Add(line);
                }
            }

            return sourceCode;
        }
    }
}