using Dargon.Nest.DevelopmentUtilities;
using dargon_client;

namespace deploy_nest_core_and_client {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.ServerClean();
         NestDeployer.ServerLikeDeployAndInit(new DargonClientDeployment());
      }
   }
}
