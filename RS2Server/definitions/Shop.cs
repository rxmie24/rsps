using RS2.Server.model;

namespace RS2.Server.definitions
{
    public class Shop
    {
        public int id; //shop id, fix for not able to serialize Dictionary, changed shops.xml format
        public string name;
        public bool generalStore;
        public bool mainStock;
        public int mainStockAmount;
        public Item[] stock;
        public Item[] mainItems;

        public Shop()
        {
        }

        public object readResolve()
        {
            foreach (Item item in stock)
            {
                if (item.getItemId() <= 0)
                {
                    item.setItemId(-1);
                }
            }
            return this;
        }

        public void updateAmounts()
        {
            for (int i = 0; i < stock.Length; i++)
            {
                if (stock[i].getItemAmount() > 0)
                {
                    stock[i].setItemAmount(stock[i].getItemAmount() - 1);
                    if (stock[i].getItemAmount() <= 0)
                    {
                        stock[i].setItemId(-1);
                        stock[i].setItemAmount(0);
                    }
                }
            }
        }

        public string getName()
        {
            return name;
        }

        public bool isGeneralStore()
        {
            return generalStore;
        }

        public Item[] getStock()
        {
            return stock;
        }

        public Item[] getMainItems()
        {
            return mainItems;
        }

        public bool hasMainStock()
        {
            return mainStock;
        }

        public int getMainStock()
        {
            return mainStockAmount;
        }

        public Item getMainItem(int slot)
        {
            return mainItems[slot];
        }

        public Item getStockItem(int slot)
        {
            return stock[slot];
        }

        public int getCurrency()
        {
            return 995;
        }

        public int findItem(int itemId)
        {
            for (int i = 0; i < stock.Length; i++)
            {
                if (stock[i].getItemId() == itemId)
                {
                    return i;
                }
            }
            return -1;
        }

        public int findFreeSlot()
        {
            for (int i = 0; i < stock.Length; i++)
            {
                if (stock[i].getItemId() <= 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public Item getSlot(int slot)
        {
            return stock[slot];
        }

        public void setSlot(int slot, Item item)
        {
            this.stock[slot] = item;
        }

        public void setStock(Item[] stock)
        {
            this.stock = stock;
        }
    }
}