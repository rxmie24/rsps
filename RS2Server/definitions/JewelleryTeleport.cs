using RS2.Server.events;
using RS2.Server.minigames.fightcave;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills.magic;

namespace RS2.Server.definitions
{
    internal class JewelleryTeleport
    {
        public JewelleryTeleport()
        {
        }

        private static string[][] LOCATIONS = {
		    // Glory amulet
		    new string[] {"Edgeville", "Karamja", "Draynor Village", "Al Kharid", "Nowhere"},
		    // Trimmed Glory amulet
		    new string[] {"Edgeville", "Karamja", "Draynor Village", "Al Kharid", "Nowhere"},
		    // Ring of dueling
		    new string[] {"Duel Arena", "Castle Wars", "Nowhere"},
		    // Games amulet
		    new string[] {"Burthorpe Games Room", "Pest Control", "Clan Wars", "Wilderness Volcano", "Nowhere"},
	    };

        private static int[][][] TELEPORT_COORDINATES = {
		    // Glory amulet
            new int[][] { //itemIndex
		        new int[] {3087, 3496}, // Edgeville (option)
		        new int[] {2918, 3176}, // Karamja (option)
		        new int[] {3105, 3251}, // Draynor (option)
		        new int[] {3293, 3163} // Al Kharid (option)
		    },
            new int[][] { //itemIndex
		    // Trimmed glory amulet
		        new int[] {3087, 3496}, // Edgeville (option)
		        new int[] {2918, 3176}, // Karamja (option)
		        new int[] {3105, 3251}, // Draynor (option)
		        new int[] {3293, 3163} // Al Kharid (option)
		    },
		    //Duel ring
            new int[][] { //itemIndex
		        new int[] {3316, 3235}, // Duel arena (option)
		        new int[] {2442, 3092} // Castle wars (option)
		    },
		    //Games amulet
            new int[][] { //itemIndex
		        new int[] {2207, 4940}, // Games room (option)
		        new int[] {2658, 2661}, // Pest control (option)
		        new int[] {3268, 3682}, // Clan wars (option)
		        new int[] {3154, 3663} // Bounty hunter (option)
            }
	    };

        private static int[][] JEWELLERY = {
		    new int[] {1704, 1706, 1708, 1710, 1712}, // Glory amulets
		    new int[] {10362, 10360, 10358, 10356, 10354}, // Trimmed glory amulets
		    new int[] {2566, 2564, 2562, 2560, 2558, 2556, 2554, 2552}, // Duel rings
		    new int[] {3867, 3865, 3863, 3861, 3859, 3857, 3855, 3853}, // Games amulets
	    };

        public static bool useJewellery(Player p, int item, int slot, bool wearingItem)
        {
            if (item == 1704 || item == 10362)
            { // Blank amulets
                p.getPackets().sendMessage("This amulet has no charges remaining.");
                return true;
            }
            if (p.getTemporaryAttribute("unmovable") != null || p.getTemporaryAttribute("cantDoAnything") != null)
            {
                return true;
            }
            int index = getItemIndex(item);
            if (index == -1)
            {
                return false;
            }
            string s = index == 2 ? "ring" : "amulet";
            p.getPackets().sendMessage("You rub the " + s + "...");
            p.getPackets().closeInterfaces();
            int interfaceId = index == 2 ? 230 : 235;
            int j = 2;
            p.getPackets().modifyText("Teleport to where?", interfaceId, 1);
            for (int i = 0; i < LOCATIONS[index].Length; i++)
            {
                p.getPackets().modifyText(LOCATIONS[index][i], interfaceId, (i + j));
            }
            if (index == 2)
            {
                p.getPackets().sendChatboxInterface2(interfaceId);
            }
            else
            {
                p.getPackets().sendChatboxInterface2(interfaceId);
            }
            JewellerySlot js = new JewelleryTeleport.JewellerySlot(index, slot, wearingItem);
            p.setTemporaryAttribute("jewelleryTeleport", js);
            return true;
        }

        private static int getItemIndex(int item)
        {
            if (item >= 1706 && item <= 1712)
            { // Normal glory amulets
                return 0;
            }
            else if (item >= 10354 && item <= 10361)
            { // Trimmed glory amulets
                return 1;
            }
            else if (item >= 2552 && item <= 2566)
            { // Duel rings
                return 2;
            }
            else if (item >= 3853 && item <= 3868)
            { // Games amulet
                return 3;
            }
            return -1;
        }

