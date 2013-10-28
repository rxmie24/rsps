using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.magic;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RS2.Server.minigames.fightpits
{
    internal class FightPits
    {
        private List<Player> playersWaiting = new List<Player>();
        private List<Player> playersPlaying = new List<Player>();
        private bool gameInProgress = false, gameStarted = false;
        private Player lastWinner = null;
        private long gameStartedTime;
        private static int MAX_GAME_LENGTH = 9;
        private bool monstersSpawned = false;

        /**
         * Used for the reward
         */
        private int tokkulReward;
        private int totalPlayers;

        private static int[][] ORB_COORDINATES = {
		    new int[] {13, 14, 15, 12, 11}, // Viewing orb interface button id's
		    new int[] {2409, 2411, 2388, 2384, 2398}, // x
		    new int[] {5158, 5137, 5138, 5157, 5150} // y
		    // NE, SE, SW, NW, middle,
	    };

        public FightPits()
        {
            startWaitingEvent();
        }

        private void startWaitingEvent()
        {
            Event gameWaitingEvent = new Event(10000);
            gameWaitingEvent.setAction(() =>
            {
                if (!gameInProgress)
                {
                    if (playersWaiting.Count() >= 2 || (playersWaiting.Count() >= 1 && playersPlaying.Count() == 1))
                    {
                        startGame();
                        gameWaitingEvent.setTick(1000);
                    }
                }
                else
                {
                    if (playersPlaying.Count() <= 1)
                    {
                        gameInProgress = false;
                        gameWaitingEvent.setTick(40000);
                        setWinner();
                    }
                    else
                    {
                        if (Environment.TickCount - gameStartedTime >= (MAX_GAME_LENGTH * 60000))
                        {
                            spawnMonsters();
                        }
                    }
                }
            });
            Server.registerEvent(gameWaitingEvent);
        }

        protected void spawnMonsters()
        {
            if (monstersSpawned || !gameInProgress)
            {
                return;
            }
            monstersSpawned = true;
            Event spawnMonstersEvent = new Event(3000);
            spawnMonstersEvent.setAction(() =>
            {
                if (!monstersSpawned || playersPlaying.Count() <= 1)
                {
                    spawnMonstersEvent.stop();
                    foreach (Npc n in Server.getNpcList())
                    {
                        if (Location.inFightPits(n.getLocation()))
                        {
                            n.setHidden(true);
                            Server.getNpcList().Remove(n);
                        }
                    }
                    return;
                }
            });
            Server.registerEvent(spawnMonstersEvent);

            foreach (Player p in playersPlaying)
            {
                teleportToWaitingRoom(p, false);
                sendNPCMessage(p, "You took to long in defeating your enemies.");
            }
            playersPlaying.Clear();
        }

        protected void setWinner()
        {
            monstersSpawned = false;
            if (playersPlaying.Count == 0)
            {
                lastWinner = null;
                return;
            }
            Player p = playersPlaying[0];
            playersPlaying.Clear();
            if (p != null)
            {
                sendNPCMessage(p, "Congratulations! You are the champion!");
                p.getPackets().sendMessage("Congratulations, you are the champion!");
                p.setTzhaarSkull();
                playersPlaying.Add(p);
                displayFightPitsInterface(p); // update the name.
                foreach (Npc n in Server.getNpcList())
                {
                    if (n != null)
                    {
                        if (n.getId() == 2618)
                        {
                            if (lastWinner != null)
                            {
                                if (lastWinner.getLoginDetails().getUsername().Equals(p.getLoginDetails().getUsername()))
                                {
                                    n.setForceText(p.getLoginDetails().getUsername() + " retains champion status!");
                                }
                                else
                                {
                                    n.setForceText(p.getLoginDetails().getUsername() + " is the new champion!");
                                }
                            }
                            else
                            {
                                n.setForceText(p.getLoginDetails().getUsername() + " is the new champion!");
                            }
                            break;
                        }
                    }
                }
                lastWinner = p;
            }
            else
            {
                lastWinner = null;
            }
        }

        protected void startGame()
        {
            gameInProgress = true;
            gameStartedTime = Environment.TickCount;
            lock (playersWaiting)
            {
                playersPlaying.AddRange(playersWaiting);
            }
            playersWaiting.Clear();
            foreach (Player p in playersPlaying)
            {
                if (p == null || p.isDead() || p.isDestroyed() || p.getTeleportTo() != null || (lastWinner != null && p.Equals(lastWinner) && Location.inFightPits(p.getLocation())))
                {
                    continue;
                }
                p.teleport(getRandomTeleport());
                resetVariables(p);
                sendNPCMessage(p, "Wait for my signal before fighting.");
                tokkulReward += p.getSkills().getCombatLevel();
                totalPlayers++;
            }
            tokkulReward *= (int)Convert.ToDouble(totalPlayers * 2.40);
            Event gameStartEvent = new Event(10000);
            gameStartEvent.setAction(() =>
            {
                gameStartEvent.stop();
                if (playersPlaying.Count == 1)
                {
                    gameInProgress = false;
                    setWinner();
                    return;
                }
                foreach (Player p in playersPlaying)
                {
                    if (p == null || p.isDestroyed())
                    {
                        continue;
                    }
                    sendNPCMessage(p, "Fight!");
                }
                gameStarted = true;
            });
            Server.registerEvent(gameStartEvent);
        }

        public void useOrb(Player p, int button)
        {
            if (p.getTemporaryAttribute("teleporting") != null)
            {
                return;
            }
            if (button == -1)
            {
                AreaEvent useOrbAreaEvent = new AreaEvent(p, 2398, 5171, 2400, 5173);
                useOrbAreaEvent.setAction(() =>
                {
                    p.getPackets().displayInventoryInterface(374);
                    p.getAppearance().setInvisible(true);
                    p.getUpdateFlags().setAppearanceUpdateRequired(true);
                    p.setTemporaryAttribute("cantDoAnything", true);
                    p.setTemporaryAttribute("unmovable", true);
                    p.getPackets().setMinimapStatus(2);
                    Event useOrbEvent = new Event(500);
                    useOrbEvent.setAction(() =>
                    {
                        useOrbEvent.stop();
                        int random = Misc.random(4);
                        p.teleport(new Location(ORB_COORDINATES[1][random], ORB_COORDINATES[2][random], 0));
                    });
                    Server.registerEvent(useOrbEvent);
                });
                Server.registerCoordinateEvent(useOrbAreaEvent);
            }
            else
            {
                if (p.getTemporaryAttribute("cantDoAnything") != null)
                {
                    if (button == 5)
                    {
                        Event useOrbTwoEvent = new Event(500);
                        useOrbTwoEvent.setAction(() =>
                        {
                            useOrbTwoEvent.stop();
                            p.getAppearance().setInvisible(false);
                            p.getUpdateFlags().setAppearanceUpdateRequired(true);
                            p.removeTemporaryAttribute("cantDoAnything");
                            p.removeTemporaryAttribute("unmovable");
                            teleportToWaitingRoom(p, false);
                            p.getPackets().closeInterfaces();
                            p.getPackets().setMinimapStatus(0);
                            p.getPackets().clearMapFlag();
                        });
                        Server.registerEvent(useOrbTwoEvent);
                        return;
                    }
                    for (int i = 0; i < ORB_COORDINATES[0].Length; i++)
                    {
                        if (button == ORB_COORDINATES[0][i])
                        {
                            int j = i;
                            p.setTemporaryAttribute("teleporting", true);
                            Event teleportToOrbEvent = new Event(500);
                            teleportToOrbEvent.setAction(() =>
                            {
                                teleportToOrbEvent.stop();
                                p.teleport(new Location(ORB_COORDINATES[1][j], ORB_COORDINATES[2][j], 0));
                                Teleport.resetTeleport(p);
                            });
                            Server.registerEvent(teleportToOrbEvent);
                            return;
                        }
                    }
                }
            }
        }

        private void sendNPCMessage(Player p, string msg)
        {
            p.getPackets().sendNPCHead(2618, 241, 1);
            p.getPackets().modifyText("TzHaar-Mej-Kah", 241, 3);
            p.getPackets().modifyText(msg, 241, 4);
            p.getPackets().animateInterface(9827, 241, 1);
            p.getPackets().sendChatboxInterface2(241);
        }

        private void resetVariables(Player p)
        {
            p.setSkullCycles(0);
            p.getSpecialAttack().resetSpecial();
            p.setLastkiller(null);
            p.setDead(false);
            p.setEntityFocus(65535);
            p.setPoisonAmount(0);
            p.clearKillersHits();
            p.setLastVengeanceTime(0);
            p.setVengeance(false);
            p.removeTemporaryAttribute("willDie");
            p.setFrozen(false);
            p.removeTemporaryAttribute("unmovable");
            p.setAntifireCycles(0);
            p.setSuperAntipoisonCycles(0);
            p.setTeleblockTime(0);
            p.removeTemporaryAttribute("teleblocked");
            p.setTarget(null);
            p.setAttacker(null);
            foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                p.getSkills().setCurLevel(skill, p.getSkills().getMaxLevel(skill));
            p.getPackets().sendSkillLevels();
            Prayer.deactivateAllPrayers(p);
            if (p.getTemporaryAttribute("cantDoAnything") != null)
            {
                p.getAppearance().setInvisible(false);
                p.getUpdateFlags().setAppearanceUpdateRequired(true);
                p.removeTemporaryAttribute("cantDoAnything");
                p.removeTemporaryAttribute("unmovable");
                teleportToWaitingRoom(p, false);
                p.getPackets().closeInterfaces();
                p.getPackets().setMinimapStatus(0);
                p.getPackets().clearMapFlag();
            }
        }

        private Location getRandomTeleport()
        {
            switch (Misc.random(4))
            {
                case 0: return new Location(2384 + Misc.random(29), 5133 + Misc.random(4), 0);
                case 1: return new Location(2410 + Misc.random(4), 5140 + Misc.random(18), 0);
                case 2: return new Location(2392 + Misc.random(11), 5141 + Misc.random(26), 0);
                case 3: return new Location(2383 + Misc.random(3), 5141 + Misc.random(15), 0);
                case 4: return new Location(2392 + Misc.random(12), 5145 + Misc.random(20), 0);
            }
            return null;
        }

        public void displayFightPitsInterface(Player p)
        {
            if (p.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            p.getPackets().sendConfig(560, playersPlaying.Count - 1);
            p.getPackets().modifyText("Current champion: " + getLastWinnerName(), 373, 0);
            p.getPackets().sendOverlay(373);
        }

        public void teleportToWaitingRoom(Player p, bool removeFromList)
        {
            int x = 2395 + Misc.random(8);
            int y = 5170 + Misc.random(3);
            if (x == 2399 && y == 5172)
            { // On viewing orb
                x++; // Move to the side of it
            }
            p.teleport(new Location(x, y, 0));
            resetVariables(p);
            if (removeFromList)
            {
                playersPlaying.Remove(p);
                if (playersPlaying.Count >= 1)
                {
                    foreach (Player player in playersPlaying)
                    {
                        displayFightPitsInterface(player);
                    }
                }
            }
        }

        public void useDoor(Player p, int doorId)
        {
            bool running = p.getWalkingQueue().isRunToggled();
            if (doorId == 9369)
            { // main entrance
                int y = p.getLocation().getY() >= 5177 ? 5177 : 5175;
                CoordinateEvent mainEntranceCoordinateEvent = new CoordinateEvent(p, new Location(2399, y, 0));
                mainEntranceCoordinateEvent.setAction(() =>
                {
                    p.getWalkingQueue().setRunToggled(false);
                    p.getWalkingQueue().resetWalkingQueue();
                    p.getPackets().clearMapFlag();
                    p.setTemporaryAttribute("unmovable", true);
                    p.getWalkingQueue().forceWalk(0, y == 5177 ? -2 : +2);
                    Event useMainDoorEvent = new Event(1000);
                    useMainDoorEvent.setAction(() =>
                    {
                        useMainDoorEvent.stop();
                        p.removeTemporaryAttribute("unmovable");
                        p.getWalkingQueue().setRunToggled(running);
                    });
                    Server.registerEvent(useMainDoorEvent);
                });
                Server.registerCoordinateEvent(mainEntranceCoordinateEvent);
            }
            else if (doorId == 9368)
            { // game door
                int y = p.getLocation().getY() >= 5169 ? 5169 : 5167;
                CoordinateEvent gameDoorCoordinateEvent = new CoordinateEvent(p, new Location(2399, y, 0));
                gameDoorCoordinateEvent.setAction(() =>
                {
                    if (playersPlaying.Count == 1)
                    {
                        sendNPCMessage(p, "Here is some TokKul, as a reward.");
                        p.getInventory().addItemOrGround(6529, tokkulReward);
                    }
                    removePlayingPlayer(p);
                    p.getWalkingQueue().setRunToggled(false);
                    p.getWalkingQueue().resetWalkingQueue();
                    p.getPackets().clearMapFlag();
                    if (y == 5167)
                    {
                        p.getWalkingQueue().forceWalk(0, +2);
                    }
                    else
                    {
                        p.getPackets().sendMessage("You are unable to bypass the hot barrier.");
                        return;
                    }
                    p.setTemporaryAttribute("unmovable", true);
                    Event gameDoorEvent = new Event(1000);
                    gameDoorEvent.setAction(() =>
                    {
                        gameDoorEvent.stop();
                        p.removeTemporaryAttribute("unmovable");
                        p.getWalkingQueue().setRunToggled(running);
                        p.getPackets().sendMessage("You leave the fight pit.");
                        resetVariables(p);
                    });
                    Server.registerEvent(gameDoorEvent);
                });
                Server.registerCoordinateEvent(gameDoorCoordinateEvent);
            }
        }

        public bool hasGameStarted()
        {
            return gameStarted;
        }

        private string getLastWinnerName()
        {
            if (lastWinner == null)
            {
                return "-";
            }
            return lastWinner.getLoginDetails().getUsername();
        }

        public void addWaitingPlayer(Player p)
        {
            lock (playersWaiting)
            {
                playersWaiting.Add(p);
            }
        }

        public void removeWaitingPlayer(Player p)
        {
            lock (playersWaiting)
            {
                playersWaiting.Remove(p);
            }
        }

        public void addPlayingPlayer(Player p)
        {
            lock (playersPlaying)
            {
                playersPlaying.Add(p);
            }
        }

        public void removePlayingPlayer(Player p)
        {
            lock (playersPlaying)
            {
                playersPlaying.Remove(p);
            }
        }
    }
}