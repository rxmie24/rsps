using RS2.Server.clans;
using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.util;

namespace RS2.Server.packethandler
{
    internal class Communication : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.PUBLIC:
                    handlePublicChat(player, packet);
                    break;

                case PacketHandlers.PacketId.CLAN_CHAT:
                    handleClanChat(player, packet);
                    break;

                case PacketHandlers.PacketId.ADD_FRIEND:
                    handleAddFriend(player, packet);
                    break;

                case PacketHandlers.PacketId.DELETE_FRIEND:
                    handleDeleteFriend(player, packet);
                    break;

                case PacketHandlers.PacketId.ADD_IGNORE:
                    handleAddIgnore(player, packet);
                    break;

                case PacketHandlers.PacketId.DELETE_IGNORE:
                    handleDeleteIgnore(player, packet);
                    break;

                case PacketHandlers.PacketId.SEND_PM:
                    handleSendPm(player, packet);
                    break;

                case PacketHandlers.PacketId.CLAN_RANKS:
                    handleClanRanks(player, packet);
                    break;

                case PacketHandlers.PacketId.CLAN_KICK:
                    handleClanKick(player, packet);
                    break;

                case PacketHandlers.PacketId.PRIVACY_SETTINGS:
                    handlePrivacySettings(player, packet);
                    break;
            }
        }

        private void handlePublicChat(Player player, Packet packet)
        {
            int colour = packet.readByte();
            int effects = packet.readByte();
            byte[] packedChatData = packet.getRemainingData();
            string unpacked = Misc.textUnpack(packedChatData);
            ChatMessage message;

            if (unpacked.StartsWith("/") && player.getClan() != null)
            {
                packedChatData = Misc.textPack(unpacked.Substring(1));
                message = new ChatMessage(colour, unpacked.Substring(1), effects, player, packedChatData);
                Server.getClanManager().newClanMessage(player.getClan(), message);
                return;
            }
            message = new ChatMessage(colour, unpacked, effects, player, packedChatData);
            player.setLastChatMessage(message);
        }

        private void handleClanChat(Player player, Packet packet)
        {
            long clanOwner = packet.readLong();
            if (clanOwner < 0)
            {
                return;
            }
            if (clanOwner == 0)
            {
                Server.getClanManager().leaveChannel(player);
                return;
            }
            string ownerName = Misc.longToPlayerName(clanOwner).ToLower();
            Server.getClanManager().enterChannel(player, ownerName);
        }

        private void handleAddFriend(Player player, Packet packet)
        {
            long name = packet.readLong();
            if (name > 0)
            {
                player.getFriends().addFriend(name);
            }
        }

        private void handleDeleteFriend(Player player, Packet packet)
        {
            long name = packet.readLong();
            if (name > 0)
            {
                player.getFriends().removeFriend(name);
            }
        }

        private void handleAddIgnore(Player player, Packet packet)
        {
            long name = packet.readLong();
            if (name > 0)
            {
                player.getFriends().addIgnore(name);
            }
        }

        private void handleDeleteIgnore(Player player, Packet packet)
        {
            long name = packet.readLong();
            if (name > 0)
            {
                player.getFriends().removeIgnore(name);
            }
        }

        private void handleSendPm(Player player, Packet packet)
        {
            long name = packet.readLong();
            byte[] privateMsgPacked = packet.getRemainingData();
            string privateMsg = Misc.textUnpack(privateMsgPacked);
            if (privateMsg != null && name > 0)
            {
                player.getFriends().sendMessage(name, privateMsg, privateMsgPacked);
            }
        }

        private void handleClanRanks(Player player, Packet packet)
        {
            int rank = packet.readByteA();
            long name = packet.readLong();
            if (name < 0 || (rank < 0 || rank > 6))
            {
                return;
            }
            Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
            if (clan != null)
            {
                ClanUser user = clan.getUserByName(Misc.longToPlayerName(name));
                if (user != null)
                {
                    user.setClanRights((Clan.ClanRank)rank);
                    Server.getClanManager().updateClan(clan);
                }
                Clan.ClanRank userExists;
                if (clan.getUsersWithRank().TryGetValue(Misc.longToPlayerName(name), out userExists))
                    clan.getUsersWithRank()[Misc.longToPlayerName(name)] = (Clan.ClanRank)rank;  //Exists already, so altar.
                else
                    clan.getUsersWithRank().Add(Misc.longToPlayerName(name), (Clan.ClanRank)rank); //Not existed yet. so add
            }
        }

        private void handleClanKick(Player player, Packet packet)
        {
            long name = packet.readLong();
            if (name < 0)
            {
                return;
            }
            Clan clan = Server.getClanManager().getClanByPlayer(player);
            if (clan != null)
            {
                ClanUser user = clan.getUserByName(player.getLoginDetails().getUsername());
                if (user != null)
                {
                    if (user.getClanRights() < clan.getKickRights())
                    {
                        player.getPackets().sendMessage("You do not have a high enough rank to kick users from this clan chat.");
                        return;
                    }
                    clan.kickUser(name);
                }
            }
        }

        private void handlePrivacySettings(Player player, Packet packet)
        {
            int publicStatus = packet.readByte();
            int privateStatus = packet.readByte();
            int tradeStatus = packet.readByte();
            player.getFriends().setPrivacyOption((Friends.STATUS)publicStatus, (Friends.STATUS)privateStatus, (Friends.STATUS)tradeStatus);
        }
    }
}