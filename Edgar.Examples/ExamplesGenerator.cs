using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Edgar.Examples.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.MapDrawing;

namespace Edgar.Examples
{
    public class ExamplesGenerator
    {
        private readonly string sourceFolder;
        private readonly string outputFolder;

        public ExamplesGenerator(string sourceFolder, string outputFolder)
        {
            this.sourceFolder = sourceFolder;
            this.outputFolder = outputFolder;
        }

        public void Run()
        {
            var examples = new List<IExampleGrid2D>()
            {
                new BasicsExample(),
                new CorridorsExample(),
                new MinimumRoomDistanceExample(),
            };

            foreach (var example in examples)
            {
                GenerateExample(example);
            }
        }

        private void GenerateExample(IExampleGrid2D example)
        {
            var sourceCode = GetSourceCode(example, "GetLevelDescription");
            var stringBuilder = new StringBuilder();
            var insideCodeBlock = false;
            var className = example.GetType().Name;

            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine($"title: {example.Name}");
            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("import { Gallery, GalleryImage } from \"@theme/Gallery\";");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine(
                $"> This documentation page was automatically generated from the source code that can be found [on github](https://github.com/OndrejNepozitek/Edgar-DotNet/blob/master/Edgar.Examples/Grid2D/{className}.cs).");
            stringBuilder.AppendLine();

            foreach (var line in sourceCode)
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

            if (insideCodeBlock)
            {
                stringBuilder.AppendLine("```");
                stringBuilder.AppendLine();
                insideCodeBlock = false;
            }

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
                    stringBuilder.AppendLine("<Gallery cols={4}>");
                    for (int i = 0; i < 4; i++)
                    {
                        stringBuilder.AppendLine($"<GalleryImage withoutLinks src={{require('!!url-loader!./{example.DocsFileName}/{resultsCounter}_{i}.svg').default}} />");
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
                var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
                generator.InjectRandomGenerator(new Random(0));

                var layoutDrawer = new SVGLayoutDrawer<int>();

                for (int i = 0; i < 4; i++)
                {
                    var level = generator.GenerateLayout();
                    var svg = layoutDrawer.DrawLayout(level, 800, forceSquare: true, flipY: true, fixedFontSize: 30);
                    Directory.CreateDirectory(Path.Combine(outputFolder, example.DocsFileName));
                    File.WriteAllText(Path.Combine(outputFolder, example.DocsFileName, $"{resultsCounter}_{i}.svg"), svg);
                }

                resultsCounter++;
            }
        }

        private List<string> GetSourceCode(IExample example, string methodName)
        {
            var className = example.GetType().Name;
            var sourceCode = new List<string>();
            var allLines = File.ReadAllLines(Path.Combine(sourceFolder, $"{className}.cs"));
            var insideMainMethod = false;

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