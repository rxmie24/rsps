namespace RS2.Server.player.skills.thieving
{
    internal class ThievingData
    {
        public ThievingData()
        {
        }

        protected static int LOCKPICK = 1523;

        protected static int[][] NPCS = {
		    new int[] {1, 2, 3, 7875, 7877, 7879}, // Men.
		    new int[] {4, 5, 6, 7881, 7883}, // Women.
		    new int[] {7}, // Farmer.
		    new int[] {1715}, // Female HAM members.
		    new int[] {1716}, // Male HAM members.
		    new int[] {15}, // Warrior
		    new int[] {187}, // Rogue
		    //Cave goblin here (no need for them in an RSPS..)
		    new int[] {2234}, // Master farmer.
		    new int[] {9, 32, 296, 297, 298, 299}, // Guard.
		    //'Fremmenik' here
		    new int[] {6174, 1880}, // Bandit guard.
		    new int[] {1926, 1931}, // Desert bandits.
		    new int[] {23, 26}, // Knights
		    new int[] {34}, // Watchman
		    //Menaphite thug here.
		    new int[] {20}, // Paladin
		    new int[] {66, 67, 68}, // Gnomes.
		    new int[] {21}, // Hero.
		    new int[] {2363, 2364, 2365, 2366, 2367}, // Elf
	    };

        protected static int[] NPC_LVL = {
		    1, // Men.
		    1, // Women.
		    10, // Farmer.
		    15, // Female HAM.
		    20, // Male HAM.
		    25, // Warrior.
		    32, // Rogue.
		    38, // Master farmer.
		    40, // Guard.
		    45, // Bandit guard.
		    53, // Desert bandits.
		    55, // Knights.
		    65, // Watchman.
		    70, // Paladin.
		    75, // Gnome.
		    80, // Hero.
		    85, // Elf.
	    };

        protected static int[][] NPC_REWARD = {
		    new int[] {995}, // Men.
		    new int[] {995}, // Women.
		    new int[] {995, 5318}, // Farmer
		    new int[] {4298, 4300, 4302, 4304, 4306, 4308, 4310, 1511, 688, 689, 687, 686, 1605, 314, 946, 995, 1267, 371, 199, 453, 444, 201, 203, 205, 175, 884, 2138, 385}, // Female H.A.M TODO - lvl 1 clue
		    new int[] {4298, 4300, 4302, 4304, 4306, 4308, 4310, 1511, 688, 689, 687, 686, 1605, 314, 946, 995, 1267, 371, 199, 453, 444, 201, 203, 205, 175, 884, 2138, 385}, // Male H.A.M TODO - lvl 1 clue
		    new int[] {995}, // Warrior
		    new int[] {995, 1523, 954, 554, 555, 556, 175}, // Rogue
		    new int[] {5318, 5319, 5320, 5321, 5322, 5323, 5324, 5305, 5306, 5307, 5308, 5309, 5310, 5311, 5096, 5097, 5098, 5099, 5100, 5101, 5102, 5103, 5104, 5105, 5106}, // Master farmer.
		    new int[] {995}, // Guard
		    new int[] {995, 175, 1523, 1823}, // Bandit guard
		    new int[] {995, 175, 1523, 1823}, // Bandits
		    new int[] {995}, // Knight
		    new int[] {995, 2309}, // Watchman
		    new int[] {995, 562}, // Paladin
		    new int[] {995, 2357, 561}, // Gnome
		    new int[] {995}, // Hero
		    new int[] {995}, // Elf
	    };

        protected static int[][] NPC_REWARD_AMOUNT = {
		    new int[] {35}, // Men.
		    new int[] {35}, // Women.
		    new int[] {150, 1}, // Farmer
		    new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 1, 6000, 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 1, 1}, // Female H.A.M
		    new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 1, 6000, 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 1, 1}, // Male H.A.M
		    new int[] {300}, // Warrior
		    new int[] {500, 1, 1, 10, 10, 10, 1}, // Rogue
		    new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // Master farmer.
		    new int[] {500}, // Guard
		    new int[] {400, 1, 1, 1}, // Bandit guard
		    new int[] {340, 1, 1, 1}, // Bandit
		    new int[] {550, 1}, // Knight
		    new int[] {600, 1}, // Watchman
		    new int[] {750, 5}, // Paladin
		    new int[] {900, 1, 2}, // Gnome
		    new int[] {1100}, // Hero
		    new int[] {1500}, // Elf
	    };

        protected static double[] NPC_XP = {
		    8, // Men & women.
		    8, // Women.
		    14.5, // Farmer.
		    18.5, // Female HAM.
		    22.5, // Male HAM.
		    26, // Warrior.
		    36.5, // Rogue.
		    43, // Master farmer.
		    46.8, // Guard.
		    65, // Bandit guard.
		    79.4, // Desert bandit.
		    84.3, // Knights.
		    137.5, // Watchman.
		    151.8, // Paladin.
		    198.3, // Gnome.
		    273.3, // Hero
	    };

        protected static string[] NPC_NAMES = {
		    "man's",
		    "woman's",
		    "farmer's",
		    "H.A.M member's",
		    "H.A.M member's",
		    "warrior's",
		    "rogue's",
		    "master farmer's",
		    "guard's",
		    "guard's",
		    "bandit's",
		    "knight's",
		    "watchman's",
		    "paladin's",
		    "gnome's",
		    "hero's",
		    "elf's"
	    };

        protected static int[][] STALLS = {
		    new int[] {4708, 4706}, // Vegatable
		    new int[] {-31152}, // Baker
		    new int[] {4876}, // AA general
		    new int[] {635}, // Tea
		    new int[] {4874}, // AA crafting
		    new int[] {4875}, // AA food
		    new int[] {-31153}, // Silk
		    new int[] {14011}, // Wine
		    new int[] {7053}, // Seed
		    new int[] {-31149}, // Fur
		    new int[] {4707, 4705}, // Fish
		    new int[] {-31154}, // Silver
		    new int[] {4877}, // AA magic
		    new int[] {4878}, // AA scimitar
		    new int[] {-31150}, // Spice stall
		    new int[] {-31151}, // Gem stall
	    };

        protected static int[] STALL_LVL = {
		    2, // Vegatable
		    5, // Baker
		    5, // AA General
		    5, // Tea
		    5, // AA crafting
		    5, // AA food
		    20, // Silk
		    22, // Wine
		    27, // Seed
		    35, // Fur
		    42, // Fish
		    50, // Silver
		    65, // AA magic
		    65, // AA scimitar
		    65, // Spice stall
		    75, // Gem stall
	    };

        protected static int[][] STALL_REWARD = {
		    new int[] {1942, 1965, 1957, 1550, 1982}, // Vegatable
		    new int[] {2309, 1891, 1901}, // Baker
		    new int[] {2347, 1931, 590}, // AA General
		    new int[] {1978}, // Tea
		    new int[] {1755, 1592, 1597}, // AA crafting
		    new int[] {1963}, // AA food
		    new int[] {950}, // Silk
		    new int[] {1935, 1937, 1987, 1993, 7919}, // Wine
		    new int[] {5318, 5319, 5320, 5321, 5322, 5323, 5324, 5096, 5097, 5098, 5099, 5100, 5101, 5102, 5103, 5104, 5105, 5106}, // Seed
		    new int[] {958}, // Fur
		    new int[] {377, 371, 359}, // Fish
		    new int[] {442}, // Silver
		    new int[] {554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566}, // AA magic
		    new int[] {1321, 1323, 1325, 1327, 1329, 1331}, // AA scimitar
		    new int[] {2007}, // Spice stall
		    new int[] {1619, 1621, 1623}, // Gem stall
	    };

        protected static int[][] STALL_REWARD_AMOUNTS = {
		    new int[] {1, 1, 1, 1, 1}, // Vegatable
		    new int[] {1, 1, 1}, // Baker
		    new int[] {1, 1, 1}, // AA General
		    new int[] {1}, // Tea
		    new int[] {1, 1, 1}, // AA crafting
		    new int[] {1}, // AA food
		    new int[] {1}, // Silk
		    new int[] {1, 1, 1, 1, 1}, // Wine
		    new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // Seed
		    new int[] {1}, // Fur
		    new int[] {1, 1, 1}, // Fish
		    new int[] {1}, // Silver
		    new int[] {10, 10, 10, 10, 10, 10, 1, 1, 1, 1, 1, 1, 1}, // AA magic
		    new int[] {1, 1, 1, 1, 1, 1}, // AA scimitar
		    new int[] {1}, // Spice
		    new int[] {1, 1, 1}, // Gem stall
	    };

        protected static double[] STALL_XP = {
		    10, // Vegatable
		    16, // Baker
		    10, // AA General
		    16, // Tea
		    10, // AA crafting
		    16, // AA food
		    24, // Silk
		    27, // Wine
		    32, // Seed
		    36, // Fur
		    42, // Fish
		    54, // Silver
		    100, // AA magic
		    160, // AA scimitar
		    81.3, // Spice stall
		    120, // Gem stall
	    };

        protected static string[] STALL_NAMES = {
	    };

        protected static int[] CHESTS = {
		    2566, // Coin chest.
		    2567, // Nature rune chest.
		    2568, // Coin chest #2.
		    2569, // Blood rune chest.
		    2570, // King Lathas chest.
	    };

        protected static int[][] CHEST_REWARD = {
		    new int[] {995}, // Ardougne coin chest.
		    new int[] {995, 561}, // Nature rune chest.
		    new int[] {995}, // Coin chest #2.
		    new int[] {995, 565}, // Blood rune chest.
		    new int[] {995, 383, 449, 1623}, // King Lathas chest.
	    };

        protected static int[][] CHEST_REWARD_AMOUNTS = {
		    new int[] {3000}, // Ardougne coin chest.
		    new int[] {750, 2}, // Nature rune chest.
		    new int[] {9000}, // Coin chest #2.
		    new int[] {1500, 10}, // Blood rune chest.
		    new int[] {10000, 1, 1, 1}, // King Lathas chest.
	    };

        protected static int[] CHEST_LVL = {
		    13, // Ardougne coin chest.
		    28, // Nature rune chest.
		    43, // Coin chest #2.
		    59, // Blood rune chest.
		    72, // King Lathas chest.
		    47, // Arrowheads chest.
	    };

        protected static double[] CHEST_XP = {
		    7.5, // Ardougne coin chest.
		    25, // Nature rune chest.
		    125, // Coin chest #2.
		    250, // Blood rune chest.
		    500, // King Lathas chest
		    150, // Arrowheads chest.
	    };

        protected static int[] HERB_SEEDS = {
		    5291, 5292, 5293, 5294, 5295, 5296, 5297, 5298, 5299, 5300, 5301
	    };
    }
}