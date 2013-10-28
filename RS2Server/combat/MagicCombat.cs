using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.magic;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.combat
{
    internal class MagicCombat : MagicData
    {
        public MagicCombat()
        {
        }

        public static void newMagicAttack(Player p, Entity target, int id, bool ancients)
        {
            int index = getSpellIndex(p, id, ancients);
            bool autoCasting = p.getTemporaryAttribute("autoCasting") != null;
            bool fakeNPC = target != null && target is Npc && ((Npc)target).getId() == 0;
            Entity lastAutocastEntity = null;
            bool frozen = false;
            if (index == -1)
            {
                return;
            }
            if (p.getTarget() == null)
            {
                if (autoCasting)
                {
                    if (Location.inMultiCombat(p.getLocation()))
                    {
                        lastAutocastEntity = (Entity)p.getTemporaryAttribute("autocastEntity") == null ? null : (Entity)p.getTemporaryAttribute("autocastEntity");
                        if (lastAutocastEntity == null || lastAutocastEntity is Player)
                        {
                            p.removeTemporaryAttribute("autoCasting");
                            Combat.resetCombat(p, 1);
                            return;
                        }
                        if (hitsMulti(p, index))
                        {
                            Location location = (Location)p.getTemporaryAttribute("autocastLocation");
                            Entity newTarget = new Npc(0);
                            newTarget.setLocation(location);
                            p.setTarget(newTarget);
                            newMagicAttack(p, newTarget, id, ancients);
                            return;
                        }
                    }
                    else
                    {
                        p.removeTemporaryAttribute("autoCasting");
                        Combat.resetCombat(p, 1);
                        return;
                    }
                }
                else
                {
                    p.removeTemporaryAttribute("autoCasting");
                    Combat.resetCombat(p, 1);
                    return;
                }
            }
            else
            {
                if (!canCastSpell(p, target, index, fakeNPC))
                {
                    p.removeTemporaryAttribute("autoCasting");
                    Combat.resetCombat(p, 1);
                    return;
                }
            }
            int distance = 8;
            if (target is Player)
            {
                if (((Player)target).getSprites().getPrimarySprite() != -1)
                {
                    distance = 8;
                }
            }
            if (!fakeNPC)
            { // we're actually attacking a real npc/player
                if (!p.getLocation().withinDistance(target.getLocation(), distance))
                {
                    p.getFollow().setFollowing(target);

                    Event attemptMagicAttackEvent = new Event(500);
                    int attemptMagicAttackCounter = 0;
                    attemptMagicAttackEvent.setAction(() =>
                    {
                        if (p.getLocation().withinDistance(target.getLocation(), distance) && p.getTarget() != null)
                        {
                            attemptMagicAttackEvent.stop();
                            newMagicAttack(p, target, id, ancients);
                            return;
                        }
                        attemptMagicAttackCounter++;
                        if (attemptMagicAttackCounter >= 12)
                        {
                            attemptMagicAttackEvent.stop();
                        }
                    });
                    Server.registerEvent(attemptMagicAttackEvent);
                    return;
                }
            }
            int timeSinceLastCast = autoCasting ? 3500 : 2000;
            if (Environment.TickCount - p.getLastMagicAttack() < timeSinceLastCast)
            {
                p.getWalkingQueue().resetWalkingQueue();
                //return;
            }
            int time = p.getLastCombatType().Equals(Combat.CombatType.MAGE) ? 1550 : 600;
            if (Environment.TickCount - p.getLastAttack() < time)
            {
                int delay = p.getLastCombatType().Equals(Combat.CombatType.MAGE) ? 1350 : 800;
                Event attemptMagicAttackEvent = new Event(500);
                attemptMagicAttackEvent.setAction(() =>
                {
                    if (Environment.TickCount - p.getLastAttack() > delay)
                    {
                        attemptMagicAttackEvent.stop();
                        newMagicAttack(p, target, id, ancients);
                    }
                });
                Server.registerEvent(attemptMagicAttackEvent);

                p.getWalkingQueue().resetWalkingQueue();
                p.getPackets().clearMapFlag();
                p.setLastCombatType(Combat.CombatType.MAGE);
                return;
            }
            if (fakeNPC && !monsterInArea(p, target))
            {
                p.removeTemporaryAttribute("autoCasting");
                Combat.resetCombat(p, 1);
                return;
            }
            int endGfx = END_GFX[index];
            double damage = Misc.random(CombatFormula.getMagicHit(p, target, getSpellMaxHit(p, index)));
            bool mp = false;
            bool magicProtect = mp;
            if (target is Player)
            {
                mp = ((Player)target).getPrayers().getHeadIcon() == PrayerData.MAGIC;
            }
            if (magicProtect)
            {
                damage *= 0.60;
            }
            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 8841)
            {
                damage *= 1.10; // void mace 10% hit increase.
            }
            if (damage == 0 && index != 41 && index != 42 && index != 43 && index != 44 && index != 45 && index != 46 && index != 47)
            {
                endGfx = 85;
            }
            if (!deleteRunes(p, RUNES[index], RUNE_AMOUNTS[index]))
            {
                p.setTarget(null);
                return;
            }
            p.getFollow().setFollowing(null);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            p.setFaceLocation(target.getLocation());
            if (HANDS_GFX[index] != -1)
            {
                p.setLastGraphics(new Graphics(HANDS_GFX[index], 0, getStartingGraphicHeight(index)));
            }
            p.setLastAnimation(new Animation(SPELL_ANIM[index]));
            p.getPackets().closeInterfaces();
            if (target is Player)
            {
                ((Player)target).getPackets().closeInterfaces();
            }
            target.setAttacker(p);
            p.setTarget(target);
            target.setLastAttacked(Environment.TickCount);
            p.setLastAttack(Environment.TickCount);
            p.setLastMagicAttack(Environment.TickCount);
            p.setCombatTurns(p.getAttackSpeed());
            Combat.setSkull(p, target);
            if (damage > 0)
            {
                frozen = freezeTarget(index, target);
                if (!frozen && index == 31)
                {
                    endGfx = 1677;
                }
            }
            if (AIR_GFX[index] != -1 || ((index == 31 || index == 27) && target is Player && ((Player)target).getWalkingQueue().isRunning()))
            {
                sendProjectile(index, target, p);
            }
            if (damage > target.getHp())
            {
                damage = target.getHp();
            }
            if (index == 47 && Misc.random(2) == 0)
            {
                endGfx = 85;
            }
            Combat.checkIfWillDie(target, damage);
            Event doMagicAttackEvent = new Event(getSpellHitDelay(index));
            doMagicAttackEvent.setAction(() =>
            {
                doMagicAttackEvent.stop();
                if (p == null || p.isDead() || !fakeNPC && (target.isDead() || target.isHidden() || target.isDestroyed()))
                {
                    return;
                }
                if (target.isAutoRetaliating() && target.getTarget() == null && damage > 0)
                {
                    if (target is Npc)
                    {
                    }
                    else
                    {
                        if (((Player)target).getTemporaryAttribute("autoCastSpell") != null)
                        {
                            int autoCastSpell = (int)((Player)target).getTemporaryAttribute("autoCastSpell");
                            ((Player)target).setTemporaryAttribute("autoCasting", true);
                            target.setTarget(p);
                            MagicCombat.newMagicAttack((Player)target, p, autoCastSpell, ((Player)target).getTemporaryAttribute("autoCastAncients") != null);
                        }
                    }
                    target.getFollow().setFollowing(p);
                    target.setEntityFocus(p.getClientIndex());
                    if ((target.getCombatTurns() <= (target.getAttackSpeed() / 2) || target.getCombatTurns() >= (target.getAttackSpeed())))
                    {
                        target.setCombatTurns(target.getAttackSpeed() / 2);
                    }
                    target.setTarget(p);
                    if (target is Player)
                    {
                        ((Player)target).getWalkingQueue().resetWalkingQueue();
                        ((Player)target).getPackets().clearMapFlag();
                    }
                }
                addMagicXp(p, target, damage, index, true);
                target.setLastGraphics(new Graphics(endGfx, 0, getGroundHeight(index, endGfx)));
                if (index == 47 && endGfx != 85)
                { // teleblock
                    if (target is Player)
                    {
                        teleblockPlayer(p, (Player)target);
                    }
                }
                if (damage != 0)
                {
                    Combat.checkRecoil(p, target, damage);
                    Combat.checkSmite(p, target, damage);
                    Combat.checkVengeance(p, target, damage);
                    hitInMulti(p, target, index);
                    applyMiasmicEffects(p, target, index);
                    if ((target.getCombatTurns() > 2 || target.getCombatTurns() < 0) && !target.isDead())
                    {
                        target.setLastAnimation(new Animation(target.getDefenceAnimation()));
                    }
                    if (index != 27)
                    {
                        target.hit((int)damage);
                        if (index == 18 || index == 22 || index == 26 || index == 30)
                        {
                            p.heal(Convert.ToInt32(damage / 4));
                        }
                    }
                    else if (index == 27)
                    {
                        Event doHitEvent = new Event(1000);
                        doHitEvent.setAction(() =>
                        {
                            doHitEvent.stop();
                            target.hit((int)damage);
                        });
                        Server.registerEvent(doHitEvent);
                    }
                }
            });
            Server.registerEvent(doMagicAttackEvent);
            if (p.getTemporaryAttribute("autoCasting") != null)
            {
                if (p.getTemporaryAttribute("autoCastSpell") != null)
                {
                    if (id != (int)p.getTemporaryAttribute("autoCastSpell"))
                    {
                        p.setTarget(null);
                        return;
                    }
                }
                if (!fakeNPC)
                {
                    p.setTemporaryAttribute("autocastLocation", target.getLocation());
                    p.setTemporaryAttribute("autocastEntity", target);
                }
                Event autoCastSpellEvent = new Event(3500);
                autoCastSpellEvent.setAction(() =>
                {
                    autoCastSpellEvent.stop();
                    if (p.getTemporaryAttribute("autoCasting") != null && p.getTemporaryAttribute("autoCastSpell") != null)
                    {
                        int autoCastSpell = (int)p.getTemporaryAttribute("autoCastSpell");
                        MagicCombat.newMagicAttack(p, p.getTarget(), autoCastSpell, p.getTemporaryAttribute("autoCastAncients") != null);
                    }
                });
                Server.registerEvent(autoCastSpellEvent);
            }
            else
            {
                p.setTarget(null);
            }
        }

        protected static void applyMiasmicEffects(Player p, Entity target, int index)
        {
            if (index < 48 || index > 51)
            {
                return;
            }
            if (target.getMiasmicEffect() == 0)
            {
                target.setMiasmicEffect(index - 47);
                if (target is Player)
                {
                    ((Player)target).getPackets().sendMessage("Your attack speed has been decreased!");
                }
                int delay = 0;
                switch (index)
                {
                    case 48: delay = 12000; break; // Miasmic rush.
                    case 49: delay = 24000; break;// Miasmic burst.
                    case 50: delay = 36000; break;// Miasmic blitz.
                    case 51: delay = 48000; break;// Miasmic barrage.
                }
                Event miasmicEffectRemoveEvent = new Event(delay);
                miasmicEffectRemoveEvent.setAction(() =>
                {
                    miasmicEffectRemoveEvent.stop();
                    target.setMiasmicEffect(0);
                });
                Server.registerEvent(miasmicEffectRemoveEvent);
            }
        }

        private static int getStartingGraphicHeight(int i)
        {
            return (i == 47 || i == 48 || i == 49 || i == 50 || i == 51) ? 0 : 100;
        }

        private static long getSpellHitDelay(int i)
        {
            return (i == 48 || i == 50 || i == 51) ? 1000 : 1700;
        }

        public static void teleblockPlayer(Player killer, Player target)
        {
            int teleblockDelay = 300000;
            if (target.getPrayers().getHeadIcon() == PrayerData.MAGIC)
            {
                teleblockDelay = 150000;
            }
            target.setTemporaryAttribute("teleblocked", true);
            target.setTeleblockTime(Environment.TickCount + teleblockDelay);
            Event removeTeleBlockEvent = new Event(teleblockDelay);
            removeTeleBlockEvent.setAction(() =>
            {
                removeTeleBlockEvent.stop();
                if (target != null)
                {
                    target.removeTemporaryAttribute("teleblocked");
                    target.setTeleblockTime(0);
                }
            });
            Server.registerEvent(removeTeleBlockEvent);
        }

        /*
         * Used to check whether we should stop autocasting.
         */

        public static bool monsterInArea(Player p, Entity mainTarget)
        {
            Location l = mainTarget.getLocation();
            foreach (Npc n in Server.getNpcList())
            {
                if (n == null || n.Equals(mainTarget) || n.isDead() || n.isHidden() || n.isDestroyed())
                {
                    continue;
                }
                if (n.getLocation().inArea(l.getX() - 1, l.getY() - 1, l.getX() + 1, l.getY() + 1))
                {
                    return true;
                }
            }
            foreach (Player target in Server.getPlayerList())
            {
                if (mainTarget == null || target.Equals(mainTarget) || target.isDead() || target.isHidden() || target.isDestroyed())
                {
                    continue;
                }
                if (target.getLocation().inArea(l.getX() - 1, l.getY() - 1, l.getX() + 1, l.getY() + 1))
                {
                    return true;
                }
            }
            return false;
        }

        public static void hitInMulti(Player p, Entity mainTarget, int index)
        {
            if (!Location.inMultiCombat(p.getLocation()) || !Location.inMultiCombat(mainTarget.getLocation()))
            {
                return;
            }
            else if (!hitsMulti(p, index))
            {
                return;
            }
            Location l = mainTarget.getLocation();
            double totalDamage = 0;
            if (mainTarget is Npc)
            {
                foreach (Npc n in Server.getNpcList())
                {
                    if (n == null || n.Equals(mainTarget) || n.isDead() || n.isHidden() || n.isDestroyed())
                    {
                        continue;
                    }
                    if (n.getLocation().inArea(l.getX() - 1, l.getY() - 1, l.getX() + 1, l.getY() + 1))
                    {
                        if (!canCastSpell2(p, mainTarget, index, false))
                        {
                            continue;
                        }
                        if (n.isAutoRetaliating() && n.getTarget() == null)
                        {
                            //n.getFollow().setFollowing(killer);
                            n.setEntityFocus(p.getClientIndex());
                            n.setCombatTurns(n.getAttackSpeed() / 2);
                            n.setTarget(p);
                        }
                        int damage = Misc.random(Misc.random(getSpellMaxHit(p, index))); //double randomize? uhh wtf?
                        int graphic = END_GFX[index];
                        if (damage == 0)
                        {
                            graphic = 85;
                        }
                        if (damage > n.getHp())
                        {
                            damage = n.getHp();
                        }
                        if (damage > 0)
                        {
                            bool frozen = freezeTarget(index, n);
                            if (!frozen && index == 31)
                            {
                                graphic = 1677;
                            }
                        }
                        totalDamage += damage;
                        n.setAttacker(p);
                        n.setLastAttacked(Environment.TickCount);
                        n.setLastGraphics(new Graphics(graphic, 0, getGroundHeight(index, graphic)));
                        n.hit(damage);
                        if ((n.getCombatTurns() > 2 || n.getCombatTurns() < 0))
                        {
                            n.setLastAnimation(new Animation(n.getDefenceAnimation()));
                        }
                        addDamage(p, n, damage);
                    }
                }
            }
            else
            {
                foreach (Player target in Server.getPlayerList())
                {
                    if (mainTarget == null || target.Equals(mainTarget) || target.isDead() || target.isHidden() || target.isDestroyed())
                    {
                        continue;
                    }
                    if (target.getLocation().inArea(l.getX() - 1, l.getY() - 1, l.getX() + 1, l.getY() + 1))
                    {
                        if (!canCastSpell2(p, mainTarget, index, false))
                        {
                            continue;
                        }
                        if (target.isAutoRetaliating() && target.getTarget() == null)
                        {
                            //n.getFollow().setFollowing(killer);
                            target.setEntityFocus(p.getClientIndex());
                            target.setCombatTurns(target.getAttackSpeed() / 2);
                            target.setTarget(p);
                        }
                        int damage = Misc.random(Misc.random(getSpellMaxHit(p, index))); //double randomize? uhh wtf?
                        int graphic = END_GFX[index];
                        if (damage == 0)
                        {
                            graphic = 85;
                        }
                        if (damage > target.getHp())
                        {
                            damage = target.getHp();
                        }
                        if (damage > 0)
                        {
                            bool frozen = freezeTarget(index, target);
                            if (!frozen && index == 31)
                            {
                                graphic = 1677;
                            }
                        }
                        totalDamage += damage;
                        target.setAttacker(p);
                        target.setLastAttacked(Environment.TickCount);
                        target.setLastGraphics(new Graphics(graphic, 0, getGroundHeight(index, graphic)));
                        target.hit(damage);
                        if ((target.getCombatTurns() > 2 || target.getCombatTurns() < 0))
                        {
                            target.setLastAnimation(new Animation(target.getDefenceAnimation()));
                        }
                        addDamage(p, target, damage);
                    }
                }
            }
            if (totalDamage > 0)
            {
                addMagicXp(p, null, totalDamage, index, false);
                if (index == 18 || index == 22 || index == 26 || index == 30)
                {
                    p.heal((int)(totalDamage * 0.25));
                }
            }
        }

        public static bool freezeTarget(int i, Entity target)
        {
            if ((i != 34 && i != 36 && i != 40 && i != 19 && i != 23 && i != 27 && i != 31) || target.isFrozen())
            {
                return false;
            }
            int delay = getFreezeDelay(i);
            target.setFrozen(true);
            if (target is Player)
            {
                ((Player)target).getPackets().sendMessage("You have been frozen!");
                ((Player)target).getWalkingQueue().resetWalkingQueue();
            }
            Event unFreezeEvent = new Event(delay);
            unFreezeEvent.setAction(() =>
            {
                unFreezeEvent.stop();
                target.setFrozen(false);
            });
            Server.registerEvent(unFreezeEvent);
            return true;
        }

        public static int getFreezeDelay(int spellToCast)
        {
            switch (spellToCast)
            {
                case 34: return 5000;
                case 36: return 10000;
                case 40: return 15000;
                case 19: return 5000;
                case 23: return 10000;
                case 27: return 15000;
                case 31: return 20000;
            }
            return 0;
        }

        public static void castCharge(Player p)
        {
            p.removeTemporaryAttribute("autoCasting");
            if (p.getSkills().getCurLevel(Skills.SKILL.MAGIC) < 80)
            {
                p.getPackets().sendMessage("You need a Magic level of 80 to cast Charge.");
                return;
            }
            if (!hasRunes(p, CHARGE_RUNES, CHARGE_RUNE_AMOUNT))
            {
                p.getPackets().sendMessage("You do not have enough runes to cast Charge.");
                return;
            }
            if (p.getTemporaryAttribute("godCharged") != null)
            {
                p.getPackets().sendMessage("You have already charged your god spells.");
                return;
            }
            if (hasGodCapeAndStaff(p))
            {
                p.getPackets().sendMessage("You must wear a God cape and wield the matching staff to cast Charge.");
                return;
            }
            if (!deleteRunes(p, CHARGE_RUNES, CHARGE_RUNE_AMOUNT))
            {
                return;
            }
            p.setTemporaryAttribute("godCharged", true);
            p.setLastGraphics(new Graphics(308, 800, 90));
            p.setLastAnimation(new Animation(811));
            p.getPackets().sendMessage("You feel charged with magical power..");
            int delay = 60000 + Misc.random(120000); //60 seconds + possible 120 seconds so, 1 minute to 2 minutes.
            Event removeGodChargeEvent = new Event(delay);
            removeGodChargeEvent.setAction(() =>
            {
                removeGodChargeEvent.stop();
                p.removeTemporaryAttribute("godCharged");
                p.getPackets().sendMessage("Your magical charge fades away.");
            });
            Server.registerEvent(removeGodChargeEvent);
        }

        private static bool hasGodCapeAndStaff(Player p)
        {
            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 2415 && p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) == 2412)
            {
                return true;
            }
            if ((p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 2416 || p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 8841) && p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) == 2413)
            {
                return true;
            }
            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 2417 && p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) == 2414)
            {
                return true;
            }
            return false;
        }

        private static void sendProjectile(int index, Entity target, Player player)
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p.getLocation().withinDistance(player.getLocation(), 60))
                {
                    if (index != 31 && index != 27)
                    { // ice barrage + ice blitz
                        p.getPackets().sendProjectile(player.getLocation(), target.getLocation(), getStartingSpeed(index), AIR_GFX[index], 50, getProjectileHeight(index), getProjectileEndHeight(index), 100, target);
                    }
                    else
                    {
                        p.getPackets().sendProjectile(player.getLocation(), target.getLocation(), getStartingSpeed(index), 368, 50, getProjectileHeight(index), getProjectileEndHeight(index), 100, target);
                    }
                }
            }
        }

        private static bool canCastSpell(Player p, Entity target, int i, bool fakeNPC)
        {
            // fakeNPC is used to keep location when autocasting.
            if (fakeNPC)
            {
                return !p.isDead();
            }
            if (target.isDead() || p.isDead() || target.isDestroyed() || p.isDestroyed())
            {
                return false;
            }
            if (target is Npc)
            {
                if (((Npc)target).getHp() <= 0)
                {
                    return false;
                }
                if (i == 47)
                {
                    p.getPackets().sendMessage("You cannot cast Teleblock upon an NPC.");
                    return false;
                }
            }
            if ((target is Player) && (p is Player))
            {
                if (Location.inFightPits(target.getLocation()) && Location.inFightPits(target.getLocation()))
                {
                    if (!Server.getMinigames().getFightPits().hasGameStarted())
                    {
                        return false;
                    }
                    return true;
                }
                if (p.getDuel() != null)
                {
                    if (((Player)target).getDuel() != null)
                    {
                        if (p.getDuel().getPlayer2().Equals(((Player)target)) && ((Player)target).getDuel().getPlayer2().Equals(p))
                        {
                            if (p.getDuel().ruleEnabled(DuelSession.RULE.NO_MAGIC))
                            {
                                p.getPackets().sendMessage("Magical combat has been disabled in this duel!");
                                return false;
                            }
                            if (p.getDuel().getStatus() == 6 && ((Player)target).getDuel().getStatus() == 6)
                            {
                                return true;
                            }
                        }
                    }
                    p.getPackets().sendMessage("That isn't your opponent.");
                    return false;
                }
                if (i == 47)
                {
                    if (((Player)target).getTemporaryAttribute("teleblocked") != null)
                    {
                        p.getPackets().sendMessage("That player already has a teleportation block upon them.");
                        return false;
                    }
                }
                if (!Location.inWilderness(target.getLocation()))
                {
                    p.getPackets().sendMessage("That player isn't in the wilderness.");
                    return false;
                }
                if (!Location.inWilderness(p.getLocation()))
                {
                    p.getPackets().sendMessage("You aren't in the wilderness.");
                    return false;
                }
                int killerWildLevel = p.getLocation().wildernessLevel();
                int targetWildLevel = ((Player)target).getLocation().wildernessLevel();
                int killerCombatLevel = p.getSkills().getCombatLevel();
                int targetCombatLevel = ((Player)target).getSkills().getCombatLevel();
                int highest = killerCombatLevel > targetCombatLevel ? killerCombatLevel : targetCombatLevel;
                int lowest = highest == killerCombatLevel ? targetCombatLevel : killerCombatLevel;
                int difference = (highest - lowest);
                if (difference > killerWildLevel || difference > targetWildLevel)
                {
                    ((Player)p).getPackets().sendMessage("You must move deeper into the wilderness to attack that player.");
                    return false;
                }
            }
            if (!Location.inMultiCombat(target.getLocation()))
            {
                if (p.getAttacker() != null && !p.getAttacker().Equals(target))
                {
                    p.getPackets().sendMessage("You are already in combat!");
                    return false;
                }
                if (target.getAttacker() != null && !target.getAttacker().Equals(p))
                {
                    string type = target is Player ? "player" : "npc";
                    p.getPackets().sendMessage("That " + type + " is already in combat.");
                    return false;
                }
            }
            if (p.getSkills().getCurLevel(Skills.SKILL.MAGIC) < SPELL_LEVEL[i])
            {
                p.getPackets().sendMessage("You need a Magic level of " + SPELL_LEVEL[i] + " to cast that spell.");
                return false;
            }
            if (!hasRunes(p, RUNES[i], RUNE_AMOUNTS[i]))
            {
                p.getPackets().sendMessage("You do not have enough runes to cast that spell.");
                return false;
            }
            if (NEEDS_STAFF[i])
            {
                if ((i != 38 && p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) != STAFF[i]) || (i == 38 && p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) != 8841 && p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) != STAFF[i]))
                {
                    p.getPackets().sendMessage("You need to wield " + STAFF_NAME[i] + " to cast this spell.");
                    return false;
                }
            }
            if (i == 37)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) != 2412)
                {
                    p.getPackets().sendMessage("You need to wear the Cape of Saradomin to be able to cast Saradomin Strike.");
                    return false;
                }
            }
            if (i == 38)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) != 2413)
                {
                    p.getPackets().sendMessage("You need to wear the Cape of Guthix to be able to cast Claws of Guthix.");
                    return false;
                }
            }
            if (i == 39)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.CAPE) != 2414)
                {
                    p.getPackets().sendMessage("You need to wear the Cape of Zamorak to be able to cast Flames of Zamorak.");
                    return false;
                }
            }
            return true;
        }

        private static bool canCastSpell2(Player p, Entity target, int i, bool fakeNPC)
        {
            // fakeNPC is used to keep location when autocasting.
            if (fakeNPC)
            {
                return !p.isDead();
            }
            if (target.isDead() || p.isDead() || target.isDestroyed() || p.isDestroyed())
            {
                return false;
            }
            if (target is Npc)
            {
                if (((Npc)target).getHp() <= 0)
                {
                    return false;
                }
            }
            if ((target is Player) && (p is Player))
            {
                if (Location.inFightPits(target.getLocation()) && Location.inFightPits(target.getLocation()))
                {
                    if (!Server.getMinigames().getFightPits().hasGameStarted())
                    {
                        return false;
                    }
                    return true;
                }
                if (p.getDuel() != null)
                {
                    if (((Player)target).getDuel() != null)
                    {
                        if (p.getDuel().getPlayer2().Equals(((Player)target)) && ((Player)target).getDuel().getPlayer2().Equals(p))
                        {
                            if (p.getDuel().ruleEnabled(DuelSession.RULE.NO_MAGIC))
                            {
                                return false;
                            }
                            if (p.getDuel().getStatus() == 6 && ((Player)target).getDuel().getStatus() == 6)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                if (!Location.inWilderness(target.getLocation()) && !Location.inWilderness(p.getLocation()))
                    return false;

                int killerWildLevel = p.getLocation().wildernessLevel();
                int targetWildLevel = ((Player)target).getLocation().wildernessLevel();
                int killerCombatLevel = p.getSkills().getCombatLevel();
                int targetCombatLevel = ((Player)target).getSkills().getCombatLevel();
                int highest = killerCombatLevel > targetCombatLevel ? killerCombatLevel : targetCombatLevel;
                int lowest = highest == killerCombatLevel ? targetCombatLevel : killerCombatLevel;
                int difference = (highest - lowest);
                if (difference > killerWildLevel || difference > targetWildLevel)
                {
                    return false;
                }
            }
            return true;
        }

        public static int spellEffect(Entity target, int i, int k)
        {
            if (target is Npc)
            {
                return 0;
            }
            int affectedLevel = ((Player)target).getSkills().getCurLevel((Skills.SKILL)i);
            return affectedLevel - affectedLevel % k;
        }

        public static int getSpellIndex(Player p, int id, bool ancients)
        {
            if (!ancients)
            {
                if (p.getMagicType() != 1)
                {
                    return -1;
                }
                switch (id)
                {
                    case 1: return 0; // Wind strike.
                    case 4: return 1; // Water strike
                    case 6: return 2; // Earth strike.
                    case 8: return 3; // Fire strike.
                    case 10: return 4; // Wind bolt.
                    case 14: return 5; // Water bolt.
                    case 17: return 6; // Earth bolt.
                    case 20: return 7; // Fire bolt.
                    case 24: return 8; // Wind blast.
                    case 27: return 9; // Water blast.
                    case 33: return 10; // Earth blast.
                    case 38: return 11; // Fire blast.
                    case 45: return 12; // Wind wave.
                    case 48: return 13; // Water wave.
                    case 52: return 14; // Earth wave.
                    case 55: return 15; // Fire wave.
                    case 22: return 32; // Crumble undead.
                    case 31: return 33; // Slayer dart.
                    case 12: return 34; // Bind.
                    case 29: return 35; // Iban blast.
                    case 30: return 36; // Snare.
                    case 41: return 37; // Saradomin strike.
                    case 42: return 38; // Claws of Guthix.
                    case 43: return 39; // Flames of Zamorak.
                    case 56: return 40; // Entangle.
                    case 2: return 41; // Confuse.
                    case 7: return 42; // Weaken.
                    case 11: return 43; // Curse.
                    case 53: return 44; // Enfeeble.
                    case 57: return 45; // Stun.
                    case 50: return 46; // Vulnerability.
                    case 63: return 47; // Teleblock.
                }
            }
            else
            {
                if (p.getMagicType() != 2)
                {
                    return -1;
                }
                switch (id)
                {
                    case 8: return 16; // Smoke rush.
                    case 12: return 17; // Shadow rush.
                    case 4: return 18; // Blood rush.
                    case 0: return 19; // Ice rush.
                    case 10: return 20; // Smoke burst.
                    case 14: return 21; // Shadow burst.
                    case 6: return 22; // Blood burst.
                    case 2: return 23; // Ice burst.
                    case 9: return 24; // Smoke blitz.
                    case 13: return 25; // Shadow blitz.
                    case 5: return 26; // Blood blitz.
                    case 1: return 27; // Ice blitz.
                    case 11: return 28; // Smoke barrage.
                    case 15: return 29; // Shadow barrage.
                    case 7: return 30; // Blood barrage.
                    case 3: return 31; // Ice barrage.
                    case 16: return 48; // Miasmic rush.
                    case 18: return 49; // Miasmic burst.
                    case 17: return 50; // Miasmic blitz.
                    case 19: return 51; // Miasmic barrage.
                }
            }
            return -1;
        }

        private static int getProjectileEndHeight(int i)
        {
            if (i == 18)
            {
                return 15;
            }
            if (i == 26 || i == 19 || i == 45 || i == 50)
            {
                return 0;
            }
            return 31;
        }

        protected static int getGroundHeight(int i, int endGfx)
        {
            if (endGfx == 85 || endGfx == 1677)
            {
                return 100;
            }
            return (i == 18 || i == 19 || i == 21 || i == 22 || i == 23 || i == 25 || i == 26 || i == 27 || i == 29 || i == 30 || i == 31 || i == 39 || i == 47 || i == 48 || i == 49 || i == 50 || i == 51) ? 0 : 100;
        }

        private static int getStartingSpeed(int i)
        {
            if (i == 32)
            {
                return 66;
            }
            if (i == 34 || i == 36 || i == 40)
            {
                return 89;
            }
            if (i == 33)
            {
                return 62;
            }
            if (i == 16 || i == 25)
            {
                return 66;
            }
            if (i == 17)
            {
                return 69;
            }
            if (i == 18)
            {
                return 68;
            }
            if (i == 47)
            {
                return 70;
            }
            return (i == 12 || i == 13 || i == 14 || i == 15 || i == 30 || i == 31 || i == 39) ? 32 : 64;
        }

        private static int getProjectileHeight(int i)
        {
            if (i == 18)
            {
                return 10;
            }
            if (i == 19 || i == 26 || i == 31)
            {
                return 0;
            }
            return i == 26 ? 20 : 45;
        }

        private static int getSpellMaxHit(Player p, int index)
        {
            switch (index)
            {
                case 0: return 2;
                case 1: return 4;
                case 2: return 6;
                case 3: return 8;
                case 4: return 9;
                case 5: return 10;
                case 6: return 11;
                case 7: return 12;
                case 8: return 13;
                case 9: return 14;
                case 10: return 15;
                case 11: return 16;
                case 12: return 17;
                case 13: return 18;
                case 14: return 19;
                case 15: return 20;
                case 16: return 13;
                case 17: return 14;
                case 18: return 15;
                case 19: return 16;
                case 20: return 17;
                case 21: return 17;
                case 22: return 21;
                case 23: return 22;
                case 24: return 23;
                case 25: return 24;
                case 26: return 25;
                case 27: return 26;
                case 28: return 27;
                case 29: return 28;
                case 30: return 29;
                case 31: return 30;
                case 32: return 15;
                case 33: return 19;
                case 34: return 3;
                case 35: return 25;
                case 36: return 3;
                case 37:
                case 38:
                case 39: if (p.getTemporaryAttribute("godCharged") != null && hasGodCapeAndStaff(p)) { return 30; } return 20;
                case 40: return 3;
                case 41: return 0;
                case 42: return 0;
                case 43: return 0;
                case 44: return 0;
                case 45: return 0;
                case 46: return 0;
                case 47: return 0;
                case 48: return 18;
                case 49: return 24;
                case 50: return 28;
                case 51: return 35;
            }
            return 0;
        }

        protected static void addMagicXp(Player p, Entity target, double hit, int index, bool baseXp)
        {
            if (target is Npc)
            {
                double xp = 0;
                switch (index)
                {
                    case 0: xp = 5.5; break; // Wind strike.
                    case 1: xp = 7.5; break;// Water strike
                    case 2: xp = 9.5; break;// Earth strike.
                    case 3: xp = 11.5; break;// Fire strike.
                    case 4: xp = 13.5; break;// Wind bolt.
                    case 5: xp = 16.5; break;// Water bolt.
                    case 6: xp = 19.5; break;// Earth bolt.
                    case 7: xp = 21.5; break;// Fire bolt.
                    case 8: xp = 25.5; break;// Wind blast.
                    case 9: xp = 28.5; break;// Water blast.
                    case 10: xp = 31.5; break;// Earth blast.
                    case 11: xp = 34.5; break;// Fire blast.
                    case 12: xp = 36.0; break;// Wind wave.
                    case 13: xp = 37.5; break;// Water wave.
                    case 14: xp = 40.0; break;// Earth wave.
                    case 15: xp = 42.5; break;// Fire wave.
                    case 32: xp = 24.5; break;// Crumble undead.
                    case 33: xp = 30.0; break;// Slayer dart.
                    case 34: xp = 30.0; break;// Bind.
                    case 35: xp = 30.0; break;// Iban blast.
                    case 36: xp = 60.0; break;// Snare.
                    case 37: xp = 61.0; break;// Saradomin strike.
                    case 38: xp = 61.0; break;// Claws of Guthix.
                    case 39: xp = 61.0; break;// Flames of Zamorak.
                    case 40: xp = 89.0; break;// Entangle.
                    case 41: xp = 13.0; break;// Confuse.
                    case 42: xp = 21.0; break;// Weaken.
                    case 43: xp = 29.0; break;// Curse.
                    case 44: xp = 83.0; break;// Enfeeble.
                    case 45: xp = 90.0; break;// Stun.
                    case 46: xp = 76.0; break;// Vulnerability.
                    case 47: xp = 80.0; break;// Teleblock.
                    case 16: xp = 30.0; break;// Smoke rush.
                    case 17: xp = 31.0; break;// Shadow rush.
                    case 18: xp = 33.0; break;// Blood rush.
                    case 19: xp = 34.0; break;// Ice rush.
                    case 20: xp = 36.0; break;// Smoke burst.
                    case 21: xp = 37.0; break;// Shadow burst.
                    case 22: xp = 39.0; break;// Blood burst.
                    case 23: xp = 40.0; break;// Ice burst.
                    case 24: xp = 42.0; break;// Smoke blitz.
                    case 25: xp = 43.0; break;// Shadow blitz.
                    case 26: xp = 45.0; break;// Blood blitz.
                    case 27: xp = 46.0; break;// Ice blitz.
                    case 28: xp = 48.0; break;// Smoke barrage.
                    case 29: xp = 48.0; break;// Shadow barrage.
                    case 30: xp = 51.0; break;// Blood barrage.
                    case 31: xp = 52.0; break;// Ice barrage.
                    case 48: xp = 36.0; break;// Miasmic rush.
                    case 49: xp = 42.0; break;// Miasmic burst.
                    case 50: xp = 48.0; break;// Miasmic blitz.
                    case 51: xp = 54.0; break;// Miasmic barrage.
                }
                double finalXp = baseXp ? (xp + (hit * 2)) : (hit * 2);
                p.getSkills().addXp(Skills.SKILL.MAGIC, finalXp);
                p.getSkills().addXp(Skills.SKILL.HITPOINTS, hit * 1.33);
                target.addToHitCount(p, hit);
            }
            else if (target != null)
            {
                target.addToHitCount(p, hit);
            }
        }

        public static void addDamage(Entity killer, Entity target, int damage)
        {
            if (!target.isDead())
            {
                target.addToHitCount(killer, damage);
            }
        }

        protected static bool hitsMulti(Player p, int index)
        {
            if (!Location.inMultiCombat(p.getLocation()))
            {
                return false;
            }
            switch (index)
            {
                case 20: return true; // Smoke burst.
                case 21: return true; // Shadow burst.
                case 22: return true; // Blood burst.
                case 23: return true; // Ice burst.
                case 28: return true; // Smoke barrage.
                case 29: return true; // Shadow barrage.
                case 30: return true; // Blood barrage.
                case 31: return true; // Ice barrage.
                case 49: return true; // Miasmic burst.
                case 51: return true; // Miasmic barrage.
            }
            return false;
        }
    }
}