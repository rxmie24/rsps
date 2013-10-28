using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal interface Command
    {
        void execute(Player player, string[] arguments);

        int minimumRightsNeeded();
    }
}