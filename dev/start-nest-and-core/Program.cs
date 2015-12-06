using Dargon.Nest.DevelopmentUtilities;
using dargon_client;

namespace start_nest_and_core {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.Clean();
         NestDeployer.DeployAndInit(new DargonClientDeployment());
      }
   }
}
