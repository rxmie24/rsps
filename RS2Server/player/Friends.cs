using RS2.Server.clans;
using RS2.Server.util;
using System.Collections.Generic;

namespace RS2.Server.player
{
    internal class Friends
    {
        private Player player;
        private List<long> friends;
        private List<long> ignores;
        private Dictionary<long, int> clanRanks;

        public enum STATUS
        {
            ON = 0,
            FRIENDS = 1,
            OFF = 2
        };

        private STATUS publicStatus = STATUS.ON;
        private STATUS privateStatus = STATUS.ON;
        private STATUS tradeStatus = STATUS.ON;

        private int messageCounter = 0;

        public Friends(Player player)
        {
            this.player = player;
            messageCounter = 1;
            friends = new List<long>(200);
            ignores = new List<long>(100);
            clanRanks = new Dictionary<long, int>(100);
            publicStatus = privateStatus = tradeStatus = 0;
        }

        public int getNextUniqueId()
        {
            if (messageCounter >= 16000000)
            {
                messageCounter = 0;
            }
            return messageCounter++;
        }

        public void refresh()
        {
            player.getPackets().setPrivacyOptions();
            player.getPackets().setFriendsListStatus();
            foreach (long friend in friends)
            {
                player.getPackets().sendFriend(friend, getWorld(friend));
            }
            long[] array = new long[ignores.Count];
            int i = 0;
            foreach (long ignore in ignores)
            {
                array[i++] = ignore;
            }
            player.getPackets().sendIgnores(array);
        }

