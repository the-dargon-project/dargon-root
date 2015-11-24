﻿using System.Text;
using Dargon.CLI;
using Dargon.Daemon;
using Dargon.Nest.DevelopmentUtilities;

namespace core_bundle {
   public class CoreBundle : DevelopmentBundle {
      public override string Name => "client";

      public CoreBundle() {
         Eggs.Add(NestEgg.FromProject("cored", typeof(CoreDaemonApplicationEgg)));
         Eggs.Add(NestEgg.FromProject("dargon-cli", typeof(DargonCliBundleDummyClass)));

         InitScript = Encoding.UTF8.GetString(global::core_bundle.Properties.Resources.init);
      }
   }
}