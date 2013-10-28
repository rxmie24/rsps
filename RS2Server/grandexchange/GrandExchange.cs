using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace RS2.Server.grandexchange
{
    internal class GrandExchange
    {
        /*
          1109: item id
          1110: amount
          1111: price per item
          1112: id of the buy/sell screen  (-1 is main screen)
          1113: main screen -1 / buy 0 / sell 1
          1114: min price
          1115: normal price
          1116: max price

          progress
          0 = nothing (dosent show the item..used when removing it)
          1 - "submitting..."
          2 - orange bar (% bought)?
          3 - orange bar (% bought)?
          4 - orange bar (% bought)?
          5 - aborted buy item
          6 - orange bar (% bought)?
          7 - orange bar (% bought)?
          -3 is orange bar sell item
          -1 = sell progress?
         */

        private Dictionary<int, GEItem[]> buyOffers;
        private Dictionary<int, GEItem[]> sellOffers;

        public GrandExchange()
        {
            buyOffers = new Dictionary<int, GEItem[]>();
            sellOffers = new Dictionary<int, GEItem[]>();

            /* Load grand exchange items */
            Console.WriteLine("Loading Grand Exchange Items");
            try
            { //TODO: Convert to ADO.NET / MDB clientside database.
                if (File.Exists(Misc.getServerPath() + @"\buyOffers.xml"))
                {
                    //Deserialize text file to a new object.
                    StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\buyOffers.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(GEDictionaryWrapper), new Type[] { typeof(BuyOffer) });
                    GEDictionaryWrapper geDictionaryWrapper = (GEDictionaryWrapper)serializer.Deserialize(objStreamReader);
                    objStreamReader.Close();
                    buyOffers = geDictionaryWrapper.GetMap();
                    Console.WriteLine("Loaded " + buyOffers.Count + " buy offers in Grand Exchange");
                }
                if (File.Exists(Misc.getServerPath() + @"\sellOffers.xml"))
                {
                    //Deserialize text file to a new object.
                    StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\sellOffers.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(GEDictionaryWrapper), new Type[] { typeof(SellOffer) });
                    GEDictionaryWrapper geDictionaryWrapper = (GEDictionaryWrapper)serializer.Deserialize(objStreamReader);
                    objStreamReader.Close();
                    sellOffers = geDictionaryWrapper.GetMap();
                    Console.WriteLine("Loaded " + sellOffers.Count + " sell offers in Grand Exchange");
                }
            }
            catch (Exception e)
            {
                Misc.WriteError(e.Message);
            }

            Event startGEEvent = new Event(5000);
            startGEEvent.setAction(() =>
            {
                processSales();
            });
            Server.registerEvent(startGEEvent);
            /*
             * This saves the grand exchange every 60 seconds.
             * Decided not to make it a event because it will impact performance when
             * grand exchange really packs up, as it does I/O operations to harddrive.
             */
            new Thread(saveGrandExchange).Start();
        }

        protected void processSales()
        {
            for (int i = 0; i < Constants.MAX_ITEMS; i++)
            {
                GEItem[] buyerArray;
                GEItem[] sellerArray;
                buyOffers.TryGetValue(i, out buyerArray);
                sellOffers.TryGetValue(i, out sellerArray);

                if (buyerArray == null || sellerArray == null)
                    continue;

                /*
                //Sells items to the buyers who is buying the biggest item amount, first.
                long totalWanted = 0;
                int totalBuyers = 0;
                foreach(GEItem buyer in buyerArray) {
                    if (!buyer.isAborted()) {
                        totalWanted += (buyer.getTotalAmount() - buyer.getAmountTraded()); // how many are wanting to be bought
                        totalBuyers++; // how many people are buying
                    }
                }
                long totalForSale = 0;
                int totalSellers = 0;
                foreach(GEItem seller in sellerArray) {
                    if (!seller.isAborted()) {
                        totalForSale += (seller.getTotalAmount() - seller.getAmountTraded()); // how many are for sale
                        totalSellers++; // how many people are selling
                    }
                }
                long highestAmount = totalForSale > totalWanted ? totalForSale : totalWanted;
                long lowestAmount = totalForSale < totalWanted ? totalForSale : totalWanted;*/
                foreach (GEItem buyer in buyerArray)
                {
                    int amountToBuy = (buyer.getTotalAmount() - buyer.getAmountTraded());
                    if (buyer.isAborted() || amountToBuy <= 0)
                    {
                        continue;
                    }
                    foreach (GEItem seller in sellerArray)
                    {
                        int amountToSell = (seller.getTotalAmount() - seller.getAmountTraded());
                        if (seller.isAborted() || amountToSell <= 0)
                        {
                            continue;
                        }
                        if (buyer.getPriceEach() >= seller.getPriceEach())
                        { // buyer will pay min what the seller wants
                            int amount = Misc.random(seller.getTotalAmount() - seller.getAmountTraded());
                            if (amount == 0)
                                amount++;
                            else if (amount > amountToBuy)
                                amount = amountToBuy;
                            int buyerPriceDifference = (buyer.getPriceEach() - seller.getPriceEach()) * amount; // buyer is paying more than the seller wants, therefore recieves this amount as a refund
                            bool buyerKeepsRefund = Misc.random(1) == 0; // if 0, the buyer gets a refund, if its 1...the seller gets more.
                            buyer.setAmountTraded(buyer.getAmountTraded() + amount);
                            seller.setAmountTraded(seller.getAmountTraded() + amount);
                            amountToBuy -= amount;
                            amountToSell -= amount;
                            Item buyerSlot1 = buyer.getSlot1();
                            if (buyerSlot1 == null)
                                buyer.setSlot1(new Item(buyer.getItem(), buyer.getAmountTraded()));
                            else
                                buyerSlot1.setItemAmount(buyer.getSlot1().getItemAmount() + amount);

                            if (buyerPriceDifference > 0 && buyerKeepsRefund)
                            {
                                Item buyerSlot2 = buyer.getSlot2();
                                if (buyerSlot2 == null)
                                    buyer.setSlot2(new Item(995, buyerPriceDifference));
                                else
                                    buyerSlot2.setItemAmount(buyer.getSlot2().getItemAmount() + buyerPriceDifference);
                                //we've already refunded the buyer, so set the buyerPriceDifference to 0 so the seller dosent get it too!
                                buyerPriceDifference = 0;
                            }
                            Item sellerSlot2 = seller.getSlot2();
                            if (sellerSlot2 == null)
                                seller.setSlot2(new Item(995, (seller.getPriceEach() + buyerPriceDifference) * seller.getAmountTraded()));
                            else
                                sellerSlot2.setItemAmount(seller.getSlot2().getItemAmount() + (seller.getPriceEach() * amount) + buyerPriceDifference);
                            Player buyerP = Server.getPlayerForName(buyer.getPlayerName());
                            Player sellerP = Server.getPlayerForName(seller.getPlayerName());
                            if (buyer.getAmountTraded() == buyer.getTotalAmount())
                                buyer.setProgress(buyer.getCompletedBarId());
                            if (seller.getAmountTraded() == seller.getTotalAmount())
                                seller.setProgress(seller.getCompletedBarId());

                            if (buyerP != null)
                            {
                                buyerP.getPackets().sendMessage("One or more of your Grand Exchange offers has been updated.");
                                buyerP.getPackets().updateGEProgress(buyer);
                                if (buyerP.getGESession() != null)
                                {
                                    Item[] items = { buyer.getSlot1(), buyer.getSlot2() };
                                    buyerP.getPackets().sendItems(-1, -1757, 523 + buyer.getSlot(), items);
                                }
                            }
                            if (sellerP != null)
                            {
                                sellerP.getPackets().sendMessage("One or more of your Grand Exchange offers has been updated.");
                                sellerP.getPackets().updateGEProgress(seller);
                                if (sellerP.getGESession() != null)
                                {
                                    Item[] items = { seller.getSlot1(), seller.getSlot2() };
                                    sellerP.getPackets().sendItems(-1, -1757, 523 + seller.getSlot(), items);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void saveGrandExchange()
        {
            XmlSerializer buySerializer = new XmlSerializer(typeof(GEDictionaryWrapper), new Type[] { typeof(BuyOffer) });
            XmlSerializer sellSerializer = new XmlSerializer(typeof(GEDictionaryWrapper), new Type[] { typeof(SellOffer) });

            while (true)
            {
                try
                {
                    using (TextWriter writer = new StreamWriter(Misc.getServerPath() + @"\buyOffers.xml"))
                    {
                        buySerializer.Serialize(writer, new GEDictionaryWrapper(buyOffers));
                        writer.Close();
                    }
                    using (TextWriter writer = new StreamWriter(Misc.getServerPath() + @"\sellOffers.xml"))
                    {
                        sellSerializer.Serialize(writer, new GEDictionaryWrapper(sellOffers));
                        writer.Close();
                    }
                }
                catch (Exception e)
                {
                    Misc.WriteError(e.Message);
                }
                try
                {
                    Thread.Sleep(5000); //60 seconds.
                }
                catch (ThreadInterruptedException) { }
            }
        }

        public void addOffer(GEItem offer)
        {
            Dictionary<int, GEItem[]> offers = offer is BuyOffer ? buyOffers : sellOffers;
            lock (offers)
            {
                if (!offers.ContainsKey(offer.getDisplayItem()))
                    offers.Add(offer.getDisplayItem(), new GEItem[1]);

                foreach (KeyValuePair<int, GEItem[]> eachOffer in offers)
                {
                    if (eachOffer.Key == offer.getDisplayItem()) //item shown in GE.
                    {
                        bool space = false;
                        for (int i = 0; i < eachOffer.Value.Length; i++)
                        {
                            if (eachOffer.Value[i] == null)
                            {
                                space = true;
                                eachOffer.Value[i] = offer;
                                break;
                            }
                        }
                        if (!space)
                        {
                            GEItem[] items = eachOffer.Value;
                            GEItem[] newItems = new GEItem[items.Length + 1];
                            int ptr = 0;
                            for (int i = 0; i < items.Length; i++)
                            {
                                if (items != null)
                                {
                                    newItems[ptr++] = items[i];
                                }
                            }
                            newItems[ptr] = offer;
                            if (!offers.ContainsKey(offer.getDisplayItem()))
                                offers.Add(offer.getDisplayItem(), newItems);
                            else
                                offers[offer.getDisplayItem()] = newItems;
                        }
                        break;
                    }
                }
            }
        }

        public bool removeOffer(GEItem offer)
        {
            Dictionary<int, GEItem[]> offers = offer is BuyOffer ? buyOffers : sellOffers;
            lock (offers)
            {
                if (!offers.ContainsKey(offer.getDisplayItem()))
                {
                    Misc.WriteError("Invalid GE item removal = Name: " + offer.getPlayerName() + " Item: " + offer.getItem() + " Amount: " + offer.getTotalAmount() + " Price: " + offer.getPriceEach());
                    return false;
                }
                foreach (KeyValuePair<int, GEItem[]> eachOffer in offers)
                {
                    if (eachOffer.Key == offer.getDisplayItem())
                    {
                        for (int i = 0; i < eachOffer.Value.Length; i++)
                        {
                            if (eachOffer.Value[i] != null)
                            {
                                if (eachOffer.Value[i].Equals(offer))
                                {
                                    eachOffer.Value[i] = null;
                                    int entries = 0;
                                    for (int j = 0; j < eachOffer.Value.Length; j++)
                                    {
                                        if (eachOffer.Value[i] != null)
                                        {
                                            entries++;
                                        }
                                    }
                                    if (entries == 0)
                                    {
                                        offers.Remove(offer.getDisplayItem());
                                    }
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        public void interactWithClerk(Player p, Npc clerk)
        {
        }

        public GEItem getOfferForSlot(Player p, int slot)
        {
            foreach (KeyValuePair<int, GEItem[]> buyOffer in buyOffers)
            {
                if (buyOffer.Value != null)
                {
                    for (int i = 0; i < buyOffer.Value.Length; i++)
                    {
                        if (buyOffer.Value[i] != null)
                        {
                            if (buyOffer.Value[i].getPlayerName().Equals(p.getLoginDetails().getUsername()) && buyOffer.Value[i].getSlot() == slot)
                            {
                                return buyOffer.Value[i];
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<int, GEItem[]> sellOffer in sellOffers)
            {
                if (sellOffer.Value != null)
                {
                    for (int i = 0; i < sellOffer.Value.Length; i++)
                    {
                        if (sellOffer.Value[i] != null)
                        {
                            if (sellOffer.Value[i].getPlayerName().Equals(p.getLoginDetails().getUsername()) && sellOffer.Value[i].getSlot() == slot)
                            {
                                return sellOffer.Value[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public void clickDesk(Player p, int x, int y, int option)
        {
            AreaEvent clickDeskAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 1, y + 1);
            clickDeskAreaEvent.setAction(() =>
            {
                p.setFaceLocation(new Location(x, y, 0));
                switch (option)
                {
                    case 1:
                        break;

                    case 2:
                        p.getPackets().closeInterfaces();
                        p.setGESession(new GESession(p));
                        break;
                }
            });
            Server.registerCoordinateEvent(clickDeskAreaEvent);
        }

        public void showCollection(Player p)
        {
        }
    }
}