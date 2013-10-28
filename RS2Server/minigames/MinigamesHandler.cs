using RS2.Server.minigames.agilityarena;
using RS2.Server.minigames.fightpits;

namespace RS2.Server.minigames
{
    internal class MinigamesHandler
    {
        private AgilityArena agilityArena;
        private FightPits fightPits;

        public MinigamesHandler()
        {
            this.agilityArena = new AgilityArena();
            this.fightPits = new FightPits();
        }

        public AgilityArena getAgilityArena()
        {
            return agilityArena;
        }

        public FightPits getFightPits()
        {
            return fightPits;
        }
    }
}