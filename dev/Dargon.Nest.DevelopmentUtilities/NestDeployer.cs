using Dargon.Nest.Internals;
using Dargon.Ryu;
using dev_egg_runner;
using ItzWarty;
using ItzWarty.IO;
using Nest.Init;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Dargon.Nest.DevelopmentUtilities {
   public static class NestDeployer {
      private static readonly IFileSystemProxy fileSystemProxy;
      private static readonly string dargonPath;

      static NestDeployer() {
         var ryu = new RyuFactory().Create();
         ryu.Touch<ItzWartyProxiesRyuPackage>();
         fileSystemProxy = ryu.Get<IFileSystemProxy>();

         dargonPath = Path.Combine(NestDeployerConstants.RootSolutionDirectoryPath, "deploy", "nest_client");
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

      public static void DeployAndInit(DevDeployment developmentDeployment) {
         var deploymentPath = Path.Combine(dargonPath, NestConstants.kDeploymentsDirectoryName, developmentDeployment.Name);
         fileSystemProxy.PrepareDirectory(deploymentPath);

         // setup bundles directory
         var bundlesPath = Path.Combine(deploymentPath, "bundles");
         fileSystemProxy.PrepareDirectory(bundlesPath);

         // deploy bundles
         foreach (var developmentBundle in developmentDeployment.Bundles) {
            var bundlePath = Path.Combine(bundlesPath, developmentBundle.Name);
            fileSystemProxy.PrepareDirectory(bundlePath);

            var bundle = BundleFactory.Local(bundlePath);
            foreach (var egg in developmentBundle.Eggs) {
               var progress = new CliProgressSpinner($"Installing egg {developmentBundle.Name}/{egg.Name}.");
               progress.Update("...");
               bundle.InstallEgg(EggFactory.InMemory(egg.Name, egg.SourceDirectory, "dev"));
               progress.Update("Done");
            }

            if (!string.IsNullOrWhiteSpace(developmentBundle.InitScript)) {
               var deployedInitJsonPath = Path.Combine(bundlePath, "init.json");
               File.WriteAllText(deployedInitJsonPath, developmentBundle.InitScript);
            }
         }

         // deploy init
         var initDirectory = NestEgg.FromProject("init", typeof(InitNestBundleDummy)).SourceDirectory;
         var initPath = Path.Combine(initDirectory, "init.exe");
         var deployedInitPath = Path.Combine(deploymentPath, new FileInfo(initPath).Name);
         File.Copy(initPath, deployedInitPath, true);

         // start init
         Process.Start(
            new ProcessStartInfo(
               deployedInitPath,
               $"-p {developmentDeployment.NestClusterPort}") {
               UseShellExecute = true
            });

         // await ctrl+c for kill nest
         var spinner = new CliProgressSpinner("Ctrl+C = Kill Nest");
         Console.CancelKeyPress += (s, e) => {
            var commanderName = "dev-nest-commander";
            var commanderProject = NestEgg.FromProject(commanderName, typeof(CommanderNestBundleDummy));
            var commanderPath = Path.Combine(commanderProject.SourceDirectory, commanderName + ".exe");

            Process.Start(
               new ProcessStartInfo(
                  commanderPath,
                  $"-p {developmentDeployment.NestClusterPort} -c kill-nest") {
                  UseShellExecute = true
               });
            e.Cancel = false;
         };
         while (true) {
            spinner.Update();
            Thread.Sleep(125);
         }
      }
   }
}