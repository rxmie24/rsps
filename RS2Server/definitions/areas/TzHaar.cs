using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.definitions.areas
{
    internal class TzHaar
    {
        public TzHaar()
        {
        }

        public static void exitTzhaar(Player p)
        {
            AreaEvent exitTzhaarAreaEvent = new AreaEvent(p, 2479, 5175, 2481, 5175);
            exitTzhaarAreaEvent.setAction(() =>
            {
                p.teleport(new Location(2866, 9571, 0));
            });
            Server.registerCoordinateEvent(exitTzhaarAreaEvent);
        }

        public static void enterTzhaar(Player p)
        {
            AreaEvent enterTzhaarAreaEvent = new AreaEvent(p, 2866, 9570, 2866, 9572);
            enterTzhaarAreaEvent.setAction(() =>
            {
                p.teleport(new Location(2480, 5175, 0));
            });
            Server.registerCoordinateEvent(enterTzhaarAreaEvent);
        }

        public static void useTzhaarBanker(Player p, int option)
        {
        }

        public static bool interactTzhaarNPC(Player p, Npc n, int option)
        {
            if (n.getId() != 2622 && n.getId() != 2620 && n.getId() != 2623 && n.getId() != 2619 && n.getId() != 2617 && n.getId() != 2618)
            {
                return false;
            }
            p.setEntityFocus(n.getClientIndex());
            int npcX = n.getLocation().getX();
            int npcY = n.getLocation().getY();
            AreaEvent interactTzhaarNpcAreaEvent = new AreaEvent(p, npcX - 1, npcY - 1, npcX + 1, npcY + 1);
            interactTzhaarNpcAreaEvent.setAction(() =>
            {
                p.setFaceLocation(n.getLocation());
                p.setEntityFocus(65535);
                switch (n.getId())
                {
                    case 2619: // Bankers
                        if (option == 1)
                        { // Talk
                        }
                        else if (option == 2)
                        { // Bank
                            p.getBank().openBank();
                        }
                        else if (option == 3)
                        { // Collect
                        }
                        break;

                    //TODO tzhaar stores
                    case 2622: // Ore shop
                        if (option == 1)
                        { // Speak
                        }
                        else if (option == 2)
                        { // Trade
                            p.setShopSession(new ShopSession(p, 3));
                        }
                        break;
                }
            });
            Server.registerCoordinateEvent(interactTzhaarNpcAreaEvent);
            return true;
        }
    }
}