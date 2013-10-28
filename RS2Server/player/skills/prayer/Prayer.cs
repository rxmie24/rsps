using RS2.Server.events;
using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.prayer
{
    internal class Prayer : PrayerData
    {
        public static bool wantToBury(Player p, int item, int slot)
        {
            for (int i = 0; i < BONES.Length; i++)
            {
                if (item == BONES[i])
                {
                    buryBone(p, i, slot);
                    return true;
                }
            }
            return false;
        }

        private static void buryBone(Player p, int i, int slot)
        {
            long lastBury = 0;
            if (p.getTemporaryAttribute("lastBury") != null)
            {
                lastBury = (int)p.getTemporaryAttribute("lastBury");
            }
            if (Environment.TickCount - lastBury < 400)
            {
                return;
            }

            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            p.getPackets().sendMessage("You dig a hole in the ground...");
            p.setLastAnimation(new Animation(BURY_ANIMATION));
            Event buryBoneEvent = new Event(300);
            buryBoneEvent.setAction(() =>
            {
                buryBoneEvent.stop();
                if (p.getInventory().deleteItem(BONES[i], slot, 1))
                {
                    p.getSkills().addXp(Skills.SKILL.PRAYER, BURY_XP[i]);
                    p.setTemporaryAttribute("lastBury", Environment.TickCount);
                    p.getPackets().sendMessage("You bury the bones.");
                }
            });
            Server.registerEvent(buryBoneEvent);
        }

        public static bool canUsePrayer(Player p, int id)
        {
            if (id < 5 || id > 57)
            {
                return false;
            }
            if (p.getSkills().getCurLevel(Skills.SKILL.PRAYER) <= 0)
            {
                return false;
            }
            int j = 0;
            for (int i = 5; i <= 57; i += 2)
            {
                if (id == i)
                {
                    if (i == 53)
                    {
                        p.getPackets().softCloseInterfaces();
                        p.getPackets().modifyText("This prayer is currently unavailable.", 210, 1);
                        p.getPackets().sendChatboxInterface(210);
                        p.getPackets().sendConfig(1168, 0);
                        continue;
                    }
                    if (p.getSkills().getGreaterLevel(Skills.SKILL.PRAYER) < PRAYER_LVL[j])
                    {
                        p.getPackets().softCloseInterfaces();
                        p.getPackets().modifyText("You need level " + PRAYER_LVL[j] + " Prayer to use " + PRAYER_NAME[j] + ".", 210, 1);
                        p.getPackets().sendChatboxInterface(210);
                        return false;
                    }
                }
                j++;
            }
            return true;
        }

        public static void deactivateAllPrayers(Player p)
        {
            p.getPrayers().setPrayerDrain(0.0);
            p.getPrayers().setSuperPrayer(0);
            p.getPrayers().setAttackPrayer(0);
            p.getPrayers().setDefencePrayer(0);
            p.getPrayers().setStrengthPrayer(0);
            p.getPrayers().setRangePrayer(0);
            p.getPrayers().setMagicPrayer(0);
            p.getPrayers().setOverheadPrayer(0);
            p.getPrayers().setHeadIcon(-1);
            p.getPrayers().setProtectItem(false);
            p.getPrayers().setRapidRestore(false);
            p.getPrayers().setRapidHeal(false);
            p.getPackets().sendConfig(89, 0);
            p.getPackets().sendConfig(90, 0);
            p.getPackets().sendConfig(91, 0);
            p.getPackets().sendConfig(83, 0);
            p.getPackets().sendConfig(86, 0);
            p.getPackets().sendConfig(92, 0);
            p.getPackets().sendConfig(84, 0);
            p.getPackets().sendConfig(87, 0);
            p.getPackets().sendConfig(93, 0);
            p.getPackets().sendConfig(85, 0);
            p.getPackets().sendConfig(88, 0);
            p.getPackets().sendConfig(94, 0);
            p.getPackets().sendConfig(862, 0);
            p.getPackets().sendConfig(866, 0);
            p.getPackets().sendConfig(864, 0);
            p.getPackets().sendConfig(863, 0);
            p.getPackets().sendConfig(865, 0);
            p.getPackets().sendConfig(867, 0);
            p.getPackets().sendConfig(1053, 0);
            p.getPackets().sendConfig(1052, 0);
            p.getPackets().sendConfig(96, 0);
            p.getPackets().sendConfig(97, 0);
            p.getPackets().sendConfig(98, 0);
            p.getPackets().sendConfig(99, 0);
            p.getPackets().sendConfig(100, 0);
            p.getPackets().sendConfig(95, 0);
            p.getPackets().sendConfig(84, 0);
            p.getPackets().sendConfig(87, 0);
            p.getPackets().sendConfig(93, 0);
            p.getPackets().sendConfig(85, 0);
            p.getPackets().sendConfig(88, 0);
            p.getPackets().sendConfig(94, 0);
            p.getPackets().sendConfig(1053, 0);
            p.getPackets().sendConfig(1052, 0);
            p.getPackets().sendConfig(864, 0);
            p.getPackets().sendConfig(866, 0);
            p.getPackets().sendConfig(863, 0);
            p.getPackets().sendConfig(865, 0);
            p.getPackets().sendConfig(867, 0);
            p.getPackets().sendConfig(862, 0);
            p.getPackets().sendConfig(84, 0);
            p.getPackets().sendConfig(87, 0);
            p.getPackets().sendConfig(93, 0);
            p.getPackets().sendConfig(85, 0);
            p.getPackets().sendConfig(88, 0);
            p.getPackets().sendConfig(94, 0);
            p.getPackets().sendConfig(1053, 0);
            p.getPackets().sendConfig(1052, 0);
            p.getPackets().sendConfig(862, 0);
            p.getPackets().sendConfig(864, 0);
            p.getPackets().sendConfig(866, 0);
            p.getPackets().sendConfig(865, 0);
            p.getPackets().sendConfig(867, 0);
            p.getPackets().sendConfig(863, 0);
            for (int i = 0; i < p.getPrayers().getPrayerActiveArray().Length; i++)
            {
                p.getPrayers().setPrayerActive(i, false);
            }
        }

        public static void startPrayerDrainEvent(Player p)
        {
            Event prayerDrainEvent = new Event(600);
            prayerDrainEvent.setAction(() =>
            {
                if (p == null || p.isDead() || !isPrayerActive(p))
                {
                    prayerDrainEvent.stop();
                    return;
                }
                double amountDrain = 0.0;
                for (int i = 0; i < p.getPrayers().getPrayerActiveArray().Length; i++)
                {
                    if (p.getPrayers().isPrayerActive(i))
                    {
                        double drain = DRAIN_RATE[i];
                        double bonus = (0.035 * p.getEquipment().getBonus(Equipment.BONUS.PRAYER));
                        drain = drain * (1 + bonus);
                        drain = 0.6 / drain;
                        amountDrain += drain;
                    }
                }
                p.decreasePrayerPoints(amountDrain);
                if (p.getSkills().getCurLevel(Skills.SKILL.PRAYER) <= 0)
                    prayerDrainEvent.stop();
            });
            Server.registerEvent(prayerDrainEvent);
        }

        protected static bool isPrayerActive(Player p)
        {
            for (int i = 0; i < p.getPrayers().getPrayerActiveArray().Length; i++)
            {
                if (p.getPrayers().isPrayerActive(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static void togglePrayer(Player p, int prayerType, int prayerID)
        {
            if (p.isDead())
            {
                return;
            }
            if (p.getSkills().getCurLevel(Skills.SKILL.PRAYER) <= 0)
            {
                deactivateAllPrayers(p);
                return;
            }
            p.getPackets().softCloseInterfaces();
            bool usingPrayer = isPrayerActive(p);
            switch (prayerType)
            {
                case 1: // defence prayers
                    switch (prayerID)
                    {
                        case 1: // thick skin
                            if (p.getPrayers().getDefencePrayer() != 1)
                            {
                                p.getPrayers().setDefencePrayer(1);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPackets().sendConfig(86, 0);
                                p.getPackets().sendConfig(92, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(83, 1);
                                p.getPrayers().setPrayerActive(0, true); // thick skin
                                p.getPrayers().setPrayerActive(5, false); // rock skin
                                p.getPrayers().setPrayerActive(13, false); // steel skin
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                            }
                            else if (p.getPrayers().getDefencePrayer() == 1)
                            {
                                p.getPrayers().setDefencePrayer(0);
                                p.getPackets().sendConfig(83, 0);
                                p.getPrayers().setPrayerActive(0, false);
                            }
                            break;

                        case 2: // rock skin
                            if (p.getPrayers().getDefencePrayer() != 2)
                            {
                                p.getPrayers().setDefencePrayer(2);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPackets().sendConfig(83, 0);
                                p.getPackets().sendConfig(92, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(86, 1);
                                p.getPrayers().setPrayerActive(0, false); // thick skin
                                p.getPrayers().setPrayerActive(5, true); // rock skin
                                p.getPrayers().setPrayerActive(13, false); // steel skin
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                            }
                            else if (p.getPrayers().getDefencePrayer() == 2)
                            {
                                p.getPrayers().setDefencePrayer(0);
                                p.getPackets().sendConfig(86, 0);
                                p.getPrayers().setPrayerActive(5, false);
                            }
                            break;

                        case 3: // steel skin
                            if (p.getPrayers().getDefencePrayer() != 3)
                            {
                                p.getPrayers().setDefencePrayer(3);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPackets().sendConfig(83, 0);
                                p.getPackets().sendConfig(86, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(92, 1);
                                p.getPrayers().setPrayerActive(0, false); // thick skin
                                p.getPrayers().setPrayerActive(5, false); // rock skin
                                p.getPrayers().setPrayerActive(13, true); // steel skin
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                            }
                            else if (p.getPrayers().getDefencePrayer() == 3)
                            {
                                p.getPrayers().setDefencePrayer(0);
                                p.getPackets().sendConfig(92, 0);
                                p.getPrayers().setPrayerActive(13, false);
                            }
                            break;
                    }
                    break;

                case 2: // strength prayers
                    switch (prayerID)
                    {
                        case 1: // burst of strength
                            if (p.getPrayers().getStrengthPrayer() != 1)
                            {
                                p.getPrayers().setStrengthPrayer(1);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(84, 1);
                                p.getPrayers().setPrayerActive(1, true); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getStrengthPrayer() == 1)
                            {
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPrayers().setPrayerActive(1, false);
                            }
                            break;

                        case 2: // superhuman strength
                            if (p.getPrayers().getStrengthPrayer() != 2)
                            {
                                p.getPrayers().setStrengthPrayer(2);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(87, 1);
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, true); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getStrengthPrayer() == 2)
                            {
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPrayers().setPrayerActive(6, false);
                            }
                            break;

                        case 3: // ultimate strength
                            if (p.getPrayers().getStrengthPrayer() != 3)
                            {
                                p.getPrayers().setStrengthPrayer(3);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(93, 1);
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, true); // ultimate strength
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getStrengthPrayer() == 3)
                            {
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPrayers().setPrayerActive(14, false);
                            }
                            break;
                    }
                    break;

                case 3:// attack prayers
                    switch (prayerID)
                    {
                        case 1: // clarity of thought
                            if (p.getPrayers().getAttackPrayer() != 1)
                            {
                                p.getPrayers().setAttackPrayer(1);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(85, 1);
                                p.getPrayers().setPrayerActive(2, true); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getAttackPrayer() == 1)
                            {
                                p.getPrayers().setAttackPrayer(0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPrayers().setPrayerActive(2, false);
                            }
                            break;

                        case 2: // improved reflexes
                            if (p.getPrayers().getAttackPrayer() != 2)
                            {
                                p.getPrayers().setAttackPrayer(2);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(88, 1);
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, true); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getAttackPrayer() == 2)
                            {
                                p.getPrayers().setAttackPrayer(0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPrayers().setPrayerActive(7, false);
                            }
                            break;

                        case 3: // incredible reflexes
                            if (p.getPrayers().getAttackPrayer() != 3)
                            {
                                p.getPrayers().setAttackPrayer(3);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(94, 1);
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, true); // incredible reflexes
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getAttackPrayer() == 3)
                            {
                                p.getPrayers().setAttackPrayer(0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPrayers().setPrayerActive(15, false);
                            }
                            break;
                    }
                    break;

                case 4: // headicons
                    switch (prayerID)
                    {
                        case 1: // Magic protect
                            if (p.getPrayers().getOverheadPrayer() != 1)
                            {
                                p.getPrayers().setOverheadPrayer(1);
                                p.getPrayers().setHeadIcon(2);
                                p.getPackets().sendConfig(96, 0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPackets().sendConfig(95, 1);
                                p.getPrayers().setPrayerActive(17, true); // magic protect
                                p.getPrayers().setPrayerActive(18, false); // range protect
                                p.getPrayers().setPrayerActive(19, false); // melee protect
                                p.getPrayers().setPrayerActive(22, false); // retribution
                                p.getPrayers().setPrayerActive(23, false); // redemption
                                p.getPrayers().setPrayerActive(24, false); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 1)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(95, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(17, false);
                            }
                            break;

                        case 2: // Ranged protect
                            if (p.getPrayers().getOverheadPrayer() != 2)
                            {
                                p.getPrayers().setOverheadPrayer(2);
                                p.getPrayers().setHeadIcon(1);
                                p.getPackets().sendConfig(95, 0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPackets().sendConfig(96, 1);
                                p.getPrayers().setPrayerActive(17, false); // magic protect
                                p.getPrayers().setPrayerActive(18, true); // range protect
                                p.getPrayers().setPrayerActive(19, false); // melee protect
                                p.getPrayers().setPrayerActive(22, false); // retribution
                                p.getPrayers().setPrayerActive(23, false); // redemption
                                p.getPrayers().setPrayerActive(24, false); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 2)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(96, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(18, false);
                            }
                            break;

                        case 3: // Melee protect
                            if (p.getPrayers().getOverheadPrayer() != 3)
                            {
                                p.getPrayers().setOverheadPrayer(3);
                                p.getPrayers().setHeadIcon(0);
                                p.getPackets().sendConfig(95, 0);
                                p.getPackets().sendConfig(96, 0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPackets().sendConfig(97, 1);
                                p.getPrayers().setPrayerActive(17, false); // magic protect
                                p.getPrayers().setPrayerActive(18, false); // range protect
                                p.getPrayers().setPrayerActive(19, true); // melee protect
                                p.getPrayers().setPrayerActive(22, false); // retribution
                                p.getPrayers().setPrayerActive(23, false); // redemption
                                p.getPrayers().setPrayerActive(24, false); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 3)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(19, false);
                            }
                            break;

                        case 4: // Retribution
                            if (p.getPrayers().getOverheadPrayer() != 4)
                            {
                                p.getPrayers().setOverheadPrayer(4);
                                p.getPrayers().setHeadIcon(3);
                                p.getPackets().sendConfig(95, 0);
                                p.getPackets().sendConfig(96, 0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPackets().sendConfig(98, 1);
                                p.getPrayers().setPrayerActive(17, false); // magic protect
                                p.getPrayers().setPrayerActive(18, false); // range protect
                                p.getPrayers().setPrayerActive(19, false); // melee protect
                                p.getPrayers().setPrayerActive(22, true); // retribution
                                p.getPrayers().setPrayerActive(23, false); // redemption
                                p.getPrayers().setPrayerActive(24, false); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 4)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(22, false);
                            }
                            break;

                        case 5: // Redemption
                            if (p.getPrayers().getOverheadPrayer() != 5)
                            {
                                p.getPrayers().setOverheadPrayer(5);
                                p.getPrayers().setHeadIcon(5);
                                p.getPackets().sendConfig(95, 0);
                                p.getPackets().sendConfig(96, 0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPackets().sendConfig(99, 1);
                                p.getPrayers().setPrayerActive(17, false); // magic protect
                                p.getPrayers().setPrayerActive(18, false); // range protect
                                p.getPrayers().setPrayerActive(19, false); // melee protect
                                p.getPrayers().setPrayerActive(22, false); // retribution
                                p.getPrayers().setPrayerActive(23, true); // redemption
                                p.getPrayers().setPrayerActive(24, false); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 5)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(23, false);
                            }
                            break;

                        case 6: // Smite
                            if (p.getPrayers().getOverheadPrayer() != 6)
                            {
                                p.getPrayers().setOverheadPrayer(6);
                                p.getPrayers().setHeadIcon(4);
                                p.getPackets().sendConfig(95, 0);
                                p.getPackets().sendConfig(96, 0);
                                p.getPackets().sendConfig(97, 0);
                                p.getPackets().sendConfig(98, 0);
                                p.getPackets().sendConfig(99, 0);
                                p.getPackets().sendConfig(100, 1);
                                p.getPrayers().setPrayerActive(17, false); // magic protect
                                p.getPrayers().setPrayerActive(18, false); // range protect
                                p.getPrayers().setPrayerActive(19, false); // melee protect
                                p.getPrayers().setPrayerActive(22, false); // retribution
                                p.getPrayers().setPrayerActive(23, false); // redemption
                                p.getPrayers().setPrayerActive(24, true); // smite
                            }
                            else if (p.getPrayers().getOverheadPrayer() == 6)
                            {
                                p.getPrayers().setOverheadPrayer(0);
                                p.getPackets().sendConfig(100, 0);
                                p.getPrayers().setHeadIcon(-1);
                                p.getPrayers().setPrayerActive(24, false);
                            }
                            break;
                    }
                    break;

                case 5: // piety/chivalry
                    switch (prayerID)
                    {
                        case 1: // Chivalry
                            if (p.getPrayers().getSuperPrayer() != 1)
                            {
                                p.getPrayers().setSuperPrayer(1);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setDefencePrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(83, 0);
                                p.getPackets().sendConfig(86, 0);
                                p.getPackets().sendConfig(92, 0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 1);
                                p.getPrayers().setPrayerActive(25, true); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(0, false); // thick skin
                                p.getPrayers().setPrayerActive(5, false); // rock skin
                                p.getPrayers().setPrayerActive(13, false); // steel skin
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                            }
                            else if (p.getPrayers().getSuperPrayer() == 1)
                            {
                                p.getPrayers().setSuperPrayer(0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPrayers().setPrayerActive(25, false);
                            }
                            break;

                        case 2: // Piety
                            if (p.getPrayers().getSuperPrayer() != 2)
                            {
                                p.getPrayers().setSuperPrayer(2);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setDefencePrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(83, 0);
                                p.getPackets().sendConfig(86, 0);
                                p.getPackets().sendConfig(92, 0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(1053, 1);
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, true); // piety
                                p.getPrayers().setPrayerActive(0, false); // thick skin
                                p.getPrayers().setPrayerActive(5, false); // rock skin
                                p.getPrayers().setPrayerActive(13, false); // steel skin
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                            }
                            else if (p.getPrayers().getSuperPrayer() == 2)
                            {
                                p.getPrayers().setSuperPrayer(0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPrayers().setPrayerActive(26, false);
                            }
                            break;
                    }
                    break;

                case 6: // Protect item.
                    switch (prayerID)
                    {
                        case 1:
                            p.getPrayers().setProtectItem(!p.getPrayers().isProtectItem());
                            p.getPackets().sendConfig(91, p.getPrayers().isProtectItem() ? 1 : 0);
                            p.getPrayers().setPrayerActive(10, p.getPrayers().isProtectItem());
                            break;
                    }
                    break;

                case 7: // Rapid restore & Rapid heal.
                    switch (prayerID)
                    {
                        case 1: // Rapid restore.
                            p.getPrayers().setRapidRestore(!p.getPrayers().isRapidRestore());
                            p.getPackets().sendConfig(89, p.getPrayers().isRapidRestore() ? 1 : 0);
                            p.getPrayers().setPrayerActive(8, p.getPrayers().isRapidRestore());
                            break;

                        case 2: // Rapid heal.
                            p.getPrayers().setRapidHeal(!p.getPrayers().isRapidHeal());
                            p.getPackets().sendConfig(90, p.getPrayers().isRapidHeal() ? 1 : 0);
                            p.getPrayers().setPrayerActive(9, p.getPrayers().isRapidRestore());
                            break;
                    }
                    break;

                case 8: // Ranged prayers.
                    switch (prayerID)
                    {
                        case 1: // Sharp Eye.
                            if (p.getPrayers().getRangePrayer() != 1)
                            {
                                p.getPrayers().setRangePrayer(1);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(862, 1);
                                p.getPrayers().setPrayerActive(3, true); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getRangePrayer() == 1)
                            {
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPrayers().setPrayerActive(3, false);
                            }
                            break;

                        case 2: // Hawk Eye.
                            if (p.getPrayers().getRangePrayer() != 2)
                            {
                                p.getPrayers().setRangePrayer(2);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(864, 1);
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, true); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getRangePrayer() == 2)
                            {
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPrayers().setPrayerActive(11, false);
                            }
                            break;

                        case 3: // Eagle Eye.
                            if (p.getPrayers().getRangePrayer() != 3)
                            {
                                p.getPrayers().setRangePrayer(3);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(866, 1);
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, true); // eagle eye
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                            }
                            else if (p.getPrayers().getRangePrayer() == 3)
                            {
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPrayers().setPrayerActive(20, false);
                            }
                            break;
                    }
                    break;

                case 9: // Magic prayers.
                    switch (prayerID)
                    {
                        case 1: // Mystic Will.
                            if (p.getPrayers().getMagicPrayer() != 1)
                            {
                                p.getPrayers().setMagicPrayer(1);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(863, 1);
                                p.getPrayers().setPrayerActive(4, true); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                            }
                            else if (p.getPrayers().getMagicPrayer() == 1)
                            {
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPrayers().setPrayerActive(4, false);
                            }
                            break;

                        case 2: // Mystic Lore.
                            if (p.getPrayers().getMagicPrayer() != 2)
                            {
                                p.getPrayers().setMagicPrayer(2);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPackets().sendConfig(865, 1);
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, true); // mystic lore
                                p.getPrayers().setPrayerActive(21, false); // mystic might
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                            }
                            else if (p.getPrayers().getMagicPrayer() == 2)
                            {
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPrayers().setPrayerActive(12, false);
                            }
                            break;

                        case 3: // Mystic Might.
                            if (p.getPrayers().getMagicPrayer() != 3)
                            {
                                p.getPrayers().setMagicPrayer(3);
                                p.getPrayers().setSuperPrayer(0);
                                p.getPrayers().setAttackPrayer(0);
                                p.getPrayers().setStrengthPrayer(0);
                                p.getPrayers().setRangePrayer(0);
                                p.getPackets().sendConfig(84, 0);
                                p.getPackets().sendConfig(87, 0);
                                p.getPackets().sendConfig(93, 0);
                                p.getPackets().sendConfig(85, 0);
                                p.getPackets().sendConfig(88, 0);
                                p.getPackets().sendConfig(94, 0);
                                p.getPackets().sendConfig(1053, 0);
                                p.getPackets().sendConfig(1052, 0);
                                p.getPackets().sendConfig(862, 0);
                                p.getPackets().sendConfig(864, 0);
                                p.getPackets().sendConfig(866, 0);
                                p.getPackets().sendConfig(863, 0);
                                p.getPackets().sendConfig(865, 0);
                                p.getPackets().sendConfig(867, 1);
                                p.getPrayers().setPrayerActive(4, false); // mystic will
                                p.getPrayers().setPrayerActive(12, false); // mystic lore
                                p.getPrayers().setPrayerActive(21, true); // mystic might
                                p.getPrayers().setPrayerActive(25, false); // chivalry
                                p.getPrayers().setPrayerActive(26, false); // piety
                                p.getPrayers().setPrayerActive(2, false); // clarity of thought
                                p.getPrayers().setPrayerActive(7, false); // improved reflexes
                                p.getPrayers().setPrayerActive(15, false); // incredible reflexes
                                p.getPrayers().setPrayerActive(1, false); // burst of strength
                                p.getPrayers().setPrayerActive(6, false); // superhuman strength
                                p.getPrayers().setPrayerActive(14, false); // ultimate strength
                                p.getPrayers().setPrayerActive(3, false); // sharp eye
                                p.getPrayers().setPrayerActive(11, false); // hawk eye
                                p.getPrayers().setPrayerActive(20, false); // eagle eye
                            }
                            else if (p.getPrayers().getMagicPrayer() == 3)
                            {
                                p.getPrayers().setMagicPrayer(0);
                                p.getPackets().sendConfig(867, 0);
                                p.getPrayers().setPrayerActive(21, false);
                            }
                            break;
                    }
                    break;
            }
            if (!usingPrayer && isPrayerActive(p))
            { // we werent using a prayer but we are now
                startPrayerDrainEvent(p);
            }
        }
    }
}