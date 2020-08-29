using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Edgar.Examples.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.Legacy.Utils.MapDrawing;
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

        // TODO: improve
        public static string AssetsFolder;

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
            new SimpleExample().Run();

            //GenerateExample(new BasicsExample());
            //GenerateExample(new CorridorsExample());
            //GenerateExample(new MinimumRoomDistanceExample());
            GenerateExample(new RealLifeExample());
        }

        private void GenerateExample<TRoom>(IExampleGrid2D<TRoom> example)
        {
            AssetsFolder = Path.Combine(outputFolder, example.DocsFileName);
            Directory.CreateDirectory(AssetsFolder);

            var className = example.GetType().Name;
            sourceCodeParser = new SourceCodeParser(Path.Combine(sourceFolder, $"{className}.cs"));

            var sourceCode = sourceCodeParser.GetMethod("GetLevelDescription", false);
            var stringBuilder = new StringBuilder();
            var insideCodeBlock = false;

            var results = example.GetResults().ToList();


            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine($"title: {example.Name}");
            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("import { Gallery, GalleryImage } from \"@theme/Gallery\";");
            stringBuilder.AppendLine();

            // stringBuilder.AppendLine($"> The source code for this example can be found at the end of this page.");
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

                    var levelDescription = results[resultsCounter];
                    var initStopwatch = new Stopwatch();
                    initStopwatch.Start();
                    var generator = new GraphBasedGeneratorGrid2D<TRoom>(levelDescription);
                    generator.InjectRandomGenerator(new Random(0));
                    initStopwatch.Stop();
                
                    var layoutDrawer = new GraphBasedGenerator.Grid2D.Drawing.SVGLayoutDrawer<TRoom>();
                    var oldMapDrawer = new DungeonDrawer<TRoom>();

                    var times = new List<long>();

                    for (int i = 0; i < 4; i++)
                    {
                        var generatorStopwatch = new Stopwatch();
                        generatorStopwatch.Start();
                        var level = generator.GenerateLayout();
                        generatorStopwatch.Stop();

                        times.Add(generatorStopwatch.ElapsedMilliseconds);

                        Console.WriteLine(generatorStopwatch.ElapsedMilliseconds + initStopwatch.ElapsedMilliseconds);

                        var svg = layoutDrawer.DrawLayout(level, 800, forceSquare: true, flipY: true, fixedFontSize: 30);
                        File.WriteAllText(Path.Combine(AssetsFolder, $"{resultsCounter}_{i}.svg"),
                            svg);

                        var bitmap = oldMapDrawer.DrawLayout(level, new DungeonDrawerOptions()
                        {
                            Width = 2000,
                            Height = 2000,
                            PaddingPercentage = 0.1f,
                        });
                        bitmap.Save(Path.Combine(AssetsFolder, $"{resultsCounter}_{i}.png"));
                    }

                    var summaryDrawer = new GeneratorSummaryDrawer<TRoom>();
                    var summary = summaryDrawer.Draw(levelDescription, 5000, generator);
                    summary.Save(Path.Combine(AssetsFolder, $"{resultsCounter}_summary.png"));

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("<Gallery cols={2}>");
                    for (int i = 0; i < 4; i++)
                    {
                        stringBuilder.AppendLine(
                            $"<GalleryImage src={{require('./{example.DocsFileName}/{resultsCounter}_{i}.png').default}} />");
                    }

                    stringBuilder.AppendLine("</Gallery>");

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("<div style={{ textAlign: 'center', marginTop: '-15px' }}>");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"*Average time to generate the level: {((times.Average() + initStopwatch.ElapsedMilliseconds) / 1000).ToString("F", CultureInfo.InvariantCulture)}s ({((initStopwatch.ElapsedMilliseconds) / 1000d).ToString("F", CultureInfo.InvariantCulture)}s init, {((times.Average()) / 1000).ToString("F", CultureInfo.InvariantCulture)}s generation itself)*");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("</div>");
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

            AddSourceCode(stringBuilder);

            Console.WriteLine(stringBuilder.ToString());
            File.WriteAllText(Path.Combine(outputFolder, $"{example.DocsFileName}.md"), stringBuilder.ToString());

            example.Run();

            foreach (var levelDescription in example.GetResults())
            {

            }
        }

        private void AddSourceCode(StringBuilder output)
        {
            var sourceCode = sourceCodeParser.GetWholeFile();
            sourceCode = RemoveRegions(sourceCode);
            sourceCode = RemoveInterface(sourceCode);
            sourceCode = ChangeNamespace(sourceCode);
            sourceCode = RemoveMarkdown(sourceCode);
            sourceCode = RemoveMultipleNewlines(sourceCode);
            sourceCode = RemoveNewlinesNearBrackets(sourceCode);

            output.AppendLine("## Source code");
            output.AppendLine();
            output.AppendLine("```");

            foreach (var line in sourceCode)
            {
                output.AppendLine(line);
            }

            output.AppendLine("```");
            output.AppendLine();
        }

        private List<string> RemoveRegions(List<string> sourceCode)
        {
            var output = new List<string>();
            var nesting = 0;
            var isInside = false;

            foreach (var line in sourceCode)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("#region") && trimmed.Contains("no-clean"))
                {
                    isInside = true;
                    nesting = 1;
                } 
                else if (isInside && trimmed.StartsWith("#region"))
                {
                    nesting++;
                }
                else if (isInside && trimmed.StartsWith("#endregion"))
                {
                    nesting--;
                }

                if (!isInside)
                {
                    output.Add(line);
                }

                if (nesting == 0)
                {
                    isInside = false;
                }
            }

            return output;
        }

        private List<string> RemoveMultipleNewlines(List<string> sourceCode)
        {
            var output = new List<string>();

            for (var i = 0; i < sourceCode.Count; i++)
            {
                var line = sourceCode[i];
                var nextLine = i < sourceCode.Count - 1 ? sourceCode[i + 1] : null;

                if (line.Trim() == "" && (nextLine != null && nextLine.Trim() == ""))
                {
                    // skip
                }
                else
                {
                    output.Add(line);
                }
            }

            return output;
        }

        private List<string> RemoveInterface(List<string> sourceCode)
        {
            var output = sourceCode.ToList();

            for (var i = 0; i < output.Count; i++)
            {
                var line = output[i];

                if (line.Contains("public class"))
                {
                    var split = line.Split(":");
                    output[i] = split[0];
                    break;
                }
            }

            return output;
        }

        private List<string> ChangeNamespace(List<string> sourceCode)
        {
            var output = sourceCode.ToList();

            for (var i = 0; i < output.Count; i++)
            {
                var line = output[i];

                if (line.Contains("namespace"))
                {
                    output[i] = "namespace Examples";
                    break;
                }
            }

            return output;
        }

        private List<string> RemoveNewlinesNearBrackets(List<string> sourceCode)
        {
            var output = new List<string>();

            for (var i = 0; i < sourceCode.Count; i++)
            {
                var line = sourceCode[i];
                var previousLine = i > 0 ? sourceCode[i - 1] : null;
                var nextLine = i < sourceCode.Count - 1 ? sourceCode[i + 1] : null;

                var previousLineIsBracket = previousLine != null && previousLine.Trim().StartsWith("{");
                var nextLineIsBracket = nextLine != null && nextLine.Trim().StartsWith("}");

                if (!(line.Trim() == "" && (previousLineIsBracket || nextLineIsBracket)))
                {
                    output.Add(line);
                }
            }

            return output;
        }

        private List<string> RemoveMarkdown(List<string> sourceCode)
        {
            return sourceCode
                .Where(x => !x.Trim().StartsWith("//md"))
                .Where(x => !x.Trim().StartsWith("//sc"))
                .ToList();
        }

        private void AddContent(List<string> sourceCode, StringBuilder output)
        {
            var codeBlockHandler = new CodeBlockHandler(output);

            for (var i = 0; i < sourceCode.Count; i++)
            {
                var line = sourceCode[i];
                var trimmed = line.TrimStart();

                if (trimmed.StartsWith("//md ") || trimmed.Equals("//md"))
                {
                    codeBlockHandler.Exit();
                    trimmed = trimmed.Remove(0, trimmed.Length > 5 ? 5 : 4);
                    output.AppendLine(trimmed);
                }
                else if (trimmed.StartsWith("//md_hide-next"))
                {
                    i++;
                }
                else if (trimmed.StartsWith("//md_sc"))
                {
                    codeBlockHandler.Exit();
                    var shortcode = trimmed.Replace("//md_sc ", "");
                    HandleShortcode(shortcode, output);
                }
                else if (trimmed.StartsWith("#region hidden"))
                {
                    var nestLevel = 1;
                    i++;

                    var description = trimmed.Replace("#region hidden:", "");

                    if (description != "" && trimmed.Contains("hidden:"))
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

                case "method_content":
                {
                    var methodName = parts[1];
                    var sourceCode = sourceCodeParser.GetMethod(methodName, false);
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