using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class UnderGround : Command
    {
        public void execute(Player player, string[] arguments)
        {
            player.teleport(new Location(player.getLocation().getX(), player.getLocation().getY() + 6400, 0));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}