using System.Text;
using Dargon.CLI;
using Dargon.Daemon;
using Dargon.Nest.DevelopmentUtilities;

namespace core_bundle {
   public class CoreBundle : DevBundle {
      public override string Name => "core";

      public CoreBundle() {
         Eggs.Add(DevEgg.FromProject("cored", typeof(CoreDaemonApplication)));
         Eggs.Add(DevEgg.FromProject("dargon-cli", typeof(DargonCliBundleDummyClass)));

         InitScript = Encoding.UTF8.GetString(global::core_bundle.Properties.Resources.init);
      }
   }
}
