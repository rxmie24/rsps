using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.fletching
{
    internal class MakeXbow : FletchingData
    {
        public static void displayOptionInterface(Player p, int type, bool stringing)
        {
            string s = "<br><br><br><br>";
            p.getPackets().sendChatboxInterface(309);
            if (!stringing)
            {
                p.getPackets().itemOnInterface(309, 2, 150, UNFINISHED_XBOW[type]);
                p.getPackets().modifyText(s + ItemData.forId(UNFINISHED_XBOW[type]).getName(), 309, 6);
                return;
            }
            p.getPackets().itemOnInterface(309, 2, 150, FINISHED_XBOW[type]);
            p.getPackets().modifyText(s + ItemData.forId(FINISHED_XBOW[type]).getName(), 309, 6);
        }

        public static void createXbow(Player p, int amount, int xbowType, bool isStringing, bool newFletch)
        {
            SkillItem item = null;
            if (newFletch || Fletching.getFletchItem(p) == null)
            {
                item = getXbow(xbowType, isStringing, amount);
                Fletching.setFletchItem(p, item);
            }
            item = (SkillItem)Fletching.getFletchItem(p);
            if (item == null || p == null)
            {
                return;
            }
            bool stringing = item.getItemTwo() == XBOW_STRING ? true : false;
            if (!canFletch(p, item))
            {
                p.getPackets().closeInterfaces();
                return;
            }
            if (p.getInventory().deleteItem(item.getItemOne()) && p.getInventory().deleteItem(item.getItemTwo()))
            {
                p.getInventory().addItem(item.getFinishedItem());
                p.getSkills().addXp(Skills.SKILL.FLETCHING, item.getXp());
                item.decreaseAmount();
                p.getPackets().closeInterfaces();
                if (!stringing)
                {
                    p.getPackets().sendMessage("You attach some limbs to the Crossbow.");
                }
                else
                {
                    p.setLastAnimation(new Animation(6677));
                    p.getPackets().sendMessage("You add a Crossbow String to the Crossbow, you have completed the " + ItemData.forId(item.getFinishedItem()).getName() + ".");
                }
            }
            if (item.getAmount() >= 1)
            {
                Event createMoreXBowEvent = new Event(1500);
                createMoreXBowEvent.setAction(() =>
                {
                    createXbow(p, -1, -1, false, false);
                    createMoreXBowEvent.stop();
                });
                Server.registerEvent(createMoreXBowEvent);
            }
        }

        private static bool canFletch(Player p, SkillItem item)
        {
            if (item == null || item.getAmount() <= 0)
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.FLETCHING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Fletching level of " + item.getLevel() + " to make that.");
                return false;
            }
            if (!p.getInventory().hasItem(item.getItemOne()))
            {
                p.getPackets().sendMessage("You don't have a " + ItemData.forId(item.getItemOne()).getName());
                return false;
            }
            if (!p.getInventory().hasItem(item.getItemTwo()))
            {
                p.getPackets().sendMessage("You don't have any " + ItemData.forId(item.getItemTwo()).getName());
                return false;
            }
            return true;
        }

        private static SkillItem getXbow(int xbowType, bool stringing, int amount)
        {
            int i = xbowType;
            if (!stringing)
            {
                return new SkillItem(UNFINISHED_XBOW[i], CROSSBOW_STOCK[0], XBOW_LIMB[i], XBOW_LVL[i], FLETCHING, XBOW_XP[i], amount);
            }
            else
            {
                return new SkillItem(FINISHED_XBOW[i], UNFINISHED_XBOW[i], XBOW_STRING, XBOW_LVL[i], FLETCHING, XBOW_XP[i], amount);
            }
        }
    }
}