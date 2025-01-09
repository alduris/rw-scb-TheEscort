using System;
using Menu;
using MoreSlugcats;
using Newtonsoft.Json;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort.Escort.Builds
{
    public class Deflector : EscortClass, IAdjustDamage
    {
        public static readonly PlayerFeature<float> deflectorSlideDmg = PlayerFloat("theescort/deflector/slide_dmg");
        public static readonly PlayerFeature<float> deflectorSlideLaunchFac = PlayerFloat("theescort/deflector/slide_launch_fac");
        public static readonly PlayerFeature<float> deflectorSlideLaunchMod = PlayerFloat("theescort/deflector/slide_launch_mod");
        public static readonly PlayerFeature<float[]> deflectorDKHypeDmg = PlayerFloats("theescort/deflector/dk_h_dmg");
        public static readonly PlayerFeature<float[]> deflectorSpearVelFac = PlayerFloats("theescort/deflector/spear_vel_fac");
        public static readonly PlayerFeature<float[]> deflectorSpearDmgFac = PlayerFloats("theescort/deflector/spear_dmg_fac");

        public static float InitSharedPerma;
        public static float SharedPermaValue = InitSharedPerma;

        public override Color DefaultColor => new (0.23f, 0.24f, 0.573f);
        public readonly Color EmpoweredColor = new Color(0.69f, 0.55f, 0.9f); // new Color(1f, 0.7f, 0.35f, 0.7f)


        /// <summary>
        /// Deflector empowered timer
        /// </summary>
        private int ampTimer;

        /// <summary>
        /// Deflector easy parry (backflip onto creature)
        /// </summary>
        private bool trampoline;

        /// <summary>
        /// Deflector parry sfx cooldown (so it doesn't make the sound like 10 times per parry)
        /// </summary>
        private int SFXcd;

        /// <summary>
        /// Super extended belly slide accumulator
        /// </summary>
        private int slideCom;

        /// <summary>
        /// Gives a micro boost when slide-pouncing
        /// </summary>
        private bool slideKick;

        /// <summary>
        /// Level of empowerment
        /// </summary>
        private int powah;

        /// <summary>
        /// Deflector permanent damage multiplier (per player or from pool)
        /// </summary>
        private float perma;

        /// <summary>
        /// Empowered damage based on level
        /// </summary>
        public float damageMult
        {
            get
            {
                return powah switch
                {
                    3 => 1000000f,
                    2 => 7f,
                    1 => 3f,
                    _ => 0.5f
                };
            }
        }

        public override void Tick(Player self)
        {
            // Increased damage when parry tick
            if (ampTimer > 0)
            {
                ampTimer--;
            }
            else
            {
                powah = 0;
            }

            // Sound FX cooldown
            if (SFXcd > 0)
            {
                SFXcd--;
            }

            if (self.rollCounter > 1)
            {
                slideCom++;
            }
            else
            {
                slideCom = 0;
            }

            if (self.room?.game?.session is ArenaGameSession ags)
            {
                perma = 0.01f * ags.ScoreOfPlayer(self, false);
            }
            else if (Plugin.config.cfgDeflecterSharedPool.Value)
            {
                perma = SharedPermaValue;
            }
        }

        public override void Update(Player self)
        {
            // VFX
            if (self != null && self.room != null)
            {
                if (ampTimer > 0)
                {
                    self.slugcatStats.runspeedFac = powah switch
                    {
                        3 => 1.4f,
                        2 => 1.3f,
                        _ => 1.2f,
                    };
                    self.slugcatStats.throwingSkill = powah > 0 ? 1 : 0;

                    // Empowered damage from parry visual effect
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    if (!Plugin.config.cfgNoticeEmpower.Value)
                    {
                        if (powah == 1 || powah == 3)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((ampTimer % 2 == 0 ? 10 : -10), 0), new Vector2(2f * (ampTimer % 2 == 0 ? 1 : -1), 0), EmpoweredColor, null, 9, 13));
                        }
                        if (powah == 2)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((ampTimer % 2 == 0 ? 10 : -10), 3), new Vector2(2f * (ampTimer % 2 == 0 ? 1 : -1), 0), EmpoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((ampTimer % 2 == 0 ? 10 : -10), -3), new Vector2(2f * (ampTimer % 2 == 0 ? 1 : -1), 0), EmpoweredColor, null, 9, 13));
                        }
                        if (powah == 3)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((ampTimer % 2 == 0 ? 10 : -10), 6), new Vector2(2f * (ampTimer % 2 == 0 ? 1 : -1), 0), EmpoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((ampTimer % 2 == 0 ? 10 : -10), -6), new Vector2(2f * (ampTimer % 2 == 0 ? 1 : -1), 0), EmpoweredColor, null, 9, 13));
                        }
                    }
                    else
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 15f, Mathf.InverseLerp(0, 20, ampTimer % 20)), 1.5f, 24f, 3.5f, EmpoweredColor * 1.5f));
                        if (powah <= 2)
                        {
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 23f, Mathf.InverseLerp(0, 20, ampTimer % 20)), 1.5f, 24f, 3.5f, EmpoweredColor * 1.5f));
                        }
                        if (powah <= 3)
                        {
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 31f, Mathf.InverseLerp(0, 20, ampTimer % 20)), 1.5f, 24f, 3.5f, EmpoweredColor * 1.5f));
                        }
                    }
                }
            }
        }

        public bool StickySpear(Player self)
        {
            return !(
                self.animation == Player.AnimationIndex.BellySlide ||
                self.animation == Player.AnimationIndex.Roll ||
                self.animation == Player.AnimationIndex.Flip
            );
        }

        public override void UpdateAnimation(Player self)
        {
            if (self.animation == Player.AnimationIndex.BellySlide)
            {
                slideKick = true;
                if (self.rollCounter < 8)
                {
                    self.rollCounter += 9;
                }
                if (self.initSlideCounter < 3)
                {
                    self.initSlideCounter += 3;
                }
                int da = 32;
                int db = 18;
                if (Plugin.config.cfgFunnyDeflSlide.Value)
                {
                    da = 46;
                    db = 22;
                }
                if (slideCom < (self.longBellySlide ? da : db) && self.rollCounter > 12 && self.rollCounter < 15)
                {
                    self.rollCounter--;
                    //self.exitBellySlideCounter--;
                }
                self.mainBodyChunk.vel.x *= Mathf.Lerp(1.1f, (self.longBellySlide ? 1.33f : 1.3f), Mathf.InverseLerp(0, (self.longBellySlide ? 20 : 10), slideCom));
            }
            else if (slideKick && self.animation == Player.AnimationIndex.RocketJump)
            {
                self.mainBodyChunk.vel.x *= 1.4f;
                self.mainBodyChunk.vel.y *= 0.9f;
                slideKick = false;
            }
            else
            {
                slideKick = false;
            }
        }

        public void DamageIncrease(Player self, Creature victim)
        {
            int points = StoryGameStatisticsScreen.GetNonSandboxKillscore(victim.Template.type);
            if (points <= 0)
            {
                if (themCreatureScores is null) Expedition.ChallengeTools.GenerateCreatureScores(ref themCreatureScores);
                if (themCreatureScores.TryGetValue(victim.Template.type.value, out var score))
                {
                    points = score;
                }
            }
            perma += points * 0.001f;
            if (self.room?.abstractRoom is not null && self.room.abstractRoom.shelter)
            {
                data.shelterSaveComplete = 0;
            }

            if (self.room?.game.IsStorySession ?? false && Plugin.config.cfgDeflecterSharedPool.Value)
            {
                SharedPermaValue = perma;
            }
        }

        public override void UpdateSaveState(ShelterDoor self, int playerNumber, bool success)
        {
            if (success && self.room?.game?.session is StoryGameSession storyGameSession)
            {
                float bonusDamage = 0;
                if (storyGameSession.saveState.deathPersistentSaveData.karma == storyGameSession.saveState.deathPersistentSaveData.karmaCap)
                {
                    bonusDamage = storyGameSession.saveState.deathPersistentSaveData.karmaCap switch
                    {
                        9 => 0.05f,
                        8 => 0.03f,
                        7 => 0.02f,
                        6 => 0.01f,
                        4 => 0.004f,
                        3 => 0.002f,
                        _ => 0
                    };
                }
                storyGameSession.saveState.miscWorldSaveData.Esave().DeflPermaDamage[playerNumber] = perma + bonusDamage;
                if (data.shelterSaveComplete <= 2)
                {
                    Ebug("Misc: " + JsonConvert.SerializeObject(storyGameSession.saveState.miscWorldSaveData.Esave()), 1, true);
                }

            }
        }

        public virtual void OnWeaponHit(Player self, PhysicalObject weapon, Creature creature, ref float damage)
        {
            damage *= damageMult + perma;
            if (powah == 3) self.room.ScreenMovement(null, default, 1.2f);
            if (weapon is not Bullet)
            {
                powah = 0;
            }
        }
    }
}
