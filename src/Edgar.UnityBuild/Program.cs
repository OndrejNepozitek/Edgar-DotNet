using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Edgar.UnityBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Expected source and target assembly paths.");
            }

            var sourceAssembly = Path.GetFullPath(args[0]);
            var targetAssembly = Path.GetFullPath(args[1]);
            var sourceDir = Path.GetDirectoryName(sourceAssembly) ?? throw new InvalidOperationException("Source assembly directory is missing.");
            var targetDir = Path.GetDirectoryName(targetAssembly) ?? throw new InvalidOperationException("Target assembly directory is missing.");

            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            Directory.CreateDirectory(targetDir);

            CopyFilesRecursively(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir));

            using var moduleDefinition = ModuleDefinition.ReadModule(sourceAssembly);
            var vectorType = moduleDefinition.Types.SingleOrDefault(x => x.Name == "Vector2Int")
                             ?? throw new InvalidOperationException("Could not find Vector2Int in the merged Edgar assembly.");
            vectorType.Name = "EdgarVector2Int";

            using var stream = new FileStream(targetAssembly, FileMode.Create, FileAccess.Write);
            moduleDefinition.Write(stream);
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}