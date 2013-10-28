namespace RS2.Server.player.skills.mining
{
    internal class MiningData
    {
        public MiningData()
        {
        }

        protected static int[] ORES = {
		    7936, // Pure essence
		    434, // Clay
		    436, // Copper ore
		    438, // Tin ore
		    668, // Blurite ore
		    3211, // Limestone
		    440, //Iron ore
		    2892, //Elemental ore
		    442, // Silver ore
		    453, // Coal
		    444, // Gold ore
		    447, // Mithril ore
		    449, // Adamantite ore
		    451 // Runite ore
	    };

        protected static int[] ROCK_LEVEL = {
		    1, // Pure essence
		    1, // Clay
		    1, // Copper ore
		    1, // Tin ore
		    10, // Blurite ore
		    10, // Limestone
		    15, //Iron ore
		    20, //Elemental ore
		    20, // Silver ore
		    30, // Coal
		    40, // Gold ore
		    55, // Mithril ore
		    70, // Adamantite ore
		    85 // Runite ore
	    };

        protected static string[] NAME = {
		    "essence", // Pure essence
		    "clay", // Clay
		    "copper", // Copper ore
		    "tin", // Tin ore
		    "blurite", // Blurite ore
		    "limestone", // Limestone
		    "iron", //Iron ore
		    "elemental", //Elemental ore
		    "silver", // Silver ore
		    "coal", // Coal
		    "gold", // Gold ore
		    "mithril", // Mithril ore
		    "adamantite", // Adamantite ore
		    "runite" // Runite ore
	    };

        protected static double[] ROCK_XP = {
		    5, // Pure essence
		    5, // Clay
		    17.5, // Copper ore
		    17.5, // Tin ore
		    17.5, // Blurite ore
		    26.5, // Limestone
		    35, //Iron ore
		    0, //Elemental ore
		    40, // Silver ore
		    50, // Coal
		    65, // Gold ore
		    80, // Mithril ore
		    95, // Adamantite ore
		    125 // Runite ore
	    };

        protected static int[] PICKAXES = {
		    1265, // Bronze
		    1267, // Iron
		    1269, // Steel
		    1271, // Mithril
		    1273, // Adamant
		    1275  // Rune
	    };

        protected static int[] PICKAXE_ANIMS = {
		    625, // Bronze
		    626, // Iron
		    627, // Steel
		    629, // Mithril
		    628, // Adamant
		    624  // Rune
	    };

        protected static int[] PICKAXE_TIME = {
		    1000, // Bronze
		    1500, // Iron
		    2000, // Steel
		    2500, // Mithril
		    3000, // Adamant
		    3500  // Rune
	    };

        protected static int[] ROCK_TIME = {
		    1000, // Pure essence
		    2500, // Clay
		    3000, // Copper ore
		    3000, // Tin ore
		    5000, // Blurite ore
		    5000, // Limestone
		    3300, //Iron ore
		    5000, //Elemental ore
		    4500, // Silver ore
		    4000, // Coal
		    4500, // Gold ore
		    7000, // Mithril ore
		    8000, // Adamantite ore
		    10000 // Runite ore
	    };

        protected static int[] GEMS = {
		    1623, // Sapphire
		    1621, // Emerald
		    1619, // Ruby
		    1617, // Diamond
	    };

        protected static int[] GLORY_AMULETS = {
		    1704, // (0)
		    1706, // (1)
		    1708, // (2)
		    1710, // (3)
		    1712, // (4)
	    };
    }
}