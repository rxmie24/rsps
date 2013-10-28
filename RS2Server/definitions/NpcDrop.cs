using RS2.Server.model;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class NpcDrop
    {
        public int[] npcs;
        public List<Item> commonDrops = null;
        public List<Item> uncommonDrops = null;
        public List<Item> rareDrops = null;
        public List<Item> alwaysDrops = null;

        public static void load()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\npcDrops.xml"))
            {
                Console.WriteLine(@"Missing data\npcDrops.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\npcDrops.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<NpcDrop>));
                List<NpcDrop> list = (List<NpcDrop>)serializer.Deserialize(objStreamReader);

                int p = 0;
                foreach (NpcDrop drop in list)
                {
                    for (int i = 0; i < drop.npcs.Length; i++)
                        NpcData.forId(drop.npcs[i]).setDrop(drop);
                    p++;
                }
                Console.WriteLine("Loaded " + p + " npc drops.");
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
        }

        public List<Item> getCommonDrops()
        {
            return commonDrops;
        }

        public List<Item> getUncommonDrops()
        {
            return uncommonDrops;
        }

        public List<Item> getRareDrops()
        {
            return rareDrops;
        }

        public List<Item> getAlwaysDrops()
        {
            return alwaysDrops;
        }
    }
}