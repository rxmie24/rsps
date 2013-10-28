using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.cooking
{
    internal class Cooking : CookingData
    {
        public Cooking()
        {
        }

        public static bool isCooking(Player p, int item, bool fire, int fireX, int fireY)
        {
            for (int i = 0; i < MEAT_RAW.Length; i++)
            {
                if (item == MEAT_RAW[i])
                {
                    if (fire)
                    {
                        int j = i;
                        AreaEvent setCookingAreaEvent = new AreaEvent(p, fireX - 1, fireY - 1, fireX + 1, fireY + 1);
                        setCookingAreaEvent.setAction(() =>
                        {
                            p.setFaceLocation(new Location(fireX, fireY, p.getLocation().getZ()));
                            if (Server.getGlobalObjects().fireExists(new Location(fireX, fireY, 0)))
                            {
                                setCookingItem(p, null);
                                p.setTemporaryAttribute("cookingFireLocation", new Location(fireX, fireY, p.getLocation().getZ()));
                                displayCookOption(p, j);
                            }
                        });
                        Server.registerCoordinateEvent(setCookingAreaEvent);
                        return true;
                    }
                    setCookingItem(p, null);
                    displayCookOption(p, i);
                    return true;
                }
            }
            for (int i = 0; i < MEAT_COOKED.Length; i++)
            {
                if (item == MEAT_COOKED[i])
                {
                    if (fire)
                    {
                        AreaEvent cookMeatAreaEvent = new AreaEvent(p, fireX - 1, fireY - 1, fireX + 1, fireY + 1);
                        cookMeatAreaEvent.setAction(() =>
                        {
                            p.setFaceLocation(new Location(fireX, fireY, p.getLocation().getZ()));
                            if (Server.getGlobalObjects().fireExists(new Location(fireX, fireY, 0)))
                            {
                                p.getInventory().replaceSingleItem(MEAT_COOKED[i], MEAT_BURNT[i]);
                                p.getPackets().sendMessage("You deliberately burn the " + ItemData.forId(MEAT_COOKED[i]).getName() + ".");
                                p.setLastAnimation(new Animation(883));
                            }
                        });
                        Server.registerCoordinateEvent(cookMeatAreaEvent);
                        return true;
                    }
                    setCookingItem(p, null);
                    p.getInventory().replaceSingleItem(MEAT_COOKED[i], MEAT_BURNT[i]);
                    p.getPackets().sendMessage("You deliberately burn the " + ItemData.forId(MEAT_COOKED[i]).getName() + ".");
                    p.setLastAnimation(new Animation(883));
                    return true;
                }
            }
            return false;
        }

        public static void cookItem(Player p, int amount, bool newCook, bool fire)
        {
            SkillItem item = null;
            if (newCook)
            {
                if (p.getTemporaryAttribute("meatItem") == null)
                {
                    return;
                }
                int i = (int)p.getTemporaryAttribute("meatItem");
                p.setTemporaryAttribute("cookCycles", 0);
                item = new SkillItem(MEAT_COOKED[i], MEAT_RAW[i], MEAT_BURNT[i], MEAT_LEVEL[i], (int)Skills.SKILL.COOKING, MEAT_XP[i], amount);
                setCookingItem(p, item);
            }
            item = (SkillItem)getCookingItem(p);
            if (item == null || p == null || !p.getInventory().hasItem(item.getItemOne()))
            {
                return;
            }
            if (fire)
            {
                if (p.getTemporaryAttribute("cookingFireLocation") == null)
                {
                    setCookingItem(p, null);
                    p.getPackets().closeInterfaces();
                    return;
                }
                else
                {
                    Location fireLocation = (Location)p.getTemporaryAttribute("cookingFireLocation");
                    if (!Server.getGlobalObjects().fireExists(new Location(fireLocation.getX(), fireLocation.getY(), fireLocation.getZ())))
                    {
                        p.getPackets().sendMessage("The fire has burnt out..");
                        setCookingItem(p, null);
                        p.getPackets().closeInterfaces();
                        return;
                    }
                }
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.COOKING) < item.getLevel())
            {
                p.getPackets().closeInterfaces();
                p.getPackets().sendMessage("You need a Cooking level of " + item.getLevel() + " to cook that.");
                return;
            }
            bool burn = getFormula(p, item);
            int newFood = burn ? item.getItemTwo() : item.getFinishedItem();
            string message = burn ? "accidentally burn" : "successfully cook";
            double xp = burn ? 0 : item.getXp();
            int cookCycles = p.getTemporaryAttribute("cookCycles") == null ? 1 : (int)p.getTemporaryAttribute("cookCycles");
            if (p.getInventory().replaceSingleItem(item.getItemOne(), newFood))
            {
                if (cookCycles >= 1)
                {
                    p.setLastAnimation(new Animation(fire ? 897 : 896));
                    p.setTemporaryAttribute("cookCycles", 0);
                }
                else
                {
                    p.setTemporaryAttribute("cookCycles", cookCycles + 1);
                }
                p.getSkills().addXp(Skills.SKILL.COOKING, xp);
                p.getPackets().sendMessage("You " + message + " the " + ItemData.forId(item.getFinishedItem()).getName() + ".");
                item.decreaseAmount();
                p.getPackets().closeInterfaces();
            }
            if (item.getAmount() >= 1)
            {
                Event cookMoreEvent = new Event(1500);
                cookMoreEvent.setAction(() =>
                {
                    cookItem(p, -1, false, fire);
                    cookMoreEvent.stop();
                });
                Server.registerEvent(cookMoreEvent);
            }
        }

        private static bool getFormula(Player p, SkillItem item)
        {
            int foodLevel = item.getLevel();
            int cookLevel = p.getSkills().getCurLevel(Skills.SKILL.COOKING);
            if (!wearingCookingGauntlets(p))
            {
                return Misc.random(cookLevel) <= Misc.random((int)(foodLevel / 1.5));
            }
            return Misc.random(cookLevel) <= Misc.random(foodLevel / 3);
        }

        private static bool wearingCookingGauntlets(Player p)
        {
            return p.getEquipment().getItemInSlot(ItemData.EQUIP.HANDS) == COOKING_GAUNTLETS;
        }

        private static void displayCookOption(Player p, int index)
        {
            string s = "<br><br><br><br>";
            p.getPackets().sendChatboxInterface(307);
            p.getPackets().itemOnInterface(307, 2, 150, MEAT_COOKED[index]);
            p.getPackets().modifyText(s + ItemData.forId(MEAT_COOKED[index]).getName(), 307, 6);
            p.setTemporaryAttribute("meatItem", index);
        }

        private static void resetAllCookingVariables(Player p)
        {
            p.removeTemporaryAttribute("meatItem");
            p.removeTemporaryAttribute("cookingFireLocation");
        }

        public static void setCookingItem(Player p, object a)
        {
            if (a == null)
            {
                resetAllCookingVariables(p);
                p.removeTemporaryAttribute("cookingItem");
                return;
            }
            p.setTemporaryAttribute("cookingItem", (object)a);
        }

        public static object getCookingItem(Player p)
        {
            return (object)p.getTemporaryAttribute("cookingItem");
        }
    }
}