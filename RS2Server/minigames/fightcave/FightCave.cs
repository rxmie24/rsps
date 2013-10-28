using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.minigames.fightcave
{
    internal class FightCave
    {
        public FightCave()
        {
        }

        public static void enterCave(Player p)
        {
            AreaEvent enterCaveAreaEvent = new AreaEvent(p, 2438, 5168, 2439, 5168);
            enterCaveAreaEvent.setAction(() =>
            {
                /*
                 * Fight cave is 20k squares from the original place, then another (200 * playerIndex) squares west.
                 */
                Location instanceLocation = new Location((20000 + 2413) + (200 * p.getIndex()), 20000 + 5116, 0);
                p.teleport(instanceLocation);
                p.setFightCave(new FightCaveSession(p));

                Event caveNpcEvent = new Event(600);
                caveNpcEvent.setAction(() =>
                {
                    caveNpcEvent.stop();
                    p.getPackets().sendNPCHead(2617, 242, 1);
                    p.getPackets().modifyText("TzHaar-Mej-Jal", 242, 3);
                    p.getPackets().modifyText("You're on your own now, JalYt.", 242, 4);
                    p.getPackets().modifyText("Pepare to fight for your life!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 1);
                    p.getPackets().sendChatboxInterface2(242);
                });
                Server.registerEvent(caveNpcEvent);
            });
            Server.registerCoordinateEvent(enterCaveAreaEvent);
        }

        public static void exitCave(Player p, int objectX, int objectY)
        {
            AreaEvent exitCaveAreaEvent = new AreaEvent(p, objectX, objectY - 1, objectX + 2, objectY - 1);
            exitCaveAreaEvent.setAction(() =>
            {
                p.setTemporaryAttribute("unmovable", true);
                Event teleFromCaveEvent = new Event(600);
                teleFromCaveEvent.setAction(() =>
                {
                    teleFromCaveEvent.stop();
                    p.getFightCave().teleFromCave(true);
                });
                Server.registerEvent(teleFromCaveEvent);
            });
            Server.registerCoordinateEvent(exitCaveAreaEvent);
        }

        public static void fightCaveAttacks(Npc npc, Player p)
        {
            if (npc.isDead() || npc.isDestroyed() || p.isDead() || p.isDestroyed() || p.isDead() || !Location.inFightCave(p.getLocation()) || p.getTeleportTo() != null)
            {
                return;
            }
            int damage = Misc.random(npc.getMaxHit());
            int prayer = p.getPrayers().getHeadIcon();
            int hitDelay = npc.getHitDelay();
            int animation = npc.getAttackAnimation();
            switch (npc.getId())
            {
                case 2734: // Tz-Kih (lvl 22)
                case 2735:
                    if (prayer == PrayerData.MELEE)
                    {
                        damage = 0;
                    }
                    break;

                case 2739: // Tz-Xil (lvl 90)
                case 2740:
                    if (prayer == PrayerData.RANGE)
                    {
                        damage = 0;
                    }
                    p.getPackets().sendProjectile(npc.getLocation(), p.getLocation(), 32, 1616, 50, 40, 34, 50, p);
                    break;

                case 2741: // Yt-MejKot (lvl 180)
                case 2742:
                    if (prayer == PrayerData.MELEE)
                    {
                        damage = 0;
                    }
                    // TODO healing
                    break;

                case 2743: // Ket-Zek (lvl 360)
                case 2744:
                    if (!p.getLocation().withinDistance(npc.getLocation(), 2))
                    {
                        hitDelay = 1600;
                        animation = 9266;
                        npc.setLastGraphics(new Graphics(1622));
                        damage = Misc.random(49);
                        if (prayer == PrayerData.MAGIC)
                        {
                            damage = 0;
                        }
                        Event sendProjectileToNpc = new Event(300);
                        sendProjectileToNpc.setAction(() =>
                        {
                            sendProjectileToNpc.stop();
                            p.getPackets().sendProjectile(npc.getLocation(), p.getLocation(), 32, 1623, 50, 40, 34, 80, p);
                        });
                        Server.registerEvent(sendProjectileToNpc);
                    }
                    else
                    {
                        damage = Misc.random(64);
                        if (prayer == PrayerData.MELEE)
                        {
                            damage = 0;
                        }
                    }
                    break;

                case 2745: // TzTok Jad (lvl 702)
                    doJadAttacks(p, npc);
                    break;
            }
            if (npc.getId() == 2745)
            {
                return;
            }
            if (animation != 65535)
            {
                npc.setLastAnimation(new Animation(animation));
            }
            p.setLastAttacked(Environment.TickCount);
            npc.setLastAttack(Environment.TickCount);
            p.setAttacker(npc);
            npc.resetCombatTurns();
            if (damage > p.getHp())
            {
                damage = p.getHp();
            }
            int npcId = npc.getId();

            Event losePrayerFightingEvent = new Event(hitDelay);
            losePrayerFightingEvent.setAction(() =>
            {
                losePrayerFightingEvent.stop();
                if (!Location.inFightCave(p.getLocation()) || p.getTeleportTo() != null)
                {
                    return;
                }
                if (npcId == 2734 || npcId == 2735)
                {
                    int prayerLevel = p.getSkills().getCurLevel(Skills.SKILL.PRAYER);
                    int newPrayerLevel = prayerLevel -= (damage + 1);
                    if (newPrayerLevel <= 0)
                    {
                        newPrayerLevel = 0;
                    }
                    p.getSkills().setCurLevel(Skills.SKILL.PRAYER, newPrayerLevel);
                    p.getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
                }
                else if (npcId == 2743 || npcId == 2744)
                {
                    if (Misc.random(1) == 0)
                    {
                        p.setLastGraphics(new Graphics(1624, 0));
                    }
                }
                if ((p.getCombatTurns() > 2 || p.getCombatTurns() < 0))
                {
                    p.setLastAnimation(new Animation(p.getDefenceAnimation()));
                }
                p.hit(damage);
            });
            Server.registerEvent(losePrayerFightingEvent);
        }

        private static void doJadAttacks(Player p, Npc npc)
        {
            if (npc.getHp() <= (npc.getMaxHp() * 0.50))
            {
                if (p.getFightCave() != null)
                {
                    if (!p.getFightCave().isHealersSpawned())
                    {
                        summonJadHealers(p, npc);
                        p.getFightCave().setHealersSpawned(true);
                    }
                }
            }
            npc.resetCombatTurns();
            npc.setEntityFocus(p.getClientIndex());
            switch (Misc.random(1))
            {
                case 0: // Range
                    npc.setLastAnimation(new Animation(9276));
                    npc.setLastGraphics(new Graphics(1625));
                    Event jadRangeAttackEvent = new Event(1600);
                    int jadRangeAttackStatus = 0;
                    jadRangeAttackEvent.setAction(() =>
                    {
                        int hit = 0;
                        int prayer = p.getPrayers().getHeadIcon();
                        if (jadRangeAttackStatus == 0)
                        {
                            jadRangeAttackStatus++;
                            jadRangeAttackEvent.setTick(1500);
                            p.setLastGraphics(new Graphics(451));
                            if (prayer == PrayerData.RANGE)
                            {
                                hit = 0;
                            }
                            else
                            {
                                hit = Misc.random(96);
                            }
                        }
                        else
                        {
                            if (prayer != PrayerData.RANGE)
                            {
                                hit = Misc.random(96);
                            }
                            jadRangeAttackEvent.stop();
                            p.setLastAttacked(Environment.TickCount);
                            npc.setLastAttack(Environment.TickCount);
                            p.setAttacker(npc);
                            if (hit > p.getHp())
                            {
                                hit = p.getHp();
                            }
                            if (!Location.inFightCave(p.getLocation()) || p.getTeleportTo() != null)
                            {
                                return;
                            }
                            if ((p.getCombatTurns() > 2 || p.getCombatTurns() < 0))
                            {
                                p.setLastAnimation(new Animation(p.getDefenceAnimation()));
                            }
                            p.hit(hit);
                            Event animationEvent = new Event(100);
                            animationEvent.setAction(() =>
                            {
                                animationEvent.stop();
                                p.setLastGraphics(new Graphics(157, 0, 100));
                            });
                            Server.registerEvent(animationEvent);
                        }
                    });
                    Server.registerEvent(jadRangeAttackEvent);
                    break;

                case 1: // Magic
                    npc.setLastGraphics(new Graphics(1626));
                    Event jadMagicAttackEvent = new Event(300);
                    int jadMagicAttackStatus = 0;
                    jadMagicAttackEvent.setAction(() =>
                    {
                        int hit = 0;
                        int prayer = p.getPrayers().getHeadIcon();
                        npc.setLastAnimation(new Animation(9278));
                        if (jadMagicAttackStatus == 0)
                        {
                            jadMagicAttackStatus++;
                            jadMagicAttackEvent.setTick(1600);
                            p.getPackets().sendProjectile(npc.getLocation(), p.getLocation(), 32, 1627, 50, 40, 34, 90, p);
                        }
                        else
                        {
                            jadMagicAttackEvent.stop();
                            if (prayer == PrayerData.MAGIC)
                            {
                                hit = 0;
                            }
                            else
                            {
                                hit = Misc.random(96);
                            }
                            p.setLastAttacked(Environment.TickCount);
                            npc.setLastAttack(Environment.TickCount);
                            p.setAttacker(npc);
                            if (hit > p.getHp())
                            {
                                hit = p.getHp();
                            }
                            if (!Location.inFightCave(p.getLocation()) || p.getTeleportTo() != null)
                            {
                                return;
                            }
                            if ((p.getCombatTurns() > 2 || p.getCombatTurns() < 0))
                            {
                                p.setLastAnimation(new Animation(p.getDefenceAnimation()));
                            }
                            p.hit(hit);
                            Event animationEvent = new Event(100);
                            animationEvent.setAction(() =>
                            {
                                animationEvent.stop();
                                p.setLastGraphics(new Graphics(157, 0, 100));
                            });
                            Server.registerEvent(animationEvent);
                        }
                    });
                    Server.registerEvent(jadMagicAttackEvent);
                    break;
            }
        }

        public static void antiTeleportMessage(Player p)
        {
            p.getPackets().sendNPCHead(2617, 242, 1);
            p.getPackets().modifyText("TzHaar-Mej-Jal", 242, 3);
            p.getPackets().modifyText("I cannot allow you to teleport from the fight cave.", 242, 4);
            p.getPackets().modifyText("In Tzhaar, you either win, or die!", 242, 5);
            p.getPackets().animateInterface(9827, 242, 1);
            p.getPackets().sendChatboxInterface2(242);
            p.getPackets().sendMessage("You are unable to teleport from the fight cave.");
        }

        private static void summonJadHealers(Player p, Npc jad)
        {
            for (int i = 0; i < 4; i++)
            {
                Npc npc = new Npc(2746);
                Location minCoords = new Location((20000 + 2363) + (200 * p.getIndex()), 25502, 0);
                Location maxCoords = new Location((20000 + 2430) + (200 * p.getIndex()), 25123, 0);
                npc.setMinimumCoords(minCoords);
                npc.setMaximumCoords(maxCoords);
                npc.setLocation(new Location((20000 + 2387) + (200 * p.getIndex()) + Misc.random(22), 20000 + 5069 + Misc.random(33), 0));
                npc.setEntityFocus(jad.getClientIndex());
                npc.setOwner(p);
                npc.getFollow().setFollowing(jad);
                npc.setTarget(null);
                Server.getNpcList().Add(npc);

                Event jadHealerEvent = new Event(2000);
                jadHealerEvent.setAction(() =>
                {
                    if (npc.isDead() || npc.isHidden() || npc.isDestroyed())
                    {
                        jadHealerEvent.stop();
                        return;
                    }
                    if (npc.getLocation().withinDistance(jad.getLocation(), 2) && !npc.inCombat())
                    {
                        if (Misc.random(7) == 0)
                        {
                            jad.setLastGraphics(new Graphics(444));
                            npc.setLastAnimation(new Animation(9254));
                            int jadMaxHp = jad.getMaxHp();
                            jad.heal((int)(jadMaxHp * 0.5));
                        }
                    }
                });
                Server.registerEvent(jadHealerEvent);
            }
        }
    }
}