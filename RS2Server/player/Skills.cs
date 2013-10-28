using RS2.Server.model;
using RS2.Server.player.skills.prayer;
using System;

namespace RS2.Server.player
{
    internal class Skills
    {
        public static int SKILL_COUNT = 24;
        public static double MAXIMUM_EXP = 200000000;

        public enum SKILL
        {
            ATTACK, DEFENCE, STRENGTH, HITPOINTS, RANGE, PRAYER,
            MAGIC, COOKING, WOODCUTTING, FLETCHING, FISHING, FIREMAKING,
            CRAFTING, SMITHING, MINING, HERBLORE, AGILITY, THIEVING,
            SLAYER, FARMING, RUNECRAFTING, HUNTER, CONSTRUCTION, SUMMONING
        };

        public static string[] SKILL_NAME = {
		    "Attack", "Defence", "Strength", "Hitpoints", "Range", "Prayer",
		    "Magic", "Cooking", "Woodcutting",  "Fletching", "Fishing", "Firemaking",
		    "Crafting", "Smithing", "Mining", "Herblore", "Agility", "Thieving",
            "Slayer", "Farming", "Runecrafting", "Hunter", "Construction", "Summoning",
	    };

        public static int[] SKILL_FLASH_BITMASKS = {
		    0x1, 0x4, 0x2, 0x40, 0x8, 0x10,
            0x20, 0x8000, 0x20000, 0x800, 0x4000, 0x10000,
            0x400, 0x2000, 0x1000, 0x100, 0x80, 0x200,
            0x80000, 0x100000, 0x40000, 0x400000, 0x200000, 0x800000
        };

        public static int[] SKILL_ICON_BITMASKS = {
            0x4000000, 0x14000000, 0x8000000, 0x18000000, 0xC000000, 0x1C000000,
            0x10000000, 0x40000000, 0x48000000, 0x4C000000, 0x3C000000, 0x44000000,
            0x2C000000, 0x38000000, 0x34000000, 0x24000000, 0x20000000, 0x28000000,
            0x50000000, 0x54000000, 0x30000000, 0x5C000000, 0x58000000, 0x60000000
        };

        private int[] curLevel = new int[SKILL_COUNT];
        private int[] maxLevel = new int[SKILL_COUNT];
        private double[] xp = new double[SKILL_COUNT];
        private Player player;

        public Skills(Player player)
        {
            this.player = player;
            for (int i = 0; i < SKILL_COUNT; i++)
            {
                curLevel[i] = 1;
                maxLevel[i] = 1;
                xp[i] = 0;
            }
            curLevel[3] = 10;
            maxLevel[3] = 10;
            xp[3] = 1184;
        }

        public int getCombatLevel()
        {
            int attack = getMaxLevel(SKILL.ATTACK);
            int defence = getMaxLevel(SKILL.DEFENCE);
            int strength = getMaxLevel(SKILL.STRENGTH);
            int hp = getMaxLevel(SKILL.HITPOINTS);
            int prayer = getMaxLevel(SKILL.PRAYER);
            int ranged = getMaxLevel(SKILL.RANGE);
            int magic = getMaxLevel(SKILL.MAGIC);
            int summoning = getMaxLevel(SKILL.SUMMONING);

            double combatLevel = (defence + hp + Math.Floor((double)prayer / 2) + Math.Floor((double)summoning / 2)) * 0.25;
            double warrior = (attack + strength) * 0.325;
            double ranger = ranged * 0.4875;
            double mage = magic * 0.4875;

            return (int)(combatLevel + Math.Max(warrior, Math.Max(ranger, mage)));
        }

        public int getCurLevel(SKILL skill)
        {
            return curLevel[Convert.ToInt32(skill)];
        }

        public int getMaxLevel(SKILL skill)
        {
            return maxLevel[Convert.ToInt32(skill)];
        }

        public int getGreaterLevel(SKILL skill)
        {
            return Math.Max(curLevel[Convert.ToInt32(skill)], maxLevel[Convert.ToInt32(skill)]);
        }

        public double getXp(SKILL skill)
        {
            return xp[Convert.ToInt32(skill)];
        }

        public int getMaxLevel(int skill)
        {
            return maxLevel[skill];
        }

