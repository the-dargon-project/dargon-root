using Dargon.Nest.DevelopmentUtilities;
using Dargon.Trinkets.Proxy;

namespace trinket_bundle {
   public class TrinketBundle : DevelopmentBundle {
      public override string Name => "trinket";

      public TrinketBundle() {
         Eggs.Add(NestEgg.FromProject("trinket-proxy", typeof(TrinketProxyEgg)));
         Eggs.Add(NestEgg.FromRelativeProject("trinket-dim", typeof(TrinketProxyEgg), "../DargonInjectedModule"));
      }
   }
}
