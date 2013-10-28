using RS2.Server.definitions;

namespace RS2.Server.player.skills.magic
{
    internal class MagicData
    {
        public MagicData()
        {
        }

        protected static int MAGIC = 6;

        protected static int[] RUNE_REPLACER = {
		    1387, // Staff of fire.
		    1381, // Staff of air.
		    1383, // Staff of water.
		    1385, // Staff of earth.
		    1393, // Fire battlestaff.
		    1395, // Water battlestaff.
		    1397, // Air battlestaff.
		    1399, // Earth battlestaff.
		    1401, // Mystic fire staff.
		    1403, // Mystic water staff.
		    1405, // Mystic air staff.
		    1407, // Mystic earth staff.
		    3053, // Lava battlestaff.
		    3053, // Lava battlestaff.
		    3054, // Mystic lava staff.
		    3054, // Mystic lava staff.
		    6562, // Mud battlestaff.
		    6562, // Mud battlestaff.
		    6563, // Mystic mud staff.
		    6563, // Mystic mud staff.
		    11736, // Steam battlstaff.
		    11736, // Steam battlstaff.
		    11738, // Mystic steam staff.
		    11738, // Mystic steam staff.
	    };

        protected static int[] REPLACABLE_RUNES = {
		    554, // Fire rune.
		    556, // Air rune.
		    555, // Water rune.
		    557, // Earth rune.
		    554, // Fire rune.
		    555, // Water rune.
		    556, // Air rune.
		    557, // Earth rune.
		    554, // Fire rune.
		    555, // Water rune.
		    556, // Air rune.
		    557, // Earth rune.
		    557, // Earth rune.
		    554, // Fire rune.
		    557, // Earth rune.
		    554, // Fire rune.
		    555, // Water rune.
		    557, // Earth rune.
		    555, // Water rune.
		    557, // Earth rune.
		    554, // Fire rune.
		    555, // Water rune.
		    554, // Fire rune.
		    555, // Water rune.
	    };

        protected static int[][] TELEPORT_RUNES = {
		    new int[] {554, 556, 563},// Varrock
		    new int[] {557, 556, 563},// Lumbridge
		    new int[] {555, 556, 563},// Falador
		    new int[] {556, 563},	// Seers/Camelot
		    new int[] {555, 563},	// Ardougne
		    new int[] {557, 563},	// Watchtower
		    new int[] {563, 554},	// Trollheim
		    new int[] {563, 554, 556},// Paddewwa
		    new int[] {563, 566},	// Sennestien
		    new int[] {563, 565},	// Kharyrll
		    new int[] {563, 555},	// Lassar
		    new int[] {563, 554, 556},// Dareeyak
		    new int[] {563, 566},	// Carrallanger
		    new int[] {563, 565},	// Annakarl
		    new int[] {563, 555},	// Ghorrock
	    };

        protected static int[][] TELEPORT_RUNES_AMOUNT = {
		    new int[] {1, 3, 1},
		    new int[] {1, 3, 1},
		    new int[] {1, 3, 1},
		    new int[] {5, 1},
		    new int[] {2, 2},
		    new int[] {2, 2},
		    new int[] {2, 2},
		    new int[] {2, 1, 1},
		    new int[] {2, 1},
		    new int[] {2, 1},
		    new int[] {2, 4},
		    new int[] {2, 3, 2},
		    new int[] {2, 2},
		    new int[] {2, 2},
		    new int[] {2, 8},
	    };

        protected static int[] TELE_X = { 3211, 3221, 2963, 2756, 2660, 2551, 2891, 3096, 3332, 3491, 2845, 2962, 3221, 3286, 2948 };
        protected static int[] TELE_Y = { 3423, 3217, 3378, 3478, 3306, 3113, 3679, 9881, 3345, 3482, 3487, 3695, 3663, 3883, 3930 };
        protected static int[] TELE_EXTRA_X = { 4, 1, 4, 3, 2, 2, 1, 4, 2, 2, 2, 2, 1, 1, 4 };
        protected static int[] TELE_EXTRA_Y = { 2, 3, 2, 2, 3, 3, 1, 2, 2, 3, 3, 3, 5, 2, 5 };
        protected static double[] TELEPORT_XP = { 35, 41, 48, 55.5, 61, 68, 68, 64, 70, 76, 82, 88, 82, 100, 106 };
        protected static int[] TELEPORT_LVL = { 25, 31, 37, 45, 51, 58, 61, 54, 60, 66, 72, 78, 84, 90, 96 };

