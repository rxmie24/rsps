using RS2.Server.clans;
using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.grandexchange;
using RS2.Server.minigames;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
 * Author:  Ramie Raufdeen
 * Purpose: Built an RS2 emulator in C# for the RSPS community, 
 *          will be releasing this on Rune-Server within the next 
 *          weeks for open source development
 *          The client is to be made and released by AJ on R-S
 */ 
namespace RS2.Server
{
    internal class Server
    {
        private static bool isRunning;                  // signals listener to shut down
        private static bool updateInProgress;           //triggers a system update.

        public static List<Event> events = new List<Event>(); //holds the events which are going to be proccessed.
        public static List<Event> eventsToAdd = new List<Event>(); //holds any pending incoming events.
        public static List<Event> eventsToRemove = new List<Event>(); //holds any pending events to remove.
        private static readonly object eventsLocker = new object();

        public static WorldObjectManager objectManager;
        public static GroundItemManager groundItemManager;
        public static MinigamesHandler minigames;
        public static GrandExchange grandExchange;
        public static ShopManager shopManager;
        public static ClanManager clanManager;
        public static PacketHandlers packetHandlers;
        public static LoginHandler loginHandler;
        public static List<Connection> connections = new List<Connection>(); //holds all connections.
        public static EntityList<Player> players = new EntityList<Player>(Constants.MAX_PLAYERS); //holds the players, connections which logged in successfully.
        public static EntityList<Npc> npcs = new EntityList<Npc>(Constants.MAX_NPCS); //holds the npcs, these are all spawned npcs in the server.
        public static Socket serverListenerSocket;      //main server socket used for listening, async operations, etc.
        public static readonly int serverStartupTime = Environment.TickCount; //only 29 days but meh, thats enough to know.
        public static int curTime;
        public static int lastInfoTime = 0;
        public static bool toggledStats = false;

