using RS2.Server.events;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;
using System;

namespace RS2.Server.definitions
{
    internal class Consumables
    {
        public Consumables()
        {
        }

        public static int[][] FOOD = {
            new int[] { //one click eat food
                319, 2142, 315, 2140, 2309, 325, 347, 2325, 333, 351,
                2327, 329, 2003, 361, 2323, 2289, 379, 1891, 373, 2293,
                1897, 2297, 3151, 3146, 3228, 7072, 355, 7062, 3363, 7078,
                3365, 339, 3367, 7064, 3379, 7088, 7170, 2878, 3144, 7530,
                7178, 5003, 7568, 7054, 7084, 365, 7082, 7188, 1985, 2343,
                2149, 7066, 1885, 2011, 7058, 7056, 2301, 7068, 7060, 7198,
                385, 397, 7208, 391, 7218, 1942, 1957, 1965, 1982, 5504, 1963,
                2108, 2114, 5972
            },
            new int[] { //like half a pie, half a cake.
                -1, -1, -1, -1, -1, -1, -1, 2333, -1, -1,
                2331, -1, 1923, -1, 2335, 2291, -1, 1893, -1, 2295,
                1899, 2299, -1, -1, -1, 1923, -1, 1923, -1, 1923,
                -1, -1, -1, 1923, -1, -1, 2313, -1, -1, -1,
                7180, -1, -1, -1, 1923, -1, 1923, 7190, -1, -1,
                -1, 1923, -1, 1923, -1, -1, 2303, 1923, -1, 7200,
                -1, -1, 7210, -1, 7220, -1, -1, -1, -1, -1, -1,
                -1, -1, -1
            },
            new int[] { //like 3rd of cake.
                -1, -1, -1, -1, -1, -1, -1, 2313, -1, -1,
                2313, -1, -1, -1, 2313, -1, -1, 1895, -1, -1,
                1901, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                2313, -1, -1, -1, -1, -1, -1, 2313, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, 2313,
                -1, -1, 2313, -1, 2313, -1, -1, -1, -1, -1, -1,
                -1, -1, -1
            }
        };

        public static int[] FOOD_HEAL = {
            1, 3, 3, 3, 5, 4, 5, 5, 7, 8,
            12, 9, 11, 10, 7, 7, 12, 4, 14, 8,
            5, 9, 3, 0, 5, 2, 6, 5, 5, 5,
            6, 7, 8, 8, 8, 5, 0, 10, 18, 11,
            6, 8, 15, 14, 5, 13, 5, 6, 2, 14,
            11, 11, 19, 19, 20, 16, 11, 13, 22, 8,
            20, 21, 10, 22, 10, 1, 1, 1, 2, 1, 3,
            2, 2, 2, 8
        };

        public static int[][] POTIONS = {
		    new int[] {125, 179, 119, 131, 3014, 137, 3038, 9745, 143, 12146, 149, 185, 155, 3022, 10004, 161, 3030, 167, 2458, 173, 3046, 193, 6691}, // 1 dose
		    new int[] {123, 177, 117, 129, 3012, 135, 3036, 9743, 141, 12144, 147, 183, 153, 3020, 10002, 159, 3028, 165, 2456, 171, 3044, 191, 6689}, // 2 dose
		    new int[] {121, 175, 115, 127, 3010, 133, 3034, 9741, 139, 12142, 145, 181, 151, 3018, 10000, 157, 3026, 163, 2454, 169, 3042, 189, 6687}, // 3 dose
		    new int[] {2428, 2446, 113, 2430, 3008, 2432, 3032, 9739, 2434, 12140, 2436, 2448, 2438, 3016, 9998, 2440, 3024, 2442, 2452, 2444, 3040, 2450, 6685} // 4 dose
	    };

