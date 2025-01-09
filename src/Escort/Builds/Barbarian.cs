using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace TheEscort.Escort.Builds
{
    public class Barbarian : EscortClass
    {
        public static readonly PlayerFeature<bool> barbarianDisallowOversizedLuggage = PlayerBool("theescort/barbarian/nooverlug");

        public Color color;
        private Creature.Grasp? cretinHold;
        private int wiggle;
        private int shieldDelay;
        private int shieldStunDelay;
        private int shieldState;

        public override void Tick(Player self)
        {
            // Checks whether the grasp is held creature
            if (cretinHold?.discontinued ?? false)
                cretinHold = null;

            if (cretinHold is null)
            {
                foreach (var grasp in self.grasps)
                {
                    if (grasp?.grabbed is Creature cretin && !IsCretinBlacklisted(cretin, self))
                    {
                        cretinHold = grasp;
                        break;
                    }
                }
            }

            // Increase shield delay count if creature held and grab held
            // TODO: Disable item swallowing
            if (cretinHold is not null && self.input[0].pckp && shieldStunDelay == 0)
            {
                if (shieldDelay < 20)
                {
                    shieldDelay++;
                }
            }
            else
            {
                shieldDelay = 0;
            }

            // Shielding status check (if they go into a shortcut or exit reset shield)
            if (shieldDelay >= 20)
            {
                shieldState = cretinHold.graspUsed == 0 ? -1 : 1;
            }
            else
            {
                shieldState = 0;
            }

            // Shield stun delay. Get stun value from creature (may just need to use cretin.stun.... if it doesn't get affected by pacifying hold)
            if (shieldStunDelay > 0)
            {
                shieldStunDelay--;
            }
        }

        public override void Update(Player self)
        {
            // Apparently nothing is needed here
        }

        private static bool IsCretinBlacklisted(Creature cretin, Player self)
        {
            // General case
            if (cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug)
                return true;

            // Edge case where small centipedes cannot be used (scenario where Barbarian can use adult centipedes as shields and weapons lol)
            if (cretin is Centipede { Small: true })
                return true;

            // Edge case where slugpups/other players with friendly fire
            if (cretin is Player { room.game.IsStorySession: true } && RWCustom.Custom.rainWorld.options.friendlyFire)
                return true;

            // Edge case where creature is gineminasaurus
            if (IsCretinOversized(cretin) && barbarianDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized)
                return true;

            return false;
        }

        private static bool IsCretinOversized(Creature cretin)
        {
            return cretin is BigEel or Centipede { Small: true } or Deer or MirosBird or Vulture or TempleGuard;
        }
    }
}
