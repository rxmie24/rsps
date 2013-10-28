using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player.skills.crafting;

namespace RS2.Server.player.skills.fletching
{
    internal class MakeAmmo : FletchingData
    {
        public MakeAmmo()
        {
        }

        public static void displayAmmoInterface(Player p, int type, bool featherless, bool bolt)
        {
            string s = "<br><br><br><br>";
            string s1 = bolt && type == 0 ? "Bronze " : "";
            string text = "";
            int item = -1;
            if (!bolt)
            {
                if (featherless)
                {
                    item = HEADLESS_ARROW;
                    text = ARROW_AMOUNT + " Headless arrows";
                }
                else
                {
                    item = ARROW[type];
                    text = ItemData.forId(item).getName() + "s";
                }
            }
            else
            {
                if (featherless)
                {
                    item = FEATHERED_BOLT[type];
                    text = ItemData.forId(item).getName();
                }
                else
                {
                    item = BOLT[type];
                    text = ItemData.forId(item).getName();
                }
            }
            p.getPackets().sendChatboxInterface(582);
            p.getPackets().itemOnInterface(582, 2, 150, item);
            p.getPackets().modifyText(s + s1 + text, 582, 5);
        }

        public static void createAmmo(Player p, int sets, int type, bool bolt, bool newFletch)
        {
            Ammo item = null;
            if (newFletch || Fletching.getFletchItem(p) == null)
            {
                item = getAmmo(type, bolt, sets);
                Fletching.setFletchItem(p, item);
            }
            item = (Ammo)Fletching.getFletchItem(p);
            if (item == null || p == null)
            {
                return;
            }
            if (!canFletch(p, item))
            {
                p.getPackets().closeInterfaces();
                return;
            }
            int amt = getArrowAmount(p, item);
            if (amt <= 0)
            {
                return;
            }
            if (p.getInventory().deleteItem(item.getItemOne(), amt) && p.getInventory().deleteItem(item.getItemTwo(), amt))
            {
                p.getInventory().addItem(item.getFinishedItem(), amt);
                p.getSkills().addXp(Skills.SKILL.FLETCHING, item.getXp() * amt);
                p.getPackets().sendMessage(getMessage(item, amt));
                item.decreaseAmount();
                p.getPackets().closeInterfaces();
            }
            if (item.getAmount() >= 1)
            {
                Event createMoreAmmoEvent = new Event(1500);
                createMoreAmmoEvent.setAction(() =>
                {
                    createAmmo(p, -1, -1, false, false);
                    createMoreAmmoEvent.stop();
                });
                Server.registerEvent(createMoreAmmoEvent);
            }
        }

        private static string getMessage(Ammo item, int amount)
        {
            string s = amount > 1 ? "s" : "";
            string s1 = amount > 1 ? "some" : "a";
            string s2 = amount > 1 ? "some" : "an";
            if (!item.isBolt())
            {
                s = amount > 1 ? "s" : "";
                s1 = amount > 1 ? "some" : "a";
                s2 = amount > 1 ? "some" : "an";
                if (item.getItemType() == 0)
                {
                    return "You attach Feathers to " + s1 + " of your Arrow shafts, you make " + amount + " " + ItemData.forId(item.getFinishedItem()).getName() + s + ".";
                }
                else
                {
                    return "You attach " + s2 + " Arrowtip" + s + " to " + s1 + " Headless arrow" + s + ", you make " + amount + " " + ItemData.forId(item.getFinishedItem()).getName() + s + ".";
                }
            }
            else
            {
                s = amount > 1 ? "s" : "";
                s1 = amount > 1 ? "some of your bolts" : "a bolt";
                s2 = amount > 1 ? "some" : "a";
                if (item.getItemType() < 8)
                {
                    return "You attach Feathers to " + s1 + ", you make " + amount + " " + ItemData.forId(item.getFinishedItem()).getName() + ".";
                }
                else
                {
                    s = amount > 1 ? "s" : "";
                    s2 = amount > 1 ? "some" : "a";
                    return "You attach " + s2 + " bolt tip" + s + " to " + s2 + " headless bolt" + s + ", you make " + amount + " " + ItemData.forId(item.getFinishedItem()).getName() + ".";
                }
            }
        }

