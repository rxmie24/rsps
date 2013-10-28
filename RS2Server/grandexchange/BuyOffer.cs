namespace RS2.Server.grandexchange
{
    public class BuyOffer : GEItem
    {
        /*public BuyOffer(int item, int totalAmount, int price, byte slot, Player p) {
		    setItem(item);
		    setTotalAmount(totalAmount);
		    setPriceEach(price);
		    setPlayerName(p.getUsername());
		    setSlot(slot);
		    setAmountTraded(0);
	    }*/

        public BuyOffer(byte slot, string playerName)
        {
            setSlot(slot);
            setPlayerName(playerName);
        }

        public BuyOffer()
        {
        }

        public override sbyte getAbortedBarId()
        {
            return 5;
        }

        public override sbyte getCompletedBarId()
        {
            return 5;
        }

        public override sbyte getOrangeBarId()
        {
            return 2;
        }

        public override sbyte getSubmittingId()
        {
            return 1;
        }

        public override int getDisplayItem()
        {
            return getItem();
        }
    }
}