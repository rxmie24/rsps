using RS2.Server.events;
using RS2.Server.model;

namespace RS2.Server.player.skills.agility
{
    internal class WildernessCourse
    {
        public WildernessCourse()
        {
        }

        public static void doCourse(Player p, int objectX, int objectY, object[] objectArray)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch ((int)objectArray[0])
            {
                case 2309: //Entrance log
                    CoordinateEvent startEntranceLogCoordinateEvent = new CoordinateEvent(p, new Location((int)objectArray[1], (int)objectArray[2], 0));
                    startEntranceLogCoordinateEvent.setAction(() =>
                    {
                        bool running = p.getWalkingQueue().isRunToggled();
                        Event comeToLogEvent = new Event(500);
                        comeToLogEvent.setAction(() =>
                        {
                            p.getWalkingQueue().setRunToggled(false);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            p.getWalkingQueue().forceWalk(0, 1); //go past gate, no animation yet.
                            comeToLogEvent.stop();
                        });
                        Server.registerEvent(comeToLogEvent);
                        int doLogWalkCounter = 0;
                        Event doLogWalkEvent = new Event(800);
                        doLogWalkEvent.setAction(() =>
                        {
                            if (doLogWalkCounter == 0)
                            { //start the animation
                                p.getAppearance().setWalkAnimation(155);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                doLogWalkEvent.setTick(500); //500 milliseconds required to make animations realistic.
                            }
                            else if (doLogWalkCounter < 16)
                            { //15 steps foward, 1 step is just quickfix TODO: Add gate opener.
                                p.getWalkingQueue().forceWalk(0, 1);
                            }
                            else if (doLogWalkCounter == 17)
                            { //stop the animation add the xp.
                                doLogWalkEvent.stop();
                                p.getAppearance().setWalkAnimation(-1);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            }
                            doLogWalkCounter++;
                        });
                        Server.registerEvent(doLogWalkEvent);
                    });
                    Server.registerCoordinateEvent(startEntranceLogCoordinateEvent);
                    break;

                case 2288: // Tunnel
                    AreaEvent startTunnelAreaEvent = new AreaEvent(p, 3003, 3937, 3005, 3938);
                    startTunnelAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX + 1 && pY == objectY) // right side
                            newMove = 1;
                        else if (pX == objectX - 1 && pY == objectY) // left side
                            newMove = 2;
                        if (newMove > 0)
                        {
                            int walkTunnelCounter = 0;
                            Event walkTunnelEvent = new Event(500);
                            walkTunnelEvent.setAction(() =>
                            {
                                if (walkTunnelCounter == 0)
                                {
                                    p.getWalkingQueue().forceWalk(0, -1);
                                }
                                else if (walkTunnelCounter == 1)
                                {
                                    p.getWalkingQueue().forceWalk(newMove == 1 ? -1 : +1, 0);
                                }
                                else
                                {
                                    doCourse(p, objectX, objectY, objectArray);
                                    walkTunnelEvent.stop();
                                }
                                walkTunnelCounter++;
                            });
                            Server.registerEvent(walkTunnelEvent);
                            return;
                        }
                        Event squeezeIntoPipeEvent = new Event(0);
                        squeezeIntoPipeEvent.setAction(() =>
                        {
                            squeezeIntoPipeEvent.stop();
                            p.getPackets().sendMessage("You squeeze into the pipe...");
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            p.setForceMovement(new ForceMovement(lX, lY, lX, lY + 3, 10, 60, 0));
                            p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() + 1, 0));
                            p.setLastAnimation(new Animation(10578));
                            bool running = p.getWalkingQueue().isRunToggled();
                            p.getWalkingQueue().setRunToggled(false);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            Event squeezeOutOfPipeEvent = new Event(1000);
                            int squeezeOutOfPipeCounter = 0;
                            squeezeOutOfPipeEvent.setAction(() =>
                            {
                                if (squeezeOutOfPipeCounter == 0)
                                {
                                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 9, 0));
                                    squeezeOutOfPipeEvent.setTick(850);
                                }
                                else if (squeezeOutOfPipeCounter == 1)
                                {
                                    ForceMovement movement = new ForceMovement(lX, lY + 9, lX, lY + 12, 10, 90, 0);
                                    p.setForceMovement(movement);
                                    squeezeOutOfPipeEvent.setTick(1100);
                                }
                                else if (squeezeOutOfPipeCounter == 2)
                                {
                                    squeezeOutOfPipeEvent.setTick(500);
                                    p.setLastAnimation(new Animation(10579));
                                    p.setForceMovement(new ForceMovement(lX, lY + 12, lX, lY + 13, 10, 40, 0));
                                }
                                else
                                {
                                    p.getPackets().sendMessage("...You squeeze out of the pipe.");
                                    squeezeOutOfPipeEvent.stop();
                                    p.removeTemporaryAttribute("unmovable");
                                    p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                    p.getWalkingQueue().setRunToggled(running);
                                    p.teleport(new Location(3004, 3950, 0));
                                }
                                squeezeOutOfPipeCounter++;
                            });
                            Server.registerEvent(squeezeOutOfPipeEvent);
                        });
                        Server.registerEvent(squeezeIntoPipeEvent);
                    });
                    Server.registerCoordinateEvent(startTunnelAreaEvent);
                    break;

                case 2283: // Rope swing
                    AreaEvent startRopeSwingAreaEvent = new AreaEvent(p, 3004, 3951, 3006, 3953);
                    startRopeSwingAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX - 1 && pY == objectY + 1) // front left
                            newMove = 1;
                        else if (pX == objectX + 1 && pY == objectY + 1) // front right
                            newMove = 2;
                        else if (pX == objectX - 1 && pY == objectY) // back left
                            newMove = 3;
                        else if (pX == objectX + 1 && pY == objectY) // back right
                            newMove = 4;
                        else if (pX == objectX && pY == objectY - 1) // very back middle
                            newMove = 5;
                        else if (pX == objectX && pY == objectY) //  middle
                            newMove = 6;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int walkRopeSwingCounter = 0;
                            Event walkRopeSwingEvent = new Event(500);
                            walkRopeSwingEvent.setAction(() =>
                            {
                                p.setFaceLocation(new Location(3005, 3958, 0));
                                if (path == 1 || path == 2)
                                {
                                    if (walkRopeSwingCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(path == 1 ? +1 : -1, 0);
                                    }
                                    else if (walkRopeSwingCounter >= 1)
                                    {
                                        walkRopeSwingEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                else if (path == 3 || path == 4)
                                {
                                    if (walkRopeSwingCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(path == 3 ? +1 : -1, +1);
                                    }
                                    else if (walkRopeSwingCounter >= 1)
                                    {
                                        walkRopeSwingEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                else if (path == 5 || path == 6)
                                {
                                    if (walkRopeSwingCounter == 0)
                                    {
                                        p.getWalkingQueue().forceWalk(0, path == 5 ? +2 : +1);
                                    }
                                    else if (walkRopeSwingCounter >= 1)
                                    {
                                        walkRopeSwingEvent.stop();
                                        doCourse(p, objectX, objectY, objectArray);
                                    }
                                }
                                walkRopeSwingCounter++;
                            });
                            Server.registerEvent(walkRopeSwingEvent);
                            return;
                        }
                        Event setupRopeSwingEvent = new Event(0);
                        setupRopeSwingEvent.setAction(() =>
                        {
                            setupRopeSwingEvent.stop();
                            bool running = p.getWalkingQueue().isRunToggled();
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            int newY = p.getLocation().getY() + 5;
                            int dir = 4;
                            p.setLastAnimation(new Animation(751));
                            p.setForceMovement(new ForceMovement(lX, lY, lX, lY + 5, 25, 57, dir));
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            Location objectLocation = new Location(3005, 3952, 0);
                            Event doRopeSwingAnimationEvent = new Event(30);
                            doRopeSwingAnimationEvent.setAction(() =>
                            {
                                doRopeSwingAnimationEvent.stop();
                                foreach (Player nearbyPlayers in Server.getPlayerList())
                                {
                                    if (nearbyPlayers != null)
                                    {
                                        nearbyPlayers.getPackets().newObjectAnimation(objectLocation, 497);
                                    }
                                }
                            });
                            Server.registerEvent(doRopeSwingAnimationEvent);

                            Event finishRopeSwingEvent = new Event(1300);
                            finishRopeSwingEvent.setAction(() =>
                            {
                                finishRopeSwingEvent.stop();
                                p.getAppearance().setWalkAnimation(-1);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                p.removeTemporaryAttribute("unmovable");
                                p.teleport(new Location(p.getLocation().getX(), newY, 0));
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            });
                            Server.registerEvent(finishRopeSwingEvent);
                        });
                        Server.registerEvent(setupRopeSwingEvent);
                    });
                    Server.registerCoordinateEvent(startRopeSwingAreaEvent);
                    break;

                case 37704: // Stepping stones
                    CoordinateEvent startSteppingStonesCoordinateEvent = new CoordinateEvent(p, new Location(3002, 3960, 0));
                    startSteppingStonesCoordinateEvent.setAction(() =>
                    {
                        bool running = p.getWalkingQueue().isRunToggled();
                        int dir = 4;
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        int stepStoneCounter = 0;
                        Event stepStoneEvent = new Event(600);
                        stepStoneEvent.setAction(() =>
                        {
                            if (stepStoneCounter >= 6)
                            {
                                stepStoneEvent.stop();
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            }
                            else
                            {
                                int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                                int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                                int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                                int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                                p.setLastAnimation(new Animation(741));
                                p.setForceMovement(new ForceMovement(lX, lY, lX - 1, lY, 5, 35, dir));
                                Event teleportStepStoneEvent = new Event(550);
                                teleportStepStoneEvent.setAction(() =>
                                {
                                    teleportStepStoneEvent.stop();
                                    p.teleport(new Location(p.getLocation().getX() - 1, p.getLocation().getY(), 0));
                                });
                                Server.registerEvent(teleportStepStoneEvent);
                                stepStoneEvent.setTick(stepStoneCounter == 6 ? 300 : 1100);
                                stepStoneCounter++;
                                p.setFaceLocation(new Location(2995, 3960, 0));
                            }
                        });
                        Server.registerEvent(stepStoneEvent);
                    });
                    Server.registerCoordinateEvent(startSteppingStonesCoordinateEvent);
                    break;

                case 2297: // Log
                    AreaEvent startLogAreaEvent = new AreaEvent(p, 3001, 3944, 3002, 3946);
                    startLogAreaEvent.setAction(() =>
                    {
                        int distanceToWalk = p.getLocation().getX() == 3001 ? -7 : -8;
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX && pY == objectY - 1)
                            newMove = 1;
                        else if (pX == objectX && pY == objectY + 1)
                            newMove = 2;
                        else if (pX == objectX + 1 && pY == objectY + 1)
                            newMove = 3;
                        else if (pX == objectX + 1 && pY == objectY - 1)
                            newMove = 4;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doLogWalkCounter = 0;
                            Event doLogWalkEvent = new Event(500);
                            doLogWalkEvent.setAction(() =>
                            {
                                if (doLogWalkCounter == 0)
                                {
                                    if (path == 1 || path == 2)
                                    {
                                        p.getWalkingQueue().forceWalk(0, path == 1 ? +1 : -1);
                                    }
                                    else if (path == 3 || path == 4)
                                    {
                                        p.getWalkingQueue().forceWalk(0, path == 4 ? +1 : -1);
                                    }
                                }
                                else
                                {
                                    doCourse(p, objectX, objectY, objectArray);
                                    doLogWalkEvent.stop();
                                }
                                doLogWalkCounter++;
                            });
                            Server.registerEvent(doLogWalkEvent);
                            return;
                        }
                        Event doLogAnimationEvent = new Event(0);
                        doLogAnimationEvent.setAction(() =>
                        {
                            doLogAnimationEvent.stop();
                            bool running = p.getWalkingQueue().isRunToggled();
                            p.getWalkingQueue().setRunToggled(false);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            p.getAppearance().setWalkAnimation(155);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getWalkingQueue().forceWalk(distanceToWalk, 0);
                            int delay = distanceToWalk == -7 ? 4100 : 4600;
                            Event doLogFinishEvent = new Event(delay);
                            doLogFinishEvent.setAction(() =>
                            {
                                doLogFinishEvent.stop();
                                p.getAppearance().setWalkAnimation(-1);
                                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                p.getWalkingQueue().setRunToggled(running);
                            });
                            Server.registerEvent(doLogFinishEvent);
                        });
                        Server.registerEvent(doLogAnimationEvent);
                    });
                    Server.registerCoordinateEvent(startLogAreaEvent);
                    break;

                case 2328: // Rocks
                    AreaEvent startRocksAreaEvent = new AreaEvent(p, 2993, 3937, 2995, 3937);
                    startRocksAreaEvent.setAction(() =>
                    {
                        int doRocksAnimationCounter = 0;
                        Event doRocksAnimationEvent = new Event(0);
                        doRocksAnimationEvent.setAction(() =>
                        {
                            if (doRocksAnimationCounter == 0)
                            {
                                p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() - 5, 0));
                                doRocksAnimationCounter++;
                                doRocksAnimationEvent.setTick(500);
                                return;
                            }
                            doRocksAnimationEvent.stop();
                            p.setLastAnimation(new Animation(740));
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.getAppearance().setWalkAnimation(0);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.setTemporaryAttribute("unmovable", true);
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            p.setForceMovement(new ForceMovement(lX, lY, lX, lY - 4, 5, 80, 4));
                            Event finishRocksEvent = new Event(1500);
                            finishRocksEvent.setAction(() =>
                            {
                                finishRocksEvent.stop();
                                p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() - 4, 0));
                                p.setLastAnimation(new Animation(65535));
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                            });
                            Server.registerEvent(finishRocksEvent);
                        });
                        Server.registerEvent(doRocksAnimationEvent);
                    });
                    Server.registerCoordinateEvent(startRocksAreaEvent);
                    break;
            }
        }
    }
}