using RS2.Server.definitions;
using RS2.Server.minigames.duelarena;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.smithing;
using RS2.Server.util;

namespace RS2.Server.packethandler
{
    //This class handles 3 walking packets just so you know.
    internal class Walk : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            int size = packet.getLength();
            if (packet.getPacketId() == PacketHandlers.PacketId.WALK_2)
            {
                size -= 14;
            }
            player.getWalkingQueue().resetWalkingQueue();
            int steps = (size - 5) / 2;

            if (steps > WalkingQueue.MAX_WALKING_WAYPOINTS)
            {
                Misc.WriteError("Warning: Walk command contains too many steps (" + steps + ") currently set to 50 [maybe need increase?]");
                return;
            }

            player.getWalkingQueue().setIsRunning(packet.readByteA() == 1);
            bool following = false;
            if (!canWalk(player, packet, following))
            {
                player.getPackets().clearMapFlag();
                player.getWalkingQueue().resetWalkingQueue();
                return;
            }

            int firstX = packet.readUShort() - (player.getLocation().getRegionX() - 6) * 8;
            int firstY = packet.readShortA() - (player.getLocation().getRegionY() - 6) * 8;
            player.getWalkingQueue().addToWalkingQueue(firstX, firstY);

            for (int i = 0; i < steps; i++) //all the waypoints.
                player.getWalkingQueue().addToWalkingQueue((packet.readByteA() + firstX), (packet.readByteS() + firstY));

            if (player.getTeleportTo() != null) //is teleporting?
                player.getWalkingQueue().resetWalkingQueue();

            if (player.getTemporaryAttribute("homeTeleporting") != null)
            {
                player.removeTemporaryAttribute("homeTeleporting");
            }
            SkillHandler.resetAllSkills(player);
            if (player.getTrade() != null)
                player.getTrade().decline();
            if (player.getDuel() != null)
            {
                if (player.getDuel().getStatus() < 4)
                    player.getDuel().declineDuel();
                else if (player.getDuel().getStatus() == 8)
                    player.getDuel().recieveWinnings(player);
            }
            if (player.getTarget() != null)
            {
                if (!following && player.getTarget().getAttacker() != null && player.getTarget().getAttacker().Equals(player))
                    player.getTarget().setAttacker(null);
            }

            if (!following)
            {
                player.getFollow().setFollowing(null);
                player.setTarget(null);
                player.removeTemporaryAttribute("autoCasting");
                if (player.getEntityFocus() != 65535)
                    player.setEntityFocus(65535);
            }
            player.getPackets().closeInterfaces();
        }

        private bool canWalk(Player player, Packet packet, bool following)
        {
            if (player.getTemporaryAttribute("smeltingBar") != null)
            {
                Smelting.setAmountToZero(player);
                return false;
            }
            else if (player.getTemporaryAttribute("teleporting") != null && player.getTemporaryAttribute("homeTeleporting") == null)
            {
                return false;
            }
            else if (player.isFrozen())
            {
                player.getPackets().sendMessage("A magic force prevents you from moving!");
                return false;
            }
            else if (player.getDuel() != null)
            {
                if (player.getDuel().ruleEnabled(DuelSession.RULE.NO_MOVEMENT))
                {
                    if (player.getDuel().getStatus() == 5 || player.getDuel().getStatus() == 6)
                    {
                        if (player.getTarget() == null)
                            player.getPackets().sendMessage("Movement is disabled for this duel.");
                        return false;
                    }
                }
            }
            else if (player.getTemporaryAttribute("unmovable") != null || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return false;
            }
            else if (player.isDead())
            {
                return false;
            }
            else if (player.getTeleportTo() != null)
            {
                return false;
            }
            return true;
        }
    }
}