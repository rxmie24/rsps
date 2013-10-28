using RS2.Server.player;
using RS2.Server.util;
using System.Collections.Generic;

namespace RS2.Server.clans
{
    internal class Clan
    {
        private string name;
        private string owner;
        private ClanRank enterRights;
        private ClanRank talkRights;
        private ClanRank kickRights;
        private ClanRank lootRights;
        private List<ClanUser> users;
        private List<long> ownerFriends;
        private Dictionary<string, ClanRank> usersWithRank;
        private Player own;

        private static string[] RANK_NAMES = {
		    "Anyone", "Any friends", "Recruit+",
		    "Corporal+", "Sergeant+",  "Lieutenant+",
		    "Captain+", "General+", "Only me"
	    };

        public enum ClanRank
        {
            FRIEND = 0,
            RECRUIT = 1,
            CORPORAL = 2,
            SERGEANT = 3,
            LIEUTENANT = 4,
            CAPTAIN = 5,
            GENERAL = 6,
            OWNER = 7,
            NO_RANK = -1
        };

        public Clan(Player p, string name, string owner)
        {
            this.name = name;
            this.owner = owner;
            this.own = p;
            this.users = new List<ClanUser>(100);
            this.ownerFriends = p.getFriends().getFriendsList();
            this.usersWithRank = new Dictionary<string, ClanRank>(250);
            this.kickRights = ClanRank.OWNER;
            this.enterRights = ClanRank.NO_RANK;
            this.talkRights = ClanRank.NO_RANK;
        }

        public void addUser(Player p)
        {
            ClanUser user = new ClanUser(p, this);
            if (p.getLoginDetails().getUsername().Equals(owner))
            {
                user.setClanRights(ClanRank.OWNER);
                own = p;
            }
            if (ownerFriends.Contains(p.getLoginDetails().getLongName()))
            {
                if (user.getClanRights() == ClanRank.NO_RANK)
                {
                    user.setClanRights(ClanRank.FRIEND);
                }
            }

            foreach (KeyValuePair<string, ClanRank> u in usersWithRank)
            {
                if (u.Key.Equals(p.getLoginDetails().getUsername()))
                {
                    user.setClanRights(u.Value);
                    break;
                }
            }
            p.setClan(this);
            lock (users)
            {
                users.Add(user);
            }
        }

        public void removeUser(Player p)
        {
            foreach (ClanUser u in users)
            {
                if (u.getClanMember().Equals(p))
                {
                    lock (users)
                    {
                        users.Remove(u);
                    }
                    p.setClan(null);
                    break;
                }
            }
        }

        public ClanUser getUser(Player p)
        {
            foreach (ClanUser u in users)
            {
                if (u.getClanMember().Equals(p))
                {
                    return u;
                }
            }
            return null;
        }

        public ClanRank getUserRank(string name)
        {
            foreach (KeyValuePair<string, ClanRank> u in usersWithRank)
            {
                if (u.Key.Equals(name))
                {
                    return u.Value;
                }
            }
            return 0;
        }

        public ClanUser getUserByName(string name)
        {
            foreach (ClanUser u in users)
            {
                if (u.getClanMember().getLoginDetails().getUsername().Equals(name))
                {
                    return u;
                }
            }
            return null;
        }

        public void kickUser(long name)
        {
            ClanUser user = getUserByName(Misc.longToPlayerName(name));
            if (user != null)
            {
                removeUser(user.getClanMember());
                user.getClanMember().getPackets().resetClanInterface();
                Server.getClanManager().updateClan(this);
                user.getClanMember().getPackets().sendMessage("You have been kicked from the clan channel.");
            }
        }

        public bool userHasRank(string name)
        {
            foreach (KeyValuePair<string, ClanRank> u in usersWithRank)
            {
                if (u.Key.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isFriendOfOwner(Player p)
        {
            return ownerFriends.Contains(p.getLoginDetails().getLongName());
        }

        public string getClanName()
        {
            return name;
        }

        public void setClanName(string name)
        {
            foreach (ClanUser u in users)
            {
                u.getClanMember().getPackets().sendMessage("The channel name has been changed to : " + Misc.formatPlayerNameForDisplay(name) + ":clan:");
            }
            this.name = name;
        }

        public string getClanOwner()
        {
            return owner;
        }

        public List<ClanUser> getUserList()
        {
            return users;
        }

        public ClanRank getKickRights()
        {
            return kickRights;
        }

        public void setKickRights(ClanRank rights)
        {
            this.kickRights = rights;
            Server.getClanManager().updateClan(this);
        }

        public ClanRank getEnterRights()
        {
            return enterRights;
        }

        public void setEnterRights(ClanRank enterRights)
        {
            this.enterRights = enterRights;
        }

        public ClanRank getTalkRights()
        {
            return talkRights;
        }

        public void setTalkRights(ClanRank talkRights)
        {
            this.talkRights = talkRights;
        }

        public ClanRank getLootRights()
        {
            return lootRights;
        }

        public void setLootRights(ClanRank lootRights)
        {
            this.lootRights = lootRights;
        }

        public string getRankString(ClanRank rank)
        {
            return RANK_NAMES[(int)rank + 1];
        }

        public Dictionary<string, ClanRank> getUsersWithRank()
        {
            return usersWithRank;
        }

        public List<long> getOwnerFriends()
        {
            return ownerFriends;
        }
    }
}