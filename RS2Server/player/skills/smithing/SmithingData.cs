namespace RS2.Server.player.skills.smithing
{
    internal class SmithingData
    {
        public SmithingData()
        {
        }

        protected static int HAMMER = 2347;

        protected static int[] BARS = {
		    2349, // Bronze
		    9467, // Blurite
		    2351, // Iron
		    2355, // Silver
		    2353, // Steel
		    2357, // Gold
		    2359, // Mithril
		    2361, // Adamant
		    2363 // Rune
	    };

        protected static int[] SMELT_LEVELS = {
		    1, // Bronze
		    8, // Blurite
		    15, // Iron
		    20, // Silver
		    30, // Steel
		    40, // Gold
		    50, // Mithril
		    70, // Adamant
		    85 // Rune
	    };

        protected static double[] SMELT_XP = {
		    6.2, // Bronze
		    8, // Blurite
		    12.5, // Iron
		    13.7, // Silver
		    17.5, // Steel
		    22.5, // Gold
		    30, // Mithril
		    37.5, // Adamant
		    50 // Rune
	    };

        protected static int[][] SMELT_ORES = {
		    new int[] {436, 438}, // Tin, Copper
		    new int[] {668, 0}, // Blurite
		    new int[] {440, 0}, // Iron
		    new int[] {442, 0}, // Silver
		    new int[] {440, 453}, // Iron, Coal
		    new int[] {444, 0}, // Gold
		    new int[] {447, 453}, // Mithril, Coal
		    new int[] {449, 433}, // Adamant, Coal
		    new int[] {451, 453} // Rune, Coal
	    };

        protected static int[][] SMELT_ORE_AMT = {
		    new int[] {1, 1}, // 1 Tin 1 Copper
		    new int[] {1, 0}, // 1 Blurite
		    new int[] {1, 0}, // 1 Iron
		    new int[] {1, 0}, // 1 Silver
		    new int[] {1, 2}, // 1 Iron 2 Coal
		    new int[] {1, 0}, // 1 Gold
		    new int[] {1, 4}, // 1 Mithril 4 Coal
		    new int[] {1, 6}, // 1 Adamant 6 Coal
		    new int[] {1, 8} // 1 Runite 8 Coal
	    };

        protected static int[] SMELT_BUTTON_AMOUNT = {
		    -2, 10, 5, 1, // Bronze (-1 means Enter X)
		    -2, 10, 5, 1, // Blurite
		    -2, 10, 5, 1, // Iron
		    -2, 10, 5, 1, // Silver
		    -2, 10, 5, 1, // Steel
		    -2, 10, 5, 1, // Gold
		    -2, 10, 5, 1, // Mithril
		    -2, 10, 5, 1, // Adamant
		    -2, 10, 5, 1 // Rune
	    };

        protected static string[] BAR_NAMES = {
		    "Bronze", // Bronze
		    "Blurite", // Blurite
		    "Iron", // Iron
		    "Silver", // Silver
		    "Steel", // Steel
		    "Gold", // Gold
		    "Mithril", // Mithril
		    "Adamant", // Adamant
		    "Rune" // Rune
	    };

        /*
         * Item / level / amt of item / amt of bars / xp / name / interface placement
         */

        protected static object[][] BRONZE = {
		    new object[] {1205, 1, 1, 1, 12, "Dagger", 18},
		    new object[] {1351, 1, 1, 1, 12, "Axe", 26},
		    new object[] {1422, 2, 1, 1, 12, "Mace", 34},
		    new object[] {1139, 3, 1, 1, 12, "Medium helm", 42},
		    new object[] {9375, 3, 10, 1, 12, "Crossbow bolts", 50},
		    new object[] {1277, 4, 1, 1, 12, "Sword", 58},
		    new object[] {819, 4, 10, 1, 12, "Dart tips", 66},
		    new object[] {4819, 4, 15, 1, 12, "Nails", 74},
		    new object[] {1794, 4, 1, 1, 12, "Bronze wire", 82},
		    new object[] {0, 0, 0, 0, 0, null, 90},
		    new object[] {0, 0, 0, 0, 0, null, 98},
		    new object[] {39, 5, 15, 1, 12, "Arrow tips", 106},
		    new object[] {1321, 5, 1, 2, 25, "Scimitar", 114},
		    new object[] {9420, 6, 1, 1, 12, "Crossbow limb", 122},
		    new object[] {1291, 6, 1, 2, 25, "Longsword", 130},
		    new object[] {864, 7, 5, 1, 12, "Throwing knife", 138},
		    new object[] {1155, 7, 1, 2, 25, "Full Helm", 146},
		    new object[] {1173, 8, 1, 2, 25, "Square shield", 154},
		    new object[] {0, 0, 0, 0, 0, null, 162},
		    new object[] {0, 0, 0, 0, 0, null, 170},
		    new object[] {1337, 9, 1, 3, 37, "Warhammer", 178},
		    new object[] {1375, 10, 1, 3, 37, "Battleaxe", 186},
		    new object[] {1103, 11, 1, 3, 37, "Chainbody", 194},
		    new object[] {1189, 12, 1, 3, 37, "Kiteshield", 202},
		    new object[] {3095, 13, 1, 2, 25, "Claws", 210},
		    new object[] {1307, 14, 1, 3, 37, "2-h sword", 218},
		    new object[] {1087, 16, 1, 3, 37, "Plateskirt", 226},
		    new object[] {1075, 16, 1, 3, 37, "Platelegs", 234},
		    new object[] {1117, 18, 1, 5, 62, "Platebody", 242},
		    new object[] {1265, 5, 1, 2, 25, "Pickaxe", 267}
	    };

        protected static object[][] IRON = {
		    new object[] {1203, 15, 1, 1, 25, "Dagger", 18},
		    new object[] {1349, 16, 1, 1, 25, "Axe", 26},
		    new object[] {1420, 17, 1, 1, 25, "Mace", 34},
		    new object[] {1137, 18, 1, 1, 25, "Medium helm", 42},
		    new object[] {9377, 18, 10, 1, 25, "Crossbow bolts", 50},
		    new object[] {1279, 19, 1, 1, 25, "Sword", 58},
		    new object[] {820, 19, 10, 1, 25, "Dart tips", 66},
		    new object[] {4820, 19, 15, 1, 25, "Nails", 74},
		    new object[] {0, 0, 0, 0, 0, null, 82},
		    new object[] {7225, 17, 1, 1, 25, "Iron spit", 90},
		    new object[] {0, 0, 0, 0, 0, null, 98},
		    new object[] {40, 20, 15, 1, 25, "Arrow tips", 106},
		    new object[] {1323, 20, 1, 2, 50, "Scimitar", 114},
		    new object[] {9423, 23, 1, 1, 25, "Crossbow limb", 122},
		    new object[] {1293, 21, 1, 2, 50, "Longsword", 130},
		    new object[] {863, 22, 5, 1, 25, "Throwing knife", 138},
		    new object[] {1153, 22, 1, 2, 50, "Full Helm", 146},
		    new object[] {1175, 23, 1, 2, 50, "Square shield", 154},
		    new object[] {4540, 26, 1, 1, 25, "Oil lantern", 162},
		    new object[] {0, 0, 0, 0, 0, null, 170},
		    new object[] {1335, 24, 1, 3, 75, "Warhammer", 178},
		    new object[] {1363, 25, 1, 3, 75, "Battleaxe", 186},
		    new object[] {1101, 26, 1, 3, 75, "Chainbody", 194},
		    new object[] {1191, 27, 1, 3, 75, "Kiteshield", 202},
		    new object[] {3096, 28, 1, 2, 50, "Claws", 210},
		    new object[] {1309, 29, 1, 3, 75, "2-h sword", 218},
		    new object[] {1081, 31, 1, 3, 75, "Plateskirt", 226},
		    new object[] {1067, 31, 1, 3, 75, "Platelegs", 234},
		    new object[] {1115, 33, 1, 5, 125, "Platebody", 242},
		    new object[] {1267, 20, 1, 2, 50, "Pickaxe", 267}
	    };

        protected static object[][] STEEL = {
		    new object[] {1207, 30, 1, 1, 37, "Dagger", 18},
		    new object[] {1353, 31, 1, 1, 37, "Axe", 26},
		    new object[] {1424, 32, 1, 1, 37, "Mace", 34},
		    new object[] {1141, 33, 1, 1, 37, "Medium helm", 42},
		    new object[] {9378, 33, 10, 1, 37, "Crossbow bolts", 50},
		    new object[] {1281, 34, 1, 1, 37, "Sword", 58},
		    new object[] {821, 34, 10, 1, 37, "Dart tips", 66},
		    new object[] {1539, 34, 15, 1, 37, "Nails", 74},
		    new object[] {0, 0, 0, 0, 0, null, 82},
		    new object[] {7225, 17, 1, 1, 25, null, 90},
		    new object[] {2370, 36, 1, 1, 37, "Studs", 98},
		    new object[] {41, 35, 15, 1, 37, "Arrow tips", 106},
		    new object[] {1325, 35, 1, 2, 75, "Scimitar", 114},
		    new object[] {9425, 36, 1, 1, 37, "Crossbow limb", 122},
		    new object[] {1295, 36, 1, 2, 75, "Longsword", 130},
		    new object[] {865, 37, 5, 1, 37, "Throwing knife", 138},
		    new object[] {1157, 37, 1, 2, 75, "Full Helm", 146},
		    new object[] {1177, 38, 1, 2, 75, "Square shield", 154},
		    new object[] {4544, 49, 1, 1, 37, "Bullseye lantern", 162},
		    new object[] {0, 0, 0, 0, 0, null, 170},
		    new object[] {1339, 39, 1, 3, 112, "Warhammer", 178},
		    new object[] {1365, 40, 1, 3, 112, "Battleaxe", 186},
		    new object[] {1105, 41, 1, 3, 112, "Chainbody", 194},
		    new object[] {1193, 42, 1, 3, 112, "Kiteshield", 202},
		    new object[] {3097, 43, 1, 2, 75, "Claws", 210},
		    new object[] {1311, 44, 1, 3, 112, "2-h sword", 218},
		    new object[] {1083, 46, 1, 3, 112, "Plateskirt", 226},
		    new object[] {1069, 46, 1, 3, 112, "Platelegs", 234},
		    new object[] {1119, 48, 1, 5, 187, "Platebody", 242},
		    new object[] {1269, 35, 1, 2, 75, "Pickaxe", 267}
	    };

        protected static object[][] MITHRIL = {
		    new object[] {1209, 50, 1, 1, 50, "Dagger", 18},
		    new object[] {1355, 51, 1, 1, 50, "Axe", 26},
		    new object[] {1428, 52, 1, 1, 50, "Mace", 34},
		    new object[] {1143, 53, 1, 1, 50, "Medium helm", 42},
		    new object[] {9379, 53, 10, 1, 50, "Crossbow bolts", 50},
		    new object[] {1285, 54, 1, 1, 50, "Sword", 58},
		    new object[] {822, 54, 10, 1, 50, "Dart tips", 66},
		    new object[] {4822, 54, 15, 1, 50, "Nails", 74},
		    new object[] {0, 0, 0, 0, 0, null, 82},
		    new object[] {0, 0, 0, 0, 0, null, 90},
		    new object[] {0, 0, 0, 0, 0, null, 98},
		    new object[] {42, 55, 15, 1, 50, "Arrow tips", 106},
		    new object[] {1329, 55, 1, 2, 100, "Scimitar", 114},
		    new object[] {9427, 56, 1, 1, 50, "Crossbow limb", 122},
		    new object[] {1299, 56, 1, 2, 100, "Longsword", 130},
		    new object[] {866, 57, 5, 1, 50, "Throwing knife", 138},
		    new object[] {1159, 57, 1, 2, 100, "Full Helm", 146},
		    new object[] {1181, 58, 1, 2, 100, "Square shield", 154},
		    new object[] {0, 0, 0, 0, 0, null, 162},
		    new object[] {9416, 59, 1, 1, 50, "Grapple tip", 170},
		    new object[] {1343, 59, 1, 3, 150, "Warhammer", 178},
		    new object[] {1369, 60, 1, 3, 150, "Battleaxe", 186},
		    new object[] {1109, 61, 1, 3, 150, "Chainbody", 194},
		    new object[] {1197, 62, 1, 3, 150, "Kiteshield", 202},
		    new object[] {3099, 63, 1, 2, 100, "Claws", 210},
		    new object[] {1315, 64, 1, 3, 150, "2-h sword", 218},
		    new object[] {1085, 66, 1, 3, 150, "Plateskirt", 226},
		    new object[] {1071, 66, 1, 3, 150, "Platelegs", 234},
		    new object[] {1121, 68, 1, 5, 250, "Platebody", 242},
		    new object[] {1273, 55, 1, 2, 100, "Pickaxe", 267}
	    };

        protected static object[][] ADAMANT = {
		    new object[] {1211, 70, 1, 1, 62.5, "Dagger", 18},
		    new object[] {1357, 71, 1, 1, 62.5, "Axe", 26},
		    new object[] {1430, 72, 1, 1, 62.5, "Mace", 34},
		    new object[] {1145, 73, 1, 1, 62.5, "Medium helm", 42},
		    new object[] {9380, 73, 10, 1, 62.5, "Crossbow bolts", 50},
		    new object[] {1287, 74, 1, 1, 62.5, "Sword", 58},
		    new object[] {823, 74, 10, 1, 62.5, "Dart tips", 66},
		    new object[] {4823, 74, 15, 1, 62.5, "Nails", 74},
		    new object[] {0, 0, 0, 0, 0, null, 82},
		    new object[] {0, 0, 0, 0, 0, null, 90},
		    new object[] {0, 0, 0, 0, 0, null, 98},
		    new object[] {43, 75, 15, 1, 62.5, "Arrow tips", 106},
		    new object[] {1331, 75, 1, 2, 125, "Scimitar", 114},
		    new object[] {9429, 76, 1, 1, 62.5, "Crossbow limb", 122},
		    new object[] {1301, 76, 1, 2, 125, "Longsword", 130},
		    new object[] {867, 77, 5, 1, 62.5, "Throwing knife", 138},
		    new object[] {1161, 77, 1, 2, 125, "Full Helm", 146},
		    new object[] {1183, 78, 1, 2, 125, "Square shield", 154},
		    new object[] {0, 0, 0, 0, 0, null, 162},
		    new object[] {0, 0, 0, 0, 0, null, 170},
		    new object[] {1345, 79, 1, 3, 187.5, "Warhammer", 178},
		    new object[] {1371, 80, 1, 3, 187.5, "Battleaxe", 186},
		    new object[] {1111, 81, 1, 3, 187.5, "Chainbody", 194},
		    new object[] {1199, 82, 1, 3, 187.5, "Kiteshield", 202},
		    new object[] {3100, 83, 1, 2, 125, "Claws", 210},
		    new object[] {1317, 84, 1, 3, 187.5, "2-h sword", 218},
		    new object[] {1091, 86, 1, 3, 187.5, "Plateskirt", 226},
		    new object[] {1073, 86, 1, 3, 187.5, "Platelegs", 234},
		    new object[] {1123, 88, 1, 5, 312, "Platebody", 242},
		    new object[] {1271, 75, 1, 2, 125, "Pickaxe", 267}
	    };

        protected static object[][] RUNE = {
		    new object[] {1213, 85, 1, 1, 75, "Dagger", 18},
		    new object[] {1359, 86, 1, 1, 75, "Axe", 26},
		    new object[] {1432, 87, 1, 1, 75, "Mace", 34},
		    new object[] {1147, 88, 1, 1, 75, "Medium helm", 42},
		    new object[] {9381, 88, 10, 1, 75, "Crossbow bolts", 50},
		    new object[] {1289, 89, 1, 1, 75, "Sword", 58},
		    new object[] {824, 89, 10, 1, 75, "Dart tips", 66},
		    new object[] {4824, 89, 15, 1, 75, "Nails", 74},
		    new object[] {0, 0, 0, 0, 0, null, 82},
		    new object[] {0, 0, 0, 0, 0, null, 90},
		    new object[] {0, 0, 0, 0, 0, null, 98},
		    new object[] {44, 90, 15, 1, 75, "Arrow tips", 106},
		    new object[] {1333, 90, 1, 2, 150, "Scimitar", 114},
		    new object[] {9431, 91, 1, 1, 75, "Crossbow limb", 122},
		    new object[] {1303, 91, 1, 2, 150, "Longsword", 130},
		    new object[] {868, 92, 5, 1, 75, "Throwing knife", 138},
		    new object[] {1163, 92, 1, 2, 150, "Full Helm", 146},
		    new object[] {1185, 93, 1, 2, 150, "Square shield", 154},
		    new object[] {0, 0, 0, 0, 0, null, 162},
		    new object[] {0, 0, 0, 0, 0, null, 170},
		    new object[] {1347, 94, 1, 3, 225, "Warhammer", 178},
		    new object[] {1373, 95, 1, 3, 225, "Battleaxe", 186},
		    new object[] {1113, 96, 1, 3, 225, "Chainbody", 194},
		    new object[] {1201, 97, 1, 3, 225, "Kiteshield", 202},
		    new object[] {3101, 98, 1, 2, 150, "Claws", 210},
		    new object[] {1319, 99, 1, 3, 225, "2-h sword", 218},
		    new object[] {1093, 99, 1, 3, 225, "Plateskirt", 226},
		    new object[] {1079, 99, 1, 3, 225, "Platelegs", 234},
		    new object[] {1127, 99, 1, 5, 375, "Platebody", 242},
		    new object[] {1275, 90, 1, 2, 150, "Pickaxe", 267}
	    };
    }
}