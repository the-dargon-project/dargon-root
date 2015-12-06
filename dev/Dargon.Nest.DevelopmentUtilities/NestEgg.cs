using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ItzWarty;

namespace Dargon.Nest.DevelopmentUtilities {
   public class NestEgg {
      public string Name { get; set; }
      public string SourceDirectory { get; set; }

      public static NestEgg FromProject(string name, Type projectType) {
         return FromRelativeProject(name, projectType, ".");
      }

      public static NestEgg FromRelativeProject(string name, Type projectType, string relativeProjectDirectoryPath) {

         var codeBase = new Uri(projectType.Assembly.CodeBase).AbsolutePath;
         var projectName = new FileInfo(codeBase).Name.Pass(x => x.Substring(0, x.Length - 4));
         var csprojName = $"{projectName}.csproj";
         var progress = new CliProgressSpinner($"{name}");
         progress.Update("Finding Project File");
         var matches = Directory.GetFiles(NestDeployerConstants.RootSolutionDirectoryPath, csprojName, SearchOption.AllDirectories);
         if (matches.Length == 0) {
            throw new FileNotFoundException($"Couldn't find {csprojName}!");
         } else if (matches.Length > 1) {
            throw new AmbiguousMatchException($"Found more than one match for {csprojName}: {matches.Join(", ")}");
         }
         var match = matches.First();
         var matchDirectory = Path.GetDirectoryName(match);
         var finalPath = Path.GetFullPath(Path.Combine(matchDirectory, relativeProjectDirectoryPath));
         progress.Update(finalPath);
         return new NestEgg() {
            Name = name,
            SourceDirectory = Path.Combine(finalPath, "bin", "Debug")
         };
      }
   }
}