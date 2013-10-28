using RS2.Server.net;
using RS2.Server.packethandler.commands;
using RS2.Server.player;

namespace RS2.Server.packethandler
{
    internal class Command : PacketHandler
    {
        public void handlePacket(Player player, Packet p)
        {
            string command = p.readRS2String().ToLower();
            CommandManager.execute(player, command);
        }
    }
}