using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.definitions.areas
{
    internal class BoatOptions
    {
        private static int[][] LOCATIONS = {
		    new int[] {3508, 3471, 0}, //Canifis
		    new int[] {2548, 3758, 0}, //Waterbirth isle
		    new int[] {2659, 2676, 0}, //Pest control
		    new int[] {2875, 3546, 0}, //Warrior guild
		    new int[] {2414, 3893, 0}, //Fremmenik shore
	    };

        private static string[] DESTINATION_NAMES = {
		    "Canifis",
		    "Waterbirth Isle",
		    "Pest Control",
		    "The Warrior Guild",
		    "Fremmenik Shore",
	    };

        private static string[] DESTINATION_MESSAGES = {
		    "It's a tight squeeze through the swamp, but the boat soon arrives in Canifis.",
		    "The boat docks at Waterbirth Isle.",
		    "The Squire welcomes you to Pest Control Island.",
		    "The boat drops you off at shore, you walk for a short distance to The Warrior Guild.",
		    "Fremmenik Shore, an icy tundra, you see dungeon entrances in the distance.",
	    };

        public BoatOptions()
        {
        }

        public static bool interactWithBoatNPC(Player p, Npc n)
        {
            int id = n.getId();
            if (id != 4540 && id != 1304 && id != 2436 && id != 3781 && id != 1361 && id != 4962)
            {
                return false;
            }
            p.setEntityFocus(n.getClientIndex());
            AreaEvent interactWithBoatNPCAreaEvent = new AreaEvent(p, n.getLocation().getX() - 1, n.getLocation().getY() - 1, n.getLocation().getX() + 1, n.getLocation().getY() + 1);
            interactWithBoatNPCAreaEvent.setAction(() =>
            {
                n.setFaceLocation(p.getLocation());
                p.setFaceLocation(n.getLocation());
                p.setEntityFocus(65535);
                switch (n.getId())
                {
                    case 4540: // Home boat
                        showBentleyDialogue(p, 240);
                        break;

                    case 1304: // Canifis sailor
                        showCanifisSailorDialogue(p, 280);
                        break;

                    case 2436: // Waterbirth isle
                        showJarvaldDialogue(p, 300);
                        break;

                    case 3781: // Pest control squire
                        showSquireDialogue(p, 340);
                        break;

                    case 1361: // Warrior guild
                        showArnorDialogue(p, 370);
                        break;

                    case 4962: // fremmenik shore
                        showCaptainBarnabyDialogue(p, 410);
                        break;
                }
            });
            Server.registerCoordinateEvent(interactWithBoatNPCAreaEvent);
            return true;
        }

        public static void showCanifisSailorDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 280:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you take me back to Oo'glog please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 281;
                    break;

                case 281:
                    p.getPackets().sendNPCHead(1304, 241, 2);
                    p.getPackets().modifyText("Sailor", 241, 3);
                    p.getPackets().modifyText("As you wish, i'll fetch the boat.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 282;
                    break;

                case 282:
                    travel(p, 0, true);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        public static void showJarvaldDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 300:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you take me back to Oo'glog please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 301;
                    break;

                case 301:
                    p.getPackets().sendNPCHead(2436, 241, 2);
                    p.getPackets().modifyText("Jarvald", 241, 3);
                    p.getPackets().modifyText("No problem. Off we go!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 302;
                    break;

                case 302:
                    travel(p, 1, true);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        public static void showSquireDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 340:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you take me back to Oo'glog please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 341;
                    break;

                case 341:
                    p.getPackets().sendNPCHead(3781, 241, 2);
                    p.getPackets().modifyText("Squire", 241, 3);
                    p.getPackets().modifyText("Certainly! Please visit Pest Control again soon.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 342;
                    break;

                case 342:
                    travel(p, 2, true);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        public static void showArnorDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 370:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you take me back to Oo'glog please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 371;
                    break;

                case 371:
                    p.getPackets().sendNPCHead(1361, 241, 2);
                    p.getPackets().modifyText("Arnor", 241, 3);
                    p.getPackets().modifyText("Of course, follow me.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 372;
                    break;

                case 372:
                    travel(p, 3, true);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        public static void showCaptainBarnabyDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 410:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you take me back to Oo'glog please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 411;
                    break;

                case 411:
                    p.getPackets().sendNPCHead(4962, 241, 2);
                    p.getPackets().modifyText("Captain Barnaby", 241, 3);
                    p.getPackets().modifyText("Yes! it's freezing here, let's go!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 412;
                    break;

                case 412:
                    travel(p, 4, true);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        public static void showBentleyDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 240:
                    p.getPackets().sendNPCHead(4540, 241, 2);
                    p.getPackets().modifyText("Captain Bentley", 241, 3);
                    p.getPackets().modifyText("Well, hello there " + p.getLoginDetails().getUsername() + ", ready to set sail?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 241;
                    break;

                case 241:
                    p.getPackets().modifyText("I'd like to travel to..", 235, 1);
                    p.getPackets().modifyText("Canifis", 235, 2);
                    p.getPackets().modifyText("Waterbirth Isle", 235, 3);
                    p.getPackets().modifyText("Pest Control", 235, 4);
                    p.getPackets().modifyText("Warrior Guild", 235, 5);
                    p.getPackets().modifyText("Fremmenik Shore", 235, 6);
                    p.getPackets().sendChatboxInterface2(235);
                    newStatus = 242;
                    break;

                case 242: // Canifis
                    travel(p, 0, false);
                    break;

                case 243: // Waterbirth isle
                    travel(p, 1, false);
                    break;

                case 244: // Pest control
                    travel(p, 2, false);
                    break;

                case 245: // Warrior guild
                    int attackLevel = p.getSkills().getMaxLevel(Skills.SKILL.ATTACK);
                    int strengthLevel = p.getSkills().getMaxLevel(Skills.SKILL.STRENGTH);
                    bool hasA99 = attackLevel == 99 || strengthLevel == 99;
                    if (((attackLevel + strengthLevel) >= 130) || hasA99)
                    {
                        travel(p, 3, false);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead(4540, 243, 2);
                        p.getPackets().modifyText("Captain Bentley", 243, 3);
                        p.getPackets().modifyText("I'm sorry " + p.getLoginDetails().getUsername() + ", I cannot take you there.", 243, 4);
                        p.getPackets().modifyText("A combined Attack & Strength level of 130 is ", 243, 5);
                        p.getPackets().modifyText("required to use The Warrior Guild.", 243, 6);
                        p.getPackets().animateInterface(9827, 243, 2);
                        p.getPackets().sendChatboxInterface2(243);
                    }
                    break;

                case 246: // Fremmenik shore
                    travel(p, 4, false);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }

        private static void travel(Player p, int index, bool returning)
        {
            p.setTemporaryAttribute("unmovable", true);
            p.getPackets().displayInterface(120);
            if (returning)
            {
                p.getPackets().sendMessage("You sail off back to Oo'glog..");
            }
            else
            {
                p.getPackets().sendMessage("You climb aboard Captain Bentley's boat and set sail to " + DESTINATION_NAMES[index] + ".");
            }
            Event travelEvent = new Event(2000);
            int travelCounter = 0;
            travelEvent.setAction(() =>
            {
                if (travelCounter == 0)
                {
                    travelCounter++;
                    travelEvent.setTick(600);
                    if (returning)
                    {
                        p.teleport(new Location(2622, 2857, 0));
                    }
                    else
                    {
                        p.teleport(new Location(LOCATIONS[index][0], LOCATIONS[index][1], LOCATIONS[index][2]));
                    }
                }
                else
                {
                    travelEvent.stop();
                    p.getPackets().sendOverlay(170);
                    p.removeTemporaryAttribute("unmovable");
                    p.getPackets().sendMessage(returning ? "The boat arrives back in Oo'glog." : DESTINATION_MESSAGES[index]);
                    p.getPackets().closeInterfaces();
                    Event removeOverlayEvent = new Event(2000);
                    removeOverlayEvent.setAction(() =>
                    {
                        removeOverlayEvent.stop();
                        p.getPackets().sendRemoveOverlay();
                        if (index == 1)
                        {
                            p.removeTemporaryAttribute("snowInterface");
                        }
                    });
                    Server.registerEvent(removeOverlayEvent);
                }
            });
            Server.registerEvent(travelEvent);
        }
    }
}