using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class OpenShop : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length > 0)
            {
                int shopIndex = 0;
                if (!int.TryParse(arguments[0], out shopIndex))
                {
                    player.getPackets().sendMessage("[OpenShop command]: ::shop shop_number or just ::shop to open first one.");
                    return;
                }

                Shop shop = Server.getShopManager().getShop(shopIndex);
                if (shop == null)
                {
                    player.getPackets().sendMessage("[OpenShop command]: shop #" + shopIndex + " doesn't exist on server");
                    return;
                }
                player.setShopSession(new ShopSession(player, shopIndex));
            }
            player.setShopSession(new ShopSession(player, 1));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}