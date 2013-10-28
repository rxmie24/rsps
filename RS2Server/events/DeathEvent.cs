using RS2.Server.combat;
using RS2.Server.minigames.barrows;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.prayer;
using RS2.Server.player.skills.slayer;
using RS2.Server.util;
using System;

namespace RS2.Server.events
{
    internal class DeathEvent : Event
    {
        private Entity lastAttacker;
        private Entity entity;
        private bool firstNpcStage;

        private static string[] DEATH_MESSAGES = {
		    "You have defeated",
		    "Can anyone defeat you? Certainly not",
		    "You were clearly a better fighter than",
		    "It's all over for",
		    "With a crushing blow you finish",
		    "regrets the day they met you in combat",
		    "has won a free ticket to Lumbridge",
		    "was no match for you"
	    };

        public DeathEvent(Entity entity)
            : base(entity is Player ? 6000 : (((Npc)entity).getDeathTime()))
        {
            this.entity = entity;
            this.firstNpcStage = false;
            this.entity.setEntityFocus(65535);
            this.entity.setLastAnimation(new Animation(entity.getDeathAnimation(), 50));
            this.lastAttacker = entity.getAttacker() == null ? null : entity.getAttacker();
            entity.setPoisonAmount(0);
            if (entity.getFollow() != null)
            {
                entity.getFollow().setFollowing(null);
            }
            if (entity.getTarget() != null)
            {
                if (entity.getTarget().getAttacker() == null || entity.getTarget().getAttacker().Equals(entity))
                    Combat.resetCombat(entity.getTarget(), 1);
            }
            if (entity.getAttacker() != null)
            {
                if (entity.getAttacker().getTarget() == null || entity.getAttacker().getTarget().Equals(entity))
                    Combat.resetCombat(entity.getAttacker(), 1);
            }
            entity.setTarget(null);
            entity.setAttacker(null);
            if (entity.getKiller() != null)
            {
                if (entity is Npc)
                {
                    if (((Npc)this.entity).getId() >= 2025 && ((Npc)this.entity).getId() <= 2030)
                    {
                        Barrows.killBrother((Player)entity.getKiller(), ((Npc)this.entity).getId());
                    }
                    if (entity.getKiller() is Player)
                    {
                        Slayer.checkSlayerKill((Player)entity.getKiller(), (Npc)entity);
                    }
                }
            }
            if (entity is Player)
            {
                if (((Player)entity).getPrayers().getHeadIcon() == PrayerData.RETRIBUTION)
                {
                    doRedemption((Player)entity);
                }
                ((Player)entity).setDistanceEvent(null);
                ((Player)entity).getWalkingQueue().resetWalkingQueue();
                ((Player)entity).getPackets().clearMapFlag();
                ((Player)entity).removeTemporaryAttribute("autoCasting");
                if (((Player)entity).getDuel() == null)
                {
                    if (!Location.inFightPits(entity.getLocation()) && !Location.inFightCave(entity.getLocation()))
                    {
                        ((Player)entity).getPackets().sendMessage("Oh dear, you are dead!");
                    }
                    else
                    {
                        ((Player)entity).getPackets().sendMessage("You have been defeated!");
                    }
                }
                ((Player)entity).setTemporaryAttribute("unmovable", true);
                if (((Player)entity).getDuel() != null)
                {
                    ((Player)entity).getDuel().getPlayer2().setTemporaryAttribute("unmovable", true);
                }
                SkillHandler.resetAllSkills((Player)entity);
                if ((entity.getKiller() is Player))
                {
                    Player killer = (Player)entity.getKiller();
                    if (killer.getDuel() == null)
                    {
                        int id = Misc.random(DEATH_MESSAGES.Length - 1);
                        string deathMessage = DEATH_MESSAGES[id];
                        if (id <= 4)
                        {
                            killer.getPackets().sendMessage(deathMessage + " " + ((Player)entity).getLoginDetails().getUsername() + ".");
                        }
                        else
                        {
                            killer.getPackets().sendMessage(((Player)entity).getLoginDetails().getUsername() + " " + deathMessage + ".");
                        }
                    }
                    else
                    {
                        killer.setPoisonAmount(0);
                    }
                }
            }
        }

