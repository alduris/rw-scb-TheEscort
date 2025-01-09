using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEscort.Escort.Builds
{
    internal interface IAffectThrown
    {
        public void ThrowSpear(Player self, Spear spear, ref float thrust);
        public void ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu); // call orig by default
    }
}
