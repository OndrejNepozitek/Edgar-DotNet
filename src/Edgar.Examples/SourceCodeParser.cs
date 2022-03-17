using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Edgar.Examples
{
    public class SourceCodeParser
    {
        private readonly string path;
        private CompilationUnitSyntax root;
        private NamespaceDeclarationSyntax namespaceSyntax;
        private ClassDeclarationSyntax mainClassSyntax;

        public SourceCodeParser(string path)
        {
            this.path = path;
            Initialize();
        }

        private void Initialize()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            root = syntaxTree.GetRoot() as CompilationUnitSyntax;
            namespaceSyntax = root.Members.OfType<NamespaceDeclarationSyntax>().First();
            mainClassSyntax = namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First();
        }

        public List<string> GetMethod(string name, bool withDeclaration)
        {
            var method = mainClassSyntax
                .Members
                .OfType<MethodDeclarationSyntax>()
                .Single(x => x.Identifier.Text == name);

            var content = withDeclaration ? method.ToFullString() : method.Body.ToFullString();

            var sourceCode = content.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToList();

            if (!withDeclaration)
            {
                sourceCode = TrimBrackets(sourceCode);
            }

            sourceCode = RemoveLeftPadding(sourceCode);

            if (!withDeclaration)
            {
                sourceCode = sourceCode.Where(x => !x.StartsWith("return")).ToList();
            }

            return sourceCode;
        }

        public List<string> GetEnum(string name)
        {
            var enumSyntax = mainClassSyntax
                .Members
                .OfType<EnumDeclarationSyntax>()
                .Single(x => x.Identifier.Text == name);

            var content = enumSyntax.ToFullString();

            var sourceCode = content.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToList();


            sourceCode = RemoveLeftPadding(sourceCode);

            return sourceCode;
        }

        public List<string> GetClass(string name)
        {
            var classSyntax = mainClassSyntax
                .Members
                .OfType<ClassDeclarationSyntax>()
                .Single(x => x.Identifier.Text == name);

            var content = classSyntax.ToFullString();

            var sourceCode = content.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToList();

            sourceCode = RemoveLeftPadding(sourceCode);

            return sourceCode;
        }

        public List<string> GetWholeFile()
        {
            var content = root.ToFullString();

            var sourceCode = content.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToList();

            return sourceCode;
        }

        private List<string> TrimBrackets(List<string> sourceCode)
        {
            sourceCode = sourceCode.ToList();

            if (sourceCode[0].Trim() == "{")
            {
                sourceCode.RemoveAt(0);
            }

            while (sourceCode.Last().Trim() == "")
            {
                sourceCode.RemoveAt(sourceCode.Count - 1);
            }

            if (sourceCode.Last().Trim() == "}")
            {
                sourceCode.RemoveAt(sourceCode.Count - 1);
            }

            return sourceCode;
        }

        private List<string> RemoveLeftPadding(List<string> sourceCode)
        {
            sourceCode = sourceCode.ToList();
            var minPadding = int.MaxValue;

            foreach (var line in sourceCode)
            {
                if (line.Trim() == "")
                {
                    continue;
                }

                var padding = line.Length - line.TrimStart().Length;
                minPadding = Math.Min(minPadding, padding);
            }

            for (var i = 0; i < sourceCode.Count; i++)
            {
                var line = sourceCode[i];

                if (line.Trim() == "")
                {
                    continue;
                }

                sourceCode[i] = line.Remove(0, minPadding);
            }

            return sourceCode;
        }
    }
}