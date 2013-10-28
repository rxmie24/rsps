namespace RS2.Server.player.skills.runecrafting
{
    internal class RuneCraftData
    {
        public RuneCraftData()
        {
        }

        protected static int ESSENCE = 7936;

        public enum POUCHES
        {
            SMALL_POUCH = 5509,
            MEDIUM_POUCH = 5510,
            LARGE_POUCH = 5512,
            GIANT_POUCH = 5514
        };

        protected static int[] ALTARS = {
		    2478, // Air
		    2479, // Mind
		    2480, // Water
		    2481, // Earth
		    2482, // Fire
		    2483, // Body
		    2484, // Cosmic
		    2487, // Chaos
		    9075, // Astral
		    2486, // Nature
		    2485, // Law
		    2488, // Death
		    565, // Blood
		    -1,	// Soul
	    };

        protected static int[] RUNES = {
		    556, // Air
		    558, // Mind
		    555, // Water
		    557, // Earth
		    554, // Fire
		    559, // Body
		    564, // Cosmic
		    562, // Chaos
		    9075, // Astral
		    561, // Nature
		    563, // Law
		    560, // Death
		    565, // Blood
	    };

        protected static int[] CRAFT_LEVEL = {
		    1, // Air
		    2, // Mind
		    5, // Water
		    9, // Earth
		    14, // Fire
		    20, // Body
		    27, // Cosmic
		    35, // Chaos
		    40, // Astral
		    44, // Nature
		    54, // Law
		    65, // Death
		    77, // Blood
	    };

        protected static double[] CRAFT_XP = {
		    5, // Air
		    5.5, // Mind
		    6, // Water
		    6.5, // Earth
		    7, // Fire
		    7.5, // Body
		    8, // Cosmic
		    8.5, // Chaos
		    8.7, // Astral
		    9, // Nature
		    9.5, // Law
		    10, // Death
		    10.5, // Blood
	    };

        protected static int[][] MULTIPLY_LEVELS = {
		    new int[] {22, 33, 44, 55, 66, 77, 88, 99}, // Air
		    new int[] {14, 28, 42, 56, 70, 84, 98}, // Mind
		    new int[] {19, 38, 57, 76, 95}, // Water
		    new int[] {26, 52, 78}, // Earth
		    new int[] {35, 70}, // Fire
		    new int[] {46, 92}, // Body
		    new int[] {59}, // Cosmic
		    new int[] {74}, // Chaos
		    new int[] {82}, // Astral
		    new int[] {91}, // Nature
		    new int[] {-1}, // Law
		    new int[] {-1}, // Death
		    new int[] {-1}, // Blood
	    };

        protected static int[][] ALTAR_COORDS = {
		    new int[] {2844, 4834}, // Air
		    new int[] {2786, 4841}, // Mind
		    new int[] {3484, 4836}, // Water
		    new int[] {2658, 4841}, // Earth
		    new int[] {2585, 4838}, // Fire
		    new int[] {2523, 4840}, // Body
		    new int[] {2142, 4833}, // Cosmic
		    new int[] {2271, 4842}, // Chaos
		    new int[] {-1, -1}, // Astral
		    new int[] {2400, 4841}, // Nature
		    new int[] {2464, 4832}, // Law
		    new int[] {2205, 4836}, // Death
		    new int[] {-1, -1}, // Blood
	    };

        protected static int[][] RUIN_TELEPORT = {
		    new int[] {2841, 4829}, // Air
		    new int[] {2793, 4828}, // Mind
		    new int[] {3494, 4832}, // Water
		    new int[] {2655, 4830}, // Earth
		    new int[] {2576, 4845}, // Fire
		    new int[] {2521, 4834}, // Body
		    new int[] {2142, 4813}, // Cosmic
		    new int[] {2281, 4837}, // Chaos
		    new int[] {-1, -1}, // Astral
		    new int[] {2400, 4835}, // Nature
		    new int[] {2464, 4818}, // Law
		    new int[] {2208, 4830}, // Death
		    new int[] {-1, -1}, // Blood
	    };

        protected static int[] ABYSS_DOORWAYS = {
		    7139, // Air
		    7140, // Mind
		    7137, // Water
		    7130, // Earth
		    7129, // Fire
		    7131, // Body
		    7132, // Cosmic
		    7134, // Chaos
		    -1, // Astral
		    7133, // Nature
		    7135, // Law
		    7136, // Death
		    7141, // Blood
		    7138, // Soul
	    };

        protected static int[] TALISMANS = {
		    1438, // Air
		    1448, // Mind
		    1444, // Water
		    1440, // Earth
		    1442, // Fire
		    1446, // Body
		    1454, // Cosmic
		    1452, // Chaos
		    -1, // Astral (dosen't exist)
		    1462, // Nature
		    1458, // Law
		    1456, // Death
		    1450, // Blood
		    1460, // Soul
	    };

        protected static int[] TIARAS = {
		    5527, // Air
		    5529, // Mind
		    5531, // Water
		    5535, // Earth
		    5537, // Fire
		    5533, // Body
		    5539, // Cosmic
		    5543, // Chaos
		    -1, // Astral (dosen't exist)
		    5541, // Nature
		    5545, // Law
		    5547, // Death
		    -1, // Blood
		    -1, // Soul
	    };

        protected static int[] RUINS = {
		    2452, // Air
		    2453, // Mind
		    2454, // Water
		    2455, // Earth
		    2456, // Fire
		    2457, // Body
		    2458, // Cosmic
		    2461, // Chaos
		    -1, // Astral (dosen't exist)
		    -1, // Nature - mapdata problem
		    2459, // Law
		    -1, // Death - mapdata problem
		    -1, // Blood
		    -1, // Soul
	    };

        /**
         * Ruins with the 'Enter' option.
         */

        protected static int[] RUINS2 = {
		    7104, // Air
		    7106, // Mind
		    7108, // Water
		    7110, // Earth
		    7112, // Fire
		    7114, // Body
		    7116, // Cosmic
		    7122, // Chaos
		    -1, // Astral (dosen't exist)
		    7120, // Nature // unavailable
		    7118, // Law
		    7124, // Death
		    -1, // Blood - unavailable
		    -1, // Soul - unavailable
	    };

        protected static int[][] RUIN_COORDS = {
		    new int[] {2984, 3291}, // Air
		    new int[] {2981, 3513}, // Mind
		    new int[] {3184, 3164}, // Water
		    new int[] {3305, 3473}, // Earth
		    new int[] {3312, 3254}, // Fire
		    new int[] {3052, 3444}, // Body
		    new int[] {2407, 4376}, // Cosmic
		    new int[] {3059, 3590}, // Chaos
		    new int[] {-1, -1}, // Astral
		    new int[] {2400, 4835}, // Nature
		    new int[] {2857, 3380}, // Law
		    new int[] {-1, -1}, // Death
		    new int[] {-1, -1}, // Blood
	    };

        protected static int[][] PORTAL_COORDS = {
		    new int[] {2841, 4828}, // Air
		    new int[] {2793, 4827}, // Mind
		    new int[] {3495, 4832}, // Water
		    new int[] {2655, 4829}, // Earth
		    new int[] {2576, 4846}, // Fire
		    new int[] {2521, 4833}, // Body
		    new int[] {2142, 4812}, // Cosmic
		    new int[] {2282, 4837}, // Chaos
		    new int[] {-1, -1}, // Astral
		    new int[] {2400, 4834}, // Nature
		    new int[] {2464, 4817}, // Law
		    new int[] {2208, 4829}, // Death
		    new int[] {-1, -1}, // Blood
	    };

        protected static int[] PORTALS = {
		    2465, // Air
		    2466, // Mind
		    2467, // Water
		    2468, // Earth
		    2469, // Fire
		    2470, // Body
		    2471, // Cosmic
		    2474, // Chaos
		    -1, // Astral (dosen't exist)
		    2473, // Nature // unavailable
		    2472, // Law
		    2475, // Death
		    -1, // Blood - unavailable
		    -1, // Soul - unavailable
	    };

        protected static int[][] ABYSS_TELEPORT_OUTER = {
		    new int[] {3059, 4817}, new int[] {3062, 4812}, new int[] {3052, 4810}, new int[] {3041, 4807},
		    new int[] {3035, 4811}, new int[] {3030, 4808}, new int[] {3026, 4810}, new int[] {3021, 4811},
		    new int[] {3015, 4810}, new int[] {3020, 4818}, new int[] {3018, 4819}, new int[] {3016, 4824},
		    new int[] {3013, 4827}, new int[] {3017, 4828}, new int[] {3015, 4837}, new int[] {3017, 4843},
		    new int[] {3014, 4849}, new int[] {3021, 4847}, new int[] {3022, 4852}, new int[] {3027, 4849},
            new int[] {3031, 4856}, new int[] {3035, 4854}, new int[] {3043, 4855}, new int[] {3045, 4852},
		    new int[] {3050, 4857}, new int[] {3054, 4855}, new int[] {3055, 4848}, new int[] {3060, 4848},
            new int[] {3059, 4844}, new int[] {3065, 4841}, new int[] {3061, 4836}, new int[] {3063, 4832},
		    new int[] {3064, 4828}, new int[] {3060, 4824}, new int[] {3063, 4821}, new int[] {3041, 4808},
            new int[] {3030, 4810}, new int[] {3018, 4816}, new int[] {3015, 4829}, new int[] {3017, 4840},
		    new int[] {3020, 4849}, new int[] {3031, 4855}, new int[] {3020, 4854}, new int[] {3035, 4855},
		    new int[] {3047, 4854}, new int[] {3060, 4846}, new int[] {3062, 4836}, new int[] {3060, 4828},
            new int[] {3063, 4820}, new int[] {3028, 4806}
	    };

        protected static int[][] ABYSS_TELEPORT_INNER = {
		    new int[] {3039, 4844}, new int[] {3045, 4843}, new int[] {3050, 4839}, new int[] {3039, 4844},
            new int[] {3052, 4833}, new int[] {3052, 4829}, new int[] {3039, 4844}, new int[] {3049, 4826},
		    new int[] {3048, 4821}, new int[] {3044, 4821}, new int[] {3034, 4820}, new int[] {3029, 4823},
		    new int[] {3028, 4829}, new int[] {3025, 4834}, new int[] {3027, 4838}, new int[] {3029, 4841},
		    new int[] {3035, 4844}
	    };
    }
}