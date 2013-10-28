namespace RS2.Server.player.skills.slayer
{
    internal class SlayerData
    {
        public SlayerData()
        {
        }

        protected static int BUY_XP_POINTS = 400;
        protected static int BUY_RING_POINTS = 75;
        protected static int BUY_DART_POINTS = 35;
        protected static int BUY_BOLT_POINTS = 35;
        protected static int BUY_ARROW_POINTS = 35;
        protected static int REASSIGN_POINTS = 30;
        protected static int PERM_REMOVE_POINTS = 100;

        protected static object[][] SLAYER_MASTERS = {
		    //Master id, combat level needed, name, min amount, max amount (plus min amount)
		    new object[] {8273, 3, "Turael", 15, 35, "east Burthorpe", "Travel north of Taverly to get here.", "Burthorpe"},
		    new object[] {8274, 30, "Mazchna", 20, 35, "north west Canifis", "Canifis is quite a walk, east of Varrock.", "Canifis"},
		    new object[] {1597, 60, "Vannaka", 30, 40, "Edgeville dungeon", "Use the trapdoor in Edgeville to get here.", "Edgeville"},
		    new object[] {1598, 80, "Chaeldar", 50, 35, "Zanaris", "You must have completed Lost City quest to get here.", "Zanaris"},
		    new object[] {8275, 100, "Duradel", 40, 60, "Shilo Village", "I hear there is a Gnome who can direct you here.", "Shilo"}
	    };

        protected static object[][] TURAEL_TASKS = {
		    new object[] {"Rats"},
		    new object[] {"Birds", 41},
		    new object[] {"Bats"},
		    new object[] {"Bears"},
		    new object[] {"Cows", 81, 397},
		    new object[] {"Crawling hands"},
		    new object[] {"Cave bugs"},
		    new object[] {"Cave slime"},
		    new object[] {"Banshees"},
		    new object[] {"Dwarves"},
		    new object[] {"Ghosts"},
		    new object[] {"Goblins"},
		    new object[] {"Spiders"},
		    new object[] {"Monkeys"},
		    new object[] {"Scorpions"}
	    };

        protected static object[][] MAZCHNA_TASKS = {
		    new object[] {"Bats"},
		    new object[] {"Bears"},
		    new object[] {"Crawling hands"},
		    new object[] {"Cave bugs"},
		    new object[] {"Cave slime"},
		    new object[] {"Banshees"},
		    new object[] {"Ghosts"},
		    new object[] {"Moss giants", 4688},
		    new object[] {"Spiders"},
		    new object[] {"Cave crawlers"},
		    new object[] {"Cockatrice"},
		    new object[] {"Zombies"},
		    new object[] {"Hill giants"},
		    new object[] {"Hobgoblins"},
		    new object[] {"Icefiends"},
		    new object[] {"Pyrefiends"},
		    new object[] {"Skeletons"},
		    new object[] {"Wolves"},
		    new object[] {"Kalphites"},
		    new object[] {"Dogs", 1594}
	    };

        protected static object[][] VANNAKA_TASKS = {
		    new object[] {"Bats"},
		    new object[] {"Crawling hands"},
		    new object[] {"Cave bugs"},
		    new object[] {"Cave slime"},
		    new object[] {"Banshees"},
		    new object[] {"Rock slug"},
		    new object[] {"Spiders"},
		    new object[] {"Cave crawlers"},
		    new object[] {"Cockatrice"},
		    new object[] {"Zombies"},
		    new object[] {"Hill giants"},
		    new object[] {"Hobgoblins"},
		    new object[] {"Dagannoth"},
		    new object[] {"Icefiends"},
		    new object[] {"Pyrefiends"},
		    new object[] {"Gargoyles"},
		    new object[] {"Skeletons"},
		    new object[] {"Wolves"},
		    new object[] {"Kalphites"},
		    new object[] {"Aberrant spectres"},
		    new object[] {"Basilisks"},
		    new object[] {"Dogs", 1594},
		    new object[] {"Bloodvelds"},
		    new object[] {"Dust devils"},
		    new object[] {"Green dragons"},
		    new object[] {"Ice giants"},
		    new object[] {"Ice warriors"},
		    new object[] {"Jellies"},
		    new object[] {"Infernal mages"},
		    new object[] {"Lesser demons"},
		    new object[] {"Moss giants", 4688},
		    new object[] {"Fire giants", 1584, 1586, 7003},
		    new object[] {"Turoth"}
	    };

        protected static object[][] CHAELDAR_TASKS = {
		    new object[] {"Banshees"},
		    new object[] {"Spiders"},
		    new object[] {"Cave crawlers"},
		    new object[] {"Cockatrice"},
		    new object[] {"Rock slug"},
		    new object[] {"Hill giants"},
		    new object[] {"Hobgoblins"},
		    new object[] {"Pyrefiends"},
		    new object[] {"Kalphites"},
		    new object[] {"Aberrant spectres"},
		    new object[] {"Basilisks"},
		    new object[] {"Bloodvelds"},
		    new object[] {"Dust devils"},
		    new object[] {"Nechryael"},
		    new object[] {"Dogs", 1594},
		    new object[] {"Blue dragons"},
		    new object[] {"Ice giants"},
		    new object[] {"Gargoyles"},
		    new object[] {"Jellies"},
		    new object[] {"Infernal mages"},
		    new object[] {"Lesser demons"},
		    new object[] {"Fire giants", 1584, 1586, 7003},
		    new object[] {"Turoth"},
		    new object[] {"Kurasks"},
		    new object[] {"Dagannoth"},
		    new object[] {"Cave horrors"},
		    new object[] {"Bronze dragons"},
		    new object[] {"Waterfiends"}
	    };

        protected static object[][] DURADEL_TASKS = {
		    new object[] {"Aberrant spectres"},
		    new object[] {"Gargoyles"},
		    new object[] {"Abyssal demons"},
		    new object[] {"Black demons"},
		    new object[] {"Black Dragons"},
		    new object[] {"Bloodvelds"},
		    new object[] {"Dogs", 1594},
		    new object[] {"Dark beasts"},
		    new object[] {"Goraks"},
		    new object[] {"Dagannoth"},
		    new object[] {"Kalphites"},
		    new object[] {"Iron dragons"},
		    new object[] {"Steel dragons"},
		    new object[] {"Mithril dragons"},
		    new object[] {"Nechryael"},
		    new object[] {"Spiritual mages"},
		    new object[] {"Suqahs"},
		    new object[] {"Greater demons"},
		    new object[] {"Fire giants", 1584, 1586, 7003},
		    new object[] {"Hellhounds"}
	    };
    }
}