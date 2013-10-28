namespace RS2.Server.player.skills.herblore
{
    internal class HerbloreData
    {
        public HerbloreData()
        {
        }

        public static int[] GRIMY_HERBS = { 199, 201, 203, 205, 207, 3049, 12174, 209, 211, 213, 3051, 215, 2485, 217, 219 };
        protected static int[] CLEAN_HERBS = { 249, 251, 253, 255, 257, 2998, 12172, 259, 261, 263, 3000, 265, 2481, 267, 269 };
        protected static int[] HERB_LVL = { 3, 5, 11, 20, 25, 30, 35, 40, 48, 54, 59, 65, 67, 70, 75 };
        protected static double[] HERB_XP = { 2.5, 3.8, 5, 6.3, 7.5, 8, 7.8, 8.8, 10, 11.3, 11.8, 12.5, 13.1, 13.8, 15 };
        protected static string[] HERB_NAME = { "Guam leaf", "Marrentill", "Tarromin", "Harralander", "Ranarr weed", "Toadflax", "Spirit weed", "Irit", "Avantoe", "Kwuarm", "Snapdragon", "Cadantine", "Lantadyme", "Dwarf weed", "Torstol" };

        protected static int[] SECONDARY = { 221, 235, 225, 223, 1581, 1975, 239, 12630, 2152, 9736, 231, 12109, 221, 235, 231, 2970, 10111, 225, 241, 223, 239, 241, 245, 3138, 247, 6693 };
        protected static int[] UNFINISHED = { 91, 93, 95, 97, 97, 97, 99, 91, 3002, 97, 99, 12181, 101, 101, 103, 103, 103, 105, 105, 3004, 107, 2483, 109, 2483, 111, 3002 };
        protected static int[] POTION_LEVEL = { 3, 5, 12, 22, 25, 26, 30, 31, 34, 36, 38, 40, 45, 48, 50, 52, 53, 55, 60, 63, 66, 69, 72, 76, 78, 81 };
        protected static double[] POTION_XP = { 25, 37.5, 50, 62.5, 80, 67.5, 75, 55, 80, 84, 87.5, 92, 100, 106.3, 112.5, 117.5, 120, 125, 137.5, 142.5, 150, 157.5, 162.5, 172.5, 175, 180 };
        protected static int[] END_POTION = { 121, 175, 115, 127, 1582, 3010, 133, 12633, 3034, 9741, 139, 12142, 145, 181, 151, 3018, 10000, 157, 187, 3026, 163, 2454, 169, 3042, 189, 6687 };

        protected static int[] UNFINISHED_POTION = { 91, 93, 95, 97, 99, 3002, 12181, 101, 103, 105, 3004, 107, 2483, 109, 111 };

        protected static int[] UNCRUSHED = { 237, 243, 1973, 9735, 10109 };
        protected static int[] CRUSHED = { 235, 241, 1975, 9736, 10111 };

        protected static int GRIND_ANIMATION = 364;
        protected static int MIX_ANIMATION = 363;
        protected static int PESTLE_AND_MORTAR = 233;
        protected static int VIAL = 229;
        protected static int VIAL_OF_WATER = 227;

        public static int[][] DOSES = {
		    new int[] {125, 179, 119, 131, 3014, 137, 3038, 9745, 143, 12146, 149, 185, 155, 3022, 10004, 161, 3030, 167, 2458, 173, 3046, 193, 6691}, // 1 dose
		    new int[] {123, 177, 117, 129, 3012, 135, 3036, 9743, 141, 12144, 147, 183, 153, 3020, 10002, 159, 3028, 165, 2456, 171, 3044, 191, 6689}, // 2 dose
		    new int[] {121, 175, 115, 127, 3010, 133, 3034, 9741, 139, 12142, 145, 181, 151, 3018, 10000, 157, 3026, 163, 2454, 169, 3042, 189, 6687}, // 3 dose
		    new int[] {2428, 2446, 113, 2430, 3008, 2432, 3032, 9739, 2434, 12140, 2436, 2448, 2438, 3016, 9998, 2440, 3024, 2442, 2452, 2444, 3040, 2450, 6685} // 4 dose
	    };
    }
}