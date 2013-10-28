using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class SkillMenu
    {
        public SkillMenu()
        {
        }

        private static int[] MENU_ID = {
		    1, 2, 5, 3, 7, 4, 12, 22, 6, 8, 9,
		    10, 11, 19, 20, 23, 13, 14, 15, 16,
		    17, 18, 21, 24
	    };

        private static int[] SUB_CONFIG = {
		    0, 1024, 2048, 3072, 4096, 5120, 6144, 7168,
		    8192, 9216, 10240, 11264, 12288, 13312, 14355
	    };

        private static Skills.SKILL[] SKILLS_ORDERED = {
            Skills.SKILL.ATTACK, Skills.SKILL.STRENGTH, Skills.SKILL.DEFENCE, Skills.SKILL.RANGE,
            Skills.SKILL.PRAYER, Skills.SKILL.MAGIC, Skills.SKILL.RUNECRAFTING, Skills.SKILL.CONSTRUCTION,
            Skills.SKILL.HITPOINTS, Skills.SKILL.AGILITY, Skills.SKILL.HERBLORE, Skills.SKILL.THIEVING,
            Skills.SKILL.CRAFTING, Skills.SKILL.FLETCHING, Skills.SKILL.SLAYER, Skills.SKILL.HUNTER,
            Skills.SKILL.MINING, Skills.SKILL.SMITHING, Skills.SKILL.FISHING, Skills.SKILL.COOKING,
            Skills.SKILL.FIREMAKING, Skills.SKILL.WOODCUTTING, Skills.SKILL.FARMING, Skills.SKILL.SUMMONING
        };

        public static int getSkillFlashingIcon(int skillIndex)
        {
            return Skills.SKILL_FLASH_BITMASKS[(int)SKILLS_ORDERED[skillIndex]];
        }

        public static void display(Player player, int buttonId)
        {
            int j = 0;

            for (int i = 125; i < 149; i++)
            {
                if (buttonId == i)
                {
                    int skillFlashFlags = (int)(player.getTemporaryAttribute("skillFlashFlags") == null ? 0 : (int)player.getTemporaryAttribute("skillFlashFlags"));

                    if ((skillFlashFlags & getSkillFlashingIcon(j)) == getSkillFlashingIcon(j))
                    {
                        skillFlashFlags &= ~getSkillFlashingIcon(j); //turn off this skill icon flashing animation bit.
                        player.getPackets().sendConfig(1179, skillFlashFlags); //stop flashing the skill you clicked on, if it was flashing.
                        player.setTemporaryAttribute("skillFlashFlags", skillFlashFlags); //update the variable for remaining flashing skill icons.
                    }
                    player.getPackets().displayInterface(499);
                    player.getPackets().sendConfig(965, MENU_ID[j]);
                    player.setTemporaryAttribute("SkillMenu", (int)MENU_ID[j]);
                    break;
                }
                j++;
            }
        }

        public static void subMenu(Player player, int buttonId)
        {
            int menu = (int)player.getTemporaryAttribute("SkillMenu");
            player.getPackets().sendConfig(965, SUB_CONFIG[buttonId - 10] + menu);
        }
    }
}