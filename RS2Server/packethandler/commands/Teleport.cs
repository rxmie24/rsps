using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Teleport : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length < 2)
            {
                player.getPackets().sendMessage("[Teleport command]: ::tele x y or even ::tele x y z");
                return;
            }
            int x, y, z = 0;
            if (!int.TryParse(arguments[0], out x) || !int.TryParse(arguments[1], out y))
            {
                player.getPackets().sendMessage("Bad x,y coordinate, teleporting home, You entered:[x=" + arguments[0] + ", y=" + arguments[1] + "]");
                player.teleport(Constants.HOME_SPAWN_LOCATION);
                return;
            }
            if (arguments.Length >= 3)
                int.TryParse(arguments[2], out z);

            player.teleport(new Location(x, y, z));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}