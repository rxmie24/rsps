using RS2.Server.events;
using RS2.Server.minigames.fightcave;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.magic
{
    internal class Teleport : MagicData
    {
        public Teleport()
        {
        }

        public static void homeTeleport(Player p)
        {
            if (p.getTemporaryAttribute("teleporting") != null || p.getTemporaryAttribute("homeTeleporting") != null || p.getTemporaryAttribute("unmovable") != null || p.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            if (Location.inFightPits(p.getLocation()))
            {
                p.getPackets().sendMessage("You are unable to teleport from the fight pits.");
                return;
            }
            if (Location.inFightCave(p.getLocation()))
            {
                FightCave.antiTeleportMessage(p);
                return;
            }
            if (p.getTemporaryAttribute("teleblocked") != null)
            {
                p.getPackets().sendMessage("A magical force prevents you from teleporting!");
                return;
            }
            if (Location.inWilderness(p.getLocation()) && p.getLocation().wildernessLevel() >= 20)
            {
                p.getPackets().sendMessage("You cannot teleport above level 20 wilderness!");
                return;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().getStatus() < 4)
                {
                    p.getDuel().declineDuel();
                }
                else if (p.getDuel().getStatus() == 5)
                {
                    p.getPackets().sendMessage("You cannot teleport whilst in a duel.");
                    return;
                }
                else if (p.getDuel().getStatus() == 8)
                {
                    if (p.getDuel().getWinner().Equals(p))
                    {
                        p.getDuel().recieveWinnings(p);
                    }
                }
            }
            p.getPackets().closeInterfaces();
            p.setTemporaryAttribute("teleporting", true);
            p.setTemporaryAttribute("homeTeleporting", true);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            SkillHandler.resetAllSkills(p);

            Event teleportHomeAnimationEvent = new Event(500);
            int currentStage = 0;
            teleportHomeAnimationEvent.setAction(() =>
            {
                if (p.getTemporaryAttribute("homeTeleporting") == null)
                {
                    p.setLastAnimation(new Animation(65535, 0));
                    p.setLastGraphics(new Graphics(65535, 0));
                    resetTeleport(p);
                    teleportHomeAnimationEvent.stop();
                    return;
                }
                if (currentStage++ >= 16)
                {
                    resetTeleport(p);
                    p.teleport(new Location(HOME_TELE[0] + Misc.random(HOME_TELE[2]), HOME_TELE[1] + Misc.random(HOME_TELE[3]), 0));
                    teleportHomeAnimationEvent.stop();
                    return;
                }
                p.setLastAnimation(new Animation(HOME_ANIMATIONS[currentStage], 0));
                p.setLastGraphics(new Graphics(HOME_GRAPHICS[currentStage], 0));
            });
            Server.registerEvent(teleportHomeAnimationEvent);
        }

        public static void teleport(Player p, int teleport)
        {
            if (!canTeleport(p, teleport))
            {
                //return;
            }
            if (!deleteRunes(p, TELEPORT_RUNES[teleport], TELEPORT_RUNES_AMOUNT[teleport]))
            {
                //	return;
            }
            p.removeTemporaryAttribute("lootedBarrowChest"); // so it resets instantly.
            p.removeTemporaryAttribute("autoCasting");
            p.setTarget(null);
            bool ancients = teleport > 6 ? true : false;
            int playerMagicSet = p.getMagicType();
            bool correctMagicSet = (!ancients && playerMagicSet == 1) || (ancients && playerMagicSet == 2);
            if (!correctMagicSet)
            {
                return;
            }
            int x = TELE_X[teleport] + Misc.random(TELE_EXTRA_X[teleport]);
            int y = TELE_Y[teleport] + Misc.random(TELE_EXTRA_Y[teleport]);
            p.getPackets().closeInterfaces();
            p.setLastAnimation(new Animation(ancients ? 9599 : 8939, 0));
            p.setLastGraphics(new Graphics(ancients ? 1681 : 1576, 0));
            p.getPackets().sendBlankClientScript(1297);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            SkillHandler.resetAllSkills(p);
            p.setTemporaryAttribute("teleporting", true);
            Event startTeleportEvent = new Event(ancients ? 2750 : 1800);
            startTeleportEvent.setAction(() =>
            {
                p.teleport(new Location(x, y, 0));
                if (!ancients)
                {
                    p.setLastAnimation(new Animation(8941, 0));
                    p.setLastGraphics(new Graphics(1577, 0));
                }
                Event endTeleportEvent = new Event(ancients ? 500 : 2000);
                endTeleportEvent.setAction(() =>
                {
                    p.getSkills().addXp(Skills.SKILL.MAGIC, TELEPORT_XP[teleport]);
                    resetTeleport(p);
                    endTeleportEvent.stop();
                });
                Server.registerEvent(endTeleportEvent);
                startTeleportEvent.stop();
            });
            Server.registerEvent(startTeleportEvent);
        }

        private static bool canTeleport(Player p, int teleport)
        {
            if (p.getTemporaryAttribute("teleporting") != null)
            {
                return false;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().getStatus() == 5 || p.getDuel().getStatus() == 6)
                {
                    p.getPackets().sendMessage("You cannot teleport whilst in a duel.");
                    return false;
                }
            }
            if (Location.inFightCave(p.getLocation()))
            {
                FightCave.antiTeleportMessage(p);
                return false;
            }
            if (p.getTemporaryAttribute("teleblocked") != null)
            {
                p.getPackets().sendMessage("A magical force prevents you from teleporting!");
                return false;
            }
            if (p.getTemporaryAttribute("unmovable") != null || p.getTemporaryAttribute("cantDoAnything") != null)
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.MAGIC) < TELEPORT_LVL[teleport])
            {
                p.getPackets().sendMessage("You need a Magic level of " + TELEPORT_LVL[teleport] + " to use this teleport!");
                return false;
            }
            if (!hasRunes(p, TELEPORT_RUNES[teleport], TELEPORT_RUNES_AMOUNT[teleport]))
            {
                p.getPackets().sendMessage("You do not have enough runes to cast this teleport.");
                return false;
            }
            if (Location.inFightPits(p.getLocation()))
            {
                p.getPackets().sendMessage("You are unable to teleport from the fight pits.");
                return false;
            }
            if (Location.inWilderness(p.getLocation()) && p.getLocation().wildernessLevel() >= 20)
            {
                p.getPackets().sendMessage("You cannot teleport above level 20 wilderness!");
                return false;
            }
            if (p.isDead())
            {
                return false;
            }
            return true;
        }

        public static bool useTeletab(Player p, int item, int slot)
        {
            int index = -1;
            for (int i = 0; i < TELETABS.Length; i++)
            {
                if (item == TELETABS[i])
                {
                    index = i;
                }
            }
            if (index == -1)
            {
                return false;
            }
            if (p.getTemporaryAttribute("teleporting") != null || p.getTemporaryAttribute("homeTeleporting") != null || p.getTemporaryAttribute("unmovable") != null || p.getTemporaryAttribute("cantDoAnything") != null)
            {
                return false;
            }
            if (p.getTemporaryAttribute("teleblocked") != null)
            {
                p.getPackets().sendMessage("A magical force prevents you from teleporting!");
                return false;
            }
            if (Location.inFightPits(p.getLocation()))
            {
                p.getPackets().sendMessage("You are unable to teleport from the fight pits.");
                return false;
            }
            if (Location.inFightCave(p.getLocation()))
            {
                FightCave.antiTeleportMessage(p);
                return false;
            }
            if (Location.inWilderness(p.getLocation()) && p.getLocation().wildernessLevel() >= 20)
            {
                p.getPackets().sendMessage("You cannot teleport above level 20 wilderness!");
                return false;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().getStatus() < 4)
                {
                    p.getDuel().declineDuel();
                }
                else if (p.getDuel().getStatus() == 8)
                {
                    if (p.getDuel().getWinner().Equals(p))
                    {
                        p.getDuel().recieveWinnings(p);
                    }
                }
            }
            int x = TELE_X[index] + Misc.random(TELE_EXTRA_X[index]);
            int y = TELE_Y[index] + Misc.random(TELE_EXTRA_Y[index]);
            p.getPackets().closeInterfaces();
            p.getPackets().sendBlankClientScript(1297);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            SkillHandler.resetAllSkills(p);
            if (p.getInventory().deleteItem(item, slot, 1))
            {
                p.setTemporaryAttribute("unmovable", true);
                p.setTemporaryAttribute("teleporting", true);
                p.setLastAnimation(new Animation(9597));
                p.setLastGraphics(new Graphics(1680, 0, 0));
                //p.setLastGraphics(new Graphics(678, 0, 0)); // blue gfx
                Event teleportEvent = new Event(900);
                int teleportCounter = 0;
                teleportEvent.setAction(() =>
                {
                    if (teleportCounter == 0)
                    {
                        p.setLastAnimation(new Animation(4071));
                        teleportCounter++;
                    }
                    else
                    {
                        p.setLastAnimation(new Animation(65535));
                        p.removeTemporaryAttribute("unmovable");
                        p.teleport(new Location(x, y, 0));
                        resetTeleport(p);
                        teleportEvent.stop();
                    }
                });
                Server.registerEvent(teleportEvent);
                return true;
            }
            return true;
        }

        public static void resetTeleport(Player p)
        {
            p.removeTemporaryAttribute("teleporting");
            p.removeTemporaryAttribute("homeTeleporting");
        }
    }
}