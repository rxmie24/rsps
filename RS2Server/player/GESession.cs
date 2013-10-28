using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.grandexchange;
using RS2.Server.model;

namespace RS2.Server.player
{
    internal class GESession
    {
        private Player p;
        private GEItem currentOffer;
        private byte slot;

        public GESession(Player p)
        {
            this.p = p;
            //Load offers.
            GEItem load;
            for (int i = 0; i < 6; i++)
            {
                load = Server.getGrandExchange().getOfferForSlot(p, i);
                if (load != null)
                    p.getPackets().updateGEProgress(load);
            }
            openExchange();
        }

        public void openExchange()
        {
            p.getPackets().sendConfig(1109, -1);
            p.getPackets().sendConfig(1110, 0);
            p.getPackets().sendConfig(1111, 0);
            p.getPackets().sendConfig(1112, -1);
            p.getPackets().sendConfig(1113, -1);
            p.getPackets().sendConfig(1114, 0);
            p.getPackets().sendConfig(1115, 0);
            p.getPackets().sendConfig(1116, 0);
            p.getPackets().displayInterface(105);
            p.getPackets().setRightClickOptions(6, 6881491, 65535, 65535);
            p.getPackets().setRightClickOptions(6, 6881489, 65535, 65535);
        }

        public void newBuyOffer(byte slot)
        {
            this.slot = slot;
            this.currentOffer = new BuyOffer();
            p.getPackets().sendConfig(1109, -1);
            p.getPackets().sendConfig(1110, 0);
            p.getPackets().sendConfig(1111, 0);
            p.getPackets().sendConfig(1112, slot);
            p.getPackets().sendConfig(1113, 0);
            p.getPackets().sendConfig(1114, 0);
            p.getPackets().sendConfig(1115, 0);
            p.getPackets().sendConfig(1116, 0);
            openItemSearch();
        }

        public void newSellOffer(byte slot)
        {
            this.slot = slot;
            p.getPackets().sendConfig(1109, -1);
            p.getPackets().sendConfig(1110, 0);
            p.getPackets().sendConfig(1111, 0);
            p.getPackets().sendConfig(1112, slot);
            p.getPackets().sendConfig(1113, 1);
            p.getPackets().sendConfig(1114, 0);
            p.getPackets().sendConfig(1115, 0);
            p.getPackets().sendConfig(1116, 0);
            p.getPackets().displayInventoryInterface(107);
            p.getPackets().setRightClickOptions(1026, 107 * 65536 + 18, 0, 27);
            object[] opts = new object[] { "", "", "", "", "Offer", -1, 0, 7, 4, 93, 7012370 };
            p.getPackets().sendClientScript(149, opts, "IviiiIsssss");
            p.getPackets().sendItems(-1, 65535, 93, p.getInventory().getItems());
        }

        public void openItemSearch()
        {
            p.getPackets().sendInterface(0, 752, 6, 389);
            p.getPackets().sendClientScript(570, new object[] { "Grand Exchange Item Search" }, "s");
        }

        public void updateSearchItem(int item)
        {
            p.getPackets().sendConfig(1109, item);
            p.getPackets().sendConfig(1110, 0);
            ItemData.Item def = ItemData.forId(item);
            if (def == null)
                return;
            p.getPackets().sendConfig(1109, item);
            p.getPackets().sendConfig(1114, def.getPrice().getNormalPrice());
            p.getPackets().sendConfig(1116, def.getPrice().getMaximumPrice());
            p.getPackets().sendConfig(1115, def.getPrice().getMinimumPrice());
            p.getPackets().sendConfig(1111, def.getPrice().getNormalPrice());
            currentOffer = new BuyOffer(slot, p.getLoginDetails().getUsername());
            currentOffer.setTotalAmount(0);
            currentOffer.setItem(item);
            currentOffer.setPriceEach(def.getPrice().getNormalPrice());
            p.getPackets().sendInterface(0, 752, 6, 137); // Removes the item search
        }

        public void setCustomAmount(int amount)
        {
            if (currentOffer != null)
            {
                currentOffer.setTotalAmount(amount);
                if (currentOffer.getTotalAmount() < 1)
                {
                    currentOffer.setTotalAmount(0);
                }
                p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
            }
        }

