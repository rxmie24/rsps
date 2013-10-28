using RS2.Server.util;
using System;
using System.IO;

namespace RS2.Server.definitions
{
    internal class MapData
    {
        public class MapList
        {
            /**
             * The region the mapdata is for.
             */
            public int region = 0;

            /**
             * The four pieces of mapdata.
             */
            public int[] data = new int[4];
        }

        /**
         * Each list contains the region and 4 pieces of data.
         */
        public static MapList[] mapLists = new MapList[1246];

        /**
         * Load the map data into memory for faster load time.
         */

        public MapData()
        {
            Console.WriteLine("Loading mapdata definitions. This is slow too much I/O maybe pack in XML? :P");
            if (!Directory.Exists(Misc.getServerPath() + @"\data\mapdata\"))
            {
                Misc.WriteError(@"mapdata directory not found, data\mapdata directory doesn't exist.");
                return;
            }

            int curId = 0;
            for (int i = 0; i < 16000; i++)
            {
                if (!File.Exists(Misc.getServerPath() + @"\data\mapdata\" + i + ".txt"))
                    continue;

                MapList list = mapLists[curId++] = new MapList();
                try
                {
                    using (StreamReader sr = new StreamReader(Misc.getServerPath() + @"\data\mapdata\" + i + ".txt"))
                    {
                        string line;
                        // Read and display lines from the file until the end of
                        // the file is reached.
                        int regionId = 0;
                        list.region = i;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!line.Equals(""))
                                list.data[regionId++] = int.Parse(line.Trim());
                        }
                    }
                }
                catch (Exception)
                {
                    // Let the user know what went wrong.
                    Misc.WriteError("[MapData]: map region " + i + " was not found!");
                }
            }
        }

        /**
         * Returns the four pieces of map data from a region.
         * @param myRegion The region to get data from.
         * @return Returns the four mapdata.
         */

        public static int[] getData(int myRegion)
        {
            foreach (MapList list in mapLists)
            {
                if (list == null)
                    continue;
                if (list.region == myRegion)
                    return list.data;
            }
            Misc.WriteError("Missing map data: " + myRegion);
            return new int[4];
        }
    }
}