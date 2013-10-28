using RS2.Server.events;
using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.crafting
{
    internal class Jewellery : CraftingData
    {
        public Jewellery()
        {
        }

        public static void showCutGemOption(Player p, int index)
        {
            int amount = p.getInventory().getItemAmount((int)GEMS[index][0]);
            if (amount > 1)
            {
                string s = "<br><br><br><br>";
                p.getPackets().itemOnInterface(309, 2, 190, (int)GEMS[index][1]);
                p.getPackets().modifyText(s + (string)GEMS[index][4], 309, 6);
                p.getPackets().sendChatboxInterface(309);
                p.setTemporaryAttribute("craftType", index + 50);
                return;
            }
            cutGem(p, index + 50, 1, true);
        }

        public static void cutGem(Player p, int index, int amount, bool newCut)
        {
            index -= 50;
            if (newCut)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(5, index, amount, (double)GEMS[index][3], (int)GEMS[index][1], (string)GEMS[index][4], (int)GEMS[index][2]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 5)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            if (!p.getInventory().hasItem(CHISEL))
            {
                p.getPackets().sendMessage("You cannot cut gems without a chisel.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem((int)GEMS[item.getCraftItem()][0]))
            {
                if (newCut)
                {
                    p.getPackets().sendMessage("You have no " + item.getMessage() + " to cut.");
                }
                else
                {
                    p.getPackets().sendMessage("You have no more " + item.getMessage() + "'s to cut.");
                }
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to cut that gem.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getInventory().deleteItem((int)GEMS[item.getCraftItem()][0]))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                p.setLastAnimation(new Animation((int)GEMS[item.getCraftItem()][5]));
                p.getPackets().sendMessage("You cut the " + item.getMessage() + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event cutMoreGemEvent = new Event(1500);
                cutMoreGemEvent.setAction(() =>
                {
                    cutGem(p, -1, -1, false);
                    cutMoreGemEvent.stop();
                });
                Server.registerEvent(cutMoreGemEvent);
            }
        }

        public static void displayJewelleryInterface(Player p)
        {
            for (int i = 0; i < JEWELLERY_INTERFACE_VARS.Length; i++)
            {
                if (p.getInventory().hasItem(JEWELLERY_INTERFACE_VARS[i][0]))
                {
                    p.getPackets().showChildInterface(675, JEWELLERY_INTERFACE_VARS[i][1], false);
                    displayJewellery(p, i);
                }
            }
            p.getPackets().displayInterface(675);
        }

        private static void displayJewellery(Player p, int index)
        {
            object[][] items = getItemArray(p, index);
            if (items == null)
            {
                return;
            }
            int SIZE = 100;
            int interfaceSlot = JEWELLERY_INTERFACE_VARS[index][2];
            p.getPackets().itemOnInterface(675, interfaceSlot, SIZE, (int)items[0][0]); // display the gold item since we know we have the bar
            interfaceSlot += 2;
            for (int i = 1; i < items.Length; i++)
            { // i is set to 1 to ignore the gold
                for (int j = 0; j < items[i].Length; j++)
                {
                    if (p.getInventory().hasItem((int)GEMS[i - 1][1]) && p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) >= (int)items[i][1])
                    { // lower it down 1 because of the gold..0 in jeweller is 0, but in GEMS is a sapphire
                        p.getPackets().itemOnInterface(675, interfaceSlot, SIZE, (int)items[i][0]);
                    }
                    else
                    {
                        p.getPackets().itemOnInterface(675, interfaceSlot, SIZE, NULL_JEWELLERY[index]);
                    }
                }
                interfaceSlot += 2;
            }
        }

        private static object[][] getItemArray(Player p, int index)
        {
            switch (index)
            {
                case 0: return RINGS;
                case 1: return NECKLACES;
                case 2: return AMULETS;
                case 3: return BRACELETS;
            }
            return null;
        }

        public static void makeJewellery(Player p, int buttonId, int amount, bool newCraft)
        {
            int index = -1;
            if (newCraft)
            {
                int itemType = getIndex(buttonId, true);
                object[][] items = getItemArray(p, itemType);
                index = getIndex(buttonId, false);
                if (index == -1)
                {
                    p.getPackets().closeInterfaces();
                    return;
                }
                p.setTemporaryAttribute("craftItem", new CraftItem(itemType, index, amount, (double)items[index][2], (int)items[index][0], (string)items[index][3], (int)items[index][1]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0)
            {
                Crafting.resetCrafting(p);
                p.getPackets().closeInterfaces();
                return;
            }
            p.getPackets().closeInterfaces();
            index = item.getCraftItem();
            Console.WriteLine(index);
            int index2 = index;
            string gemType = (string)GEMS[index2 - 1][4];
            string s = index == 3 ? "an" : "a";
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to craft a " + gemType + " " + item.getMessage() + ".");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(JEWELLERY_INTERFACE_VARS[item.getCraftType()][0]))
            {
                p.getPackets().sendMessage("You need " + s + " " + item.getMessage() + " mould to craft that.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(GOLD_BAR))
            {
                p.getPackets().sendMessage("You need a Gold bar to craft an item of jewellery.");
                Crafting.resetCrafting(p);
                return;
            }
            if (index2 > 0)
            { // We dont need gems for gold bars
                if (!p.getInventory().hasItem((int)GEMS[index2 - 1][1]))
                {
                    p.getPackets().sendMessage("You don't have a cut " + (string)GEMS[index2 - 1][4] + ".");
                    Crafting.resetCrafting(p);
                    return;
                }
            }
            if (p.getInventory().deleteItem(GOLD_BAR))
            {
                if (index2 > 0)
                {
                    if (!p.getInventory().deleteItem((int)GEMS[index2 - 1][1]))
                    {
                        return;
                    }
                }
                p.setLastAnimation(new Animation(3243));
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                string message = index2 == 0 ? "You smelt the gold bar into a Gold " + item.getMessage() : "You fuse the Gold bar and " + (string)GEMS[index2 - 1][4] + " together, and create a " + (string)GEMS[index2 - 1][4] + " " + item.getMessage();
                p.getPackets().sendMessage(message + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event makeMoreJewelleryEvent = new Event(1500);
                makeMoreJewelleryEvent.setAction(() =>
                {
                    makeJewellery(p, -1, -1, false);
                    makeMoreJewelleryEvent.stop();
                });
                Server.registerEvent(makeMoreJewelleryEvent);
            }
        }

        private static int getIndex(int buttonId, bool getItemType)
        {
            for (int i = 0; i < JEWELLERY_BUTTON_IDS.Length; i++)
            {
                for (int j = 0; j < JEWELLERY_BUTTON_IDS[i].Length; j++)
                {
                    if (buttonId == JEWELLERY_BUTTON_IDS[i][j])
                    {
                        return getItemType ? i : j;
                    }
                }
            }
            return -1;
        }

        public static void showStringAmulet(Player p, int index)
        {
            int amount = p.getInventory().getItemAmount(BALL_OF_WOOL);
            int amount2 = p.getInventory().getItemAmount(STRUNG_AMULETS[index][1]);
            if (amount > 1 && amount2 > 1)
            {
                string s = "<br><br><br><br>";
                p.getPackets().itemOnInterface(309, 2, 80, STRUNG_AMULETS[index][0]);
                p.getPackets().modifyText(s + (string)GEMS[index - 1][4] + " Amulet", 309, 6);
                p.getPackets().sendChatboxInterface(309);
                p.setTemporaryAttribute("craftType", index + 100);
                return;
            }
            stringAmulet(p, index + 100, 1, true);
        }

        public static void stringAmulet(Player p, int index, int amount, bool newString)
        {
            index -= 100;
            if (newString)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(4, index, amount, STRINGING_XP, STRUNG_AMULETS[index][0], (string)GEMS[index][4] + " amulet", 1));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 4)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            if (!p.getInventory().hasItem(BALL_OF_WOOL))
            {
                p.getPackets().sendMessage("You do not have a Ball of wool.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(STRUNG_AMULETS[item.getCraftItem()][1]))
            {
                string s = item.getCraftItem() == 1 || item.getCraftItem() == 5 ? "an" : "a";
                p.getPackets().sendMessage("You don't have " + s + " " + GEMS[item.getCraftItem()][4] + " unstrung amulet to string.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getInventory().deleteItem(STRUNG_AMULETS[item.getCraftItem()][1]) && p.getInventory().deleteItem(BALL_OF_WOOL))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, STRINGING_XP);
                p.getPackets().sendMessage("You add a string to the amulet.");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event stringMoreAmuletEvent = new Event(1000);
                stringMoreAmuletEvent.setAction(() =>
                {
                    stringAmulet(p, -1, -1, false);
                    stringMoreAmuletEvent.stop();
                });
                Server.registerEvent(stringMoreAmuletEvent);
            }
        }
    }
}