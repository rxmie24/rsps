namespace RS2.Server.player.skills.fishing
{
    internal class FishingData
    {
        public FishingData()
        {
        }

        protected static int[] SPOT_IDS ={
		    233, // Bait - sardine
		    952, // Net - Shrimp, anchovie
		    309, // Lure/bait - Trout, salmon /
		    312, // Cage/Harpoon - Lobster / Tuna/Swordfish/Shark
		    313, // Net/Harpoon - Big net / Shark
		    952, // Net - Monkfish
	    };

        protected static int[][] FIRST_SPOT_FISH ={
		    new int[] {327, 345, 349}, // Sardine, herring, pike
		    new int[] {317, 321}, // shrimp, anchovies
		    new int[] {335, 331}, // trout, salmon
		    new int[] {377}, // Lobster
		    new int[] {405, 353, 407, 401, 341, 363}, // Casket, mackerel, oyster, seaweed, cod, bass
		    new int[] {7944}, // Monkfish
	    };

        protected static string[][] FIRST_CATCH_NAME = {
		    new string[] {"Sardine", "Herring", "Pike"},
		    new string[] {"Shrimp", "Anchovies"},
		    new string[] {"Trout", "Salmon"},
		    new string[] {"Lobster"},
		    new string[] {"Casket", "Mackerel", "Oyster", "Seaweed", "Cod", "Bass"},
		    new string[] {"Monfish"},
	    };

        protected static int[] FIRST_SPOT_TIME = {
		    5000, // Sardine, herring, pike
		    5000, // shrimp, anchovies
		    6000, // trout, salmon
		    4000, // Lobster
		    4000, // Casket, mackerel, oyster, seaweed, cod, bass
		    6000, // Monkfish
	    };

        protected static int[] SECOND_SPOT_TIME = {
		    -1,
		    -1,
		    -1,
		    9000,
		    11000,
	    };

        protected static int[] FIRST_SPOT_MINTIME = {
		    3000,
		    3000,
		    3500,
		    500,
		    2000,
		    3000,
	    };

        protected static int[] SECOND_SPOT_MINTIME = {
		    -1,
		    -1,
		    -1,
		    4000,
		    2000,
	    };

        protected static int[][] SECOND_SPOT_FISH ={
		    new int[] {-1},
		    new int[] {-1},
		    new int[] {-1},
		    new int[] {359, 371, 383},
		    new int[] {383},
		    new int[] {-1},
	    };

        protected static string[][] SECOND_CATCH_NAME = {
		    new string[] {""},
		    new string[] {""},
		    new string[] {"Tuna", "Swordfish", "Shark"},
		    new string[] {"Shark"},
		    new string[] {""},
		    new string[] {""},
	    };

        protected static int[][] FIRST_SPOT_LEVEL ={
		    new int[] {5, 10, 25},
		    new int[] {1, 15},
		    new int[] {20, 30},
		    new int[] {40},
		    new int[] {16, 16, 16, 16, 23, 46},
		    new int[] {62},
	    };

        protected static int[][] SECOND_SPOT_LEVEL ={
		    new int[] {-1},
		    new int[] {-1},
		    new int[] {-1},
		    new int[] {35, 50, 76},
		    new int[] {76},
		    new int[] {-1},
	    };

        protected static double[][] FIRST_SPOT_XP ={
		    new double[] {20, 30, 60},
		    new double[] {10, 40},
		    new double[] {50, 70},
		    new double[] {90},
		    new double[] {10, 20, 10, 1, 45, 100},
		    new double[] {120},
	    };

        protected static double[][] SECOND_SPOT_XP ={
		    new double[] {-1},
		    new double[] {-1},
		    new double[] {-1},
		    new double[] {80, 100, 110},
		    new double[] {110},
		    new double[] {-1},
	    };

        protected static int[][] PRIMARY_ITEM ={
		    new int[] {307}, // Fishing rod
		    new int[] {303}, // Small net
		    new int[] {309}, // Fly fishing rod
		    new int[] {301, 311}, // Lobster pot / Harpoon
		    new int[] {305, 311}, // Harpoon
		    new int[] {303}, // Big net
	    };

        protected static int[][] SECONDARY_ITEM ={
		    new int[] {313}, // Bait
		    new int[] {-1},
		    new int[] {314}, // Feather
		    new int[] {-1, -1},
		    new int[] {-1, -1},
		    new int[] {-1},
	    };

        protected static string[][] PRIMARY_NAME = {
		    new string[] {"a fishing rod"},
		    new string[] {"a small fishing net"},
		    new string[] {"a fly-fishing rod"},
		    new string[] {"a lobster pot", "harpoon"},
		    new string[] {"a big net", "harpoon"},
		    new string[] {"a small fishing net"}
	    };

        protected static string[][] SECONDARY_NAME = {
		    new string[] {"some fishing bait"},
		    new string[] {""},
		    new string[] {"feathers"},
		    new string[] {"", ""},
		    new string[] {"", ""},
		    new string[] {""},
	    };

        protected static int[][] FISHING_ANIMATION ={
		    new int[] {622}, // Rod casting anim
		    new int[] {621}, // Small net
		    new int[] {622}, // Rod casting anim
		    new int[] {619, 618}, // Lobster pot
		    new int[] {620, 618}, // Big net
		    new int[] {621}, // Small net
	    };

        protected static int[][] FISHING_ANIMATION2 ={
		    new int[] {623}, // Rod continue fishing anim
		    new int[] {621}, // Small net
		    new int[] {623}, // Rod continue fishing anim
		    new int[] {619, 618}, // Lobster pot
		    new int[] {620, 618}, // Big net
		    new int[] {621}, // Small net
	    };
    }
}