using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.runecrafting;
using RS2.Server.util;

namespace RS2.Server.definitions.areas
{
    internal class HomeArea
    {
        public HomeArea()
        {
        }

        public static void interactWithAubury(Player p, Npc n, int option)
        {
            p.setEntityFocus(n.getClientIndex());
            AreaEvent interactWithAuburyAreaEvent = new AreaEvent(p, n.getLocation().getX() - 1, n.getLocation().getY() - 1, n.getLocation().getX() + 1, n.getLocation().getY() + 1);
            interactWithAuburyAreaEvent.setAction(() =>
            {
                n.setFaceLocation(p.getLocation());
                p.setFaceLocation(n.getLocation());
                p.setEntityFocus(65535);
                switch (option)
                {
                    case 1: // talk to

                        break;

                    case 2: // trade
                        p.setShopSession(new ShopSession(p, 3));
                        break;

                    case 3: // teleport
                        RuneCraft.teleportToEssMine(p, n);
                        break;
                }
            });
            Server.registerCoordinateEvent(interactWithAuburyAreaEvent);
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
                showAliDialogue(p, 156);
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
                case 156:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Hello?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 157;
                    break;

                case 157:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("Well, hello there " + p.getLoginDetails().getUsername() + ", can i help you?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 158;
                    break;

                case 158:
                    p.getPackets().sendChatboxInterface2(230);
                    p.getPackets().modifyText("Can you take me to Al-Kharid please?", 230, 2);
                    p.getPackets().modifyText("You don't look like you're from around here..", 230, 3);
                    p.getPackets().modifyText("Nothing, sorry to bother you", 230, 4);
                    newStatus = 159;
                    break;

                case 159:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Can you take me to Al-Kharid please?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 160;
                    break;

                case 160:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("Certainly, let's go..", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 161;
                    break;

                case 161:
                    p.setTemporaryAttribute("unmovable", true);
                    p.getPackets().displayInterface(120);
                    p.getPackets().sendMessage("You follow Ali across vast lands..");
                    Event moveToAlKharidEvent = new Event(2000);
                    int moveToAlKharidCounter = 0;
                    moveToAlKharidEvent.setAction(() =>
                    {
                        if (moveToAlKharidCounter == 0)
                        {
                            moveToAlKharidCounter++;
                            moveToAlKharidEvent.setTick(600);
                            p.teleport(new Location(3311 + Misc.random(2), 3199 + Misc.random(3), 0));
                        }
                        else
                        {
                            moveToAlKharidEvent.stop();
                            p.getPackets().sendOverlay(170);
                            p.removeTemporaryAttribute("unmovable");
                            p.getPackets().sendMessage("..Eventually, you find yourself in Al-Kharid.");
                            p.getPackets().closeInterfaces();
                            Event removeOverlayEvent = new Event(2000);
                            removeOverlayEvent.setAction(() =>
                            {
                                removeOverlayEvent.stop();
                                p.getPackets().sendRemoveOverlay();
                            });
                            Server.registerEvent(removeOverlayEvent);
                        }
                    });
                    Server.registerEvent(moveToAlKharidEvent);
                    break;

                case 162:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("You don't look like you're from around here..", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 163;
                    break;

                case 163:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("That's because i'm not!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 164;
                    break;

                case 164:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I thought as much, Where are you from?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 165;
                    break;

                case 165:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("I usually reside in a small desert town, called Al-Kharid.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 166;
                    break;

                case 166:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("What does Al-Kharid have to offer?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 167;
                    break;

                case 167:
                    p.getPackets().sendNPCHead(1862, 244, 2);
                    p.getPackets().modifyText("Ali Morissane", 244, 3);
                    p.getPackets().modifyText("Sand! lots, and lots of sand! On a more serious note..", 244, 4);
                    p.getPackets().modifyText("A mine full of ore rich rocks, a duel arena where", 244, 5);
                    p.getPackets().modifyText("I hear fortunes are gained and lost. Also, a small market,", 244, 6);
                    p.getPackets().modifyText("which has been attracting crafty thieves as of late!", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 168;
                    break;

                case 168:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I may have to pay Al-Kharid a visit...", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 169;
                    break;

                case 169:
                    p.getPackets().sendNPCHead(1862, 242, 2);
                    p.getPackets().modifyText("Ali Morissane", 242, 3);
                    p.getPackets().modifyText("There is one problem. We have also been having", 242, 4);
                    p.getPackets().modifyText("trouble with some of the locals recently.", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 170;
                    break;

                case 170:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Trouble with the locals?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 171;
                    break;

                case 171:
                    p.getPackets().sendNPCHead(1862, 243, 2);
                    p.getPackets().modifyText("Ali Morissane", 243, 3);
                    p.getPackets().modifyText("Yes. Although I really shouldn't say anything but", 243, 4);
                    p.getPackets().modifyText("perhaps you can help. Bandits have overtaken some of", 243, 5);
                    p.getPackets().modifyText("the empty tents in Al-Kharid, and are quite aggressive...", 243, 6);
                    p.getPackets().animateInterface(9827, 243, 2);
                    p.getPackets().sendChatboxInterface2(243);
                    newStatus = 172;
                    break;

                case 172:
                    p.getPackets().sendNPCHead(1862, 244, 2);
                    p.getPackets().modifyText("Ali Morissane", 244, 3);
                    p.getPackets().modifyText("..And that's not all! Deep under the town lies a beast,", 244, 4);
                    p.getPackets().modifyText("I hear it's a mixture of a bug and a fly...I expect it", 244, 5);
                    p.getPackets().modifyText("would take a fine warrior to defeat it and no doubt", 244, 6);
                    p.getPackets().modifyText("he would be generously rewarded.", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 173;
                    break;

                case 173:
                    p.getPackets().sendPlayerHead(65, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 65, 3);
                    p.getPackets().modifyText("Do you have a map, or directions", 65, 4);
                    p.getPackets().modifyText("to Al-Kharid?", 65, 5);
                    p.getPackets().animateInterface(9827, 65, 2);
                    p.getPackets().sendChatboxInterface2(65);
                    newStatus = 174;
                    break;

                case 174:
                    p.getPackets().sendNPCHead(1862, 243, 2);
                    p.getPackets().modifyText("Ali Morissane", 243, 3);
                    p.getPackets().modifyText("I don't i'm afraid, although I travel back reguarly and", 243, 4);
                    p.getPackets().modifyText("happen to have room for a passenger. So if you'd", 243, 5);
                    p.getPackets().modifyText("ever like to go to Al-Kharid, don't hesitate to ask me.", 243, 6);
                    p.getPackets().animateInterface(9827, 243, 2);
                    p.getPackets().sendChatboxInterface2(243);
                    newStatus = 175;
                    break;

                case 175:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Thankyou for the offer, I may take you up on it soon.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 176;
                    break;

                case 176:
                    p.getPackets().sendNPCHead(1862, 241, 2);
                    p.getPackets().modifyText("Ali Morissane", 241, 3);
                    p.getPackets().modifyText("I'll see you then!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 177:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Nothing, sorry to bother you!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }
    }
}