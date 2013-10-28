using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class NpcSpawn
    {
        public int id;
        public Location location;
        public Location minimumCoords;
        public Location maximumCoords;
        public WalkType walkType;
        public FaceDirection faceDirection = FaceDirection.NORTH;

        public static void load()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\npcs.xml"))
            {
                Misc.WriteError(@"Missing data\npcs.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\npcs.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<NpcSpawn>));
                List<NpcSpawn> spawns = (List<NpcSpawn>)serializer.Deserialize(objStreamReader);

                foreach (NpcSpawn ns in spawns)
                {
                    Npc n = new Npc(ns.id, ns.location);
                    n.setMinimumCoords(ns.minimumCoords);
                    n.setMaximumCoords(ns.maximumCoords);
                    n.setWalkType(ns.walkType);
                    n.setFaceDirection(ns.faceDirection);
                    Server.getNpcList().Add(n);
                }
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
            Console.WriteLine("Spawned " + Server.getNpcList().Count + " npcs.");
        }
    }
}