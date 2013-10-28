using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.combat
{
    internal class RangeCombat
    {
        /*
	     * Current range weapons
         * --------------------------------------------------
	     * Normal longbows + shortbows
	     * Knives
	     * Darts
	     * Thrownaxes
	     * Dorgeshuun c'bow
	     * Crossbows
	     * Dark bow
	     * Morrigans javelin + thrownaxe
	     * Seercull
	     * All Crystal bows
	     * All Karils X'bows
	     * Obsidian ring
         * ---------------------------------------------------
	     * TODO javelins & poisoned variations of weapons.
	     */

        public enum RangeType
        {
            ACCURATE,
            RAPID,
            LONGRANGE,
        }

        private static int[] RANGE_WEAPONS = {
		    839, 841, 843, 845, 847, 849, 851, 853, 855, 857, 859, 861,
		    863, 864, 865, 866, 867, 868, 869, 800, 801, 802, 803, 804,
		    805, 806, 807, 808, 809, 810, 811, 8880, 9174, 9176, 9177,
		    9179, 9181, 9183, 9185, 11235, 13879, 13883, 6724, 4212, 4214,
		    4215, 4216, 4217, 4218, 4219, 4220, 4221, 4222, 4223, 4734,
		    4934, 4935, 4936, 4937, 4938, 6522
	    };

        private static int[] NORMAL_BOWS = { 839, 841, 843, 845, 847, 849, 851, 853, 855, 857, 859, 861 };
        private static int[] ARROWS = { 882, 884, 886, 888, 890, 892 };
        private static int[] ARROW_DB_GFX = { 18, 19, 20, 21, 22, 24 };
        private static int[] DOUBLE_ARROW_DB_GFX = { 1104, 1105, 1105, 1107, 1108, 1109 };
        private static int[] ARROW_PROJ_GFX = { 10, 11, 12, 13, 14, 15 };
        private static int[] CROSSBOWS = { 9174, 9176, 9177, 9179, 9181, 9183, 9185 };
        private static int[] BOLTS = { 9335, 9336, 9337, 9338, 9339, 9340, 9341, 9342, 9236, 9237, 9238, 9239, 9240, 9241, 9242, 9243, 9244, 9245 };
        private static int[] TIPPED_BOLTS = { 9335, 9336, 9337, 9338, 9339, 9340, 9341, 9342 };
        private static int[] ENCHANTED_BOLTS = { 9236, 9237, 9238, 9239, 9240, 9241, 9242, 9243, 9244, 9245 };
        private static int[] BOLT_PROJ_GFX = { 318, 319, 320, 321, 322, 323, 324, 325 };
        private static int[] KARIL_BOWS = { 4734, 4928, 4929, 4930, 4931 };
        private static int BOLT_RACK = 4740;
        private static int OBBY_RING = 6522;
        private static int DARK_BOW = 11235;
        private static int DRAGON_ARROW = 11212;

        private enum BOW_TYPE
        {
            NOT_BOW,
            NORMAL_BOW,
            CROSSBOW,
            KARIL_BOW,
            CRYSTAL_BOW,
            OBBY_RING,
            DARK_BOW
        };

        public RangeCombat()
        {
        }

        public static void rangeCombatLoop(Entity killer, Entity target)
        {
            if (!hasValidBowArrow(killer))
            {
                killer.setTarget(null);
                return;
            }
            int hitDelay = getHitDelay(killer, target);
            if (killer is Player)
            {
                ((Player)killer).getWalkingQueue().resetWalkingQueue();
                ((Player)killer).getPackets().closeInterfaces();
                ((Player)killer).getPackets().clearMapFlag();
                ((Player)killer).setLastCombatType(Combat.CombatType.RANGE);
            }
            int drawback = getDrawbackGraphic(killer);
            if (drawback != -1)
            {
                killer.setLastGraphics(new Graphics(drawback, 0, 90));
            }
            displayProjectile(killer, target);
            deductArrow(killer);
            int arrowType = getArrowType(killer);
            int usingBow = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            int usingArrows = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
            int damage1 = Misc.random((int)getDamage((Player)killer, target, usingBow, usingArrows));
            int damage2 = usingBow == DARK_BOW ? Misc.random((int)getDamage((Player)killer, target, usingBow, usingArrows)) : 0;

            /*
             * If the damage of the first attack will kill the target.
             * Make sure the target doesn't die from that first attack.
             * But instead split the attack into two attacks.
             */
            if (damage1 >= target.getHp())
            {
                int randomHp = Misc.random(target.getHp());
                damage1 = randomHp;
                damage2 = target.getHp() - randomHp;
            }
            else
            {
                int hpRemaining = target.getHp() - damage1;
                if (damage2 > hpRemaining)
                {
                    damage2 = hpRemaining;
                }
            }
            int totalDamage = damage1 + damage2;

            Combat.checkIfWillDie(target, (damage1 + damage2));
            Event doRangedAttackEvent = new Event(hitDelay);
            doRangedAttackEvent.setAction(() =>
            {
                int damage = damage1;
                if (getBowType(killer) == BOW_TYPE.CROSSBOW)
                {
                    damage = applyBoltGraphic((Player)killer, target, damage1, arrowType);
                    totalDamage = damage + damage2;
                }
                if ((target.getCombatTurns() > 2 || target.getCombatTurns() < 0) && !target.isDead())
                {
                    target.setLastAnimation(new Animation(target.getDefenceAnimation()));
                }
                target.hit(damage);
                Combat.addXp(killer, target, totalDamage);
                Combat.checkRecoil(killer, target, totalDamage);
                Combat.checkSmite(killer, target, totalDamage);
                Combat.checkVengeance(killer, target, totalDamage);
                if (killer is Player && arrowType != -1 && arrowType != BOLT_RACK)
                {
                    createGroundArrow(killer, target, arrowType);
                }
                doRangedAttackEvent.stop();
            });
            Server.registerEvent(doRangedAttackEvent);
            //If the bow you are using is a darkbow do a second attack and animation.
            if (getBowType(killer) == BOW_TYPE.DARK_BOW)
            {
                deductArrow(killer);
                Event displayProjectileEvent = new Event(200);
                displayProjectileEvent.setAction(() =>
                {
                    displayProjectile(killer, target);
                    displayProjectileEvent.stop();
                });
                Server.registerEvent(displayProjectileEvent);

                Event shootArrowEvent = new Event(hitDelay + 400);
                shootArrowEvent.setAction(() =>
                {
                    target.hit(damage2);
                    if (killer is Player && arrowType != -1 && arrowType != BOLT_RACK)
                    {
                        createGroundArrow(killer, target, arrowType);
                    }
                    shootArrowEvent.stop();
                });
                Server.registerEvent(shootArrowEvent);
            }
        }

        private static double getDamage(Player killer, Entity target, int usingBow, int usingArrows)
        {
            double damage = CombatFormula.getRangeHit(killer, target, usingBow, usingArrows);
            if (target is Player)
            {
                int prayerType = ((Player)target).getPrayers().getHeadIcon();
                if (prayerType == PrayerData.RANGE)
                {
                    return damage * 0.60;
                }
                else
                {
                    return damage;
                }
            }
            return damage;
        }

        private static int applyBoltGraphic(Player killer, Entity target, int damage, int bolt)
        {
            int hit = Misc.random(10);
            if (hit != 0 || getBowType(killer) != BOW_TYPE.CROSSBOW)
            {
                return damage;
            }
            double maxDamage = getDamage((Player)killer, target, ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON), bolt);
            switch (bolt)
            {
                case 9236: // Opal.
                    target.setLastGraphics(new Graphics(749));
                    maxDamage *= 1.25;
                    break;

                case 9237: // Jade.
                    target.setLastGraphics(new Graphics(756));
                    //TODO Falling emote
                    break;

                case 9238: // Pearl.
                    target.setLastGraphics(new Graphics(750));
                    break;

                case 9239: // Topaz.
                    target.setLastGraphics(new Graphics(757, 0, 0));
                    if (target is Player)
                    {
                        int magicLevel = ((Player)target).getSkills().getCurLevel(Skills.SKILL.MAGIC);
                        if (magicLevel == 1)
                            return (int)maxDamage;
                        int magicLevelDeduction = Misc.random(1, 10);
                        magicLevelDeduction = Math.Min(magicLevelDeduction, (magicLevel - 1));
                        string s = magicLevelDeduction == 1 ? "" : "s";
                        ((Player)target).getSkills().setCurLevel(Skills.SKILL.MAGIC, magicLevel - magicLevelDeduction);
                        ((Player)target).getPackets().sendSkillLevel(Skills.SKILL.MAGIC);
                        ((Player)target).getPackets().sendMessage("Your Magic level has been reduced by " + magicLevelDeduction + " level" + s + " !");
                    }
                    break;

                case 9240: // Sapphire.
                    target.setLastGraphics(new Graphics(751));
                    if (target is Player)
                    {
                        int prayerLevel = ((Player)target).getSkills().getCurLevel(Skills.SKILL.PRAYER);
                        if (prayerLevel == 1)
                            return (int)maxDamage;
                        int prayerLevelDeduction = Misc.random(1, 10);
                        prayerLevelDeduction = Math.Min(prayerLevelDeduction, (prayerLevel - 1));
                        string s = prayerLevelDeduction == 1 ? "" : "s";
                        ((Player)target).getSkills().setCurLevel(Skills.SKILL.PRAYER, prayerLevel - prayerLevelDeduction);
                        ((Player)target).getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
                        ((Player)target).getPackets().sendMessage("Your Prayer level has been lowered by " + prayerLevelDeduction + " level" + s + " !");
                        killer.getPackets().sendMessage("You steal " + prayerLevelDeduction + " Prayer point" + s + " from your opponent!");

                        int prayerLevelObtained = Math.Min(killer.getSkills().getCurLevel(Skills.SKILL.PRAYER) + prayerLevelDeduction, killer.getSkills().getMaxLevel(Skills.SKILL.PRAYER));
                        killer.getSkills().setCurLevel(Skills.SKILL.PRAYER, prayerLevelObtained);
                        killer.getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
                    }
                    break;

                case 9241: // Emerald.
                    if (!target.isPoisoned())
                    {
                        Server.registerEvent(new PoisonEvent(target, 6));
                        target.setLastGraphics(new Graphics(752));
                    }
                    break;

                case 9242: // Ruby
                    target.setLastGraphics(new Graphics(754));
                    int currentHP = killer.getHp();
                    bool has11Percent = (currentHP * 0.11) < currentHP;
                    int removeFromOpponent = (int)(target.getHp() * 0.20); //20% off opponents HP.
                    if (has11Percent)
                    {
                        killer.hit((int)(currentHP * 0.10));
                        target.hit(removeFromOpponent);
                        killer.getPackets().sendMessage("You sacrifice some of your own health to deal more damage to your opponent!");
                    }
                    break;

                case 9243: // Diamond.
                    target.setLastGraphics(new Graphics(758));
                    maxDamage *= 1.15;
                    //TODO this affects opponents range defence for X minutes.
                    break;

                case 9244: // Dragon.
                    bool hitsFire = true;
                    if (target is Player)
                    {
                        int shield = ((Player)target).getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD);
                        /*
                         * Opponent has anti-fire shield.
                         */
                        if (shield == 11283 || shield == 1540)
                        {
                            hitsFire = false;
                        }
                    }
                    else
                    {
                        int id = ((Npc)target).getId();
                        /*
                         * NPC is a dragon
                         */
                        if ((id >= 50 && id <= 55) || id == 941 || (id >= 1589 && id <= 1592) || id == 2642 || id == 3376
                            || id == 3588 || id == 3590 || (id >= 4665 && id <= 4684) || id == 5178 || id == 5362 || id == 5363)
                        {
                            hitsFire = false;
                        }
                    }
                    if (hitsFire)
                    {
                        target.setLastGraphics(new Graphics(756));
                        maxDamage *= 1.45; //increase damage by 145%.
                    }
                    break;

                case 9245: // Onyx.
                    target.setLastGraphics(new Graphics(753));
                    maxDamage *= 1.15;
                    killer.heal(Misc.random((int)(maxDamage * 0.60)));
                    break;
            }
            damage = Misc.random((int)maxDamage);
            if (Misc.random(2) == 0 && bolt != 9242)
            {
                damage = (int)((maxDamage * 0.50) + Misc.random((int)(maxDamage * 0.50)));
            }
            if (damage > target.getHp())
            {
                damage = target.getHp();
            }
            return damage;
        }

        public static int getArrowType(Entity killer)
        {
            int arrow = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
            BOW_TYPE bowType = getBowType(killer);
            if (bowType == BOW_TYPE.CRYSTAL_BOW || bowType == BOW_TYPE.OBBY_RING) //these have no arrows.
                return -1;
            return arrow;
        }

        public static int getHitDelay(Entity killer, Entity target)
        {
            int distance = killer.getLocation().distanceToPoint(target.getLocation());
            int[] DELAYS1 = { 700, 700, 800, 1050, 1050, 1100, 1200, 1200, 1300, 1300 };
            int[] DELAYS2 = { 450, 450, 400, 600, 600, 650, 700, 750, 800, 800 };
            int[] DELAYS = getBowType(killer) == BOW_TYPE.OBBY_RING ? DELAYS2 : DELAYS1;
            if (distance > 9)
            {
                return 1000;
            }
            return DELAYS[distance];
        }

        public static void createGroundArrow(Entity killer, Entity target, int arrow)
        {
            if (Misc.random(1) == 1)
            {
                return;
            }
            GroundItem i = new GroundItem(arrow, 1, target.getLocation(), ((Player)killer));
            if (Server.getGroundItems().addToStack(arrow, 1, target.getLocation(), ((Player)killer)))
            {
                return;
            }
            else
            {
                Server.getGroundItems().newEntityDrop(i);
            }
        }

        public static bool hasAmmo(Entity killer)
        {
            if (killer is Npc)
            {
                return true;
            }
            bool hasAmmo = false;
            int ammo = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
            int weapon = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            for (int i = 0; i < NORMAL_BOWS.Length; i++)
            {
                if (weapon == NORMAL_BOWS[i] || weapon == DARK_BOW)
                {
                    for (int j = 0; j < ARROWS.Length; j++)
                    {
                        ammo = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
                        if (ammo == ARROWS[j])
                        {
                            hasAmmo = true;
                        }
                        if (weapon == DARK_BOW)
                        {
                            if (((Player)killer).getEquipment().getAmountInSlot(ItemData.EQUIP.ARROWS) < 2)
                            {
                                hasAmmo = false;
                                ((Player)killer).getPackets().sendMessage("You need atleast 2 arrows to use the Dark bow.");
                                return false;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < CROSSBOWS.Length; i++)
            {
                if (weapon == CROSSBOWS[i])
                {
                    for (int j = 0; j < BOLTS.Length; j++)
                    {
                        ammo = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
                        if (ammo == BOLTS[j])
                        {
                            hasAmmo = true;
                        }
                    }
                }
            }
            for (int i = 0; i < KARIL_BOWS.Length; i++)
            {
                if (weapon == KARIL_BOWS[i])
                {
                    if (ammo == BOLT_RACK)
                    {
                        hasAmmo = true;
                    }
                }
            }
            if (weapon >= 4214 && weapon <= 4223)
            { // Crystal bows full to 1/10.
                hasAmmo = true;
            }
            else
                if (weapon == OBBY_RING)
                {
                    hasAmmo = true;
                }
                else
                    if (weapon == DARK_BOW)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == DRAGON_ARROW)
                        {
                            hasAmmo = true;
                        }
                    }
            if (!hasAmmo)
            {
                ((Player)killer).getPackets().sendMessage("You have no ammo in your quiver!");
                killer.setTarget(null);
            }
            return hasAmmo;
        }

        public static void deductArrow(Entity killer)
        {
            if (killer is Npc)
            {
                return;
            }
            int weapon = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            if (weapon >= 4214 && weapon <= 4223)
            {
                degradeCrystalBow(killer);
                return;
            }
            int amount;
            if (weapon == OBBY_RING)
            {
                amount = ((Player)killer).getEquipment().getAmountInSlot(ItemData.EQUIP.WEAPON);
                ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.WEAPON).setItemAmount(amount - 1);
                if (((Player)killer).getEquipment().getAmountInSlot(ItemData.EQUIP.WEAPON) <= 0)
                {
                    ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.WEAPON).setItemId(-1);
                    ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.WEAPON).setItemAmount(0);
                    ((Player)killer).getPackets().sendMessage("You have run out of Toktz-xil-ul!");
                    killer.setTarget(null);
                    ((Player)killer).getEquipment().unequipItem(ItemData.EQUIP.WEAPON);
                }
                ((Player)killer).getPackets().refreshEquipment();
                return;
            }
            amount = ((Player)killer).getEquipment().getAmountInSlot(ItemData.EQUIP.ARROWS);
            ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.ARROWS).setItemAmount(amount - 1);
            if (((Player)killer).getEquipment().getAmountInSlot(ItemData.EQUIP.ARROWS) <= 0)
            {
                ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.ARROWS).setItemId(-1);
                ((Player)killer).getEquipment().getSlot(ItemData.EQUIP.ARROWS).setItemAmount(0);
                ((Player)killer).getPackets().sendMessage("You have run out of ammo!");
                killer.setTarget(null);
            }
            ((Player)killer).getPackets().refreshEquipment();
        }

        private static void displayProjectile(Entity killer, Entity target)
        {
            int distance = killer.getLocation().distanceToPoint(target.getLocation());
            int[] speed1 = { 60, 60, 60, 63, 65, 67, 69, 69, 71, 73, 73, 73, 73, 73 };
            int[] speed2 = { 35, 35, 35, 38, 41, 45, 47, 50, 53, 73, 73, 73, 73, 73 };
            int[] speed = getBowType(killer) == BOW_TYPE.OBBY_RING ? speed2 : speed1;
            int finalSpeed = getBowType(killer) == BOW_TYPE.DARK_BOW ? speed[distance] + 10 : speed[distance];
            foreach (Player p in Server.getPlayerList())
            {
                if (p.getLocation().withinDistance(killer.getLocation(), 60))
                {
                    p.getPackets().sendProjectile(killer.getLocation(), target.getLocation(), getStartingSpeed(killer), getProjectileGfx(killer), 50, 31, distance < 2 ? 34 : 39, finalSpeed, target);
                }
            }
        }

        public static void displayDBSpecProjectile(Entity killer, Entity target)
        {
            int distance = killer.getLocation().distanceToPoint(target.getLocation());
            if (distance >= 10)
            {
                return;
            }
            int[] speed = { 50, 57, 64, 73, 75, 77, 79, 79, 81, 83, 84, 84, 84 };
            int finalSpeed = speed[distance] + 10;
            foreach (Player p in Server.getPlayerList())
            {
                if (p.getLocation().withinDistance(killer.getLocation(), 60))
                {
                    p.getPackets().sendProjectile(killer.getLocation(), target.getLocation(), getStartingSpeed(killer), 1099, 50, 31, distance < 2 ? 34 : 39, finalSpeed, target);
                }
            }
        }

        public static void displayMSpecProjectile(Entity killer, Entity target)
        {
            int distance = killer.getLocation().distanceToPoint(target.getLocation());
            int[] speed = { 25, 25, 30, 33, 37, 39, 40, 41, 43, 46 };
            int finalSpeed = speed[distance] + 5;
            foreach (Player p in Server.getPlayerList())
            {
                if (p.getLocation().withinDistance(killer.getLocation(), 60))
                {
                    p.getPackets().sendProjectile(killer.getLocation(), target.getLocation(), 29, 249, 50, 36, 40, finalSpeed, target);
                }
            }
        }

        private static int getStartingSpeed(Entity killer)
        {
            if (killer is Player)
            {
                BOW_TYPE bowType = getBowType(killer);
                if (bowType == BOW_TYPE.NORMAL_BOW)
                {
                    return 50;
                }
                else if (bowType == BOW_TYPE.CROSSBOW)
                {
                    return 50;
                }
                else if (bowType == BOW_TYPE.KARIL_BOW)
                {
                    return 50;
                }
                else if (bowType == BOW_TYPE.CRYSTAL_BOW)
                {
                    return 50;
                }
                else if (bowType == BOW_TYPE.OBBY_RING)
                {
                    return 30;
                }
            }
            return 50;
        }

        private static int getProjectileGfx(Entity killer)
        {
            if (killer is Player)
            {
                BOW_TYPE bowType = getBowType(killer);
                if (bowType == BOW_TYPE.NORMAL_BOW)
                {
                    for (int i = 0; i < ARROWS.Length; i++)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == ARROWS[i])
                        {
                            return ARROW_PROJ_GFX[i];
                        }
                    }
                }
                else if (bowType == BOW_TYPE.CROSSBOW)
                {
                    for (int i = 0; i < BOLTS.Length; i++)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == BOLTS[i])
                        {
                            return 27;//BOLT_PROJ_GFX[i];
                        }
                    }
                }
                else if (bowType == BOW_TYPE.KARIL_BOW)
                {
                    if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == BOLT_RACK)
                    {
                        return 27;
                    }
                }
                else if (bowType == BOW_TYPE.CRYSTAL_BOW)
                {
                    return 249;
                }
                else if (bowType == BOW_TYPE.OBBY_RING)
                {
                    return 442;
                }
                else if (bowType == BOW_TYPE.DARK_BOW)
                {
                    if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == DRAGON_ARROW)
                    {
                        return 1121;
                    }
                    for (int i = 0; i < ARROWS.Length; i++)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == ARROWS[i])
                        {
                            return ARROW_PROJ_GFX[i];
                        }
                    }
                }
            }
            return -1;
        }

        private static BOW_TYPE getBowType(Entity killer)
        {
            for (int i = 0; i < NORMAL_BOWS.Length; i++)
            {
                if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == NORMAL_BOWS[i])
                {
                    return BOW_TYPE.NORMAL_BOW;
                }
            }
            for (int i = 0; i < CROSSBOWS.Length; i++)
            {
                if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == CROSSBOWS[i])
                {
                    return BOW_TYPE.CROSSBOW;
                }
            }
            for (int i = 0; i < KARIL_BOWS.Length; i++)
            {
                if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == KARIL_BOWS[i])
                {
                    return BOW_TYPE.KARIL_BOW;
                }
            }
            if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) >= 4214 && ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) <= 4223)
            {
                return BOW_TYPE.CRYSTAL_BOW;
            }
            else if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == OBBY_RING)
            {
                return BOW_TYPE.OBBY_RING;
            }
            else if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == DARK_BOW)
            {
                return BOW_TYPE.DARK_BOW;
            }
            return BOW_TYPE.NOT_BOW;
        }

        public static int getDrawbackGraphic(Entity killer)
        {
            if (killer is Player)
            {
                BOW_TYPE bowType = getBowType(killer);
                if (bowType == BOW_TYPE.NORMAL_BOW)
                {
                    for (int i = 0; i < ARROWS.Length; i++)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == ARROWS[i])
                        {
                            return ARROW_DB_GFX[i];
                        }
                    }
                }
                else if (bowType == BOW_TYPE.CROSSBOW)
                {
                    return -1;
                }
                else if (bowType == BOW_TYPE.KARIL_BOW)
                {
                    return -1;
                }
                else if (bowType == BOW_TYPE.CRYSTAL_BOW)
                {
                    return 250;
                }
                else if (bowType == BOW_TYPE.OBBY_RING)
                {
                    return -1;
                }
                else if (bowType == BOW_TYPE.DARK_BOW)
                {
                    for (int i = 0; i < ARROWS.Length; i++)
                    {
                        if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == ARROWS[i])
                        {
                            return DOUBLE_ARROW_DB_GFX[i];
                        }
                    }
                    if (((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS) == DRAGON_ARROW)
                    {
                        return 1111;
                    }
                }
            }
            return -1;
        }

        public static bool isUsingRange(Entity killer)
        {
            if (killer is Player)
            {
                int weapon = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
                for (int i = 0; i < RANGE_WEAPONS.Length; i++)
                {
                    if (weapon == RANGE_WEAPONS[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void degradeCrystalBow(Entity killer)
        {
            // TODO crystal bow degrading
        }

        public static bool hasValidBowArrow(Entity killer)
        {
            if (killer is Npc)
            {
                return true;
            }
            int BRONZE = 882, IRON = 884, STEEL = 886, MITHRIL = 888, ADAMANT = 890,  /*RUNE = 892,*/ BOLT_RACK = 4740;
            int weapon = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            int ammo = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
            string weaponName = ItemData.forId(weapon).getName();
            if (weapon == 841 || weapon == 845)
            {
                if (ammo != BRONZE && ammo != IRON)
                {
                    ((Player)killer).getPackets().sendMessage("You can only use arrows upto Iron with a " + weaponName + "!");
                    return false;
                }
            }
            else
                if (weapon == 843 || weapon == 845)
                {
                    if (ammo != BRONZE && ammo != IRON && ammo != STEEL)
                    {
                        ((Player)killer).getPackets().sendMessage("You can only use arrows upto Steel with an " + weaponName + "!");
                        return false;
                    }
                }
                else
                    if (weapon == 847 || weapon == 849)
                    {
                        if (ammo != BRONZE && ammo != IRON && ammo != STEEL && ammo != MITHRIL)
                        {
                            ((Player)killer).getPackets().sendMessage("You can only use arrows upto Mithril with a " + weaponName + "!");
                            return false;
                        }
                    }
                    else
                        if (weapon == 851 || weapon == 853)
                        {
                            if (ammo != BRONZE && ammo != IRON && ammo != STEEL && ammo != MITHRIL && ammo != ADAMANT)
                            {
                                ((Player)killer).getPackets().sendMessage("You can only use arrows upto Adamant with a " + weaponName + "!");
                                return false;
                            }
                        }
                        else
                            if (weapon == 4734 || weapon == 4934 || weapon == 4935 || weapon == 4936 || weapon == 4937)
                            {
                                if (ammo != BOLT_RACK)
                                {
                                    ((Player)killer).getPackets().sendMessage("You can only use Bolt Rack's with a Karil's crossbow!");
                                    return false;
                                }
                            }
            return true;
        }
    }
}