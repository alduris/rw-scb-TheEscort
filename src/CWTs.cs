using System.Runtime.CompilerServices;
using TheEscort.Escort;

namespace TheEscort
{
    public static class CWTs
    {

        private static ConditionalWeakTable<Player, EscortData> escortCWT = new();

        public static bool TryGetEscortClass(this Player self, out EscortData escort)
        {
            escort = null;
            if (escortCWT.TryGetValue(self, out var value))
            {
                escort = value;
                return true;
            }
            if (IsEscort(self))
            {
                #warning replace me
                escortCWT.Add(self, new EscortData());
                return true;
            }
            return false;
        }

        public static bool IsEscort(this Player self)
        {
            return self.slugcatStats.name == Enums.SlugcatNames.EscortMe;
        }
    }
}
