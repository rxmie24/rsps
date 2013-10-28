using System;

namespace RS2.Server.npc
{
    internal class NpcSkills
    {
        public static int SKILL_COUNT = 4; //Npc has 4 skills I guess.

        private Npc npc;

        public enum SKILL
        {
            ATTACK, DEFENCE, STRENGTH, HITPOINTS
        };

        private int[] curLevel = new int[SKILL_COUNT];
        private int[] maxLevel = new int[SKILL_COUNT];

        public NpcSkills(Npc npc)
        {
            this.npc = npc;
            for (int i = 0; i < SKILL_COUNT - 1; i++)
            {
                curLevel[i] = 1;
                maxLevel[i] = 1;
            }
            curLevel[3] = 10;
            maxLevel[3] = 10;
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

        public void setCurLevel(SKILL skill, int lvl)
        {
            int i_skill = Convert.ToInt32(skill);
            curLevel[i_skill] = lvl;
            if (curLevel[i_skill] <= 0)
                curLevel[i_skill] = 0;
            else if (curLevel[i_skill] > maxLevel[i_skill])
                curLevel[i_skill] = maxLevel[i_skill];
        }

        public void setMaxLevel(SKILL skill, int lvl)
        {
            maxLevel[Convert.ToInt32(skill)] = lvl;
        }
    }
}