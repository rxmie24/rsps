using RS2.Server.definitions;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class ReloadLadderXml : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Reload Ladders & Stairs XML File]: This command is only for server developers.");
                player.getPackets().sendMessage("Reloading... [Could crash server if populated, as all ladders get erased]");
                LaddersAndStairs.load();
                player.getPackets().sendMessage("Reloaded.");
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}