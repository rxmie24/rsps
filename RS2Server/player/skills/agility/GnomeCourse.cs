using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using System.Collections.Generic;

namespace RS2.Server.player.skills.agility
{
    internal class GnomeCourse
    {
        private static List<Npc> shoutNPCs = new List<Npc>();

        private static string[] SHOUT_MESSAGES = {
		    "Okay get over that log, quick quick!",
		    "Move it, move it, move it!",
		    "That's it - straight up!",
		    "Come on scaredy cat, get across that rope!",
		    "My Granny can move faster than you!",
	    };

        public GnomeCourse()
        {
            shoutNPCs = new List<Npc>();
            shoutNPCs.Add(Server.getNpcList()[21]); //TODO: Maybe make this class spawn these Npc's?
            shoutNPCs.Add(Server.getNpcList()[22]);
            shoutNPCs.Add(Server.getNpcList()[28]);
            shoutNPCs.Add(Server.getNpcList()[29]);
            shoutNPCs.Add(Server.getNpcList()[26]);
        }

        public static void doCourse(Player p, int objectX, int objectY, object[] objectArray)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            int agilityStage = (int)(p.getTemporaryAttribute("agilityStage") == null ? 0 : p.getTemporaryAttribute("agilityStage"));
            switch ((int)objectArray[0])
            {
                case 2295: // Log
                    CoordinateEvent doLogCoordinateEvent = new CoordinateEvent(p, new Location((int)objectArray[3], (int)objectArray[4], 0));
                    doLogCoordinateEvent.setAction(() =>
                    {
                        shoutNPCs[0].setForceText(SHOUT_MESSAGES[0]);
                        p.getPackets().sendMessage("You walk carefully across the slippery log...");
                        bool running = p.getWalkingQueue().isRunToggled();
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        p.getAppearance().setWalkAnimation(155);
                        p.getUpdateFlags().setAppearanceUpdateRequired(true);
                        p.getWalkingQueue().forceWalk(0, -7);
                        Event doLogEvent = new Event(4300);
                        doLogEvent.setAction(() =>
                        {
                            doLogEvent.stop();
                            p.getPackets().sendMessage("...and make it safely to the other side.");
                            p.removeTemporaryAttribute("unmovable");
                            p.getAppearance().setWalkAnimation(-1);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                            p.getWalkingQueue().setRunToggled(running);
                        });
                        Server.registerEvent(doLogEvent);
                    });
                    Server.registerCoordinateEvent(doLogCoordinateEvent);
                    break;

                case 2285: // Net #1
                    AreaEvent doNetOneAreaEvent = new AreaEvent(p, 2471, 3426, 2476, 3426);
                    doNetOneAreaEvent.setAction(() =>
                    {
                        shoutNPCs[1].setForceText(SHOUT_MESSAGES[1]);
                        p.getPackets().sendMessage("You climb the netting...");
                        p.setLastAnimation(new Animation(828));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() - 1, 0));
                        p.setTemporaryAttribute("unmovable", true);
                        Event doNetOneEvent = new Event(1000);
                        doNetOneEvent.setAction(() =>
                        {
                            doNetOneEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(2473, p.getLocation().getY() - 2, 1));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(doNetOneEvent);
                    });
                    Server.registerCoordinateEvent(doNetOneAreaEvent);
                    break;