        public void incrementAmount(int increment)
        {
            if (currentOffer != null)
            {
                if (currentOffer is SellOffer)
                {
                    if (increment == 1000)
                    {
                        currentOffer.setTotalAmount(p.getInventory().getItemAmount(currentOffer.getItem()));
                        p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
                        return;
                    }
                    else
                    {
                        currentOffer.setTotalAmount(increment);
                    }
                }
                else
                {
                    currentOffer.setTotalAmount(currentOffer.getTotalAmount() + increment);
                }
                p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
            }
        }

        public void decreaseAmount(int decrement)
        {
            if (currentOffer != null)
            {
                currentOffer.setTotalAmount(currentOffer.getTotalAmount() - decrement);
                if (currentOffer.getTotalAmount() < 1)
                {
                    currentOffer.setTotalAmount(0);
                }
                p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
            }
        }

        public void offerSellItem(int inventorySlot)
        {
            Item item = p.getInventory().getSlot(inventorySlot);
            if (item != null && item.getItemId() > 0 && item.getItemAmount() > 0)
            {
                int itemToShow = item.getItemId();
                ItemData.Item def = item.getDefinition();

                if (def == null)
                {
                    p.getPackets().sendMessage("This item cannot be found in item definitions, please report it.");
                    return;
                }

                if (def.isNoted())
                {
                    itemToShow = ItemData.getUnNotedItem(item.getItemId());
                    if (itemToShow == item.getItemId())
                    { // item is only noted
                        p.getPackets().sendMessage("An unnoted equivalent of this item cannot be found, please report it.");
                        return;
                    }
                    else
                    {
                        def = ItemData.forId(itemToShow); //update item def with un-noted def.. to fix price.
                    }
                }

                if (def.isPlayerBound() || (def.getPrice().getNormalPrice() == 0 && def.getPrice().getMaximumPrice() == 0 && def.getPrice().getMinimumPrice() == 0))
                { //look at unNoted item price, of a item which was previously noted.
                    p.getPackets().sendMessage("This item cannot be sold on the Grand Exchange.");
                    return;
                }

                p.getPackets().sendConfig(1109, itemToShow);
                p.getPackets().sendConfig(1110, item.getItemAmount());
                p.getPackets().sendConfig(1114, def.getPrice().getNormalPrice());
                p.getPackets().sendConfig(1116, def.getPrice().getMaximumPrice());
                p.getPackets().sendConfig(1115, def.getPrice().getMinimumPrice());
                p.getPackets().sendConfig(1111, def.getPrice().getNormalPrice());
                currentOffer = new SellOffer(item.getItemId(), item.getItemAmount(), def.getPrice().getNormalPrice(), slot, p.getLoginDetails().getUsername());
                currentOffer.setUnNotedId(item.getDefinition().isNoted() ? itemToShow : item.getItemId());
            }
        }

        public void setCustomPrice(int amount)
        {
            if (currentOffer != null)
            {
                ItemData.ItemPrice price = currentOffer.getItemPrice();
                if (amount < price.getMinimumPrice())
                    currentOffer.setPriceEach(price.getMinimumPrice());
                else if (amount > price.getMaximumPrice())
                    currentOffer.setPriceEach(price.getMaximumPrice());
                else
                    currentOffer.setPriceEach(amount);
                p.getPackets().sendConfig(1111, currentOffer.getPriceEach());
            }
        }

        public void setPrice(int i)
        {
            if (currentOffer != null)
            {
                ItemData.ItemPrice price = currentOffer.getItemPrice();
                if (i == 0)
                { // Min
                    currentOffer.setPriceEach(price.getMinimumPrice());
                }
                else if (i == 1)
                { // Med
                    currentOffer.setPriceEach(price.getNormalPrice());
                }
                else if (i == 2)
                { // Max
                    currentOffer.setPriceEach(price.getMaximumPrice());
                }
                else if (i == 3)
                { // -1
                    currentOffer.setPriceEach(currentOffer.getPriceEach() - 1);
                    if (currentOffer.getPriceEach() <= price.getMinimumPrice())
                        currentOffer.setPriceEach(price.getMinimumPrice());
                }
                else if (i == 4)
                { // +1
                    currentOffer.setPriceEach(currentOffer.getPriceEach() + 1);
                    if (currentOffer.getPriceEach() >= price.getMaximumPrice())
                        currentOffer.setPriceEach(price.getMaximumPrice());
                }
                p.getPackets().sendConfig(1111, currentOffer.getPriceEach());
            }
        }

