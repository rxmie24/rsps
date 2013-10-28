using RS2.Server.player;
using System;

namespace RS2.Server.definitions
{
    internal class ProtectedItems
    {
        private static int INVENTORY = 0, EQUIPMENT = 1;

        public static int[] getProtectedItem1(Player p)
        {
            int[] protectedItem = new int[2];
            protectedItem[0] = -1;
            for (int i = 0; i < 28; i++)
            {
                if (p.getInventory().getSlot(i).itemId == -1) continue;
                int price = p.getInventory().getSlot(i).getDefinition().getPrice().getMaximumPrice();
                if ((price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice()))
                {
                    protectedItem[0] = p.getInventory().getSlot(i).getItemId();
                    protectedItem[1] = INVENTORY;
                }
            }
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                if (p.getEquipment().getSlot(equip).itemId == -1) continue;
                int price = p.getEquipment().getSlot(equip).getDefinition().getPrice().getMaximumPrice();
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    protectedItem[0] = p.getEquipment().getSlot(equip).getItemId();
                    protectedItem[1] = EQUIPMENT;
                }
            }
            return protectedItem;
        }

        public static int[] getProtectedItem2(Player p)
        {
            int[] protectedItem = new int[2];
            protectedItem[0] = -1;
            int[] protectedItem1 = getProtectedItem1(p);
            bool save;
            for (int i = 0; i < 28; i++)
            {
                if (p.getInventory().getSlot(i).itemId == -1) continue;
                int amt = p.getInventory().getItemAmount(p.getInventory().getItemInSlot(i));
                int price = p.getInventory().getSlot(i).getDefinition().getPrice().getMaximumPrice();
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[1] == INVENTORY)
                    {
                        if (protectedItem1[0] == p.getInventory().getItemInSlot(i))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getInventory().getSlot(i).getItemId();
                        protectedItem[1] = INVENTORY;
                    }
                }
            }
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                if (p.getEquipment().getSlot(equip).itemId == -1) continue;
                int price = p.getEquipment().getSlot(equip).getDefinition().getPrice().getMaximumPrice();
                int amt = p.getEquipment().getAmountInSlot(equip);
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[1] == EQUIPMENT)
                    {
                        if (protectedItem1[0] == p.getEquipment().getItemInSlot(equip))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getEquipment().getSlot(equip).getItemId();
                        protectedItem[1] = EQUIPMENT;
                    }
                }
            }
            return protectedItem;
        }

        public static int[] getProtectedItem3(Player p)
        {
            int[] protectedItem = new int[2];
            protectedItem[0] = -1;
            int[] protectedItem1 = getProtectedItem1(p);
            int[] protectedItem2 = getProtectedItem2(p);
            bool save;
            for (int i = 0; i < 28; i++)
            {
                if (p.getInventory().getSlot(i).itemId == -1) continue;
                int amt = p.getInventory().getItemAmount(p.getInventory().getItemInSlot(i));
                int price = p.getInventory().getSlot(i).getDefinition().getPrice().getMaximumPrice();
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[1] == INVENTORY)
                    {
                        if (protectedItem1[0] == p.getInventory().getItemInSlot(i))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (protectedItem2[1] == INVENTORY)
                    {
                        if (protectedItem2[0] == p.getInventory().getItemInSlot(i))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (protectedItem1[1] == INVENTORY && protectedItem2[1] == INVENTORY)
                    {
                        if (protectedItem1[0] == p.getInventory().getItemInSlot(i) && protectedItem2[0] == p.getInventory().getItemInSlot(i))
                        {
                            if (amt < 3)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getInventory().getSlot(i).getItemId();
                        protectedItem[1] = INVENTORY;
                    }
                }
            }
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                if (p.getEquipment().getSlot(equip).itemId == -1) continue;
                int price = p.getEquipment().getSlot(equip).getDefinition().getPrice().getMaximumPrice();
                int amt = p.getEquipment().getAmountInSlot(equip);
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[1] == EQUIPMENT)
                    {
                        if (protectedItem1[0] == p.getEquipment().getItemInSlot(equip))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (protectedItem2[1] == EQUIPMENT)
                    {
                        if (protectedItem2[0] == p.getEquipment().getItemInSlot(equip))
                        {
                            if (amt < 2)
                            {
                                save = false;
                            }
                        }
                    }
                    if (protectedItem1[1] == EQUIPMENT && protectedItem2[1] == EQUIPMENT)
                    {
                        if (protectedItem1[0] == p.getEquipment().getItemInSlot(equip) && protectedItem2[0] == p.getEquipment().getItemInSlot(equip))
                        {
                            if (amt < 3)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getEquipment().getSlot(equip).getItemId();
                        protectedItem[1] = EQUIPMENT;
                    }
                }
            }
            return protectedItem;
        }

        public static int[] getProtectedItem4(Player p)
        {
            int[] protectedItem = new int[2];
            protectedItem[0] = -1;
            int[] protectedItem1 = getProtectedItem1(p);
            int[] protectedItem2 = getProtectedItem2(p);
            int[] protectedItem3 = getProtectedItem3(p);
            bool save;
            for (int i = 0; i < 28; i++)
            {
                if (p.getInventory().getSlot(i).itemId == -1) continue;
                int amt = p.getInventory().getItemAmount(p.getInventory().getItemInSlot(i));
                int price = p.getInventory().getSlot(i).getDefinition().getPrice().getMaximumPrice();
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[0] == p.getInventory().getItemInSlot(i) && protectedItem1[1] == INVENTORY)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem2[0] == p.getInventory().getItemInSlot(i) && protectedItem2[1] == INVENTORY)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem3[0] == p.getInventory().getItemInSlot(i) && protectedItem3[1] == INVENTORY)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (amt == 2)
                    {
                        int[][] array = { protectedItem1, protectedItem2, protectedItem3 };
                        int k = 0;
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (array[j][0] == p.getInventory().getItemInSlot(i) && array[j][1] == INVENTORY)
                            {
                                k++;
                            }
                        }
                        if (k >= 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem1[1] == INVENTORY && protectedItem2[1] == INVENTORY && protectedItem3[1] == INVENTORY)
                    {
                        if (protectedItem1[0] == p.getInventory().getItemInSlot(i) && protectedItem2[0] == p.getInventory().getItemInSlot(i) && protectedItem3[0] == p.getInventory().getItemInSlot(i))
                        {
                            if (amt < 4)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getInventory().getSlot(i).getItemId();
                        protectedItem[1] = INVENTORY;
                    }
                }
            }
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                if (p.getEquipment().getSlot(equip).itemId == -1) continue;
                int price = p.getEquipment().getSlot(equip).getDefinition().getPrice().getMaximumPrice();
                int amt = p.getEquipment().getAmountInSlot(equip);
                if (price > ItemData.forId(protectedItem[0]).getPrice().getMaximumPrice())
                {
                    save = true;
                    if (protectedItem1[0] == p.getEquipment().getItemInSlot(equip) && protectedItem1[1] == EQUIPMENT)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem2[0] == p.getEquipment().getItemInSlot(equip) && protectedItem2[1] == EQUIPMENT)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem3[0] == p.getEquipment().getItemInSlot(equip) && protectedItem3[1] == EQUIPMENT)
                    {
                        if (amt < 2)
                        {
                            save = false;
                        }
                    }
                    if (amt == 2)
                    {
                        int[][] array = { protectedItem1, protectedItem2, protectedItem3 };
                        int k = 0;
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (array[j][0] == p.getEquipment().getItemInSlot(equip) && array[j][1] == EQUIPMENT)
                            {
                                k++;
                            }
                        }
                        if (k >= 2)
                        {
                            save = false;
                        }
                    }
                    if (protectedItem1[1] == EQUIPMENT && protectedItem2[1] == EQUIPMENT && protectedItem3[1] == EQUIPMENT)
                    {
                        if (protectedItem1[0] == p.getEquipment().getItemInSlot(equip) && protectedItem2[0] == p.getEquipment().getItemInSlot(equip) && protectedItem3[0] == p.getEquipment().getItemInSlot(equip))
                        {
                            if (amt < 4)
                            {
                                save = false;
                            }
                        }
                    }
                    if (save)
                    {
                        protectedItem[0] = p.getEquipment().getSlot(equip).getItemId();
                        protectedItem[1] = EQUIPMENT;
                    }
                }
            }
            return protectedItem;
        }

        public static void displayItemsInterface(Player p)
        {
            int amountToKeep = p.isSkulled() ? 0 : 3;
            if (p.getPrayers().isProtectItem())
            {
                amountToKeep = p.isSkulled() ? 1 : 4;
            }
            int item1 = getProtectedItem1(p)[0];
            int item2 = getProtectedItem2(p)[0];
            int item3 = getProtectedItem3(p)[0];
            int item4 = amountToKeep == 4 ? getProtectedItem4(p)[0] : -1;
            if (amountToKeep == 1)
            {
                item2 = 65535;
                item3 = 65535;
                item3 = item1;
            }
            if (amountToKeep == 0)
            {
                item1 = 65535;
                item2 = 65535;
                item3 = 65535;
                item4 = 65535;
            }
            object[] opts = new object[] { 17598720, 20221838, "You're marked with a <col=ff3333>skull<col=ff981f>.", 0, 1, item4, item1, item2, item3, /* Items to keep */ amountToKeep /* Items to keep */, 0 };
            p.getPackets().displayInterface(102);
            p.getPackets().sendClientScript2(204, 118, opts, "iiooooiisii");
            p.getPackets().setRightClickOptions(1278, (102 * 65536) + 21, 0, 40);
            p.getPackets().setRightClickOptions(1278, (102 * 65536) + 18, 0, 4);
        }
    }
}