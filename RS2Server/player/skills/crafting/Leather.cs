using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.crafting
{
    internal class Leather : CraftingData
    {
        public Leather()
        {
        }

        public static void openLeatherInterface(Player p, int type)
        {
            p.setTemporaryAttribute("leatherCraft", type);
            if (type == 4)
            { // Cowhide
                p.getPackets().displayInterface(154);
                return;
            }
            int i = type;
            int k = 2;
            int l = 8;
            string s = "<br><br><br><br>";
            for (int j = 0; j < 3; j++)
            {
                p.getPackets().itemOnInterface(304, k, 180, (int)LEATHER_ITEMS[i][0]);
                p.getPackets().modifyText(s + (string)LEATHER_ITEMS[i][4], 304, l);
                l += 4;
                i += 4;
                k++;
            }
            p.getPackets().sendChatboxInterface(304);
        }

        public static void craftDragonHide(Player p, int amount, int itemIndex, int leatherType, bool newCraft)
        {
            if (newCraft)
            {
                itemIndex = leatherType != 0 ? itemIndex += leatherType : itemIndex;
                p.setTemporaryAttribute("craftItem", new CraftItem(leatherType, itemIndex, amount, (double)LEATHER_ITEMS[itemIndex][2], (int)LEATHER_ITEMS[itemIndex][0], (string)LEATHER_ITEMS[itemIndex][4], (int)LEATHER_ITEMS[itemIndex][1]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            int index = item.getCraftItem();
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to craft that item.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItemAmount(TANNED_HIDE[item.getCraftType()], (int)LEATHER_ITEMS[index][3]))
            {
                p.getPackets().sendMessage("You need " + (int)LEATHER_ITEMS[index][3] + " dragonhide to craft that.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(NEEDLE))
            {
                p.getPackets().sendMessage("You need a needle if you wish to craft leather.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItemAmount(THREAD, (int)LEATHER_ITEMS[index][3]))
            {
                p.getPackets().sendMessage("You need " + (int)LEATHER_ITEMS[index][3] + " thread to craft that.");
                Crafting.resetCrafting(p);
                return;
            }
            string s = index < 4 ? "a" : "a pair of";
            for (int j = 0; j < (int)LEATHER_ITEMS[index][3]; j++)
            {
                if (!p.getInventory().deleteItem(TANNED_HIDE[item.getCraftType()]))
                {
                    return;
                }
            }
            p.getInventory().deleteItem(THREAD, (int)LEATHER_ITEMS[index][3]);
            p.getInventory().addItem(item.getFinishedItem());
            p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
            p.setLastAnimation(new Animation(1249));
            p.getPackets().sendMessage("You craft " + s + " " + item.getMessage() + ".");
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event craftMoreDragonHide = new Event(1500);
                craftMoreDragonHide.setAction(() =>
                {
                    craftDragonHide(p, -1, -1, -1, false);
                    craftMoreDragonHide.stop();
                });
                Server.registerEvent(craftMoreDragonHide);
            }
        }

        public static void craftNormalLeather(Player p, int index, int amount, bool newCraft)
        {
            index -= 28;
            if (newCraft)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(4, index, amount, (double)NORMAL_LEATHER[index][2], (int)NORMAL_LEATHER[index][0], (string)NORMAL_LEATHER[index][3], (int)NORMAL_LEATHER[index][1]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 4 || item.getCraftItem() < 0)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            if (!p.getInventory().hasItem(TANNED_HIDE[4]))
            {
                p.getPackets().sendMessage("You have no Leather.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(NEEDLE))
            {
                p.getPackets().sendMessage("You need a needle if you wish to craft leather.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(THREAD))
            {
                p.getPackets().sendMessage("You have no thread.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to craft that item.");
                Crafting.resetCrafting(p);
                return;
            }
            int i = item.getCraftItem();
            string s = i == 0 || i == 5 || i == 6 ? "a" : "a pair of";
            if (p.getInventory().deleteItem(THREAD) && p.getInventory().deleteItem(TANNED_HIDE[4]))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                p.setLastAnimation(new Animation(1249));
                p.getPackets().sendMessage("You make " + s + " " + item.getMessage() + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event craftMoreNormalLeather = new Event(1500);
                craftMoreNormalLeather.setAction(() =>
                {
                    craftNormalLeather(p, -1, -1, false);
                    craftMoreNormalLeather.stop();
                });
                Server.registerEvent(craftMoreNormalLeather);
            }
        }
    }
}