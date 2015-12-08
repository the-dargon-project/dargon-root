using Dargon.Nest.DevelopmentUtilities;
using Dargon.Trinkets.Hosted;
using Dargon.Trinkets.Proxy;

namespace trinket_bundle {
   public class TrinketBundle : DevBundle {
      public override string Name => "trinket";

      public TrinketBundle() {
         Eggs.Add(DevEgg.FromProject("trinket-proxy", typeof(TrinketProxyApplication)));
         Eggs.Add(DevEgg.FromRelativeProject("trinket-dim", typeof(TrinketProxyApplication), "../DargonInjectedModule"));
         Eggs.Add(DevEgg.FromProject("trinket-managed", typeof(HostedTrinketApplication)));
      }
   }
}
