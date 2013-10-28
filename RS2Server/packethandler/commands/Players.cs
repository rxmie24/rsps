using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Players : Command
    {
        public void execute(Player player, string[] arguments)
        {
            //I miss those interfaces that people used to do in 317/377 serers..
            //TODO: add that interface!.
            player.getPackets().sendMessage("Total players online: " + Server.getPlayerList().Count);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}