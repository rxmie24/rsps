using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.crafting
{
    internal class Clay : CraftingData
    {
        public Clay()
        {
        }

        public static void displayClayOptions(Player p, int craftType)
        {
            string s = "<br><br><br><br>";
            int j = 2;
            int k = 10;
            for (int i = 0; i < CLAY_ITEMS.Length; i++)
            {
                p.getPackets().itemOnInterface(306, j, 130, (int)CLAY_ITEMS[i][0]);
                p.getPackets().modifyText(s + CLAY_ITEMS[i][5], 306, k);
                j++;
                k += 4;
            }
            p.getPackets().sendChatboxInterface(306);
        }

        public static void craftClay(Player p, int amount, int craftType, int craftItem, bool newCraft)
        {
            if (newCraft)
            {
                if ((craftType != 1 && craftType != 2) || craftItem < 0 || craftItem > 4)
                {
                    return;
                }
                int index = craftItem;
                int endItem = craftType == 1 ? 0 : 1;
                int xp = craftType == 1 ? 3 : 4;
                p.setTemporaryAttribute("craftItem", new CraftItem(craftType, craftItem, amount, (double)CLAY_ITEMS[index][xp], (int)CLAY_ITEMS[index][endItem], (string)CLAY_ITEMS[index][5], (int)CLAY_ITEMS[index][2]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0)
            {
                Crafting.resetCrafting(p);
                return;
            }
            int neededItem = item.getCraftType() == 1 ? CLAY : (int)CLAY_ITEMS[item.getCraftItem()][0];
            string s = item.getCraftType() == 1 ? "You mould the clay into a " + item.getMessage() : "You bake the " + item.getMessage() + " in the oven";
            string s1 = item.getCraftType() == 1 ? "You need some soft clay to mould a " + item.getMessage() : "You need a pre-made " + item.getMessage() + " to put in the oven";
            int animation = item.getCraftType() == 1 ? 883 : 899;
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to make a " + item.getMessage() + ".");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(neededItem))
            {
                p.getPackets().sendMessage(s1 + ".");
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            if (p.getInventory().deleteItem(neededItem))
            {
                if (p.getInventory().addItem(item.getFinishedItem()))
                {
                    p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                    p.getPackets().sendMessage(s + ".");
                    p.setLastAnimation(new Animation(animation));
                }
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event craftMoreClayEvent = new Event(1500);
                craftMoreClayEvent.setAction(() =>
                {
                    craftClay(p, -1, -1, -1, false);
                    craftMoreClayEvent.stop();
                });
                Server.registerEvent(craftMoreClayEvent);
            }
        }
    }
}