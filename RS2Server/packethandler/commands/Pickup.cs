using RS2.Server.definitions;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Pickup : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Pickup command]: ::item itemId amount or just ::item itemId");
                return;
            }

            int itemId = 0;
            if (!int.TryParse(arguments[0], out itemId))
                itemId = 0;
            int amount = 0;
            if (arguments.Length >= 2 && !int.TryParse(arguments[1], out amount))
                amount = 0;
            if (!player.inCombat())
            {
                if (amount > 1 && !ItemData.forId(itemId).isNoted() && !ItemData.forId(itemId).isStackable())
                {
                    for (int i = 0; i < amount; i++)
                    {
                        if (!player.getInventory().addItem(itemId))
                        {
                            break;
                        }
                    }
                }
                else if (amount == 0)
                {
                    player.getInventory().addItem(itemId);
                }
                else
                {
                    player.getInventory().addItem(itemId, amount);
                }
            }
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}