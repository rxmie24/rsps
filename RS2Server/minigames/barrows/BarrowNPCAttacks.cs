using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.minigames.barrows
{
    internal class BarrowNPCAttacks
    {
        public BarrowNPCAttacks()
        {
        }

        public static void attack(Npc npc, Entity target)
        {
            if (npc.isDead() || npc.getOwner() == null || npc.isDestroyed() || target.isDead() || target.isDestroyed())
            {
                return;
            }
            int damage = Misc.random(npc.getMaxHit());
            int prayer = ((Player)target).getPrayers().getHeadIcon();
            int hitDelay = npc.getHitDelay();
            bool special = false;
            switch (npc.getId())
            {
                case 2026: // Dharok
                    int healthHit = (npc.getMaxHp() - npc.getHp()) / 2;
                    damage = Misc.random(damage + healthHit);
                    if (Misc.random(1) == 0)
                    {
                        if (damage < (npc.getMaxHp() / 3))
                        {
                            damage = (npc.getMaxHp() / 4) + Misc.random(damage + healthHit) - (npc.getMaxHp() / 4);
                        }
                    }
                    if (prayer == PrayerData.MELEE)
                    {
                        damage = 0;
                    }
                    break;

                case 2025: // Ahrim
                    hitDelay = 1000;
                    ((Player)target).getPackets().sendProjectile(npc.getLocation(), target.getLocation(), 32, 156, 50, 40, 34, 60, target);
                    if (Misc.random(3) == 0)
                    {
                        special = true;
                        Skills.SKILL[] weakenableSkills = (Skills.SKILL[])Enum.GetValues(typeof(Skills.SKILL));
                        Skills.SKILL weakenedSkill = weakenableSkills[Misc.random(0, 2)];
                        int currentLevel = ((Player)target).getSkills().getCurLevel(weakenedSkill);
                        int newLevel = currentLevel - Misc.random(((Player)target).getSkills().getMaxLevel(weakenedSkill) / 12);
                        newLevel = Math.Max(0, newLevel);
                        ((Player)target).getSkills().setCurLevel(weakenedSkill, newLevel);
                        ((Player)target).getPackets().sendSkillLevel(weakenedSkill);
                    }
                    if (prayer == PrayerData.MAGIC)
                    {
                        damage = 0;
                    }
                    break;

                case 2027: // Guthan
                    if (prayer == PrayerData.MELEE)
                    {
                        damage = 0;
                    }
                    if (Misc.random(3) == 0)
                    {
                        special = true;
                        target.setLastGraphics(new Graphics(398));
                        npc.heal(Misc.random(damage));
                    }
                    break;

                case 2030: // Verac
                    if (Misc.random(1) == 0 && prayer != 0)
                    {
                        if (damage <= npc.getMaxHit() / 2)
                        {
                            damage += npc.getMaxHit() / 2;
                            if (damage > npc.getMaxHit())
                            {
                                damage = npc.getMaxHit();
                            }
                        }
                    }
                    break;

                case 2029: // Torag
                    if (Misc.random(3) == 0)
                    {
                        special = true;
                    }
                    if (prayer == PrayerData.MELEE)
                    {
                        damage = 0;
                    }
                    break;

                case 2028: // Karil
                    hitDelay = 700;
                    ((Player)target).getPackets().sendProjectile(npc.getLocation(), target.getLocation(), 32, 27, 50, 40, 34, 40, target);
                    if (Misc.random(10) == 0)
                    {
                        special = true;
                        int agility = ((Player)target).getSkills().getCurLevel(Skills.SKILL.AGILITY);
                        int newAgility = agility / 4;
                        if (newAgility <= 1)
                            newAgility = 1;
                        ((Player)target).getSkills().setCurLevel(Skills.SKILL.AGILITY, newAgility);
                        ((Player)target).getPackets().sendSkillLevel(Skills.SKILL.AGILITY);
                    }
                    if (Misc.random(1) == 0)
                    {
                        damage = damage > 0 ? damage : Misc.random(npc.getMaxHit());
                    }
                    if (prayer == PrayerData.RANGE)
                    {
                        damage = 0;
                    }
                    break;
            }
            npc.setLastAnimation(new Animation(npc.getAttackAnimation()));
            target.setLastAttacked(Environment.TickCount);
            npc.setLastAttack(Environment.TickCount);
            npc.resetCombatTurns();
            target.setAttacker(npc);
            if ((target.getCombatTurns() > 2 || target.getCombatTurns() < 0))
            {
                target.setLastAnimation(new Animation(target.getDefenceAnimation()));
            }
            if (damage > target.getHp())
            {
                damage = target.getHp();
            }
            Event doHitEvent = new Event(hitDelay);
            doHitEvent.setAction(() =>
            {
                if (npc.getId() == 2025)
                {
                    if (special)
                    {
                        target.setLastGraphics(new Graphics(400, 0, 100));
                        ((Player)target).getPackets().sendMessage("You feel weakened.");
                    }
                    target.setLastGraphics(new Graphics(damage == 0 ? 85 : 157, 0, 100));
                }
                else if (npc.getId() == 2027)
                {
                    if (special)
                    {
                        if (!npc.isDead())
                        {
                            int newHp = npc.getHp() + damage;
                            if (newHp > npc.getMaxHp())
                            {
                                newHp = npc.getMaxHp();
                            }
                            npc.setHp(newHp);
                        }
                    }
                }
                else if (npc.getId() == 2029)
                {
                    if (special)
                    {
                        target.setLastGraphics(new Graphics(399, 0, 100));
                        int energy = ((Player)target).getRunEnergy();
                        int newEnergy = energy - (int)(energy * 0.50);
                        if (newEnergy < 0)
                        {
                            newEnergy = 0;
                        }
                        ((Player)target).getPackets().sendMessage("You feel drained of energy.");
                        ((Player)target).setRunEnergy(newEnergy);
                        ((Player)target).getPackets().sendEnergy();
                    }
                }
                else if (npc.getId() == 2028)
                {
                    if (special)
                    {
                        target.setLastGraphics(new Graphics(399));
                        ((Player)target).getPackets().sendMessage("You feel less agile.");
                    }
                }
                target.hit(damage);
                doHitEvent.stop();
            });
        }
    }
}