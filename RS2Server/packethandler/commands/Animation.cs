using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Animation : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Animation command]: ::emote emote_number");
                return;
            }

            int animation = 0;
            if (!int.TryParse(arguments[0], out animation))
            {
                player.getPackets().sendMessage("[Animation command]: ::emote emote_number");
                return;
            }

            player.setLastAnimation(new model.Animation(animation));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}