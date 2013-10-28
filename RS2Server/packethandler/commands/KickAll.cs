using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class KickAll : Command
    {
        public void execute(Player player, string[] arguments)
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                    p.getPackets().forceLogout(); //even kicks yourself too lol.
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}