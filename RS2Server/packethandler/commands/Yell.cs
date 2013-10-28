using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Yell : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Yell command]: what you expecting to yell blank message?");
                return;
            }

            string yellMsg = string.Join(" ", arguments);

            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    p.getPackets().sendMessage(player.getLoginDetails().getUsername() + ": " + yellMsg);
                }
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}