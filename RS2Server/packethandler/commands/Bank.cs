using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Bank : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (!player.inCombat())
                player.getBank().openBank();
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}