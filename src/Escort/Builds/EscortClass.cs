namespace TheEscort.Escort.Builds
{
    public abstract class EscortClass
    {
        public abstract void Tick(Player self);

        public abstract void Update(Player self);

        public virtual void GrabUpdate(Player self, bool eu) { }

        public virtual void DebugPrint(Player self)
        {
            //
        }
    }
}
