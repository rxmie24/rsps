using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills;

namespace RS2.Server.packethandler
{
    internal class PlayerInteract : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.ATTACK_PLAYER:
                    handleAttackPlayer(player, packet);
                    break;

                case PacketHandlers.PacketId.FOLLOW:
                    handleFollowPlayer(player, packet);
                    break;

                case PacketHandlers.PacketId.TRADE:
                    handleTradePlayer(player, packet);
                    break;

                case PacketHandlers.PacketId.MAGIC_ON_PLAYER:
                    handleMagicOnPlayer(player, packet);
                    break;
            }
        }

        private void handleAttackPlayer(Player player, Packet packet)
        {
            int index = packet.readLEShortA();
            if (index < 0 || index >= Constants.MAX_PLAYERS || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            Player p2 = Server.getPlayerList()[index];
            if (p2 == null)
                return;

            player.setFaceLocation(p2.getLocation());
            player.getPackets().closeInterfaces();
            if (Location.atDuelArena(player.getLocation()))
            {
                if (player.getDuel() != null)
                {
                    if (player.getDuel().getStatus() < 4)
                    {
                        player.getDuel().declineDuel();
                        return;
                    }
                    else if (player.getDuel().getStatus() == 5 && player.getDuel().getPlayer2().Equals(p2))
                    {
                        player.getPackets().sendMessage("The duel has not yet started!");
                        return;
                    }
                    else if ((player.getDuel().getStatus() == 5 || player.getDuel().getStatus() == 6) && !player.getDuel().getPlayer2().Equals(p2))
                    {
                        return;
                    }
                    else if (player.getDuel().getStatus() == 6)
                    {
                        Combat.newAttack(player, p2);
                        return;
                    }
                }
                if (!player.getLocation().withinDistance(p2.getLocation(), 1))
                {
                    int x = p2.getLocation().getX();
                    int y = p2.getLocation().getY();
                    AreaEvent attackAreaEvent = new AreaEvent(player, x - 1, y - 1, x + 1, y + 1);
                    attackAreaEvent.setAction(() =>
                    {
                        player.getWalkingQueue().resetWalkingQueue();
                        player.getPackets().clearMapFlag();
                        if (p2.getGESession() != null || (p2.getDuel() != null && !p2.getDuel().getPlayer2().Equals(player)) || p2.getTrade() != null || p2.getShopSession() != null || p2.getBank().isBanking())
                        {
                            player.getPackets().sendMessage("That player is busy at the moment.");
                            return;
                        }
                        if (p2.wantsToDuel(player))
                        {
                            p2.setFaceLocation(player.getLocation());
                            player.getPackets().closeInterfaces();
                            p2.getPackets().closeInterfaces();
                            player.setDuelSession(new DuelSession(player, p2));
                            p2.setDuelSession(new DuelSession(p2, player));
                            return;
                        }
                        player.setFaceLocation(p2.getLocation());
                        p2.getPackets().sendMessage(player.getLoginDetails().getUsername() + ":duelstake:");
                        player.getPackets().sendMessage("Sending duel request...");
                        player.newDuelRequest(p2);
                    });
                    Server.registerCoordinateEvent(attackAreaEvent);
                    return;
                }
                if (p2.getGESession() != null || (p2.getDuel() != null && !p2.getDuel().getPlayer2().Equals(player)) || p2.getTrade() != null || p2.getShopSession() != null || p2.getBank().isBanking())
                {
                    player.getPackets().sendMessage("That player is busy at the moment.");
                    return;
                }
                if (p2.wantsToDuel(player))
                {
                    player.getPackets().closeInterfaces();
                    p2.getPackets().closeInterfaces();
                    p2.setFaceLocation(player.getLocation());
                    player.setDuelSession(new DuelSession(player, p2));
                    p2.setDuelSession(new DuelSession(p2, player));
                    return;
                }
                player.newDuelRequest(p2);
                p2.getPackets().sendMessage(player.getLoginDetails().getUsername() + ":duelstake:");
                player.getPackets().sendMessage("Sending duel request...");
                return;
            }
            Combat.newAttack(player, p2);
        }

        private void handleFollowPlayer(Player player, Packet packet)
        {
            int index = packet.readLEShortA();
            if (index < 0 || index >= Constants.MAX_PLAYERS || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            Player p2 = Server.getPlayerList()[index];
            if (p2 == null)
            {
                return;
            }
            player.getFollow().setFollowing(p2);
        }

        private void handleTradePlayer(Player player, Packet packet)
        {
            int index = packet.readLEShortA();
            if (index < 0 || index >= Constants.MAX_PLAYERS || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            Player p2 = Server.getPlayerList()[index];
            if (p2 == null)
            {
                return;
            }
            player.setFaceLocation(p2.getLocation());
            if (player.getTrade() != null)
            {
                player.getTrade().decline();
                return;
            }
            player.getPackets().closeInterfaces();
            if (!player.getLocation().withinDistance(p2.getLocation(), 1))
            {
                int x = p2.getLocation().getX();
                int y = p2.getLocation().getY();
                AreaEvent tradePlayerAreaEvent = new AreaEvent(player, x - 1, y - 1, x + 1, y + 1);
                tradePlayerAreaEvent.setAction(() =>
                {
                    player.getWalkingQueue().resetWalkingQueue();
                    player.getPackets().clearMapFlag();
                    if (p2.getGESession() != null || p2.getTrade() != null || p2.getDuel() != null || p2.getShopSession() != null || p2.getBank().isBanking())
                    {
                        player.getPackets().sendMessage("That player is busy at the moment.");
                        return;
                    }
                    if (p2.wantsToTrade(player))
                    {
                        player.getPackets().closeInterfaces();
                        p2.getPackets().closeInterfaces();
                        p2.setFaceLocation(player.getLocation());
                        player.setTrade(new TradeSession(player, p2));
                        p2.setTrade(new TradeSession(p2, player));
                        return;
                    }
                    player.setFaceLocation(p2.getLocation());
                    p2.getPackets().sendMessage(player.getLoginDetails().getUsername() + ":tradereq:");
                    player.getPackets().sendMessage("Sending trade offer...");
                    player.newTradeRequest(p2);
                });
                Server.registerCoordinateEvent(tradePlayerAreaEvent);
                return;
            }
            if (p2.getGESession() != null || p2.getTrade() != null || p2.getDuel() != null || p2.getShopSession() != null || p2.getBank().isBanking())
            {
                player.getPackets().sendMessage("That player is busy at the moment.");
                return;
            }
            if (p2.wantsToTrade(player))
            {
                player.getPackets().closeInterfaces();
                p2.getPackets().closeInterfaces();
                p2.setFaceLocation(player.getLocation());
                player.setTrade(new TradeSession(player, p2));
                p2.setTrade(new TradeSession(p2, player));
                return;
            }
            player.newTradeRequest(p2);
            p2.getPackets().sendMessage(player.getLoginDetails().getUsername() + ":tradereq:");
            player.getPackets().sendMessage("Sending trade offer...");
        }

        private void handleMagicOnPlayer(Player player, Packet packet)
        {
            int junk = packet.readShortA();
            int id = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            int index = packet.readLEShortA();
            if (index < 0 || index >= Constants.MAX_PLAYERS || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            Player p2 = Server.getPlayerList()[index];
            if (p2 == null)
            {
                return;
            }
            player.getPackets().closeInterfaces();
            player.setTarget(p2);
            MagicCombat.newMagicAttack(player, p2, id, interfaceId == 193);
        }
    }
}