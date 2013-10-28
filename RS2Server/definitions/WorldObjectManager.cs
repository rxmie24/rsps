using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills.farming;
using RS2.Server.player.skills.runecrafting;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;

namespace RS2.Server.definitions
{
    internal class WorldObjectManager
    {
        private List<WorldObject> objects;
        private static int FIRE = 1;
        private FarmingPatches farmingPatches;
        private DoorControl doors;
        public byte[, ,] regionData = new byte[4, 104, 104];

        public WorldObjectManager()
        {
            objects = new List<WorldObject>();
            farmingPatches = new FarmingPatches();
            doors = new DoorControl();
            loadObjects(); //offical objects
            loadSpawnedObjects(); //custom spawned objects
            loadDeletedObjects(); //deleted objects.
        }

        public void lowerHealth(ushort originalId, Location location)
        {
            WorldObject worldObject = getObject(originalId, location);
            if (worldObject == null)
            {
                addObjectToList(originalId, location);
            }
            if (worldObject.getOriginalId() == 733)
            { // Web
                if (Misc.random(6) == 0)
                {
                    changeObject(worldObject);
                }
                return;
            }
            worldObject = getObject(originalId, location);
            worldObject.setObjectHealth(worldObject.getObjectHealth() - 1);
            if (worldObject.getObjectHealth() <= 0)
            {
                changeObject(worldObject);
            }
        }

        private void changeObject(WorldObject worldObject)
        {
            if (worldObject != null)
            {
                worldObject.setSecondForm(true);
                foreach (Player p in Server.getPlayerList())
                {
                    if (p != null)
                    {
                        if (p.getLocation().withinDistance(worldObject.getLocation(), 60))
                        {
                            if (!worldObject.isFire())
                            {
                                p.getPackets().removeObject(worldObject.getLocation(), worldObject.getFace(), worldObject.getType());
                                p.getPackets().createObject(worldObject.getSecondaryId(), worldObject.getLocation(), worldObject.getFace(), worldObject.getType());
                            }
                            else
                            {
                                p.getPackets().createObject(worldObject.getOriginalId(), worldObject.getLocation(), worldObject.getFace(), worldObject.getType());
                            }
                        }
                    }
                }
                int delay = worldObject.isFire() ? (60000 + Misc.random(90000)) : worldObject.getRestoreDelay();
                Event restoreObjectEvent = new Event(delay);
                restoreObjectEvent.setAction(() =>
                {
                    restoreObject(worldObject);
                    restoreObjectEvent.stop();
                });
                Server.registerEvent(restoreObjectEvent);
            }
        }

