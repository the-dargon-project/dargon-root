using Dargon.Client;
using Dargon.Nest.DevelopmentUtilities;
using Dargon.ThumbnailGenerator;

namespace client_bundle {
   public class ClientBundle : NestBundle {
      public override string Name => "client";

      public ClientBundle() {
         Eggs.Add(NestEgg.FromProject("dargon-client", typeof(DargonClientEgg)));
         Eggs.Add(NestEgg.FromProject("thumbnail-generator", typeof(ThumbnailGenerationEgg)));
      }
   }
}
