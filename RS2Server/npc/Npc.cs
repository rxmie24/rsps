using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.warriorguild;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;

namespace RS2.Server.npc
{
    public enum FaceDirection
    {
        NORTH = 1,
        EAST = 4,
        SOUTH = 6,
        WEST = 3
    };

    public enum WalkType
    {
        STATIC,
        RANGE
    };

    internal class Npc : Entity
    {
        private int id;
        private Player owner;
        private AppearanceUpdateFlags updateFlags;
        private FaceDirection faceDirection = FaceDirection.NORTH;
        private WalkType walkType;
        private NpcData npcDef;
        private NpcSkills skills; //npc skills
        private Location minimumCoords;
        private Location maximumCoords;

        public Npc(int id)
        {
            this.id = id;
            npcDef = NpcData.forId(id);
            skills = new NpcSkills(this);
            skills.setMaxLevel(NpcSkills.SKILL.HITPOINTS, npcDef.getHitpoints()); //this must be first.
            skills.setCurLevel(NpcSkills.SKILL.HITPOINTS, npcDef.getHitpoints());
            this.setWalkType(WalkType.RANGE);
            this.faceDirection = FaceDirection.NORTH;
            this.updateFlags = new AppearanceUpdateFlags(this);
        }

        public Npc(int id, Location location)
            : this(id)
        {
            setLocation(location);
        }

        public void tick()
        {
            getSprites().setSprites(-1, -1);
            int sprite = -1;
            if (inCombat())
            {
                Combat.combatLoop(this);
            }
            if (getFollow().getFollowing() != null && !isFrozen())
            {
                getFollow().followEntity();
                return;
            }
            if (minimumCoords == null || maximumCoords == null) return; //cannot process walking
            if (Misc.randomDouble() > 0.8 && walkType == WalkType.RANGE && !inCombat() && !isDead() && !isFrozen())
            {
                int tgtX = getLocation().getX() + Misc.random(-1, 2); //random number from -1,0,1
                int tgtY = getLocation().getY() + Misc.random(-1, 2); //random number from -1,0,1
                sprite = Misc.direction(getLocation().getX(), getLocation().getY(), tgtX, tgtY);
                if (tgtX > maximumCoords.getX() || tgtX < minimumCoords.getX() || tgtY > maximumCoords.getY() || tgtY < minimumCoords.getY())
                {
                    sprite = -1;
                }
                if (sprite != -1)
                {
                    sprite >>= 1;
                    faceDirection = (FaceDirection)sprite;
                    getSprites().setSprites(sprite, -1);
                    setLocation(new Location(tgtX, tgtY, getLocation().getZ()));
                }
                return;
            }
        }

        public int getId()
        {
            return id;
        }

        public NpcData getDefinition()
        {
            return npcDef;
        }

        /**
	    * @param walkType the walkType to set
	    */

        public void setWalkType(WalkType walkType)
        {
            this.walkType = walkType;
        }

        /**
         * @return the walkType
         */

        public WalkType getWalkType()
        {
            return walkType;
        }

        public void setOwner(Player p)
        {
            this.owner = p;
        }

        public Player getOwner()
        {
            return owner;
        }

        /**
         * @param minimumCoords the minimumCoords to set
         */

        public void setMinimumCoords(Location minimumCoords)
        {
            if (minimumCoords == null)
                this.minimumCoords = new Location(0, 0, 0);
            else
                this.minimumCoords = minimumCoords;
        }

        /**
         * @return the minimumCoords
         */

        public Location getMinimumCoords()
        {
            return minimumCoords;
        }

        /**
         * @param maximumCoords the maximumCoords to set
         */

        public void setMaximumCoords(Location maximumCoords)
        {
            if (maximumCoords == null)
                this.maximumCoords = new Location(0, 0, 0);
            else
                this.maximumCoords = maximumCoords;
        }

        /**
         * @return the maximumCoords
         */

        public Location getMaximumCoords()
        {
            return maximumCoords;
        }

        public AppearanceUpdateFlags getUpdateFlags()
        {
            return updateFlags;
        }

        public void setFaceDirection(FaceDirection faceDirection)
        {
            this.faceDirection = faceDirection;
        }

        public int getFaceDirection()
        {
            return Convert.ToInt32(faceDirection);
        }

        public override void setHp(int newHp)
        {
            skills.setCurLevel(NpcSkills.SKILL.HITPOINTS, newHp);
        }

        public override int getHp()
        {
            return skills.getCurLevel(NpcSkills.SKILL.HITPOINTS);
        }

        public override int getMaxHp()
        {
            return skills.getMaxLevel(NpcSkills.SKILL.HITPOINTS);
        }

        public override bool isAutoRetaliating()
        {
            return skills.getCurLevel(NpcSkills.SKILL.HITPOINTS) > 0;
        }

        public override int getDefenceAnimation()
        {
            return npcDef.getDefenceAnimation();
        }

        public override int getDeathAnimation()
        {
            return npcDef.getDeathAnimation();
        }

        public override int getHitDelay()
        {
            return Animations.getNPCHitDelay(this);
        }

        public override int getAttackAnimation()
        {
            return npcDef.getAttackAnimation();
        }

        public override int getAttackSpeed()
        {
            if (getMiasmicEffect() > 0)
            {
                return npcDef.getAttackSpeed() * 2;
            }
            return npcDef.getAttackSpeed();
        }

