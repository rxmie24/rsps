using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class CharacterAppearance : Command
    {
        public void execute(Player player, string[] arguments)
        {
            ConfigureAppearance.openInterface(player);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}