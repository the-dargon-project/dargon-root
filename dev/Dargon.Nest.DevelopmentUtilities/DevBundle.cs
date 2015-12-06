using System.Collections.Generic;

namespace Dargon.Nest.DevelopmentUtilities {
   public abstract class DevBundle {
      public abstract string Name { get; }
      public string InitScript { get; protected set; }
      public List<NestEgg> Eggs { get; } = new List<NestEgg>();
   }
}