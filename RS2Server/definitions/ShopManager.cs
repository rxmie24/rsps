using RS2.Server.events;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    internal class ShopManager
    {
        private Dictionary<int, Shop> shops;

        public ShopManager()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\shops.xml"))
            {
                Misc.WriteError(@"Missing data\shops.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\shops.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<Shop>));

                List<Shop> listShops = (List<Shop>)serializer.Deserialize(objStreamReader);
                shops = new Dictionary<int, Shop>();
                foreach (Shop shop in listShops)
                {
                    shops.Add(shop.id, shop);
                }
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }

            //shops = (Map<Integer, Shop>) XStreamUtil.getXStream().fromXML(new FileInputStream("data/shops.xml"));
            Event updateShopAmountsEvent = new Event(60000);
            updateShopAmountsEvent.setAction(() =>
            {
                updateShopAmounts();
            });
            Server.registerEvent(updateShopAmountsEvent);
            Console.WriteLine("Loaded " + shops.Count + " shops.");
        }

        private void updateShopAmounts()
        {
            foreach (KeyValuePair<int, Shop> s in shops)
            {
                s.Value.updateAmounts();
                foreach (Player p in Server.getPlayerList())
                {
                    if (p == null || p.getShopSession() == null)
                    {
                        continue;
                    }
                    if (p.getShopSession().getShopId() == s.Key)
                    {
                        p.getPackets().sendItems(-1, 64271, 31, s.Value.getStock());
                    }
                }
            }
        }

        public Shop getShop(int id)
        {
            return shops[id];
        }
    }
}