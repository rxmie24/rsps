namespace RS2.Server.player.skills.fletching
{
    internal class FletchingData
    {
        public FletchingData()
        {
        }

        protected static int FLETCHING = 9;

        protected static int KNIFE = 946;
        protected static int[] LOGS = { 1511, 1521, 1519, 1517, 1515, 1513 };
        protected static int ARROW_AMOUNT = 15;

        protected static int CUT_ANIMATION = 946;
        protected static int STRING_ANIMATION = 946;

        protected static int[] UNSTRUNG_SHORTBOW = { 50, 54, 60, 64, 68, 72 };
        protected static int[] STRUNG_SHORTBOW = { 841, 843, 849, 853, 857, 861 };
        protected static int[] SHORTBOW_LVL = { 5, 20, 35, 50, 65, 80 };
        protected static double[] SHORTBOW_XP = { 5, 16.5, 33, 50, 67.5, 83 };

        protected static int[] UNSTRUNG_LONGBOW = { 48, 56, 58, 62, 66, 70 };
        protected static int[] STRUNG_LONGBOW = { 839, 845, 847, 851, 855, 859 };
        protected static int[] LONGBOW_LVL = { 10, 25, 40, 55, 70, 85 };
        protected static double[] LONGBOW_XP = { 10, 25, 41.5, 58, 75, 91.5 };

        protected static int[] UNFINISHED_XBOW = { 9454, 9456, 9457, 9459, 9461, 9463, 9465 };
        protected static int[] FINISHED_XBOW = { 9174, 9176, 9177, 9179, 9181, 9183, 9185 };
        protected static int[] XBOW_LIMB = { 9420, 9422, 9423, 9425, 9427, 9429, 9431 };
        protected static int[] CROSSBOW_STOCK = { 9440, 9442, 9444, 9446, 9448, 9450, 9452 };
        protected static int[] XBOW_LVL = { 9, 24, 39, 46, 54, 61, 69 };
        protected static double[] XBOW_XP = { 6, 16, 22, 27, 32, 41, 50 };

        protected static int XBOW_STRING = 9438;
        protected static int BOWSTRING = 1777;

        protected static int ARROW_SHAFTS = 52;
        protected static double ARROW_SHAFT_XP = 0.33;
        protected static int ARROW_SHAFT_LVL = 1;
        protected static int FEATHER = 314;
        protected static int HEADLESS_ARROW = 53;

        protected static double HEADLESS_ARROW_XP = 1.0;
        protected static int HEADLESS_ARROW_LVL = 1;

        protected static int[] ARROW = { 882, 884, 886, 888, 890, 892, 11212 };
        protected static int[] ARROWHEAD = { 39, 40, 41, 42, 43, 44, 11237 };
        protected static int[] ARROW_LVL = { 1, 15, 30, 45, 60, 75, 90 };
        protected static double[] ARROW_XP = { 1.3, 2.5, 5.0, 7.5, 10, 12.5, 15 };

        protected static int[] FEATHERLESS_BOLT = { 9375, 9376, 9377, 9378, 9379, 9380, 9381, 9382 };
        protected static int[] FEATHERED_BOLT = { 877, 9139, 9140, 9141, 9142, 9143, 9144, 9145 };
        protected static int[] FEATHERLESS_BOLT_LVL = { 9, 24, 39, 46, 54, 61, 69, 43 };
        protected static double[] FEATHERLESS_BOLT_XP = { 0.5, 1, 1.5, 3.5, 5, 7, 10, 2.5 };

        protected static int[] BOLT_TIPS = { 45, 9187, 46, 9188, 9189, 9190, 9191, 9192, 9193, 9194 };
        protected static int[] BOLT = { 879, 9335, 880, 9336, 9337, 9338, 9339, 9340, 9341, 9342 };

        protected static int[] HEADLESS_BOLT = { 877, 9139, 9140, 9141, 9142, 9142, 9143, 9143, 9144, 9144 };
        protected static int[] HEADLESS_BOLT_LVL = { 11, 26, 41, 48, 56, 58, 63, 65, 71, 73 };
        protected static double[] HEADLESS_BOLT_XP = { 1.6, 2.4, 3.2, 3.9, 4.7, 5.5, 6.3, 7, 8.2, 9.4 };

        protected static string[] MESSAGE = { "a Shortbow", "a Longbow", ARROW_AMOUNT + " Arrow shafts", "a Crossbow handle" };

        public static object[][] GEMS = {
		    // cut, bolt, level, xp, name, cut emote
		    new object[] {1609, 45, 11, 1.5, "Opal", 886},
		    new object[] {1611, 9187, 26, 2.0, "Jade", 886},
		    new object[] {411, 46, 41, 3.2, "Pearl", 886},
		    new object[] {1613, 9188, 48, 3.9, "Red topaz", 887},
		    new object[] {1607, 9189, 56, 4.0, "Sapphire", 888},
		    new object[] {1605, 9190, 58, 5.5, "Emerald", 889},
		    new object[] {1603, 9191, 63, 6.0, "Ruby", 887},
		    new object[] {1601, 9192, 65, 7.0, "Diamond", 886},
		    new object[] {1615, 9193, 71, 8.2, "Dragonstone", 885},
		    new object[] {6573, 9194, 73, 9.4, "Onyx", 2717}
	    };
    }
}