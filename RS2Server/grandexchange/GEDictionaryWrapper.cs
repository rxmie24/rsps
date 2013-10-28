using System.Collections.Generic;

namespace RS2.Server.grandexchange
{
    public class GEDictionaryWrapper
    {
        public List<int> Keys { get; set; }

        public List<GEItem[]> Values { get; set; }

        public GEDictionaryWrapper()
        {
        }

        public GEDictionaryWrapper(Dictionary<int, GEItem[]> map)
        {
            Keys = new List<int>(map.Keys);
            Values = new List<GEItem[]>(map.Values);
        }

        public Dictionary<int, GEItem[]> GetMap()
        {
            Dictionary<int, GEItem[]> map = new Dictionary<int, GEItem[]>();
            for (int i = 0; i < Keys.Count; i++)
            {
                map[Keys[i]] = Values[i];
            }
            return map;
        }
    }
}