using System.Collections.Generic;

namespace Dargon.Nest.DevelopmentUtilities {
   public abstract class DevDeployment {
      public abstract string Name { get; }
      public List<DevBundle> Bundles { get; } = new List<DevBundle>();
      public List<DevEgg> AdditionalEggs { get; } = new List<DevEgg>();
      public virtual int NestClusterPort { get; } = -1;
      public virtual string Version { get; set; } = "dev";
   }
}