        protected static int[] HOME_TELE = { 2330, 3170, 2, 3 };

        protected static int[] HOME_ANIMATIONS = { 1722, 1723, 1724, 1725, 2798, 2799, 2800, 3195, 4643, 4645, 4646, 4847, 4848, 4849, 4850, 4851, 4852 };
        protected static int[] HOME_GRAPHICS = { 775, 800, 801, 802, 803, 804, 1703, 1704, 1705, 1706, 1707, 1708, 1709, 1710, 1711, 1712, 1713 };

        protected static int[] TELETABS = { 8007, 8008, 8009, 8010, 8011, 8012 };

        protected static int[] CHARGE_RUNES = { 554, 565, 556 };
        protected static int[] CHARGE_RUNE_AMOUNT = { 3, 3, 3 };

        protected static int[] SPELL_LEVEL = {
            1, 5, 9, 13, 17, 23, 29, 35, 41, 47,
            53, 59, 62, 65, 70, 75, 50, 52, 56, 58,
            62, 64, 68, 70, 74, 76, 80, 82, 86, 88,
            92, 94, 39, 50, 20, 50, 50, 60, 60, 60,
            79, 85, 3, 11, 19, 73, 80, 66, 85, 61, 73,
            85, 97,
        };

        protected static int[] HANDS_GFX = {
            90, 93, 96, 99, 117, 120, 123, 126, 132, 135,
            138, 129, 158, 161, 164, 155, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, 366, -1, -1,
            -1, -1, 145, -1, 177, 87, 177, -1, -1, -1,
            177, 108, 102, 170, 167, 173, 105, 1841, 1845,
            1848, 1850, 1853
        };

        protected static int[] AIR_GFX = {
            91, 94, 97, 100, 118, 121, 124, 127, 133, 136,
            139, 130, 159, 162, 165, 156, 384, 378, 374, 360,
            -1, -1, -1, -1, 384, 380, 374, -1, -1, -1,
            -1, -1, 146, 328, 178, 88, 178, -1, -1, -1,
            178, 109, 103, 171, 168, 174, 106, 1842, -1,
            -1, -1, -1
        };

        protected static int[] END_GFX = {
            92, 95, 98, 101, 119, 122, 125, 128, 134, 137,
            140, 131, 160, 163, 166, 157, 385, 379, 373, 361,
            387, 382, 375, 363, 390, 381, 375, 367, 388, 383,
            376, 369, 147, 329, 179, 89, 180, 76, 77, 78,
            181, 110, 104, 172, 169, 107, 107, 1843, 1847, 1849,
            1851, 1854
        };

        protected static int[] SPELL_ANIM = {
            711, 711, 711, 711, 711, 711, 711, 711, 711, 711,
            711, 711, 727, 727, 727, 727, 1978, 1978, 1978, 1978,
            1979, 1979, 1979, 1979, 1978, 1978, 1978, 1978, 1979, 1979,
            1979, 1979, 724, 1576, 710, 708, 710, 811, 811, 811,
            710, 724, 729, 724, 724, 729, 729, 10503, 10513, 10516,
            10524, 10518
        };

