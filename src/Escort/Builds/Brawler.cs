using System;
using System.Collections.Generic;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Enums.Sounds;


namespace TheEscort.Escort.Builds
{
    public class Brawler : EscortClass, IAffectThrown
    {
        private enum BrawlerWeapon
        {
            NONE,

            /// <summary>
            /// Super shank (spear + creature)
            /// </summary>
            SuperShank,

            /// <summary>
            /// Shanking (spear)
            /// </summary>
            Shank,

            /// <summary>
            /// Punching (rock)
            /// </summary>
            Punch,

            /// <summary>
            /// Explosive punch (scavenger bomb)
            /// </summary>
            PowerPunch
        }

        private BrawlerWeapon WeaponFromGrasp(Player self, int grasp)
        {
            if (self.grasps[grasp] is not null)
            {
                return self.grasps[grasp].grabbed switch
                {
                    Spear and not ExplosiveSpear => BrawlerWeapon.Shank,
                    Rock => BrawlerWeapon.Punch,
                    ScavengerBomb => BrawlerWeapon.PowerPunch,
                    _ => BrawlerWeapon.NONE
                };
            }
            return BrawlerWeapon.NONE;
        }

        public static readonly PlayerFeature<float> brawlerSlideLaunchFac = PlayerFloat("theescort/brawler/slide_launch_fac");
        public static readonly PlayerFeature<float> brawlerDKHypeDmg = PlayerFloat("theescort/brawler/dk_h_dmg");
        public static readonly PlayerFeature<float[]> brawlerSpearVelFac = PlayerFloats("theescort/brawler/spear_vel_fac");
        public static readonly PlayerFeature<float[]> brawlerSpearDmgFac = PlayerFloats("theescort/brawler/spear_dmg_fac");
        public static readonly PlayerFeature<float> brawlerSpearThrust = PlayerFloat("theescort/brawler/spear_thrust");
        public static readonly PlayerFeature<float[]> brawlerSpearShankY = PlayerFloats("theescort/brawler/spear_shank");
        public static readonly PlayerFeature<float> brawlerRockHeight = PlayerFloat("theescort/brawler/rock_height");

        public override Color DefaultColor => new (0.447f, 0.235f, 0.53f);
        private bool wall = false;

        private bool shankSpearTumbler = false;
        private Vector2 shankDir = Vector2.zero;

        private Stack<Weapon> meleeWeapon = new(1);
        private int throwUsed = -1;
        private int throwGrab = -1;

        private int revertWall = -1;
        private Stack<Spear> wallSpear = new(1);
        private BrawlerWeapon currWeapon = BrawlerWeapon.NONE;
        private BrawlerWeapon lastWeapon = BrawlerWeapon.NONE;

        private bool AnyShank => currWeapon == BrawlerWeapon.SuperShank || currWeapon == BrawlerWeapon.Shank;

        private int setCooldown = 20;

        public override void Tick(Player self)
        {
            if (revertWall > 0)
            {
                revertWall--;
            }
            if (throwGrab > 0)
            {
                throwGrab--;
            }
        }

