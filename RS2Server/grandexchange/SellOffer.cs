namespace RS2.Server.grandexchange
{
    public class SellOffer : GEItem
    {
        public SellOffer()
        {
        }

        public SellOffer(int item, int totalAmount, int price, byte slot, string username)
        {
            setItem(item);
            setTotalAmount(totalAmount);
            setPriceEach(price);
            setPlayerName(username);
            setSlot(slot);
            setAmountTraded(0);
        }

        public override sbyte getAbortedBarId()
        {
            return -3;
        }

        public override sbyte getCompletedBarId()
        {
            return -3;
        }

        public override sbyte getOrangeBarId()
        {
            return -2;
        }

        public override sbyte getSubmittingId()
        {
            return -7;
        }

        public override int getDisplayItem()
        {
            return getUnNotedId();
        }
    }
}