        public static bool teleport(Player p, int opt, JewellerySlot js)
        {
            if (js == null)
            {
                return false;
            }
            if (js.index == -1 || js.index > 3 || opt > 6)
            {
                return false;
            }
            if (!canTeleport(p, js))
            {
                p.getPackets().closeInterfaces();
                return true;
            }
            if ((js.index == 2 && opt == 4) || (js.index != 2 && opt == 6))
            {
                p.getPackets().sendMessage("You stay where you are.");
                p.getPackets().closeInterfaces();
                return true;
            }
            opt -= 2; // Used to get the 'index' from the button id.
            p.setLastGraphics(new Graphics(1684));
            p.setLastAnimation(new Animation(9603));
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            p.setTemporaryAttribute("teleporting", true);
            p.setTemporaryAttribute("unmovable", true);
            p.removeTemporaryAttribute("autoCasting");
            p.removeTemporaryAttribute("lootedBarrowChest");
            p.setTarget(null);
            changeJewellery(p, js);
            int option = opt;
            p.getPackets().closeInterfaces();
            Event teleportEvent = new Event(2000);
            teleportEvent.setAction(() =>
            {
                teleportEvent.stop();
                p.teleport(new Location(TELEPORT_COORDINATES[js.index][option][0], TELEPORT_COORDINATES[js.index][option][1], 0));
                p.setLastAnimation(new Animation(65535));
                Teleport.resetTeleport(p);
                p.removeTemporaryAttribute("unmovable");
            });
            Server.registerEvent(teleportEvent);
            return true;
        }

        private static bool canTeleport(Player p, JewellerySlot js)
        {
            if (p.getTemporaryAttribute("teleporting") != null)
            {
                return false;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().getStatus() < 4)
                {
                    p.getDuel().declineDuel();
                }
                else if (p.getDuel().getStatus() == 5 || p.getDuel().getStatus() == 6)
                {
                    p.getPackets().sendMessage("You cannot teleport whilst in a duel.");
                    return false;
                }
                else if (p.getDuel().getStatus() == 8)
                {
                    p.getDuel().recieveWinnings(p);
                }
            }
            int wildLvl = js.index == 1 || js.index == 2 ? 30 : 20;
            if (Location.inWilderness(p.getLocation()) && p.getLocation().wildernessLevel() >= wildLvl)
            {
                p.getPackets().sendMessage("You cannot teleport above level " + wildLvl + " wilderness!");
                return false;
            }
            if (Location.inFightCave(p.getLocation()))
            {
                FightCave.antiTeleportMessage(p);
                return false;
            }
            if (p.getTemporaryAttribute("teleblocked") != null)
            {
                p.getPackets().sendMessage("A magical force prevents you from teleporting!");
                return false;
            }
            if (Location.inFightPits(p.getLocation()))
            {
                p.getPackets().sendMessage("You are unable to teleport from the fight pits.");
                return false;
            }
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return false;
            }
            return true;
        }

        protected static void changeJewellery(Player p, JewellerySlot js)
        {
            bool gloryAmulet = js.index < 2;
            bool newItem = true;
            string s = js.index == 2 ? "Ring of Dueling" : js.index == 3 ? "Games necklace" : "Amulet of Glory";
            for (int i = 0; i < JEWELLERY[js.index].Length; i++)
            {
                int charges = i;
                if (!js.wearing)
                {
                    Item item = p.getInventory().getSlot(js.slot);
                    if (item.getItemId() == JEWELLERY[js.index][i])
                    {
                        if (gloryAmulet)
                        {
                            charges--;
                        }
                        string t = charges > 1 ? " charges" : " charge";
                        if (charges > 0)
                        {
                            p.getPackets().sendMessage("The " + s + " now has " + charges + t + " .");
                        }
                        else if (gloryAmulet && charges == 0)
                        {
                            p.getPackets().sendMessage("The Amulet of Glory has run out of charges.");
                        }
                        else if (!gloryAmulet && charges <= 1)
                        {
                            newItem = false;
                            p.getPackets().sendMessage("The " + s + " crumbles to dust.");
                            p.getInventory().deleteItem(item.getItemId(), js.slot, 1);
                        }
                        if (newItem)
                        {
                            item.setItemId(JEWELLERY[js.index][i - 1]);
                            p.getPackets().refreshInventory();
                        }
                        break;
                    }
                }
                else
                {
                    Item item = p.getEquipment().getSlot((ItemData.EQUIP)js.slot);
                    if (item.getItemId() == JEWELLERY[js.index][i])
                    {
                        if (gloryAmulet)
                        {
                            charges--;
                        }
                        string t = charges > 1 ? " charges" : " charge";
                        if (charges > 0)
                        {
                            p.getPackets().sendMessage("The " + s + " now has " + charges + t + " .");
                        }
                        else if (gloryAmulet && charges == 0)
                        {
                            p.getPackets().sendMessage("The Amulet of Glory has run out of charges.");
                        }
                        else if (!gloryAmulet && charges <= 1)
                        {
                            newItem = false;
                            p.getPackets().sendMessage("The " + s + " crumbles to dust.");
                            item.setItemId(-1);
                            item.setItemAmount(0);
                        }
                        if (newItem)
                        {
                            item.setItemId(JEWELLERY[js.index][i - 1]);
                        }
                        p.getPackets().refreshEquipment();
                        break;
                    }
                }
            }
        }

        public class JewellerySlot
        {
            public bool wearing;
            public int slot;
            public int index;

            public JewellerySlot(int index, int slot, bool wearing)
            {
                this.index = index;
                this.slot = slot;
                this.wearing = wearing;
            }
        }
    }
}