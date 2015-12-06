using client_bundle;
using core_bundle;
using Dargon.Nest.DevelopmentUtilities;
using nest_bundle;
using trinket_bundle;

namespace dargon_client {
   public class DargonClientDeployment : DevDeployment {
      public override string Name => "dargon-client";
      public override int NestClusterPort => 21999;

      public DargonClientDeployment() {
         Bundles.Add(new ClientBundle());
         Bundles.Add(new CoreBundle());
         Bundles.Add(new NestBundle());
         Bundles.Add(new TrinketBundle());
      }
   }
}
