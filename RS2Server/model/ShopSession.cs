using RS2.Server.definitions;
using RS2.Server.player;

namespace RS2.Server.model
{
    internal class ShopSession
    {
        private Shop shop;
        private Player player;
        private int shopId;
        private bool inMainStock;

        public ShopSession(Player p, int id)
        {
            this.player = p;
            this.shopId = id;
            this.shop = Server.getShopManager().getShop(id);
            openShop();
        }

        public void refreshShop()
        {
            player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
            player.getPackets().sendItems(-1, 64271, 31, shop.getStock());
            refreshGlobal();
        }

        private void refreshGlobal()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p == null || p.getShopSession() == null || p == player)
                {
                    continue;
                }
                if (p.getShopSession().getShopId() == shopId)
                {
                    p.getPackets().sendItems(-1, 64271, 31, shop.getStock());
                }
            }
        }

        public int getShopId()
        {
            return shopId;
        }

        public void openShop()
        {
            object[] invparams = new object[] { "", "", "", "", "Sell 50", "Sell 10", "Sell 5", "Sell 1", "Value", -1, 0, 7, 4, 93, 621 << 16 };
            object[] shopparams = new object[] { "", "", "", "", "Buy 50", "Buy 10", "Buy 5", "Buy 1", "Value", -1, 0, 4, 10, 31, (620 << 16) + 24 };
            player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
            player.getPackets().sendItems(-1, 64271, 31, shop.getStock());
            player.getPackets().showChildInterface(620, 34, shop.isGeneralStore());
            player.getPackets().modifyText(shop.getName(), 620, 22);
            player.getPackets().displayInterface(620);
            player.getPackets().displayInventoryInterface(621);
            player.getPackets().sendClientScript(150, invparams, "IviiiIsssssssss");
            player.getPackets().sendClientScript(150, shopparams, "IviiiIsssssssss");
            player.getPackets().setRightClickOptions(1278, (621 * 65536), 0, 27);
            player.getPackets().setRightClickOptions(2360446, (621 * 65536), 0, 27);
            if (shop.hasMainStock())
            {
                inMainStock = true;
                object[] setshopparams = new object[] { shop.getMainStock(), 93 };
                player.getPackets().sendClientScript(25, setshopparams, "vg");
                openMainShopInterface();
            }
            else
            {
                openPlayerShopInterface();
            }
        }

        public void openMainShop()
        {
            if (!inMainStock && shop.hasMainStock())
            {
                openMainShopInterface();
                inMainStock = true;
                return;
            }
            player.getPackets().sendMessage("This shop does not have a main stock.");
        }

        public void openPlayerShop()
        {
            if (inMainStock)
            {
                openPlayerShopInterface();
                inMainStock = false;
            }
        }

        public void openMainShopInterface()
        {
            player.getPackets().showChildInterface(620, 23, true);
            player.getPackets().showChildInterface(620, 24, false);
            player.getPackets().showChildInterface(620, 29, true);
            player.getPackets().showChildInterface(620, 25, false);
            player.getPackets().showChildInterface(620, 27, false);
            player.getPackets().showChildInterface(620, 26, true);
            player.getPackets().setRightClickOptions(1278, (620 * 65536) + 23, 0, 40);
        }

        public void openPlayerShopInterface()
        {
            player.getPackets().showChildInterface(620, 23, false);
            player.getPackets().showChildInterface(620, 24, true);
            player.getPackets().showChildInterface(620, 29, false);
            player.getPackets().showChildInterface(620, 25, true);
            player.getPackets().showChildInterface(620, 27, true);
            player.getPackets().showChildInterface(620, 26, false);
            player.getPackets().setRightClickOptions(1278, (620 * 65536) + 24, 0, 40);
        }

        public void buyItem(int slot, int amount)
        {
            Item item = null;
            int stockLength = inMainStock ? shop.getMainItems().Length : shop.getStock().Length;
            if (slot < 0 || slot > stockLength)
            {
                return;
            }
            if (inMainStock)
            {
                item = shop.getMainItem(slot);
            }
            else
            {
                item = shop.getStockItem(slot);
            }
            if (item == null || item.getItemAmount() < 1 || item.getItemId() < 1)
            {
                return;
            }
            if (ItemData.forId(item.getItemId()).isPlayerBound() && !inMainStock)
            {
                player.getPackets().sendMessage("How did this get in here..");
                return;
            }
            if (amount > item.getItemAmount())
            {
                amount = item.getItemAmount();
            }
            Inventory inv = player.getInventory();
            bool stackable = item.getDefinition().isStackable();
            int amountToAdd = amount;
            int itemToAdd = item.getItemId();
            int itemPrice = item.getDefinition().getPrice().getMaximumPrice();
            long totalPrice = (amountToAdd * itemPrice);
            if (totalPrice > int.MaxValue || totalPrice < 0)
            {
                amountToAdd = inv.getItemAmount(995) / itemPrice;
            }
            if (itemPrice <= 0)
            {
                itemPrice = 1;
            }
            if (!inv.hasItemAmount(shop.getCurrency(), itemPrice))
            {
                player.getPackets().sendMessage("You don't have enough coins to purchase that item.");
                return;
            }
            if (!stackable)
            {
                if (inv.findFreeSlot() == -1)
                {
                    player.getPackets().sendMessage("Not enough space in your inventory.");
                    return;
                }
                if (amount > 1)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        if (!inMainStock && shop.getStockItem(slot).getItemId() != itemToAdd || !inMainStock && shop.getStockItem(slot).getItemAmount() < 1)
                        {
                            player.getPackets().sendMessage("An error occured whilst buying the item.");
                            break;
                        }
                        if (!inv.deleteItem(shop.getCurrency(), itemPrice) && itemPrice > 0)
                        {
                            player.getPackets().sendMessage("You didn't have enough coins to purchase the full amount.");
                            break;
                        }
                        if (!inv.addItem(itemToAdd))
                        {
                            player.getPackets().sendMessage("You didn't have enough inventory space to purchase the full amount.");
                            break;
                        }
                        if (!inMainStock)
                        {
                            shop.getStockItem(slot).setItemAmount(shop.getStockItem(slot).getItemAmount() - 1);
                            if (shop.getStockItem(slot).getItemAmount() <= 0)
                            {
                                shop.getStockItem(slot).setItemId(-1);
                                shop.getStockItem(slot).setItemAmount(0);
                                refreshShop();
                                break;
                            }
                            refreshShop();
                        }
                        else
                        {
                            player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
                        }
                    }
                }
                else if (amount == 1)
                {
                    if (!inMainStock && shop.getStockItem(slot).getItemId() != itemToAdd || !inMainStock && shop.getStockItem(slot).getItemAmount() < 1)
                    {
                        player.getPackets().sendMessage("An error occured whilst buying the item.");
                        return;
                    }
                    if (!inv.deleteItem(shop.getCurrency(), itemPrice) && itemPrice > 0)
                    {
                        player.getPackets().sendMessage("You didn't have enough coins to purchase the full amount.");
                        return;
                    }
                    if (!inv.addItem(itemToAdd))
                    {
                        player.getPackets().sendMessage("You didn't have enough inventory space to purchase the full amount.");
                        return;
                    }
                    if (!inMainStock)
                    {
                        shop.getStockItem(slot).setItemAmount(shop.getStockItem(slot).getItemAmount() - 1);
                        if (shop.getStockItem(slot).getItemAmount() <= 0)
                        {
                            shop.getStockItem(slot).setItemId(-1);
                            shop.getStockItem(slot).setItemAmount(0);
                        }
                        refreshShop();
                        return;
                    }
                    player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
                    return;
                }
            }
            else
            {
                if (inv.findFreeSlot() == -1 && inv.findItem(itemToAdd) == -1)
                {
                    player.getPackets().sendMessage("Not enough space in your inventory.");
                    return;
                }
                if (!inMainStock && shop.getStockItem(slot).getItemId() != itemToAdd || !inMainStock && shop.getStockItem(slot).getItemAmount() < 1)
                {
                    player.getPackets().sendMessage("An error occured whilst buying the item.");
                    return;
                }
                bool moneyShort = false;
                if (!inv.hasItemAmount(shop.getCurrency(), itemPrice * amountToAdd))
                {
                    moneyShort = true;
                    amountToAdd = inv.getItemAmount(995) / itemPrice;
                    if (amountToAdd < 1)
                    {
                        player.getPackets().sendMessage("You don't have enough coins to purchase that item.");
                        return;
                    }
                }
                if (inv.deleteItem(shop.getCurrency(), itemPrice * amountToAdd) && itemPrice > 0)
                {
                    if (inv.addItem(itemToAdd, amountToAdd))
                    {
                        if (moneyShort)
                        {
                            player.getPackets().sendMessage("You didn't have enough money to purchase the full amount.");
                        }
                        if (!inMainStock)
                        {
                            shop.getStockItem(slot).setItemAmount(shop.getStockItem(slot).getItemAmount() - amountToAdd);
                            if (shop.getStockItem(slot).getItemAmount() <= 0)
                            {
                                shop.getStockItem(slot).setItemId(-1);
                                shop.getStockItem(slot).setItemAmount(0);
                            }
                            refreshShop();
                            return;
                        }
                        player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
                    }
                }
            }
        }

        //TODO redump GE prices with all 3 prices, then edit 'sellitem' to give min price
        //and 'buyitem' to give medium price

        public void sellItem(int slot, int amount)
        {
            Inventory inv = player.getInventory();
            if (slot < 0 || slot > 28)
            {
                return;
            }
            if (inv.getItemInSlot(slot) == -1)
            {
                return;
            }
            int itemId = inv.getItemInSlot(slot);
            bool noted = ItemData.forId(itemId).isNoted();
            int itemToRemove = itemId;
            long currentCurrencyInInven = player.getInventory().getItemAmount(shop.getCurrency());
            if (noted)
            {
                itemToRemove = ItemData.getUnNotedItem(itemId);
            }
            if (!shopWillBuyItem(itemToRemove))
            {
                return;
            }
            int price = ItemData.forId(itemToRemove).getPrice().getMinimumPrice();
            int shopSlot = shop.findItem(itemToRemove);
            if (shopSlot == -1)
            {
                shopSlot = shop.findFreeSlot();
                if (shopSlot == -1)
                {
                    player.getPackets().sendMessage("This shop is too full to buy anymore stock.");
                    return;
                }
            }
            if (inv.getAmountInSlot(slot) > amount)
            {
                if (inv.findFreeSlot() == -1 && !inv.hasItem(shop.getCurrency()))
                {
                    player.getPackets().sendMessage("Not enough space in your inventory.");
                    return;
                }
            }
            if (amount == 1)
            {
                if ((currentCurrencyInInven + price) >= int.MaxValue)
                {
                    player.getPackets().sendMessage("Not enough space in your inventory");
                    return;
                }
                if (!inv.deleteItem(itemId))
                {
                    return;
                }
                if (price > 0)
                {
                    inv.addItem(shop.getCurrency(), price);
                }
                shop.getSlot(shopSlot).setItemId(itemToRemove);
                shop.getSlot(shopSlot).setItemAmount(shop.getSlot(shopSlot).getItemAmount() + 1);
                refreshShop();
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    if ((currentCurrencyInInven + price) >= int.MaxValue)
                    {
                        player.getPackets().sendMessage("Not enough space in your inventory");
                        break;
                    }
                    if (!inv.deleteItem(itemId))
                    {
                        break;
                    }
                    if (price > 0)
                    {
                        inv.addItem(shop.getCurrency(), price);
                        currentCurrencyInInven += price;
                    }
                    shop.getSlot(shopSlot).setItemId(itemToRemove);
                    shop.getSlot(shopSlot).setItemAmount(shop.getSlot(shopSlot).getItemAmount() + 1);
                }
                refreshShop();
            }
            if (inMainStock)
            {
                openPlayerShop();
            }
        }

        private bool shopWillBuyItem(int itemId)
        {
            bool canSell = shop.isGeneralStore();
            if (!shop.isGeneralStore())
            {
                foreach (Item i in shop.getMainItems())
                {
                    if (itemId == i.getItemId())
                    {
                        canSell = true;
                    }
                }
            }
            if (!canSell || ItemData.forId(itemId).isPlayerBound() || itemId == 995)
            {
                player.getPackets().sendMessage("This shop will not buy that item.");
                return false;
            }
            return true;
        }

        public void valueItem(int slot, bool inventory)
        {
            Item item = inventory ? player.getInventory().getSlot(slot) : inMainStock ? shop.getMainItem(slot) : shop.getSlot(slot);
            int id = item.getItemId();
            if (item.getItemId() <= 0)
            {
                return;
            }
            bool noted = ItemData.forId(item.getItemId()).isNoted();
            if (noted)
            {
                id = ItemData.getUnNotedItem(item.getItemId());
            }
            if (!shopWillBuyItem(id))
            {
                return;
            }
            ItemData.Item def = ItemData.forId(id);
            if (inventory)
            {
                player.getPackets().sendMessage("This shop will pay " + def.getPrice().getMinimumPrice().ToString("#,##0") + " coins for 1 " + def.getName() + ".");
                return;
            }
            player.getPackets().sendMessage("1 " + def.getName() + " costs " + def.getPrice().getMaximumPrice().ToString("#,##0") + " coins.");
        }

        public bool isInMainStock()
        {
            return inMainStock;
        }

        public void closeShop()
        {
            player.setShopSession(null);
        }
    }
}