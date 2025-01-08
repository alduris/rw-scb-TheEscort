using BepInEx;
using SlugBase.Features;
using System;
using RWCustom;
using MoreSlugcats;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Barbarian tweak values
        // public static readonly PlayerFeature<> barbarian = Player("theescort/barbarian/");
        // public static readonly PlayerFeature<float> barbarian = PlayerFloat("theescort/barbarian/");
        // public static readonly PlayerFeature<float[]> barbarian = PlayerFloats("theescort/barbarian/");


        public void Esclass_BB_Tick(Player self, ref Escort e)
        {
        }

        private void Esclass_BB_Update(Player self, ref Escort e)
        {
            
        }


        /// <summary>
        /// Checks whether the creature is not on the Barbarian blacklist (which will allow Barbarian to use the creature as a weapon)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklist(Creature cretin, Player self = null)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the creature is not on the Barbarian blacklist (which will allow Barbarian to use the creature as a weapon) (Single expression for shits and giggles)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklistSingleExpressionA(Creature cretin, Player self = null)
        {
            return !(cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug || (cretin is Centipede c && c.Small) || (cretin is Player pal && (cretin.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || pal.room?.game?.session is not ArenaGameSession && ModManager.CoopAvailable) && !RWCustom.Custom.rainWorld.options.friendlyFire) || (cretin is BigEel or Centipede or Deer or MirosBird or Vulture && self is not null && barbarianDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized));
        }
    }
}
