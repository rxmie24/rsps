namespace RS2.Server.player.skills.woodcutting
{
    internal class WoodcuttingData
    {
        public WoodcuttingData()
        {
        }

        protected static int WOODCUTTING = 8;

        protected static int[] AXES = {
		    1351, // Bronze
		    1349, // Iron
		    1353, // Steel
		    1361, // Black
		    1355, // Mithril
		    1357, // Adamant
		    1359, // Rune
		    6739, // Dragon
		    13661, // Inferno adze
	    };

        protected static int[] AXE_LVL = {
		    1, // Bronze
		    1, // Iron
		    6, // Steel
		    6, // Black
		    21, // Mithril
		    31, // Adamant
		    41, // Rune
		    61, // Dragon
		    61, // Inferno adze
	    };

        protected static int[] AXE_ANIMATION = {
		    879, // Bronze
		    877, // Iron
		    875, // Steel
		    873, // Black
		    871, // Mithril
		    869, // Adamant
		    867, // Rune
		    2846, // Dragon
		    10251, // Inferno adze
	    };

        public static int[] AXE_DELAY = {
		    1000, // Bronze
		    1500, // Iron
		    2000, // Steel
		    2500, // Black
		    3000, // Mithril
		    3500, // Adamant
		    4000, // Rune
		    4500, // Dragon
		    5000, // Inferno adze
	    };

        /*
         * CHANGE THESE FOR FARMING TOO!
         */

        protected static int[] TREE_DELAY = {
		    4000, // Normal logs
		    5500, // Oak logs
		    6000, // Willow logs
		    7000, // Achey logs
		    6500, // Teak logs
		    7250, // Maple logs
		    6000, // Hollow bark
		    7500, // Mahogany logs
		    4000, // Arctic pine logs
		    7000, // Eucalyptus logs
		    7000, // Yew logs
		    11000, // Magic logs
	    };

        protected static int[] TREES = {
		    1276, // Normal tree
		    1278, // Normal tree
		    2409, // Normal tree
		    1277, // Normal tree with but different coloured stump
		    3034, // Normal tree with dark stump -- TODO correct stump
		    3033, // Normal tree with dark stump -- TODO correct stump
		    10041, // Normal tree
		    1282, // Dead tree
		    1283, // Dead tree
		    1284, // Dead tree
		    1285, // Dead tree
		    1286, // Dead tree
		    1289, // Dead tree
		    1290, // Dead tree
		    1365, // Dead tree
		    1383, // Dead tree
		    1384, // Dead tree
		    1291, // Dead tree
		    3035, // Dead tree
		    3036, // Dead tree
		    1315, // Evergreen
		    1316, // Evergreen
		    1318, // Snowy Evergreen
		    1319, // Snowy Evergreen
		    1330, // Snow covered tree
		    1331, // Snow covered tree
		    1332, // Snow covered tree
		    3879, // Evergreen from elf land
		    3881, // Evergreen from elf land (slightly bigger than one above)
		    3882, // Evergreen from elf land (slightly bigger than one above)
		    3883, // Small Evergreen from elf land
		    1281, // Normal Oak tree
		    3037, // Oak tree dark stump
		    1308, // Normal Willow tree
		    5551, // Normal Willow tree
		    5552, // Normal Willow tree
		    5553, // Normal Willow tree
		    2023, // Normal Achey tree
		    9036, // Normal Teak tree
		    15062, // Normal Teak tree (same as above?)
		    1307, // Normal Maple tree
		    4674, // Exactly same as above
		    2289, // Normal Hollow tree
		    4060, // Normal Hollow tree (bigger than above)
		    9034, // Normal Mahogany tree
		    4674, // Exactly same as above
		    1280, // Normal tree
		    24168, // Dying tree in Varrock
		    21273, // Arctic pine
		    28951, // Normal Eucalyptus tree
		    28952, // Normal Eucalyptus tree (smaller)
		    28953, // Normal Eucalyptus tree (smallest)
		    1309, // Normal Yew tree
		    1306, // Normal Magic tree
		    14309, //PC game island tree - TODO stump
	    };

        protected static int[] NORMAL_TREES = {
		    1276, // Normal tree
		    1278, // Normal tree
		    2409, // Normal tree
		    1280, // Normal tree
		    1277, // Normal tree with but different coloured stump
		    3034, // Normal tree with dark stump
		    3033, // Normal tree with dark stump
		    10041, // Normal tree
		    24168, // Dying tree in Varrock
		    1282, // Dead tree
		    1283, // Dead tree
		    1284, // Dead tree
		    1285, // Dead tree
		    1286, // Dead tree
		    1289, // Dead tree
		    1290, // Dead tree
		    1365, // Dead tree
		    1383, // Dead tree
		    1384, // Dead tree
		    1291, // Dead tree
		    3035, // Dead tree
		    3036, // Dead tree
		    1315, // Evergreen
		    1316, // Evergreen
		    1318, // Snowy Evergreen
		    1319, // Snowy Evergreen
		    1330, // Snow covered tree
		    1331, // Snow covered tree
		    1332, // Snow covered tree
		    3879, // Evergreen from elf land
		    3881, // Evergreen from elf land (slightly bigger than one above)
		    3882, // Evergreen from elf land (slightly bigger than one above)
		    3883, // Small Evergreen from elf land
		    14309, // Tree on PC game island
		    21273, // Arctic pine
	    };

        protected static int[] OAK_TREES = {
		    1281, // Normal Oak tree
		    3037, // Oak tree dark stump
	    };

        protected static int[] WILLOW_TREES = {
		    1308, // Normal Willow tree
		    5551, // Normal Willow tree
		    5552, // Normal Willow tree
		    5553, // Normal Willow tree
	    };

        protected static int[] ACHEY_TREES = {
		    2023, // Normal Achey tree
	    };

        protected static int[] TEAK_TREES = {
		    9036, // Normal Teak tree
		    15062, // Normal Teak tree (same as above?)
	    };

        protected static int[] MAPLE_TREES = {
		    1307, // Normal Maple tree
		    4674 // Exactly same as above
	    };

        protected static int[] HOLLOW_TREES = {
		    2289, // Normal Hollow tree
		    4060 // Normal Hollow tree (bigger than above)
	    };

        protected static int[] MAHOGANY_TREES = {
		    9034, // Normal Mahogany tree
	    };

        protected static int[] ARCTIC_PINE = {
		    21273, // Normal Arctic pine
	    };

        protected static int[] EUCALYPTUS = {
		    28951, // Normal Eucalyptus tree
		    28952, // Normal Eucalyptus tree (smaller)
		    28953, // Normal Eucalyptus tree (smallest)
	    };

        protected static int[] YEW_TREES = {
		    1309, // Normal Yew tree
	    };

        protected static int[] MAGIC_TREES = {
		    1306, // Normal Magic tree
	    };

        protected static int[] LOGS = {
		    1511, // Normal logs
		    1521, // Oak logs
		    1519, // Willow logs
		    2862, // Achey logs
		    6333, // Teak logs
		    1517, // Maple logs
		    3239, // Hollow bark
		    6332, // Mahogany logs
		    10810, // Arctic pine logs
		    12581, // Eucalyptus logs
		    1515, // Yew logs
		    1513, // Magic logs
	    };

        /*
         * CHANGE FARMING CUT XP TOO!
         */

        protected static double[] XP = {
		    25, // Normal logs
		    37.5, // Oak logs
		    67.5, // Willow logs
		    25, // Achey logs
		    85, // Teak logs
		    100, // Maple logs
		    82.5, // Hollow bark
		    125, // Mahogany logs
		    140.2, // Arctic pine logs
		    165, // Eucalyptus logs
		    175, // Yew logs
		    1250, // Magic logs
	    };

        protected static int[] LEVEL = {
		    1, // Normal logs
		    15, // Oak logs
		    30, // Willow logs
		    1, // Achey logs
		    35, // Teak logs
		    45, // Maple logs
		    45, // Hollow bark
		    50, // Mahogany logs
		    54, // Arctic pine logs
		    58, // Eucalyptus logs
		    60, // Yew logs
		    75, // Magic logs
	    };

        protected static string[] TREE_NAME = {
		    "Normal", // Normal logs
		    "Oak", // Oak logs
		    "Willow", // Willow logs
		    "Achey", // Achey logs
		    "Teak", // Teak logs
		    "Maple", // Maple logs
		    "Bark", // Hollow bark
		    "Mahogany", // Mahogany logs
		    "Arctic pine", // Arctic pine logs
		    "Eucalyptus", // Eucalyptus logs
		    "Yew", // Yew logs
		    "Magic", // Magic logs
	    };

        protected static int[] TREE_SIZE = {
		    2, // Normal tree
		    2, // Normal tree
		    2, // Normal tree
		    2, // Normal tree with but different coloured stump
		    2, // Normal tree with dark stump -- TODO correct stump
		    2, // Normal tree with dark stump -- TODO correct stump
		    2, // Normal tree
		    2, // Dead tree
		    2, // Dead tree
		    2, // Dead tree
		    2, // Dead tree
		    1, // Dead tree
		    1, // Dead tree
		    2, // Dead tree
		    1, // Dead tree
		    1, // Dead tree
		    1, // Dead tree
		    1, // Dead tree
		    2, // Dead tree
		    2, // Dead tree
		    2, // Evergreen
		    3, // Evergreen
		    3, // Snowy Evergreen
		    3, // Snowy Evergreen
		    3, // Snow covered tree
		    3, // Snow covered tree
		    3, // Snow covered tree
		    3, // Evergreen from elf land
		    3, // Evergreen from elf land (slightly bigger than one above)
		    3, // Evergreen from elf land (slightly bigger than one above)
		    2, // Small Evergreen from elf land
		    3, // Normal Oak tree
		    3, // Oak tree dark stump
		    2, // Normal Willow tree
		    2, // Normal Willow tree
		    2, // Normal Willow tree
		    2, // Normal Willow tree
		    2, // Normal Achey tree
		    1, // Normal Teak tree
		    1, // Normal Teak tree (same as above?)
		    2, // Normal Maple tree
		    2, // Exactly same as above
		    1, // Normal Hollow tree
		    2, // Normal Hollow tree (bigger than above)
		    3, // Normal Mahogany tree
		    2, // Exactly same as above
		    2, // Normal tree
		    1, // Dying tree in Varrock
		    2, // Arctic pine
		    4, // Normal Eucalyptus tree
		    3, // Normal Eucalyptus tree (smaller)
		    2, // Normal Eucalyptus tree (smallest)
		    3, // Normal Yew tree
		    2, // Normal Magic tree
		    2, //PC game island tree
	    };

        protected static int[] NEST_SEEDS = {
		    5312, 5313, 5314, 5315, 5316
	    };

        protected static int[] NEST_JEWELLERY = {
		    1635, 1637, 1639, 1641, 1643
	    };
    }
}