        public void checkOffer(int slot)
        {
            this.currentOffer = Server.getGrandExchange().getOfferForSlot(p, slot);
            if (currentOffer != null)
            {
                ItemData.ItemPrice price = currentOffer.getItemPrice();
                p.getPackets().sendConfig(1109, currentOffer.getItem());
                p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
                p.getPackets().sendConfig(1111, currentOffer.getPriceEach());
                p.getPackets().sendConfig(1112, currentOffer.getSlot());
                p.getPackets().sendConfig(1113, 0);
                p.getPackets().sendConfig(1114, price.getNormalPrice());
                p.getPackets().sendConfig(1116, price.getMaximumPrice());
                p.getPackets().sendConfig(1115, price.getMinimumPrice());
                Item slot1 = currentOffer.getSlot1();
                Item slot2 = currentOffer.getSlot2();
                if (currentOffer is BuyOffer)
                {
                    //slot1 = currentOffer.getAmountTraded() == 0 ? null : new Item(currentOffer.getItem(), currentOffer.getAmountTraded());
                    //slot2 = (currentOffer.getAmountTraded() == currentOffer.getTotalAmount()) ||  currentOffer.getAmountTraded() == 0 ? null : new Item(995, (currentOffer.getTotalAmount() - currentOffer.getAmountTraded()) * currentOffer.getPriceEach());
                }
                else
                {
                    //slot1 = (currentOffer.getAmountTraded() == currentOffer.getTotalAmount()) ||  currentOffer.getAmountTraded() == 0 ? null : new Item(currentOffer.getUnNotedId(), currentOffer.getTotalAmount() - currentOffer.getAmountTraded());
                    //slot2 = currentOffer.getAmountTraded() == 0 ? null : new Item(995, (currentOffer.getAmountTraded()) * currentOffer.getPriceEach());
                }
                Item[] items = { slot1, slot2 };
                currentOffer.setSlot1(slot1);
                currentOffer.setSlot2(slot2);
                p.getPackets().sendItems(-1, -1757, 523 + currentOffer.getSlot(), items);
            }
        }

        public void confirmOffer()
        {
            if (currentOffer == null)
            {
                return;
            }
            if (currentOffer is BuyOffer)
            {
                int gpAmount = currentOffer.getTotalAmount() * currentOffer.getPriceEach();
                if (currentOffer.getTotalAmount() <= 0)
                {
                    p.getPackets().sendMessage("You must choose the quantity you wish to buy!");
                    return;
                }
                else if (!p.getInventory().hasItemAmount(995, gpAmount))
                {
                    p.getPackets().sendMessage("You don't have enough coins in your inventory to cover the offer.");
                    return;
                }
                else if (!p.getInventory().deleteItem(995, gpAmount))
                {
                    return;
                }
            }
            else if (currentOffer is SellOffer)
            {
                if (currentOffer.getTotalAmount() <= 0)
                {
                    p.getPackets().sendMessage("You must choose the quantity you wish to sell!");
                    return;
                }
                else if (!p.getInventory().hasItemAmount(currentOffer.getItem(), currentOffer.getTotalAmount()))
                {
                    p.getPackets().sendMessage("You do not have enough of this item in your inventory to cover the offer.");
                    return;
                }
                if (ItemData.forId(currentOffer.getItem()).isNoted() || ItemData.forId(currentOffer.getItem()).isStackable())
                {
                    if (!p.getInventory().deleteItem(currentOffer.getItem(), currentOffer.getTotalAmount()))
                    {
                        return;
                    }
                }
                else
                {
                    //UnNoted variant of this item, so remove multiple items from inventory.
                    int i = 0;
                    for (int j = 0; j < currentOffer.getTotalAmount(); j++)
                    {
                        if (!p.getInventory().deleteItem(currentOffer.getUnNotedId()))
                        {
                            currentOffer.setTotalAmount(i);
                            p.getPackets().sendConfig(1110, currentOffer.getTotalAmount());
                            break;
                        }
                        i++;
                    }
                }
            }
            p.getPackets().sendConfig(1113, -1);
            p.getPackets().sendConfig(1112, -1);
            currentOffer.setProgress(currentOffer.getSubmittingId());
            p.getPackets().updateGEProgress(currentOffer);
            Server.getGrandExchange().addOffer(currentOffer);
            GEItem offer = currentOffer;
            currentOffer = null;
            Event updateGEProgressEvent = new Event(500);
            updateGEProgressEvent.setAction(() =>
            {
                updateGEProgressEvent.stop();
                offer.setProgress(offer.getOrangeBarId());
                p.getPackets().updateGEProgress(offer);
            });
            Server.registerEvent(updateGEProgressEvent);
        }

