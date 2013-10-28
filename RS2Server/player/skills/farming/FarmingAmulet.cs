using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.farming
{
    internal class FarmingAmulet
    {
        public FarmingAmulet()
        {
        }

        private static object[][] PATCHES = {
		    // min x, min y, extra x, extra y, name
		    new object[] {3064, 3311, 2, 3, "Draynor allotment"}, // Draynor allotment
		    new object[] {2801, 3472, 2, 2, "Catherby allotment"}, // Catherby allotment
		    new object[] {2671, 3364, 4, 1, "Ardougne allotment"}, // Ardougne allotment
		    new object[] {3610, 3531, 3, 3, "Canifis allotment"}, // Canifis allotment
		    new object[] {3225, 3461, 3, 3, "Varrock tree patch"}, // Varrock tree
		    new object[] {3184, 3229, 4, 3, "Lumbridge tree patch"}, // Lumbridge tree
		    new object[] {3002, 3376, 2, 3, "Falador tree patch"}, // Falador tree
		    new object[] {2932, 3433, 2, 3, "Taverly tree patch"}, // Taverly tree
		    new object[] {2858, 3427, 4, 4, "Catherby fruit tree patch"}, // Catherby fruit tree
		    new object[] {2478, 3466, 3, 4, "Gnome stronghold fruit tree patch"}, // Gnome stronghold fruit tree
		    new object[] {2762, 3208, 3, 1, "Brimhaven fruit tree patch"}, // Brimhaven fruit tree
		    new object[] {2489, 3182, 3, 2, "Yanille fruit tree patch"}, // Yanille fruit tree
	    };

        public static bool showOptions(Player p, int item)
        {
            if (item != 12622 || p.getTemporaryAttribute("unmovable") != null)
            {
                return false;
            }
            p.getPackets().softCloseInterfaces();
            p.getPackets().sendChatboxInterface2(232);
            p.getPackets().modifyText("Allotments", 232, 2);
            p.getPackets().modifyText("Trees", 232, 3);
            p.getPackets().modifyText("Fruit trees", 232, 4);
            p.getPackets().modifyText("Exit", 232, 5);
            p.setTemporaryAttribute("dialogue", 450);
            return true;
        }

        public static void displayAllotmentOptions(Player p)
        {
            p.getPackets().sendChatboxInterface2(235);
            p.getPackets().modifyText("Draynor", 235, 2);
            p.getPackets().modifyText("Catherby", 235, 3);
            p.getPackets().modifyText("Ardougne", 235, 4);
            p.getPackets().modifyText("Canifis", 235, 5);
            p.getPackets().modifyText("Go back", 235, 6);
            p.setTemporaryAttribute("dialogue", 451);
        }

        public static void displayTreeOptions(Player p)
        {
            p.getPackets().sendChatboxInterface2(235);
            p.getPackets().modifyText("Varrock", 235, 2);
            p.getPackets().modifyText("Lumbridge", 235, 3);
            p.getPackets().modifyText("Falador", 235, 4);
            p.getPackets().modifyText("Taverly", 235, 5);
            p.getPackets().modifyText("Go back", 235, 6);
            p.setTemporaryAttribute("dialogue", 452);
        }

        public static void displayFruitTreeOptions(Player p)
        {
            p.getPackets().sendChatboxInterface2(235);
            p.getPackets().modifyText("Catherby", 235, 2);
            p.getPackets().modifyText("Gnome Stronghold", 235, 3);
            p.getPackets().modifyText("Brimhaven", 235, 4);
            p.getPackets().modifyText("Yanille", 235, 5);
            p.getPackets().modifyText("Go back", 235, 6);
            p.setTemporaryAttribute("dialogue", 453);
        }

        public static void teleportToPatch(Player p, int option)
        {
            p.setTemporaryAttribute("unmovable", true);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().softCloseInterfaces();
            p.getPackets().displayInterface(120);

            Event teleportToPatchEvent = new Event(2000);
            int teleportToPatchCounter = 0;
            teleportToPatchEvent.setAction(() =>
            {
                if (teleportToPatchCounter == 0)
                {
                    teleportToPatchCounter++;
                    teleportToPatchEvent.setTick(600);
                    p.teleport(new Location((int)PATCHES[option][0] + Misc.random((int)PATCHES[option][2]), (int)PATCHES[option][1] + Misc.random((int)PATCHES[option][3]), 0));
                }
                else
                {
                    teleportToPatchEvent.stop();
                    p.removeTemporaryAttribute("unmovable");
                    p.getPackets().sendMessage("You are teleported to the " + PATCHES[option][4] + ".");
                    p.getPackets().closeInterfaces();
                }
            });
        }
    }
}