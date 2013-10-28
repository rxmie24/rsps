using RS2.Server.definitions;
using RS2.Server.model;

namespace RS2.Server.player
{
    internal class Inventory
    {
        public const int MAX_INVENTORY_SLOTS = 28;
        private Item[] slots = new Item[MAX_INVENTORY_SLOTS];
        private int[] protectedItems;
        private static int MAX_AMOUNT = int.MaxValue;
        private Player p;

        public Inventory(Player p)
        {
            this.p = p;
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Item(-1, 0);
            }
        }

        public int getTotalFreeSlots()
        {
            int j = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == -1)
                {
                    j++;
                }
            }
            return j;
        }

        public int findFreeSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool hasItem(int itemId)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == itemId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool hasItemAmount(int itemId, long amount)
        {
            int j = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == itemId)
                {
                    j += slots[i].getItemAmount();
                }
            }
            return j >= amount;
        }

        public int findItem(int itemId)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == itemId)
                {
                    return i;
                }
            }
            return -1;
        }

        public int getItemAmount(int itemId)
        {
            int j = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].getItemId() == itemId)
                {
                    j += slots[i].getItemAmount();
                }
            }
            return j;
        }

        public void clearAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].setItemId(-1);
                slots[i].setItemAmount(0);
            }
            p.getPackets().refreshInventory();
        }

        public bool replaceSingleItem(int itemToReplace, int itemToAdd)
        {
            int slot = findItem(itemToReplace);
            if (slot == -1)
            {
                return false;
            }
            if ((slots[slot].getItemId() != itemToReplace) || getAmountInSlot(slot) <= 0)
            {
                return false;
            }
            slots[slot].setItemId(itemToAdd);
            slots[slot].setItemAmount(1);
            p.getPackets().refreshInventory();
            return true;
        }

        public bool replaceItemSlot(int itemToReplace, int itemToAdd, int slot)
        {
            if (slots[slot].getItemId() != itemToReplace || slots[slot].getItemAmount() <= 0)
            {
                return false;
            }
            slots[slot].setItemId(itemToAdd);
            slots[slot].setItemAmount(1);
            p.getPackets().refreshInventory();
            return true;
        }

        public void addItemOrGround(int item, int amount)
        {
            /*
             * We try to add the item to the inventory..
             */
            if (addItem(item, amount))
            {
                return;
            }
            /*
             * It didn't add the item above, yet we have room for it?..odd.
             */
            if (getTotalFreeSlots() > 0)
            {
                return;
            }
            /*
             * Add the item to the ground.
             */
            GroundItem g = new GroundItem(item, amount, p.getLocation(), p);
            Server.getGroundItems().newEntityDrop(g);
        }

        public void addItemOrGround(int item)
        {
            addItemOrGround(item, 1);
        }

        public bool addItem(int item)
        {
            return addItem(item, 1, findFreeSlot());
        }

        public bool addItem(int item, int amount)
        {
            return addItem(item, amount, findFreeSlot());
        }

        public bool addItem(int itemId, int amount, int slot)
        {
            if (itemId < 0 || itemId > Constants.MAX_ITEMS)
                return false;
            bool stackable = ItemData.forId(itemId).isStackable();
            if (amount <= 0)
            {
                return false;
            }
            if (!stackable)
            {
                if (getTotalFreeSlots() <= 0)
                {
                    p.getPackets().sendMessage("Not enough space in your inventory.");
                    return false;
                }
                if (slots[slot].getItemId() != -1)
                {
                    slot = findFreeSlot();
                    if (slot == -1)
                    {
                        p.getPackets().sendMessage("Not enough space in your inventory.");
                        return false;
                    }
                }
                slots[slot].setItemId(itemId);
                slots[slot].setItemAmount(1);
                p.getPackets().refreshInventory();
                return true;
            }
            else if (stackable)
            {
                if (hasItem(itemId))
                {
                    slot = findItem(itemId);
                }
                else if (getTotalFreeSlots() <= 0)
                {
                    p.getPackets().sendMessage("Not enough space in your inventory.");
                    return false;
                }
                long newAmount = ((long)amount + slots[slot].getItemAmount());
                if (newAmount > MAX_AMOUNT)
                {
                    p.getPackets().sendMessage("Not enough space in your inventory.");
                    return false;
                }
                if (slots[slot].getItemId() != -1 && slots[slot].getItemId() != itemId)
                {
                    slot = findFreeSlot();
                    if (slot == -1)
                    {
                        p.getPackets().sendMessage("Not enough space in your inventory.");
                        return false;
                    }
                }
                slots[slot].setItemId(itemId);
                slots[slot].setItemAmount(slots[slot].getItemAmount() + amount);
                p.getPackets().refreshInventory();
                return true;
            }
            return false;
        }

        public void deleteAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                deleteItem(slots[i].getItemId(), i, slots[i].getItemAmount());
            }
        }

        public bool deleteItem(int item)
        {
            return deleteItem(item, findItem(item), 1);
        }

        public bool deleteItem(int item, int amount)
        {
            return deleteItem(item, findItem(item), amount);
        }

        public bool deleteItem(int itemId, int slot, int amount)
        {
            if (slot == -1)
            {
                return false;
            }
            if (slots[slot].getItemId() == itemId && slots[slot].getItemAmount() >= amount)
            {
                slots[slot].setItemAmount(slots[slot].getItemAmount() - amount);
                if (slots[slot].getItemAmount() <= 0)
                {
                    slots[slot].setItemId(-1);
                    slots[slot].setItemAmount(0);
                }
                p.getPackets().refreshInventory();
                return true;
            }
            return false;
        }

        public int getAmountInSlot(int slot)
        {
            if (slot < 0 || slot > 28)
            {
                return -1;
            }
            return slots[slot].getItemAmount();
        }

        public int getItemInSlot(int slot)
        {
            if (slot < 0 || slot > 28)
            {
                return -1;
            }
            return slots[slot].getItemId();
        }

        public Item getSlot(int slot)
        {
            if (slot < 0 || slot > 28)
            {
                return null;
            }
            return slots[slot];
        }

        public Item[] getItems()
        {
            return slots;
        }

        public void setProtectedItems(int[] protectedItems)
        {
            this.protectedItems = protectedItems;
        }

        public int[] getProtectedItems()
        {
            return protectedItems;
        }

        public int getProtectedItem(int i)
        {
            return protectedItems[i];
        }
    }
}