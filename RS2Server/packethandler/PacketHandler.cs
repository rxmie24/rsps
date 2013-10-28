using RS2.Server.net;
using RS2.Server.player;

namespace RS2.Server.packethandler
{
    internal interface PacketHandler
    {
        void handlePacket(Player player, Packet p);
    }
}