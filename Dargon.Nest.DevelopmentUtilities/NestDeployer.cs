using System;
using System.Collections.Generic;

namespace Dargon.Nest.DevelopmentUtilities {
   public static class NestDeployer {
      public static void Deploy(params NestBundle[] bundles) {

      }
   }

   public abstract class NestBundle {
      public abstract string Name { get; }
      public List<NestEgg> Eggs { get; } = new List<NestEgg>();
   }

   public class NestEgg {
      public string Name { get; set; }
      public string SourceDirectory { get; set; }

      public static NestEgg FromProject(string name, Type projectType) {
         return FromRelativeProject(name, projectType, ".");
      }

      public static NestEgg FromRelativeProject(string name, Type projectType, string relativeProjectDirectoryPath) {
         Console.WriteLine($"Project {name} at {projectType.Assembly.CodeBase} relative {relativeProjectDirectoryPath}.");
         return null;
      }
   }
}