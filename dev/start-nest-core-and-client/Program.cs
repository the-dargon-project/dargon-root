using Dargon.Nest.DevelopmentUtilities;
using dargon_client;

namespace start_nest_core_and_client {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.Clean();
         NestDeployer.DeployAndInit(new DargonClientDeployment());
      }
   }
}