        public override void hit(int damage)
        {
            if (isDead())
            {
                damage = 0;
            }
            hit(damage, damage <= 0 ? Hits.HitType.NO_DAMAGE : Hits.HitType.NORMAL_DAMAGE);
        }

        public override void hit(int damage, Hits.HitType type)
        {
            bool damageOverZero = damage > 0;
            if (damage > skills.getCurLevel(NpcSkills.SKILL.HITPOINTS))
            {
                damage = skills.getCurLevel(NpcSkills.SKILL.HITPOINTS);
            }
            if (damageOverZero && damage == 0)
            {
                type = Hits.HitType.NO_DAMAGE;
            }
            if (!updateFlags.isHitUpdateRequired())
            {
                getHits().setHit1(new Hits.Hit(damage, type));
                updateFlags.setHitUpdateRequired(true);
            }
            else
            {
                if (!updateFlags.isHit2UpdateRequired())
                {
                    getHits().setHit2(new Hits.Hit(damage, type));
                    updateFlags.setHit2UpdateRequired(true);
                }
                else
                {
                    lock (queuedHits)
                    {
                        queuedHits.Enqueue(new Hits.Hit(damage, type));
                    }
                }
            }
            skills.setCurLevel(NpcSkills.SKILL.HITPOINTS, skills.getCurLevel(NpcSkills.SKILL.HITPOINTS) - damage);
            if (skills.getCurLevel(NpcSkills.SKILL.HITPOINTS) == 0)
            {
                if (!isDead())
                {
                    Server.registerEvent(new DeathEvent(this));
                    setDead(true);
                }
            }
        }

        public override void heal(int healAmount)
        {
            skills.setCurLevel(NpcSkills.SKILL.HITPOINTS, skills.getCurLevel(NpcSkills.SKILL.HITPOINTS) + healAmount);
        }

        public NpcSkills getNpcSkills()
        {
            return skills;
        }

        public override int getMaxHit()
        {
            return npcDef.getMaxHit();
        }

        public int getAttackType()
        {
            return npcDef.getAttackType();
        }

        public int respawnTime()
        {
            return npcDef.getRespawn();
        }

        public int getDeathTime()
        {
            int id = this.id;
            if (id == 6203)
            {
                return 4000;
            }
            else if (id >= 2734 && id <= 2745)
            { // Fight cave monsters
                return 3300;
            }
            else if (id >= 4278 && id <= 4284)
            { // animated armours
                return 2500;
            }
            return 5000;
        }

        public override void dropLoot()
        {
            Entity killer = this.getKiller();
            Player p = killer is Player ? (Player)killer : null;
            NpcDrop drop = this.npcDef.getDrop();
            if (killer == null || p == null)
            {
                return;
            }
            if (drop != null)
            {
                try
                {
                    List<Item> drops = new List<Item>();
                    int random = Misc.random(100);
                    int random2 = 100 - random;
                    if (random2 == 0)
                    {
                        random2++;
                    }
                    if (random2 < 25)
                    { // 25% - semi rare
                        if (drop.getUncommonDrops() != null && drop.getUncommonDrops().Count > 0)
                            drops.Add(drop.getUncommonDrops()[Misc.random(drop.getUncommonDrops().Count - 1)]);
                    }
                    else if (random2 >= 25 && random2 < 95)
                    { // 65%  common
                        if (drop.getCommonDrops() != null && drop.getCommonDrops().Count > 0)
                            drops.Add(drop.getCommonDrops()[Misc.random(drop.getCommonDrops().Count - 1)]);
                    }
                    else if (random2 >= 95)
                    { // 5% - rare
                        if (drop.getRareDrops() != null && drop.getRareDrops().Count > 0)
                            drops.Add(drop.getRareDrops()[Misc.random(drop.getRareDrops().Count - 1)]);
                    }
                    random = random2;
                    if (drop.getAlwaysDrops().Count != 0)
                    {
                        foreach (Item d in drop.getAlwaysDrops())
                        {
                            drops.Add(d);
                        }
                    }
                    foreach (Item randomItem in drops)
                    {
                        int amount = randomItem.getItemAmount();
                        int itemId = randomItem.getItemId();
                        if (amount < 0)
                        {
                            amount = Misc.random((amount - (amount * 2)));
                            if (amount == 0)
                            {
                                amount = 1;
                            }
                        }
                        if (itemId == 8844)
                        { // defender
                            itemId = WarriorGuildData.DEFENDERS[((Player)killer).getDefenderWave()];
                        }
                        bool stackable = ItemData.forId(itemId).isNoted() || ItemData.forId(itemId).isStackable();
                        if (stackable || (!stackable && amount == 1))
                        {
                            if (Server.getGroundItems().addToStack(itemId, amount, this.getLocation(), p))
                            {
                            }
                            else
                            {
                                GroundItem item = new GroundItem(itemId, amount, this.getLocation(), p);
                                Server.getGroundItems().newEntityDrop(item);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < amount; i++)
                            {
                                GroundItem item = new GroundItem(itemId, 1, this.getLocation(), p);
                                Server.getGroundItems().newEntityDrop(item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Misc.WriteError("Error at npc dropLoot, msg=" + e.Message);
                }
            }
        }

        public override bool isDestroyed()
        {
            return !Server.getNpcList().Contains(this);
        }
    }
}