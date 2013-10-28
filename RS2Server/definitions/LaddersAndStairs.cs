using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.magic;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RS2.Server.definitions
{
    public class LoadedLaddersAndStairs
    {
        public enum Event_Type
        {
            COORDINATE_POSITION,
            AREA_POSITION
        };

        public class HeightObject
        {
            public int objectId = -1;
            public int option = 1;
            public Location objectLocation = null;
            public Location teleportLocation = null;
            public Location standLocation = null;
            public Location minCoords = null;
            public Location maxCoords = null;
            public int animation = -1;
            public LoadedLaddersAndStairs.Event_Type type = Event_Type.COORDINATE_POSITION;
            private int teleDelay = 1000;

            public HeightObject()
            {
            }

            public HeightObject(int objectId, Location objectLocation, Location standLocation, Location teleportLocation, int animation)
            {
                this.objectId = objectId;
                this.objectLocation = objectLocation;
                this.teleportLocation = teleportLocation;
                this.animation = animation;
                this.standLocation = standLocation;
                this.type = Event_Type.COORDINATE_POSITION;
            }

            public HeightObject(int objectId, Location objectLocation, Location teleportLocation, Location minCoords, Location maxCoords, int animation)
            {
                this.objectId = objectId;
                this.objectLocation = objectLocation;
                this.teleportLocation = teleportLocation;
                this.animation = animation;
                this.minCoords = minCoords;
                this.maxCoords = maxCoords;
                this.type = Event_Type.AREA_POSITION;
            }

            public HeightObject(int objectId, Location loc, Location teleLoc, Location minCoords, Location maxCoords)
            {
                this.objectId = objectId;
                this.objectLocation = loc;
                this.teleportLocation = teleLoc;
                this.minCoords = minCoords;
                this.maxCoords = maxCoords;
                this.type = Event_Type.AREA_POSITION;
            }

            public int getId()
            {
                return objectId;
            }

            public Location getLocation()
            {
                return objectLocation;
            }

            public Location getTeleLocation()
            {
                return teleportLocation;
            }

            public int getTeleDelay()
            {
                if (animation == 828)
                {
                    return 1000;
                }
                return teleDelay;
            }

            public int getAnimation()
            {
                return animation;
            }

            public LoadedLaddersAndStairs.Event_Type getType()
            {
                return type;
            }

            public Location getStandLocation()
            {
                return standLocation;
            }

            public Location getMinCoords()
            {
                return minCoords;
            }

            public Location getMaxCoords()
            {
                return maxCoords;
            }

            public int getOption()
            {
                return option;
            }
        }

        public class Lever
        {
            public int id;
            public Location faceLocation;
            public Location leverLocation;
            public Location teleLocation;
            private bool inUse;

            public Lever()
            {
            }

            public Lever(int id, Location leverLocation, Location faceLocation, Location teleLocation)
            {
                this.id = id;
                this.leverLocation = leverLocation;
                this.faceLocation = faceLocation;
                this.teleLocation = teleLocation;
                this.inUse = false;
            }

            public Location getTeleLocation()
            {
                return teleLocation;
            }

            public Location getFaceLocation()
            {
                return faceLocation;
            }

            public int getId()
            {
                return id;
            }

            public bool isInUse()
            {
                return inUse;
            }

            public void setInUse(bool inUse)
            {
                this.inUse = inUse;
            }

            public Location getLeverLocation()
            {
                return leverLocation;
            }
        }

        public static List<HeightObject> objects = new List<HeightObject>();
        public static List<Lever> levers = new List<Lever>();

        public LoadedLaddersAndStairs()
        {
            if (!File.Exists(Misc.getServerPath() + @"\data\laddersAndStairs.xml"))
            {
                Console.WriteLine(@"Missing data\laddersAndStairs.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\laddersAndStairs.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<HeightObject>));
                objects = (List<HeightObject>)serializer.Deserialize(objStreamReader);
                Console.WriteLine("Loaded " + objects.Count + " ladders and stair teleports.");
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }

            if (!File.Exists(Misc.getServerPath() + @"\data\levers.xml"))
            {
                Console.WriteLine(@"Missing data\levers.xml");
                return;
            }
            try
            {
                //Deserialize text file to a new object.
                StreamReader objStreamReader = new StreamReader(Misc.getServerPath() + @"\data\levers.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<Lever>));
                levers = (List<Lever>)serializer.Deserialize(objStreamReader);
                Console.WriteLine("Loaded " + levers.Count + " lever teleports.");
            }
            catch (Exception e)
            {
                Misc.WriteError((e.InnerException == null ? e.ToString() : e.InnerException.ToString()));
            }
        }
    }

    internal class LaddersAndStairs
    {
        private static LoadedLaddersAndStairs instance = null;

        public static void load()
        {
            instance = new LoadedLaddersAndStairs();
        }

        public static bool useObject(Player p, int id, Location location, int option)
        {
            foreach (LoadedLaddersAndStairs.HeightObject heightObject in LoadedLaddersAndStairs.objects)
            {
                if (heightObject.getId() == id)
                {
                    if (heightObject.getLocation().Equals(location) && heightObject.getOption() == option)
                    {
                        LoadedLaddersAndStairs.HeightObject obj = heightObject;
                        if (heightObject.getType() == LoadedLaddersAndStairs.Event_Type.COORDINATE_POSITION)
                        {
                            CoordinateEvent useObjectCoordinateEvent = new CoordinateEvent(p, heightObject.getStandLocation());
                            useObjectCoordinateEvent.setAction(() =>
                            {
                                LaddersAndStairs.teleport(p, obj);
                            });
                            Server.registerCoordinateEvent(useObjectCoordinateEvent);
                        }
                        else if (heightObject.getType() == LoadedLaddersAndStairs.Event_Type.AREA_POSITION)
                        {
                            AreaEvent useObjectAreaEvent = new AreaEvent(p, heightObject.getMinCoords().getX(), heightObject.getMinCoords().getY(), heightObject.getMaxCoords().getX(), heightObject.getMaxCoords().getY());
                            useObjectAreaEvent.setAction(() =>
                            {
                                LaddersAndStairs.teleport(p, obj);
                            });
                            Server.registerCoordinateEvent(useObjectAreaEvent);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static void teleport(Player p, LoadedLaddersAndStairs.HeightObject obj)
        {
            p.getWalkingQueue().resetWalkingQueue();
            p.setTemporaryAttribute("unmovable", true);
            p.setFaceLocation(obj.getLocation());
            if (obj.getAnimation() != -1)
            {
                p.setLastAnimation(new Animation(obj.getAnimation()));
            }
            Event teleportEvent = new Event(obj.getAnimation() != -1 ? obj.getTeleDelay() : 500);
            teleportEvent.setAction(() =>
            {
                teleportEvent.stop();
                p.teleport(obj.getTeleLocation());
                p.removeTemporaryAttribute("unmovable");
            });
            Server.registerEvent(teleportEvent);
        }

        public static void useLever(Player p, int id, Location leverLocation)
        {
            if (p.getTemporaryAttribute("teleporting") != null)
            {
                return;
            }
            foreach (LoadedLaddersAndStairs.Lever lever in LoadedLaddersAndStairs.levers)
            {
                if (lever.getId() == id)
                {
                    if (lever.getLeverLocation().Equals(leverLocation))
                    {
                        LoadedLaddersAndStairs.Lever l = lever;
                        //TODO when in use it cant be used (in use = lever is facing down)
                        CoordinateEvent useLeverCoordinateEvent = new CoordinateEvent(p, l.getLeverLocation());
                        useLeverCoordinateEvent.setAction(() =>
                        {
                            p.setFaceLocation(l.getFaceLocation());
                            if (p.getTemporaryAttribute("teleblocked") != null)
                            {
                                p.getPackets().sendMessage("A magical force prevents you from teleporting!");
                                return;
                            }
                            else if ((p.getTemporaryAttribute("teleporting") != null))
                            {
                                return;
                            }
                            p.setLastAnimation(new Animation(2140));
                            p.getPackets().closeInterfaces();
                            p.setTemporaryAttribute("teleporting", true);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            SkillHandler.resetAllSkills(p);
                            l.setInUse(true);
                            Event useLeverEvent = new Event(700);
                            useLeverEvent.setAction(() =>
                            {
                                useLeverEvent.stop();
                                p.setLastAnimation(new Animation(8939, 0));
                                p.setLastGraphics(new Graphics(1576, 0));
                                l.setInUse(false);
                                Event setLeverTeleportEvent = new Event(1800);
                                setLeverTeleportEvent.setAction(() =>
                                {
                                    setLeverTeleportEvent.stop();
                                    p.teleport(l.getTeleLocation());
                                    p.setLastAnimation(new Animation(8941, 0));
                                    p.setLastGraphics(new Graphics(1577, 0));
                                    Teleport.resetTeleport(p);
                                });
                                Server.registerEvent(setLeverTeleportEvent);
                            });
                            Server.registerEvent(useLeverEvent);
                        });
                        Server.registerCoordinateEvent(useLeverCoordinateEvent);
                        break;
                    }
                }
            }
        }
    }
}