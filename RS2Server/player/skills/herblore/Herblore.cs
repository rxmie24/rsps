using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.herblore
{
    internal class Herblore : HerbloreData
    {
        public Herblore()
        {
        }

        public static bool doingHerblore(Player p, int itemUsed, int usedWith)
        {
            int itemOne = itemUsed;
            int itemTwo = usedWith;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    itemOne = usedWith;
                    itemTwo = itemUsed;
                }
                for (int j = 0; j < SECONDARY.Length; j++)
                {
                    if (itemOne == SECONDARY[j] && itemTwo == UNFINISHED[j])
                    {
                        setHerbloreItem(p, null);
                        shopPotionOptions(p, j);
                        return true;
                    }
                }
                for (int j = 0; j < UNCRUSHED.Length; j++)
                {
                    if (itemOne == UNCRUSHED[j] && itemTwo == PESTLE_AND_MORTAR)
                    {
                        setHerbloreItem(p, null);
                        showGrindOptions(p, j);
                        return true;
                    }
                }
                for (int j = 0; j < CLEAN_HERBS.Length; j++)
                {
                    if (itemOne == CLEAN_HERBS[j] && itemTwo == VIAL_OF_WATER)
                    {
                        setHerbloreItem(p, null);
                        shopUnfinishedOptions(p, j);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool emptyPotion(Player p, int item, int slot)
        {
            if (item == VIAL_OF_WATER)
            {
                if (p.getInventory().replaceItemSlot(VIAL_OF_WATER, VIAL, slot))
                {
                    return true;
                }
            }
            for (int i = 0; i < DOSES.Length; i++)
            {
                for (int j = 0; j < DOSES[0].Length; j++)
                {
                    if (item == DOSES[i][j])
                    {
                        p.getInventory().replaceItemSlot(item, VIAL, slot);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void shopPotionOptions(Player p, int index)
        {
            string s = "<br><br><br><br>";
            p.getPackets().sendChatboxInterface(309);
            p.getPackets().itemOnInterface(309, 2, 110, END_POTION[index]);
            p.getPackets().modifyText(s + ItemData.forId(END_POTION[index]).getName(), 309, 6);
            p.setTemporaryAttribute("completePotion", index);
        }

        private static void shopUnfinishedOptions(Player p, int index)
        {
            string s = "<br><br><br><br>";
            string s1 = index == 6 ? "Spirit weed" : ItemData.forId(CLEAN_HERBS[index]).getName();
            p.getPackets().sendChatboxInterface(309);
            p.getPackets().itemOnInterface(309, 2, 110, UNFINISHED_POTION[index]);
            p.getPackets().modifyText(s + " " + s1 + " potion (unf) ", 309, 6);
            p.setTemporaryAttribute("unfinishedPotion", index);
        }

        private static void showGrindOptions(Player p, int index)
        {
            string s = "<br><br><br><br>";
            p.getPackets().sendChatboxInterface(309);
            p.getPackets().itemOnInterface(309, 2, 150, CRUSHED[index]);
            p.getPackets().modifyText(s + ItemData.forId(CRUSHED[index]).getName(), 309, 6);
            p.setTemporaryAttribute("herbloreGrindItem", index);
        }

        public static void completePotion(Player p, int amount, bool newMix)
        {
            if (newMix && p.getTemporaryAttribute("completePotion") == null)
            {
                return;
            }
            if (!newMix && p.getTemporaryAttribute("herbloreItem") == null)
            {
                return;
            }
            if (newMix)
            {
                if (p.getTemporaryAttribute("completePotion") == null)
                {
                    return;
                }
                int index = (int)p.getTemporaryAttribute("completePotion");
                p.setTemporaryAttribute("herbloreItem", new Potion(END_POTION[index], UNFINISHED[index], SECONDARY[index], POTION_LEVEL[index], POTION_XP[index], amount));
            }
            Potion item = (Potion)p.getTemporaryAttribute("herbloreItem");
            if (item == null || p == null || item.getAmount() <= 0)
            {
                resetAllHerbloreVariables(p);
                return;
            }
            if (!p.getInventory().hasItem(item.getSecondary()) || !p.getInventory().hasItem(item.getUnfinished()))
            {
                resetAllHerbloreVariables(p);
                return;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.HERBLORE) < item.getLevel())
            {
                p.getPackets().sendMessage("You need a Herblore level of " + item.getLevel() + " to make that potion.");
                resetAllHerbloreVariables(p);
                return;
            }
            string s = ItemData.forId(item.getFinished()).getName().Replace("(3)", "");
            if (p.getInventory().deleteItem(item.getUnfinished()) && p.getInventory().deleteItem(item.getSecondary()))
            {
                if (p.getInventory().addItem(item.getFinished()))
                {
                    item.decreaseAmount();
                    p.setLastAnimation(new Animation(MIX_ANIMATION));
                    p.getSkills().addXp(Skills.SKILL.HERBLORE, item.getXp());
                    p.getPackets().sendMessage("You add the ingredient into the murky vial, you have completed the potion.");
                    p.getPackets().closeInterfaces();
                }
            }
            if (item.getAmount() >= 1)
            {
                Event completeMorePotionsEvent = new Event(750);
                completeMorePotionsEvent.setAction(() =>
                {
                    completePotion(p, item.getAmount(), false);
                    completeMorePotionsEvent.stop();
                });
                Server.registerEvent(completeMorePotionsEvent);
            }
        }

        public static void makeUnfinishedPotion(Player p, int amount, bool newMix)
        {
            if (newMix && p.getTemporaryAttribute("unfinishedPotion") == null)
            {
                return;
            }
            if (!newMix && p.getTemporaryAttribute("herbloreItem") == null)
            {
                return;
            }
            if (newMix)
            {
                if (p.getTemporaryAttribute("unfinishedPotion") == null)
                {
                    return;
                }
                int index = (int)p.getTemporaryAttribute("unfinishedPotion");
                p.setTemporaryAttribute("herbloreItem", new Potion(UNFINISHED_POTION[index], -1, CLEAN_HERBS[index], -1, -1, amount));
            }
            Potion item = (Potion)p.getTemporaryAttribute("herbloreItem");
            if (!p.getInventory().hasItem(item.getSecondary()) || !p.getInventory().hasItem(VIAL_OF_WATER))
            {
                return;
            }
            if (p.getInventory().deleteItem(VIAL_OF_WATER) && p.getInventory().deleteItem(item.getSecondary()))
            {
                if (p.getInventory().addItem(item.getFinished()))
                {
                    item.decreaseAmount();
                    p.setLastAnimation(new Animation(MIX_ANIMATION));
                    p.getPackets().sendMessage("You mix the " + ItemData.forId(item.getSecondary()).getName() + " into a vial of water.");
                    p.getPackets().closeInterfaces();
                }
            }
            if (item.getAmount() >= 1)
            {
                Event makeMoreUnfinishedPotionsEvent = new Event(750);
                makeMoreUnfinishedPotionsEvent.setAction(() =>
                {
                    makeUnfinishedPotion(p, item.getAmount(), false);
                    makeMoreUnfinishedPotionsEvent.stop();
                });
                Server.registerEvent(makeMoreUnfinishedPotionsEvent);
            }
        }

        public static void grindIngredient(Player p, int amount, bool newGrind)
        {
            if (newGrind && p.getTemporaryAttribute("herbloreGrindItem") == null)
            {
                return;
            }
            if (!newGrind && p.getTemporaryAttribute("herbloreItem") == null)
            {
                return;
            }
            if (newGrind)
            {
                if (p.getTemporaryAttribute("herbloreGrindItem") == null)
                {
                    return;
                }
                int index = (int)p.getTemporaryAttribute("herbloreGrindItem");
                p.setTemporaryAttribute("herbloreItem", new SkillItem(CRUSHED[index], UNCRUSHED[index], -1, -1, -1, -1, amount));
            }
            SkillItem item = (SkillItem)p.getTemporaryAttribute("herbloreItem");
            if (!p.getInventory().hasItem(PESTLE_AND_MORTAR))
            {
                p.getPackets().sendMessage("You do not have a Pestle and mortar.");
                return;
            }
            if (p.getInventory().replaceSingleItem(item.getItemOne(), item.getFinishedItem()))
            {
                item.decreaseAmount();
                p.setLastAnimation(new Animation(GRIND_ANIMATION));
                p.getPackets().sendMessage("You grind the " + ItemData.forId(item.getItemOne()).getName() + " into a fine dust.");
                p.getPackets().closeInterfaces();
            }
            if (item.getAmount() >= 1)
            {
                Event grindMoreIngredientEvent = new Event(750);
                grindMoreIngredientEvent.setAction(() =>
                {
                    grindIngredient(p, item.getAmount(), false);
                    grindMoreIngredientEvent.stop();
                });
                Server.registerEvent(grindMoreIngredientEvent);
            }
        }

        private static void identifyHerb(Player p, int herb)
        {
            if (p.getTemporaryAttribute("identifyHerbTimer") != null)
            {
                if (Environment.TickCount - (int)p.getTemporaryAttribute("identifyHerbTimer") < 250)
                {
                    return;
                }
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.HERBLORE) < HERB_LVL[herb])
            {
                p.getPackets().sendMessage("You need a Herblore level of " + HERB_LVL[herb] + " to clean that herb.");
                return;
            }
            if (p.getInventory().replaceSingleItem(GRIMY_HERBS[herb], CLEAN_HERBS[herb]))
            {
                p.getPackets().sendMessage("You clean the " + HERB_NAME[herb] + ".");
                p.getSkills().addXp(Skills.SKILL.HERBLORE, HERB_XP[herb]);
                p.setTemporaryAttribute("identifyHerbTimer", Environment.TickCount);
            }
        }

        public static bool idHerb(Player p, int item)
        {
            for (int j = 0; j < GRIMY_HERBS.Length; j++)
            {
                if (item == GRIMY_HERBS[j])
                {
                    identifyHerb(p, j);
                    setHerbloreItem(p, null);
                    return true;
                }
            }
            return false;
        }

        private static void resetAllHerbloreVariables(Player p)
        {
            p.removeTemporaryAttribute("herbloreGrindItem");
            p.removeTemporaryAttribute("fillingVials");
            p.removeTemporaryAttribute("unfinishedPotion");
            p.removeTemporaryAttribute("completePotion");
        }

        public static void setHerbloreItem(Player p, object a)
        {
            if (a == null)
            {
                resetAllHerbloreVariables(p);
                p.removeTemporaryAttribute("herbloreItem");
                return;
            }
            p.setTemporaryAttribute("herbloreItem", (object)a);
        }

        public static bool mixDoses(Player p, int itemUsed, int usedWith, int usedSlot, int withSlot)
        {
            int ONE = 0, TWO = 1, THREE = 2, FOUR = 3;
            for (int i = 0; i < DOSES.Length; i++)
            {
                for (int j = 0; j < DOSES[ONE].Length; j++)
                {
                    if (itemUsed == DOSES[ONE][j] && usedWith == DOSES[ONE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], DOSES[TWO][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[TWO][j] && usedWith == DOSES[TWO][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], DOSES[THREE][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[THREE][j] && usedWith == DOSES[THREE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], DOSES[TWO][j], usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], DOSES[FOUR][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[ONE][j] && usedWith == DOSES[TWO][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], DOSES[THREE][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[TWO][j] && usedWith == DOSES[ONE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], DOSES[THREE][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[ONE][j] && usedWith == DOSES[THREE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], DOSES[FOUR][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[THREE][j] && usedWith == DOSES[ONE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], VIAL, usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[ONE][j], DOSES[FOUR][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[TWO][j] && usedWith == DOSES[THREE][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], DOSES[ONE][j], usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], DOSES[FOUR][j], withSlot);
                        return true;
                    }
                    if (itemUsed == DOSES[THREE][j] && usedWith == DOSES[TWO][j])
                    {
                        p.getInventory().replaceItemSlot(DOSES[THREE][j], DOSES[ONE][j], usedSlot);
                        p.getInventory().replaceItemSlot(DOSES[TWO][j], DOSES[FOUR][j], withSlot);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}