using core_bundle;
using Dargon.Nest.DevelopmentUtilities;
using nest_bundle;
using trinket_bundle;

namespace start_nest_and_core {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.Clean();
         NestDeployer.Deploy(
            new NestBundle(),
            new CoreBundle(),
            new TrinketBundle()
         );
         NestDeployer.ExecInit(21999);
         NestDeployer.KillOnCtrlC(21999);
      }
   }
}
