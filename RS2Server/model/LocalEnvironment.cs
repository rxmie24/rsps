using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;

namespace RS2.Server.model
{
    internal class LocalEnvironment
    {
        private Player player;
        private List<Player> newPlayers = new List<Player>(); //first here.. (draw new player packet to you)
        private List<Player> seenPlayers = new List<Player>(); //second here. (possible update packet to you)
        private HashSet<Player> removedPlayers = new HashSet<Player>(); //third here. (remove player packet to you)

        private List<Npc> newNpcs = new List<Npc>(); //first here.. (draw new npc packet to you)
        private List<Npc> seenNpcs = new List<Npc>(); //second here. (possible update packet to you)
        private HashSet<Npc> removedNpcs = new HashSet<Npc>(); //third here. (remove npc packet to you)

        public LocalEnvironment(Player player)
        {
            this.player = player;
        }

        public void updatePlayersInArea() //before update sent to all players.
        {
            /*
             * Checks all players on server location with yours.
             * If one of the players isn't in your seenPlayers list and is around you in distance.
             * Then make sure you see him after this.
             */
            foreach (Player p in Server.getPlayerList())
            {
                if (p == null) Misc.WriteError("A null player refernece in playerList? is that possible?");
                if (p == null || p == player || !p.isActive() || p.isDisconnected() || p.isDestroyed() || p.getUpdateFlags().isTeleporting())
                    continue;
                if (!seenPlayers.Contains(p) && player.getLocation().withinDistance(p.getLocation()))
                    newPlayers.Add(p);
            }
        }

        public void organizePlayers() //after update sent to all players.
        {
            //Remove removedPlayers from seenPlayers.
            seenPlayers.RemoveAll(new Predicate<Player>(delegate(Player player) { return removedPlayers.Contains(player); }));
            //Clear removed players.
            removedPlayers.Clear();
            //Add new players to seen Players.
            seenPlayers.AddRange(newPlayers);
            //Clear new players (These new players were already added to seenPlayers).
            newPlayers.Clear();

            //Populate a new list of players that are possible candidates for removing next time around.
            foreach (Player p in seenPlayers)
            {
                if (p == null) Misc.WriteError("A null in seenPlayers? is that possible?");
                if (!player.getLocation().withinDistance(p.getLocation()) || p.getUpdateFlags().isTeleporting() || p.isDisconnected() || p.isDestroyed())
                    removedPlayers.Add(p);
            }
        }

        public void updateNpcsInArea() //before update sent to all players.
        {
            /*
             * Checks all players on server location with yours.
             * If one of the players isn't in your seenPlayers list and is around you in distance.
             * Then make sure you see him after this.
             */
            foreach (Npc n in Server.getNpcList())
            {
                if (n == null) Misc.WriteError("A null npc refernece in npcList? is that possible?");

                if (n == null || n.isHidden())
                    continue;

                if (!seenNpcs.Contains(n) && player.getLocation().withinDistance(n.getLocation()))
                    newNpcs.Add(n);
            }
        }

        public void organizeNpcs() //after update sent to all players.
        {
            //Remove removedNpcs from seenNpcs.
            seenNpcs.RemoveAll(new Predicate<Npc>(delegate(Npc npc) { return removedNpcs.Contains(npc); }));
            //Clear removed npcs.
            removedNpcs.Clear();
            //Add new npcs to seen npcs.
            seenNpcs.AddRange(newNpcs);
            //Clear new players (These new players were already added to seenPlayers).
            newNpcs.Clear();

            //Populate a new list of players that are possible candidates for removing next time around.
            foreach (Npc n in seenNpcs)
            {
                if (n == null) Misc.WriteError("A null in seenNpcs? is that possible?");
                if (!player.getLocation().withinDistance(n.getLocation()) || n.isHidden())
                    removedNpcs.Add(n);
            }
        }

        public List<Player> getNewPlayers()
        {
            return newPlayers;
        }

        public List<Player> getSeenPlayers()
        {
            return seenPlayers;
        }

        public HashSet<Player> getRemovedPlayers()
        {
            return removedPlayers;
        }

        public List<Npc> getNewNpcs()
        {
            return newNpcs;
        }

        public List<Npc> getSeenNpcs()
        {
            return seenNpcs;
        }

        public HashSet<Npc> getRemovedNpcs()
        {
            return removedNpcs;
        }
    }
}