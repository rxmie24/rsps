using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class SpawnNpc : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[SpawnNpc command]: ::npc npc_id");
                return;
            }

            int npcId = 0;
            if (!int.TryParse(arguments[0], out npcId))
            {
                player.getPackets().sendMessage("[SpawnNpc command]: ::npc npc_id");
                return;
            }

            Npc npc = new Npc(npcId, player.getLocation());
            npc.setMinimumCoords(new Location(player.getLocation().getX() - 5, player.getLocation().getY() - 5, player.getLocation().getZ()));
            npc.setMaximumCoords(new Location(player.getLocation().getX() + 5, player.getLocation().getY() + 5, player.getLocation().getZ()));
            Server.getNpcList().Add(npc);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}