        public static void Main(string[] args)
        {
            //MapData.MapList(); //this has to be packed once all mapdata is gotten.
            ObjectData.load();
            ItemData.load(); //this has to be first because npcDrops use itemData.. i think.
            NpcData.load(); //first load the npc data.
            NpcDrop.load(); //second load the npc drops. [order does matter here, as it binds to npcData].
            NpcSpawn.load(); //finally you can spawn the npcs.
            LaddersAndStairs.load();
            objectManager = new WorldObjectManager();
            groundItemManager = new GroundItemManager();
            shopManager = new ShopManager();
            minigames = new MinigamesHandler();
            grandExchange = new GrandExchange();
            clanManager = new ClanManager();
            packetHandlers = new PacketHandlers();
            loginHandler = new LoginHandler();

            registerEvent(new RunEnergyEvent());
            registerEvent(new LevelChangeEvent());
            registerEvent(new SpecialRestoreEvent());
            registerEvent(new SkullCycleEvent());
            registerEvent(new AreaVariables());
            registerEvent(new AggressiveNpcEvent());
            registerEvent(new LowerPotionCyclesEvent());
            objectManager.getFarmingPatches().processPatches();
            isRunning = true;
            new Thread(new ThreadStart(Server.eventProcessingThread)).Start();
            Console.Title = "RS2 530 C# Server";
            // setup the listener
            try
            {
                IPEndPoint ipe = new IPEndPoint(0, Constants.SERVER_PORT);
                serverListenerSocket = new System.Net.Sockets.Socket(ipe.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverListenerSocket.Bind(ipe);
                serverListenerSocket.Listen(25); //backlog
                serverListenerSocket.BeginAccept(new AsyncCallback(acceptCallback), serverListenerSocket);
                Console.WriteLine("RS2 530 C# server started on port " + Constants.SERVER_PORT);
            }
            catch (SocketException ioe)
            {
                Misc.WriteError("Error: Unable to startup listener on " + Constants.SERVER_PORT + " - port already in use?");
                Misc.WriteError(ioe.Message.ToString());
                isRunning = false;
            }

            while (isRunning)
            {
                // could do game updating stuff in here...
                // maybe do all the major stuff here in a big loop and just do the packet
                // sending/receiving in the client subthreads. The actual packet forming code
                // will reside within here and all created packets are then relayed by the subthreads.
                // This way we avoid all the sync'in issues
                // The rough outline could look like:
                // doPlayers()		// updates all player related stuff
                // doNpcs()		// all npc related stuff
                // doObjects()
                // doWhatever()
                curTime = Environment.TickCount;

                if (curTime - lastInfoTime >= 2000 && !toggledStats)
                {
                    Console.Title = "RS2 C# Server [Players: " + players.Count + "][Connections: " + connections.Count + "]";
                    lastInfoTime = curTime;
                    toggledStats = true;
                }
                else if (curTime - lastInfoTime >= 4000 && toggledStats)
                {
                    Console.Title = "RS2 C# Server [Events Running: " + events.Count + "]";
                    lastInfoTime = curTime;
                    toggledStats = false;
                }

                lock (connections)
                {
                    foreach (Connection c in connections.ToArray()) //these are logged in players.
                        //ThreadPool.QueueUserWorkItem(c.processQueuedPackets);
                        c.processQueuedPackets(null);

                    foreach (Connection c in connections.ToArray()) //update server.
                    {
                        if (LoginHandler.removableConnection(c))
                            removeConnection(c);
                    }
                }

                lock (players)
                {
                    foreach (Player p in players)
                    {
                        p.tick();
                        p.processQueuedHits();
                        //if (p.getWalkingQueue().hasNextStep() || p.getTeleportTo() != null)
                        p.getWalkingQueue().getNextPlayerMovement();
                    }
                }

                lock (npcs)
                {
                    foreach (Npc n in npcs)
                    {
                        n.tick();
                        n.processQueuedHits();
                    }
                }

                lock (players)
                {
                    foreach (Player p in players)
                    {
                        if (p == null)
                            continue;
                        if (p.isActive())
                        {
                            PlayerUpdate.update(p);
                            NpcUpdate.update(p);

                            //In case the player turns active in the loop below make sure it doesn't clear flags.
                            if (!p.getUpdateFlags().isClearable()) p.getUpdateFlags().setClearable(true);
                        }
                    }
                    foreach (Player p in players)
                    {
                        if (p.isActive() && p.getUpdateFlags().isClearable() && p.getUpdateFlags().isUpdateRequired())
                            p.getUpdateFlags().clear();
                        p.getHits().clear();
                        if (!p.getConnection().socket.Connected || p.isDisconnected())
                            unregister(p); //This must be after PlayerUpdate or chance of messing up playerIndexes for PlayerUpdate
                    }
                }
                lock (npcs)
                {
                    foreach (Npc n in npcs)
                    {
                        if (n.getUpdateFlags().isUpdateRequired())
                            n.getUpdateFlags().clear();
                        n.getHits().clear();
                    }
                }

                try { System.Threading.Thread.Sleep(500 - (Environment.TickCount - curTime)); }
                catch { }
            }

            Console.WriteLine("Server has shutdown successfully, press any key to exit console.");
            //Console.ReadKey(true);
            Console.ReadLine();
        }

        public static void eventProcessingThread()
        {
            /**
            * Processes any pending events.
            */
            while (Server.isRunning)
            {
                lock (eventsLocker)
                {
                    foreach (Event e in eventsToAdd)
                        events.Add(e);
                    eventsToAdd.Clear();
                    foreach (Event e in events)
                    {
                        if (e.isStopped())
                            eventsToRemove.Add(e);
                        else if (e.isReady())
                            e.run();
                    }
                    foreach (Event e in eventsToRemove)
                        events.Remove(e);
                    eventsToRemove.Clear();
                }
                try { System.Threading.Thread.Sleep(10); }
                catch { }
            }
            /* End of Events*/
        }

        public static void removeConnection(Connection connection)
        {
            if (connection == null) return;
            Player player = connection.getPlayer();
            if (player != null)
                unregister(player); //if connection has a player by some luck.. unregister him as well.
            if (connection.socket.Connected)
            {
                connection.socket.Shutdown(SocketShutdown.Both);
                connection.socket.Close();
            }
            connection = null;
        }

        public static Player getPlayerForName(string name)
        {
            foreach (Player p in players)
            {
                if (p != null && p.getLoginDetails().getUsername().Equals(name))
                {
                    return p;
                }
            }
            return null;
        }

        /**
         * Gets the item manager.
         * @return
        */

        public static GroundItemManager getGroundItems()
        {
            return groundItemManager;
        }

        /**
         * Gets the object manager.
         * @return
         */

        public static WorldObjectManager getGlobalObjects()
        {
            return objectManager;
        }

        public static GrandExchange getGrandExchange()
        {
            return grandExchange;
        }

        public static ShopManager getShopManager()
        {
            return shopManager;
        }

        public static MinigamesHandler getMinigames()
        {
            return minigames;
        }

        public static ClanManager getClanManager()
        {
            return clanManager;
        }

        /**
         * Gets the players list.
         * @return
         */

        public static EntityList<Player> getPlayerList()
        {
            return players;
        }

        /**
         * Gets the npcs list.
         * @return
         */

        public static EntityList<Npc> getNpcList()
        {
            return npcs;
        }

        public static void setUpdateInProgress(bool updateInProgress)
        {
            Server.updateInProgress = updateInProgress;
        }

        public static bool isUpdateInProgress()
        {
            return updateInProgress;
        }

        /**
         * Register a player.
         * @param p as Player
         * @return the player slot
         */

        public static int register(Connection connection)
        {
            if (connection == null)
                return -1;
            Player player = connection.getPlayer();
            if (player == null)
                return -1;
            if (player.getLoginDetails() == null)
                return -1;

            players.Add(player);
            int slot = players.IndexOf(player);
            if (slot != -1)
            {
                Console.WriteLine("Registered " + player.getLoginDetails().getUsername() + " [pid = " + slot + ", online = " + players.Count + "]");
            }
            else
            {
                Console.WriteLine("Could not register " + player.getLoginDetails().getUsername() + " - too many online [online = " + players.Count + "]");
            }
            return slot;
        }

        /**
         * Unregister a player.
         * @param p as Player
         */

        public static void unregister(Player p)
        {
            if (p.getTrade() != null)
            {
                p.getTrade().decline();
                p.setTrade(null);
            }
            minigames.getFightPits().removeWaitingPlayer(p);
            minigames.getFightPits().removePlayingPlayer(p);
            if (p.getTemporaryAttribute("cantDoAnything") != null && Location.inFightPits(p.getLocation()))
            {
                minigames.getFightPits().useOrb(p, 5);
                return;
            }
            if (Location.inFightPits(p.getLocation()))
            {
                Server.getMinigames().getFightPits().teleportToWaitingRoom(p, false);
                return;
            }
            if (p.getFightCave() != null)
            {
                if (!p.getFightCave().isGamePaused())
                {
                    p.getFightCave().teleFromCave(true);
                    return;
                }
                else
                {
                    p.setLocation(new Location(2439, 5169, 0));
                }
            }
            if (!Combat.isXSecondsSinceCombat(p, p.getLastAttacked(), 10000) || p.isDead() || p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            if (p.getDuel() != null)
            {
                if (p.getDuel().getStatus() == 8)
                {
                    if (p.getDuel().getWinner().Equals(p))
                    {
                        p.getDuel().recieveWinnings(p);
                    }
                }
                else
                {
                    if (p.getDuel().getStatus() == 5 || p.getDuel().getStatus() == 6)
                    {
                        p.getDuel().finishDuel(true, true);
                    }
                }
            }
            removeAllPlayersNPCs(p);

            foreach (Player p2 in players)
            {
                if (p == p2) continue;
                //Remove me from anyone who has Seen me.. or attempting to see me (new players).
                if (p2.getLocalEnvironment().getSeenPlayers().Contains(p) || p2.getLocalEnvironment().getNewPlayers().Contains(p))
                    p2.getLocalEnvironment().getRemovedPlayers().Add(p);
            }

            clanManager.leaveChannel(p);
            loginHandler.addSavePlayer(p);
            players.Remove(p);
            p.getFriends().unregistered();
            Console.WriteLine("Unregistered " + p.getLoginDetails().getUsername() + " [online = " + players.Count + "]");
        }

        /**
         * Will remove all NPCs which are spawned specifically for this player.
         */

        public static void removeAllPlayersNPCs(Player p)
        {
            foreach (Npc n in npcs)
            {
                if (n != null)
                {
                    if (n.getOwner() != null)
                    {
                        if (n.getOwner().Equals(p))
                        {
                            n.setHidden(true);
                            npcs.Remove(n);
                        }
                    }
                }
            }
        }

        /**
         * Registers an event.
         * @param event
         */

        public static void registerEvent(Event theEvent)
        {
            lock (eventsLocker)
            {
                eventsToAdd.Add(theEvent);
            }
        }

        /**
	     * Registers a 'coordiante' event.
	     * @param event
	     */

        public static void registerCoordinateEvent(CoordinateEvent coordinateEvent)
        {
            Event newCoordinateEvent = new Event(0);
            newCoordinateEvent.setAction(() =>
            {
                bool standingStill = coordinateEvent.getPlayer().getSprites().getPrimarySprite() == -1 && coordinateEvent.getPlayer().getSprites().getSecondarySprite() == -1;
                if (coordinateEvent.getPlayer().getDistanceEvent() == null || !coordinateEvent.getPlayer().getDistanceEvent().Equals(coordinateEvent))
                {
                    newCoordinateEvent.stop();
                    return;
                }
                if (standingStill)
                {
                    if ((coordinateEvent.getPlayer().getLocation().Equals(coordinateEvent.getTargetLocation()) && coordinateEvent.getPlayer().getLocation().Equals(coordinateEvent.getOldLocation())) || coordinateEvent.getFailedAttempts() >= 15)
                    {
                        if (newCoordinateEvent.getTick() == 0)
                        {
                            coordinateEvent.run();
                            newCoordinateEvent.stop();
                            coordinateEvent.setPlayerNull();
                        }
                        else
                        {
                            if (!coordinateEvent.hasReached())
                            {
                                coordinateEvent.setReached(true);
                            }
                            else
                            {
                                coordinateEvent.run();
                                newCoordinateEvent.stop();
                                coordinateEvent.setPlayerNull();
                            }
                        }
                    }
                }
                else
                {
                    if (!coordinateEvent.getPlayer().getLocation().Equals(coordinateEvent.getOldLocation()))
                    {
                        coordinateEvent.setOldLocation(coordinateEvent.getPlayer().getLocation());
                    }
                    else
                    {
                        coordinateEvent.incrementFailedAttempts();
                    }
                }
                newCoordinateEvent.setTick(200);
            });
            registerEvent(newCoordinateEvent);
        }

        public static void registerCoordinateEvent(AreaEvent areaEvent)
        {
            Event newCoordinateEvent = new Event(0);
            newCoordinateEvent.setAction(() =>
            {
                bool standingStill = areaEvent.getPlayer().getSprites().getPrimarySprite() == -1 && areaEvent.getPlayer().getSprites().getSecondarySprite() == -1;
                if (areaEvent.getPlayer().getDistanceEvent() == null || !areaEvent.getPlayer().getDistanceEvent().Equals(areaEvent))
                {
                    newCoordinateEvent.stop();
                    return;
                }
                if (standingStill)
                {
                    if (areaEvent.inArea())
                    {
                        areaEvent.run();
                        newCoordinateEvent.stop();
                        areaEvent.setDistanceEventNull();
                        return;
                    }
                }
                newCoordinateEvent.setTick(500);
            });
            registerEvent(newCoordinateEvent);
        }

        private static void acceptCallback(IAsyncResult result)
        {
            Connection connection = new Connection();
            try
            {
                //Finish accepting the connection
                Socket s = (Socket)result.AsyncState;
                connection = new Connection();
                connection.socket = s.EndAccept(result);
                connection.buffer = new byte[5000];
                lock (connections)
                {
                    connections.Add(connection);
                }
                //Queue recieving of data from the connection
                connection.socket.BeginReceive(connection.buffer, 0, connection.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), connection);
                //Queue the accept of the next incomming connection
                serverListenerSocket.BeginAccept(new AsyncCallback(acceptCallback), serverListenerSocket);
            }
            catch (SocketException)
            {
                if (connection.socket != null)
                {
                    connection.socket.Close();
                    lock (connections)
                    {
                        connections.Remove(connection);
                    }
                }
                //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                serverListenerSocket.BeginAccept(new AsyncCallback(acceptCallback), serverListenerSocket);
            }
            catch (Exception)
            {
                if (connection.socket != null)
                {
                    connection.socket.Close();
                    lock (connections)
                    {
                        connections.Remove(connection);
                    }
                }
                //Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
                serverListenerSocket.BeginAccept(new AsyncCallback(acceptCallback), serverListenerSocket);
            }
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            //get our connection from the callback
            Connection connection = (Connection)result.AsyncState;
            //catch any errors, we'd better not have any
            try
            {
                if (connection == null || connection.socket == null) return;

                //Grab our buffer and count the number of bytes receives
                int bytesRead = connection.socket.EndReceive(result);
                //make sure we've read something, if we haven't it supposadly means that the client disconnected
                if (bytesRead > 0)
                {
                    connection.appendChuckedPackets(bytesRead); //appends current buffer data end of raw packets.

                    if (connection.loginStage == 6)
                    { //encrypted packets time
                        connection.packetDecoder();
                    }
                    else
                    {
                        loginHandler.doLogin(connection);
                    }

                    //Queue the next receive
                    connection.socket.BeginReceive(connection.buffer, 0, connection.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), connection);
                }
                else
                {
                    //Callback run but no data, close the connection
                    //supposadly means a disconnect
                    //and we still have to close the socket, even though we throw the event later
                    connection.socket.Close();
                    lock (connections)
                    {
                        connections.Remove(connection);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SocketException || e is ObjectDisposedException)
                {
                    //Something went terribly wrong
                    //which shouldn't have happened
                    if (connection.socket != null)
                    {
                        connection.socket.Close();
                        lock (connections)
                        {
                            connections.Remove(connection);
                        }
                    }
                }
                else
                {
                    Misc.WriteError(e.Message);
                }
            }
        }
    }
}