        public void setCurLevel(SKILL skill, int lvl)
        {
            int i_skill = Convert.ToInt32(skill);

            curLevel[i_skill] = lvl;
            //Hitpoints and prayer are the only two skills which can hit 0.
            if (curLevel[i_skill] <= 0)
            {
                if (skill == SKILL.HITPOINTS || skill == SKILL.PRAYER)
                    curLevel[i_skill] = 0;
                else
                    curLevel[i_skill] = 1;
            }
            if (skill == SKILL.PRAYER)
            {
                if (curLevel[i_skill] <= 0 || lvl == 0)
                {
                    curLevel[i_skill] = 0;
                    player.getPackets().sendMessage("You have run out of Prayer points, please recharge your prayer at an altar.");
                    Prayer.deactivateAllPrayers(player);
                }
            }
        }

        public void setXp(SKILL skill, int newXp)
        {
            xp[Convert.ToInt32(skill)] = newXp;
            maxLevel[Convert.ToInt32(skill)] = getLevelForXp(skill);
        }

        public int getLevelForXp(SKILL skill)
        {
            double exp = xp[Convert.ToInt32(skill)];
            int points = 0;
            int output = 0;
            for (int lvl = 1; lvl < 100; lvl++)
            {
                points += (int)Math.Floor((double)lvl + 300.0 * Math.Pow(2.0, (double)lvl / 7.0));
                output = (int)Math.Floor((double)points / 4);
                if ((output - 1) >= exp)
                {
                    return lvl;
                }
            }
            return 99;
        }

        public static int getXpForLevel(int level)
        {
            int points = 0;
            int output = 0;
            for (int lvl = 1; lvl <= level; lvl++)
            {
                points += (int)Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
                if (lvl >= level)
                {
                    return output;
                }
                output = (int)Math.Floor((double)points / 4);
            }
            return 0;
        }

        public void addXp(SKILL skill, double exp)
        {
            int skillIndex = Convert.ToInt32(skill);
            int oldLevel = getLevelForXp(skill);
            xp[skillIndex] += exp;
            if (xp[skillIndex] >= MAXIMUM_EXP)
            {
                xp[skillIndex] = MAXIMUM_EXP;
            }
            int newLevel = getLevelForXp(skill);
            if (newLevel > oldLevel && newLevel <= 99)
            {
                if (skillIndex != 3)
                {
                    curLevel[skillIndex] = newLevel;
                    maxLevel[skillIndex] = newLevel;
                }
                else
                {
                    curLevel[3]++;
                    maxLevel[3]++;
                }
                levelUp(player, skill);
            }
            player.getPackets().sendSkillLevel(skill);
        }

        public static void levelUp(Player player, SKILL skill)
        {
            int skillIndex = Convert.ToInt32(skill);
            String s = "<br><br><br>";
            String s1 = "<br><br><br><br>";
            if (player.getTemporaryAttribute("teleporting") == null)
            {
                player.setLastGraphics(new Graphics(199, 0, 100));
            }

            int skillFlashFlags = (int)(player.getTemporaryAttribute("skillFlashFlags") == null ? 0 : (int)player.getTemporaryAttribute("skillFlashFlags"));
            skillFlashFlags |= SKILL_FLASH_BITMASKS[skillIndex];
            player.setTemporaryAttribute("skillFlashFlags", skillFlashFlags);

            player.getPackets().sendMessage("You've just advanced a " + SKILL_NAME[skillIndex] + " level! You have reached level " + player.getSkills().getMaxLevel(skillIndex) + ".");
            player.getPackets().modifyText(s + "Congratulations, you have just advanced a " + SKILL_NAME[skillIndex] + " level!", 740, 0);
            player.getPackets().modifyText(s1 + "You have now reached level " + player.getSkills().getMaxLevel(skillIndex) + ".", 740, 1);
            player.getPackets().modifyText("", 740, 2);
            player.getPackets().sendConfig(1179, SKILL_ICON_BITMASKS[skillIndex] | skillFlashFlags); //start flashing appropriate skill icons
            player.getPackets().sendChatboxInterface2(740);
            player.getUpdateFlags().setAppearanceUpdateRequired(true);
        }

        public bool hasMultiple99s()
        {
            int j = 0;
            for (int i = 0; i < SKILL_COUNT; i++)
            {
                if (maxLevel[i] >= 99)
                {
                    j++;
                }
            }
            return j > 1;
        }

        public int getTotalXp()
        {
            double total = 0;
            for (int i = 0; i < SKILL_COUNT; i++)
            {
                total += xp[i];
            }
            return (int)total;
        }

        public int getTotalLevel()
        {
            int total = 0;
            for (int i = 0; i < SKILL_COUNT; i++)
            {
                total += maxLevel[i];
            }
            return total;
        }
    }
}