using Dargon.Nest.DevelopmentUtilities;
using Dargon.Trinkets.Proxy;

namespace trinket_bundle {
   public class TrinketBundle : NestBundle {
      public override string Name => "trinket";

      public TrinketBundle() {
         Eggs.Add(NestEgg.FromProject("trinket", typeof(TrinketProxyEgg)));
         Eggs.Add(NestEgg.FromRelativeProject("trinket-dim", typeof(TrinketProxyEgg), "../DargonInjectedModule"));
      }
   }
}
