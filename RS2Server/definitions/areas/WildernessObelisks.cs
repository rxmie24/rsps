using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;

namespace RS2.Server.definitions.areas
{
    internal class WildernessObelisks
    {
        private static int ACTIVATED_ID = 14825;

        private static bool[] obeliskActivated = new bool[6];

        /*
         * These are the MIDDLE of the platform, since the activators are all 2 squares from the center.
         */

        private static int[][] OBELISK_LOCATIONS = {
		    new int[] {3035, 3732, 0}, // 27 wild
		    new int[] {2980, 3866, 0}, // 44 wild
		    new int[] {3156, 3620, 0}, // 13 wild
		    new int[] {3219, 3656, 0}, // 18 wild
		    new int[] {3307, 3916, 0}, // 50 wild
		    new int[] {3106, 3794, 0}  // 35 wild
	    };

        private static ushort[] OBELISK_ID = {
		    14827, // 27 wild
		    14826, // 44 wild
		    14829, // 13 wild
		    14830, // 18 wild
		    14831, // 50 wild
		    14828, // 35 wild
	    };

        public WildernessObelisks()
        {
        }

        public static bool useWildernessObelisk(Player p, int id, Location loc)
        {
            for (int i = 0; i < OBELISK_ID.Length; i++)
            {
                if (id == OBELISK_ID[i])
                {
                    if (loc.inArea(OBELISK_LOCATIONS[i][0] - 2, OBELISK_LOCATIONS[i][1] - 2, OBELISK_LOCATIONS[i][0] + 2, OBELISK_LOCATIONS[i][1] + 2))
                    {
                        AreaEvent useWildernessObeliskAreaEvent = new AreaEvent(p, loc.getX() - 1, loc.getY() - 1, loc.getX() + 1, loc.getY() + 1);
                        useWildernessObeliskAreaEvent.setAction(() =>
                        {
                            activateObelisk(i);
                        });
                        Server.registerCoordinateEvent(useWildernessObeliskAreaEvent);
                    }
                    return true;
                }
            }
            return false;
        }

        private static void activateObelisk(int index)
        {
            if (obeliskActivated[index])
                return;

            Location[] obeliskLocations = getLocations(index);
            for (int i = 0; i < 4; i++)
            {
                WorldObject obj = new WorldObject(OBELISK_ID[index], ACTIVATED_ID, obeliskLocations[i], 0, 10);
                obj.setSecondForm(true);
                Server.getGlobalObjects().add(obj);
                foreach (Player p in Server.getPlayerList())
                {
                    p.getPackets().createObject(ACTIVATED_ID, obeliskLocations[i], 0, 10);
                }
            }
            obeliskActivated[index] = true;
            Event activateObeliskEvent = new Event(4000 + (Misc.random(4)) * 1000);
            activateObeliskEvent.setAction(() =>
            {
                activateObeliskEvent.stop();
                int randomOb = index;
                while (randomOb == index)
                {
                    // While loop so if the random one is the same one, it picks a new one
                    randomOb = Misc.random(OBELISK_ID.Length);
                }
                int random = randomOb;
                foreach (Player p in Server.getPlayerList())
                {
                    if (p != null)
                    {
                        if (p.getLocation().inArea(OBELISK_LOCATIONS[index][0] - 2, OBELISK_LOCATIONS[index][1] - 2, OBELISK_LOCATIONS[index][0] + 2, OBELISK_LOCATIONS[index][1] + 2))
                        {
                            // TODO get the big purple graphic
                            p.setLastGraphics(new Graphics(1690));
                            p.setLastAnimation(new Animation(8939));
                            Player p2 = p;

                            Event obeliskTeleportEvent = new Event(1200);
                            obeliskTeleportEvent.setAction(() =>
                            {
                                obeliskTeleportEvent.stop();
                                p2.teleport(new Location((OBELISK_LOCATIONS[random][0] - 1) + Misc.random(2), (OBELISK_LOCATIONS[random][1] - 1) + Misc.random(2), 0));

                                Event obeliskAnimationEvent = new Event(500);
                                obeliskAnimationEvent.setAction(() =>
                                {
                                    obeliskAnimationEvent.stop();
                                    p2.setLastAnimation(new Animation(8941));
                                });
                                Server.registerEvent(obeliskAnimationEvent);
                            });
                            Server.registerEvent(obeliskTeleportEvent);
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    WorldObject obj = Server.getGlobalObjects().getObject(OBELISK_ID[index], obeliskLocations[i]);
                    Server.getGlobalObjects().restoreObject(obj);
                }
                obeliskActivated[index] = false;
            });
        }

        private static Location[] getLocations(int index)
        {
            Location[] loc = new Location[4];
            int x = OBELISK_LOCATIONS[index][0];
            int y = OBELISK_LOCATIONS[index][1];
            loc[0] = new Location(x - 2, y - 2, 0); // SW
            loc[1] = new Location(x - 2, y + 2, 0); // NW
            loc[2] = new Location(x + 2, y + 2, 0); // NE
            loc[3] = new Location(x + 2, y - 2, 0); // SE
            return loc;
        }
    }
}