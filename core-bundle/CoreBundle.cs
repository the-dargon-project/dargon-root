using Dargon.CLI;
using Dargon.Daemon;
using Dargon.Nest.DevelopmentUtilities;

namespace core_bundle {
   public class CoreBundle : NestBundle {
      public override string Name => "client";

      public CoreBundle() {
         Eggs.Add(NestEgg.FromProject("cored", typeof(CoreDaemonApplicationEgg)));
         Eggs.Add(NestEgg.FromProject("dargon-cli", typeof(DargonCliBundleDummyClass)));
      }
   }
}
