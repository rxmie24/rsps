namespace RS2.Server.player.skills.crafting
{
    internal class CraftingData
    {
        public CraftingData()
        {
        }

        protected static int CLAY = 1761;
        protected static int CLAY_TABLE = 2642;
        protected static int CLAY_OVEN = 11601;

        protected static object[][] CLAY_ITEMS = {
		    // unfired, fired(finished) level, spin xp, fire xp, message/name
		    new object[] {1787, 1931, 1, 6.3, 6.3, "Pot"},
		    new object[] {1789, 2313, 7, 15.0, 10.0, "Pie dish"},
		    new object[] {1791, 1923, 8, 18.0, 15.0, "Bowl"},
		    new object[] {5352, 5350, 19, 20.0, 17.5, "Plant pot"},
		    new object[] {4438, 4440, 25, 20.0, 20.0, "Pot lid"},
	    };

        protected static int MOLTEN_GLASS = 1775;
        protected static int GLASSBLOWING_PIPE = 1785;

        protected static object[][] GLASS_ITEMS = {
		    // finished item, level, xp, name
		    new object[] {1919, 1, 17.5, "Beer glass"},
		    new object[] {4529, 4, 19.0, "Candle lantern"},
		    new object[] {4522, 12, 25.0, "Oil lamp"},
		    new object[] {4537, 26, 50.0, "Oil lantern"},
		    new object[] {229, 33, 35.0, "Vial"},
		    new object[] {6667, 42, 42.5, "Fishbowl"},
		    new object[] {567, 46, 52.5, "Orb"},
		    new object[] {4542, 49, 55.0, "Lantern lens"},
		    new object[] {10973, 87, 70, "Dorgeshuun light orb"}
	    };

        protected static int NEEDLE = 1733;
        protected static int THREAD = 1734;
        protected static int COWHIDE = 1739;

        protected static int[] UNTANNED_HIDE = {
		    1753, 1751, 1749, 1751, 1739
	    };

        protected static int[] TANNED_HIDE = {
		    1745, 2505, 2507, 2509, 1741
	    };

        protected static object[][] NORMAL_LEATHER = {
		    // finished item, level, xp, name
		    new object[] {1129, 14, 25.0, "Leather body"},
		    new object[] {1059, 1, 13.8, "Leather gloves"},
		    new object[] {1061, 7, 16.3, "Leather boots"},
		    new object[] {1063, 11, 22.0, "Leather vambraces"},
		    new object[] {1095, 18, 27.0, "Leather chaps"},
		    new object[] {1169, 38, 37.0, "Coif"},
		    new object[] {1167, 9, 18.5, "Leather cowl"},
	    };

        protected static object[][] LEATHER_ITEMS = {
		    //finished item, level, xp, # of hides, name
		    new object[] {1135, 63, 186.0, 3, "Green body"},
		    new object[] {2499, 71, 210.0, 3, "Blue body"},
		    new object[] {2501, 77, 234.0, 3, "Red body"},
		    new object[] {2503, 84, 258.0, 3, "Black body"},

		    new object[] {1065, 57, 62.0, 1, "Green vambraces"},
		    new object[] {2487, 66, 70.0, 1, "Blue vambraces"},
		    new object[] {2489, 73, 78.0, 1, "Red vambraces"},
		    new object[] {2491, 79, 86.0, 1, "Black vampbraces"},

		    new object[] {1099, 60, 124.0, 2, "Green chaps"},
		    new object[] {2493, 68, 140.0, 2, "Blue chaps"},
		    new object[] {2495, 75, 156.0, 2, "Red chaps"},
		    new object[] {2497, 82, 172.0, 2, "Black chaps"},
	    };

        protected static int CHISEL = 1755;

        protected static object[][] GEMS = {
		    // uncut, cut, level, xp, name, cut emote
		    new object[] {1625, 1609, 1, 15.0, "Opal", 886},
		    new object[] {1627, 1611, 13, 20.0, "Jade", 886},
		    new object[] {1629, 1613, 16, 25.0, "Red topaz", 887},
		    new object[] {1623, 1607, 20, 50.0, "Sapphire", 888},
		    new object[] {1621, 1605, 27, 67.5, "Emerald", 889},
		    new object[] {1619, 1603, 34, 85.0, "Ruby", 887},
		    new object[] {1617, 1601, 43, 107.5, "Diamond", 886},
		    new object[] {1631, 1615, 55, 137.5, "Dragonstone", 885},
		    new object[] {6571, 6573, 67, 167.5, "Onyx", 2717},
	    };

        protected static int[] CUT_GEMS = {
		    1607, 1605, 1603, 1601, 1615, 6573
	    };

        protected static int NULL_RING = 1647;
        protected static int GOLD_BAR = 2357;

        protected static int[] NULL_JEWELLERY = {
		    1647, 1666, 1685, 11067
	    };

        protected static int[][] JEWELLERY_BUTTON_IDS = {
			    new int[] {20, 22, 24, 26, 28, 30, 32},
			    new int[] {39, 41, 43, 45, 47, 49, 51},
			    new int[] {58, 60, 62, 64, 66, 68, 70},
			    new int[] {77, 79, 81, 83, 85, 87, 89}
		};

        protected static int[][] JEWELLERY_INTERFACE_VARS = {
		    // mould id, id to remove the text, index the images start at
		    new int[] {1592, 14, 19},
		    new int[] {1597, 33, 38},
		    new int[] {1595, 52, 57},
		    new int[] {11065, 71, 76},
	    };

        protected static object[][] RINGS = {
		    new object[] {1635, 5, 15.0, "ring"},
		    new object[] {1637, 30, 40.0, "ring"},
		    new object[] {1639, 27, 55.0, "ring"},
		    new object[] {1641, 34, 70.0, "ring"},
		    new object[] {1643, 43, 85.0, "ring"},
		    new object[] {1645, 55, 100.0, "ring"},
		    new object[] {6575, 67, 115.0, "ring"},
	    };

        protected static object[][] NECKLACES = {
		    new object[] {1654, 6, 20.0, "necklace"},
		    new object[] {1656, 22, 55.0, "necklace"},
		    new object[] {1658, 29, 60.0, "necklace"},
		    new object[] {1660, 40, 75.0, "necklace"},
		    new object[] {1662, 56, 90.0, "necklace"},
		    new object[] {1664, 72, 105.0, "necklace"},
		    new object[] {6577, 82, 120.0, "necklace"},
	    };

        protected static object[][] BRACELETS = {
		    new object[] {11069, 7, 25.0, "bracelet"},
		    new object[] {11072, 23, 60.0, "bracelet"},
		    new object[] {11074, 30, 65.0, "bracelet"},
		    new object[] {11076, 42, 80.0, "bracelet"},
		    new object[] {11078, 58, 95.0, "bracelet"},
		    new object[] {11080, 74, 110.0, "bracelet"},
		    new object[] {11130, 84, 125.0, "bracelet"},
	    };

        protected static object[][] AMULETS = {
		    //finished id, level, xp, message
		    new object[] {1673, 8, 30.0, "amulet"},
		    new object[] {1675, 24, 65.0, "amulet"},
		    new object[] {1677, 31, 70.0, "amulet"},
		    new object[] {1679, 50, 85.0, "amulet"},
		    new object[] {1681, 70, 100.0, "amulet"},
		    new object[] {1683, 80, 150.0, "amulet"},
		    new object[] {6579, 90, 165.0, "amulet"},
	    };

        protected static int STRINGING_XP = 4;

        protected static int[][] STRUNG_AMULETS = {
		    //finished id, not finished id
		    new int[] {1692, 1673},
		    new int[] {1694, 1675},
		    new int[] {1696, 1677},
		    new int[] {1698, 1679},
		    new int[] {1700, 1681},
		    new int[] {1702, 1683},
		    new int[] {6581, 6579},
	    };

        protected static int SPINNING_WHEEL = 6;

        protected static string[] SPIN_FINISH = {
		    "Ball of Wool", "Bow String", "Crossbow String"
	    };

        protected static object[][] SPINNING_ITEMS = {
		    //finished id, needed id, level, xp, message
		    new object[] {1759, 1737, 1, 2.5, "Wool"},
		    new object[] {1777, 1779, 10, 15.0, "Flax"},
		    new object[] {9438, 9436, 10, 15.0, "Sinew"}
	    };

        protected static int BALL_OF_WOOL = 1759;

        protected static int SILVER_BAR = 2355;

        protected static object[][] SILVER_ITEMS = {
		    // unfinished, mould Id, level, xp, message
		    new object[] {1714, 1599, 16, 50.0, "Unholy symbol"},
		    new object[] {5525, 5523, 23, 52.5, "Tiara"},
	    };
    }
}