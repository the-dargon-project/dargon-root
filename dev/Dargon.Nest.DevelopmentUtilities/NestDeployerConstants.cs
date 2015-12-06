using System;

namespace Dargon.Nest.DevelopmentUtilities {
   public static class NestDeployerConstants {
      private const string kRootSolutionName = "dargon-root";

      public static string RootSolutionDirectoryPath => GetRootSolutionDirectoryPath();

      private static string GetRootSolutionDirectoryPath() {
         var codebase = new Uri(typeof(NestDeployer).Assembly.CodeBase).AbsolutePath;
         return codebase.Substring(0, codebase.IndexOf(kRootSolutionName) + kRootSolutionName.Length);
      }
   }
}