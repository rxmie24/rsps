using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.definitions.areas
{
    internal class AlKharid
    {
        public AlKharid()
        {
        }

        public static void interactWithAliMorissaae(Player p, Npc n)
        {
            p.setEntityFocus(n.getClientIndex());
            AreaEvent interactWithAliMorissaaeAreaEvent = new AreaEvent(p, n.getLocation().getX() - 1, n.getLocation().getY() - 1, n.getLocation().getX() + 1, n.getLocation().getY() + 1);
            interactWithAliMorissaaeAreaEvent.setAction(() =>
            {
                n.setFaceLocation(p.getLocation());
                p.setFaceLocation(n.getLocation());
                p.setEntityFocus(65535);
                showAliDialogue(p, 205);
            });
            Server.registerCoordinateEvent(interactWithAliMorissaaeAreaEvent);
        }

        public static void showAliDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            switch (status)
            {
                case 205:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("Would you like to go back to Lletya?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 206;
                    break;

                case 206:
                    p.getPackets().sendChatboxInterface2(228);
                    p.getPackets().modifyText("Yes please, let's go", 228, 2);
                    p.getPackets().modifyText("Not yet", 228, 3);
                    newStatus = 207;
                    break;

                case 207:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Yes please, let's go.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 208;
                    break;

                case 208:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("Very well.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 209;
                    break;

                case 209:
                    p.setTemporaryAttribute("unmovable", true);
                    p.getPackets().displayInterface(120);
                    p.getPackets().sendMessage("Your camel trots off slowly out of the desert..");
                    Event moveBackToLletyaEvent = new Event(2000);
                    int moveBackToLletyaCounter = 0;
                    moveBackToLletyaEvent.setAction(() =>
                    {
                        if (moveBackToLletyaCounter == 0)
                        {
                            moveBackToLletyaCounter++;
                            moveBackToLletyaEvent.setTick(600);
                            p.teleport(new Location(2340, 3799, 0));
                        }
                        else
                        {
                            moveBackToLletyaEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.getPackets().sendMessage("..You and Ali are back in Lletya.");
                            p.getPackets().closeInterfaces();
                        }
                    });
                    Server.registerEvent(moveBackToLletyaEvent);
                    break;

                case 210:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Not yet.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 211;
                    break;

                case 211:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("No problem, I will be here when you decide to leave.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }
    }
}