using RS2.Server.npc;
using RS2.Server.player;
using System;

namespace RS2.Server.events
{
    internal class LevelChangeEvent : Event
    {
        private int status;

        public LevelChangeEvent()
            : base(22000)
        {
            status = 0;
        }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                bool updated = false;
                bool rapidHeal = p.getPrayers().isRapidHeal();
                bool rapidRestore = p.getPrayers().isRapidRestore();
                if (p.isDead() || p == null || (status == 0 && !rapidHeal && !rapidRestore))
                {
                    continue;
                }
                foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                {
                    if (skill == Skills.SKILL.PRAYER || (skill == Skills.SKILL.HITPOINTS && p.inCombat()))
                    {
                        continue;
                    }
                    else if (status == 0)
                    {
                        if ((rapidHeal && !rapidRestore && skill != Skills.SKILL.HITPOINTS) || (!rapidHeal && rapidRestore && skill == Skills.SKILL.HITPOINTS))
                        {
                            continue;
                        }
                    }
                    if (p.getSkills().getCurLevel(skill) < p.getSkills().getMaxLevel(skill))
                    {
                        p.getSkills().setCurLevel(skill, p.getSkills().getCurLevel(skill) + 1);
                        updated = true;
                    }
                    else if (p.getSkills().getCurLevel(skill) > p.getSkills().getMaxLevel(skill) && skill != Skills.SKILL.HITPOINTS && status == 1)
                    { // status == 1 so stats DONT go down 2x faster.
                        p.getSkills().setCurLevel(skill, p.getSkills().getCurLevel(skill) - 1);
                        updated = true;
                    }
                }
                if (updated)
                {
                    p.getPackets().sendSkillLevels();
                }
            }
            if (status == 1)
            {
                foreach (Npc n in Server.getNpcList())
                {
                    if (n.getHp() < n.getMaxHp() && !n.isDead() && !n.inCombat())
                    {
                        n.heal(1);
                    }
                }
            }
            status = status == 0 ? 1 : 0;
        }
    }
}