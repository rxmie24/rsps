using RS2.Server.minigames.agilityarena;
using RS2.Server.minigames.barrows;
using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.events
{
    internal class AreaVariables : Event
    {
        public AreaVariables()
            : base(500) { }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    updateVariables(p);
                }
            }
        }

        /*
         * NOTE: Anything that goes in here and varies between HD and LD,
         * reset the variable in ActionSender.configureGameScreen
         */

        public void updateVariables(Player p)
        {
            int currentLevel = p.getLocation().wildernessLevel();
            if (currentLevel != -1)
            { //Is in wilderness.
                int lastWildLevel = (p.getTemporaryAttribute("wildLvl") == null) ? -1 : (int)p.getTemporaryAttribute("wildLvl");

                if (currentLevel != lastWildLevel)
                {
                    if (currentLevel > 0)
                    {
                        p.setTemporaryAttribute("wildLvl", currentLevel);
                        if (p.getTemporaryAttribute("inWild") == null)
                        {
                            p.getPackets().sendPlayerOption("Attack", 1, 1);
                            p.getPackets().sendOverlay(381);
                            p.setTemporaryAttribute("inWild", true);
                        }
                    }
                    else
                    {
                        if (p.getTemporaryAttribute("inWild") != null)
                        {
                            p.getPackets().sendPlayerOption("null", 1, 1);
                            p.getPackets().sendRemoveOverlay();
                            p.removeTemporaryAttribute("wildLvl");
                            p.removeTemporaryAttribute("inWild");
                        }
                    }
                }
            }
            if (Location.inMultiCombat(p.getLocation()))
            {
                if (p.getTemporaryAttribute("inMulti") == null)
                {
                    p.getPackets().displayMultiIcon();
                    p.setTemporaryAttribute("inMulti", true);
                }
            }
            else
            {
                if (p.getTemporaryAttribute("inMulti") != null)
                {
                    p.getPackets().removeMultiIcon();
                    p.removeTemporaryAttribute("inMulti");
                }
            }
            if (Location.atDuelArena(p.getLocation()))
            {
                if (p.getDuel() != null)
                {
                    if (p.getDuel().getStatus() == 5 || p.getDuel().getStatus() == 6)
                    {
                        p.getPackets().sendPlayerOption("Fight", 1, 1);
                    }
                }
                if (p.getTemporaryAttribute("challengeUpdate") != null)
                {
                    p.getPackets().sendPlayerOption("Challenge", 1, 0);
                    p.removeTemporaryAttribute("challengeUpdate");
                }
                if (p.getTemporaryAttribute("atDuelArea") == null)
                {
                    p.getPackets().sendPlayerOption("Challenge", 1, 0);
                    p.getPackets().sendOverlay(638);
                    p.setTemporaryAttribute("atDuelArea", true);
                }
            }
            else
            {
                if (p.getTemporaryAttribute("atDuelArea") != null)
                {
                    p.getPackets().sendPlayerOption("null", 1, 0);
                    p.getPackets().sendRemoveOverlay();
                    p.removeTemporaryAttribute("atDuelArea");
                }
            }
            if (Location.atBarrows(p.getLocation()))
            {
                if (p.getTemporaryAttribute("atBarrows") == null)
                {
                    p.getPackets().sendOverlay(24);
                    p.getPackets().setMinimapStatus(2);
                    p.getPackets().sendConfig(452, 2652256); // doors
                    if (p.getTemporaryAttribute("betweenDoors") == null)
                    {
                        if (Barrows.betweenDoors(p))
                        {
                            p.setTemporaryAttribute("betweenDoors", true);
                            p.getPackets().sendConfig(1270, 1);
                        }
                    }
                    p.getPackets().modifyText("Kill Count: " + p.getBarrowKillCount(), 24, 0);
                    p.setTemporaryAttribute("atBarrows", true);
                    Barrows.prayerDrainEvent(p);
                    bool allBrothersKilled = true;
                    for (int i = 0; i < 6; i++)
                    {
                        if (!p.getBarrowBrothersKilled(i))
                        {
                            allBrothersKilled = false;
                        }
                    }
                    if (allBrothersKilled)
                    {
                        Barrows.startEarthQuake(p);
                    }
                }
            }
            else
            {
                if (p.getTemporaryAttribute("atBarrows") != null)
                {
                    bool allBrothersKilled = true;
                    for (int i = 0; i < 6; i++)
                    {
                        if (!p.getBarrowBrothersKilled(i))
                        {
                            allBrothersKilled = false;
                        }
                    }
                    if (allBrothersKilled)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            p.setBarrowBrothersKilled(i, false);

                            p.setBarrowTunnel(-1);
                            p.setBarrowKillCount(0);
                            p.getPackets().resetCamera();
                            p.removeTemporaryAttribute("lootedBarrowChest");
                        }
                        p.getPackets().resetCamera();
                        p.getPackets().sendRemoveOverlay();
                        p.removeTemporaryAttribute("atBarrows");
                        p.removeTemporaryAttribute("barrowTunnel");
                        p.getPackets().setMinimapStatus(0);
                        Barrows.removeBrotherFromGame(p);
                    }
                }
                if (Location.atGodwars(p.getLocation()))
                {
                    if (p.getTemporaryAttribute("atGodwars") == null)
                    {
                        p.getPackets().sendOverlay(601);
                        p.setTemporaryAttribute("atGodwars", true);
                    }
                }
                else
                {
                    if (p.getTemporaryAttribute("atGodwars") != null)
                    {
                        p.getPackets().sendRemoveOverlay();
                        p.removeTemporaryAttribute("atGodwars");
                    }
                }
                if (Location.atAgilityArena(p.getLocation()))
                {
                    if (p.getTemporaryAttribute("atAgilityArena") == null)
                    {
                        p.getPackets().sendOverlay(5);
                        AgilityArena.updatePillarForPlayer(p);
                        p.setTemporaryAttribute("atAgilityArena", true);
                    }
                    if (p.getLocation().getZ() == 0)
                    {
                        p.removeTemporaryAttribute("atAgilityArena");
                        p.getPackets().sendRemoveOverlay();
                        p.setAgilityArenaStatus(0);
                        p.setTaggedLastAgilityPillar(false);
                    }
                }
                else
                {
                    if (p.getTemporaryAttribute("atAgilityArena") != null)
                    {
                        p.getPackets().sendRemoveOverlay();
                        p.setAgilityArenaStatus(0);
                        p.setTaggedLastAgilityPillar(false);
                        p.removeTemporaryAttribute("atAgilityArena");
                    }
                }
                /*
                 * We check the cantDoAnything variable to determine whether they're using the orb.
                 */
                if (Location.inFightPitsWaitingArea(p.getLocation()))
                {
                    if (p.getTemporaryAttribute("waitingForFightPits") == null)
                    {
                        Server.getMinigames().getFightPits().addWaitingPlayer(p);
                        p.setTemporaryAttribute("waitingForFightPits", true);
                    }
                }
                else
                {
                    if (p.getTemporaryAttribute("waitingForFightPits") != null && p.getTemporaryAttribute("cantDoAnything") == null)
                    {
                        Server.getMinigames().getFightPits().removeWaitingPlayer(p);
                        p.removeTemporaryAttribute("waitingForFightPits");
                    }
                }
                if (Location.inFightPits(p.getLocation()))
                {
                    if (p.getTemporaryAttribute("cantDoAnything") == null)
                    {
                        if (p.getTemporaryAttribute("inFightPits") == null)
                        {
                            p.getPackets().sendPlayerOption("Attack", 1, 1);
                            Server.getMinigames().getFightPits().displayFightPitsInterface(p);
                            p.setTemporaryAttribute("inFightPits", true);
                        }
                    }
                }
                else
                {
                    if (p.getTemporaryAttribute("inFightPits") != null)
                    {
                        p.getPackets().sendPlayerOption("null", 1, 1);
                        p.getPackets().sendRemoveOverlay();
                        p.removeTemporaryAttribute("inFightPits");
                    }
                }
                if (Location.onWaterbirthIsle(p.getLocation()))
                {
                    if (p.getTemporaryAttribute("snowInterface") == null)
                    {
                        p.getPackets().sendOverlay(370);
                        p.setTemporaryAttribute("snowInterface", true);
                    }
                }
                else
                {
                    if (p.getTemporaryAttribute("snowInterface") != null)
                    {
                        p.getPackets().sendRemoveOverlay();
                        p.removeTemporaryAttribute("snowInterface");
                    }
                }
            }
        }
    }
}