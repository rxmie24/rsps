using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;
using System.Collections.Generic;

namespace RS2.Server.clans
{
    internal class ClanManager
    {
        //TODO coinshare/lootshare
        private List<Clan> clans;

        public ClanManager()
        {
            clans = new List<Clan>();
        }

        public void enterChannel(Player p, string owner)
        {
            if (p.getClan() != null)
            {
                return;
            }
            p.getPackets().sendMessage("Attempting to join channel...:clan:");
            if (owner.Equals(p.getLoginDetails().getUsername()))
            {
                Clan newClan = new Clan(p, owner, owner);
                addChannel(newClan);
            }
            Event enterChannelEvent = new Event(700);
            enterChannelEvent.setAction(() =>
            {
                enterChannelEvent.stop();
                foreach (Clan c in clans)
                {
                    if (c != null)
                    {
                        if (c.getClanOwner().Equals(owner))
                        {
                            if (c.getUserList().Count >= 100)
                            {
                                p.getPackets().sendMessage("The channel is full.");
                                return;
                            }
                            if (!owner.Equals(p.getLoginDetails().getUsername()))
                            {
                                if (c.getEnterRights() != Clan.ClanRank.NO_RANK)
                                {
                                    if (c.getEnterRights() == Clan.ClanRank.FRIEND)
                                    {
                                        if (!c.isFriendOfOwner(p) && !c.userHasRank(p.getLoginDetails().getUsername()))
                                        {
                                            p.getPackets().sendMessage("You do not have a high enough rank to enter this clan chat.");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        bool canEnter = true;
                                        foreach (KeyValuePair<string, Clan.ClanRank> u in c.getUsersWithRank())
                                        {
                                            if (u.Key.Equals(p.getLoginDetails().getUsername()))
                                            {
                                                if (u.Value < c.getEnterRights())
                                                {
                                                    canEnter = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!canEnter)
                                        {
                                            p.getPackets().sendMessage("You do not have a high enough rank to enter this clan chat.");
                                            return;
                                        }
                                    }
                                }
                            }
                            c.addUser(p);
                            updateClan(c);
                            p.getPackets().sendMessage("Now talking in channel : " + Misc.formatPlayerNameForDisplay(c.getClanName() + ":clan:"));
                            p.getPackets().sendMessage("To talk, start each line of chat with the / symbol. :clan:");
                            return;
                        }
                    }
                }
                p.getPackets().sendMessage("The channel you tried to join does not exist. :clan:");
            });
            Server.registerEvent(enterChannelEvent);
        }

        public void leaveChannel(Player p)
        {
            foreach (Clan c in clans)
            {
                if (c != null)
                {
                    if (c.getUser(p) != null)
                    {
                        c.removeUser(p);
                        p.getPackets().resetClanInterface();
                        p.getPackets().sendMessage("You have left the channel. :clan:");
                        updateClan(c);
                        break;
                    }
                }
            }
        }

        public void updateClan(Clan c)
        {
            foreach (ClanUser cu in c.getUserList())
            {
                Player p = cu.getClanMember();

                if (p != null)
                    p.getPackets().updateClan(c);
            }
        }

        public void newClanMessage(Clan c, ChatMessage m)
        {
            Player p = m.getPlayer();
            if (!c.getClanOwner().Equals(p.getLoginDetails().getUsername()))
            {
                if (c.getTalkRights() != Clan.ClanRank.NO_RANK)
                {
                    if (c.getTalkRights() == 0)
                    {
                        if (!c.isFriendOfOwner(p) && !c.userHasRank(p.getLoginDetails().getUsername()))
                        {
                            p.getPackets().sendMessage("You do not have a high enough rank to talk in this clan chat.");
                            return;
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, Clan.ClanRank> u in c.getUsersWithRank())
                        {
                            if (u.Key.Equals(p.getLoginDetails().getUsername()))
                            {
                                if (u.Value < c.getTalkRights())
                                {
                                    p.getPackets().sendMessage("You do not have a high enough rank to talk in this clan chat.");
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //Send the clan chat message to all clan members.
            foreach (ClanUser cu in c.getUserList())
            {
                p = cu.getClanMember();
                if (p != null)
                    p.getPackets().newClanMessage(c, m);
            }
        }

        public void openClanSetup(Player p)
        {
            p.getPackets().displayInterface(590);
            Clan clan = getClanByOwner(p.getLoginDetails().getUsername());
            if (clan == null)
            {
                Clan newClan = new Clan(p, "Clan name", p.getLoginDetails().getUsername());
                addChannel(newClan);
                p.getPackets().sendMessage("Your clan chat has been succesfully set up.");
                return;
            }
            p.getPackets().modifyText(Misc.formatPlayerNameForDisplay(clan.getClanName()), 590, 22);
            p.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
            p.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
            p.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
            p.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
        }

        public Clan getClanByOwner(string owner)
        {
            foreach (Clan c in clans)
            {
                if (c != null)
                {
                    if (c.getClanOwner().Equals(owner))
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        public Clan getClanByPlayer(Player p)
        {
            foreach (Clan c in clans)
            {
                if (c != null)
                {
                    if (c.getUser(p) != null)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        private void addChannel(Clan clan)
        {
            lock (clans)
            {
                clans.Add(clan);
            }
        }

        public void deleteChannel(Clan clan)
        {
            lock (clans)
            {
                clans.Remove(clan);
            }
        }
    }
}