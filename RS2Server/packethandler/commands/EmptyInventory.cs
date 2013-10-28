using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class EmptyInventory : Command
    {
        public void execute(Player player, string[] arguments)
        {
            player.getInventory().deleteAll();
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}