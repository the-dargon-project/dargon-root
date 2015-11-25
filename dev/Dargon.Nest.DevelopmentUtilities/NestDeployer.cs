using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using dev_egg_runner;
using Dargon.Ryu;
using ItzWarty;
using ItzWarty.IO;
using Nest.Init;

namespace Dargon.Nest.DevelopmentUtilities {
   public static class NestDeployerConstants {
      private const string kRootSolutionName = "dargon-root";

      public static string RootSolutionDirectoryPath => GetRootSolutionDirectoryPath();

      private static string GetRootSolutionDirectoryPath() {
         var codebase = new Uri(typeof(NestDeployer).Assembly.CodeBase).AbsolutePath;
         return codebase.Substring(0, codebase.IndexOf(kRootSolutionName) + kRootSolutionName.Length);
      }
   }

   public static class NestDeployer {
      private static readonly IFileSystemProxy fileSystemProxy;
      private static readonly string dargonPath;

      static NestDeployer() {
         var ryu = new RyuFactory().Create();
         ryu.Touch<ItzWartyProxiesRyuPackage>();
         fileSystemProxy = ryu.Get<IFileSystemProxy>();

         dargonPath = Path.Combine(NestDeployerConstants.RootSolutionDirectoryPath, "deploy", "Dargon");
         fileSystemProxy.PrepareDirectory(dargonPath);
      }

      public static void Clean() {
         while (true) {
            try {
               if (Directory.Exists(dargonPath)) {
                  Directory.Delete(dargonPath, true);
               }
               break;
            } catch (IOException e) {
               Console.WriteLine($"Can't clear {dargonPath}: {e}");
               Thread.Sleep(2000);
            } catch (UnauthorizedAccessException e) {
               Console.WriteLine($"This sometimes happens (ignore?): {e}");
            }
         }
         Directory.CreateDirectory(dargonPath);
      }

      public static void Deploy(params DevelopmentBundle[] bundles) {
         var nestsPath = Path.Combine(dargonPath, "nests");
         fileSystemProxy.PrepareDirectory(nestsPath);

         foreach (var bundle in bundles) {
            var bundleNestPath = Path.Combine(nestsPath, bundle.Name);
            fileSystemProxy.PrepareDirectory(bundleNestPath);

            var bundleNest = new LocalDargonNest(bundleNestPath);
            foreach (var egg in bundle.Eggs) {
               var progress = new CliProgressSpinner($"Installing egg {bundle.Name}/{egg.Name}.");
               var updateState = new UpdateState();
               updateState.PropertyChanged += (s, e) => {
                  progress.Update((float)updateState.SubProgress);
               };
               bundleNest.InstallEgg(
                  new InMemoryEgg(egg.Name, "dev", egg.SourceDirectory),
                  updateState);
            }

            if (!string.IsNullOrWhiteSpace(bundle.InitScript)) {
               var deployedInitJsonPath = Path.Combine(bundleNestPath, "init.json");
               File.WriteAllText(deployedInitJsonPath, bundle.InitScript);
            }
         }
      }

      public static void ExecInit(int port = 21337) {
         var initDirectory = NestEgg.FromProject("init", typeof(InitNestBundleDummy)).SourceDirectory;
         var initPath = Path.Combine(initDirectory, "init.exe");
         var deployedInitPath = Path.Combine(dargonPath, new FileInfo(initPath).Name);
         File.Copy(initPath, deployedInitPath, true);

         Process.Start(
            new ProcessStartInfo(
               deployedInitPath,
               $"-p {port}") {
               UseShellExecute = true
            });
      }
      public static void KillOnCtrlC(int port = 21337) {
         var spinner = new CliProgressSpinner("Ctrl+C = Kill Nest");
         Console.CancelKeyPress += (s, e) => {
            var commanderName = "dev-nest-commander";
            var commanderProject = NestEgg.FromProject(commanderName, typeof(CommanderNestBundleDummy));
            var commanderPath = Path.Combine(commanderProject.SourceDirectory, commanderName + ".exe");

            Process.Start(
               new ProcessStartInfo(
                  commanderPath,
                  $"-p {port} -c kill-nest") {
                  UseShellExecute = true
               });
            Environment.Exit(0);
         };
         while (true) {
            spinner.Update();
            Thread.Sleep(125);
         }
      }
   }

   public abstract class DevelopmentBundle {
      public abstract string Name { get; }
      public string InitScript { get; protected set; }
      public List<NestEgg> Eggs { get; } = new List<NestEgg>();
   }

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