        protected static int[][] RUNES = {
    	    new int[] {556, 558},// Wind strike.
    	    new int[] {555, 556, 558},// Water strike.
    	    new int[] {557, 556, 558},// Earth strike.
    	    new int[] {554, 556, 558},// Fire strike.
    	    new int[] {556, 562},// Wind bolt.
    	    new int[] {555, 556, 562},// Water bolt.
    	    new int[] {557, 556, 562},// Earth bolt.
    	    new int[] {554, 556, 562},// Fire bolt.
    	    new int[] {556, 560},// Wind blast.
    	    new int[] {555, 556, 560},// Water blast.
    	    new int[] {557, 556, 560},// Earth blast.
    	    new int[] {554, 556, 560},// Fire blast
    	    new int[] {556, 565},// Wind wave.
    	    new int[] {555, 556, 565},// Water wave.
    	    new int[] {557, 556, 565},// Earth wave.
    	    new int[] {554, 556, 565},// Fire wave.
    	    new int[] {562, 560, 554, 556},// Smoke rush.
    	    new int[] {562, 560, 554, 566},// Shadow rush.
    	    new int[] {562, 560, 565},// Blood rush.
    	    new int[] {562, 560, 555},// Ice rush.
    	    new int[] {562, 560, 554, 556},// Smoke Blitz.
    	    new int[] {562, 560, 554, 566},// Shadow Blitz.
    	    new int[] {562, 560, 565},// Blood Blitz.
    	    new int[] {562, 560, 555},// Ice Blitz.
    	    new int[] {560, 565, 554, 556},// Smoke blitz.
    	    new int[] {560, 565, 554, 566},// Shadow blitz.
    	    new int[] {560, 565},// Blood blitz.
    	    new int[] {560, 565, 555},// Ice blitz.
    	    new int[] {560, 565, 554, 556},// Smoke Barr..
    	    new int[] {560, 565, 556, 566},// Shadow Barr..
    	    new int[] {560, 565, 566},// Blood Barr..
    	    new int[] {560, 565, 555},// Ice Barr..
    	    new int[] {557, 556, 562},// Crumble Undead.
    	    new int[] {560, 558},// Slayer Dart.
    	    new int[] {557, 555, 561},// Bind.
    	    new int[] {554, 560},// Iban Blast.
    	    new int[] {557, 555, 561},// Snare.
    	    new int[] {554, 565, 556},// Saradomin Strike.
    	    new int[] {554, 565, 556},// Claws of Guthix.
    	    new int[] {554, 565, 556},// Flames of Zamorak.
    	    new int[] {557, 555, 561},// Entangle.
    	    new int[] {555, 557, 559},// Confuse.
    	    new int[] {555, 557, 559},// Weaken.
    	    new int[] {555, 557, 559},// Curse.
    	    new int[] {557, 555, 566},// Enfeeble.
    	    new int[] {557, 555, 566},// Stun.
    	    new int[] {557, 555, 566},// Vulnerability.
    	    new int[] {562, 563, 560},// Teleblock.
    	    new int[] {562, 557, 566}, // Miasmic rush.
    	    new int[] {562, 557, 566}, // Miasmic burst.
    	    new int[] {565, 557, 566}, // Miasmic blitz.
    	    new int[] {565, 557, 566} // Miasmic barrage.
        };

