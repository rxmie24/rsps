using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class NpcData
    {
        public static int MELEE = 0;
        public static int RANGE = 1;
        public static int MAGIC = 2;

        private static Dictionary<int, NpcData> definitions;

        public static void load()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\npcData.xml"))
            {
                Misc.WriteError(@"Missing data\npcData.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\npcData.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<NpcData>));
                List<NpcData> defs = (List<NpcData>)serializer.Deserialize(objStreamReader);

                definitions = new Dictionary<int, NpcData>();
                foreach (NpcData def in defs)
                {
                    definitions.Add(def.getId(), def);
                }
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
            Console.WriteLine("Loaded " + definitions.Count + " npc definitions.");
        }

        public static NpcData forId(int id)
        {
            NpcData d;
            if (!definitions.TryGetValue(id, out d))
            {
                d = new NpcData();
                d.id = id;
                d.name = "NPC #" + d.id;
                d.examine = "It's an NPC.";
                definitions.Add(id, d);
            }
            return d;
        }

        public static int getTotalNpcDefinitions()
        {
            return definitions.Count;
        }

        public int id;
        public string name, examine;
        private int size = 1;
        public int respawn = 20, combat = 0, hitpoints = 1, maxHit = 0, attackSpeed = 8, attackAnim = 422, defenceAnim = 404, deathAnim = 7197;
        public bool aggressive = false, superAggressive = false, bossMonster = false;
        private int attackType = MELEE;
        private NpcDrop drop = null;

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

        public int getRespawn()
        {
            return respawn;
        }

        public int getCombat()
        {
            return combat;
        }

        public int getHitpoints()
        {
            return hitpoints;
        }

        public int getMaxHit()
        {
            return maxHit;
        }

        public int getSize()
        {
            return size;
        }

        public int getAttackSpeed()
        {
            return attackSpeed;
        }

        public int getAttackAnimation()
        {
            return attackAnim;
        }

        public int getDefenceAnimation()
        {
            return defenceAnim;
        }

        public int getDeathAnimation()
        {
            return deathAnim;
        }

        public bool isAggressive()
        {
            return aggressive;
        }

        public int getAttackType()
        {
            return attackType;
        }

        public void setSuperAggressive(bool superAggressive)
        {
            this.superAggressive = superAggressive;
        }

        public bool isSuperAggressive()
        {
            return superAggressive;
        }

        public bool isBoss()
        {
            return bossMonster;
        }

        public NpcDrop getDrop()
        {
            return drop;
        }

        public void setDrop(NpcDrop d)
        {
            this.drop = d;
        }
    }
}