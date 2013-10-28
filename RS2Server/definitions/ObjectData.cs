using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class ObjectData
    {
        public ushort id;
        public string name, examine;

        public ObjectData()
        {
        } //xml de/serialization.

        /**
         * Each list contains the region and 4 pieces of data.
         */
        private static Dictionary<int, ObjectData> definitions;

        public static void load()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\objectData.xml"))
            {
                Misc.WriteError(@"Missing data\objectData.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\objectData.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<ObjectData>));
                List<ObjectData> defs = (List<ObjectData>)serializer.Deserialize(objStreamReader);

                definitions = new Dictionary<int, ObjectData>();
                foreach (ObjectData def in defs)
                {
                    definitions.Add(def.getId(), def);
                }
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
            Console.WriteLine("Loaded " + definitions.Count + " object definitions.");
        }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public string getExamine()
        {
            return examine;
        }

        public static ObjectData forId(int id)
        {
            ObjectData d;
            if (!definitions.TryGetValue(id, out d))
            {
                d = new ObjectData();
                d.id = (ushort)id;
                d.name = "Object #" + d.id;
                d.examine = "This object is missing examine information..";
                definitions.Add(id, d);
            }
            return d;
        }
    }
}