        public override void Update(Player self)
        {
            // Melee weapon use
            try
            {
                if (meleeWeapon.Count > 0 && throwGrab == 0 && self.grasps[throwUsed] == null)
                {
                    if (meleeWeapon.Peek() == null)
                    {
                        Ebug("NULL IN STACK!");
                        meleeWeapon.Clear();
                        return;
                    }
                    Ebug("Weapon mode was: " + meleeWeapon.Peek().mode);

                    // Post weapon usage cooldowns
                    try
                    {
                        if (self.room != null && meleeWeapon.Peek().mode == Weapon.Mode.StuckInCreature)
                        {
                            var sfxChunk = meleeWeapon.Peek().meleeHitChunk;
                            self.room.PlaySound(Escort_SFX_Brawler_Shank, sfxChunk);
                            self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, sfxChunk);
                            if (self.slowMovementStun > 0) self.Blink(30);
                            else
                            {
                                self.slowMovementStun += 20;
                            }
                        }
                        else if (meleeWeapon.Peek() is Rock)
                        {
                            self.slowMovementStun += 25;
                            currWeapon = BrawlerWeapon.Punch;
                        }
                        else if (meleeWeapon.Peek() is ScavengerBomb sb)
                        {
                            self.slowMovementStun += 40;
                            currWeapon = BrawlerWeapon.PowerPunch;
                            sb.ignited = false;
                            sb.burn = 0f;
                        }
                    }
                    catch (Exception exceptPostWeapEff)
                    {
                        Ebug(exceptPostWeapEff, "Post Weapon Usage Cooldown fail!");
                        throw exceptPostWeapEff;
                    }

                    // Spear post effects
                    try
                    {
                        if (meleeWeapon.Peek() is Spear)
                        {
                            meleeWeapon.Peek().doNotTumbleAtLowSpeed = shankSpearTumbler;
                            self.slowMovementStun += 20;
                        }
                    }
                    catch (Exception exceptPostSpearEff)
                    {
                        Ebug(exceptPostSpearEff, "Post Spear Usage Thing Fail!");
                        throw exceptPostSpearEff;
                    }

                    // Item retrieval effect
                    try
                    {
                        if (self.room != null && meleeWeapon.Peek().mode != Weapon.Mode.StuckInWall)
                        {
                            meleeWeapon.Peek().ChangeMode(Weapon.Mode.Free);
                            self.SlugcatGrab(meleeWeapon.Pop(), throwUsed);
                        }
                        else
                        {
                            meleeWeapon.Pop();
                        }
                    }
                    catch (Exception exceptGetBackItem)
                    {
                        Ebug(exceptGetBackItem, "Item retrieval fail!");
                        throw exceptGetBackItem;
                    }

                    if (e.isChunko)
                    {
                        self.slowMovementStun += (int)(10 * self.TotalMass / e.originalMass) - 10;
                    }
                    setCooldown = self.slowMovementStun;
                    throwGrab = -1;
                    throwUsed = -1;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Something went wrong when applying effect for melee weapon!");
            }

            if (meleeWeapon.Count == 0)
            {
                var leftHand = WeaponFromGrasp(self, 0);
                var rightHand = WeaponFromGrasp(self, 1);
                lastWeapon = (leftHand, rightHand) switch
                {
                    (BrawlerWeapon.SuperShank, BrawlerWeapon.SuperShank) => BrawlerWeapon.SuperShank,
                    (BrawlerWeapon.NONE, not BrawlerWeapon.NONE) => rightHand,
                    (not BrawlerWeapon.NONE, BrawlerWeapon.NONE) => leftHand,
                    (_, _) => BrawlerWeapon.NONE
                };
            }

            // Brawler wall spear
            if (wallSpear.Count > 0 && revertWall == 0)
            {
                wallSpear.Pop().doNotTumbleAtLowSpeed = wall;
                revertWall = -1;
            }

            // VFX
            if (self.room != null && throwGrab > 0 && meleeWeapon.Count > 0)
            {
                for (int i = -4; i < 5; i++)
                {
                    self.room.AddObject(new Spark(self.mainBodyChunk.pos + new Vector2(self.mainBodyChunk.Rotation.x * 5f, i * 0.5f), new Vector2(self.mainBodyChunk.Rotation.x * (10f - throwGrab) * (3f - (0.4f * Mathf.Abs(i))), throwGrab * 0.5f), new Color(0.8f, 0.4f, 0.6f), null, 4, 6));
                }
            }
        }

        public override bool HeavyCarry(Player self, PhysicalObject obj)
        {
            if (obj.TotalMass <= self.TotalMass * ratioed * 2 && obj is Creature)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (self.grasps[i] != null && self.grasps[i].grabbed != obj && self.grasps[i].grabbed is Spear && self.grasps[1 - i] != null && self.grasps[1 - i].grabbed == obj)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override Player.ObjectGrabability Grabability(Player.ObjectGrabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Creature c && !c.dead)
            {
                if (obj is JetFish || obj is Fly || obj is TubeWorm || obj is Cicada || obj is MoreSlugcats.Yeek || (obj is Player && obj == self))
                {
                    return base.Grabability(orig, self, obj);
                }
                if (c.Stunned && c is Lizard && Esconfig_Dunkin() && data.lizardDunk is not null)
                {
                    if (e.LizGoForWalk == 0)
                    {
                        e.LizGoForWalk = 320;
                    }
                    e.LizardDunk = true;
                }
                return Player.ObjectGrabability.BigOneHand;
            }
            return base.Grabability(orig, self, obj);
        }