        protected static int[][] RUNE_AMOUNTS = {
    	    new int[] {1, 1},// Wind strike
    	    new int[] {1, 1, 1},// Water strike
    	    new int[] {2, 1, 1},// Earth strike
    	    new int[] {3, 1, 1},// Fire strike
    	    new int[] {1, 1},// Wind bolt
    	    new int[] {2, 2, 1},// Water bolt
    	    new int[] {3, 2, 1},// Earth bolt
    	    new int[] {3, 2, 1},// Fire bolt.
    	    new int[] {3, 1},// Wind blast
    	    new int[] {3, 3, 1},// Water blast
    	    new int[] {4, 3, 1},// Earth blast.
    	    new int[] {5, 4, 1},// Fire blast.
    	    new int[] {5, 1},// Wind wave.
    	    new int[] {7, 5, 1},// Water wave.
    	    new int[] {7, 5, 1},// Earth wave.
    	    new int[] {7, 5, 1},// Fire wave.
    	    new int[] {2, 2, 1, 1},// Smoke rush.
    	    new int[] {2, 2, 1, 1},// Shadow rush.
    	    new int[] {2, 2, 1},// Blood rush.
    	    new int[] {2, 2, 2},// Ice rush.
    	    new int[] {4, 2, 2, 2},// Smoke Blitz.
    	    new int[] {4, 2, 1, 2},// Shadow Blitz.
    	    new int[] {4, 2, 2},// Blood Blitz.
    	    new int[] {4, 2, 4},// Ice Blitz.
    	    new int[] {2, 2, 2, 2},// Smoke blitz.
    	    new int[] {2, 2, 2, 2},// Shadow blitz.
    	    new int[] {2, 4},// Blood blitz.
    	    new int[] {2, 2, 3},// Ice blitz.
    	    new int[] {4, 2, 4, 4},// Smoke Barr..
    	    new int[] {4, 2, 4, 3},// Shadow Barr..
    	    new int[] {4, 4, 1},// Blood Barr..
    	    new int[] {4, 2, 6},// Blood Barr..
    	    new int[] {2, 2, 1},// Crumble undead.
    	    new int[] {1, 4},// Slayer dart.
    	    new int[] {3, 3, 2},// Bind.
    	    new int[] {5, 1},// Iban Blast.
    	    new int[] {4, 4, 3},// Snare.
    	    new int[] {2, 2, 4},// Saradomin Strike.
    	    new int[] {1, 2, 4},// Claws of Guthix.
    	    new int[] {4, 2, 1},// Flames of Zamorak.
    	    new int[] {5, 5, 4},// Entangle.
    	    new int[] {3, 2, 1},// Confuse.
    	    new int[] {3, 2, 1},// Weaken.
    	    new int[] {2, 3, 1},// Curse.
    	    new int[] {8, 8, 1},// Enfeeble.
    	    new int[] {12, 12, 1},// Stun.
    	    new int[] {5, 5, 1},// Vulnerability.
    	    new int[] {1, 1, 1},// Teleblock.
    	    new int[] {2, 1, 1}, // Miasmic rush.
    	    new int[] {4, 1, 1}, // Miasmic burst.
    	    new int[] {2, 3, 3}, // Miasmic blitz.
    	    new int[] {4, 4, 4} // Miasmic barrage.
        };

        protected static int[] STAFF = {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 4170, 0, 1409, 0, 2415, 2416, 2417,
            0, 0, 0, 0, 0, 0, 0, 0, 13867, 13867, 13867,
            13867
        };

        protected static bool[] NEEDS_STAFF = {
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, true, false, true, false, true, true, true,
            false, false, false, false, false, false, false, false, true, true,
            true, true
        };

        protected static string[] STAFF_NAME = {
            "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "",
            "", "", "", "a Slayer's staff", "", "Iban's staff", "", "the Staff of Saradomin", "the Staff of Guthix", "the Staff of Zamorak",
            "", "", "", "", "", "", "", "", "Zuriel's staff", "Zuriel's staff", "Zuriel's staff", "Zuriel's staff"
        };

        protected static int[] AUTOCAST_CONFIG = {
    	    45, 47, 49, 51, // Strike spells.
    	    53, 55, 57, 59, // Bolt spells.
    	    61, 63, 65, 67, // Blast spells.
    	    69, 71, 73, 75, // Wave spells.
    	    13, 15, 17, 19, // Rush spells.
    	    21, 23, 25, 27, // Blitz spells.
    	    29, 31, 33, 35, // Blitz spells.
    	    37, 39, 41, 43, // Barrage spells.
    	    77, // Crumble undead.
    	    79, // Slayer dart.
    	    81, // Guthix claws.
        };

        protected static string[] AUTOCAST_NAME = {
    	    "Wind Strike", "Water Strike", "Earth Strike", "Fire Strike",
    	    "Wind Bolt", "Water Bolt", "Earth Bolt", "Fire Bolt",
    	    "Wind Blast", "Water Blast", "Earth Blast", "Fire Blast",
    	    "Wind Wave", "Water Wave", "Earth Wave", "Fire Wave",
    	    "Smoke Rush", "Shadow Rush", "Blood Rush", "Ice Rush",
    	    "Smoke Burst", "Shadow Burst", "Blood Burst", "Ice Burst",
    	    "Smoke Blitz", "Shadow Blitz", "Blood Blitz", "Ice Blitz",
    	    "Smoke Barr.", "Shadow Barr.", "Blood Barr.", "Ice Barr.",
    	    "Crumble Und.", "Slayer dart", "Guthix Claws"
        };

