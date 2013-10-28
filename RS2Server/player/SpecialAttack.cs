using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.player
{
    internal class SpecialAttack
    {
        //TODO add all variations of weapons (p, p+, p++ etc)
        private static int POISON_AMOUNT = 6;

        private int specialAmount;
        private bool usingSpecial;
        private Player p;

        public SpecialAttack(Player p)
        {
            this.p = p;
        }

        public bool doSpecialAttack(Entity killer, Entity target)
        {
            int weapon = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            int currentPower = specialAmount;
            int neededPower = getRequiredAmount(weapon);
            bool rangeWeapon = false;
            if (!usingSpecial)
            {
                return false;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().ruleEnabled(DuelSession.RULE.NO_SPECIAL_ATTACKS))
                {
                    p.getPackets().sendMessage("Special attacks have been disabled for this duel!");
                    usingSpecial = false;
                    refreshBar();
                    return false;
                }
            }
            if (neededPower > currentPower)
            {
                ((Player)killer).getPackets().sendMessage("You don't have enough special power left.");
                usingSpecial = false;
                refreshBar();
                return false;
            }
            double damage = -1;
            double damage2 = -1;
            double damage3 = -1;
            double damage4 = -1;
            bool doubleHit = false;
            int increasedMaxHit = 0;
            damage = CombatFormula.getSpecialMeleeHit((Player)killer, target, weapon);
            damage2 = CombatFormula.getSpecialMeleeHit((Player)killer, target, weapon);
            int hitDelay = killer.getHitDelay();
            //int totalDamage = 0;
            int usingBow = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            int usingArrows = ((Player)killer).getEquipment().getItemInSlot(ItemData.EQUIP.ARROWS);
            bool usingRangePrayer = false;
            if (target is Player)
            {
                usingRangePrayer = ((Player)target).getPrayers().getHeadIcon() == PrayerData.RANGE;
            }
            switch (weapon)
            {
                case 4151: // Whip.
                    killer.setLastAnimation(new Animation(1658));
                    target.setLastGraphics(new Graphics(341, 0, 100));
                    if (Misc.random(3) == 0 && damage > 0)
                    {
                        damage = p.getMaxHit();
                    }
                    else
                    {
                        damage = 0;
                    }
                    if (target is Player)
                    {
                        int energy = ((Player)target).getRunEnergy() / 4;
                        ((Player)killer).setRunEnergy(((Player)killer).getRunEnergy() + energy);
                        ((Player)target).setRunEnergy(((Player)target).getRunEnergy() - energy);
                        if (((Player)killer).getRunEnergy() > 100)
                        {
                            ((Player)killer).setRunEnergy(100);
                        }
                        if (((Player)target).getRunEnergy() < 0)
                        {
                            ((Player)target).setRunEnergy(0);
                        }
                    }
                    break;

                case 1215: // Dragon daggers.
                case 1231:
                case 5680:
                case 5698:
                    if (damage > 0)
                    {
                        damage = p.getMaxHit(20);
                        damage2 = p.getMaxHit(20);
                    }
                    killer.setLastAnimation(new Animation(1062));
                    killer.setLastGraphics(new Graphics(252, 0, 100));
                    doubleHit = true;
                    /*if (misc.random(3) == 0 && CombatFormula.getMeleeHit(p, target) > 0) {
                        if (damage <= (CombatFormula.getMaxHit(p, 20) / 2) && damage2 <= (CombatFormula.getMaxHit(p, 20) / 2)) {
                            damage = (CombatFormula.getMaxHit(p, 20) / 2) + (misc.randomDouble() * CombatFormula.getMaxHit(p, 20)) / 2);
                            damage2 = (CombatFormula.getMaxHit(p, 20) / 2) + (misc.randomDouble() * CombatFormula.getMaxHit(p, 20)) / 2);
                        }
                    } else if (misc.random(2) == 0) {
                        damage = 0;
                        damage2 = 0;
                    }*/
                    double a = damage + damage2;
                    if (damage > target.getHp())
                    {
                        damage = target.getHp();
                    }
                    a -= damage;
                    if (damage2 > a)
                    {
                        damage2 = a;
                    }
                    /*if (damage > target.getHp()) {
                        int randomHp = misc.random(target.getHp());
                        damage = randomHp;
                        damage2 = target.getHp() - randomHp;
                    } else {
                        int hpRemaining = target.getHp() - damage;
                        if (damage2 > hpRemaining) {
                            damage2 = hpRemaining;
                        }
                    }*/
                    break;

                case 1305: // Dragon longsword.
                    killer.setLastAnimation(new Animation(1058));
                    killer.setLastGraphics(new Graphics(248, 0, 100));
                    damage = p.getMaxHit(30);
                    break;

                case 11694: // Armadyl godsword.
                    killer.setLastGraphics(new Graphics(1222, 0, 100));
                    killer.setLastAnimation(new Animation(7074));
                    damage += p.getMaxHit() * 0.25;
                    break;

                case 11696: // Bandos godsword.
                    killer.setLastGraphics(new Graphics(1223, 0, 100));
                    killer.setLastAnimation(new Animation(7073));
                    damage += p.getMaxHit() * 0.10;
                    break;

                case 11698: // Saradomin godsword.
                    killer.setLastGraphics(new Graphics(1220, 0, 100));
                    killer.setLastAnimation(new Animation(7071));
                    int newHp = (int)(damage * 0.50);
                    int newPrayer = (int)(damage * 0.25);
                    if (newHp < 10)
                    {
                        newHp = 10;
                    }
                    if (newPrayer < 5)
                    {
                        newPrayer = 5;
                    }
                    ((Player)killer).heal(newHp);
                    ((Player)killer).getSkills().setCurLevel(Skills.SKILL.PRAYER, ((Player)killer).getSkills().getCurLevel(Skills.SKILL.PRAYER) + newPrayer);
                    if (((Player)killer).getSkills().getCurLevel(Skills.SKILL.PRAYER) > ((Player)killer).getSkills().getMaxLevel(Skills.SKILL.PRAYER))
                    {
                        ((Player)killer).getSkills().setCurLevel(Skills.SKILL.PRAYER, ((Player)killer).getSkills().getMaxLevel(Skills.SKILL.PRAYER));
                    }
                    break;

                case 11700: // Zamorak godsword
                    killer.setLastGraphics(new Graphics(1221, 0, 100));
                    killer.setLastAnimation(new Animation(7070));
                    target.setLastGraphics(new Graphics(369));
                    MagicCombat.freezeTarget(31, target);
                    break;

                case 11730: // Saradomin sword
                    target.setLastGraphics(new Graphics(1207, 0, 100));
                    killer.setLastAnimation(new Animation(7072));
                    increasedMaxHit = 16;
                    break;

                case 1434: // Dragon mace
                    hitDelay = 700;
                    killer.setLastGraphics(new Graphics(251, 0, 75));
                    killer.setLastAnimation(new Animation(1060));
                    damage = p.getMaxHit(60);
                    break;

                case 3204: // Dragon halberd
                    // TODO halberd
                    break;

                case 4587: // Dragon scimitar
                    killer.setLastGraphics(new Graphics(347, 0, 100));
                    killer.setLastAnimation(new Animation(451));
                    if (target is Player)
                    {
                        if (((Player)target).getPrayers().getOverheadPrayer() >= 1 && ((Player)target).getPrayers().getOverheadPrayer() <= 3)
                        {
                            ((Player)target).getPrayers().setOverheadPrayer(0);
                            ((Player)target).getPrayers().setHeadIcon(-1);
                            ((Player)target).getPackets().sendMessage("The Dragon scimitar slashes through your prayer protection!");
                            ((Player)target).getPackets().sendConfig(95, 0);
                            ((Player)target).getPackets().sendConfig(97, 0);
                            ((Player)target).getPackets().sendConfig(98, 0);
                            ((Player)target).getPackets().sendConfig(99, 0);
                            ((Player)target).getPackets().sendConfig(100, 0);
                            ((Player)target).getPackets().sendConfig(96, 0);
                        }
                    }
                    break;

                case 14484: // Dragon claws
                    doubleHit = true;
                    killer.setLastGraphics(new Graphics(1950));
                    killer.setLastAnimation(new Animation(10961));
                    if (Misc.random(1) == 0 && damage > 0)
                    {
                        if (damage < p.getMaxHit(20) * 0.75)
                        {
                            damage = (p.getMaxHit(20) * 0.75 + (Misc.randomDouble() * (p.getMaxHit(20) * 0.25)));
                        }
                    }
                    damage = (int)Math.Floor(damage);
                    damage2 = (int)Math.Floor(damage * 0.50);
                    damage3 = (int)Math.Floor(damage2 * 0.50);
                    damage4 = (int)Math.Floor(damage3 + 1);
                    break;

                case 1249: // Dragon spear
                    //TODO leave due to noclipping?
                    break;

                case 6739: // Dragon axe
                    //TODO find emote and graphic
                    break;

                case 7158: // Dragon 2h
                    killer.setLastAnimation(new Animation(3157));
                    killer.setLastGraphics(new Graphics(559));
                    //TODO multi combat
                    break;

                case 3101: // Rune claws
                    killer.setLastGraphics(new Graphics(274));
                    break;

                case 4153: // Granite maul
                    killer.setLastAnimation(new Animation(1667));
                    killer.setLastGraphics(new Graphics(340, 0, 100));
                    //doubleHit = true;
                    break;

                case 10887: // Barrelchest anchor
                    break;

                case 11061: // Ancient mace
                    break;

                case 13902: // Statius' warhammer
                    killer.setLastAnimation(new Animation(10505));
                    killer.setLastGraphics(new Graphics(1840));
                    damage += killer.getMaxHit() * 0.25;
                    if (target is Player)
                    {
                        int defenceLevel = ((Player)target).getSkills().getCurLevel(Skills.SKILL.DEFENCE);
                        int newDefence = (int)(defenceLevel * 0.30);
                        if (newDefence < 1)
                        {
                            newDefence = 1;
                        }
                        ((Player)target).getSkills().setCurLevel(Skills.SKILL.DEFENCE, defenceLevel - newDefence);
                        ((Player)target).getPackets().sendSkillLevel(Skills.SKILL.DEFENCE);
                    }
                    break;

                case 13899: // Vesta's longsword
                    killer.setLastAnimation(new Animation(10502));
                    damage += killer.getMaxHit() * 0.20;
                    break;

                case 13905: // Vesta's spear
                    killer.setLastAnimation(new Animation(10499));
                    killer.setLastGraphics(new Graphics(1835));
                    break;

                case 13883: // Morrigans throwing axe
                    break;

                case 13879: // Morrigans javelin

                case 8880: // Dorgeshuun crossbow
                    break;

                case 861: // Magic shortbow
                case 859: // Magic longbow
                case 10284: // Magic composite bow
                    rangeWeapon = true;
                    if (p.getEquipment().getAmountInSlot(ItemData.EQUIP.ARROWS) < 2)
                    {
                        p.getPackets().sendMessage("You need 2 arrows to use the Magic bow special attack!");
                        return false;
                    }
                    damage = (int)CombatFormula.getRangeHit((Player)killer, target, usingBow, usingArrows);
                    damage2 = (int)CombatFormula.getRangeHit((Player)killer, target, usingBow, usingArrows);
                    damage *= 1.05;
                    damage2 *= 1.05;
                    if (usingRangePrayer)
                    {
                        damage *= 0.60;
                        damage2 *= 0.60;
                    }
                    double a1 = damage + damage2;
                    if (damage > target.getHp())
                    {
                        damage = target.getHp();
                    }
                    a1 -= damage;
                    if (damage2 > a1)
                    {
                        damage2 = a1;
                    }
                    /*if (damage >= target.getHp()) {
                        int randomHp = misc.random(target.getHp());
                        damage = randomHp;
                        damage2 = target.getHp() - randomHp;
                    } else {
                        int hpRemaining = target.getHp() - damage;
                        if (damage2 > hpRemaining) {
                            damage2 = hpRemaining;
                        }
                    }*/
                    p.setLastAnimation(new Animation(1074));
                    p.setLastGraphics(new Graphics(256, 0, 90));
                    RangeCombat.deductArrow(killer);
                    RangeCombat.deductArrow(killer);
                    int arrowType = RangeCombat.getArrowType(killer);
                    hitDelay = 1000;
                    int MSpecCounter = 0;
                    Event displayMSpecProjectileEvent = new Event(0);
                    displayMSpecProjectileEvent.setAction(() =>
                    {
                        RangeCombat.displayMSpecProjectile(killer, target);
                        MSpecCounter++;
                        if (MSpecCounter == 1)
                        {
                            displayMSpecProjectileEvent.setTick(500);
                            p.setLastGraphics(new Graphics(256, 0, 90));
                            Event doMSpecHitEvent = new Event(900);
                            doMSpecHitEvent.setAction(() =>
                            {
                                doMSpecHitEvent.stop();
                                target.hit((int)damage2);
                                RangeCombat.createGroundArrow(killer, target, arrowType);
                            });
                            Server.registerEvent(doMSpecHitEvent);
                        }
                        else
                        {
                            displayMSpecProjectileEvent.stop();
                            return;
                        }
                        MSpecCounter++;
                    });
                    Server.registerEvent(displayMSpecProjectileEvent);
                    break;

                case 805: // Rune thrownaxe
                    rangeWeapon = true;
                    break;

                case 6724: // Seercull
                    rangeWeapon = true;
                    break;

                case 11235: // Dark bow
                    rangeWeapon = true;
                    if (p.getEquipment().getAmountInSlot(ItemData.EQUIP.ARROWS) < 2)
                    {
                        p.getPackets().sendMessage("You need 2 arrows to use the Dark bow!");
                        return false;
                    }
                    int minHit = 8;
                    damage = (int)CombatFormula.getRangeHit((Player)killer, target, usingBow, usingArrows);
                    damage2 = (int)CombatFormula.getRangeHit((Player)killer, target, usingBow, usingArrows);
                    if (usingBow == 11235)
                    { // Dark bow
                        if (usingArrows == 11212)
                        { // Dragon arrows
                            minHit = usingRangePrayer ? 4 : 8;
                            damage *= 1.50;
                            damage2 *= 1.50;
                            if (damage < minHit)
                            {
                                damage = minHit;
                            }
                            if (damage2 < minHit)
                            {
                                damage2 = minHit;
                            }
                        }
                        else
                        { // Other arrow
                            minHit = usingRangePrayer ? 3 : 5;
                            damage *= 1.30;
                            damage2 *= 1.30;
                            if (damage < minHit)
                            {
                                damage = minHit;
                            }
                            if (damage2 < minHit)
                            {
                                damage2 = minHit;
                            }
                        }
                    }
                    if (usingRangePrayer)
                    {
                        damage *= 0.60;
                        damage2 *= 0.60;
                    }
                    double a2 = damage + damage2;
                    if (damage > target.getHp())
                    {
                        damage = target.getHp();
                    }
                    a2 -= damage;
                    if (damage2 > a2)
                    {
                        damage2 = a2;
                    }
                    /*if (damage >= target.getHp()) {
                        int randomHp = misc.random(target.getHp());
                        damage = randomHp;
                        damage2 = target.getHp() - randomHp;
                    } else {
                        int hpRemaining = target.getHp() - damage;
                        if (damage2 > hpRemaining) {
                            damage2 = hpRemaining;
                        }
                    }*/
                    p.setLastGraphics(new Graphics(RangeCombat.getDrawbackGraphic(killer), 0, 90));
                    RangeCombat.deductArrow(killer);
                    RangeCombat.deductArrow(killer);
                    hitDelay = RangeCombat.getHitDelay(killer, target);
                    int delayHit = hitDelay;
                    hitDelay = 1200;
                    int arrowType1 = RangeCombat.getArrowType(killer);
                    int DBSpecCounter = 0;
                    Event DBSpecProjectile = new Event(0);
                    DBSpecProjectile.setAction(() =>
                    {
                        RangeCombat.displayDBSpecProjectile(killer, target);
                        DBSpecCounter++;
                        if (DBSpecCounter == 1)
                        {
                            DBSpecProjectile.setTick(200);
                            Event rangeHitEvent = new Event(delayHit + 600);
                            rangeHitEvent.setAction(() =>
                            {
                                target.hit((int)damage2);
                                rangeHitEvent.stop();
                                RangeCombat.createGroundArrow(killer, target, arrowType1);
                            });
                            Server.registerEvent(rangeHitEvent);
                        }
                        else
                        {
                            DBSpecProjectile.stop();
                            return;
                        }
                        DBSpecCounter++;
                    });
                    Server.registerEvent(DBSpecProjectile);
                    break;
            }
            specialAmount -= neededPower;
            p.setSpecialAmount(specialAmount);
            usingSpecial = false;
            refreshBar();
            killer.resetCombatTurns();
            bool hitDouble = doubleHit;
            if (target is Player)
            {
                if (!rangeWeapon)
                {
                    if (((Player)target).getPrayers().getHeadIcon() == PrayerData.MELEE)
                    {
                        damage = (int)(damage * 0.60);
                    }
                }
                else
                {
                    if (((Player)target).getPrayers().getHeadIcon() == PrayerData.RANGE)
                    {
                        damage = (int)(damage * 0.60);
                    }
                }
            }
            damage = ((weapon == 4151) ? damage : (Misc.randomDouble() * damage));
            damage2 = Misc.randomDouble() * damage2;
            int d = (int)((damage == 0 && weapon != 11730) ? 0 : (damage + increasedMaxHit));
            int d2 = (int)damage2;
            int d3 = (int)damage3; // only used for d claws
            int d4 = (int)damage4; // only used for d claws
            if (canPoison(weapon))
            {
                if (!target.isPoisoned() && Misc.random(5) == 0 && (hitDouble ? (d2 > 0 || d > 0) : d > 0))
                {
                    Server.registerEvent(new PoisonEvent(target, POISON_AMOUNT));
                }
            }
            int hhitDelay = hitDelay;
            int totDamage = Convert.ToInt32(damage + damage2 + damage3 + damage4);
            Combat.checkIfWillDie(target, totDamage);
            Event hitEvent = new Event(hhitDelay);
            hitEvent.setAction(() =>
            {
                hitEvent.stop();
                if (!target.isDead())
                {
                    target.setLastAnimation(new Animation(target.getDefenceAnimation()));
                }
                target.hit(d);
                if (hitDouble)
                {
                    target.hit(d2);
                }
                if (d3 != -1 || d4 != -1)
                {
                    target.hit(d3);
                    target.hit(d4);
                }
                Combat.checkRecoil(killer, target, totDamage);
                Combat.checkSmite(killer, target, totDamage);
                Combat.checkVengeance(killer, target, totDamage);
                Combat.addXp(killer, target, totDamage);
            });
            Server.registerEvent(hitEvent);
            return true;
        }

        public void dragonBattleaxe()
        {
            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) != 1377)
            {
                return;
            }
            int neededPower = getRequiredAmount(p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON));
            if (neededPower > specialAmount)
            {
                p.getPackets().sendMessage("You don't have enough special power left.");
                usingSpecial = false;
                refreshBar();
                return;
            }
            specialAmount -= neededPower;
            p.setSpecialAmount(specialAmount);
            usingSpecial = false;
            refreshBar();
            p.setLastAnimation(new Animation(1056));
            p.setLastGraphics(new Graphics(246));
            p.setForceText("Raarrrrrgggggghhhhhhh!");
            Consumables.statBoost(p, Skills.SKILL.STRENGTH, 0.2, 0, false);

            p.getPackets().sendSkillLevel(Skills.SKILL.STRENGTH);
        }

        public void refreshBar()
        {
            p.getPackets().sendConfig(300, specialAmount * 10);
            p.getPackets().sendConfig(301, usingSpecial ? 1 : 0);
        }

        public void toggleSpecBar()
        {
            usingSpecial = usingSpecial ? false : true;
            refreshBar();
            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 4153 && specialAmount >= getRequiredAmount(4153))
            {
                p.setCombatTurns(p.getAttackSpeed());
            }
        }

        public void resetSpecial()
        {
            specialAmount = 100;
            usingSpecial = false;
            refreshBar();
        }

        private int getRequiredAmount(int weapon)
        {
            int[] weapons = {4151, 1215, 1231, 5680, 5698, 1305, 11694, 11696, 11698, 11700, 11730, 1434, 1377, 3204, 4587, 14484, 1249,
			    6739, 7158, 8880, 861, 859, 10284, 805, 6724, 11235, 3101, 4153, 10887, 11061, 13902, 13899, 13905, 13883, 13879
		    };
            int[] amount = {
				    50, // Abyssal whip
				    25, // Dragon dagger
				    25, // Dragon dagger
				    25, // Dragon dagger
				    25, // Dragon dagger
				    50, // Dragon longsword
				    50, // Armadyl godsword
				    100, // Bandos godsword
				    50, // Saradomin godsword
				    60, // Zamorak godsword
				    100, // Saradomin sword
				    25, // Dragon mace
				    100, // Dragon battleaxe
				    30, // Dragon halberd
				    55, // Dragon scimitar
				    50, // Dragon claws
				    25, // Dragon spear
				    100, // Dragon axe
				    55, // Dragon 2h sword
				    90, // Dorgeshuun crossbow
				    55, // Magic shortbow
				    40, // Magic longbow
				    40, // Magic composite bow
				    10, // Rune thrownaxe
				    100, // Seercull
				    65, // Dark bow
				    30, // Rune claws
				    50, // Granite maul
				    50, // Barrelchest anchor
				    100, // Ancient mace
				    50, // Statius' warhammer
				    25, // Vesta's longsword
				    50, // Vesta's spear
				    50, // Morrigans throwing axe
				    50, // Morrigans javelin
			    };
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapon == weapons[i])
                {
                    return amount[i];
                }
            }
            return 0;
        }

        private bool canPoison(int weapon)
        {
            int[] weapons = { 1231, 5680, 5698, 1263, 5716, 5730 };
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapon == weapons[i])
                {
                    return true;
                }
            }
            return false;
        }

        public void setSpecialAmount(int specialAmount)
        {
            this.specialAmount = specialAmount;
            refreshBar();
        }

        public int getSpecialAmount()
        {
            return specialAmount;
        }

        public void setUsingSpecial(bool usingSpecial)
        {
            this.usingSpecial = usingSpecial;
        }

        public bool isUsingSpecial()
        {
            return usingSpecial;
        }
    }
}