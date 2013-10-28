namespace RS2.Server.player.skills.farming
{
    public class FarmingData
    {
        public FarmingData()
        {
        }

        protected static int FARMING = 19;

        public enum PatchType
        {
            VEGATABLE,
            VEGATABLE_1,
            HERB,
            FLOWER,
            TREE,
            FRUIT_TREE
        }

        protected static int[] TOOLS = {
		    5341, // Rake
		    5343, // Dibber
		    5325, // Trowl
		    952, // Spade
		    5356 // Plant pot filled with compost
	    };

        protected static object[][] SEEDS = {
		    //Patch type, seed id, crop id, time to grow (millis), plant XP, harvest XP, level, name, seed amount, fruit tree harvest xp
		    new object[]{PatchType.VEGATABLE, 5318, 1942, 14000L, 8.0, 9.0, 1, "potato", "a"}, // Potato
		    new object[]{PatchType.VEGATABLE, 5319, 1957, 2100000L, 9.5, 10.5, 5, "onion", "an"}, // Onion
		    new object[]{PatchType.VEGATABLE, 5324, 1965, 2100000L, 10.0, 11.5, 7, "cabbage", "a"}, // Cabbage
		    new object[]{PatchType.VEGATABLE, 5322, 1982, 2100000L, 12.5, 14.0, 12, "tomato", "a"}, // Tomato
		    new object[]{PatchType.VEGATABLE, 5320, 5986, 3300000L, 17.0, 19.0, 20, "sweetcorn", "a"}, // Sweetcorn
		    new object[]{PatchType.VEGATABLE, 5323, 5504, 3300000L, 26.0, 29.0, 31, "strawberry", "a"}, // Strawberry
		    new object[]{PatchType.VEGATABLE, 5321, 5982, 4500000L, 48.5, 54.5, 47, "watermelon", "a"}, // Watermelon
		    new object[]{PatchType.VEGATABLE_1, 5318, 1942, 14000L, 8.0, 9.0, 1, "potato", "a"}, // Potato
		    new object[]{PatchType.VEGATABLE_1, 5319, 1957, 2100000L, 9.5, 10.5, 5, "onion", "an"}, // Onion
		    new object[]{PatchType.VEGATABLE_1, 5324, 1965, 2100000L, 10.0, 11.5, 7, "cabbage", "a"}, // Cabbage
		    new object[]{PatchType.VEGATABLE_1, 5322, 1982, 2100000L, 12.5, 14.0, 12, "tomato", "a"}, // Tomato
		    new object[]{PatchType.VEGATABLE_1, 5320, 5986, 3300000L, 17.0, 19.0, 20, "sweetcorn", "a"}, // Sweetcorn
		    new object[]{PatchType.VEGATABLE_1, 5323, 5504, 3300000L, 26.0, 29.0, 31, "strawberry", "a"}, // Strawberry
		    new object[]{PatchType.VEGATABLE_1, 5321, 5982, 4500000L, 48.5, 54.5, 47, "watermelon", "a"}, // Watermelon
		    new object[]{PatchType.HERB, 5291, 199, 4500000L, 11.0, 12.5, 9, "guam", "a"}, // Guam
		    new object[]{PatchType.HERB, 5292, 201, 4500000L, 13.5, 15.0, 14, "marrentill", "a"}, // Marrentill
		    new object[]{PatchType.HERB, 5293, 203, 4500000L, 16.0, 18.0, 19, "tarromin", "a"}, // Tarromin
		    new object[]{PatchType.HERB, 5294, 205, 4500000L, 21.5, 24.0, 26, "harralander", "a"}, // Harralander
		    new object[]{PatchType.HERB, 5295, 207, 4500000L, 27.0, 30.5, 32, "ranarr", "a"}, // Ranarr
		    new object[]{PatchType.HERB, 5296, 3049, 4500000L, 34.0, 38.5, 38, "toadflax", "a"}, // Toadflax
		    new object[]{PatchType.HERB, 5297, 209, 4500000L, 43.0, 48.5, 44, "irit", "an"}, // Irit
		    new object[]{PatchType.HERB, 5298, 211, 4500000L, 54.5, 61.5, 50, "avantoe", "an"}, // Avantoe
		    new object[]{PatchType.HERB, 5299, 213, 4500000L, 69.0, 78.0, 56, "kwuarm", "a"}, // Kwuarm
		    new object[]{PatchType.HERB, 5300, 3051, 4500000L, 87.5, 98.5, 62, "snapdragon", "a"}, // Snapdragon
		    new object[]{PatchType.HERB, 5301, 215, 4500000L, 106.5, 120.0, 67, "cadantine", "a"}, // Cadantine
		    new object[]{PatchType.HERB, 5302, 2485, 4500000L, 134.5, 151.5, 73, "lantadyme", "a"}, // Lantadyme
		    new object[]{PatchType.HERB, 5303, 217, 4500000L, 170.5, 192.0, 79, "dwarf weed", "a"}, // Dwarf weed
		    new object[]{PatchType.HERB, 5304, 219, 4500000L, 199.5, 224.5, 85, "torstol", "a"}, // Torstol
		    new object[]{PatchType.FLOWER, 5096, 6010, 1050000L, 8.5, 47.0, 2, "marigold", "a"}, // Marigold
		    new object[]{PatchType.FLOWER, 5097, 6014, 1050000L, 12.0, 66.5, 11, "rosemary", "a"}, // Rosemary
		    new object[]{PatchType.FLOWER, 5098, 6012, 1050000L, 19.5, 111.0, 24, "nasturtium", "a"}, // Nasturtium
		    new object[]{PatchType.FLOWER, 5099, 5738, 1050000L, 20.5, 115.5, 25, "woad", "a"}, // Woad leaves
		    new object[]{PatchType.FLOWER, 5100, 225, 1050000L, 21.5, 120.0, 26, "limpwurt", "a"}, // Limpwurt
		    /*
		     * Now we use sapling id's instead of seed ids, last id is root id
		     * 10 is the level needed, 11 is the xp, 12 is the cut delay
		     */
		    new object[]{PatchType.TREE, 5370, 1521, 8400000L, 14.0, 467.3, 14, "oak", 6043, "an", 15, 37.5, 5500}, // Oak
		    new object[]{PatchType.TREE, 5371, 1519, 13200000L, 25.0, 1456.3, 30, "willow", 6045, "a", 30, 67.5, 6000}, // Willow
		    new object[]{PatchType.TREE, 5372, 1517, 18000000L, 45.0, 3403.4, 45, "maple", 6047, "a", 45, 100.0, 7250}, // Maple
		    new object[]{PatchType.TREE, 5373, 1515, 22800000L, 81.0, 7069.9, 60, "yew", 6049, "a", 60, 175.0, 7000}, // Yew
		    new object[]{PatchType.TREE, 5374, 1513, 27600000L, 145.5, 13768.3, 75, "magic", 6051, "a", 75, 250.0, 11000}, // Magic
		    new object[]{PatchType.FRUIT_TREE, 5496, 1955, 52800000L, 22.0, 1199.5, 27, "apple", 8.5, "an"}, // Apple
		    new object[]{PatchType.FRUIT_TREE, 5497, 1963, 52800000L, 28.0, 1750.5, 33, "banana", 10.5, "a"}, // Banana
		    new object[]{PatchType.FRUIT_TREE, 5498, 2108, 52800000L, 35.5, 2470.2, 39, "orange", 13.5, "an"}, // Orange
		    new object[]{PatchType.FRUIT_TREE, 5499, 5970, 52800000L, 40.0, 2906.9, 42, "curry", 15.0, "a"}, // Curry
		    new object[]{PatchType.FRUIT_TREE, 5500, 2114, 52800000L, 57.0, 4605.7, 51, "pineapple", 21.5, "a"}, // Pineapple
		    new object[]{PatchType.FRUIT_TREE, 5501, 5972, 52800000L, 72.0, 6146.4, 57, "papaya", 27.0, "a"}, // Papaya
		    new object[]{PatchType.FRUIT_TREE, 5502, 5974, 52800000L, 110.5, 10150.1, 68, "coconut", 41.5, "a"} // Palm tree
	    };

        protected static object[][] PATCHES = {
		    //id, x1, y1, x2, y2, configId, config multiplyer, amount of seeds needed
		    new object[]{PatchType.VEGATABLE, 8550, 3050, 3307, 3054, 3312, 504, 1, 3},  // Draynor west patch
		    new object[]{PatchType.VEGATABLE, 8551, 3055, 3303, 3059, 3308, 504, 256, 3},  // Draynor east patch
		    new object[]{PatchType.VEGATABLE, 8552, 2805, 3466, 2814, 3468, 504, 65536, 3},  // Catherby north patch
		    new object[]{PatchType.VEGATABLE, 8553, 2805, 3459, 2814, 3461, 504, 16777216, 3},  // Catherby south patch
		    new object[]{PatchType.VEGATABLE_1, 8554, 2662, 3377, 2671, 3379, 505, 1, 3},  // Ardougne north patch
		    new object[]{PatchType.VEGATABLE_1, 8555, 2662, 3370, 2671, 3372, 505, 256, 3},  // Ardougne south patch
		    new object[]{PatchType.VEGATABLE_1, 8556, 3597, 3525, 3601, 3530, 505, 65536, 3},  // Canifis west patch
		    new object[]{PatchType.VEGATABLE_1, 8557, 3602, 3521, 3606, 3526, 505, 16777216, 3},  // Canifis east patch
		    new object[]{PatchType.HERB, 8150, 3058, 3311, 3059, 3312, 515, 1, 1},  // Draynor herb patch
		    new object[]{PatchType.HERB, 8151, 2813, 3463, 2814, 3464, 515, 256, 1},  // Catherby herb patch
		    new object[]{PatchType.HERB, 8152, 2670, 3374, 2671, 3375, 515, 65536, 1},  // Ardougne herb patch
		    new object[]{PatchType.HERB, 8153, 3605, 3529, 3606, 3530, 515, 16777216, 1},  // Canifis herb patch
		    new object[]{PatchType.FLOWER, 7847, 3054, 3307, 3055, 3308, 508, 1, 1},  // Draynor flower patch
		    new object[]{PatchType.FLOWER, 7848, 2809, 3463, 2810, 3464, 508, 256, 1},  // Catherby flower patch
		    new object[]{PatchType.FLOWER, 7849, 2666, 3374, 2667, 3375, 508, 65536, 1},  // Ardougne flower patch
		    new object[]{PatchType.FLOWER, 7850, 3601, 3525, 3602, 3526, 508, 16777216, 1},  // Canifis flower patch
		    new object[]{PatchType.TREE, 8388, 2935, 3437, 2937, 3439, 502, 1, 1},  // Taverly tree patch
		    new object[]{PatchType.TREE, 8389, 3003, 3372, 3005, 3374, 502, 256, 1},  // Falador tree patch
		    new object[]{PatchType.TREE, 8390, 3228, 3458, 3230, 3460, 502, 65536, 1},  // Varrock tree patch
		    new object[]{PatchType.TREE, 8391, 3192, 3230, 3194, 3232, 502, 16777216, 1},  // Lumbridge tree patch
		    new object[]{PatchType.FRUIT_TREE, 7962, 2475, 3445, 2476, 3446, 503, 1, 1},  // Gnome fruit tree patch
		    new object[]{PatchType.FRUIT_TREE, 7963, 2489, 3179, 2490, 3180, 503, 256, 1},  // Yanille fruit tree patch
		    new object[]{PatchType.FRUIT_TREE, 7964, 2764, 3212, 2765, 3213, 503, 65536, 1},  // Brimhaven fruit tree patch
		    new object[]{PatchType.FRUIT_TREE, 7965, 2860, 3433, 2861, 3434, 503, 16777216, 1}  // Catherby fruit tree patch
	    };

        protected static int[] TREE_CUT_LEVELS = {
		    15, // Oak
		    30, // Willow
		    45, // Maple
		    60, // Yew
		    75 // Magic
	    };

        protected static object[][] SAPLING_DATA = {
		    // Seed id, sapling 1 id, sapling 2 id, name
		    new object[]{5312, 5364, 5370, "acorn"}, // Oak
		    new object[]{5313, 5365, 5371, "willow"}, // Willow
		    new object[]{5314, 5366, 5372, "maple"}, // Maple
		    new object[]{5315, 5367, 5373, "yew"}, // Yew
		    new object[]{5316, 5368, 5374, "magic"}, // Magic
		    new object[]{5283, 5480, 5496, "apple", 8.5}, // Apple
		    new object[]{5284, 5481, 5497, "banana", 10.5}, // Banana
		    new object[]{5285, 5482, 5498, "orange", 13.5}, // Orange
		    new object[]{5286, 5483, 5499, "curry", 15.0}, // Curry
		    new object[]{5287, 5484, 5500, "pineapple", 21.5}, // Pineapple
		    new object[]{5288, 5485, 5501, "papaya", 27.0}, // Papaya
		    new object[]{5289, 5486, 5502, "coconut", 41.5} // Palm tree
	    };

        protected static int[][] ALLOTMENT_PATCH_CONFIGS = {
		    new int[] {6, 7, 8, 9, 10}, // Potato
		    new int[] {13, 14, 15, 16, 17}, // Onion
		    new int[] {20, 21, 22, 23, 24}, // Cabbage
		    new int[] {27, 28, 29, 30, 31}, // Tomato
		    new int[] {34, 35, 36, 37, 38, 39, 40}, // Sweetcorn
		    new int[] {43, 44, 45, 46, 47, 48, 49}, // Strawberry
		    new int[] {52, 53, 54, 55, 56, 57, 58, 59, 60}  // Watermelon
	    };

        protected static int[][] HERB_PATCH_CONFIGS = {
		    /*
		     * These are all the same
		     */
		    new int[] {4, 5, 6, 7, 8}, // Guam
		    new int[] {11, 12, 13, 14, 15}, // Marrentill
		    new int[] {18, 19, 20, 21, 22}, // Tarromin
		    new int[] {25, 26, 27, 28, 29}, // Harralander
		    new int[] {32, 33, 34, 35, 36}, // Ranarr
		    new int[] {39, 40, 41, 42, 43}, // Toadflax
		    new int[] {46, 47, 48, 48, 50}, // Irit
		    new int[] {53, 54, 55, 56, 57}, // Avantoe
		    new int[] {68, 69, 70, 71, 72}, // Kwuarm
		    new int[] {75, 76, 77, 78, 79}, // Snapdragon
		    new int[] {82, 83, 84, 85, 86}, // Cadantine
		    new int[] {89, 90, 91, 92, 93}, // Lantadyme
		    new int[] {96, 97, 98, 99, 100}, // Dwarf weed
		    new int[] {103, 104, 105, 106, 107} // Torstol
	    };

        protected static int[][] FLOWER_PATCH_CONFIGS = {
		    new int[] {9, 10, 11, 12}, // Marigold
		    new int[] {13, 14, 15, 16, 17}, // Rosemary
		    new int[] {18, 19, 20, 21, 22}, // Nasturtium
		    new int[] {23, 24, 25, 26, 27}, // Woad leaves
		    new int[] {28, 29, 30, 31, 32} // Limpwurt
	    };

        protected static int[][] TREE_PATCH_CONFIGS = {
		    /*
		     * Third to last is check-health, second to last is 'chop', last is stump
		     */
		    new int[] {8, 9, 10, 11, 12, 13, 14}, // Oak
		    new int[] {15, 16, 17, 18, 19, 20, 21, 22, 23}, // Willow
		    new int[] {24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34}, // Maple
		    new int[] {35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47}, // Yew
		    new int[] {48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62} // Magic
	    };

        protected static int[][] FRUIT_TREE_PATCH_CONFIGS = {
		    new int[] {8, 9, 10, 11, 12, 13, /*fruit*/ 15, 16, 17, 18, 19, 20, /* check health*/34, /*chop*/14, /*stump*/33}, // Apple
		    new int[] {35, 36, 37, 38, 39, 40, /*fruit*/ 42, 43, 44, 45, 46, 47, /* check health*/61, /*chop*/41, /*stump*/60}, // Banana
		    new int[] {72, 73, 74, 75, 76, 77, /*fruit*/ 79, 80, 81, 82, 83, 84, /* check health*/98, /*chop*/78, /*stump*/97}, // Orange
		    new int[] {99, 100, 101, 102, 103, 104, /*fruit*/ 106, 107, 108, 109, 110, 111, /* check health*/125, /*chop*/105, /*stump*/124}, // Curry
		    new int[] {136, 137, 138, 139, 140, 141, /*fruit*/ 143, 144, 145, 146, 147, 148, /* check health*/162, /*chop*/142, /*stump*/161}, // Pineapple
		    new int[] {163, 164, 165, 166, 167, 168, /*fruit*/ 170, 171, 172, 173, 174, 175, /* check health*/189, /*chop*/169, /*stump*/188}, // Papaya
		    new int[] {200, 201, 202, 203, 204, 205, /*fruit*/ 207, 208, 209, 210, 211, 212, /* check health*/226, /*chop*/206, /*stump*/225} // Palm tree
	    };

        protected static int[] WEEDS_CONFIG = {
		    1, 2, 3
	    };
    }
}