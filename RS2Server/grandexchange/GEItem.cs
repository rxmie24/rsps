using RS2.Server.definitions;
using RS2.Server.model;

namespace RS2.Server.grandexchange
{
    public abstract class GEItem
    {
        public string playerName;
        public int item;
        public int totalAmount;
        public int amountTraded;
        public int priceEach;
        public byte slot;
        public bool aborted;
        public sbyte progress;
        public Item slot1;
        public Item slot2;
        public int unNotedId;

        public abstract sbyte getSubmittingId();

        public abstract sbyte getOrangeBarId();

        public abstract sbyte getCompletedBarId();

        public abstract sbyte getAbortedBarId();

        public abstract int getDisplayItem();

        public GEItem()
        {
            this.aborted = false;
            this.progress = 0;
        }

        public void setItem(int item)
        {
            this.item = item;
        }

        public string getPlayerName()
        {
            return playerName;
        }

        public void setPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        public int getTotalAmount()
        {
            return totalAmount;
        }

        public void setTotalAmount(int totalAmount)
        {
            this.totalAmount = totalAmount;
        }

        public int getAmountTraded()
        {
            return amountTraded;
        }

        public void setAmountTraded(int amountTraded)
        {
            this.amountTraded = amountTraded;
        }

        public int getItem()
        {
            return item;
        }

        public void setPriceEach(int priceEach)
        {
            this.priceEach = priceEach;
        }

        public int getPriceEach()
        {
            return priceEach;
        }

        public void setSlot(byte slot)
        {
            this.slot = slot;
        }

        public byte getSlot()
        {
            return slot;
        }

        public void setAborted(bool aborted)
        {
            this.aborted = aborted;
        }

        public bool isAborted()
        {
            return aborted;
        }

        public void setProgress(sbyte progress)
        {
            this.progress = progress;
        }

        public sbyte getProgress()
        {
            return progress;
        }

        public Item getSlot1()
        {
            return slot1;
        }

        public void setSlot1(Item slot1)
        {
            this.slot1 = slot1;
        }

        public Item getSlot2()
        {
            return slot2;
        }

        public void setSlot2(Item slot2)
        {
            this.slot2 = slot2;
        }

        public void setUnNotedId(int id)
        {
            this.unNotedId = id;
        }

        public int getUnNotedId()
        {
            return unNotedId;
        }

        public ItemData.ItemPrice getItemPrice()
        {
            return ItemData.forId(getUnNotedId() > 0 ? getUnNotedId() : getItem()).getPrice();
        }
    }
}