        private static Ammo getAmmo(int type, bool bolt, int amount)
        {
            int i = type;
            if (!bolt)
            {
                if (i == 7)
                {
                    return new Ammo(HEADLESS_ARROW, FEATHER, ARROW_SHAFTS, HEADLESS_ARROW_XP, HEADLESS_ARROW_LVL, 0, false, amount);
                }
                if (i >= 0 && i < 7)
                {
                    return new Ammo(ARROW[i], HEADLESS_ARROW, ARROWHEAD[i], ARROW_XP[i], ARROW_LVL[i], i, false, amount);
                }
            }
            else
            {
                if (i < 8)
                {
                    return new Ammo(FEATHERED_BOLT[i], FEATHER, FEATHERLESS_BOLT[i], FEATHERLESS_BOLT_XP[i], FEATHERLESS_BOLT_LVL[i], i, true, amount);
                }
                if (i >= 8)
                {
                    i -= 8;
                    return new Ammo(BOLT[i], HEADLESS_BOLT[i], BOLT_TIPS[i], HEADLESS_BOLT_XP[i], HEADLESS_BOLT_LVL[i], i, true, amount);
                }
            }
            return null;
        }

        private static int getArrowAmount(Player p, Ammo item)
        {
            if (item == null || p == null)
            {
                return 0;
            }
            int amt = item.isBolt() ? 12 : 15;
            int itemOneAmount = p.getInventory().getItemAmount(item.getItemOne());
            int itemTwoAmount = p.getInventory().getItemAmount(item.getItemTwo());
            int lowestStack = itemOneAmount > itemTwoAmount ? itemTwoAmount : itemOneAmount;
            int finalAmount = lowestStack > amt ? amt : lowestStack;
            return finalAmount;
        }

        private static bool canFletch(Player p, Ammo item)
        {
            if (item == null || item.getAmount() <= 0)
            {
                return false;
            }
            string s = item.getItemOne() == HEADLESS_ARROW ? "s." : ".";
            string s1 = item.getItemTwo() == HEADLESS_ARROW ? "s." : ".";
            if (p.getSkills().getGreaterLevel(Skills.SKILL.FLETCHING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Fletching level of " + item.getLevel() + " to make that.");
                return false;
            }
            if (!p.getInventory().hasItem(item.getItemOne()))
            {
                p.getPackets().sendMessage("You don't have any " + ItemData.forId(item.getItemOne()).getName() + s);
                return false;
            }
            if (!p.getInventory().hasItem(item.getItemTwo()))
            {
                p.getPackets().sendMessage("You don't have any " + ItemData.forId(item.getItemTwo()).getName() + s1);
                return false;
            }
            return true;
        }

        public static void displayGemOptions(Player p, int index)
        {
            int amount = p.getInventory().getItemAmount((int)GEMS[index][0]);
            if (amount > 1)
            {
                string s = "<br><br><br><br>";
                p.getPackets().itemOnInterface(309, 2, 190, (int)GEMS[index][1]);
                p.getPackets().modifyText(s + (string)GEMS[index][4] + " bolt tips (x12)", 309, 6);
                p.getPackets().sendChatboxInterface(309);
                return;
            }
            makeBoltTip(p, index, 1, true);
        }

        public static void makeBoltTip(Player p, int index, int amount, bool newCut)
        {
            if (newCut)
            {
                p.setTemporaryAttribute("fletchItem", new CraftItem(5, index, amount, (double)GEMS[index][3], (int)GEMS[index][1], (string)GEMS[index][4], (int)GEMS[index][2]));
            }
            CraftItem item = (CraftItem)p.getTemporaryAttribute("fletchItem");
            if (item == null || p == null || item.getAmount() <= 0)
            {
                Fletching.setFletchItem(p, null);
                return;
            }
            p.getPackets().closeInterfaces();
            if (!p.getInventory().hasItem(1755))
            {
                p.getPackets().sendMessage("You need a chisel to cut gems into bolt tips.");
                Fletching.setFletchItem(p, null);
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
                    p.getPackets().sendMessage("You have no more " + item.getMessage() + "s to cut.");
                }
                Fletching.setFletchItem(p, null);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.FLETCHING) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Fletching level of " + item.getLevel() + " to cut that gem.");
                Fletching.setFletchItem(p, null);
                return;
            }
            int amountToMake = item.getCraftItem() == 9 ? 24 : 12;
            if (p.getInventory().deleteItem((int)GEMS[item.getCraftItem()][0]))
            {
                p.getInventory().addItem(item.getFinishedItem(), amountToMake);
                p.getSkills().addXp(Skills.SKILL.FLETCHING, item.getXp());
                p.setLastAnimation(new Animation((int)GEMS[item.getCraftItem()][5]));
                p.getPackets().sendMessage("You cut the " + item.getMessage() + " into " + amountToMake + " " + item.getMessage() + " bolt tips.");
            }
            item.decreaseAmount();
            if (item.getAmount() >= 1)
            {
                Event makeMoreBoltTipEvent = new Event(1500);
                makeMoreBoltTipEvent.setAction(() =>
                {
                    makeBoltTip(p, -1, -1, false);
                    makeMoreBoltTipEvent.stop();
                });
                Server.registerEvent(makeMoreBoltTipEvent);
            }
        }
    }
}