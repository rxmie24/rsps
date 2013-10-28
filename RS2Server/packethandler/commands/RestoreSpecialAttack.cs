using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class RestoreSpecialAttack : Command
    {
        public void execute(Player player, string[] arguments)
        {
            player.getSpecialAttack().resetSpecial();
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}