using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.fletching
{
    internal class MakeBows : FletchingData
    {
        public MakeBows()
        {
        }

        public static void displayBowOptions(Player p, int index, bool stringing, int type)
        {
            string s = "<br><br><br><br>";
            if (!stringing)
            {
                if (index == 0)
                {
                    p.getPackets().sendChatboxInterface(305);
                    p.getPackets().itemOnInterface(305, 2, 175, 52);
                    p.getPackets().itemOnInterface(305, 3, 175, 50);
                    p.getPackets().itemOnInterface(305, 4, 175, 48);
                    p.getPackets().itemOnInterface(305, 5, 175, 9440);
                    p.getPackets().modifyText(s + ARROW_AMOUNT + " Arrow Shafts", 305, 9);
                    p.getPackets().modifyText(s + "Short Bow", 305, 13);
                    p.getPackets().modifyText(s + "Long Bow", 305, 17);
                    p.getPackets().modifyText(s + "Crossbow Stock", 305, 21);
                    return;
                }
                p.getPackets().sendChatboxInterface(303);
                p.getPackets().itemOnInterface(303, 2, 175, UNSTRUNG_SHORTBOW[index]);
                p.getPackets().itemOnInterface(303, 3, 175, UNSTRUNG_LONGBOW[index]);
                p.getPackets().modifyText(s + "Short Bow", 303, 7);
                p.getPackets().modifyText(s + "Long Bow", 303, 11);
            }
            else
            {
                int[] bows = type == 0 ? STRUNG_SHORTBOW : STRUNG_LONGBOW;
                p.getPackets().sendChatboxInterface(309);
                p.getPackets().itemOnInterface(309, 2, 150, bows[index]);
                p.getPackets().modifyText(s + ItemData.forId(bows[index]).getName(), 309, 6);
            }
        }

        public static void cutLog(Player p, int amount, int logType, int itemType, bool isStringing, bool newFletch)
        {
            Bow item = null;
            if (newFletch)
            {
                item = getBow(itemType, logType, amount, isStringing);
                Fletching.setFletchItem(p, item);
            }
            item = (Bow)Fletching.getFletchItem(p);
            if (item == null || p == null)
            {
                return;
            }
            bool stringing = item.isStringing();
            if (!canFletch(p, item, stringing))
            {
                p.getPackets().closeInterfaces();
                return;
            }
            int animation = getAnimation(item);
            if (!stringing)
            {
                int amt = item.getItemType() == 2 ? ARROW_AMOUNT : 1;
                if (p.getInventory().deleteItem(LOGS[item.getLogType()]))
                {
                    p.getInventory().addItem(item.getFinishedItem(), amt);
                    p.getSkills().addXp(Skills.SKILL.FLETCHING, item.getXp());
                    item.decreaseAmount();
                    p.getPackets().sendMessage("You carefully cut the wood into " + MESSAGE[item.getItemType()] + ".");
                    p.setLastAnimation(new Animation(animation));
                }
            }
            else
            {
                int[] bows = item.getItemType() == 0 ? UNSTRUNG_SHORTBOW : UNSTRUNG_LONGBOW;
                if (p.getInventory().deleteItem(BOWSTRING) && p.getInventory().deleteItem(bows[item.getLogType()]))
                {
                    p.getInventory().addItem(item.getFinishedItem());
                    p.getSkills().addXp(Skills.SKILL.FLETCHING, item.getXp());
                    item.decreaseAmount();
                    p.getPackets().sendMessage("You add a string to the bow.");
                    p.setLastAnimation(new Animation(animation));
                }
            }
            p.getPackets().closeInterfaces();
            if (item.getAmount() >= 1)
            {
                Event cutMoreLogsEvent = new Event(1500);
                cutMoreLogsEvent.setAction(() =>
                {
                    cutLog(p, -1, -1, -1, false, false);
                    cutMoreLogsEvent.stop();
                });
                Server.registerEvent(cutMoreLogsEvent);
            }
        }

        private static int getAnimation(Bow item)
        {
            if (item.isStringing())
            {
                return item.getItemType() == 0 ? 6678 + item.getLogType() : 6684 + item.getLogType();
            }
            return item.getLogType() == 5 ? 7211 : 1248;
        }

        private static bool canFletch(Player p, Bow item, bool stringing)
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
            if (!stringing)
            {
                if (!p.getInventory().hasItem(LOGS[item.getLogType()]))
                {
                    p.getPackets().sendMessage("You have run out of logs.");
                    return false;
                }
                if (!p.getInventory().hasItem(KNIFE))
                {
                    p.getPackets().sendMessage("You need a knife to fletch logs.");
                    return false;
                }
            }
            else
            {
                int[] bows = item.getItemType() == 0 ? UNSTRUNG_SHORTBOW : UNSTRUNG_LONGBOW;
                if (!p.getInventory().hasItem(bows[item.getLogType()]))
                {
                    p.getPackets().sendMessage("You do not have a bow to string.");
                    return false;
                }
                if (!p.getInventory().hasItem(BOWSTRING))
                {
                    p.getPackets().sendMessage("You need Bowstring if you wish to string a bow!");
                    return false;
                }
            }
            return true;
        }

        private static Bow getBow(int itemType, int logType, int amount, bool stringing)
        {
            int i = logType;
            if (stringing)
            {
                if (itemType == 0)
                {
                    return new Bow(STRUNG_SHORTBOW[i], logType, itemType, SHORTBOW_XP[i], SHORTBOW_LVL[i], amount, true);
                }
                else if (itemType == 1)
                {
                    return new Bow(STRUNG_LONGBOW[i], logType, itemType, LONGBOW_XP[i], LONGBOW_LVL[i], amount, true);
                }
            }
            switch (itemType)
            {
                case 0: // Unstrung Shortbow
                    return new Bow(UNSTRUNG_SHORTBOW[i], logType, itemType, SHORTBOW_XP[i], SHORTBOW_LVL[i], amount, false);

                case 1: // Unstrung Longbow
                    return new Bow(UNSTRUNG_LONGBOW[i], logType, itemType, LONGBOW_XP[i], LONGBOW_LVL[i], amount, false);

                case 2: // Arrow Shafts
                    return new Bow(ARROW_SHAFTS, 0, 2, ARROW_SHAFT_XP * ARROW_AMOUNT, ARROW_SHAFT_LVL, amount, false);

                case 3: // Crossbow Stock
                    return new Bow(9440, i, 3, XBOW_XP[0], XBOW_LVL[0], amount, false);
            }
            return null;
        }
    }
}