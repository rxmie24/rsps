using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player.skills.mining;
using RS2.Server.player.skills.woodcutting;

namespace RS2.Server.player.skills.runecrafting
{
    internal class AbyssObstacles
    {
        public AbyssObstacles()
        {
        }

        public static void mineRock(Player p, int x, int y)
        {
            AreaEvent mineRockAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 3, y + 1);
            mineRockAreaEvent.setAction(() =>
            {
                if (!Mining.hasPickaxe(p))
                {
                    p.getPackets().sendMessage("You need a pickaxe to get past this obstacle.");
                    return;
                }
                p.getWalkingQueue().resetWalkingQueue();
                p.setFaceLocation(new Location(x + 1, y, 0));
                p.setLastAnimation(new Animation(Mining.getPickaxeAnimation(p)));
                p.setTemporaryAttribute("unmovable", true);
                Event mineRockEvent = new Event(1900);
                mineRockEvent.setAction(() =>
                {
                    int status = 0;
                    int[] ROCKS = { 7158, 7159, 7160 };
                    if (status < 3)
                    {
                        p.getPackets().createObject(ROCKS[status], new Location(x, y, 0), 0, 10);
                    }
                    status++;
                    if (status == 1)
                    {
                        mineRockEvent.setTick(1300);
                    }
                    if (status == 3)
                    {
                        p.setLastAnimation(new Animation(65535));
                        mineRockEvent.setTick(800);
                    }
                    if (status == 4)
                    {
                        mineRockEvent.stop();
                        teleportPastObstacle(p);
                        p.removeTemporaryAttribute("unmovable");
                    }
                });
                Server.registerEvent(mineRockEvent);
                return;
            });
            Server.registerCoordinateEvent(mineRockAreaEvent);
        }

        public static void burnBoil(Player p, int x, int y)
        {
            AreaEvent burnBoilAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 3, y + 2);
            burnBoilAreaEvent.setAction(() =>
            {
                if (!p.getInventory().hasItem(590))
                {
                    p.getPackets().sendMessage("You need a tinderbox to get past this obstacle.");
                    return;
                }
                p.setFaceLocation(new Location(x + 1, y, 0));
                p.setLastAnimation(new Animation(733));
                p.setTemporaryAttribute("unmovable", true);
                Event burnBoilEvent = new Event(1900);
                burnBoilEvent.setAction(() =>
                {
                    int status = 0;
                    int[] BOIL = { 7165, 7166, 7167 };
                    if (status < 3)
                    {
                        p.getPackets().createObject(BOIL[status], new Location(x, y, 0), x == 3060 ? 3 : 1, 10);
                    }
                    status++;
                    if (status == 1)
                    {
                        burnBoilEvent.setTick(1300);
                    }
                    if (status == 3)
                    {
                        p.setLastAnimation(new Animation(65535));
                        burnBoilEvent.setTick(1000);
                    }
                    if (status == 4)
                    {
                        burnBoilEvent.stop();
                        teleportPastObstacle(p);
                        p.removeTemporaryAttribute("unmovable");
                    }
                });
                Server.registerEvent(burnBoilEvent);
                return;
            });
            Server.registerCoordinateEvent(burnBoilAreaEvent);
        }

        public static void useAgilityTunnel(Player p, int x, int y)
        {
            AreaEvent useAgilityTunnelAreaEvent = new AreaEvent(p, x - 2, y - 2, y + 2, y + 2);
            useAgilityTunnelAreaEvent.setAction(() =>
            {
                p.getWalkingQueue().resetWalkingQueue();
                p.setLastAnimation(new Animation(844));
                p.setTemporaryAttribute("unmovable", true);
                p.setFaceLocation(new Location(x, y, 0));
                Event useAgilityTunnelEvent = new Event(1000);
                useAgilityTunnelEvent.setAction(() =>
                {
                    p.getPackets().sendMessage("You squeeze through the gap.");
                    teleportPastObstacle(p);
                    p.removeTemporaryAttribute("unmovable");
                    useAgilityTunnelEvent.stop();
                });
                Server.registerEvent(useAgilityTunnelEvent);
                return;
            });
            Server.registerCoordinateEvent(useAgilityTunnelAreaEvent);
        }

        public static void passEyes(Player p, int x, int y)
        {
            int var = x == 3058 ? x + 2 : x - 1;
            AreaEvent passEyesAreaEvent = new AreaEvent(p, var, y, var, y + 2);
            passEyesAreaEvent.setAction(() =>
            {
                p.getWalkingQueue().resetWalkingQueue();
                p.setFaceLocation(new Location(x, y, 0));
                p.setTemporaryAttribute("unmovable", true);
                p.getPackets().sendMessage("You attempt to distract the eyes...");
                Event passEyesEvent = new Event(1900);
                passEyesEvent.setAction(() =>
                {
                    int status = 0;
                    int[] EYES = { 7168, 7169, 7170 };
                    if (status == 0)
                    {
                        p.getPackets().createObject(EYES[1], new Location(x, y, 0), x == 3058 ? 3 : 1, 10);
                    }
                    status++;
                    if (status == 1)
                    {
                        passEyesEvent.setTick(1300);
                    }
                    if (status == 2)
                    {
                        p.setLastAnimation(new Animation(65535));
                        passEyesEvent.setTick(800);
                    }
                    if (status == 3)
                    {
                        passEyesEvent.stop();
                        p.getPackets().sendMessage("You distract the eyes and pass them.");
                        teleportPastObstacle(p);
                        p.removeTemporaryAttribute("unmovable");
                    }
                });
                Server.registerEvent(passEyesEvent);
                return;
            });
            Server.registerCoordinateEvent(passEyesAreaEvent);
        }

        public static void chopTendrils(Player p, int x, int y)
        {
            int var = x == 3057 ? x + 2 : x - 1;
            AreaEvent chopTendrilsAreaEvent = new AreaEvent(p, var, y, var, y + 2);
            chopTendrilsAreaEvent.setAction(() =>
            {
                if (!Woodcutting.hasAxe(p))
                {
                    p.getPackets().sendMessage("You need an axe to get past this obstacle.");
                    return;
                }
                p.getWalkingQueue().resetWalkingQueue();
                p.setFaceLocation(new Location(x + 1, y, 0));
                p.setLastAnimation(new Animation(Woodcutting.getAxeAnimation(p)));
                p.setTemporaryAttribute("unmovable", true);
                Event chopTendrilsEvent = new Event(1900);
                chopTendrilsEvent.setAction(() =>
                {
                    int status = 0;
                    int[] TENDRILS = { 7161, 7162, 7163 };
                    if (status < 3)
                    {
                        p.getPackets().createObject(TENDRILS[status], new Location(x, y, 0), x == 3057 ? 3 : 1, 10);
                    }
                    status++;
                    if (status == 1)
                    {
                        p.setLastAnimation(new Animation(Woodcutting.getAxeAnimation(p)));
                        chopTendrilsEvent.setTick(1300);
                    }
                    if (status == 3)
                    {
                        p.getPackets().sendMessage("You clear your way through the tendrils.");
                        p.setLastAnimation(new Animation(65535));
                        chopTendrilsEvent.setTick(800);
                    }
                    if (status == 4)
                    {
                        chopTendrilsEvent.stop();
                        teleportPastObstacle(p);
                        p.removeTemporaryAttribute("unmovable");
                    }
                });
                Server.registerEvent(chopTendrilsEvent);
                return;
            });
            Server.registerCoordinateEvent(chopTendrilsAreaEvent);
        }

        private static void teleportPastObstacle(Player p)
        {
            p.teleport(RuneCraft.teleportInner());
        }
    }
}