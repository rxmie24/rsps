using RS2.Server.minigames.barrows;
using RS2.Server.minigames.fightcave;
using RS2.Server.minigames.godwars;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.combat
{
    internal class NPCAttack
    {
        public NPCAttack()
        {
        }

        protected static int[] NPCS = {
		    2025, 2026, 2027, 2030, 2029, 2028, // Barrow brothers
		    6263, 6260, 6265, // Bandos room
		    6247, 6250, 6252, // Saradomin room
		    6203, 6208, 6206, // Zamorak room
		    6222, 6223, 6225, // Armadyl room
		    2734, 2735, // Level 22 Tzhaar
		    2739, 2740, // Level 90 Tzhaar
		    2741, 2742, // Level 180 Tzhaar
		    2743, 2744, // Level 360 Tzhaar
		    2745, // TzTok-Jad
	    };

        private const int DHAROK = 2026, AHRIM = 2025, TORAG = 2029, KARIL = 2028, VERAC = 2030, GUTHAN = 2027;

        public static bool npcAttack(Npc npc, Entity target)
        {
            if (npcHasAttack(npc))
            {
                doNpcAttack(npc, target);
                return true;
            }
            return false;
        }

        private static void doNpcAttack(Npc npc, Entity target)
        {
            switch (npc.getId())
            {
                case DHAROK:
                case AHRIM:
                case TORAG:
                case VERAC:
                case KARIL:
                case GUTHAN:
                    BarrowNPCAttacks.attack(npc, target);
                    break;

                case 6263:
                case 6260:
                case 6265:
                case 6247:
                case 6250:
                case 6252:
                case 6203:
                case 6208:
                case 6206:
                case 6222:
                case 6223:
                case 6225:
                    GodwarsAttacks.attack(npc, target);
                    break;

                case 2734:
                case 2735:
                case 2739:
                case 2740:
                case 2741:
                case 2742:
                case 2743:
                case 2744:
                case 2745:
                    FightCave.fightCaveAttacks(npc, ((Player)target));
                    break;
            }
        }

        private static bool npcHasAttack(Npc npc)
        {
            for (int i = 0; i < NPCS.Length; i++)
            {
                if (npc.getId() == NPCS[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}