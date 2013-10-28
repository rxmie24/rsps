using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Height : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Height command]: ::height 0 [0,1,2,3] are possible");
                return;
            }

            int heightLevel = 0;
            if (!int.TryParse(arguments[0], out heightLevel))
            {
                player.getPackets().sendMessage("[Height command]: ::height 0 [0,1,2,3] are possible");
                return;
            }

            player.teleport(new Location(player.getLocation().getX(), player.getLocation().getY(), heightLevel));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}