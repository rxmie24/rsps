using RS2.Server.player;
using System;

namespace RS2.Server.packethandler.commands
{
    internal class SetLevel : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length < 2)
            {
                player.getPackets().sendMessage("[SetLevel command]: ::setLevel skillId level or ::setLevel skillName level");
                return;
            }

            int skillId = -1;
            int newLevel = 1;
            if (!int.TryParse(arguments[1], out newLevel))
            {
                player.getPackets().sendMessage("[SetLevel command]: ::setLevel skillId level or ::setLevel skillName level");
                return;
            }

            if (newLevel > 99) newLevel = 99;

            if (!int.TryParse(arguments[0], out skillId))
            {
                //string based skills
                arguments[0] = arguments[0].ToUpper();

                if (!Enum.IsDefined(typeof(Skills.SKILL), arguments[0]))
                {
                    player.getPackets().sendMessage("[SetLevel command]: wrong.. try like ::setlevel attack 99 or ::setlevel 0 99");
                    return;
                }
                try
                {
                    Skills.SKILL skillName = (Skills.SKILL)Enum.Parse(typeof(Skills.SKILL), arguments[0], true);
                    player.getSkills().setXp(skillName, 0);
                    player.getSkills().setCurLevel(skillName, newLevel);
                    player.getSkills().addXp(skillName, Skills.getXpForLevel(newLevel));
                    player.getPackets().sendSkillLevel(skillName);
                }
                catch (ArgumentException)
                {
                    player.getPackets().sendMessage("[SetLevel command]: wrong.. try like ::setlevel attack 99 or ::setlevel 0 99");
                }
            }
            else
            {
                if (!Enum.IsDefined(typeof(Skills.SKILL), skillId))
                {
                    player.getPackets().sendMessage("[SetLevel command]: wrong.. try like ::setlevel attack 99 or ::setlevel 0 99");
                    return;
                }
                Skills.SKILL skillName = (Skills.SKILL)skillId;
                player.getSkills().setXp(skillName, 0);
                player.getSkills().setCurLevel(skillName, newLevel);
                player.getSkills().addXp(skillName, Skills.getXpForLevel(newLevel));
                player.getPackets().sendSkillLevel(skillName);
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}