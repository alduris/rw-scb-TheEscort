using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEscort.Escort.Builds
{
    internal interface IAdjustDamage
    {
        public void OnWeaponHit(Player self, PhysicalObject weapon, Creature creature, ref float damage);
    }
}