        public override bool LegalToGrab(bool orig, Player self, Creature grabCheck)
        {
            if (grabCheck is Overseer or PoleMimic or TempleGuard or Deer or DaddyLongLegs or Leech or TentaclePlant or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish)
            {
                return base.LegalToGrab(orig, self, grabCheck);
            }

            return grabCheck.TotalMass <= self.TotalMass * ratioed * 2;
        }

        public void ThrowSpear(Player self, Spear spear, ref float thrust)
        {
            if (
                !brawlerSpearVelFac.TryGet(self, out float[] bSpearVel) ||
                !brawlerSpearDmgFac.TryGet(self, out float[] bSpearDmg) ||
                //!brawlerSpearThrust.TryGet(self, out float bSpearThr) ||
                !brawlerSpearShankY.TryGet(self, out float[] bSpearY)
            )
            {
                return;
            }
            try
            {
                if (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection)
                {

                }
                else
                {
                    spear.spearDamageBonus *= bSpearDmg[0];
                    if (self.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        spear.firstChunk.vel.x *= bSpearVel[0];
                    }
                    else if (self.bodyMode == Player.BodyModeIndex.Stand)
                    {
                        spear.firstChunk.vel.x *= bSpearVel[1];
                    }
                    else
                    {
                        spear.firstChunk.vel.x *= bSpearVel[2];
                    }
                }
                if (self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump)
                {
                    thrust *= 2;
                }
                else
                {
                    thrust *= 0.4f;
                }
                if (currWeapon == BrawlerWeapon.SuperShank)
                {
                    //spear.throwDir = new RWCustom.IntVector2(0, -1);
                    spear.firstChunk.pos = shankDir;
                    //spear.firstChunk.vel.y = -(Math.Abs(spear.firstChunk.vel.y)) * bSpearY[0];
                    //spear.firstChunk.pos += new Vector2(0f, bSpearY[1]);
                    spear.firstChunk.vel *= bSpearY[0];
                    //spear.doNotTumbleAtLowSpeed = true;
                    if (lastWeapon == BrawlerWeapon.SuperShank)
                    {
                        spear.firstChunk.vel.x *= 0.15f;
                        spear.doNotTumbleAtLowSpeed = true;
                    }
                    currWeapon = BrawlerWeapon.NONE;
                    spear.spearDamageBonus = bSpearDmg[1];
                    spear.spearDamageBonus *= Mathf.Max(0.15f, Mathf.InverseLerp(0, 20, 20 - self.slowMovementStun));
                }
                else
                {
                    if (currWeapon == BrawlerWeapon.Shank)
                    {
                        currWeapon = BrawlerWeapon.NONE;
                    }
                    if (wallSpear.Count > 0)
                    {
                        wallSpear.Pop().doNotTumbleAtLowSpeed = wall;
                    }
                    wall = spear.doNotTumbleAtLowSpeed;
                    revertWall = 4;
                    wallSpear.Push(spear);
                    spear.doNotTumbleAtLowSpeed = true;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error while applying Brawler-specific speartoss");
            }
        }

        public void ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            if (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection)
            {
                orig(self, grasp, eu);
                return;
            }

            for (int j = 0; j < 2; j++)
            {
                // Shank
                if (
                    self.grasps[j] != null &&
                    self.grasps[j].grabbed != null &&
                    self.grasps[j].grabbed is Spear s &&
                    self.grasps[j].grabbed is not ExplosiveSpear &&
                    self.grasps[1 - j] != null &&
                    self.grasps[1 - j].grabbed != null &&
                    self.grasps[1 - j].grabbed is Creature cs
                )
                {
                    if (cs.dead || cs.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || cs.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && cs is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
                    {
                        break;
                    }
                    if (self.slowMovementStun > 30)
                    {
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.4f);
                        return;
                    }
                    Creature c = cs;
                    //c.firstChunk.vel.y += 1f;
                    orig(self, 1 - j, eu);
                    c.mainBodyChunk.vel *= 0.5f;
                    //s.alwaysStickInWalls = false;
                    //if (c.mainBodyChunk != null){
                    //    s.meleeHitChunk = c.mainBodyChunk;
                    //}

                    //s.firstChunk.pos = self.mainBodyChunk.pos + new Vector2(0f, 80f);
                    //s.firstChunk.vel = new Vector2(c.mainBodyChunk.pos.x - s.firstChunk.pos.x, c.mainBodyChunk.pos.y - s.firstChunk.pos.y - 5f);
                    //s.firstChunk.pos = c.mainBodyChunk.pos;
                    //Vector2 v = (c.firstChunk.pos - s.firstChunk.pos).normalized * 3f;
                    shankDir = c.mainBodyChunk.pos;
                    Ebug(self, "Hey " + cs.GetType() + ", Like a cuppa tea? Well it's a mugging now.");
                    currWeapon = BrawlerWeapon.SuperShank;
                    if (meleeWeapon.Count > 0)
                    {
                        meleeWeapon.Pop().doNotTumbleAtLowSpeed = shankSpearTumbler;
                    }
                    shankSpearTumbler = s.doNotTumbleAtLowSpeed;
                    meleeWeapon.Push(s);
                    throwGrab = 5;
                    throwUsed = j;
                    lastWeapon = BrawlerWeapon.SuperShank;
                    s.doNotTumbleAtLowSpeed = true;
                    orig(self, j, eu);
                    //self.SlugcatGrab(s, j);
                    return;
                }
                // Alternate Shank
                else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Spear sa && self.grasps[j].grabbed is not ExplosiveSpear)
                {
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
                    {
                        continue;
                    }
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.2f);
                        return;
                    }
                    Ebug(self, "SHANK!");
                    currWeapon = BrawlerWeapon.Shank;
                    shankDir = sa.firstChunk.pos;
                    if (meleeWeapon.Count > 0)
                    {
                        meleeWeapon.Pop().doNotTumbleAtLowSpeed = shankSpearTumbler;
                    }
                    shankSpearTumbler = sa.doNotTumbleAtLowSpeed;
                    meleeWeapon.Push(sa);
                    throwGrab = 5;
                    throwUsed = j;
                    lastWeapon = BrawlerWeapon.Shank;
                    sa.doNotTumbleAtLowSpeed = true;
                    orig(self, j, eu);
                    return;
                }
                // Punch
                else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Rock r)
                {
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
                    {
                        continue;
                    }
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Creature)
                    {
                        break;
                    }
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to punch!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1f);
                        return;
                    }
                    Ebug(self, "PUNCH!");
                    currWeapon = BrawlerWeapon.Punch;
                    meleeWeapon.Push(r);
                    throwGrab = 4;
                    throwUsed = j;
                    currWeapon = BrawlerWeapon.Punch;
                    orig(self, j, eu);
                    return;
                }
                // Explosive punch
                else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is ScavengerBomb sb)
                {
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
                    {
                        continue;
                    }
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Creature)
                    {
                        break;
                    }
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to explopunch!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1f);
                        return;
                    }
                    try
                    {
                        Ebug(self, "EXPLOPUNCH!");
                        currWeapon = BrawlerWeapon.PowerPunch;
                        meleeWeapon.Push(sb);
                        throwGrab = 4;
                        throwUsed = j;
                        lastWeapon = BrawlerWeapon.PowerPunch;
                        orig(self, j, eu);
                    }
                    catch (Exception exploerr)
                    {
                        Ebug(exploerr, "Failure to explopunch!");
                    }
                    return;
                }
            }
            orig(self, grasp, eu);
        }

        public void OnRockThrow(Player self)
        {
            if (!brawlerRockHeight.TryGet(self, out float roH))
            {
                return;
            }
            self.firstChunk.vel.y *= roH;
        }
    }
}