        public void abortOffer()
        {
            if (currentOffer != null)
            {
                if (currentOffer.isAborted())
                {
                    return;
                }
                Item slot1 = null;
                Item slot2 = null;
                if (currentOffer is BuyOffer)
                {
                    slot1 = currentOffer.getAmountTraded() == 0 ? null : new Item(currentOffer.getItem(), currentOffer.getAmountTraded());
                    slot2 = currentOffer.getAmountTraded() == currentOffer.getTotalAmount() ? null : new Item(995, (currentOffer.getTotalAmount() - currentOffer.getAmountTraded()) * currentOffer.getPriceEach());
                }
                else
                {
                    slot1 = currentOffer.getAmountTraded() == currentOffer.getTotalAmount() ? null : new Item(currentOffer.getUnNotedId(), currentOffer.getTotalAmount() - currentOffer.getAmountTraded());
                    slot2 = currentOffer.getAmountTraded() == 0 ? null : new Item(995, (currentOffer.getAmountTraded()) * currentOffer.getPriceEach());
                }
                Item[] items = { slot1, slot2 };
                currentOffer.setSlot1(slot1);
                currentOffer.setSlot2(slot2);
                p.getPackets().sendItems(-1, -1757, 523 + currentOffer.getSlot(), items);
                currentOffer.setProgress(currentOffer.getAbortedBarId());
                currentOffer.setAborted(true);
                p.getPackets().updateGEProgress(currentOffer);
            }
        }