        public int getWorld(long friend)
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.getLoginDetails().getLongName() == friend)
                    {
                        return p.getWorld();
                    }
                }
            }
            return 0;
        }

        public void addFriend(long name)
        {
            Player friend = null;
            if (friends.Count >= 200)
            {
                player.getPackets().sendMessage("Your friends list is full.");
                return;
            }
            if (friends.Contains((long)name))
            {
                player.getPackets().sendMessage(Misc.formatPlayerNameForDisplay(Misc.longToPlayerName(name)) + " is already on your friends list.");
                return;
            }
            friends.Add((long)name);
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.getLoginDetails().getLongName() == name)
                    {
                        friend = p;
                    }
                }
            }
            if (friend != null)
            {
                if (privateStatus != STATUS.OFF)
                {
                    friend.getFriends().registered(player);
                }
                Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                if (clan != null)
                {
                    clan.getOwnerFriends().Add(name);
                    if (clan.getUser(friend) != null)
                    {
                        if (clan.getUser(friend).getClanRights() == Clan.ClanRank.NO_RANK)
                        {
                            clan.getUser(friend).setClanRights(Clan.ClanRank.FRIEND);
                            Server.getClanManager().updateClan(clan);
                        }
                    }
                }
                if (friend.getFriends().getPrivateStatus() == STATUS.OFF || (friend.getFriends().getPrivateStatus() == STATUS.FRIENDS && !friend.getFriends().isFriend(player)))
                {
                    return;
                }
                player.getPackets().sendFriend(name, getWorld(name));
            }
        }

        private bool isFriend(Player player)
        {
            long n = player.getLoginDetails().getLongName();
            if (friends.Contains(n))
            {
                return true;
            }
            return false;
        }

        private STATUS getPrivateStatus()
        {
            return privateStatus;
        }

        public void addIgnore(long name)
        {
            if (ignores.Count >= 100)
            {
                player.getPackets().sendMessage("Your ignore list is full.");
                return;
            }
            if (ignores.Contains((long)name))
            {
                player.getPackets().sendMessage(Misc.formatPlayerNameForDisplay(Misc.longToPlayerName(name)) + " is already on your ignore list.");
                return;
            }
            ignores.Add((long)name);
        }

        public void removeFriend(long name)
        {
            Player friend = null;
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.getLoginDetails().getLongName() == name)
                    {
                        friend = p;
                    }
                }
            }
            if (friend == null)
            {
                return;
            }
            if (privateStatus == STATUS.FRIENDS)
            {
                friend.getFriends().unregistered(player);
            }
            Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
            if (clan != null)
            {
                clan.getOwnerFriends().Remove(name);
                if (clan.getUser(friend) != null)
                {
                    if (clan.getUser(friend).getClanRights() == Clan.ClanRank.FRIEND)
                    {
                        clan.getUser(friend).setClanRights(Clan.ClanRank.NO_RANK);
                        Server.getClanManager().updateClan(clan);
                    }
                }
            }
            friends.Remove((long)name);
        }

        public void removeIgnore(long name)
        {
            ignores.Remove((long)name);
        }

        public List<long> getFriendsList()
        {
            return friends;
        }

        public List<long> getIgnoresList()
        {
            return ignores;
        }

        public void registered()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    p.getFriends().registered(player);
                }
            }
        }

        private void registered(Player p)
        {
            long n = p.getLoginDetails().getLongName();
            if (friends.Contains(n))
            {
                player.getPackets().sendFriend(n, getWorld(n));
            }
        }

        public void unregistered()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    p.getFriends().unregistered(player);
                }
            }
        }

        private void unregistered(Player p)
        {
            long n = p.getLoginDetails().getLongName();
            if (friends.Contains(n))
            {
                player.getPackets().sendFriend(n, 0);
            }
        }

        public void sendMessage(long name, string text, byte[] packed)
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null && !p.Equals(player))
                {
                    if (p.getLoginDetails().getLongName() == name)
                    {
                        if (privateStatus == STATUS.OFF)
                        {
                            privateStatus = STATUS.FRIENDS;
                            setPrivacyOption(publicStatus, privateStatus, tradeStatus);
                        }
                        p.getPackets().sendReceivedPrivateMessage(player.getLoginDetails().getLongName(), player.getRights(), text, packed);
                        player.getPackets().sendSentPrivateMessage(name, text);
                        return;
                    }
                }
            }
            player.getPackets().sendMessage(Misc.formatPlayerNameForDisplay(Misc.longToPlayerName(name)) + " is currently unavailable.");
        }

        public void login()
        {
            if (privateStatus == STATUS.OFF)
            {
                return;
            }
            else if (privateStatus == STATUS.FRIENDS)
            {
                foreach (Player p in Server.getPlayerList())
                {
                    if (p != null)
                    {
                        if (friends.Contains(p.getLoginDetails().getLongName()))
                        {
                            p.getFriends().registered(player);
                        }
                    }
                }
            }
            else if (privateStatus == STATUS.ON)
            {
                registered();
            }
        }

        public void setPrivacyOptions(STATUS pub, STATUS priv, STATUS trade)
        {
            this.publicStatus = pub;
            this.privateStatus = priv;
            this.tradeStatus = trade;
        }

        public void setPrivacyOption(STATUS pub, STATUS priv, STATUS trade)
        {
            publicStatus = pub;
            tradeStatus = trade;
            if (priv != privateStatus)
            {
                if (priv == STATUS.ON)
                {
                    registered();
                }
                else if (priv == STATUS.OFF)
                {
                    unregistered();
                }
                else if (priv == STATUS.FRIENDS)
                {
                    if (privateStatus == STATUS.ON)
                    {
                        foreach (Player p in Server.getPlayerList())
                        {
                            if (p != null)
                            {
                                if (p.getFriends().getFriendsList().Contains(player.getLoginDetails().getLongName()))
                                {
                                    if (!friends.Contains(p.getLoginDetails().getLongName()))
                                    {
                                        p.getFriends().unregistered(player);
                                    }
                                }
                            }
                        }
                    }
                    else if (privateStatus == STATUS.OFF)
                    {
                        foreach (Player p in Server.getPlayerList())
                        {
                            if (p != null)
                            {
                                if (friends.Contains(p.getLoginDetails().getLongName()))
                                {
                                    p.getFriends().registered(player);
                                }
                            }
                        }
                    }
                }
                privateStatus = priv;
            }
            player.getPackets().setPrivacyOptions();
        }

        public STATUS getPrivacyOption(int option)
        {
            switch (option)
            {
                case 0: return publicStatus;
                case 1: return privateStatus;
                case 2: return tradeStatus;
            }
            return publicStatus;
        }
    }
}