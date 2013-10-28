using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Kick : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Kick command]: ::kick playerName, or else try ::kickall");
                return;
            }

            arguments[0] = arguments[0].ToLower();
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.getLoginDetails().getUsername().ToLower().Equals(arguments[0]))
                    {
                        p.getPackets().logout();
                        player.getPackets().sendMessage("You have successfully kicked " + arguments[0]);
                        break;
                    }
                }
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}