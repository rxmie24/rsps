using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    internal class DoorControl
    {
        private List<Door> doors;
        private static int PLAYER_CHANGE_DELAY = 1200;
        private static int CHANGE_CYCLE_TIME = 180000;

        public DoorControl()
        {
            loadDoors();
            startCloseDoorEvent();
        }

        private void loadDoors()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\doors.xml"))
            {
                Misc.WriteError(@"Missing data\doors.xml");
                return;
            }

            //Deserialize text file to a new object.
            try
            {
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\doors.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<Door>));
                doors = (List<Door>)serializer.Deserialize(objStreamReader);
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }

            Console.WriteLine("Loaded " + doors.Count + " door configurations.");
        }

        private void startCloseDoorEvent()
        {
            Event closeDoorEvent = new Event(CHANGE_CYCLE_TIME);
            closeDoorEvent.setAction(() =>
            {
                foreach (Door door in doors)
                {
                    if (door != null)
                    {
                        if (door.isDoorOpen() && !door.isInstantClose() && Misc.random(1) == 0)
                        {
                            changeDoor(door);
                        }
                    }
                }
            });
            Server.registerEvent(closeDoorEvent);
        }

        public bool useDoor(Player p, int doorId, int doorX, int doorY, int doorHeight)
        {
            Location doorLocation = new Location(doorX, doorY, doorHeight);
            foreach (Door door in doors)
            {
                int id = door.isDoorOpen() ? door.getOpenDoorId() : door.getClosedDoorId();
                if (id == doorId)
                {
                    if (door.getDoorLocation().Equals(doorLocation))
                    {
                        if (door.isDoorOpen() && (Environment.TickCount - door.getLastChangeTime() <= PLAYER_CHANGE_DELAY))
                        {
                            // door was opened in the last PLAYER_CHANGE_DELAY ms..cant be instantly closed
                            return true;
                        }
                        else if (!door.isClosable() && door.isDoorOpen())
                        {
                            // door cannot be closed by a player
                            return true;
                        }
                        Door d = door;
                        AreaEvent useDoorAreaEvent = new AreaEvent(p, doorLocation.getX() - 1, doorLocation.getY() - 1, doorLocation.getX() + 1, doorLocation.getY() + 1);
                        useDoorAreaEvent.setAction(() =>
                        {
                            changeDoor(p, d);
                        });
                        Server.registerCoordinateEvent(useDoorAreaEvent);
                        return true;
                    }
                }
            }
            return false;
        }

        public void changeDoor(Door door)
        {
            updateDoorForPlayers(door);
        }

        public void changeDoor(Player p, Door door)
        {
            p.setFaceLocation(door.getDoorLocation());
            updateDoorForPlayers(door);
        }

        private void updateDoorForPlayers(Door door)
        {
            int id = door.isDoorOpen() ? door.getClosedDoorId() : door.getOpenDoorId();
            Location loc = door.isDoorOpen() ? door.getClosedDoorLocation() : door.getOpenDoorLocation();
            int direction = door.isDoorOpen() ? door.getClosedDirection() : door.getOpenDirection();
            Location loc1 = door.isDoorOpen() ? door.getOpenDoorLocation() : door.getClosedDoorLocation();
            int direction1 = door.isDoorOpen() ? door.getOpenDirection() : door.getClosedDirection();
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.getLocation().withinDistance(door.getDoorLocation(), 60))
                    {
                        p.getPackets().removeObject(loc1, direction1, 0);
                        p.getPackets().createObject(id, loc, direction, 0);
                    }
                }
            }
            door.setDoorOpen(!door.isDoorOpen());
            door.setLastChangeTime(Environment.TickCount);
        }

        public void refreshDoorsForPlayer(Player p)
        {
            foreach (Door door in doors)
            {
                if (door.getDoorLocation().withinDistance(p.getLocation(), 60))
                {
                    int id = door.isDoorOpen() ? door.getOpenDoorId() : door.getClosedDoorId();
                    Location loc = door.isDoorOpen() ? door.getOpenDoorLocation() : door.getClosedDoorLocation();
                    int direction = door.isDoorOpen() ? door.getOpenDirection() : door.getClosedDirection();
                    Location loc1 = door.isDoorOpen() ? door.getClosedDoorLocation() : door.getOpenDoorLocation();
                    int direction1 = door.isDoorOpen() ? door.getClosedDirection() : door.getOpenDirection();
                    p.getPackets().removeObject(loc1, direction1, 0);
                    p.getPackets().createObject(id, loc, direction, 0);
                }
            }
        }
    }
}