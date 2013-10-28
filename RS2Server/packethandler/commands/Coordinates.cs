using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Coordinates : Command
    {
        public void execute(Player player, string[] arguments)
        {
            Location loc = player.getLocation();
            player.getPackets().sendMessage("Coordinates are X = [" + loc.getX() + "] Y = [" + loc.getY() + "] Z = [" + loc.getZ() + "]");
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}