                case 35970: // Tree climb
                    AreaEvent treeClimbAreaEvent = new AreaEvent(p, 2472, 3422, 2474, 3423);
                    treeClimbAreaEvent.setAction(() =>
                    {
                        shoutNPCs[2].setForceText(SHOUT_MESSAGES[2]);
                        p.getPackets().sendMessage("You climb the tree...");
                        p.setLastAnimation(new Animation(828));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setFaceLocation(new Location(2473, 3422, 1));
                        p.setTemporaryAttribute("unmovable", true);
                        Event treeClimbEvent = new Event(1000);
                        treeClimbEvent.setAction(() =>
                        {
                            treeClimbEvent.stop();
                            p.getPackets().sendMessage("...to the platform above.");
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(2473, 3420, 2));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(treeClimbEvent);
                    });
                    Server.registerCoordinateEvent(treeClimbAreaEvent);
                    break;

                case 2312: // Rope balance
                    CoordinateEvent ropeBalanceCoordinateEvent = new CoordinateEvent(p, new Location((int)objectArray[3], (int)objectArray[4], 2));
                    ropeBalanceCoordinateEvent.setAction(() =>
                    {
                        shoutNPCs[3].setForceText(SHOUT_MESSAGES[3]);
                        p.getPackets().sendMessage("You carefully cross the tightrope.");
                        bool running = p.getWalkingQueue().isRunToggled();
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        p.getAppearance().setWalkAnimation(155);
                        p.getUpdateFlags().setAppearanceUpdateRequired(true);
                        p.getWalkingQueue().forceWalk(+6, 0);
                        Event ropeBalanceEvent = new Event(3700);
                        ropeBalanceEvent.setAction(() =>
                        {
                            ropeBalanceEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.getAppearance().setWalkAnimation(-1);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                            p.getWalkingQueue().setRunToggled(running);
                        });
                        Server.registerEvent(ropeBalanceEvent);
                    });
                    Server.registerCoordinateEvent(ropeBalanceCoordinateEvent);
                    break;

                case 2314: // Tree climb #2
                    AreaEvent treeclimbTwoAreaEvent = new AreaEvent(p, 2485, 3418, 2486, 3420);
                    treeclimbTwoAreaEvent.setAction(() =>
                    {
                        p.getPackets().sendMessage("You climb down the tree...");
                        p.setLastAnimation(new Animation(828));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setFaceLocation(new Location(2486, 3419, 2));
                        p.setTemporaryAttribute("unmovable", true);
                        Event treeClimbTwoEvent = new Event(1000);
                        treeClimbTwoEvent.setAction(() =>
                        {
                            treeClimbTwoEvent.stop();
                            p.getPackets().sendMessage("You land on the ground.");
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(2486, 3420, 0));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(treeClimbTwoEvent);
                    });
                    Server.registerCoordinateEvent(treeclimbTwoAreaEvent);
                    break;

                case 2286: // Net #2
                    AreaEvent doNetTwoAreaEvent = new AreaEvent(p, 2483, 3425, 2488, 3425);
                    doNetTwoAreaEvent.setAction(() =>
                    {
                        shoutNPCs[4].setForceText(SHOUT_MESSAGES[4]);
                        p.getPackets().sendMessage("You climb the netting...");
                        p.setLastAnimation(new Animation(828));
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() + 1, 0));
                        p.setTemporaryAttribute("unmovable", true);
                        Event doNetTwoEvent = new Event(1000);
                        doNetTwoEvent.setAction(() =>
                        {
                            doNetTwoEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 2, 0));
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                        });
                        Server.registerEvent(doNetTwoEvent);
                    });
                    Server.registerCoordinateEvent(doNetTwoAreaEvent);
                    break;

                case 4058: // Right obstacle pipe
                    AreaEvent passRightObstraclePipeAreaEvent = new AreaEvent(p, 2486, 3430, 2488, 3431);
                    passRightObstraclePipeAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX + 1 && pY == objectY)
                            newMove = 1;
                        else if (pX == objectX - 1 && pY == objectY)
                            newMove = 2;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doCourseCounter = 0;
                            Event doCourseEvent = new Event(500);
                            doCourseEvent.setAction(() =>
                            {
                                if (doCourseCounter == 0)
                                {
                                    p.getWalkingQueue().forceWalk(0, -1);
                                }
                                else if (doCourseCounter == 1)
                                {
                                    p.getWalkingQueue().forceWalk(path == 1 ? -1 : +1, 0);
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
                        Event squeezeIntoRightPipeEvent = new Event(0);
                        squeezeIntoRightPipeEvent.setAction(() =>
                        {
                            squeezeIntoRightPipeEvent.stop();
                            p.getPackets().sendMessage("You squeeze into the pipe...");
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            p.setForceMovement(new ForceMovement(lX, lY, lX, lY + 4, 10, 60, 0));
                            p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() + 1, 0));
                            p.setLastAnimation(new Animation(10578));
                            bool running = p.getWalkingQueue().isRunToggled();
                            p.getWalkingQueue().setRunToggled(false);
                            p.getWalkingQueue().resetWalkingQueue();
                            p.getPackets().clearMapFlag();
                            p.setTemporaryAttribute("unmovable", true);
                            int squeezeOutOfRightPipeCounter = 0;
                            Event squeezeOutOfRightPipeEvent = new Event(1150);
                            squeezeOutOfRightPipeEvent.setAction(() =>
                            {
                                if (squeezeOutOfRightPipeCounter == 0)
                                {
                                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 3, 0));
                                    squeezeOutOfRightPipeEvent.setTick(450);
                                    squeezeOutOfRightPipeCounter++;
                                    return;
                                }
                                else if (squeezeOutOfRightPipeCounter == 1)
                                {
                                    p.setForceMovement(new ForceMovement(lX, lY + 3, lX, lY + 6, 10, 60, 0));
                                    squeezeOutOfRightPipeEvent.setTick(900);
                                    squeezeOutOfRightPipeCounter++;
                                }
                                else if (squeezeOutOfRightPipeCounter == 2)
                                {
                                    squeezeOutOfRightPipeEvent.setTick(500);
                                    p.setLastAnimation(new Animation(10579));
                                    p.setForceMovement(new ForceMovement(lX, lY + 6, lX, lY + 7, 10, 40, 0));
                                    squeezeOutOfRightPipeCounter++;
                                }
                                else
                                {
                                    p.getPackets().sendMessage("...You squeeze out of the pipe.");
                                    squeezeOutOfRightPipeEvent.stop();
                                    p.removeTemporaryAttribute("unmovable");
                                    p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                    p.getWalkingQueue().setRunToggled(running);
                                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 5, 0));
                                }
                            });
                            Server.registerEvent(squeezeOutOfRightPipeEvent);
                        });
                        Server.registerEvent(squeezeIntoRightPipeEvent);
                    });
                    Server.registerCoordinateEvent(passRightObstraclePipeAreaEvent);
                    break;

                case 154: // Left obstacle pipe
                    AreaEvent passLeftObstraclePipeAreaEvent = new AreaEvent(p, 2483, 3430, 2485, 3431);
                    passLeftObstraclePipeAreaEvent.setAction(() =>
                    {
                        int newMove = 0;
                        int pX = p.getLocation().getX();
                        int pY = p.getLocation().getY();
                        if (pX == objectX + 1 && pY == objectY)
                            newMove = 1;
                        else if (pX == objectX - 1 && pY == objectY)
                            newMove = 2;
                        else if (pX == 2483)
                            newMove = 3;
                        else if (pX == 2485)
                            newMove = 4;
                        if (newMove > 0)
                        {
                            int path = newMove;
                            int doCourseCounter = 0;
                            Event doCourseEvent = new Event(500);
                            doCourseEvent.setAction(() =>
                            {
                                if (path == 3 || path == 4)
                                {
                                    p.getWalkingQueue().forceWalk(path == 3 ? +1 : -1, 0);
                                    doCourseCounter = 2;
                                }
                                if (doCourseCounter == 0)
                                {
                                    p.getWalkingQueue().forceWalk(0, -1);
                                }
                                else if (doCourseCounter == 1)
                                {
                                    p.getWalkingQueue().forceWalk(path == 1 ? -1 : +1, 0);
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
                        Event squeezeIntoLeftPipeEvent = new Event(0);
                        squeezeIntoLeftPipeEvent.setAction(() =>
                        {
                            squeezeIntoLeftPipeEvent.stop();
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
                            int squeezeOutOfLeftPipeCounter = 0;
                            Event squeezeOutOfLeftPipeEvent = new Event(1150);
                            squeezeOutOfLeftPipeEvent.setAction(() =>
                            {
                                if (squeezeOutOfLeftPipeCounter == 0)
                                {
                                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 3, 0));
                                    squeezeOutOfLeftPipeEvent.setTick(450);
                                    squeezeOutOfLeftPipeCounter++;
                                    return;
                                }
                                else if (squeezeOutOfLeftPipeCounter == 1)
                                {
                                    p.setForceMovement(new ForceMovement(lX, lY + 3, lX, lY + 6, 10, 60, 0));
                                    squeezeOutOfLeftPipeEvent.setTick(900);
                                    squeezeOutOfLeftPipeCounter++;
                                }
                                else if (squeezeOutOfLeftPipeCounter == 2)
                                {
                                    squeezeOutOfLeftPipeEvent.setTick(500);
                                    p.setLastAnimation(new Animation(10579));
                                    p.setForceMovement(new ForceMovement(lX, lY + 6, lX, lY + 7, 10, 40, 0));
                                    squeezeOutOfLeftPipeCounter++;
                                }
                                else
                                {
                                    p.getPackets().sendMessage("...You squeeze out of the pipe.");
                                    squeezeOutOfLeftPipeEvent.stop();
                                    p.removeTemporaryAttribute("unmovable");
                                    p.getSkills().addXp(Skills.SKILL.AGILITY, (double)objectArray[7]);
                                    p.getWalkingQueue().setRunToggled(running);
                                    p.teleport(new Location(p.getLocation().getX(), p.getLocation().getY() + 5, 0));
                                }
                            });
                            Server.registerEvent(squeezeOutOfLeftPipeEvent);
                        });
                        Server.registerEvent(squeezeIntoLeftPipeEvent);
                    });
                    Server.registerCoordinateEvent(passLeftObstraclePipeAreaEvent);
                    break;
            }
        }
    }
}