        public override void runAction()
        {
            if (entity is Npc)
            {
                if (!firstNpcStage)
                {
                    Combat.resetCombat(entity, 1);
                    entity.setHidden(true);
                    entity.dropLoot();
                    Npc n = (Npc)entity;
                    int respawnRate = n.respawnTime();
                    if (respawnRate == -1)
                    {
                        // TODO healers (2746)
                        Player killer = (Player)n.getOwner();
                        if (killer.getFightCave() != null)
                            killer.getFightCave().decreaseMobAmount(n.getId() == 2745);
                        Server.getNpcList().Remove(n);
                        this.stop();
                        return;
                    }
                    this.firstNpcStage = true;
                    this.setTick(n.respawnTime() * 500);
                }
                else
                {
                    this.stop();
                    entity.clearKillersHits();
                    entity.setHp(entity.getMaxHp());
                    entity.setDead(false);
                    entity.setHidden(false);
                    entity.setFrozen(false);
                }
            }
            else if (entity is Player)
            {
                this.stop();
                Player p = (Player)entity;
                if (Location.inFightPits(entity.getLocation()))
                {
                    Server.getMinigames().getFightPits().teleportToWaitingRoom(p, true);
                    return;
                }
                else if (Location.inFightCave(entity.getLocation()))
                {
                    p.getFightCave().teleFromCave(true);
                    return;
                }
                if (p.getDuel() == null)
                {
                    entity.dropLoot();
                    entity.clearKillersHits();
                    entity.setLastAttackType(1);
                    entity.setLastAttack(0);
                    entity.setTarget(null);
                    entity.setAttacker(null);
                    entity.teleport((Location)Constants.HOME_SPAWN_LOCATION.Clone());
                    if (p.getInventory().getProtectedItems() != null)
                    {
                        for (int i = 0; i < p.getInventory().getProtectedItems().Length; i++)
                        {
                            p.getInventory().addItem(p.getInventory().getProtectedItem(i));
                        }
                        p.getInventory().setProtectedItems(null);
                    }
                    p.setSkullCycles(0);
                    p.getSpecialAttack().resetSpecial();
                    p.getAttackStyle().setDefault();
                    p.getEquipment().setWeapon();
                    entity.setLastkiller(null);
                    Combat.resetCombat(entity, 1);
                    entity.setDead(false);
                    p.setLastVengeanceTime(0);
                    p.setVengeance(false);
                    p.setAntifireCycles(0);
                    p.setSuperAntipoisonCycles(0);
                    p.removeTemporaryAttribute("willDie");
                    p.setFrozen(false);
                    p.removeTemporaryAttribute("unmovable");
                    Prayer.deactivateAllPrayers(p);
                    p.setTeleblockTime(0);
                    p.removeTemporaryAttribute("teleblocked");
                    p.removeTemporaryAttribute("autoCastSpell");
                    foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                        p.getSkills().setCurLevel(skill, p.getSkills().getMaxLevel(skill));
                    p.getPackets().sendSkillLevels();
                }
                else
                {
                    p.getDuel().finishDuel(true, false);
                }
            }
        }

        private void doRedemption(Player p)
        {
            p.setLastGraphics(new Graphics(437));
            if (lastAttacker == null)
            {
                return;
            }
            if (lastAttacker.isDead() || lastAttacker.isHidden() || lastAttacker.isDestroyed())
            {
                return;
            }
            Location l = p.getLocation();
            int maxHit = (int)(p.getSkills().getMaxLevel(Skills.SKILL.PRAYER) * 0.25);
            if (lastAttacker.getLocation().inArea(l.getX() - 1, l.getY() - 1, l.getX() + 1, l.getY() + 1))
            {
                int damage = Misc.random(maxHit);
                if (damage > lastAttacker.getHp())
                {
                    damage = lastAttacker.getHp();
                }
                lastAttacker.hit(damage);
            }
            p.getSkills().setCurLevel(Skills.SKILL.PRAYER, 0);
            p.getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
        }
    }
}