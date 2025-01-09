namespace TheEscort.Escort.Builds
{
    public abstract class EscortClass
    {
        // Common
        public abstract void Tick(Player self);
        public abstract void Update(Player self);

        // Grab stuff
        public virtual void GrabUpdate(Player self, bool eu) { }
        public virtual bool HeavyCarry(Player self, PhysicalObject obj) => false;
        public virtual Player.ObjectGrabability Grabability(Player.ObjectGrabability orig, Player self, PhysicalObject obj) => orig;
        public virtual bool LegalToGrab(bool orig, Player self, Creature grabCheck) => orig;

        // Violence
        public virtual void ThrowSpear(Player self, Spear spear, ref float thrust) { }
        public virtual void ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu) => orig(self, grasp, eu);

        // Debug
        public virtual void DebugPrint(Player self)
        {
            // TODO:
        }
    }
}
