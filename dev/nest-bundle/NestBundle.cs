using Dargon.Nest.Daemon;
using Dargon.Nest.DevelopmentUtilities;
using Dargon.Nest.Repl;
using nest_host;
using nest_spawner;

namespace nest_bundle {
   public class NestBundle : DevBundle {
      public override string Name => "nest";

      public NestBundle() {
         Eggs.Add(NestEgg.FromProject("nest", typeof(ReplGlobals)));
         Eggs.Add(NestEgg.FromProject("nestd", typeof(NestDaemonBundleDummy)));
         Eggs.Add(NestEgg.FromProject("nest-host", typeof(NestHostBundleDummy)));
         Eggs.Add(NestEgg.FromProject("nest-spawner", typeof(NestSpawnerBundleDummy)));
      }
   }
}
