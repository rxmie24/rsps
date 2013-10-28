using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.crafting
{
    internal class Spinning : CraftingData
    {
        public Spinning()
        {
        }

        public static void displaySpinningInterface(Player p)
        {
            int k = 2;
            int l = 8;
            string s = "<br><br><br><br>";
            for (int j = 0; j < 3; j++)
            {
                p.getPackets().itemOnInterface(304, k, 180, (int)SPINNING_ITEMS[j][0]);
                p.getPackets().modifyText(s + (string)SPIN_FINISH[j], 304, l);
                l += 4;
                k++;
            }
            p.setTemporaryAttribute("craftType", 6);
            p.getPackets().sendChatboxInterface(304);
        }

        public static void craftSpinning(Player p, int amount, int index, bool newCraft)
        {
            if (newCraft)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(6, index, amount, (double)SPINNING_ITEMS[index][3], (int)SPINNING_ITEMS[index][0], (string)SPINNING_ITEMS[index][4], (int)SPINNING_ITEMS[index][2]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 6)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            int i = item.getCraftItem();
            if (!p.getInventory().hasItem((int)SPINNING_ITEMS[i][1]))
            {
                p.getPackets().sendMessage("You have no " + item.getMessage() + ".");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to spin that.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getInventory().deleteItem((int)SPINNING_ITEMS[i][1]))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                p.setLastAnimation(new Animation(894));
                p.getPackets().sendMessage("You spin the " + item.getMessage() + " into a " + SPIN_FINISH[i] + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event craftMoreSpinningEvent = new Event(750);
                craftMoreSpinningEvent.setAction(() =>
                {
                    craftSpinning(p, -1, -1, false);
                    craftMoreSpinningEvent.stop();
                });
                Server.registerEvent(craftMoreSpinningEvent);
            }
        }
    }
}