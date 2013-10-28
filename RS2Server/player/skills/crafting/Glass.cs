using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.crafting
{
    internal class Glass : CraftingData
    {
        public Glass()
        {
        }

        public static void displayGlassOption(Player p)
        {
            p.getPackets().displayInterface(542);
        }

        public static void craftGlass(Player p, int amount, int index, bool newCraft)
        {
            if (newCraft)
            {
                p.setTemporaryAttribute("craftItem", new CraftItem(3, index, amount, (double)GLASS_ITEMS[index][2], (int)GLASS_ITEMS[index][0], (string)GLASS_ITEMS[index][3], (int)GLASS_ITEMS[index][1]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("craftItem");
            if (item == null || p == null || item.getAmount() <= 0 || item.getCraftType() != 3)
            {
                Crafting.resetCrafting(p);
                return;
            }
            p.getPackets().closeInterfaces();
            if (!p.getInventory().hasItem(MOLTEN_GLASS))
            {
                p.getPackets().sendMessage("You have no molten glass.");
                Crafting.resetCrafting(p);
                return;
            }
            if (!p.getInventory().hasItem(GLASSBLOWING_PIPE))
            {
                p.getPackets().sendMessage("You need a glassblowing pipe if you wish to make a glass item.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.CRAFTING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Crafting level of " + item.getLevel() + " to craft that item.");
                Crafting.resetCrafting(p);
                return;
            }
            if (p.getInventory().deleteItem(MOLTEN_GLASS))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.CRAFTING, item.getXp());
                p.setLastAnimation(new Animation(884));
                p.getPackets().sendMessage("You blow through the pipe, shaping the molten glass into a " + item.getMessage() + ".");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event craftMoreGlassEvent = new Event(1500);
                craftMoreGlassEvent.setAction(() =>
                {
                    craftGlass(p, -1, -1, false);
                    craftMoreGlassEvent.stop();
                });
                Server.registerEvent(craftMoreGlassEvent);
            }
        }
    }
}