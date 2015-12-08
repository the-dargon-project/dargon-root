using Dargon.Nest.Internals;
using Dargon.Nest.Internals.Eggs;
using Dargon.Ryu;
using dev_egg_runner;
using ItzWarty;
using ItzWarty.IO;
using Nest.Init;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dargon.Nest.DevelopmentUtilities {
   public static class NestDeployer {
      private static readonly IFileSystemProxy fileSystemProxy;
      private static readonly string nestClientPath;
      private static readonly string nestServerPath;

      static NestDeployer() {
         var ryu = new RyuFactory().Create();
         ryu.Touch<ItzWartyProxiesRyuPackage>();
         fileSystemProxy = ryu.Get<IFileSystemProxy>();

         nestClientPath = Path.Combine(NestDeployerConstants.RootSolutionDirectoryPath, "deploy", "nest_client");
         fileSystemProxy.PrepareDirectory(nestClientPath);

//         nestServerPath = Path.Combine(NestDeployerConstants.RootSolutionDirectoryPath, "deploy", "nest_server");
         nestServerPath = @"C:\Apache24\htdocs"; //Path.Combine(NestDeployerConstants.RootSolutionDirectoryPath, "deploy", "nest_server");
         fileSystemProxy.PrepareDirectory(nestServerPath);
      }

      public static void ClientClean() => CleanDirectory(nestClientPath);

      public static void ServerClean() => CleanDirectory(nestServerPath);

      private static void CleanDirectory(string path) {
         while (true) {
            try {
               if (Directory.Exists(path)) {
                  Directory.Delete(path, true);
               }
               break;
            } catch (IOException e) {
               Console.WriteLine($"Can't clear {path}: {e}");
               Thread.Sleep(2000);
            } catch (UnauthorizedAccessException e) {
               Console.WriteLine($"This sometimes happens (ignore?): {e}");
            }
         }
         Directory.CreateDirectory(path);
      }

      public static void ServerLikeDeployAndInit(DevDeployment developmentDeployment) {
         var eggsDirectoryPath = Path.Combine(nestServerPath, NestConstants.kEggsDirectoryName);
         fileSystemProxy.PrepareDirectory(eggsDirectoryPath);

         var bundlesDirectoryPath = Path.Combine(nestServerPath, NestConstants.kBundlesDirectoryName);
         fileSystemProxy.PrepareDirectory(bundlesDirectoryPath);

         var deploymentsDirectoryPath = Path.Combine(nestServerPath, NestConstants.kDeploymentsDirectoryName);
         fileSystemProxy.PrepareDirectory(deploymentsDirectoryPath);

         foreach (var developmentBundle in developmentDeployment.Bundles) {
            foreach (var developmentEgg in developmentBundle.Eggs) {
               ServerLikeDeployEgg(developmentDeployment, eggsDirectoryPath, developmentEgg);
            }

            // create bundle release
            var bundleReleasePath = Path.Combine(bundlesDirectoryPath, developmentBundle.Name, NestConstants.kReleasesDirectoryName, developmentBundle.Name + "-" + developmentDeployment.Version);
            fileSystemProxy.PrepareDirectory(bundleReleasePath);

            var eggsString = string.Join("\r\n", developmentBundle.Eggs.Select(e => e.Name + " " + developmentDeployment.Version));
            fileSystemProxy.WriteAllText(Path.Combine(bundleReleasePath, NestConstants.kEggsFileName), eggsString);

            var initScript = developmentBundle.InitScript ?? "";
            fileSystemProxy.WriteAllText(Path.Combine(bundleReleasePath, NestConstants.kInitJsonFileName), initScript);
         }

         // Deploy additional eggs (namely, init)
         foreach (var developmentEgg in developmentDeployment.AdditionalEggs) {
            ServerLikeDeployEgg(developmentDeployment, eggsDirectoryPath, developmentEgg);

            // point latest to our egg
            var stablePath = Path.Combine(eggsDirectoryPath, developmentEgg.Name, NestConstants.kChannelsDirectoryName, "stable");
            fileSystemProxy.PrepareParentDirectory(stablePath);
            File.WriteAllText(stablePath, developmentDeployment.Version);
         }

         // Deploy the deployment
         var deploymentDirectoryPath = Path.Combine(deploymentsDirectoryPath, developmentDeployment.Name);
         var deploymentLatestPath = Path.Combine(deploymentDirectoryPath, NestConstants.kChannelsDirectoryName, "stable");
         fileSystemProxy.PrepareParentDirectory(deploymentLatestPath);
         fileSystemProxy.WriteAllText(deploymentLatestPath, developmentDeployment.Version);

         var deploymentReleasePath = Path.Combine(deploymentDirectoryPath, NestConstants.kReleasesDirectoryName, developmentDeployment.Name + "-" + developmentDeployment.Version);
         fileSystemProxy.PrepareDirectory(deploymentReleasePath);

         var bundlesString = developmentDeployment.Bundles.Select(b => b.Name + " " + developmentDeployment.Version).Join("\r\n");
         fileSystemProxy.WriteAllText(Path.Combine(deploymentReleasePath, NestConstants.kBundlesFileName), bundlesString);
      }

      private static void ServerLikeDeployEgg(DevDeployment deployment, string serverEggsDirectoryPath, DevEgg developmentEgg) {
         var serverEggDirectory = Path.Combine(serverEggsDirectoryPath, developmentEgg.Name);

         // import dev egg to cache directory
         var cache = new NestFileCache(Path.Combine(serverEggDirectory, NestConstants.kCacheDirectoryName));
         var inMemoryEgg = EggFactory.InMemory(developmentEgg.Name, developmentEgg.SourceDirectory, "dev");
         var cacheDeployTasks = new List<Task>();
         var fileList = inMemoryEgg.EnumerateFilesAsync().Result.ToArray();
         foreach (var fileEntry in fileList) {
            var cacheDeployTask = cache.OpenOrAddAndOpenAsync(fileEntry.Guid, add => {
               var fullPath = inMemoryEgg.ComputeFullPath(fileEntry.InternalPath);
               return IoUtilities.ReadBytesAsync(fullPath);
            });
            cacheDeployTasks.Add(cacheDeployTask);
         }

         // create release
         var releaseDirectoryPath = Path.Combine(serverEggDirectory, NestConstants.kReleasesDirectoryName, developmentEgg.Name + "-" + deployment.Version);
         fileSystemProxy.PrepareDirectory(releaseDirectoryPath);
         var filelistContents = EggFileEntrySerializer.Serialize(fileList);
         File.WriteAllText(Path.Combine(releaseDirectoryPath, NestConstants.kFileListFileName), filelistContents);
      }

      public static void ClientLikeDeployAndInit(DevDeployment developmentDeployment) {
         var clientDeploymentPath = Path.Combine(nestClientPath, NestConstants.kDeploymentsDirectoryName, developmentDeployment.Name);
         fileSystemProxy.PrepareDirectory(clientDeploymentPath);

         // create cache directory
         var cacheDirectoryPath = IoUtilities.CombinePath(clientDeploymentPath, NestConstants.kCacheDirectoryName);
         IoUtilities.PrepareDirectory(cacheDirectoryPath);

         // setup bundles directory
         var clientBundlesPath = Path.Combine(clientDeploymentPath, "bundles");
         fileSystemProxy.PrepareDirectory(clientBundlesPath);

         // deploy bundles
         foreach (var developmentBundle in developmentDeployment.Bundles) {
            var bundlePath = Path.Combine(clientBundlesPath, developmentBundle.Name);
            fileSystemProxy.PrepareDirectory(bundlePath);

            var bundle = BundleFactory.Local(bundlePath);
            foreach (var egg in developmentBundle.Eggs) {
               var progress = new CliProgressSpinner($"Installing egg {developmentBundle.Name}/{egg.Name}.");
               progress.Update("...");
               bundle.InstallEggAsync(EggFactory.InMemory(egg.Name, egg.SourceDirectory, "dev")).Wait();
               progress.Update("Done");
            }

            if (!string.IsNullOrWhiteSpace(developmentBundle.InitScript)) {
               var deployedInitJsonPath = Path.Combine(bundlePath, "init.json");
               File.WriteAllText(deployedInitJsonPath, developmentBundle.InitScript);
            }
         }

         // deploy init, sort of a hack
         var clientBundle = BundleFactory.Local(clientDeploymentPath);
         var developmentInitEgg = DevEgg.FromProject("init", typeof(InitNestBundleDummy));
         clientBundle.InstallEggAsync(EggFactory.InMemory("init", developmentInitEgg.SourceDirectory, "dev")).Wait();

         var initEggDirectory = IoUtilities.CombinePath(clientDeploymentPath, "init");
         var deployedInitPath = Path.Combine(initEggDirectory, "init.exe");

         // start init
         Console.WriteLine("Starting init: " + deployedInitPath);
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
            var commanderProject = DevEgg.FromProject(commanderName, typeof(CommanderNestBundleDummy));
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