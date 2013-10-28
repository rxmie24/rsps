namespace RS2.Server.player.skills.firemaking
{
    internal class FiremakingData
    {
        public FiremakingData()
        {
        }

        protected static int TINDERBOX = 590;
        protected static ushort FIRE_OBJECT = 2732;
        protected static int ASHES = 592;
        protected static int START_DELAY = 4500;

        protected static int[] LOGS = {
		    1511, 1521, 1519, 1517, 1515, 1513
	    };

        protected static int[] FIRE_LEVEL = {
		    1,  15, 30, 56, 60, 76
	    };

        protected static double[] FIRE_XP = {
		    40, 60, 90, 135, 202.5, 303.8
	    };

        protected static int[] COLOURED_LOGS = {
		    7404, // Red
		    7405, // Green
		    7406, // Blue
		    10329, // Purple
		    10328 // White
	    };

        protected static ushort[] COLOURED_FIRES = {
		    11404, // Red
		    11405, // Green
		    11406, // Blue
		    20001, // Purple
		    20000 // White
	    };

        protected static int[] FIRELIGHTERS = {
		    7329,
		    7330,
		    7331,
		    10326,
		    10327
	    };

        protected static int[][] OTHER_ITEMS = {
		    new int[] {596, 594}, // Torches
		    new int[] {36, 33}, // Candles
		    new int[] {4529, 4534}, // Candle lantern
		    new int[] {4522, 4524}, // Oil lamp
		    new int[] {4537, 4539}, // Oil lantern
		    new int[] {7051, 7053}, // Bug lantern
		    new int[] {4548, 4550}, // Bullseye lantern
		    new int[] {5014, 5013} // Mining helmet
	    };
    }
}