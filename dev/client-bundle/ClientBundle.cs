using System.Text;
using Dargon.Client;
using Dargon.Nest.DevelopmentUtilities;
using Dargon.ThumbnailGenerator;

namespace client_bundle {
   public class ClientBundle : DevBundle {
      public override string Name => "client";

      public ClientBundle() {
         Eggs.Add(DevEgg.FromProject("dargon-client", typeof(DargonClientApplication)));
         Eggs.Add(DevEgg.FromProject("thumbnail-generator", typeof(ThumbnailGenerationApplication)));

         InitScript = Encoding.UTF8.GetString(Properties.Resources.init);
      }
   }
}
