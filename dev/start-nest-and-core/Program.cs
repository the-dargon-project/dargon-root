﻿using Dargon.Nest.DevelopmentUtilities;
using dargon_client;

namespace start_nest_and_core {
   public static class Program {
      public static void Main(string[] args) {
         NestDeployer.ClientClean();
         NestDeployer.ClientLikeDeployAndInit(new DargonClientDeployment());
      }
   }
}
