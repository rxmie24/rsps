using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.smithing
{
    internal class Smithing : SmithingData
    {
        public Smithing()
        {
        }

        public static bool wantToSmithOnAnvil(Player p, int item, Location anvilLoc)
        {
            for (int i = 0; i < BARS.Length; i++)
            {
                if (item == BARS[i])
                {
                    AreaEvent displaySmithOptionsAreaEvent = new AreaEvent(p, anvilLoc.getX() - 1, anvilLoc.getY() - 1, anvilLoc.getX() + 1, anvilLoc.getY() + 1);
                    displaySmithOptionsAreaEvent.setAction(() =>
                    {
                        displaySmithOptions(p, i);
                    });
                    Server.registerCoordinateEvent(displaySmithOptionsAreaEvent);
                    return true;
                }
            }
            return false;
        }

        private static void displaySmithOptions(Player p, int index)
        {
            object[][] variables = getInterfaceVariables(p, index);
            if (variables == null)
            {
                return;
            }
            p.getPackets().showChildInterface(300, 65, variables[6][5] != null);
            p.getPackets().showChildInterface(300, 81, variables[8][5] != null);
            p.getPackets().showChildInterface(300, 89, variables[9][5] != null);
            p.getPackets().showChildInterface(300, 97, variables[10][5] != null);
            p.getPackets().showChildInterface(300, 161, variables[18][5] != null);
            p.getPackets().showChildInterface(300, 169, variables[19][5] != null);
            p.getPackets().showChildInterface(300, 209, variables[24][5] != null);
            p.getPackets().showChildInterface(300, 266, variables[29][5] != null);
            for (int j = 0; j < variables.Length; j++)
            {
                bool canSmith = p.getSkills().getGreaterLevel(Skills.SKILL.SMITHING) >= (int)variables[j][1];
                string barColour = p.getInventory().hasItemAmount(BARS[index], (int)variables[j][3]) ? "<col=00FF00>" : "";
                string amount = barColour + variables[j][3];
                string name = (string)variables[j][5];
                string s = (int)variables[j][3] > 1 ? "s" : "";
                int child = (int)variables[j][6];

                p.getPackets().itemOnInterface(300, child, (int)variables[j][2], (int)variables[j][0]);
                p.getPackets().modifyText(canSmith ? "<col=FFFFFF>" + name : name, 300, (child + 1));
                if (!barColour.Equals(""))
                {
                    p.getPackets().modifyText(barColour + amount + " Bar" + s, 300, (child + 2));
                }
            }
            p.getPackets().displayInterface(300);
            p.setTemporaryAttribute("smithingBarType", index);
        }

        public static void smithItem(Player p, int button, int offset, bool newSmith)
        {
            int index = -1;
            if (!newSmith && p.getTemporaryAttribute("smithingItem") == null)
            {
                return;
            }
            if (newSmith)
            {
                if (p.getTemporaryAttribute("smithingBarType") == null)
                {
                    return;
                }
                index = (int)p.getTemporaryAttribute("smithingBarType");
                if (index == -1)
                {
                    return;
                }
                object[][] variables = getInterfaceVariables(p, index);
                int amountToMake = -1;
                int item = -1;
                for (int i = 6; i > 0; i--)
                {
                    for (int j = 0; j < variables.Length; j++)
                    {
                        if ((int)variables[j][6] + i == button)
                        {
                            offset = i;
                            item = j;
                            break;
                        }
                    }
                }
                if (offset == 4)
                {
                    p.getPackets().displayEnterAmount();
                    p.setTemporaryAttribute("interfaceVariable", new EnterVariable(300, button));
                }
                else
                    if (offset == 3)
                    {
                        amountToMake = 28;
                    }
                    else
                        if (offset == 5)
                        {
                            amountToMake = 5;
                        }
                        else
                            if (offset == 6)
                            {
                                amountToMake = 1;
                            }
                if (offset >= 400)
                {
                    amountToMake = offset - 400;
                }
                p.setTemporaryAttribute("smithingItem", new SmithBar(BARS[index], (int)variables[item][3], (int)variables[item][1], (int)variables[item][4], (int)variables[item][0], (int)variables[item][2], amountToMake));
            }
            SmithBar smithbar = (SmithBar)p.getTemporaryAttribute("smithingItem");
            if (!canSmith(p, smithbar))
            {
                return;
            }
            p.getPackets().closeInterfaces();
            p.setLastAnimation(new Animation(898));
            for (int i = 0; i < smithbar.getBarAmount(); i++)
            {
                if (!p.getInventory().deleteItem(smithbar.getBarType()))
                {
                    return;
                }
            }
            if (p.getInventory().addItem(smithbar.getFinishedItem(), smithbar.getFinishedItemAmount()))
            {
                p.getSkills().addXp(Skills.SKILL.SMITHING, smithbar.getXp());
            }
            smithbar.decreaseAmount();
            if (smithbar.getAmount() >= 1)
            {
                Event smithMoreItemEvent = new Event(1500);
                smithMoreItemEvent.setAction(() =>
                {
                    smithItem(p, -1, 1, false);
                    smithMoreItemEvent.stop();
                });
                Server.registerEvent(smithMoreItemEvent);
            }
        }

        private static bool canSmith(Player p, SmithBar item)
        {
            if (p == null || item == null)
            {
                return false;
            }
            if (item.getAmount() <= 0)
            {
                return false;
            }
            if (!p.getInventory().hasItem(HAMMER))
            {
                p.getPackets().sendMessage("You need a hammer if you wish to smith.");
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.SMITHING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Smithing level of " + item.getLevel() + " to make that item.");
                return false;
            }
            if (!p.getInventory().hasItemAmount(item.getBarType(), item.getBarAmount()))
            {
                p.getPackets().sendMessage("You don't have enough bars to make that item.");
                return false;
            }
            return true;
        }

        private static object[][] getInterfaceVariables(Player p, int i)
        {
            switch (i)
            {
                case 0:
                    return BRONZE;
                // 1 is blurite
                case 2:
                    return IRON;
                // 3 is silver
                case 4:
                    return STEEL;
                // 5 is gold
                case 6:
                    return MITHRIL;

                case 7:
                    return ADAMANT;

                case 8:
                    return RUNE;
            }
            return null;
        }

        public static void resetSmithing(Player p)
        {
            p.removeTemporaryAttribute("smithingItem");
            p.removeTemporaryAttribute("smithingBarType");
        }
    }
}