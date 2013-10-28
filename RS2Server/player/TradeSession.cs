using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player
{
    internal class TradeSession
    {
        private Player player;
        private Player player2;
        private Item[] items = new Item[Inventory.MAX_INVENTORY_SLOTS];
        private Item loanItem;
        private int status = 1;
        private bool tradeModified;

        public TradeSession(Player player, Player player2)
        {
            this.player = player;
            this.player2 = player2;
            openTrade();
            player.getTradeRequests().Clear();
        }

        private void openTrade()
        {
            player.getPackets().displayInterface(335);
            player.getPackets().displayInventoryInterface(336);
            player.getPackets().setRightClickOptions(1278, (335 * 65536) + 30, 0, 27);
            player.getPackets().setRightClickOptions(1026, (335 * 65536) + 32, 0, 27);
            player.getPackets().setRightClickOptions(1278, (336 * 65536), 0, 27);
            player.getPackets().setRightClickOptions(2360446, (336 * 65536), 0, 27);
            object[] opts1 = new object[] { "", "", "", "Value", "Remove X", "Remove All", "Remove 10", "Remove 5", "Remove 1", -1, 0, 7, 4, 90, 21954590 };
            object[] opts2 = new object[] { "", "", "Lend", "Value", "Offer X", "Offer All", "Offer 10", "Offer 5", "Offer 1", -1, 0, 7, 4, 93, 22020096 };
            object[] opts3 = new object[] { "", "", "", "", "", "", "", "", "Value", -1, 0, 7, 4, 90, 21954592 };
            player.getPackets().sendClientScript(150, opts1, "IviiiIsssssssss");
            player.getPackets().sendClientScript(150, opts2, "IviiiIsssssssss");
            player.getPackets().sendClientScript(695, opts3, "IviiiIsssssssss");

            player.getPackets().modifyText("Trading with: " + player2.getLoginDetails().getUsername(), 335, 15);
            player.getPackets().modifyText(player2.getLoginDetails().getUsername() + " has " + player2.getInventory().getTotalFreeSlots() + " free inventory slots.", 335, 21);
            player.getPackets().modifyText("", 335, 36);

            refreshTrade();
        }

        public bool tradeItem(int slot, int amount)
        {
            int itemId = player.getInventory().getItemInSlot(slot);
            bool stackable = ItemData.forId(itemId).isStackable();
            int tradeSlot = findItem(itemId);
            if (amount <= 0 || itemId == -1 || status > 2)
            {
                return false;
            }
            if (ItemData.forId(itemId).isPlayerBound())
            {
                player.getPackets().sendMessage("You cannot trade that item.");
                return false;
            }
            if (!stackable)
            {
                tradeSlot = findFreeSlot();
                if (tradeSlot == -1)
                {
                    player.getPackets().sendMessage("An error occured whilst trying to find free a trade slot.");
                    return false;
                }
                if (amount > player.getInventory().getItemAmount(itemId))
                {
                    amount = player.getInventory().getItemAmount(itemId);
                }
                for (int i = 0; i < amount; i++)
                {
                    tradeSlot = findFreeSlot();
                    if (!player.getInventory().deleteItem(itemId) || tradeSlot == -1)
                    {
                        break;
                    }
                    items[tradeSlot] = new Item(itemId, 1);
                }
                if (status == 2 || player2.getTrade().getStatus() == 2)
                {
                    this.status = 1;
                    player2.getTrade().setStatus(1);
                    player.getPackets().modifyText("", 335, 36);
                    player2.getPackets().modifyText("", 335, 36);
                }
                refreshTrade();
                return true;
            }
            else if (stackable)
            {
                tradeSlot = findItem(itemId);
                if (tradeSlot == -1)
                {
                    tradeSlot = findFreeSlot();
                    if (tradeSlot == -1)
                    {
                        player.getPackets().sendMessage("An error occured whilst trying to find free a trade slot.");
                        return false;
                    }
                }
                if (amount > player.getInventory().getAmountInSlot(slot))
                {
                    amount = player.getInventory().getAmountInSlot(slot);
                }
                if (player.getInventory().deleteItem(itemId, amount))
                {
                    if (items[tradeSlot] == null)
                    {
                        items[tradeSlot] = new Item(itemId, amount);
                    }
                    else
                    {
                        if (items[tradeSlot].getItemId() == itemId)
                        {
                            items[tradeSlot].setItemId(itemId);
                            items[tradeSlot].setItemAmount(items[tradeSlot].getItemAmount() + amount);
                        }
                    }
                    if (status == 2 || player2.getTrade().getStatus() == 2)
                    {
                        this.status = 1;
                        player2.getTrade().setStatus(1);
                        player.getPackets().modifyText("", 335, 36);
                        player2.getPackets().modifyText("", 335, 36);
                    }
                    refreshTrade();
                    return true;
                }
            }
            return false;
        }

        public void removeItem(int slot, int amount)
        {
            if (status > 2 || items[slot] == null)
            {
                return;
            }
            int itemId = getItemInSlot(slot);
            int tradeSlot = findItem(itemId);
            bool stackable = ItemData.forId(itemId).isStackable();
            if (tradeSlot == -1)
            {
                Misc.WriteError("user tried to remove non-existing item from trade! " + player.getLoginDetails().getUsername());
                return;
            }
            if (amount > getItemAmount(itemId))
            {
                amount = getItemAmount(itemId);
            }
            if (!stackable)
            {
                for (int i = 0; i < amount; i++)
                {
                    tradeSlot = findItem(itemId);
                    if (player.getInventory().addItem(itemId, amount))
                    {
                        items[tradeSlot].setItemAmount(getAmountInSlot(tradeSlot) - amount);
                        if (getAmountInSlot(tradeSlot) <= 0)
                        {
                            items[tradeSlot] = null;
                        }
                        player2.getPackets().tradeWarning(tradeSlot);
                    }
                }
                if (status == 2 || player2.getTrade().getStatus() == 2)
                {
                    this.status = 1;
                    player2.getTrade().setStatus(1);
                    player.getPackets().modifyText("", 335, 36);
                    player2.getPackets().modifyText("", 335, 36);
                }
                refreshTrade();
            }
            else
            {
                tradeSlot = findItem(itemId);
                if (player.getInventory().addItem(itemId, amount))
                {
                    items[tradeSlot].setItemAmount(getAmountInSlot(tradeSlot) - amount);
                    if (getAmountInSlot(tradeSlot) <= 0)
                    {
                        items[tradeSlot] = null;
                    }
                    player2.getPackets().tradeWarning(tradeSlot);
                }
            }
            if (status == 2 || player2.getTrade().getStatus() == 2)
            {
                this.status = 1;
                player2.getTrade().setStatus(1);
                player.getPackets().modifyText("", 335, 36);
                player2.getPackets().modifyText("", 335, 36);
            }
            refreshTrade();
            tradeModified = true;
        }

        private void refreshTrade()
        {
            //sendItems(-1, 63761, 541, player.getInventory().getItems()); // your loan item
            //sendItems(-2, 60530, 541, player.getTrade().getLoanItem()); // thier loan item
            player.getPackets().sendItems(-1, 64212, 90, items);
            player2.getPackets().sendItems(-2, 60981, 90, items);
            player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
        }

        public void accept()
        {
            long finalAmount = 0;
            if (status == 1)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    finalAmount = 0;
                    if (items[i] != null)
                    {
                        long tradeAmount = items[i].getItemAmount();
                        long p2InvenAmount = player2.getInventory().getItemAmount(items[i].getItemId());
                        finalAmount = (long)(tradeAmount + p2InvenAmount);
                        if (finalAmount >= int.MaxValue)
                        {
                            player.getPackets().sendMessage("Other player has too many of item: " + ItemData.forId(items[i].getItemId()).getName() + ".");
                            return;
                        }
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    finalAmount = 0;
                    if (player2.getTrade().getSlot(i) != null)
                    {
                        long p2tradeAmount = player2.getTrade().getAmountInSlot(i);
                        long invenAmount = player.getInventory().getItemAmount(player2.getTrade().getSlot(i).getItemId());
                        finalAmount = (long)(p2tradeAmount + invenAmount);
                        if (finalAmount >= int.MaxValue)
                        {
                            player.getPackets().sendMessage("You have too many of item: " + ItemData.forId(player2.getTrade().getSlot(i).getItemId()).getName() + ".");
                            return;
                        }
                    }
                }
                if (player2.getTrade().getAmountOfItems() > player.getInventory().getTotalFreeSlots())
                {
                    player.getPackets().sendMessage("You don't have enough inventory space for this trade.");
                    return;
                }
                if (getAmountOfItems() > player2.getInventory().getTotalFreeSlots())
                {
                    player.getPackets().sendMessage("Other player dosen't have enough inventory space for this trade.");
                    return;
                }
                this.status = 2;
            }
            if (status == 2)
            {
                player.getPackets().modifyText("Waiting for other player...", 335, 36);
                player2.getPackets().modifyText("Other player has accepted...", 335, 36);
                if (player2.getTrade().getStatus() == 2)
                {
                    displayConfirmation();
                    player2.getTrade().displayConfirmation();
                }
                return;
            }
            if (status == 3)
            {
                this.status = 4;
                player.getPackets().modifyText("Waiting for other player...", 334, 33);
                player2.getPackets().modifyText("Other player has accepted...", 334, 33);
                if (player2.getTrade().getStatus() == 4)
                {
                    completeTrade();
                    player2.getTrade().completeTrade();
                    player.getPackets().closeInterfaces();
                    player2.getPackets().closeInterfaces();
                    player.getPackets().sendMessage("You accept the trade.");
                    player2.getPackets().sendMessage("You accept the trade.");
                }
            }
        }

        private void completeTrade()
        {
            Item[] p2Items = player2.getTrade().getTradeItems();
            for (int i = 0; i < p2Items.Length; i++)
            {
                if (p2Items[i] != null)
                {
                    player.getInventory().addItem(p2Items[i].getItemId(), p2Items[i].getItemAmount());
                }
            }
        }

        public void decline()
        {
            player.getPackets().sendMessage("You decline the trade.");
            player2.getPackets().sendMessage("Other player declined the trade.");
            giveBack();
            player2.getTrade().giveBack();
            player.getPackets().closeInterfaces();
            player2.getPackets().closeInterfaces();
        }

        public void displayConfirmation()
        {
            this.status = 3;
            player.getPackets().displayInterface(334);
            player.getPackets().showChildInterface(334, 37, true);
            player.getPackets().showChildInterface(334, 41, true);
            player.getPackets().modifyText("Trading with: <br>" + player2.getLoginDetails().getUsername(), 334, 45);
            player.getPackets().modifyText(getItemList(), 334, 37);
            player.getPackets().modifyText(player2.getTrade().getItemList(), 334, 41);
            if (player2.getTrade().isTradeModified())
            {
                player.getPackets().showChildInterface(334, 46, true);
            }
        }

        public string getItemList()
        {
            string list = "";
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    list += "<col=FF9040>" + items[i].getDefinition().getName();
                    if (items[i].getItemAmount() > 1)
                    {
                        list += "<col=FFFFFF> x <col=FFFFFF>" + items[i].getItemAmount() + "<br>";
                    }
                    else
                    {
                        list += "<br>";
                    }
                }
            }
            if (list == "")
            {
                list = "<col=FFFFFF>Absolutely nothing!";
            }
            return list;
        }

        private void giveBack()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (!player.getInventory().addItem(items[i].getItemId(), items[i].getItemAmount()))
                    {
                        Misc.WriteError("Possible trade dupe " + player.getLoginDetails().getUsername());
                    }
                }
            }
        }

        public void setStatus(int i)
        {
            this.status = i;
        }

        public int getStatus()
        {
            return status;
        }

        public Item[] getTradeItems()
        {
            return items;
        }

        public void setLoanItem(Item loanItem)
        {
            this.loanItem = loanItem;
        }

        public Item getLoanItem()
        {
            return loanItem;
        }

        public bool isTradeModified()
        {
            return tradeModified;
        }

        public int getTotalFreeSlots()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    j++;
                }
            }
            return j;
        }

        public int findFreeSlot()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool hasItem(int itemId)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool hasItemAmount(int itemId, int amount)
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        j += items[i].getItemAmount();
                    }
                }
            }
            return j >= amount;
        }

        public int findItem(int itemId)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int getItemAmount(int itemId)
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        j += items[i].getItemAmount();
                    }
                }
            }
            return j;
        }

        public int getAmountOfItems()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() > -1)
                    {
                        j++;
                    }
                }
            }
            return j;
        }

        public int getAmountInSlot(int slot)
        {
            return items[slot].getItemAmount();
        }

        public int getItemInSlot(int slot)
        {
            return items[slot].getItemId();
        }

        public Item getSlot(int slot)
        {
            return items[slot];
        }
    }
}