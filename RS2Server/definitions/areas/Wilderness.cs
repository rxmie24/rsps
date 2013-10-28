using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.magic;
using System;

namespace RS2.Server.definitions.areas
{
    internal class Wilderness
    {
        private static int[][] LEVER_COORDINATES = {
		    new int[] {2538, 2832, 0}, // neitiznot lever
		    new int[] {3097, 3475, 0} // edgeville lever
	    };

        private static int[][] LEVER_FACE_COORDINATES = {
		    new int[] {2538, 2831, 0}, // neitiznot lever
		    new int[] {3098, 3475, 0} // edgeville lever
	    };

        public Wilderness()
        {
        }

        public static void handleLever(Player p, int id, Location loc)
        {
            if ((p.getTemporaryAttribute("teleporting") != null))
            {
                return;
            }
            for (int i = 0; i < LEVER_COORDINATES.Length; i++)
            {
                Location loc1 = new Location(LEVER_COORDINATES[i][0], LEVER_COORDINATES[i][1], LEVER_COORDINATES[i][2]);
                if (loc.Equals(loc1))
                {
                    int index = i;
                    Location loc2 = new Location(LEVER_FACE_COORDINATES[i][0], LEVER_FACE_COORDINATES[i][1], LEVER_FACE_COORDINATES[i][2]);
                    CoordinateEvent wildernessHandleLeverCoordinateEvent = new CoordinateEvent(p, loc);
                    wildernessHandleLeverCoordinateEvent.setAction(() =>
                    {
                        p.setFaceLocation(loc2);
                        displayWildernessLeverOptions(p, index);
                    });
                    Server.registerCoordinateEvent(wildernessHandleLeverCoordinateEvent);
                    return;
                }
            }
            LaddersAndStairs.useLever(p, id, loc); // Used for default levers/levers with no options
        }

        public static void displayWildernessLeverOptions(Player p, int leverIndex)
        {
            int dialogueIndex = 140 + leverIndex;
            p.setTemporaryAttribute("dialogue", dialogueIndex);
            string option1 = leverIndex == 0 ? "Edgeville" : "Home";
            p.getPackets().modifyText(option1, 230, 2);
            p.getPackets().modifyText("Mage bank", 230, 3);
            p.getPackets().modifyText("Nowhere", 230, 4);
            p.getPackets().sendChatboxInterface2(230);
        }

        public static void leverTeleport(Player p, int option)
        {
            p.getPackets().closeInterfaces();
            Location teleLocation = new Location(LEVER_COORDINATES[option][0], LEVER_COORDINATES[option][1], LEVER_COORDINATES[option][2]);

            Event leverTeleportEvent = new Event(200);
            leverTeleportEvent.setAction(() =>
            {
                leverTeleportEvent.stop();
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
                Event levelTeleportStartEvent = new Event(700);
                levelTeleportStartEvent.setAction(() =>
                {
                    levelTeleportStartEvent.stop();
                    p.setLastAnimation(new Animation(8939, 0));
                    p.setLastGraphics(new Graphics(1576, 0));
                    Event levelTeleportFinishEvent = new Event(1800);
                    levelTeleportFinishEvent.setAction(() =>
                    {
                        levelTeleportFinishEvent.stop();
                        p.teleport(teleLocation);
                        p.setLastAnimation(new Animation(8941, 0));
                        p.setLastGraphics(new Graphics(1577, 0));
                        Teleport.resetTeleport(p);
                    });
                    Server.registerEvent(levelTeleportFinishEvent);
                });
                Server.registerEvent(levelTeleportStartEvent);
            });
            Server.registerEvent(leverTeleportEvent);
        }

        public static void slashWeb(Player p, ushort webId, Location webLocation)
        {
            AreaEvent slashWebAreaEvent = new AreaEvent(p, webLocation.getX() - 1, webLocation.getY() - 1, webLocation.getX() + 1, webLocation.getY() + 1);
            slashWebAreaEvent.setAction(() =>
            {
                long lastSlash = 0;
                p.setFaceLocation(webLocation);
                if (p.getTemporaryAttribute("lastWebSlash") != null)
                {
                    lastSlash = (int)p.getTemporaryAttribute("lastWebSlash");
                }
                if (Environment.TickCount - lastSlash <= 800)
                {
                    return;
                }
                if (Server.getGlobalObjects().originalObjectExists(webId, webLocation))
                {
                    p.setLastAnimation(new Animation(p.getAttackAnimation()));
                    p.setTemporaryAttribute("lastWebSlash", Environment.TickCount);
                    Event attemptCutWebEvent = new Event(500);
                    attemptCutWebEvent.setAction(() =>
                    {
                        attemptCutWebEvent.stop();
                        bool webExists = Server.getGlobalObjects().originalObjectExists(webId, webLocation);
                        Server.getGlobalObjects().lowerHealth(webId, webLocation);
                        if (Server.getGlobalObjects().originalObjectExists(webId, webLocation))
                        {
                            p.getPackets().sendMessage("You fail to cut through the web.");
                        }
                        else
                        {
                            if (webExists)
                            { // This means we slashed it, if !webExists, someone else slashed it in the last 500ms
                                p.getPackets().sendMessage("You slash through the web!");
                            }
                        }
                    });
                    Server.registerEvent(attemptCutWebEvent);
                }
            });
            Server.registerCoordinateEvent(slashWebAreaEvent);
        }

        public static void crossDitch(Player p, int x, int y)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            AreaEvent crossDitchAreaEvent = new AreaEvent(p, x, y - 1, x, y + 2);
            crossDitchAreaEvent.setAction(() =>
            {
                p.getPackets().closeInterfaces();
                p.getWalkingQueue().resetWalkingQueue();
                p.setTemporaryAttribute("unmovable", true);
                int newY = p.getLocation().getY() >= 3523 ? p.getLocation().getY() - 3 : p.getLocation().getY() + 3;
                int dir = newY == 3 ? 0 : 4;
                Location faceLocation = new Location(p.getLocation().getX(), dir == 3 ? 3523 : 3520, 0);
                p.setFaceLocation(faceLocation);
                Event crossDitchMoveEvent = new Event(500);
                crossDitchMoveEvent.setAction(() =>
                {
                    crossDitchMoveEvent.stop();
                    p.setLastAnimation(new Animation(6132));
                    int regionX = p.getUpdateFlags().getLastRegion().getRegionX();
                    int regionY = p.getUpdateFlags().getLastRegion().getRegionY();
                    int lX = (p.getLocation().getX() - ((regionX - 6) * 8));
                    int lY = (p.getLocation().getY() - ((regionY - 6) * 8));
                    ForceMovement movement = new ForceMovement(lX, lY, lX, newY, 33, 60, dir);
                    p.setForceMovement(movement);
                    p.setFaceLocation(new Location(x, y, 0));
                    Event crossDitchTeleportEvent = new Event(1250);
                    crossDitchTeleportEvent.setAction(() =>
                    {
                        crossDitchTeleportEvent.stop();
                        int playerY = p.getLocation().getY();
                        int nY = playerY >= 3523 ? 3520 : 3523;
                        p.teleport(new Location(p.getLocation().getX(), nY, 0));
                        p.removeTemporaryAttribute("unmovable");
                    });
                    Server.registerEvent(crossDitchTeleportEvent);
                });
                Server.registerEvent(crossDitchMoveEvent);
            });
            Server.registerCoordinateEvent(crossDitchAreaEvent);
        }
    }
}