        public static void configureSelectSpellInterface(Player p)
        {
            p.removeTemporaryAttribute("autoCasting");
            if (p.getMagicType() == 1)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 4170)
                { // Slayer staff.
                    p.getPackets().sendTab(83, 310);
                    return;
                }
                else if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 8841)
                { // Void knight mace.
                    p.getPackets().sendTab(83, 406);
                    return;
                }
                else
                {
                    p.getPackets().sendTab(83, 319);
                }
            }
            else if (p.getMagicType() == 2)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == 4675)
                { // Ancient staff.
                    p.getPackets().sendTab(83, 388);
                }
                else
                {
                    p.getPackets().sendMessage("You cannot autocast Ancient Magic with this staff.");
                }
            }
        }

        public static void cancelAutoCast(Player p, bool cancelCast)
        {
            if (cancelCast)
            {
                p.getEquipment().setWeapon();
                //p.getPackets().sendConfig(43, 0);
                resetSpellIcon(p);
                p.removeTemporaryAttribute("autoCastSpell");
                p.removeTemporaryAttribute("autoCastAncients");
                p.getPackets().showChildInterface(90, 83, true);
                p.getPackets().modifyText("Spell", 90, 11);
                return;
            }
            if (p.getTemporaryAttribute("autoCastSpell") != null)
            {
                setAutoCastSpell(p, (int)p.getTemporaryAttribute("autoCastSpell2"), (int)p.getTemporaryAttribute("autoCastSpell"), (bool)(p.getTemporaryAttribute("autoCastAncients") != null ? true : false));
            }
            else
            {
                //p.getPackets().sendConfig(43, 0);
                resetSpellIcon(p);
                p.getEquipment().setWeapon();
                p.getPackets().showChildInterface(90, 83, true);
                p.getPackets().modifyText("Spell", 90, 11);
                p.removeTemporaryAttribute("autoCastSpell");
                p.removeTemporaryAttribute("autoCastAncients");
                p.removeTemporaryAttribute("autoCastSpell2");
            }
        }

        private static void resetSpellIcon(Player p)
        {
            for (int i = 0; i < AUTOCAST_CONFIG.Length; i++)
            {
                p.getPackets().showChildInterface(90, AUTOCAST_CONFIG[i], false);
            }
        }

        public static void setAutoCastSpell(Player p, int spell, int spellIndex, bool ancients)
        {
            p.getPackets().modifyText(AUTOCAST_NAME[spell], 90, 11);
            p.getPackets().showChildInterface(90, 83, false);
            p.getPackets().showChildInterface(90, AUTOCAST_CONFIG[spell], true);
            p.getPackets().sendConfig(43, 3);
            p.getPackets().sendTab(83, 90);
            p.getPackets().modifyText(p.getEquipment().getSlot(ItemData.EQUIP.WEAPON).getDefinition().getName(), 90, 0);
            p.setTemporaryAttribute("autoCastSpell", spellIndex);
            p.setTemporaryAttribute("autoCastSpell2", spell);
            if (ancients)
            {
                p.setTemporaryAttribute("autoCastAncients", ancients);
            }
            else
            {
                p.removeTemporaryAttribute("autoCastAncients");
            }
        }

        public static bool hasRunes(Player p, int[] runes, int[] amount)
        {
            for (int i = 0; i < runes.Length; i++)
            {
                if (checkForStaffs(p, runes[i]) != -1)
                {
                    if (!p.getInventory().hasItemAmount(runes[i], amount[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static int checkForStaffs(Player p, int rune)
        {
            for (int i = 0; i < RUNE_REPLACER.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == RUNE_REPLACER[i])
                {
                    if (rune == REPLACABLE_RUNES[i])
                    {
                        rune = -1;
                        break;
                    }
                }
            }
            return rune;
        }

        public static bool deleteRunes(Player p, int[] runes, int[] amount)
        {
            for (int i = 0; i < runes.Length; i++)
            {
                if (checkForStaffs(p, runes[i]) != -1)
                {
                    if (!p.getInventory().deleteItem(runes[i], amount[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}