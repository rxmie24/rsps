using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.crafting
{
    internal class Silver : CraftingData
    {
        public Silver()
        {
        }

        public static void displaySilverOptions(Player p)
        {
            string s = "<br><br><br><br>";
            p.getPackets().sendChatboxInterface(303);
            p.getPackets().itemOnInterface(303, 2, 175, (int)SILVER_ITEMS[0][0]);
            p.getPackets().itemOnInterface(303, 3, 175, (int)SILVER_ITEMS[1][0]);
            p.getPackets().modifyText(s + (string)SILVER_ITEMS[0][4], 303, 7);
            p.getPackets().modifyText(s + (string)SILVER_ITEMS[1][4], 303, 11);
            p.setTemporaryAttribute("craftType", 120);
        }

        public static void newSilverItem(Player p, int amount, int index, bool newCraft)
        {
            index -= 120;
            if (newCraft)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(3, index, amount, (double)SILVER_ITEMS[index][3], (int)SILVER_ITEMS[index][0], (string)SILVER_ITEMS[index][4], (int)SILVER_ITEMS[index][2]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 3)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            string s = item.getCraftItem() == 0 ? "an" : "a";
            if (!p.getInventory().hasItem((int)SILVER_ITEMS[item.getCraftItem()][1]))
            {
                p.getPackets().sendMessage("You need " + s + " " + item.getMessage() + " mould to make that.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(SILVER_BAR))
            {
                p.getPackets().sendMessage("You don't have a Silver bar.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to smelt that.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getInventory().deleteItem(SILVER_BAR))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                p.setLastAnimation(new Animation(3243));
                p.getPackets().sendMessage("You smelt the Silver bar in to " + s + " " + item.getMessage() + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event makeMoreSilverItemEvent = new Event(1500);
                makeMoreSilverItemEvent.setAction(() =>
                {
                    newSilverItem(p, -1, -1, false);
                    makeMoreSilverItemEvent.stop();
                });
                Server.registerEvent(makeMoreSilverItemEvent);
            }
        }
    }
}