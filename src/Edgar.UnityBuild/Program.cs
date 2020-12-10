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
            var assemblyName = "EdgarSingleFile.dll";
            var sourceDir = "EdgarSingleFile";
            var targetDir = "EdgarSingleFileModified";

            if (File.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }
            
            Directory.CreateDirectory(targetDir);

            CopyFilesRecursively(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir));

            var moduleDefinition = ModuleDefinition.ReadModule(Path.Combine(sourceDir, assemblyName));
            var vectorType = moduleDefinition.Types.Single(x => x.Name == "Vector2Int");
            vectorType.Name = "EdgarVector2Int";

            var stream = new FileStream(Path.Combine(targetDir, assemblyName), FileMode.Create);
            moduleDefinition.Write(stream);
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}
