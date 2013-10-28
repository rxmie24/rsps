using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class ClientSideObjectDump : Command
    {
        public void execute(Player player, string[] arguments)
        {
            player.getPackets().sendMessage("Spawning all possible objects please wait..");
            for (int i = 0; i < 50000; i++)
                player.getPackets().createObject(i, player.getLocation(), 0, 10);
            player.getPackets().sendMessage("Dumping complete, now add dump to server.");
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}