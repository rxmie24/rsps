using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class MaxHit : Command
    {
        public void execute(Player player, string[] arguments)
        {
            player.getPackets().sendMessage("Maxhit = " + player.getMaxHit());
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}