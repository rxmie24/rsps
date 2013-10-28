namespace RS2.Server.minigames.warriorguild
{
    internal class WarriorGuildData
    {
        public WarriorGuildData()
        {
        }

        protected static int ANIMATOR_ID = 15621;

        protected static int[][] ANIMATOR_LOCATIONS = {
		    new int[] {2851, 3536}, //x, y - western animator
		    new int[] {2857, 3536}, //x, y - eastern animator
	    };

        protected static int[][] ARMOUR_SETS = {
		    // helm, body, legs..from bronze to rune
		    new int[] {1155, 1117, 1075}, // bronze
		    new int[] {1153, 1115, 1067}, // iron
		    new int[] {1157, 1119, 1069}, // steel
		    new int[] {1165, 1125, 1077}, // black
		    new int[] {1159, 1121, 1071}, // mithril
		    new int[] {1161, 1123, 1073}, // adamant
		    new int[] {1163, 1127, 1079}, // rune
	    };

        /*
         * Id's of the animated armour NPCs, from bronze to rune.
         */

        protected static int[] ANIMATED_ARMOUR = {
		    4278, // bronze
		    4279, // iron
		    4280, // steel
		    4281, // black
		    4282, // mithril
		    4283, // adamant
		    4284, // rune
	    };

        protected static string[] ARMOUR_TYPE = {
		    "Bronze",
		    "Iron",
		    "Steel",
		    "Black",
		    "Mithril",
		    "Adamant",
		    "Rune"
	    };

        protected static int[] TOKEN_AMOUNT = {
		    5, // bronze
		    10, // iron
		    15, // steel
		    20, // black
		    25, // mithril
		    30, // adamant
		    40, // rune
	    };

        public static int[] DEFENDERS = {
		    8844, // Bronze defender
		    8845, // Iron defender
		    8846, // Steel defender
		    8847, // Black defender
		    8848, // Mithril defender
		    8849, // Adamant defender
		    8850, // Rune defender
	    };
    }
}