        public static bool isEating(Player p, int item, int slot)
        {
            for (int i = 0; i < FOOD.Length; i++)
            {
                for (int j = 0; j < FOOD[i].Length; j++)
                {
                    if (item == FOOD[i][j])
                    {
                        eatFood(p, i, j, slot);
                        return true;
                    }
                }
            }
            for (int i = 0; i < POTIONS.Length; i++)
            {
                for (int j = 0; j < POTIONS[i].Length; j++)
                {
                    if (item == POTIONS[i][j])
                    {
                        drinkPotion(p, i, j, slot);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void drinkPotion(Player p, int i, int j, int slot)
        {
            //TODO antipoisons/antifire
            int lastPotion = -1;
            int delay = 500;
            long lastDrink = 0;
            if (p.isDead() || p.getTemporaryAttribute("willDie") != null)
            {
                return;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().ruleEnabled(DuelSession.RULE.NO_DRINKS))
                {
                    p.getPackets().sendMessage("Drinks have been disabled for this duel!");
                    return;
                }
            }
            if (p.getTemporaryAttribute("lastDrankPotion") != null)
            {
                lastPotion = (int)p.getTemporaryAttribute("lastDrankPotion");
            }
            if (p.getTemporaryAttribute("drinkPotionTimer") != null)
            {
                lastDrink = (int)p.getTemporaryAttribute("drinkPotionTimer");
            }
            int time = (j == lastPotion) ? 1000 : 500;
            if (Environment.TickCount - lastDrink < time)
            {
                return;
            }
            p.getPackets().closeInterfaces();
            p.setTemporaryAttribute("drinkPotionTimer", Environment.TickCount);
            p.setTemporaryAttribute("lastDrankPotion", j);
            p.setTarget(null);
            p.resetCombatTurns();
            p.setEntityFocus(65535);
            p.removeTemporaryAttribute("autoCasting");

            Event drinkPotionEvent = new Event(delay);
            drinkPotionEvent.setAction(() =>
            {
                drinkPotionEvent.stop();
                if (p.isDead() || p.getSkills().getCurLevel(Skills.SKILL.HITPOINTS) <= 0)
                {
                    return;
                }
                int item = i != 0 && POTIONS[i - 1][j] != -1 ? POTIONS[i - 1][j] : 229;
                if (!p.getInventory().replaceItemSlot(POTIONS[i][j], item, slot))
                {
                    return;
                }
                string drinkPotion = ItemData.forId(POTIONS[0][j]).getName().Replace("(", "").Replace(")", "").Replace("3", "").Replace("2", "").Replace("1", "").ToLower();
                p.getPackets().sendMessage("You drink some of your " + drinkPotion + ".");
                p.getPackets().sendMessage("You have " + (i == 0 ? "no" : i.ToString()) + " doses of potion left.");
                switch (j)
                {
                    case 0: //Attack potion [+3 and 10% of max attack]
                        statBoost(p, Skills.SKILL.ATTACK, 0.10, 3, false);
                        break;

                    case 1: //Antipoison potion
                        p.setPoisonAmount(0);
                        break;

                    case 2: //Strength potion [+3 and 10% of max strength]
                        statBoost(p, Skills.SKILL.STRENGTH, 0.10, 3, false);
                        break;

                    case 3: //Restore potion [restores randomly between 10-39 points]
                        restorePotion(p, Skills.SKILL.DEFENCE, 10, 39);
                        restorePotion(p, Skills.SKILL.ATTACK, 10, 39);
                        restorePotion(p, Skills.SKILL.STRENGTH, 10, 39);
                        restorePotion(p, Skills.SKILL.RANGE, 10, 39);
                        restorePotion(p, Skills.SKILL.HITPOINTS, 10, 39);
                        break;

                    case 4: //Energy potion [restores 20% energy]
                        double newEnergy = p.getRunEnergy() * 0.20;
                        p.setRunEnergy(((p.getRunEnergy() + (int)newEnergy >= 100) ? 100 : (p.getRunEnergy() + (int)newEnergy)));
                        break;

                    case 5: //Defence potion [Should be +3 and 10% of max defence]
                        statBoost(p, Skills.SKILL.DEFENCE, 0.10, 3, false);
                        break;

                    case 6: //Agility potion [restores 2 or 3 agility points]
                        int newAgility = Misc.random(2, 3) + p.getSkills().getCurLevel(Skills.SKILL.AGILITY);
                        if (newAgility < p.getSkills().getMaxLevel(Skills.SKILL.AGILITY))
                            p.getSkills().setCurLevel(Skills.SKILL.AGILITY, newAgility);
                        break;

                    case 7: //Combat potion [Should be 10% of attack+strength and +3 to each].
                        statBoost(p, Skills.SKILL.ATTACK, 0.10, 3, false);
                        statBoost(p, Skills.SKILL.STRENGTH, 0.10, 3, false);
                        break;

                    case 8: //Prayer potion, [heals 7-31, formula = 7+floor(prayerlevel/4)]
                        int newPrayer = 7 + (int)Math.Floor((double)(p.getSkills().getMaxLevel(Skills.SKILL.PRAYER) / 4)) + p.getSkills().getCurLevel(Skills.SKILL.PRAYER);
                        if (newPrayer < p.getSkills().getCurLevel(Skills.SKILL.PRAYER))
                            p.getSkills().setCurLevel(Skills.SKILL.PRAYER, newPrayer);
                        checkOverdose(p, Skills.SKILL.PRAYER);
                        break;

                    case 9: //Summoning potion [25% of players summoning + 7]
                        int newSummoning = (7 + (int)((double)p.getSkills().getMaxLevel(Skills.SKILL.SUMMONING) * 0.25)) + p.getSkills().getCurLevel(Skills.SKILL.SUMMONING);
                        if (newSummoning < p.getSkills().getCurLevel(Skills.SKILL.SUMMONING))
                            p.getSkills().setCurLevel(Skills.SKILL.SUMMONING, newSummoning);

                        statBoost(p, Skills.SKILL.STRENGTH, 0.10, 3, false);
                        break;

                    case 10: //Super attack potion [15% of players attack + 5]
                        statBoost(p, Skills.SKILL.ATTACK, 0.15, 5, false);
                        break;

                    case 11: // super antipoison
                        p.setPoisonAmount(0);
                        p.setSuperAntipoisonCycles(20);
                        break;

                    case 12: //Fishing potion [fishing +3]
                        if (p.getSkills().getCurLevel(Skills.SKILL.FISHING) < (p.getSkills().getMaxLevel(Skills.SKILL.FISHING) + 3))
                            p.getSkills().setCurLevel(Skills.SKILL.FISHING, p.getSkills().getCurLevel(Skills.SKILL.FISHING) + 3);
                        break;

                    case 13:
                        p.setRunEnergy(p.getRunEnergy() + 20);
                        if (p.getRunEnergy() >= 100)
                        {
                            p.setRunEnergy(100);
                        }
                        break;

                    case 14: //Hunter potion [hunting + 3]
                        if (p.getSkills().getCurLevel(Skills.SKILL.HUNTER) < (p.getSkills().getMaxLevel(Skills.SKILL.HUNTER) + 3))
                            p.getSkills().setCurLevel(Skills.SKILL.HUNTER, p.getSkills().getCurLevel(Skills.SKILL.HUNTER) + 3);
                        break;

                    case 15: //Super strength [strength 15% +5]
                        statBoost(p, Skills.SKILL.STRENGTH, 0.15, 5, false);
                        break;

                    case 16: //restores all skills by 33%.
                        foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                            superRestorePotion(p, skill, 0.33);
                        break;

                    case 17://Super defence [defence 15% +5]
                        statBoost(p, Skills.SKILL.DEFENCE, 0.15, 5, false);
                        break;

                    case 18: // Antifire potion
                        p.setAntifireCycles(20);
                        break;

                    case 19: //Ranging potions
                        statBoost(p, Skills.SKILL.RANGE, 0.10, 4, false);
                        break;

                    case 20: //Magic potion.
                        if (p.getSkills().getCurLevel(Skills.SKILL.MAGIC) < (p.getSkills().getMaxLevel(Skills.SKILL.MAGIC) + 4))
                            p.getSkills().setCurLevel(Skills.SKILL.MAGIC, p.getSkills().getCurLevel(Skills.SKILL.MAGIC) + 4);
                        break;

                    case 21: //Zamorak brew potion. [Attack %20+2][Strength %12 +2][Defense -10% + -2][hitpoints -10% + 20]
                        statBoost(p, Skills.SKILL.ATTACK, 0.20, 2, false);
                        statBoost(p, Skills.SKILL.STRENGTH, 0.12, 2, false);
                        statBoost(p, Skills.SKILL.DEFENCE, 0.10, 2, true);
                        statBoost(p, Skills.SKILL.HITPOINTS, 0.10, 20, true);
                        break;

                    case 22: //Saradomin brew potion. [Hitpoints +%15][Defense +25%][Strength, Attack, Magic and Ranged -10%]
                        statBoost(p, Skills.SKILL.HITPOINTS, 0.15, 0, false);
                        statBoost(p, Skills.SKILL.DEFENCE, 0.25, 0, false);
                        statBoost(p, Skills.SKILL.STRENGTH, 0.10, 0, true);
                        statBoost(p, Skills.SKILL.ATTACK, 0.10, 0, true);
                        statBoost(p, Skills.SKILL.MAGIC, 0.10, 0, true);
                        statBoost(p, Skills.SKILL.RANGE, 0.10, 0, true);
                        break;
                }
                p.setLastAnimation(new Animation(829));
                p.getPackets().sendSkillLevels();
            });
            Server.registerEvent(drinkPotionEvent);
        }

        private static void eatFood(Player p, int i, int j, int slot)
        {
            //delay at which you eat food 0.5 seconds (half a second per eat).
            int delay = 500;
            //If you are dead or yourHp is zero (this should be enough), also if variable is set to die from next attack
            if (p.isDead() || p.getHp() <= 0 || p.getTemporaryAttribute("willDie") != null)
                return;

            //Last time you ate timer was previously set.
            if (p.getTemporaryAttribute("eatFoodTimer") != null)
            {
                //Check if the timer has passed the time of 1.2 seconds
                if (Environment.TickCount - (int)p.getTemporaryAttribute("eatFoodTimer") < 1200)
                {
                    return;
                }
            }
            //if you are in a duel
            if (p.getDuel() != null)
            {
                //dueling with No Food rule enabled
                if (p.getDuel().ruleEnabled(DuelSession.RULE.NO_FOOD))
                {
                    p.getPackets().sendMessage("Food has been disabled for this duel!");
                    return;
                }
            }
            //Set timer right now because you are eating some food.
            p.setTemporaryAttribute("eatFoodTimer", Environment.TickCount);
            //while you are eating the target you are attacking gets reset.
            p.setTarget(null);
            p.resetCombatTurns();
            p.setEntityFocus(65535);
            p.getPackets().closeInterfaces();
            p.removeTemporaryAttribute("autoCasting");

            //start eating the food at delay which is 0.5 of a second / half a second.
            Event eatFoodEvent = new Event(delay);
            eatFoodEvent.setAction(() =>
            {
                //make the food eating event stop after this time.
                eatFoodEvent.stop();
                //if you are dead or your hp is zero aka dead.
                if (p.isDead() || p.getHp() <= 0)
                {
                    return;
                }
                int newHealth = p.getSkills().getCurLevel(Skills.SKILL.HITPOINTS) + FOOD_HEAL[j];
                int item = i != 2 && FOOD[i + 1][j] != -1 ? FOOD[i + 1][j] : -1;
                if (!p.getInventory().replaceItemSlot(FOOD[i][j], item, slot))
                {
                    return;
                }
                p.getPackets().sendMessage("You eat the " + ItemData.forId(FOOD[i][j]).getName().ToLower() + ".");
                p.getSkills().setCurLevel(Skills.SKILL.HITPOINTS, newHealth > p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS) ? p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS) : newHealth);
                p.setLastAnimation(new Animation(829));
                p.getPackets().sendSkillLevel(Skills.SKILL.HITPOINTS);
            });
            Server.registerEvent(eatFoodEvent);
        }

        public static void statBoost(Player p, Skills.SKILL skill, double percentage, int additionalLevels, bool decreaseStat)
        {
            int maxPossibleIncrease = (int)((double)p.getSkills().getMaxLevel(skill) * percentage) + additionalLevels;
            maxPossibleIncrease += p.getSkills().getMaxLevel(skill);

            if (!decreaseStat)
            { //increase stat
                p.getSkills().setCurLevel(skill, maxPossibleIncrease);
            }
            else
            {
                p.getSkills().setCurLevel(skill, p.getSkills().getCurLevel(skill) - maxPossibleIncrease);
                if (p.getSkills().getCurLevel(skill) <= 0)
                    p.getSkills().setCurLevel(skill, 1);
            }
        }

        public static void restorePotion(Player p, Skills.SKILL skill, int minRestore, int maxRestore)
        {
            int restoreAmount = Misc.random(minRestore, maxRestore);
            if (p.getSkills().getCurLevel(skill) >= p.getSkills().getMaxLevel(skill))
                return;
            int mustRestore = p.getSkills().getMaxLevel(skill) - p.getSkills().getCurLevel(skill);
            if (restoreAmount > mustRestore) restoreAmount = mustRestore;
            p.getSkills().setCurLevel(skill, p.getSkills().getCurLevel(skill) + restoreAmount);
            checkOverdose(p, skill);
        }

        public static void superRestorePotion(Player p, Skills.SKILL skill, double percentage)
        {
            percentage = p.getSkills().getMaxLevel(skill) * percentage;
            if (p.getSkills().getCurLevel(skill) == p.getSkills().getMaxLevel(skill) * percentage)
                return;
            if (p.getSkills().getCurLevel(skill) >= p.getSkills().getMaxLevel(skill))
                return;
            p.getSkills().setCurLevel(skill, p.getSkills().getCurLevel(skill) + (int)percentage);
            checkOverdose(p, skill);
            if (p.getSkills().getCurLevel(skill) <= 0)
                p.getSkills().setCurLevel(skill, 1);
        }

        public static void checkOverdose(Player p, Skills.SKILL skill)
        {
            if (p.getSkills().getCurLevel(skill) >= p.getSkills().getMaxLevel(skill))
            {
                p.getSkills().setCurLevel(skill, p.getSkills().getMaxLevel(skill));
            }
        }
    }
}