using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class Animations
    {
        public Animations()
        {
        }

        public static int getAttackAnimation(Player p)
        {
            ItemData.Item weaponDef = p.getEquipment().getSlot(ItemData.EQUIP.WEAPON).getDefinition();
            if (weaponDef == null) return 422;
            string weapon = weaponDef.getName();

            if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) <= 0)
            {
                return 422;
            }
            if (weapon.Contains("whip"))
            {
                return 1658;
            }
            if (weapon.Contains("2h") || weapon.Contains("godsword") || weapon.Contains("Saradomin sword"))
            {
                return 7041;
            }
            if (weapon.Contains("shortbow") || weapon.Contains("longbow") || weapon.Contains("Crystal bow") || weapon.Contains("Dark bow"))
            {
                return 426;
            }
            if (weapon.EndsWith("crossbow") && !weapon.Contains("Karil's"))
            {
                return 4230;
            }
            if (weapon.EndsWith("xil-ul"))
            {
                return 2614;
            }
            if (weapon.Contains("Karil"))
            {
                return 2075;
            }
            if (weapon.Contains("claws"))
            {
                return 451;
            }
            if (weapon.Contains("halberd"))
            {
                return 440;
            }
            if (weapon.Contains("battleaxe"))
            {
                return 395;
            }
            if (weapon.Contains("pickaxe") || weapon.Contains("mace") || weapon.Contains("warhammer") || weapon.Contains("staff") || weapon.Contains("Staff") || weapon.Contains("wand"))
            {
                return 393;
            }
            if (weapon.Contains("Granite maul"))
            {
                return 1665;
            }
            if (weapon.Contains("dagger"))
            {
                return 376;
            }
            if (weapon.Contains("Dharok"))
            {
                return 2067;
            }
            if (weapon.Contains("hammer"))
            {
                return 2068;
            }
            if (weapon.Contains("flail"))
            {
                return 2062;
            }
            if (weapon.Contains("Guthan") || weapon.Contains("spear"))
            {
                return 2080;
            }
            if (weapon.Contains("thrownaxe"))
            {
                return 385;
            }
            if (weapon.Contains("mace"))
            {
                return 393;
            }
            if (!weapon.Contains("Dragon") && weapon.Contains("longsword") || weapon.EndsWith(" sword"))
            {
                return 400;
            }
            if (weapon.Contains("Dragon longsword") || weapon.Contains("scimitar"))
            {
                return 451;
            }
            return 422;
        }

        public static int getAttackSpeed(Player p)
        {
            ItemData.Item weaponDef = p.getEquipment().getSlot(ItemData.EQUIP.WEAPON).getDefinition();
            if (weaponDef == null) return 5;
            string weapon = weaponDef.getName();
            if (weapon.Contains("dart") || weapon.Contains("knife"))
            {
                return 3;
            }
            if (weapon.Contains("whip") || weapon.Contains("dagger") || weapon.EndsWith(" sword") || weapon.Contains("scimitar") || weapon.Contains("claws")
                    || weapon.Contains("Toktz-xil-ak") || weapon.Contains("Toktz-xil-ek") || weapon.Contains("Saradomin sword") || weapon.Contains("Saradomin staff")
                    || weapon.Contains("Guthix staff") || weapon.Contains("Zamorak staff") || weapon.Contains("Slayer") || weapon.Contains("ancient") || weapon.Contains("shortbow")
                    || weapon.Contains("Karil") || weapon.Contains("Toktz-xil-ul"))
            {
                return 4;
            }
            if (weapon.Contains("longsword") || weapon.Contains("mace") || weapon.EndsWith(" axe") || (weapon.Contains("spear") && !weapon.Contains("Guthan")) || weapon.Contains("pickaxe")
                    || weapon.Contains("Tzhaar-ket-em") || weapon.Contains("hammer") || weapon.Contains("flail")
                    || (weapon.Contains("staff") && !weapon.Contains("Guthix") && !weapon.Contains("Saradomin") && !weapon.Contains("Zamorak") && !weapon.Contains("Slayer")) || weapon.Contains("Staff")
                    || weapon.Contains("Iban") || weapon.Contains("composite") || weapon.Contains("Seercull") || weapon.Contains("Crystal") || weapon.Contains("thrownaxe"))
            {
                return 5;
            }
            if (weapon.EndsWith("battleaxe") || weapon.Contains("warhammer") || weapon.Contains("godsword") || weapon.Contains("Toktz-mej-tal") || weapon.Contains("Ahrim")
                    || weapon.Contains("Zuriel") || weapon.Contains("longbow") || (weapon.EndsWith("crossbow") && !weapon.Contains("Karil")) || weapon.Contains("javelin"))
            {
                return 6;
            }
            if (weapon.Contains("Guthan") || weapon.Contains("2h") || weapon.Contains("halberd") || weapon.Contains("Granite maul") || weapon.Contains("Tzhaar-ket-om") || weapon.Contains("Dharok"))
            {
                return 7;
            }
            if (weapon.Contains("ogre"))
            {
                return 8;
            }
            if (weapon.Contains("Dark bow"))
            {
                return 9;
            }
            return 5;
        }

        public static int getDefenceAnimation(Player p)
        {
            int weaponId = p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            int shield = p.getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD);
            ItemData.Item weaponDef = p.getEquipment().getSlot(ItemData.EQUIP.WEAPON).getDefinition();
            if (weaponDef == null) return 404;
            string weapon = weaponDef.getName();

            if (shield != -1)
            {
                ItemData.Item shieldDef = p.getEquipment().getSlot(ItemData.EQUIP.SHIELD).getDefinition();
                if (shieldDef == null) return 1156;
                string shieldName = shieldDef.getName();
                if (shield >= 8844 && shield <= 8850)
                { // Defenders
                    return 4177;
                }
                if (shieldName.Contains("book") || shieldName.Contains("Book"))
                {
                    return 404;
                }
                return 1156;
            }
            if (weaponId <= 0)
            {
                return 424;
            }
            if (weapon.Contains("xil-ul"))
            {
                return 425;
            }
            if (weapon.EndsWith("whip"))
            {
                return 1659;
            }
            if (weapon.Contains("Granite maul"))
            {
                return 1666;
            }
            if (weapon.Contains("Dharok") || weapon.Contains("flail"))
            {
                return 2063;
            }
            if (weapon.Contains("shortbow") || weapon.Contains("longbow") || weapon.Contains("Karil") || weapon.Contains("Crystal") || weapon.Contains("Dark bow"))
            {
                return 425;
            }
            if (weapon.Contains("2h") || weapon.Contains("godsword") || weapon.Contains("Saradomin sword"))
            {
                return 7050;
            }
            if (weapon.Contains("staff") || weapon.Contains("Staff") || weapon.Contains("halberd")
                || weapon.Contains("warspear") || weapon.Contains("spear"))
            {
                return 420;
            }
            if (weapon.Contains("claws"))
            {
                return 4177;
            }
            if (weapon.Contains("wand") || weapon.Contains("longsword") || weapon.EndsWith("_sword")
                || weapon.Contains("battleaxe") || weapon.Contains("mace") || weapon.Contains("scimitar")
                || weapon.Contains("axe") || weapon.Contains("warhammer") || weapon.Contains("dagger"))
            {
                return 397;
            }
            return 404;
        }

        public static int getNPCHitDelay(Npc npc)
        {
            /*switch (npc.getId())
            {
            }*/
            return 450;
        }

        public static int getPlayerHitDelay(Player p)
        {
            /*switch (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON))
            {
            }*/
            return 400;
        }

        public static int getStandAnim(Player p)
        {
            int id = p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            string weapon = ItemData.forId(id).getName();
            if (weapon.Contains("Dharok"))
            {
                return 2065;
            }
            if (weapon.Contains("flail"))
            {
                return 2061;
            }
            if (weapon.Contains("Karil"))
            {
                return 2074;
            }
            if (weapon.Contains("Tzhaar-ket-om"))
            {
                return 0x811;
            }
            if (weapon.Equals("Saradomin staff") || weapon.Equals("Guthix staff") || weapon.Equals("Zamorak staff"))
            {
                return 0x328;
            }
            if (weapon.Contains("Guthan") || weapon.EndsWith("spear") || weapon.EndsWith("halberd") || weapon.Contains("Staff") || weapon.Contains("staff") || weapon.Contains("wand") || weapon.Contains("Dragon longsword") || weapon.Equals("Void knight mace"))
            {
                return 809;
            }
            if (weapon.Contains("2h") || weapon.EndsWith("godsword") || weapon.Equals("Saradomin sword"))
            {
                return 7047;
            }
            if (weapon.Equals("Abyssal whip"))
            {
                return 10080;
            }
            if (weapon.Contains("Granite maul"))
            {
                return 1662;
            }
            return 808;
        }

        public static int getWalkAnim(Player p)
        {
            int id = p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            string weapon = ItemData.forId(id).getName();
            if (weapon.Equals("Saradomin staff") || weapon.Equals("Guthix staff") || weapon.Equals("Zamorak staff"))
            {
                return 0x333;
            }
            if (weapon.Contains("flail"))
            {
                return 2060;
            }
            if (weapon.Contains("Karil"))
            {
                return 2076;
            }
            if (weapon.Contains("Granite maul"))
            {
                return 1663;
            }
            if (weapon.Equals("Abyssal whip"))
            {
                return 1660;
            }
            if (id == 4718 || weapon.EndsWith("2h sword") || weapon.Contains("Tzhaar-ket-om") || weapon.EndsWith("godsword") || weapon.Equals("Saradomin sword"))
            {
                return 7046;
            }
            if (weapon.Contains("Guthan") || weapon.Contains("spear") || weapon.EndsWith("halberd") || weapon.Contains("Staff") || weapon.Contains("staff") || weapon.Contains("wand") || weapon.Equals("Void knight mace"))
            {
                return 1146;
            }
            return 819;
        }

        public static int getRunAnim(Player p)
        {
            int id = p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON);
            string weapon = ItemData.forId(id).getName();
            if (weapon.Contains("Dharok") || weapon.EndsWith("2h sword") || weapon.Contains("Tzhaar-ket-om") || weapon.EndsWith("godsword") || weapon.Equals("Saradomin sword"))
            {
                return 7039;
            }
            if (weapon.Equals("Abyssal whip"))
            {
                return 1661;
            }
            if (weapon.Contains("Granite maul"))
            {
                return 1664;
            }
            if (weapon.Contains("flail"))
            {
                return 1831;
            }
            if (weapon.Contains("Karil"))
            {
                return 2077;
            }
            if (weapon.Contains("Saradomin staff") || weapon.Contains("Guthix staff") || weapon.Contains("Zamorak staff") || weapon.Equals("Void knight mace"))
            {
                return 824;
            }
            if (weapon.Contains("staff") || weapon.Contains("Staff") || weapon.Contains("halberd")
                || weapon.Contains("wand") || weapon.Contains("Dragon longsword") || weapon.Contains("warspear"))
            {
                return 1210;
            }
            return 824;
        }
    }
}