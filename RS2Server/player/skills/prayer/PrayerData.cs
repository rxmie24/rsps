namespace RS2.Server.player.skills.prayer
{
    internal class PrayerData
    {
        public PrayerData()
        {
        }

        public static int MELEE = 0;
        public static int RANGE = 1;
        public static int MAGIC = 2;
        public static int SMITE = 4;
        public static int REDEMPTION = 5;
        public static int RETRIBUTION = 3;

        public static int BURY_ANIMATION = 827;

        public static int[] BONES = {
		    526, // Normal
		    2859, // Wolf
		    528, // Burnt
		    3179, // Monkey
		    530, // Bat
		    532, // Big
		    10977, // Curved
		    10976, // Long
		    3125, // Jogre
		    4812, // Zogre
		    3123, // Shaikahan
		    534, // Baby dragon
		    6812, // Wyvern
		    536, // Dragon
		    4830, // Fayrg
		    4832, // Raurg
		    6729, // Dagannoth
		    4834, // Ourg
	    };

        public static double[] BURY_XP = {
		    4.5, // Normal
		    4.5, // Wolf
		    4.5, // Burnt
		    5, // Monkey
		    5.3, // Bat
		    15, // Big
		    15, // Curved
		    15, // Long
		    15, // Jogre
		    22.5, // Zogre
		    25, // Shaikahan
		    30, // Baby dragon
		    50, // Wyvern
		    72, // Dragon
		    84, // Fayrg
		    96, // Raurg
		    125, // Dagannoth
		    140, // Ourg
	    };

        public static int[] PRAYER_LVL = {
		    1, 4, 7, 8, 9, 10, 13, 16, 19, 22, 25, 26, 27, 28,
		    31, 34, 35, 37, 40, 43, 44, 45, 46, 49, 52, 60, 70
	    };

        public static int[] PRAYER_CONFIG = {
		    83, 84, 85, 862, 863, 86, 87, 88, 89, 90, 91, 864,
		    865, 92, 93, 94, 1168, 95, 96, 97, 866, 867, 98, 99, 100,
		    1052, 1053
	    };

        public static string[] PRAYER_NAME = {
		    "Thick Skin", "Burst of Strength", "Clarity of Thought", "Sharp Eye", "Mystic Will", "Rock Skin", "Superhuman Strength",
		    "Improved Reflexes", "Rapid Restore", "Rapid Heal", "Protect Item", "Hawk Eye", "Mystic Lore", "Steel Skin",
		    "Ultimate Strength", "Incredible Reflexes", "Protect from Summoning", "Protect from Magic", "Protect from Ranged",
		    "Protect from Melee", "Eagle Eye", "Mystic Might", "Retribution", "Redemption", "Smite", "Chivalry", "Piety"
	    };

        /*
         * How many seconds to deplete 1 point
         */

        public static double[] DRAIN_RATE = { //seconds to waste 1 prayer.
		    12.0, 12.0, 12.0, 12.0, 12.0, 6.0, 6.0,
		    6.0, 36.0, 18.0, 18.0, 6.0, 6.0, 3.0, 3.0,
		    3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 12.0,
		    6.0, 1.8, 1.8, 1.5
	    };
    }
}