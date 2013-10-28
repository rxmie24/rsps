using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Interface : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Interface command]: ::inter interface_number");
                return;
            }

            int intreface = 0;
            if (!int.TryParse(arguments[0], out intreface))
            {
                player.getPackets().sendMessage("[Interface command]: ::inter interface_number");
                return;
            }

            player.getPackets().displayInterface(intreface);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}