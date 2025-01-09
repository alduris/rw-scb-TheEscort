using UnityEngine;

namespace TheEscort.Escort.Builds
{
    public abstract class EscortClass
    {
        public EscortData data;

        // Common
        public abstract Color DefaultColor { get; }
        public abstract void Tick(Player self);
        public abstract void Update(Player self);

        // Grab stuff
        public virtual void GrabUpdate(Player self, bool eu) { }
        public virtual bool HeavyCarry(Player self, PhysicalObject obj) => false;
        public virtual Player.ObjectGrabability Grabability(Player.ObjectGrabability orig, Player self, PhysicalObject obj) => orig;
        public virtual bool LegalToGrab(bool orig, Player self, Creature grabCheck) => orig;

        // Misc updating
        public virtual void UpdateAnimation(Player self) { }
        public virtual void UpdateSaveState(ShelterDoor self, int playerNumber, bool success) { }

        // Debug
        public virtual void DebugPrint(Player self)
        {
            // TODO:
        }
    }
}
