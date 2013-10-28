using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class SwitchMagic : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Switch magic command]: ::switch type (example ::switch 1)");
                return;
            }

            int type = 0;

            if (!int.TryParse(arguments[0], out type))
            {
                player.getPackets().sendMessage("[Switch magic command]: ::switch type (example ::switch 1)");
                return;
            }
            switch (type)
            {
                case 3:
                    player.setMagicType(3);
                    player.getPackets().sendTab(player.isHd() ? 99 : 89, 430);
                    break;

                case 2:
                    player.setMagicType(2);
                    player.getPackets().sendTab(player.isHd() ? 99 : 89, 193);
                    break;

                case 1:
                    player.setMagicType(1);
                    player.getPackets().sendTab(player.isHd() ? 99 : 89, 192);
                    break;
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}