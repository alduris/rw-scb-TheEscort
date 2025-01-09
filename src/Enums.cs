using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEscort
{
    public static class Enums
    {
        /*public class Build : ExtEnum<Build>
        {
            public static readonly Build Default = new("EscortMe");
            public static readonly Build Brawler = new("EscortBriish");
            public static readonly Build Deflector = new("EscortGamer");
            public static readonly Build Escapist = new("EscortHax");
            public static readonly Build Railgunner = new("EscortRizzgayer");
            public static readonly Build Speedster = new("EscortCheese");
            public static readonly Build Gilded = new("EscortDrip");
            public static readonly Build Blaster = new("EscortForce");
            public static readonly Build Barbarian = new("EscortProWrestler");

            public Build(string value, bool register = false) : base(value, register) { }
        }*/

        public static class SlugcatNames
        {
            public static SlugcatStats.Name EscortMe;
            public static SlugcatStats.Name EscortSocks;
            public static SlugcatStats.Name ShadowEscort;

            public static void Register()
            {
                EscortMe ??= new("EscortMe");
                EscortSocks ??= new("EscortSocks");
                ShadowEscort ??= new("EscortDummy", true);
            }

            public static void Unregister()
            {
                ShadowEscort?.Unregister();
                ShadowEscort ??= null;
            }
        }

        public static class Sounds
        {
            /// <summary>
            /// Urufu calls you a failure.
            /// </summary>
            public static SoundID Escort_SFX_Death;
            /// <summary>
            /// Urufu says "Sick Flip!"
            /// </summary>
            public static SoundID Escort_SFX_Flip;
            /// <summary>
            /// Urufu says "Cool Flip!"
            /// </summary>
            public static SoundID Escort_SFX_Flip2;
            /// <summary>
            /// Urufu says "Nice Flip!"
            /// </summary>
            public static SoundID Escort_SFX_Flip3;
            /// <summary>
            /// ...around and around and around and around and around and around and around and around and around and around and around and around and around and around and around...
            /// </summary>
            public static SoundID Escort_SFX_Roll;
            /// <summary>
            /// Uru says "Boop" in multiple pitches and inflections
            /// </summary>
            public static SoundID Escort_SFX_Boop;
            /// <summary>
            /// That Deltarune explosion sfx edited to sound extremely cheap, juuust like Rails.
            /// </summary>
            public static SoundID Escort_SFX_Railgunner_Death;
            /// <summary>
            /// Lizard, grab. Lizard, grab. Lizard, grab. Lizard, grab. Lizard,grab. Lizard, grab. Lizard, grab. Lizard, grab.
            /// </summary>
            public static SoundID Escort_SFX_Lizard_Grab;
            /// <summary>
            /// A thump made by a kick drum
            /// </summary>
            public static SoundID Escort_SFX_Impact;
            /// <summary>
            /// A high pitch bell hit
            /// </summary>
            public static SoundID Escort_SFX_Parry;
            /// <summary>
            /// A metal knife rubbing against another knife
            /// </summary>
            public static SoundID Escort_SFX_Brawler_Shank;
            /// <summary>
            /// A distorted clap
            /// </summary>
            public static SoundID Escort_SFX_Pole_Bounce;
            /// <summary>
            /// Urufu calls you fat (Rotund World exclusive!)
            /// </summary>
            public static SoundID Escort_SFX_Uhoh_Big;
            /// <summary>
            /// Urufu is being real sneaky
            /// </summary>
            public static SoundID Esconfig_SFX_Sectret;
            /// <summary>
            /// Literal SILENCE
            /// </summary>
            public static SoundID Escort_SFX_Placeholder;
            /// <summary>
            /// Bass pluck reversed (inspired by Bearhugger's incoming punch sfx from Punch-Out Wii)
            /// </summary>
            public static SoundID Escort_SFX_Gild_Stomp;

            //(Urufu announces your spawn for 2023 April Fools)
            // public static SoundID Escort_SFX_Spawn;

            public static void Register()
            {
                Escort_SFX_Death ??= new SoundID("Escort_Failure", true);
                Escort_SFX_Flip ??= new SoundID("Escort_Flip", true);
                Escort_SFX_Roll ??= new SoundID("Escort_Roll", true);
                Escort_SFX_Boop ??= new SoundID("Escort_Boop", true);
                Escort_SFX_Railgunner_Death ??= new SoundID("Escort_Rail_Fail", true);
                Escort_SFX_Lizard_Grab ??= new SoundID("Escort_Liz_Grab", true);
                Escort_SFX_Impact ??= new SoundID("Escort_Impact", true);
                Escort_SFX_Parry ??= new SoundID("Escort_Parry", true);
                Escort_SFX_Flip2 ??= new SoundID("Escort_Flip_More", true);
                Escort_SFX_Flip3 ??= new SoundID("Escort_Flip_Even_More", true);
                Escort_SFX_Brawler_Shank ??= new SoundID("Escort_Brawl_Shank", true);
                Escort_SFX_Pole_Bounce ??= new SoundID("Escort_Pole_Bounce", true);
                Escort_SFX_Uhoh_Big ??= new SoundID("Escort_Rotunded", true);
                Esconfig_SFX_Sectret ??= new SoundID("Esconfig_Sectret", true);
                Escort_SFX_Placeholder ??= new SoundID("Esplaceholder", true);
                Escort_SFX_Gild_Stomp ??= new SoundID("Escort_Gild_Stomp", true);
                //Escort_SFX_Spawn ??= new SoundID("Escort_Spawn", true);
            }

            public static void Unregister()
            {
                Escort_SFX_Death?.Unregister();             Escort_SFX_Death = null;
                Escort_SFX_Flip?.Unregister();              Escort_SFX_Flip = null;
                Escort_SFX_Roll?.Unregister();              Escort_SFX_Roll = null;
                Escort_SFX_Boop?.Unregister();              Escort_SFX_Boop = null;
                Escort_SFX_Railgunner_Death?.Unregister();  Escort_SFX_Railgunner_Death = null;
                Escort_SFX_Lizard_Grab?.Unregister();       Escort_SFX_Lizard_Grab = null;
                Escort_SFX_Impact?.Unregister();            Escort_SFX_Impact = null;
                Escort_SFX_Parry?.Unregister();             Escort_SFX_Parry = null;
                Escort_SFX_Flip2?.Unregister();             Escort_SFX_Flip2 = null;
                Escort_SFX_Flip3?.Unregister();             Escort_SFX_Flip3 = null;
                Escort_SFX_Brawler_Shank?.Unregister();     Escort_SFX_Brawler_Shank = null;
                Escort_SFX_Pole_Bounce?.Unregister();       Escort_SFX_Pole_Bounce = null;
                Escort_SFX_Uhoh_Big?.Unregister();          Escort_SFX_Uhoh_Big = null;
                Esconfig_SFX_Sectret?.Unregister();         Esconfig_SFX_Sectret = null;
                Escort_SFX_Placeholder?.Unregister();       Escort_SFX_Placeholder = null;
                Escort_SFX_Gild_Stomp?.Unregister();        Escort_SFX_Gild_Stomp = null;
                //Escort_SFX_Spawn?.Unregister();             Escort_SFX_Spawn = null;
            }
        }
    }
}
