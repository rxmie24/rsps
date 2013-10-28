using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class PlayerAsNpc : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[PlayerAsNpc command]: ::pnpc npcId");
                return;
            }

            int npcId = 0;
            if (!int.TryParse(arguments[0], out npcId))
            {
                player.getPackets().sendMessage("[PlayerAsNpc command]: ::pnpc npcId [npcId is not a number error]");
                return;
            }

            player.getAppearance().setNpcId(npcId);
            player.getUpdateFlags().setAppearanceUpdateRequired(true);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}