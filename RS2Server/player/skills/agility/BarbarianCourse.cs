using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.agility
{
    internal class BarbarianCourse
    {
        public BarbarianCourse()
        {
        }

        public static void doCourse(Player p, int objectX, int objectY, object[] objectArray)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
                return;

            switch ((int)objectArray[0])
            {
                case 20210: // Entrance tunnel
                    AreaEvent entranceTunnelAreaEvent = new AreaEvent(p, 2551, 3558, 2553, 3561);
                    entranceTunnelAreaEvent.setAction(() =>
                    {
                        entranceTunnelAreaEvent.stop();
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX + 1 && pY == objectY)
                            newMove = 1;
                        else if (pX == objectX - 1 && pY == objectY)
                            newMove = 2;
                        else if (pX == objectX - 1 && pY == objectY + 1)
                            newMove = 3;
                        else if (pX == objectX + 1 && pY == objectY + 1)
                            newMove = 4;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doCourseCounter = 0;
                            Event doCourseEvent = new Event(500);
                            doCourseEvent.setAction(() =>
                            {
                                if (doCourseCounter == 0)
                                {
                                    p.getWalkingQueue().forceWalk(0, (path == 1 || path == 2) ? -1 : (path == 3 || path == 4) ? +1 : 0);
                                }
                                else if (doCourseCounter == 1)
                                {
                                    p.getWalkingQueue().forceWalk((path == 1 || path == 4) ? -1 : (path == 2 || path == 3) ? +1 : 0, 0);
                                }
                                else
                                {
                                    doCourse(p, objectX, objectY, objectArray);
                                    doCourseEvent.stop();
                                }
                                doCourseCounter++;
                            });
                            Server.registerEvent(doCourseEvent);
                            return;
                        }
                        int startEnterTunnelCounter = 0;
                        Event startEnterTunnelEvent = new Event(0);
                        startEnterTunnelEvent.setAction(() =>
                        {
                            if (startEnterTunnelCounter == 0)
                            {
                                p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() <= 3558 ? 3561 : 3558, 0));
                                startEnterTunnelEvent.setTick(500);
                                startEnterTunnelCounter++;
                            }
                            else
                            {
                                startEnterTunnelEvent.stop();
                                bool running = p.getWalkingQueue().isRunToggled();
                                int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                                int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                                int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                                int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                                int newLocalY = p.getLocation().getY() == 3558 ? lY + 3 : lY - 3;
                                int newY = newLocalY > lY ? p.getLocation().getY() + 3 : p.getLocation().getY() - 3;
                                int dir = newLocalY > lY ? 0 : 4;
                                p.setForceMovement(new ForceMovement(lX, lY, lX, newLocalY, 10, 60, dir));
                                p.setLastAnimation(new Animation(10580));
                                p.getWalkingQueue().resetWalkingQueue();
                                p.getPackets().clearMapFlag();
                                p.setTemporaryAttribute("unmovable", true);
                                Event enterTunnelEvent = new Event(1500);
                                enterTunnelEvent.setAction(() =>
                                {
                                    enterTunnelEvent.stop();
                                    p.removeTemporaryAttribute("unmovable");
                                    p.teleport(new Location(p.getLocation().getX(), newY, 0));
                                    p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                    p.getWalkingQueue().setRunToggled(running);
                                });
                                Server.registerEvent(enterTunnelEvent);
                            }
                        });
                        Server.registerEvent(startEnterTunnelEvent);
                    });
                    Server.registerCoordinateEvent(entranceTunnelAreaEvent);
                    break;

                case 2282: // Swing
                    AreaEvent swingAreaEvent = new AreaEvent(p, 2550, 3554, 2552, 3555);
                    swingAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX - 1 && pY == objectY + 4) // front left
                            newMove = 1;
                        else if (pX == objectX + 1 && pY == objectY + 4) // front right
                            newMove = 2;
                        else if (pX == objectX - 1 && pY == objectY + 5) // back left
                            newMove = 3;
                        else if (pX == objectX + 1 && pY == objectY + 5) // back right
                            newMove = 4;
                        else if (pX == objectX && pY == objectY + 5) // back middle
                            newMove = 5;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doCourseCounter = 0;
                            Event doCourseEvent = new Event(500);
                            doCourseEvent.setAction(() =>
                            {
                                p.setFaceLocation(new Location(2551, 3549, 0));
                                if (path == 1 || path == 2)
                                {
                                    if (doCourseCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(path == 1 ? +1 : -1, 0);
                                    }
                                    else if (doCourseCounter >= 1)
                                    {
                                        doCourseEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                else if (path == 3 || path == 4)
                                {
                                    if (doCourseCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(path == 3 ? +1 : -1, -1);
                                    }
                                    else if (doCourseCounter >= 1)
                                    {
                                        doCourseEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                else if (path == 5)
                                {
                                    if (doCourseCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(0, -1);
                                    }
                                    else if (doCourseCounter >= 1)
                                    {
                                        doCourseEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                doCourseCounter++;
                            });
                            Server.registerEvent(doCourseEvent);
                            return;
                        }

                        Event doSwingEvent = new Event(0);
                        doSwingEvent.setAction(() =>
                        {
                            doSwingEvent.stop();
                            bool running = p.getWalkingQueue().isRunToggled();
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            int newY = p.getLocation().getY() - 5;
                            int dir = 4;
                            p.setLastAnimation(new Animation(751));
                            p.setForceMovement(new ForceMovement(lX, lY, lX, lY - 5, 25, 57, dir));
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            Location objectLocation = new Location(2551, 3550, 0);
                            Event showObjectAnimationEvent = new Event(30);
                            showObjectAnimationEvent.setAction(() =>
                            {
                                showObjectAnimationEvent.stop();
                                foreach (Player nearbyPlayers in Server.getPlayerList())
                                {
                                    if (nearbyPlayers != null)
                                    {
                                        nearbyPlayers.getPackets().newObjectAnimation(objectLocation, 497);
                                    }
                                }
                            });
                            Server.registerEvent(showObjectAnimationEvent);
                            Event finishSwingEvent = new Event(1300);
                            finishSwingEvent.setAction(() =>
                            {
                                finishSwingEvent.stop();
                                p.getAppearance().setWalkAnimation(-1);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                p.removeTemporaryAttribute("unmovable");
                                p.teleport(new Location(p.getLocation().getX(), newY, 0));
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            });
                            Server.registerEvent(finishSwingEvent);
                        });
                        Server.registerEvent(doSwingEvent);
                    });
                    Server.registerCoordinateEvent(swingAreaEvent);
                    break;

                case 2294: // Log
                    AreaEvent logAreaEvent = new AreaEvent(p, 2550, 3545, 2551, 3547);
                    logAreaEvent.setAction(() =>
                    {
                        int distanceToWalk = p.getLocation().getX() == 2550 ? -9 : -10;
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX && pY == objectY - 1)
                            newMove = 1;
                        else if (pX == objectX && pY == objectY + 1)
                            newMove = 2;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doLogCounter = 0;
                            Event doLogEvent = new Event(500);
                            doLogEvent.setAction(() =>
                            {
                                if (doLogCounter++ == 0)
                                {
                                    p.getWalkingQueue().forceWalk(0, path == 1 ? +1 : -1);
                                }
                                else
                                {
                                    doCourse(p, objectX, objectY, objectArray);
                                    doLogEvent.stop();
                                }
                            });
                            Server.registerEvent(doLogEvent);
                            return;
                        }
                        Event startLogEvent = new Event(0);
                        startLogEvent.setAction(() =>
                        {
                            startLogEvent.stop();
                            bool running = p.getWalkingQueue().isRunToggled();
                            p.getWalkingQueue().setRunToggled(false);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            p.getAppearance().setWalkAnimation(155);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getWalkingQueue().forceWalk(distanceToWalk, 0);
                            int delay = distanceToWalk == -9 ? 5400 : 5900;
                            Event endLogEvent = new Event(delay);
                            endLogEvent.setAction(() =>
                            {
                                endLogEvent.stop();
                                p.getAppearance().setWalkAnimation(-1);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            });
                            Server.registerEvent(endLogEvent);
                        });
                        Server.registerEvent(startLogEvent);
                    });
                    Server.registerCoordinateEvent(logAreaEvent);
                    break;

                case 20211: // Net
                    AreaEvent netAreaEvent = new AreaEvent(p, 2539, 3545, 2539, 3546);
                    netAreaEvent.setAction(() =>
                    {
                        p.setLastAnimation(new Animation(828));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setFaceLocation(new Location(p.getLocation().getX() - 1, p.getLocation().getY(), 0));
                        p.setTemporaryAttribute("unmovable", true);
                        Event netEvent = new Event(1000);
                        netEvent.setAction(() =>
                        {
                            netEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(p.getLocation().getX() - 2, p.getLocation().getY(), 1));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(netEvent);
                    });
                    Server.registerCoordinateEvent(netAreaEvent);
                    break;

                case 2302: // Balance beam
                    CoordinateEvent balanceBeamCoordinateEvent = new CoordinateEvent(p, new Location((int)objectArray[3], (int)objectArray[4], 1));
                    balanceBeamCoordinateEvent.setAction(() =>
                    {
                        bool running = p.getWalkingQueue().isRunToggled();
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        p.getAppearance().setWalkAnimation(157);
                        p.getUpdateFlags().setAppearanceUpdateRequired(true);
                        p.getWalkingQueue().forceWalk(-4, 0);
                        Event balanceBeamEvent = new Event(2450);
                        balanceBeamEvent.setAction(() =>
                        {
                            balanceBeamEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.getAppearance().setWalkAnimation(-1);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                            p.getWalkingQueue().setRunToggled(running);
                        });
                        Server.registerEvent(balanceBeamEvent);
                    });
                    Server.registerCoordinateEvent(balanceBeamCoordinateEvent);
                    break;

                case 1948: // Crumbling wall
                    AreaEvent crumblingWallAreaEvent = new AreaEvent(p, objectX - 1, objectY - 1, objectX, objectY + 1);
                    crumblingWallAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX && pY == objectY - 1)
                            newMove = 1;
                        else if (pX == objectX && pY == objectY + 1)
                            newMove = 2;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            Event startCrumblingWallEvent = new Event(500);
                            int startCrumblingWallCounter = 0;
                            startCrumblingWallEvent.setAction(() =>
                            {
                                if (startCrumblingWallCounter == 0)
                                {
                                    p.getWalkingQueue().forceWalk(-1, 0);
                                }
                                else if (startCrumblingWallCounter == 1)
                                {
                                    p.getWalkingQueue().forceWalk(0, path == 1 ? +1 : -1);
                                }
                                else
                                {
                                    doCourse(p, objectX, objectY, objectArray);
                                    startCrumblingWallEvent.stop();
                                }
                                startCrumblingWallCounter++;
                            });
                            Server.registerEvent(startCrumblingWallEvent);
                            return;
                        }
                        int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                        int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                        int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                        int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                        p.setForceMovement(new ForceMovement(lX, lY, lX + 2, lY, 10, 60, 1));
                        p.setLastAnimation(new Animation(4853));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        Event finishCrumblingWallEvent = new Event(1250);
                        finishCrumblingWallEvent.setAction(() =>
                        {
                            finishCrumblingWallEvent.stop();
                            p.setFaceLocation(new Location(p.getLocation().getX() + 1, p.getLocation().getY(), 0));
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(p.getLocation().getX() + 2, p.getLocation().getY(), 0));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(finishCrumblingWallEvent);
                    });
                    Server.registerCoordinateEvent(crumblingWallAreaEvent);
                    break;
            }
        }

        public static void useLadder(Player p)
        {
            CoordinateEvent useLadderCoordinateEvent = new CoordinateEvent(p, new Location(2532, 3546, 1));
            useLadderCoordinateEvent.setAction(() =>
            {
                p.setLastAnimation(new Animation(828));
                p.setTemporaryAttribute("unmovable", true);
                Event useLadderEvent = new Event(1000);
                useLadderEvent.setAction(() =>
                {
                    useLadderEvent.stop();
                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY(), 0));
                    p.removeTemporaryAttribute("unmovable");
                });
                Server.registerEvent(useLadderEvent);
            });
            Server.registerCoordinateEvent(useLadderCoordinateEvent);
        }
    }
}