        public bool fireExists(Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.isFire())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public WorldObject getObjectByOriginalId(ushort id, Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.getOriginalId() == id)
                    {
                        return o;
                    }
                }
            }
            return null;
        }

        public WorldObject getObjectbySecondaryId(ushort id, Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.getSecondaryId() == id)
                    {
                        return o;
                    }
                }
            }
            return null;
        }

        public void restoreObject(WorldObject worldObject)
        {
            if (worldObject != null)
            {
                foreach (Player p in Server.getPlayerList())
                {
                    if (p != null)
                    {
                        if (p.getLocation().withinDistance(worldObject.getLocation(), 60))
                        {
                            if (!worldObject.isFire())
                            {
                                p.getPackets().createObject(worldObject.getOriginalId(), worldObject.getLocation(), worldObject.getFace(), worldObject.getType());
                            }
                            else
                            {
                                p.getPackets().removeObject(worldObject.getLocation(), worldObject.getFace(), worldObject.getType());
                            }
                        }
                    }
                }
                if (worldObject.isFire())
                {
                    GroundItem item = new GroundItem(592, 1, worldObject.getLocation(), worldObject.getOwner());
                    Server.getGroundItems().newWorldItem(item);
                }
            }
            lock (objects)
            {
                objects.Remove(worldObject);
            }
        }

        private void addObjectToList(ushort originalId, Location location)
        {
            WorldObject worldObject = new WorldObject(originalId, getSecondaryId(originalId), getFace(originalId, location), location, getRestore(originalId), getHealth(originalId));
            worldObject.setType(getType(worldObject));
            lock (objects)
            {
                objects.Add(worldObject);
            }
        }

        public void add(WorldObject worldObject)
        {
            lock (objects)
            {
                objects.Add(worldObject);
            }
        }

        public void remove(WorldObject worldObject)
        {
            lock (objects)
            {
                objects.Remove(worldObject);
            }
        }

        public void refreshGlobalObjects(Player p)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null && (o.isFire() || o.getOwner() == null || o.getOwner() == p) || o.isSpawnedObject() || o.shouldDeleteObject())
                {
                    if (o.getLocation().withinDistance(p.getLocation(), 60))
                    {
                        if (o.shouldDeleteObject())
                        {
                            p.getPackets().removeObject(o.getLocation(), o.getFace(), o.getType());
                        }
                        else if (!o.isFire())
                        {
                            p.getPackets().createObject(o.isSecondForm() ? o.getSecondaryId() : o.getOriginalId(), o.getLocation(), o.getFace(), o.getType());
                        }
                        else
                        {
                            p.getPackets().createObject(o.getOriginalId(), o.getLocation(), o.getFace(), o.getType());
                        }
                    }
                }
            }
            doors.refreshDoorsForPlayer(p);
            if (RuneCraft.wearingTiara(p))
            {
                RuneCraft.toggleRuin(p, p.getEquipment().getItemInSlot(0), true);
            }
        }

        public bool tileAvailable(Player p)
        {
            Location l = p.getLocation();
            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
            int x = l.getX() - ((regionX - 6) * 8);
            int y = l.getY() - ((regionY - 6) * 8);
            return (regionData[l.getZ(), x, y]) == 0;
        }

        public void newFire(Player p, ushort fireId, Location location)
        {
            WorldObject worldObject = new WorldObject(fireId, location, FIRE);
            lock (objects)
            {
                objects.Add(worldObject);
            }
            worldObject.setOwner(p);
            changeObject(worldObject);
        }

        public bool originalObjectExists(ushort originalId, Location location)
        {
            WorldObject worldObject = getObject(originalId, location);
            if (worldObject == null)
            {
                addObjectToList(originalId, location);
                return true;
            }
            return worldObject != null && !worldObject.isSecondForm() && worldObject.getObjectHealth() > 0;
        }

        public bool objectExists(ushort originalId, Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.getOriginalId() == originalId)
                        return true;
                }
            }
            return true;
        }

        public WorldObject getObject(ushort originalId, Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.getOriginalId() == originalId)
                    {
                        return o;
                    }
                }
            }
            return null;
        }

        public int getFace(ushort originalId, Location location)
        {
            foreach (WorldObject o in objects)
            {
                if (o != null)
                {
                    if (o.getLocation().Equals(location) && o.getOriginalId() == originalId)
                    {
                        return o.getFace();
                    }
                }
            }
            return 0;
        }

        private int getHealth(ushort originalId)
        {
            int random = 1;
            switch (originalId)
            {
                case 733: // Wilderness web
                    return Misc.random(10);
                /*
                 * Oak trees
                 */
                case 1281: // Normal Oak tree
                case 3037: // Oak tree dark stump
                    return 4 + Misc.random(18);
                /*
                 * Willow trees
                 */
                case 1308: // Normal Willow tree
                case 5551: // Normal Willow tree
                case 5552: // Normal Willow tree
                case 5553: // Normal Willow tree
                    return 12 + Misc.random(20);
                /*
                 * Teak trees
                 */
                case 9036: // Normal Teak tree
                case 15062: // Normal Teak tree (same as above?)
                    return 20 + Misc.random(15);
                /*
                 * Maple trees
                 */
                case 1307: // Normal Maple tree
                case 4674:// Exactly same as above
                    return 10 + Misc.random(7);
                /*
                 * Hollow trees
                 */
                case 2289: // Normal Hollow tree
                case 4060: // Normal Hollow tree (bigger than above)
                    return 5 + Misc.random(9);

                case 9034: // Normal Mahogany tree
                    return 25 + Misc.random(13);
                /*
                 * Eucalyptus trees
                 */
                case 28951: // Normal Eucalyptus tree
                case 28952: // Normal Eucalyptus tree (smaller)
                case 28953: // Normal Eucalyptus tree (smallest)
                    return 7 + Misc.random(10);

                case 1309: // Yew tree
                    return 17 + Misc.random(20);

                case 1306: // Normal Magic tree
                    return 20 + Misc.random(15);
                /*
                 * Coal rocks
                 */
                case 11930:
                case 11931:
                case 11932:
                case 14850:
                case 14851:
                case 14852:
                case 31068:
                case 31069:
                case 31070:
                    random = Misc.random(10);
                    if (random == 0)
                    {
                        random = 1;
                    }
                    return random;
            }
            return 1;
        }

        private int getRestore(int originalId)
        {
            switch (originalId)
            {
                case 733: // Wilderness web
                    return 90000;

                case 34384: // Bakers stall.
                case 635: // Tea stall.
                case 4875: // AA food stall.
                case 4708: // Vegatable stall.
                case 4706: // Vegatable stall.
                case 4876: // AA general.
                case 4874: // AA crafting.
                    return 3500;

                case 4705: // Fish stall.
                case 4707: // Fish stall.
                case 34383: // Silk stall.
                case 14011: // Wine stall.
                case 34382: // Silver stall
                case 34387: // Fur stall.
                case 7053: // Seed stall.
                    return 5000;

                case 34386: // Spice stall
                    return 6000;

                case 4877: // AA magic stall.
                    return 8000;

                case 4878: // AA scimitar stall.
                    return 10000;

                case 34385: // Gem stall.
                    return 8500;

                case 2566: //Coin chest.
                case 2567: //Nature rune chest.
                case 2568: //Coin chest #2.
                case 2569: //Blood rune chest.
                case 2570: //King Lathas chest
                    return 180000;
                /*
                * Normal & dead trees
                */
                case 1276: // Normal tree
                case 1278: // Normal tree
                case 2409: // Normal tree
                case 1277: // Normal tree with but different coloured stump
                case 3034: // Normal tree with dark stump
                case 3033: // Normal tree with dark stump
                case 10041: // Normal tree
                case 1282: // Dead tree
                case 1283: // Dead tree
                case 1284: // Dead tree
                case 1285: // Dead tree
                case 1286: // Dead tree
                case 1289: // Dead tree
                case 1290: // Dead tree
                case 1365: // Dead tree
                case 1383: // Dead tree
                case 1384: // Dead tree
                case 1291: // Dead tree
                case 3035: // Dead tree
                case 3036: // Dead tree
                case 1315: // Evergreen
                case 1316: // Evergreen
                case 1318: // Snowy Evergreen
                case 1319: // Snowy Evergreen
                case 1330: // Snow covered tree
                case 1331: // Snow covered tree
                case 1332: // Snow covered tree
                case 3879: // Evergreen from elf land
                case 3881: // Evergreen from elf land (slightly bigger than one above)
                case 3882: // Evergreen from elf land (slightly bigger than one above)
                case 3883: // Small Evergreen from elf land
                case 1280: // Normal tree orange stump
                    return 25000;
                /*
                 * Oak trees
                 */
                case 1281: // Normal Oak tree
                case 3037: // Oak tree dark stump
                    return 30000;
                /*
                 * Willow trees
                 */
                case 1308: // Normal Willow tree
                case 5551: // Normal Willow tree
                case 5552: // Normal Willow tree
                case 5553: // Normal Willow tree
                    return 60000;

                case 2023: // Achey tree
                    return 60000;
                /*
                 * Teak trees
                 */
                case 9036: // Normal Teak tree
                case 15062: // Normal Teak tree (same as above?)
                    return 70000;
                /*
                 * Maple trees
                 */
                case 1307: // Normal Maple tree
                case 4674:// Exactly same as above
                    return 60000;
                /*
                 * Hollow trees
                 */
                case 2289: // Normal Hollow tree
                case 4060: // Normal Hollow tree (bigger than above)
                    return 50000;

                case 9034: // Normal Mahogany tree
                    return 60000;

                case 21273: // Normal Arctic pine
                    return 50000;

                /*
                 * Eucalyptus trees
                 */
                case 28951: // Normal Eucalyptus tree
                case 28952: // Normal Eucalyptus tree (smaller)
                case 28953: // Normal Eucalyptus tree (smallest)
                    return 90000;

                case 1309: // Yew tree
                    return 60000;

                case 1306: // Normal Magic tree
                    return 90000;
                /*
                 * Clay rocks
                 */
                case 15504:
                case 15503:
                case 15505:
                case 11189:
                case 11190:
                case 11191:
                case 31062:
                case 31063:
                case 31064:
                case 32429:
                case 32430:
                case 32431:
                    return 2000;
                /*
                 * Tin rocks
                 */
                case 11959:
                case 11958:
                case 11957:
                case 11933:
                case 11934:
                case 11935:
                case 31077:
                case 31078:
                case 31079:
                /*
                 * Copper rocks
                 */
                case 11960:
                case 11961:
                case 11962:
                case 11936:
                case 11937:
                case 11938:
                case 31080:
                case 31081:
                case 31082:
                    return 3000;
                /*
                 * Iron rocks
                 */
                case 11954:
                case 11955:
                case 11956:
                case 14856:
                case 14857:
                case 14858:
                case 31071:
                case 31072:
                case 31073:
                case 32441:
                case 32442:
                case 32443:
                    return 3500;
                /*
                 * Silver rocks
                 */
                case 11948:
                case 11949:
                case 11950:
                case 11165:
                case 11186:
                case 11187:
                case 11188:
                case 31074:
                case 31075:
                case 31076:
                case 32444:
                case 32445:
                case 32446:
                case 15579:
                case 15580:
                case 15581:
                    return 18000;
                /*
                 * Gold rocks
                 */
                case 11951:
                case 11952:
                case 11953:
                case 11183:
                case 11184:
                case 11185:
                case 31065:
                case 31066:
                case 31067:
                case 32432:
                case 32433:
                case 32434:
                case 15576:
                case 15577:
                case 15578:
                    return 25000;
                /*
                 * Coal rocks
                 */
                case 11930:
                case 11931:
                case 11932:
                case 14850:
                case 14851:
                case 14852:
                case 31068:
                case 31069:
                case 31070:
                case 32426:
                case 32427:
                case 32428:
                    return 25000;
                /*
                 * Mithril rocks
                 */
                case 11945:
                case 11946:
                case 11947:
                case 11942:
                case 11943:
                case 11944:
                case 14853:
                case 14854:
                case 14855:
                case 31086:
                case 31087:
                case 31088:
                case 32438:
                case 32439:
                case 32440:
                    return 30000;
                /*
                 * Adamant rocks
                 */
                case 11963:
                case 11964:
                case 11965:
                case 11939:
                case 11940:
                case 11941:
                case 14862:
                case 14863:
                case 14864:
                case 31083:
                case 31084:
                case 31085:
                case 32435:
                case 32436:
                case 32437:
                    return 45000;
                /*
                 * Rune rocks
                 */
                case 14859:
                case 14860:
                case 14861:
                    return 120000;
            }
            return 10000;
        }

        private int getSecondaryId(int originalId)
        {
            ushort emptyRock = 0;
            /*
             * Dark brown rocks
             */
            for (int i = 11945; i <= 11966; i++)
            {
                if (emptyRock == 11558 || emptyRock == 0)
                {
                    emptyRock = 11555;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            /*
             * Very dark brown rocks
             */
            for (int i = 32426; i <= 32446; i++)
            {
                if (emptyRock == 33403 || emptyRock == 0)
                {
                    emptyRock = 33400;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            /*
             * Sand-coloured rocks
             */
            emptyRock = 0;
            for (int i = 11183; i <= 11191; i++)
            {
                if (emptyRock == 11555 || emptyRock == 0)
                {
                    emptyRock = 11552;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            /*
             * Beige coloured rocks
             */
            emptyRock = 0;
            for (int i = 11930; i <= 11944; i++)
            {
                if (emptyRock == 11555 || emptyRock == 0)
                {
                    emptyRock = 11552;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            /*
             * Very light coloured sandy rocks
             */
            emptyRock = 0;
            for (int i = 31062; i <= 31088; i++)
            {
                if (emptyRock == 31062 || emptyRock == 0)
                {
                    emptyRock = 31059;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            /*
             * Black rocks
             */
            emptyRock = 0;
            for (int i = 14850; i <= 14864; i++)
            {
                if (emptyRock == 14835 || emptyRock == 0)
                {
                    emptyRock = 14832;
                }
                if (originalId == i)
                {
                    return emptyRock;
                }
                emptyRock++;
            }
            switch (originalId)
            {
                case 733: // Wilderness web
                    return 734;

                case 635: // Varrock tea stall.
                case 4708: // Vegatable stall.
                case 4706: // Vegatabale stall.
                case 4705: // Fish stall.
                case 4707: // Fish stall.
                    return 634;

                case 14011: // Draynor wine stall.
                case 7053: // Seed stall.
                    return 14012;

                case 34384: // Bakers stall.
                case 34383: // Silk stall.
                case 34387: // Fur stall.
                case 34382: // Silver stall.
                case 34386: // Spice stall.
                case 34385: // Gem stall.
                    return 34381; // Ardougne blank stalls

                case 4876: // AA general store.
                case 4874: // AA food store.
                case 4875: // AA crafting store.
                case 4877: // AA magic store.
                case 4878: // AA scimitar store.
                    return 4797; // AA blank desk

                case 2566: //Coin chest.
                case 2567: //Nature rune chest.
                case 2568: //Coin chest #2.
                case 2569: //Blood rune chest.
                case 2570: //King Lathas chest
                    return 2604;

                case 1276: // Normal tree
                case 1278: // Normal tree
                case 2409:
                case 10041:
                case 1315: // Evergreen
                case 1316: // Evergreen
                    return 1342;

                case 3879: // Evergreen from elf land
                case 3881: // Evergreen from elf land (slightly bigger than one above)
                case 3882: // Evergreen from elf land (slightly bigger than one above)
                    return 3880;

                case 3883: // Small evergreen from elf land
                    return 3884;

                case 1281: // Normal Oak tree
                    return 1356;

                case 1318: // Snowy Evergreen
                case 1319: // Snowy Evergreen
                case 1330: // Snow covered tree
                case 1331: // Snow covered tree
                case 1332: // Snow covered tree
                case 3037: // Oak tree dark stump
                    return 1357;

                case 1383: // Dark stump dead tree
                case 1384: // grey dead tree
                case 3033:
                case 3034:
                    return 1341;

                case 1291: // Snowy dead tree
                    return 23054;

                case 2023: // achey tree
                    return 3371;

                case 24168: // Dying tree
                    return 24169;

                case 5551: // Normal Willow tree
                case 5552: // Normal Willow tree
                case 5553: // Normal Willow tree
                    return 5554;

                case 3035: // Big dead tree
                case 3036:
                    return 1347;

                case 1307: // Normal Maple tree
                case 4674: // Exactly same as above
                    return 1344;

                case 1277:
                case 1280:
                    return 1343;

                case 16604: // Dream tree
                    return 16605;

                case 21273: // Arctic pine
                    return 21274;

                case 2289: // Normal Hollow tree
                    return 2310;

                case 4060: // Normal Hollow tree (bigger than above)
                    return 4061;

                case 1309: // Yew tree
                    return 1355;

                case 1286:
                case 1289:
                    return 1351;

                case 28951: // Normal Eucalyptus tree
                    return 28954;

                case 1306: // magic tree
                    return 7401;

                case 1290: // Big white dead tree
                    return 1354;

                case 1365: // green dead tree
                    return 1352;

                case 28952: // Normal Eucalyptus tree (smaller)
                    return 28955;

                case 28953: // Normal Eucalyptus tree (smallest)
                    return 28956;

                case 1285:
                    return 1349;

                case 9034: // Mahogany
                    return 9035;

                case 9036: // teak
                case 15062:
                    return 9037;

                //3371 - achey
                case 1282:
                case 1283:
                case 1284:
                    return 12733;
                /*
                 * Clay i think idk cant remember lulz
                 */
                case 15503:
                    return 11555;

                case 15504:
                    return 11556;

                case 15505:
                    return 11557;
                /*
                 * Tzhaar rocks
                 */
                case 15576:
                case 15579:
                    return 15582;

                case 15577:
                case 15580:
                    return 15583;

                case 15578:
                case 15581:
                    return 15584;
            }
            return -1;
        }

        private int getType(WorldObject worldObject)
        {
            switch (worldObject.getOriginalId())
            {
                case 733: // Wilderness web
                    if (worldObject.getOriginalId() == 733)
                    {
                        Location web1Location = new Location(3092, 3957, 0); // Eastern mage bank web
                        Location web2Location = new Location(3095, 3957, 0); // Western mage bank web
                        if (worldObject.getLocation().Equals(web1Location) || worldObject.getLocation().Equals(web2Location))
                        {
                            return 0;
                        }
                    }
                    break;
            }
            return 10;
        }

        public DoorControl getDoors()
        {
            return doors;
        }

        public FarmingPatches getFarmingPatches()
        {
            return farmingPatches;
        }

        private void loadObjects()
        {
            string line = "";
            string token = "";
            string token2 = "";
            string token2_2 = "";
            string[] token3 = new string[10];

            if (!File.Exists(Misc.getServerPath() + @"\data\objectLocations.cfg"))
            {
                Misc.WriteError(@"Missing data\objectLocations.cfg");
                return;
            }
            try
            {
                StreamReader sr = File.OpenText(Misc.getServerPath() + @"\data\objectLocations.cfg");

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    int spot = line.IndexOf("=");

                    if (spot > -1)
                    {
                        token = line.Substring(0, spot);
                        token = token.Trim();
                        token2 = line.Substring(spot + 1);
                        token2 = token2.Trim();
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token3 = token2_2.Split('\t');
                        if (token.Equals("o"))
                        {
                            int x = int.Parse(token3[0]);
                            int y = int.Parse(token3[1]);
                            ushort id = ushort.Parse(token3[2]);
                            int face = int.Parse(token3[3]);
                            int type = 10;
                            WorldObject worldObject = new WorldObject(id, new Location(x, y, 0), face, type);
                            objects.Add(worldObject);
                        }
                    }
                    else
                    {
                        if (line.Equals("[ENDOFLIST]"))
                        {
                            Console.WriteLine("Loaded " + objects.Count + " object definitions.");
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading objectLocations.cfg msg=" + e.Message);
                Console.ReadLine();
            }
        }

        public void loadSpawnedObjects()
        {
            string line = "";
            string token = "";
            string token2 = "";
            string token2_2 = "";
            string[] token3 = new string[10];

            if (!File.Exists(Misc.getServerPath() + @"\data\spawnedObjects.cfg"))
            {
                Misc.WriteError(@"Missing data\spawnedObjects.cfg");
                return;
            }
            try
            {
                StreamReader sr = File.OpenText(Misc.getServerPath() + @"\data\spawnedObjects.cfg");
                int amount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    int spot = line.IndexOf("=");

                    if (spot > -1)
                    {
                        token = line.Substring(0, spot);
                        token = token.Trim();
                        token2 = line.Substring(spot + 1);
                        token2 = token2.Trim();
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token3 = token2_2.Split('\t');
                        if (token.Equals("object"))
                        {
                            amount++;
                            ushort id = ushort.Parse(token3[0]);
                            Location location = new Location(int.Parse(token3[1]), int.Parse(token3[2]), int.Parse(token3[3]));
                            int face = int.Parse(token3[4]);
                            int type = int.Parse(token3[5]);
                            WorldObject worldObject = new WorldObject(id, location, face, type, true);

                            // these 3 methods are to set variables for spawned rocks, trees etc.
                            worldObject.setRestore(getRestore(id));
                            worldObject.setObjectHealth(getHealth(id));
                            worldObject.setSecondaryId(getSecondaryId(id));

                            objects.Add(worldObject);
                            add(worldObject); // we KNOW this object is legit..since we spawned it.
                        }
                    }
                    else
                    {
                        if (line.Equals("[ENDOFLIST]"))
                        {
                            Console.WriteLine("Loaded " + amount + " custom objects.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading spawnedObjects.cfg msg=" + e.Message);
                Console.ReadLine();
            }
        }

        public void loadDeletedObjects()
        {
            string line = "";
            string token = "";
            string token2 = "";
            string token2_2 = "";
            string[] token3 = new string[10];

            if (!File.Exists(Misc.getServerPath() + @"\data\deletedObjects.cfg"))
            {
                Misc.WriteError(@"Missing data\deletedObjects.cfg");
                return;
            }
            try
            {
                StreamReader sr = File.OpenText(Misc.getServerPath() + @"\data\deletedObjects.cfg");
                int amount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    int spot = line.IndexOf("=");

                    if (spot > -1)
                    {
                        token = line.Substring(0, spot);
                        token = token.Trim();
                        token2 = line.Substring(spot + 1);
                        token2 = token2.Trim();
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token2_2 = token2.Replace("\t\t", "\t");
                        token3 = token2_2.Split('\t');
                        if (token.Equals("object"))
                        {
                            amount++;
                            Location loc = new Location(int.Parse(token3[0]), int.Parse(token3[1]), int.Parse(token3[2]));
                            int face = int.Parse(token3[3]);
                            int type = int.Parse(token3[4]);
                            WorldObject deletedObject = new WorldObject(loc, face, type, true);
                            objects.Add(deletedObject);
                        }
                    }
                    else
                    {
                        if (line.Equals("[ENDOFLIST]"))
                        {
                            Console.WriteLine("Loaded " + amount + " deleted objects.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading deletedObjects.cfg msg=" + e.Message);
                Console.ReadLine();
            }
        }
    }
}