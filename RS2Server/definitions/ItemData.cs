using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class ItemData
    {
        public enum EQUIP
        {
            HAT = 0,
            CAPE = 1,
            AMULET = 2,
            WEAPON = 3,
            CHEST = 4,
            SHIELD = 5,
            LEGS = 7,
            HANDS = 9,
            FEET = 10,
            RING = 12,
            ARROWS = 13,
            NOTHING = -1
        };

        public class ItemPrice
        {
            public int minPrice = 1;
            public int maxPrice = 3;
            public int normPrice = 2;

            public int getMinimumPrice()
            {
                return minPrice;
            }

            public void setMinimumPrice(int minPrice)
            {
                this.minPrice = minPrice;
            }

            public int getMaximumPrice()
            {
                return maxPrice;
            }

            public void setMaximumPrice(int maxPrice)
            {
                this.maxPrice = maxPrice;
            }

            public int getNormalPrice()
            {
                return (maxPrice + minPrice) / 2;
            }

            public void setNormalPrice(int normPrice)
            { //pretty useless it's calculated above.
                this.normPrice = normPrice;
            }
        }

        public class Item
        {
            public int id;
            public string name = "null";
            public string examine = "null";
            public bool stackable = false;
            public bool noted = false;
            public int equipId = -1;
            public bool playerBound = false;
            public int animation = 1426;
            public ItemPrice price;
            public int[] bonus = new int[13];

            public ItemPrice getPrice()
            {
                return price;
            }

            public void setPrice(ItemPrice price)
            {
                this.price = price;
            }

            public int[] getBonuses()
            {
                return bonus;
            }

            public int getBonus(int id)
            {
                return bonus[id];
            }

            public void setBonus(int id, int value)
            {
                bonus[id] = value;
            }

            public string getExamine()
            {
                return examine;
            }

            public void setExamine(string examine)
            {
                this.examine = examine;
            }

            public int getId()
            {
                return id;
            }

            public void setId(int id)
            {
                this.id = id;
            }

            public bool isStackable()
            {
                return stackable;
            }

            public void setStackable(bool stackable)
            {
                this.stackable = stackable;
            }

            public string getName()
            {
                return name;
            }

            public void setName(string name)
            {
                this.name = name;
            }

            public bool isNoted()
            {
                return noted;
            }

            public void setNoted(bool noted)
            {
                this.noted = noted;
            }

            public int getEquipId()
            {
                return equipId;
            }

            public void setEquipId(int id)
            {
                equipId = id;
            }

            public bool isPlayerBound()
            {
                return playerBound;
            }

            public void setPlayerBound(bool playerBound)
            {
                this.playerBound = playerBound;
            }

            public int getAnimation()
            {
                return animation;
            }

            public void setAnimation(int animation)
            {
                this.animation = animation;
            }
        }

        /**
         * Items which contains a Item class for a single item.
         */
        public static Dictionary<int, Item> items;

        public static void load()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\itemData.xml"))
            {
                Misc.WriteError(@"missing data\itemData.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\itemData.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
                List<Item> list = (List<Item>)serializer.Deserialize(objStreamReader);

                items = new Dictionary<int, Item>();
                foreach (Item def in list)
                    items.Add(def.getId(), def);

                Console.WriteLine("Loaded " + items.Count + " item definitions.");
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
        }

        public static void setNotedItemPrices()
        {
            foreach (Item def in items.Values)
            {
                if (def.isNoted())
                {
                    int unNoted = getUnNotedItem(def.getId());
                    if (unNoted != -1)
                    {
                        def.setPrice(items[unNoted].getPrice());
                    }
                }
            }
        }

        public static Item forId(int itemId)
        {
            if (itemId > items.Count || itemId < -1)
                return null;

            return items[itemId];
        }

        public static int getNotedItem(int itemId)
        {
            if (itemId + 1 > items.Count)
                return -1;
            Item itemDef = items[itemId];
            Item nextItem = items[itemId + 1];
            if (nextItem.getName().ToLower().Equals(itemDef.getName().ToLower()))
            {
                if (nextItem.isStackable() && nextItem.isNoted())
                    return nextItem.getId();
            }

            foreach (Item item in items.Values)
            {
                if (item.getName().ToLower().Equals(itemDef.getName().ToLower()) && !item.Equals(itemDef))
                {
                    if (item.getExamine().StartsWith("Swap"))
                        return item.getId();
                }
            }
            return itemDef.getId();
        }

        public static int getUnNotedItem(int itemId)
        {
            if (itemId > items.Count)
                return -1;
            Item itemDef = items[itemId];
            Item previousItem = items[(itemId == 0 ? itemId : itemId - 1)];
            if (previousItem.getName().ToLower().Equals(itemDef.getName().ToLower()))
            {
                if (!previousItem.isStackable() && !previousItem.isNoted())
                {
                    return previousItem.getId();
                }
            }

            foreach (Item item in items.Values)
            {
                if (item.getName().ToLower().Equals(itemDef.getName().ToLower()) && !item.Equals(itemDef))
                {
                    if (!item.getExamine().StartsWith("Swap"))
                        return item.getId();
                }
            }
            return itemDef.getId();
        }

        /*
         * Unused
         */

        public static bool isNote(int itemId)
        {
            if (itemId > items.Count)
                return false;
            Item itemDef = items[itemId];
            Item previousItem = items[(itemId == 0 ? itemId : itemId - 1)];
            if (!itemDef.isStackable())
                return false;
            if (itemDef.getExamine().StartsWith("Swap"))
                return true;

            if (previousItem.getName().ToLower().Equals(itemDef.getName().ToLower()))
            {
                if (!previousItem.isStackable() && itemDef.isStackable())
                    return true;
            }
            return false;
        }

        public static bool isPlayerBound(int item)
        {
            for (int i = 0; i < PLAYER_BOUND_ITEMS.Length; i++)
            {
                if (item == PLAYER_BOUND_ITEMS[i])
                    return true;
            }
            return false;
        }

        public static bool isFullBody(Item def)
        {
            if (def == null) return false;
            string weapon = def.getName();
            for (int i = 0; i < FULL_BODY.Length; i++)
            {
                if (weapon.Contains(FULL_BODY[i]) || def.getId() == 544)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isFullHat(Item def)
        {
            if (def == null) return false;
            string weapon = def.getName();
            for (int i = 0; i < FULL_HAT.Length; i++)
            {
                if (weapon.EndsWith(FULL_HAT[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isFullMask(Item def)
        {
            if (def == null) return false;
            string weapon = def.getName();
            for (int i = 0; i < FULL_MASK.Length; i++)
            {
                if (weapon.EndsWith(FULL_MASK[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] CAPES = { "cape", "Cape", "cloak" };
        private static string[] HATS = { "Bunny", "sallet", "cowl", "helm", "hood", "tiara", "coif", "Coif", "hat", "partyhat", "Hat", "full helm (t)", "full helm (g)", "hat (t)", "hat (g)", "cav", "boater", "helmet", "mask", "Helm of neitiznot" };
        private static string[] BOOTS = { "boots", "Boots" };
        private static string[] GLOVES = { "gloves", "gauntlets", "Gloves", "vambraces", "vamb", "bracers" };
        private static string[] SHIELDS = { "Toktz-ket-xil", "kiteshield", "sq shield", "Toktz-ket", "books", "book", "kiteshield (t)", "kiteshield (g)", "kiteshield(h)", "defender", "shield" };
        private static string[] AMULETS = { "scarf", "stole", "amulet", "necklace", "Amulet of" };
        private static string[] ARROWS = { "arrow", "arrows", "bolts (e)", "bolt (e)", "arrow(p)", "arrow(+)", "arrow(s)", "bolt", "bolts", "Bolt rack", "Opal bolts", "Dragon bolts" };
        private static string[] RINGS = { "ring", "Ring" };
        private static string[] BODY = { "hauberk", "platebody", "chainbody", "robetop", "leathertop", "platemail", "top", "brassard", "Robe top", "body", "platebody (t)", "platebody (g)", "body(g)", "body_(g)", "chestplate", "torso", "shirt", "Runecrafter robe", };
        private static string[] LEGS = { "cuisse", "knight robe", "platelegs", "plateskirt", "skirt", "bottoms", "chaps", "platelegs (t)", "platelegs (g)", "bottom", "skirt", "skirt (g)", "skirt (t)", "chaps (g)", "chaps (t)", "tassets", "legs" };

        private static string[] WEAPONS = {"sceptre", "Tzhaar-Ket-Om","Excalibur","dark bow", "Pharaoh's","wand", "adze", "Karil's x-bow","warhammer","claws","scimitar","longsword","sword","longbow","shortbow","dagger","mace","halberd","spear",
	    "whip","axe","flail","crossbow","Torags hammer's","dagger(p)", "dagger(p+)","dagger(p++)","dagger(+)","dagger(s)","spear(p)","spear(+)",
	    "spear(s)","spear(kp)","maul","dart","dart(p)","javelin","javelin(p)","knife","knife(p)","Longbow","Shortbow",
	    "Crossbow","Toktz-xil","Toktz-mej","Tzhaar-ket","staff","Staff","godsword","c'bow","Crystal bow","Dark bow", "anchor"};

        /* Fullbody is an item that covers your arms. */
        private static string[] FULL_BODY = { "Morrigan's leather body", "hauberk", "Ghostly robe", "Monk's robe", "Granite", "Vesta", "Runecrafter robe", "top", "shirt", "platebody", "Ahrims robetop", "Karils leathertop", "brassard", "Robe top", "robetop", "platebody (t)", "platebody (g)", "chestplate", "torso", "Dragon chainbody" };
        /* Fullhat covers your head but not your beard. */
        private static string[] FULL_HAT = { "3rd age", "cowl", "Berserker", "med helm", "coif", "Dharoks helm", "hood", "Initiate helm", "Coif", "Helm of neitiznot" };
        /* Fullmask covers your entire head. */
        private static string[] FULL_MASK = { "sallet", "full helm(t)", "full helm(g)", "full helm", "mask", "Verac's helm", "Guthan's helm", "Torag's helm", "Karil's coif", "full helm (t)", "full helm (g)", "mask" };

        public static EQUIP getItemType(int wearId)
        {
            if (wearId > items.Count)
                return EQUIP.NOTHING;

            string weapon = items[wearId].getName().ToLower();
            for (int i = 0; i < CAPES.Length; i++)
            {
                if (weapon.Contains(CAPES[i]))
                    return EQUIP.CAPE;
            }
            for (int i = 0; i < HATS.Length; i++)
            {
                if (weapon.Contains(HATS[i]))
                    return EQUIP.HAT;
            }
            for (int i = 0; i < BOOTS.Length; i++)
            {
                if (weapon.EndsWith(BOOTS[i]) || weapon.StartsWith(BOOTS[i]))
                    return EQUIP.FEET;
            }
            for (int i = 0; i < GLOVES.Length; i++)
            {
                if (weapon.EndsWith(GLOVES[i]) || weapon.StartsWith(GLOVES[i]))
                    return EQUIP.HANDS;
            }
            for (int i = 0; i < SHIELDS.Length; i++)
            {
                if (weapon.Contains(SHIELDS[i]))
                    return EQUIP.SHIELD;
            }
            for (int i = 0; i < AMULETS.Length; i++)
            {
                if (weapon.EndsWith(AMULETS[i]) || weapon.StartsWith(AMULETS[i]))
                    return EQUIP.AMULET;
            }
            for (int i = 0; i < ARROWS.Length; i++)
            {
                if (weapon.EndsWith(ARROWS[i]) || weapon.StartsWith(ARROWS[i]))
                    return EQUIP.ARROWS;
            }
            for (int i = 0; i < RINGS.Length; i++)
            {
                if (weapon.EndsWith(RINGS[i]) || weapon.StartsWith(RINGS[i]))
                    return EQUIP.RING;
            }
            for (int i = 0; i < BODY.Length; i++)
            {
                if (weapon.Contains(BODY[i]) || wearId == 544 || wearId == 6107 || wearId == 1037)
                    return EQUIP.CHEST;
            }
            for (int i = 0; i < LEGS.Length; i++)
            {
                if (weapon.Contains(LEGS[i]) || wearId == 542 || wearId == 6108 || wearId == 1033)
                    return EQUIP.LEGS;
            }
            for (int i = 0; i < WEAPONS.Length; i++)
            {
                if (weapon.EndsWith(WEAPONS[i]) || weapon.StartsWith(WEAPONS[i]))
                    return EQUIP.WEAPON;
            }
            return EQUIP.NOTHING;
        }

        public static bool isTwoHanded(int itemID)
        {
            string weapon = items[itemID].getName();
            for (int i = 0; i < TWO_HANDED.Length; i++)
            {
                if (weapon.EndsWith(TWO_HANDED[i]) || weapon.StartsWith(TWO_HANDED[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] TWO_HANDED = {"shortbow", "longbow", "godsword", "greataxe", "flail", "halberd", "hammer", "spear", "hammers", "2h sword",
		    "Saradomin sword", "Granite maul", "claws", "Karil's crossbow", "Tzhaar-ket-om", "crystal bow", "Dark bow"};

        /**
         * An array of non-tradable items.
         * First block of items don't get destroyed when dropped, the second block do.
         */

        public static int[] PLAYER_BOUND_ITEMS = {
		    6570, // Fire cape.
		    2996, // Agility ticket.
		    10551, // Fighter torso.
		    8850, // Rune defender.
		    8840, // Void skirt.
		    8839, // Void top.
		    11665, // Melee void helm.
		    11663, // Range void helm.
		    11664, // Mage void helm.
		    8842, // Void gloves.
		    8851, // Warrior guild token.
	    };
    }
}