using System.Collections.Generic;

namespace Dargon.Nest.DevelopmentUtilities {
   public abstract class DevDeployment {
      public abstract string Name { get; }
      public List<DevBundle> Bundles { get; } = new List<DevBundle>();
      public virtual int NestClusterPort { get; } = -1;
   }
}