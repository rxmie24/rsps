using RS2.Server.definitions;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.player.skills.magic;
using RS2.Server.player.skills.runecrafting;
using System;

namespace RS2.Server.player
{
    internal class Equipment
    {
        private Item[] slots = new Item[14];
        private Player player;

        public enum BONUS
        {
            STAB_ATTACK,
            SLASH_ATTACK,
            CRUSH_ATTACK,
            MAGIC_ATTACK,
            RANGED_ATTACK,
            STAB_DEFENCE,
            SLASH_DEFENCE,
            CRUSH_DEFENCE,
            MAGIC_DEFENCE,
            RANGED_DEFENCE,
            SUMMONING,
            STRENGTH,
            PRAYER
        }

        private string[] BONUS_NAMES = new string[] { "Stab", "Slash", "Crush", "Magic", "Ranged", "Stab", "Slash", "Crush", "Magic", "Ranged", "Summoning", "Strength", "Prayer" };
        private int[] bonuses = new int[13]; //equipment bonuses

        public Equipment(Player player)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Item(-1, 0);
            }
            this.player = player;
        }

        public Item[] getEquipment()
        {
            return slots;
        }

        public bool equipItem(int itemID, int slot)
        {
            ItemData.EQUIP equipType = ItemData.getItemType(itemID);
            int amount = player.getInventory().getAmountInSlot(slot);
            bool stackable = ItemData.forId(itemID).isStackable();
            bool twoHanded = ItemData.isTwoHanded(itemID);
            if (equipType == ItemData.EQUIP.NOTHING)
            {
                player.getPackets().sendMessage("Unable to find an item slot for item : " + itemID + " , please report this to a staff member.");
                return false;
            }
            if (duelRuleActive(equipType))
            {
                return true;
            }
            if (twoHanded)
            {
                if (player.getInventory().getTotalFreeSlots() < getNeeded2HSlots())
                {
                    player.getPackets().sendMessage("Not enough space in your inventory.");
                    return false;
                }
            }
            if (!player.getInventory().deleteItem(itemID, slot, amount))
            {
                return false;
            }
            if (twoHanded && getItemInSlot(ItemData.EQUIP.SHIELD) != -1)
            {
                if (!unequipItem(ItemData.EQUIP.SHIELD))
                {
                    return false;
                }
            }
            if (equipType == ItemData.EQUIP.SHIELD)
            {
                if (getItemInSlot(ItemData.EQUIP.WEAPON) != -1)
                {
                    if (ItemData.isTwoHanded(slots[3].getItemId()))
                    {
                        if (!unequipItem(ItemData.EQUIP.WEAPON))
                        {
                            return false;
                        }
                    }
                }
            }
            int equipSlot = Convert.ToInt32(equipType);
            if (slots[equipSlot].getItemId() != itemID && slots[equipSlot].getItemId() > 0)
            {
                if (!player.getInventory().addItem(slots[equipSlot].getItemId(), slots[equipSlot].getItemAmount(), slot))
                {
                    return false;
                }
                if (equipType == ItemData.EQUIP.HAT)
                {
                    RuneCraft.toggleRuin(player, slots[equipSlot].getItemId(), false);
                    if (RuneCraft.getTiaraIndex(itemID) != -1)
                    { // switching from tiara to tiara.
                        RuneCraft.toggleRuin(player, itemID, true);
                    }
                }
            }
            else if (stackable && slots[equipSlot].getItemId() == itemID)
            {
                amount = slots[equipSlot].getItemAmount() + amount;
            }
            else if (slots[equipSlot].getItemId() != -1)
            {
                player.getInventory().addItem(slots[equipSlot].getItemId(), slots[equipSlot].getItemAmount(), slot);
            }
            slots[equipSlot].setItemId(itemID);
            slots[equipSlot].setItemAmount(amount);
            player.getPackets().refreshEquipment();
            player.getUpdateFlags().setAppearanceUpdateRequired(true);
            if (equipType == ItemData.EQUIP.HAT)
            {
                RuneCraft.toggleRuin(player, itemID, RuneCraft.wearingTiara(player));
            }
            if (equipType == ItemData.EQUIP.WEAPON)
            {
                setWeapon();
                MagicData.cancelAutoCast(player, true);
            }
            refreshBonuses();
            player.setEntityFocus(65535);
            return true;
        }

        private bool duelRuleActive(ItemData.EQUIP equipType)
        {
            if (player.getDuel() == null)
            {
                return false;
            }
            if (player.getDuel().getStatus() == 5 || player.getDuel().getStatus() == 6)
            {
                ItemData.EQUIP[] slot = {ItemData.EQUIP.HAT,   ItemData.EQUIP.CAPE,  ItemData.EQUIP.AMULET,
                                        ItemData.EQUIP.WEAPON, ItemData.EQUIP.CHEST, ItemData.EQUIP.SHIELD,
                                        ItemData.EQUIP.LEGS,   ItemData.EQUIP.HANDS, ItemData.EQUIP.FEET,
                                        ItemData.EQUIP.RING,   ItemData.EQUIP.ARROWS};

                DuelSession.RULE[] rule = {DuelSession.RULE.HAT,    DuelSession.RULE.CAPE,   DuelSession.RULE.AMULET,
                                           DuelSession.RULE.WEAPON, DuelSession.RULE.BODY,   DuelSession.RULE.SHIELD,
                                           DuelSession.RULE.LEGS,   DuelSession.RULE.GLOVES, DuelSession.RULE.BOOTS,
                                           DuelSession.RULE.RING,   DuelSession.RULE.ARROWS};
                for (int j = 0; j < rule.Length; j++)
                {
                    if (player.getDuel().ruleEnabled(rule[j]))
                    {
                        if (equipType == slot[j])
                        {
                            player.getPackets().sendMessage("You cannot equip that item in this duel.");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void clearAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].setItemId(-1);
                slots[i].setItemAmount(0);
            }
            setWeapon();
            player.getPackets().refreshEquipment();
            player.getUpdateFlags().setAppearanceUpdateRequired(true);
            refreshBonuses();
        }

        public bool unequipItem(ItemData.EQUIP slot)
        {
            int equipSlotIndex = (int)slot;
            if (player.getInventory().addItem(slots[equipSlotIndex].getItemId(), slots[equipSlotIndex].getItemAmount()))
            {
                if (slot == ItemData.EQUIP.HAT)
                {
                    if (RuneCraft.wearingTiara(player))
                    {
                        RuneCraft.toggleRuin(player, getItemInSlot(slot), false);
                    }
                }
                slots[equipSlotIndex].setItemId(-1);
                slots[equipSlotIndex].setItemAmount(0);
                player.getPackets().refreshEquipment();
                player.getUpdateFlags().setAppearanceUpdateRequired(true);
                refreshBonuses();
                player.setEntityFocus(65535);
                if (slot == ItemData.EQUIP.WEAPON)
                {
                    setWeapon();
                    MagicData.cancelAutoCast(player, true);
                }
                return true;
            }
            return false;
        }

        private int getNeeded2HSlots()
        {
            int shield = slots[5].getItemId();
            int weapon = slots[3].getItemId();
            if ((shield != -1 && weapon == -1) || (shield == -1 && weapon != -1) || (shield == -1 && weapon == -1))
            {
                return 0;
            }
            return 1;
        }

        public void setWeapon()
        {
            if (slots[3].getItemId() == -1)
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 92);
                player.getPackets().modifyText("Unarmed", 92, 0);
                AttackInterface.setButtonForAttackStyle(player, 92);
                return;
            }
            string weapon = slots[3].getDefinition().getName();
            player.getSpecialAttack().setUsingSpecial(false);
            player.setTarget(null);
            int interfaceId = -1;
            if (weapon.Equals("Abyssal whip"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 93);
                player.getPackets().modifyText(weapon, 93, 0);
                interfaceId = 93;
            }
            else if (weapon.Equals("Granite maul") || weapon.Equals("Tzhaar-ket-om") || weapon.Equals("Torags hammers"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 76);
                player.getPackets().modifyText(weapon, 76, 0);
                interfaceId = 76;
            }
            else if (weapon.Equals("Veracs flail") || (weapon.EndsWith("mace") && !weapon.Equals("Void knight mace")))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 88);
                player.getPackets().modifyText(weapon, 88, 0);
                interfaceId = 88;
            }
            else if (weapon.EndsWith("crossbow") || weapon.EndsWith(" c'bow"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 79);
                player.getPackets().modifyText(weapon, 79, 0);
                interfaceId = 79;
            }
            else if (weapon.EndsWith("bow") || weapon.EndsWith("bow full") || weapon.Equals("Seercull"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 77);
                player.getPackets().modifyText(weapon, 77, 0);
                interfaceId = 77;
            }
            else if (weapon.StartsWith("Staff") || weapon.EndsWith("staff") || weapon.Equals("Toktz-mej-tal") || weapon.Equals("Void knight mace"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 90);
                player.getPackets().modifyText(weapon, 90, 0);
                interfaceId = 90;
            }
            else if (weapon.EndsWith("dart") || weapon.EndsWith("knife") || weapon.EndsWith("javelin") || weapon.EndsWith("thrownaxe") || weapon.Equals("Toktz-xil-ul"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 91);
                player.getPackets().modifyText(weapon, 91, 0);
                interfaceId = 91;
            }
            else if (weapon.EndsWith("dagger") || weapon.EndsWith("dagger(s)") || weapon.EndsWith("dagger(+)") || weapon.EndsWith("dagger(p)") || weapon.EndsWith("dagger(p++)"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 89);
                player.getPackets().modifyText(weapon, 89, 0);
                interfaceId = 89;
            }
            else if (weapon.EndsWith("pickaxe"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 83);
                player.getPackets().modifyText(weapon, 83, 0);
                interfaceId = 83;
            }
            else if (weapon.EndsWith("axe") || weapon.EndsWith("battleaxe") || weapon.EndsWith("adze"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 75);
                player.getPackets().modifyText(weapon, 75, 0);
                interfaceId = 75;
            }
            else if (weapon.EndsWith("halberd"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 84);
                player.getPackets().modifyText(weapon, 84, 0);
                interfaceId = 84;
            }
            else if (weapon.EndsWith("spear") || weapon.Equals("Guthans warspear"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 85);
                player.getPackets().modifyText(weapon, 85, 0);
                interfaceId = 85;
            }
            else if (weapon.EndsWith("claws"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 78);
                player.getPackets().modifyText(weapon, 78, 0);
                interfaceId = 78;
            }
            else if (weapon.EndsWith("2h sword") || weapon.EndsWith("godsword") || weapon.Equals("Saradomin sword"))
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 82);
                player.getPackets().modifyText(weapon, 81, 0);
                interfaceId = 81;
            }
            else
            {
                player.getPackets().sendTab(player.isHd() ? 93 : 83, 82);
                player.getPackets().modifyText(weapon, 82, 0);
                interfaceId = 82;
            }
            AttackInterface.setButtonForAttackStyle(player, interfaceId);
            setSpecials();
        }

        private void setSpecials()
        {
            int weaponId = slots[3].getItemId();
            if (weaponId == 4151)
            {
                player.getPackets().showChildInterface(93, 10, true);
            }
            else if (weaponId == 5698 || weaponId == 1231 || weaponId == 5680
                  || weaponId == 1215 || weaponId == 8872 || weaponId == 8874
                  || weaponId == 8876 || weaponId == 8878)
            {
                player.getPackets().showChildInterface(89, 12, true);
            }
            else if (weaponId == 35 || weaponId == 1305 || weaponId == 4587
                  || weaponId == 6746 || weaponId == 11037 || weaponId == 13902)
            {
                player.getPackets().showChildInterface(82, 12, true);
            }
            else if (weaponId == 7158 || weaponId == 11694 || weaponId == 11696
                  || weaponId == 11698 || weaponId == 11700 || weaponId == 11730)
            {
                player.getPackets().showChildInterface(81, 12, true);
            }
            else if (weaponId == 859 || weaponId == 861 || weaponId == 6724
                  || weaponId == 10284 || weaponId == 859 || weaponId == 11235)
            {
                player.getPackets().showChildInterface(77, 13, true);
            }
            else if (weaponId == 8880)
            {
                player.getPackets().showChildInterface(79, 10, true);
            }
            else if (weaponId == 3101 || weaponId == 14484)
            {
                player.getPackets().showChildInterface(78, 12, true);
            }
            else if (weaponId == 1434 || weaponId == 11061 || weaponId == 10887)
            {
                player.getPackets().showChildInterface(88, 12, true);
            }
            else if (weaponId == 1377 || weaponId == 6739)
            {
                player.getPackets().showChildInterface(75, 12, true);
            }
            else if (weaponId == 4153)
            {
                player.getPackets().showChildInterface(76, 10, true);
            }
            else if (weaponId == 3204)
            {
                player.getPackets().showChildInterface(84, 10, true);

                /* SPEARS */
            }
            else if (weaponId == 1249 || weaponId == 13905)
            {
                player.getPackets().showChildInterface(85, 10, true);
            }
            player.getSpecialAttack().refreshBar();
        }

        public int getStandWalkAnimation()
        {
            if (player.getAppearance().getWalkAnimation() > 0)
            {
                return player.getAppearance().getWalkAnimation();
            }
            if (slots[3].getItemId() == -1)
            {
                return 1426;
            }
            return ItemData.forId(slots[3].getItemId()).getAnimation();
        }

        public Item getSlot(ItemData.EQUIP slot)
        {
            return slots[Convert.ToInt32(slot)];
        }

        public int getAmountInSlot(ItemData.EQUIP slot)
        {
            return slots[Convert.ToInt32(slot)].getItemAmount();
        }

        public int getItemInSlot(ItemData.EQUIP slot)
        {
            return slots[Convert.ToInt32(slot)].getItemId();
        }

        public void displayEquipmentScreen()
        {
            player.getWalkingQueue().resetWalkingQueue();
            player.getPackets().clearMapFlag();
            object[] opts = new object[] { "", "", "", "", "Wear<col=ff9040>", -1, 0, 7, 4, 93, 43909120 };
            player.getPackets().displayInterface(667);
            refreshBonuses();
            player.getPackets().displayInventoryInterface(149);
            player.getPackets().sendClientScript2(172, 149, opts, "IviiiIsssss");
            player.getPackets().setRightClickOptions(1278, (667 * 65536) + 14, 0, 13);
        }

        public int getBonus(int bonusType)
        {
            return bonuses[bonusType];
        }

        public int getBonus(BONUS bonusType)
        {
            return bonuses[Convert.ToInt32(bonusType)];
        }

        public void refreshBonuses()
        {
            for (int i = 0; i < 13; i++)
            {
                bonuses[i] = 0;
            }
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                if (getItemInSlot(equip) != -1)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        bonuses[j] += getSlot(equip).getDefinition().getBonus(j);
                    }
                }
            }
            player.getPackets().modifyText("0 Kg", 667, 32); // weight
            int id = 36;
            for (int i = 0; i < bonuses.Length; i++)
            {
                string s = bonuses[i] > 0 ? "+" : "";
                player.getPackets().modifyText(BONUS_NAMES[i] + ": " + s + bonuses[i], 667, id);
                id++;
                if (id == 35)
                {
                    id = 41;
                }
                if (id == 47)
                {
                    id = 48;
                }
            }
        }
    }
}