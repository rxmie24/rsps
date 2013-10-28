using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Test : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Animation command]: ::t emote_number");
                return;
            }

            int animation = 0;
            if (!int.TryParse(arguments[0], out animation))
            {
                player.getPackets().sendMessage("[Animation command]: ::emote emote_number");
                return;
            }
            //1179 flash + skill icon  , 1230 = make box
            player.getPackets().playSoundEffect(animation, 1, 0);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}