using core_bundle;
using Dargon.Nest.DevelopmentUtilities;
using trinket_bundle;

namespace start_nest_and_core {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.Deploy(
            new CoreBundle(),
            new TrinketBundle()
         );
      }
   }
}
