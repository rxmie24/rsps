using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.combat
{
    internal class Combat
    {
        public Combat()
        {
        }

        public enum CombatType
        {
            MELEE,
            MAGE,
            RANGE
        }

        public static void newAttack(Entity killer, Entity target)
        {
            if (killer.getLastAttack() > 0)
            {
                /*if (Environment.TickCount - killer.getLastAttack() >= (killer.getAttackSpeed() * 500)) {
                    killer.setCombatTurns(killer.getAttackSpeed());
                }*/
            }
            else
            {
                killer.setCombatTurns(killer.getAttackSpeed());
            }
            killer.setEntityFocus(target.getClientIndex());
            killer.setTarget(target);
            checkAutoCast(killer, target);
            killer.setFaceLocation(target.getLocation());
        }

        private static void checkAutoCast(Entity killer, Entity target)
        {
            if (killer is Npc)
            {
                return;
            }
            if (((Player)killer).getTemporaryAttribute("autoCastSpell") != null)
            {
                int id = (int)((Player)killer).getTemporaryAttribute("autoCastSpell");
                bool ancients = (bool)(((Player)killer).getTemporaryAttribute("autoCastAncients") != null ? true : false);
                ((Player)killer).setTemporaryAttribute("autoCasting", true);
                MagicCombat.newMagicAttack((Player)killer, target, id, ancients);
            }
        }

        public static void combatLoop(Entity killer)
        {
            bool usingRange = killer is Player ? RangeCombat.isUsingRange(killer) : npcUsesRange(killer);
            Entity target = killer.getTarget();
            killer.incrementCombatTurns();
            bool autoCasting = killer is Npc ? false : ((Player)killer).getTemporaryAttribute("autoCasting") != null;
            bool dragonfire = false;
            bool guthanSpecial = false;

            //if you are auto casting you are not in combat loop.
            if (autoCasting)
                return;

            //If who you were attacking or who attacked you doesn't exist anymore. [Most important, should be first]
            if (target == null || (killer.getAttacker() == null && target == null))
            {
                //stop fighting.
                resetCombat(killer, 1);
                return;
            }

            //If it's the npc attacking and npc isn't owned by a player or player is attacking
            if ((killer is Npc) && ((Npc)killer).getOwner() == null || killer is Player)
            {
                if (killer.getLastAttacked() > 0 || killer.getLastAttack() > 0)
                {
                    //if the last time npc or player was attacked was 6 seconds ago or last time npc or player attacked was 6 seconds ago
                    if (isXSecondsSinceCombat(killer, killer.getLastAttacked(), 6000) && isXSecondsSinceCombat(killer, killer.getLastAttack(), 6000))
                    {
                        //stop fighting.
                        resetCombat(killer, 1);
                        return;
                    }
                }
            }

            //If you are a player and using range then your distance is 8 or if you are a npc using range get the npc's attackRange otherwise get the sie of the npc as distance.
            int distance = (killer is Player && usingRange) ? 8 : killer is Npc && usingRange ? getNpcAttackRange(killer) : getNPCSize(killer, target);
            //if you the player are not using range and you are attacking another player
            if (!usingRange && killer is Player && target is Player)
            {
                //if the player who is getting attacked is not standing still.
                if (((Player)target).getSprites().getPrimarySprite() != -1)
                {
                    //if you are using range on a player who is moving then distance to attack is 11, otherwise no range it's 3.
                    distance = usingRange ? 11 : 3;
                }
            }

            //If all[player vs player], [player vs npc] or [npc vs player] are within distance of each other.
            if (!killer.getLocation().withinDistance(target.getLocation(), distance) && !target.getLocation().withinDistance(killer.getLocation(), distance))
            {
                return;
            }

            //Can you [npc or player] even attack the entity
            if (!canAttack(killer, target, usingRange))
            {
                //stop fighting.
                resetCombat(killer, 0);
                return;
            }

            //are you [npc or player] using ranged attacks?
            if (usingRange)
            {
                //if you are a player
                if (killer is Player)
                {
                    //Do you have ammo and a bow?
                    if (RangeCombat.hasAmmo(killer) && RangeCombat.hasValidBowArrow(killer))
                    {
                        ((Player)killer).getWalkingQueue().resetWalkingQueue();
                        ((Player)killer).getPackets().closeInterfaces();
                        ((Player)killer).getPackets().clearMapFlag();
                    }
                    else
                    {
                        //You cannot attack the monster as you don't have ammo or a bow.
                        killer.setTarget(null);
                        return;
                    }
                }
            }

            //are you a player who is attacking a npc.
            if (target is Npc && killer is Player)
            {
                //If you are attacking Zilyana boss.
                if (((Npc)target).getId() == 6247)
                { // Zilyana (sara boss)
                    //TODO: Stop any walking err why only zilyana boss?, have to check this out later.
                    ((Player)killer).getWalkingQueue().resetWalkingQueue();
                    ((Player)killer).getPackets().clearMapFlag();
                }
            }

            //All the checks above are passed, below starts the actual assigning of target and doing the attack.

            //if you [npc or player] attacking turn is greater or equal to your attacking speed.
            if (killer.getCombatTurns() >= killer.getAttackSpeed())
            {
                //if [npc or player] has auto attack back on, and who they are attacking still exists.
                if (target.isAutoRetaliating() && target.getTarget() == null)
                {
                    //make the [npc or player] follow who they are trying to attack.
                    target.getFollow().setFollowing(killer);
                    //make the [npc or player] face up to their attacker.
                    target.setEntityFocus(killer.getClientIndex());

                    if ((target.getCombatTurns() >= (target.getAttackSpeed() / 2)) && target.getAttacker() == null)
                    {
                        target.setCombatTurns(target.getAttackSpeed() / 2);
                    }
                    //assign the [npc or player] who is getting attacked it's target who is attacking them.
                    target.setTarget(killer);
                    //if the person who is getting attacked is a player
                    if (target is Player)
                    {
                        //stop the movement of player who is getting attacked
                        ((Player)target).getWalkingQueue().resetWalkingQueue();
                        ((Player)target).getPackets().clearMapFlag();
                    }
                }
                //set the attack delay, if you are using range then delay is 2.4 seconds, otherwise magic.. 2.75 seconds.
                int delay = usingRange ? 2400 : 2750;
                //if delay has come up.
                if (Environment.TickCount - killer.getLastMagicAttack() < delay)
                {
                    //If the player who is attacking using range.
                    if (usingRange && killer is Player)
                    {
                        //Stop the movement of the attacker who is using ranged attacks.
                        ((Player)killer).getWalkingQueue().resetWalkingQueue();
                        ((Player)killer).getPackets().clearMapFlag();
                    }
                    return;
                }
                //if the attacker is a npc
                if (killer is Npc)
                {
                    //perform the npc attack as a killer on your target (most likely a player)
                    if (NPCAttack.npcAttack((Npc)killer, target))
                    {
                        return;
                        //if the dice 50/50 kicks in and the npc attacking is a dragon.
                    }
                    else if ((Misc.random(2) == 0) && isDragon(killer))
                    {
                        //do your dragon fire as a dragon npc.
                        doDragonfire(killer, target);
                        //dragonfire was done, variable used to stop some attack animation.
                        dragonfire = true;
                    }
                }
                //If the person getting attacked is a player.
                if (target is Player)
                {
                    //Close all your interfaces.
                    ((Player)target).getPackets().closeInterfaces();
                }
                //if the attacker [npc or player] has a attack animation and dragonfire variable wasn't set.
                if ((killer.getAttackAnimation() != 65535) && !dragonfire)
                {
                    //do your attack animation as a [npc or player].
                    killer.setLastAnimation(new Animation(killer.getAttackAnimation()));
                }
                //If the [player or npc] is using ranged attacks
                if (!usingRange)
                {
                    if (target.getCombatTurns() < 0 || target.getCombatTurns() > 0)
                    {
                        //if the [player or npc] getting attacked has a defensive animation.
                        if (target.getDefenceAnimation() != 65535)
                        {
                            //do a blocking/defensive animation.
                            target.setLastAnimation(new Animation(target.getDefenceAnimation()));
                        }
                    }
                }
                //make the attacker [player or npc] start following the attacked.
                killer.getFollow().setFollowing(target);
                //set a timer for the [player or npc] which indicates the last time they were attacked by killer.
                target.setLastAttacked(Environment.TickCount);
                //set a timer for the [player or npc] which indicates the last time they attacked the target.
                killer.setLastAttack(Environment.TickCount);
                //reset the combat turns. [this makes sure both attackers don't attack at same time]
                killer.resetCombatTurns();
                //assign the [npc or player] who is getting attacked it's target who is attacking them.
                target.setAttacker(killer);
                //set a skulls, the method checks if [player attacks player] also [checks if player is dueling or in fightpits or has skull], otherwise gets skull.
                setSkull(killer, target);
                //if the attacker is a player.
                if (killer is Player)
                {
                    //set attacking player's combatType to melee attack.
                    ((Player)killer).setLastCombatType(CombatType.MELEE);
                    //close all your interfaces as a attacker
                    ((Player)killer).getPackets().closeInterfaces();
                    //if you the attacking player is using a special attack.
                    if (((Player)killer).getSpecialAttack().isUsingSpecial())
                    {
                        //do your special attack on your target which may be a [player or npc]
                        if (((Player)killer).getSpecialAttack().doSpecialAttack(killer, target))
                        {
                            return;
                        }
                        //if you the attacking player is wearing guthan armour set.
                    }
                    else if (CombatFormula.wearingGuthan((Player)killer))
                    {
                        //roll a 25% dice.
                        if (Misc.random(4) == 0)
                        {
                            //if dice hits 25%, show some kind of graphics..
                            killer.setLastGraphics(new Graphics(398, 0));
                            //set some variable to indicate you are using guthan special.
                            guthanSpecial = true;
                        }
                    }
                }
                //if you the [player or npc] is using range.
                if (usingRange)
                {
                    //Go into the RangeCombat ranging logic loop processing class.
                    RangeCombat.rangeCombatLoop(killer, target);
                    return;
                }
                //if the dragon npc did his dragonfire attack quit right here.
                if (dragonfire)
                {
                    return;
                }
                //copies guthanSpecial variable to a different variable so it won't change when passed into a Event.
                bool guthanSpec = guthanSpecial;
                //get the damage you as the attacker [player or npc] will do on target [player or npc]
                double damage = getDamage(killer, target);
                //checks if damage will kill the player, sets a temporary variable 'willDie'
                checkIfWillDie(target, damage);
                //trigger the attack event based on the attackers [player or npc] hit delay
                Event attackEvent = new Event(killer.getHitDelay());
                attackEvent.setAction(() =>
                {
                    //stop attack event after this run
                    attackEvent.stop();
                    //add the XP for the killer [player only].
                    addXp(killer, target, damage);
                    //set the hit to be sent on the attacked target [player or npc]
                    target.hit((int)damage);
                    //if the attacker [player] is using the Smite prayer drain prayer from target [player] based on damage done
                    checkSmite(killer, target, damage);
                    //if the attack [pla
                    checkRecoil(killer, target, damage);
                    checkVengeance(killer, target, damage);
                    //if you are using the guthanSpecial which does some healing
                    if (guthanSpec)
                    {
                        // heals 30% of the damage, and an added random 70% of the damage
                        killer.heal((int)(damage * 0.30 + (Misc.randomDouble() * (damage * 0.70))));
                    }
                    //if the target you are attacking is a npc.
                    if (target is Npc)
                    {
                        //if it is Tzhaar monsters, you as the attacker will take 1 damage.
                        if (((Npc)target).getId() == 2736 || ((Npc)target).getId() == 2737)
                        { // Tzhaar lvl 45s
                            killer.hit(1); // their recoil attack
                        }
                    }
                });
                Server.registerEvent(attackEvent);
            }
        }

        private static bool isDragon(Entity killer)
        {
            int id = (((Npc)killer).getId());
            return id == 53 || id == 54 || id == 55 || id == 941 || id == 1590 || id == 1591 || id == 1692 || id == 5362 || id == 5364;
        }

        public static void doDragonfire(Entity killer, Entity target)
        {
            killer.setLastAnimation(new Animation(81));
            killer.setLastGraphics(new Graphics(1, 0, 50));

            Event doDragonFireEvent = new Event(600);
            doDragonFireEvent.setAction(() =>
            {
                doDragonFireEvent.stop();
                string message = null;
                int fireDamage = 55;
                bool wearingShield = ((Player)target).getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD) == 1540 || ((Player)target).getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD) == 11283 || ((Player)target).getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD) == 11284;
                if (((Player)target).getAntifireCycles() > 0)
                {
                    if (wearingShield)
                    {
                        message = "Your shield and potion combine to fully protect you from the dragon's breath.";
                        fireDamage = 0;
                    }
                    else
                    {
                        message = "Your antifire potion partially protects you from the dragon's breath.";
                        fireDamage = 20;
                    }
                }
                else if (wearingShield)
                {
                    message = "Your shield deflects some of the dragon's breath.";
                    fireDamage = 20;
                }
                else
                {
                    message = fireDamage == 0 ? "The dragon's breath has no effect, you got lucky this time!" : "The dragon's breath horribly burns you!";
                }
                ((Player)target).getPackets().sendMessage(message);
                target.hit(Misc.random(fireDamage));
            });
        }

        public static void checkIfWillDie(Entity target, double damage)
        {
            if (target is Npc)
            {
                return;
            }
            bool willDie = ((Player)target).getHp() - damage <= 0;
            if (willDie)
            {
                ((Player)target).setTemporaryAttribute("willDie", true);
            }
        }

        public static int getNPCSize(Entity killer, Entity target)
        {
            if (killer is Player && target is Player)
            {
                return 1;
            }
            if (target == null || killer == null)
            {
                return 1;
            }
            if (target is Player)
            {
                target = killer;
            }
            switch (((Npc)target).getId())
            {
                case 6247:
                case 6203:
                case 6208:
                case 6204:
                case 6223:
                case 6227:
                case 6225:
                    return 3;

                case 6222:
                    return 4;
            }
            return 1;
        }

        public static int getNpcAttackRange(Entity killer)
        {
            if (killer.getLocation().getX() >= 19000)
            {
                return ((Npc)killer).getMaximumCoords().getX() - ((Npc)killer).getMinimumCoords().getX();
            }
            return 15;
        }

        public static bool npcUsesRange(Entity killer)
        {
            if (killer is Player)
            {
                return false;
            }
            int id = ((Npc)killer).getId();
            switch (id)
            {
                case 2028:
                case 2025:
                case 6263:
                case 6265:
                case 6250:
                case 6208:
                case 6206:
                case 6222:
                case 6223:
                case 6225:
                case 2739:
                case 2740:
                case 2743:
                case 2744:
                case 2745:
                    return true;
            }
            return false;
        }

        public static bool isXSecondsSinceCombat(Entity entity, long timeSinceHit, int time)
        {
            return Environment.TickCount - timeSinceHit > time;
        }

        public static void checkRecoil(Entity killer, Entity target, double damage)
        {
            if (target is Npc)
            {
                return;
            }
            bool hasRecoil = (((Player)target).getEquipment().getItemInSlot(ItemData.EQUIP.RING) == 2550);
            if (hasRecoil)
            {
                //if the damage done on target is more then 10, then divide it by 10.
                //otherwise if damage is less then 10 and greater then 0 do a hit of 1 damage otherwise hit of 0 damage
                double hit = damage > 10 ? (damage / 10) : (damage <= 10 && damage > 0) ? 1 : 0;
                //no damage exit.
                if (hit == 0)
                {
                    return;
                }
                //the attacker [yourself] gets hit for the damage set above, as recoiled damage.
                killer.hit((int)hit);
                //the attacked who is wearing the recoil ring loses one 1 charge from his character.
                ((Player)target).setRecoilCharges((((Player)target).getRecoilCharges() - 1));
                //if the person has 0 or less recoil charges left
                if (((Player)target).getRecoilCharges() <= 0)
                {
                    //get the item in ring slot, could be any ring, possible to destory other rings if you lag it of course.
                    Item ringSlot = ((Player)target).getEquipment().getSlot(ItemData.EQUIP.RING);
                    //set the current ring in your ring slot to -1 [destoryed]
                    ringSlot.setItemId(-1);
                    //set amount of the ring you are wearing to 0, none.
                    ringSlot.setItemAmount(0);
                    //send the attacked a equipment update packet.
                    ((Player)target).getPackets().refreshEquipment();
                    //send the attacked player a message that he lost his ring.
                    ((Player)target).getPackets().sendMessage("Your Ring of recoil has shattered!");
                    //reset your recoil charges on your account back to 40.
                    ((Player)target).setRecoilCharges(40);
                }
            }
        }

        public static void checkSmite(Entity killer, Entity target, double damage)
        {
            if (killer is Npc || target is Npc || damage <= 0)
            {
                return;
            }
            if (((Player)target).getSkills().getCurLevel(Skills.SKILL.PRAYER) > 0 && !((Player)target).isDead())
            {
                if (((Player)killer).getPrayers().getHeadIcon() == PrayerData.SMITE)
                {
                    ((Player)target).getSkills().setCurLevel(Skills.SKILL.PRAYER, ((Player)target).getSkills().getCurLevel(Skills.SKILL.PRAYER) - (int)(damage / 4));
                    ((Player)target).getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
                }
            }
        }

        public static double getDamage(Entity killer, Entity target)
        {
            if (killer is Npc)
            {
                int maxDamage = killer.getMaxHit();
                if (maxDamage > target.getHp())
                {
                    maxDamage = target.getHp();
                }
                if (target is Player)
                {
                    NpcData npcDef = NpcData.forId(((Npc)killer).getId());
                    int npcAttackStyle = ((Npc)killer).getAttackType();

                    if (((Player)target).getPrayers().getHeadIcon() == PrayerData.MELEE && npcAttackStyle == NpcData.MELEE)
                    {
                        return 0;
                    }
                    else
                        if (Misc.random((int)CombatFormula.getNPCMeleeAttack((Npc)killer)) < Misc.random((int)CombatFormula.getMeleeDefence((Player)target, (Player)target)))
                        {
                            return 0;
                        }
                }
                return Misc.random(maxDamage);
            }
            else
            {
                if (target is Npc)
                {
                    if (Misc.random((int)CombatFormula.getMeleeAttack((Player)killer)) < Misc.random((int)CombatFormula.getNPCMeleeDefence((Npc)target)))
                    {
                        return 0;
                    }
                }
                double damage = CombatFormula.getMeleeHit((Player)killer, target);
                if (target is Player)
                {
                    if (((Player)target).getPrayers().getHeadIcon() == PrayerData.MELEE)
                    {
                        damage = (int)(damage * 0.60);
                    }
                }
                if (damage > target.getHp())
                {
                    damage = target.getHp();
                }
                return damage;
            }
        }

        public static void setSkull(Entity killer, Entity target)
        {
            if ((killer is Player) && (target is Player))
            {
                if (Location.inFightPits(killer.getLocation()))
                {
                    return;
                }
                if (((Player)killer).getDuel() == null)
                {
                    if ((killer.getLastKiller() == null || (!killer.getLastKiller().Equals(target)) && !((Player)killer).isSkulled()))
                    {
                        ((Player)killer).renewSkull();
                        target.setLastkiller(killer);
                    }
                }
            }
        }

        private static bool canAttack(Entity killer, Entity target, bool usingRange)
        {
            if (target.isDead() || killer.isDead() || target.isDestroyed() || killer.isDestroyed())
            {
                return false;
            }
            if (killer is Npc)
            {
                if (((Npc)killer).getId() < 2025 && ((Npc)killer).getId() > 2030)
                {
                    if (!target.getLocation().inArea(((Npc)killer).getMinimumCoords().getX(), ((Npc)killer).getMinimumCoords().getY(), ((Npc)killer).getMaximumCoords().getX(), ((Npc)killer).getMaximumCoords().getY()))
                    {
                        return false;
                    }
                }
            }
            if (target is Npc)
            {
                if (((Npc)target).getHp() <= 0)
                {
                    return false;
                }
                if (((Npc)target).getId() >= 6222 && ((Npc)target).getId() <= 6228)
                {
                    if (!usingRange && killer is Player)
                    {
                        ((Player)killer).getPackets().sendMessage("You are unable to reach the flying beast..");
                        return false;
                    }
                }
            }
            if ((target is Player) && (killer is Player))
            {
                if (Location.inFightPits(target.getLocation()) && Location.inFightPits(((Player)killer).getLocation()))
                {
                    if (!Server.getMinigames().getFightPits().hasGameStarted())
                    {
                        return false;
                    }
                    return true;
                }
                if (((Player)killer).getDuel() != null)
                {
                    if (((Player)target).getDuel() != null)
                    {
                        if (((Player)killer).getDuel().getPlayer2().Equals(((Player)target)) && ((Player)target).getDuel().getPlayer2().Equals(((Player)killer)))
                        {
                            if (((Player)killer).getDuel().ruleEnabled(DuelSession.RULE.NO_MELEE))
                            {
                                ((Player)killer).getPackets().sendMessage("Melee combat has been disabled in this duel!");
                                return false;
                            }
                            if (((Player)killer).getDuel().getStatus() == 6 && ((Player)target).getDuel().getStatus() == 6)
                            {
                                return true;
                            }
                        }
                    }
                    ((Player)killer).getPackets().sendMessage("That isn't your opponent.");
                    return false;
                }
                else
                    if (!Location.inWilderness(target.getLocation()))
                    {
                        ((Player)killer).getPackets().sendMessage("That player isn't in the wilderness.");
                        return false;
                    }
                    else
                        if (!Location.inWilderness(killer.getLocation()))
                        {
                            ((Player)killer).getPackets().sendMessage("You aren't in the wilderness.");
                            return false;
                        }
                int killerWildLevel = ((Player)killer).getLocation().wildernessLevel();
                int targetWildLevel = ((Player)target).getLocation().wildernessLevel();
                int killerCombatLevel = ((Player)killer).getSkills().getCombatLevel();
                int targetCombatLevel = ((Player)target).getSkills().getCombatLevel();
                int highest = killerCombatLevel > targetCombatLevel ? killerCombatLevel : targetCombatLevel;
                int lowest = highest == killerCombatLevel ? targetCombatLevel : killerCombatLevel;
                int difference = (highest - lowest);
                if (difference > killerWildLevel || difference > targetWildLevel)
                {
                    ((Player)killer).getPackets().sendMessage("You must move deeper into the wilderness to attack that player.");
                    return false;
                }
            }
            if (!Location.inMultiCombat(target.getLocation()))
            {
                if (killer.getAttacker() != null && !killer.getAttacker().Equals(target) && killer.getLastAttacked() > 0)
                {
                    if (killer is Player)
                    {
                        ((Player)killer).getPackets().sendMessage("You are already in combat!");
                    }
                    return false;
                }
                if (target.getAttacker() != null && !target.getAttacker().Equals(killer) && target.getLastAttacked() > 0)
                {
                    if (killer is Player)
                    {
                        string type = target is Player ? "player" : "npc";
                        ((Player)killer).getPackets().sendMessage("That " + type + " is already in combat.");
                    }
                    return false;
                }
            }
            return true;
        }

        public static void checkVengeance(Entity killer, Entity target, double damage)
        {
            if (target is Npc || damage <= 0 || ((target.getHp() - damage) <= 0))
            {
                return;
            }
            if (((Player)target).hasVengeance())
            {
                if (Environment.TickCount - ((Player)target).getLastVengeanceTime() <= 600)
                {
                    return;
                }
                int vengDamage = Misc.random((int)(damage * 0.75));
                ((Player)target).setForceText("Taste vengeance!");
                ((Player)target).setVengeance(false);
                killer.hit(vengDamage);
                ((Player)killer).setLastVengeanceTime(Environment.TickCount);
                return;
            }
            return;
        }

        public static void addXp(Entity killer, Entity target, double damage)
        {
            int xpRate = 230;
            if ((killer is Player) && (target is Npc))
            {
                Player p = (Player)killer;
                CombatType type = p.getLastCombatType();
                AttackStyle.CombatSkill fightType = p.getAttackStyle().getSkill();
                AttackStyle.CombatStyle fightStyle = p.getAttackStyle().getStyle();
                if (type == CombatType.MELEE)
                {
                    if (!fightType.Equals(AttackStyle.CombatSkill.CONTROLLED))
                    {
                        Skills.SKILL skill = Skills.SKILL.ATTACK;
                        if (fightType.Equals(AttackStyle.CombatSkill.ACCURATE))
                        {
                            skill = Skills.SKILL.ATTACK;
                        }
                        else if (fightType.Equals(AttackStyle.CombatSkill.DEFENSIVE))
                        {
                            skill = Skills.SKILL.DEFENCE;
                        }
                        else if (fightType.Equals(AttackStyle.CombatSkill.AGGRESSIVE))
                        {
                            skill = Skills.SKILL.STRENGTH;
                        }
                        p.getSkills().addXp(skill, (xpRate * damage));
                        p.getSkills().addXp(Skills.SKILL.HITPOINTS, (xpRate * 0.30));
                    }
                    else
                    {
                        p.getSkills().addXp(Skills.SKILL.ATTACK, ((xpRate * 0.30) * damage));
                        p.getSkills().addXp(Skills.SKILL.DEFENCE, ((xpRate * 0.30) * damage));
                        p.getSkills().addXp(Skills.SKILL.STRENGTH, ((xpRate * 0.30) * damage));
                        p.getSkills().addXp(Skills.SKILL.HITPOINTS, (0.25 * damage));
                    }
                }
                else
                {
                    if (fightStyle.Equals(AttackStyle.CombatStyle.RANGE_ACCURATE) || fightStyle.Equals(AttackStyle.CombatStyle.RANGE_RAPID))
                    {
                        p.getSkills().addXp(Skills.SKILL.RANGE, (xpRate * damage));
                    }
                    else if (fightStyle.Equals(AttackStyle.CombatStyle.RANGE_DEFENSIVE))
                    {
                        p.getSkills().addXp(Skills.SKILL.RANGE, ((xpRate * 0.50) * damage));
                        p.getSkills().addXp(Skills.SKILL.DEFENCE, ((xpRate * 0.50) * damage));
                    }
                    p.getSkills().addXp(Skills.SKILL.HITPOINTS, ((xpRate * 0.30) * damage));
                }
            }
            target.addToHitCount(killer, damage);
        }

        public static void resetCombat(Entity killer, int type)
        {
            if (killer != null)
            {
                killer.setEntityFocus(65535);
                killer.setTarget(null);
                killer.getFollow().setFollowing(null);
                if (type == 1)
                {
                    killer.setLastAttack(0);
                    killer.setLastAttacked(0);
                    if (killer.getAttacker() != null)
                    {
                        if (killer.getAttacker().getEntityFocus() != -1)
                        {
                            if (killer.getAttacker().getEntityFocus() == killer.getClientIndex())
                            {
                                killer.getAttacker().setEntityFocus(65535);
                            }
                        }
                        killer.setAttacker(null);
                    }
                }
            }
        }
    }
}