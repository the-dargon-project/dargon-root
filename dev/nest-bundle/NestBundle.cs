using Dargon.Nest.Daemon;
using Dargon.Nest.DevelopmentUtilities;
using Dargon.Nest.Repl;
using nest_host;
using nest_spawner;

namespace nest_bundle {
   public class NestBundle : DevBundle {
      public override string Name => "nest";

      public NestBundle() {
         Eggs.Add(DevEgg.FromProject("nest", typeof(ReplGlobals)));
         Eggs.Add(DevEgg.FromProject("nestd", typeof(NestDaemonBundleDummy)));
         Eggs.Add(DevEgg.FromProject("nest-host", typeof(NestHostBundleDummy)));
         Eggs.Add(DevEgg.FromProject("nest-spawner", typeof(NestSpawnerBundleDummy)));
      }
   }
}