        public void collectSlot1(bool noted)
        {
            if (currentOffer != null)
            {
                if (currentOffer.getSlot1() != null)
                {
                    int item = currentOffer.getSlot1().getItemId();
                    bool stackable = ItemData.forId(item).isStackable();
                    if (noted)
                    {
                        int notedId = ItemData.getNotedItem(item);
                        if (notedId == item)
                        {
                            // Cant be withdrawn as a note.
                            if (stackable)
                            {
                                // arrows etc
                                if (p.getInventory().getTotalFreeSlots() == 0 && !p.getInventory().hasItem(item))
                                {
                                    p.getPackets().sendMessage("Your inventory is full.");
                                }
                                else
                                {
                                    if (p.getInventory().addItem(item, currentOffer.getSlot1().getItemAmount()))
                                    {
                                        //currentOffer.setTotalAmount(currentOffer.getTotalAmount() - currentOffer.getSlot1().getItemAmount());
                                        p.getPackets().sendMessage("This item cannot be collected as a note.");
                                        currentOffer.setSlot1(null);
                                    }
                                }
                            }
                            else
                            {
                                // isnt noted... and isnt stackable, withdraw as regular items
                                for (int i = 0; i < currentOffer.getSlot1().getItemAmount(); i++)
                                {
                                    if (!p.getInventory().addItem(item))
                                    {
                                        break;
                                    }
                                    currentOffer.getSlot1().setItemAmount(currentOffer.getSlot1().getItemAmount() - 1);
                                    //currentOffer.setTotalAmount(currentOffer.getTotalAmount() - 1);
                                    if (currentOffer.getSlot1().getItemAmount() <= 0)
                                    {
                                        currentOffer.setSlot1(null);
                                        break;
                                    }
                                }
                                p.getPackets().sendMessage("This item cannot be collected as a note.");
                            }
                        }
                        else
                        {
                            // is noted
                            if (p.getInventory().getTotalFreeSlots() == 0 && !p.getInventory().hasItem(notedId))
                            {
                                p.getPackets().sendMessage("Your inventory is full.");
                            }
                            else
                            {
                                if (p.getInventory().addItem(notedId, currentOffer.getSlot1().getItemAmount()))
                                {
                                    //	currentOffer.setTotalAmount(currentOffer.getTotalAmount() - currentOffer.getSlot1().getItemAmount());
                                    currentOffer.setSlot1(null);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Unnoted
                        if (stackable)
                        {
                            // arrows etc
                            if (p.getInventory().getTotalFreeSlots() == 0 && !p.getInventory().hasItem(item))
                            {
                                p.getPackets().sendMessage("Your inventory is full.");
                            }
                            else
                            {
                                if (p.getInventory().addItem(item, currentOffer.getSlot1().getItemAmount()))
                                {
                                    //currentOffer.setTotalAmount(currentOffer.getTotalAmount() - currentOffer.getSlot1().getItemAmount());
                                    currentOffer.setSlot1(null);
                                }
                            }
                        }
                        else
                        {
                            //regular items
                            for (int i = 0; i < currentOffer.getSlot1().getItemAmount(); i++)
                            {
                                if (!p.getInventory().addItem(item))
                                {
                                    break;
                                }
                                //currentOffer.setTotalAmount(currentOffer.getTotalAmount() - 1);
                                currentOffer.getSlot1().setItemAmount(currentOffer.getSlot1().getItemAmount() - 1);
                                if (currentOffer.getSlot1().getItemAmount() <= 0)
                                {
                                    currentOffer.setSlot1(null);
                                    break;
                                }
                            }
                        }
                    }
                    Item[] items = { currentOffer.getSlot1(), currentOffer.getSlot2() };
                    p.getPackets().sendItems(-1, -1757, 523 + currentOffer.getSlot(), items);
                    if (((currentOffer.getAmountTraded() == currentOffer.getTotalAmount()) || currentOffer.isAborted()) && currentOffer.getSlot1() == null && currentOffer.getSlot2() == null)
                    {
                        if (Server.getGrandExchange().removeOffer(currentOffer))
                        {
                            resetInterface();
                            currentOffer = null;
                        }
                    }
                }
            }
        }

        public void collectSlot2()
        {
            if (currentOffer != null)
            {
                if (currentOffer.getSlot2() != null)
                {
                    if (p.getInventory().addItem(995, currentOffer.getSlot2().getItemAmount()))
                    {
                        currentOffer.setSlot2(null);
                        Item[] items = { currentOffer.getSlot1(), currentOffer.getSlot2() };
                        p.getPackets().sendItems(-1, -1757, 523 + currentOffer.getSlot(), items);
                    }
                    if (((currentOffer.getAmountTraded() == currentOffer.getTotalAmount()) || currentOffer.isAborted()) && currentOffer.getSlot1() == null && currentOffer.getSlot2() == null)
                    {
                        if (Server.getGrandExchange().removeOffer(currentOffer))
                        {
                            resetInterface();
                            currentOffer = null;
                        }
                    }
                }
            }
        }

        public void resetInterface()
        {
            p.getPackets().sendConfig(1109, -1);
            p.getPackets().sendConfig(1110, -1);
            p.getPackets().sendConfig(1112, -1);
            p.getPackets().sendConfig(1113, -1);
            p.getPackets().sendConfig(1114, -1);
            p.getPackets().sendConfig(1116, -1);
            p.getPackets().sendConfig(1115, -1);
            p.getPackets().sendConfig(1111, -1);
            p.getPackets().resetGESlot(currentOffer.getSlot());
        }

        public GEItem getCurrentOffer()
        {
            return currentOffer;
        }
    }
}