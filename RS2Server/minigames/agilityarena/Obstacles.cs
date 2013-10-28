using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills.agility;

namespace RS2.Server.minigames.agilityarena
{
    internal class Obstacles : AgilityData
    {
        public Obstacles()
        {
        }

        public static void doObstacle(Player p, int index)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (index)
            {
                case 0: // 3 planks, northern (east side)
                case 1: // 3 planks, middle (east side)
                case 2: // 3 planks, southern (east side)
                case 3: // 3 planks, northern (west side)
                case 4: // 3 planks, middle (west side)
                case 5: // 3 planks, southern (west side)
                    int logXCoord = index <= 2 ? (int)AGILITY_ARENA_OBJECTS[index][1] + 1 : (int)AGILITY_ARENA_OBJECTS[index][1] - 1;
                    int logDirectionX = index <= 2 ? -7 : +7;
                    CoordinateEvent plankObstaclesCoordinateEvent = new CoordinateEvent(p, new Location(logXCoord, (int)AGILITY_ARENA_OBJECTS[index][2], 3));
                    plankObstaclesCoordinateEvent.setAction(() =>
                    {
                        bool running = p.getWalkingQueue().isRunToggled();
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        p.getAppearance().setWalkAnimation(155);
                        p.getUpdateFlags().setAppearanceUpdateRequired(true);
                        p.getWalkingQueue().forceWalk(logDirectionX, 0);
                        Event plankObstaclesEvent = new Event(4300);
                        plankObstaclesEvent.setAction(() =>
                        {
                            plankObstaclesEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.getAppearance().setWalkAnimation(-1);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.getSkills().addXp(Skills.SKILL.AGILITY, (double)AGILITY_ARENA_OBJECTS[index][3]);
                            p.getWalkingQueue().setRunToggled(running);
                        });
                        Server.registerEvent(plankObstaclesEvent);
                    });
                    Server.registerCoordinateEvent(plankObstaclesCoordinateEvent);
                    break;

                case 6: //Handholds obstacle east of planks.
                case 7: //Handholds obstacle west of planks.
                    int handHoldsDirectionX = 1;
                    int handHoldsDirectionY = 1;
                    if (index == 6)
                    {
                        handHoldsDirectionX = -1;
                        handHoldsDirectionY = +1;
                    }
                    else if (index == 7)
                    {
                        handHoldsDirectionX = +1;
                        handHoldsDirectionY = -1;
                    }
                    CoordinateEvent handholdsObstacleCoordinateEvent = new CoordinateEvent(p, new Location((int)AGILITY_ARENA_OBJECTS[index][1], (int)AGILITY_ARENA_OBJECTS[index][2], 3));
                    handholdsObstacleCoordinateEvent.setAction(() =>
                    {
                        bool running = p.getWalkingQueue().isRunToggled();
                        p.getWalkingQueue().setRunToggled(false);
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().clearMapFlag();
                        p.setTemporaryAttribute("unmovable", true);
                        p.setLastAnimation(new Animation(1121));
                        p.setFaceLocation(new Location(p.getLocation().getX(), p.getLocation().getY() + handHoldsDirectionY, 3));
                        Event handholdsObstaclesEvent = new Event(700);
                        int handholdsObstaclesCounter = 0;
                        handholdsObstaclesEvent.setAction(() =>
                        {
                            p.setLastAnimation(new Animation(1122));
                            int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                            int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                            int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                            int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                            p.setForceMovement(new ForceMovement(lX, lY, lX + handHoldsDirectionX, lY, 0, 5, 0));
                            if (handholdsObstaclesCounter++ >= 7)
                            {
                                p.setLastAnimation(new Animation(65535));
                                handholdsObstaclesEvent.stop();
                                p.removeTemporaryAttribute("unmovable");
                                p.getSkills().addXp(Skills.SKILL.AGILITY, (double)AGILITY_ARENA_OBJECTS[index][3]);
                                p.getWalkingQueue().setRunToggled(running);
                                return;
                            }
                            Event teleportEvent = new Event(500);
                            teleportEvent.setAction(() =>
                            {
                                teleportEvent.stop();
                                p.teleport(new Location(p.getLocation().getX() + handHoldsDirectionX, p.getLocation().getY(), 3));
                            });
                            Server.registerEvent(teleportEvent);
                        });
                        Server.registerEvent(handholdsObstaclesEvent);
                    });
                    Server.registerCoordinateEvent(handholdsObstacleCoordinateEvent);
                    break;
            }
        }
    }
}