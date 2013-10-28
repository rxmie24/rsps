using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.minigames.barrows
{
    internal class BrokenBarrows
    {
        public BrokenBarrows()
        {
        }

        private static int BOB_ID = 519;

        private static object[][] DATA = {
		    // Fixed id, broken id, cost, name
		    new object[] {4753, 4980, 60000, "Verac helm"},
		    new object[] {4755, 4986, 100000, "Verac flail"},
		    new object[] {4757, 4992, 90000, "Verac brassard"},
		    new object[] {4759, 4998, 80000, "Verac plateskirt"},
		    new object[] {4745, 4956, 60000, "Torag helm"},
		    new object[] {4747, 4962, 100000, "Torag hammers"},
		    new object[] {4749, 4968, 90000, "Torag platebody"},
		    new object[] {4751, 4974, 80000, "Torag platelegs"},
		    new object[] {4716, 4884, 60000, "Dharok helm"},
		    new object[] {4718, 4890, 100000, "Dharok greataxe"},
		    new object[] {4720, 4896, 90000, "Dharok platebody"},
		    new object[] {4722, 4902, 80000, "Dharok platelegs"},
		    new object[] {4708, 4860, 60000, "Ahrim hood"},
		    new object[] {4710, 4866, 100000, "Ahrim staff"},
		    new object[] {4712, 4872, 90000, "Ahrim robetop"},
		    new object[] {4714, 4878, 80000, "Ahrim robeskirt"},
		    new object[] {4732, 4932, 60000, "Karil coif"},
		    new object[] {4734, 4938, 100000, "Karil crossbow"},
		    new object[] {4736, 4944, 90000, "Karil leathertop"},
		    new object[] {4738, 4950, 80000, "Karil leatherskirt"},
		    new object[] {4724, 4908, 60000, "Guthan helm"},
		    new object[] {4726, 4914, 100000, "Guthan warspear"},
		    new object[] {4728, 4920, 90000, "Guthan platebody"},
		    new object[] {4730, 4926, 80000, "Guthan chainskirt"}
	    };

        public static int getBrokenId(int fixedId)
        {
            for (int i = 0; i < DATA.Length; i++)
            {
                if (fixedId == (int)DATA[i][0])
                {
                    return (int)DATA[i][1];
                }
            }
            return fixedId;
        }

        private static int getIndex(int item)
        {
            for (int i = 0; i < DATA.Length; i++)
            {
                if (item == (int)DATA[i][1] || item == (int)DATA[i][0])
                {
                    return i;
                }
            }
            return -1;
        }

        public static void talkToBob(Player p, Npc npc, int item, int option)
        {
            p.setEntityFocus(npc.getClientIndex());
            AreaEvent talkToBobAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
            talkToBobAreaEvent.setAction(() =>
            {
                npc.setFaceLocation(p.getLocation());
                p.setFaceLocation(npc.getLocation());
                p.setEntityFocus(65535);
                if (option == 0)
                { // use item on bob
                    if (item > 0)
                    {
                        p.setTemporaryAttribute("bobsAxesBarrowItem", item);
                        showBobDialogue(p, 101);
                    }
                }
                else if (option == 1)
                { // talk
                    showBobDialogue(p, 107);
                }
                else if (option == 2)
                { // trade
                    p.setShopSession(new ShopSession(p, 4));
                }
            });
            Server.registerCoordinateEvent(talkToBobAreaEvent);
        }

        public static void showBobDialogue(Player p, int status)
        {
            p.getPackets().softCloseInterfaces();
            int index = -1;
            int newStatus = -1;
            if (p.getTemporaryAttribute("bobsAxesBarrowItem") != null)
            {
                int item = (int)p.getTemporaryAttribute("bobsAxesBarrowItem");
                index = getIndex(item);
                if (index == -1)
                {
                    return;
                }
                else if (item == (int)DATA[index][0])
                {
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("That item isn't broken..", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    return;
                }
                else if (item != (int)DATA[index][1])
                {
                    return;
                }
            }
            switch (status)
            {
                case 101:
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("That'll cost you " + ((int)DATA[index][2]).ToString("#,##0") + " gold coins to fix, are you sure?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 102;
                    break;

                case 102:
                    p.getPackets().modifyText("Yes, I'm sure!", 557, 2);
                    p.getPackets().modifyText("On second thoughts, no thanks.", 557, 3);
                    p.getPackets().sendChatboxInterface2(557);
                    newStatus = 103;
                    break;

                case 103:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Yes, I'm sure!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 104;
                    break;

                case 104:
                    if (!p.getInventory().hasItemAmount(995, (int)DATA[index][2]))
                    {
                        p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                        p.getPackets().modifyText("Bob", 241, 3);
                        p.getPackets().modifyText("You don't have enough money to pay for the repair!", 241, 4);
                        p.getPackets().animateInterface(9827, 241, 2);
                        p.getPackets().sendChatboxInterface2(241);
                        break;
                    }
                    else
                    {
                        if (!p.getInventory().hasItem((int)DATA[index][1]))
                        {
                            p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                            p.getPackets().modifyText("Bob", 241, 3);
                            p.getPackets().modifyText("The item seems to have gone from your inventory.", 241, 4);
                            p.getPackets().animateInterface(9827, 241, 2);
                            p.getPackets().sendChatboxInterface2(241);
                            break;
                        }
                        else if (p.getInventory().deleteItem(995, (int)DATA[index][2]))
                        {
                            p.getInventory().replaceSingleItem((int)DATA[index][1], (int)DATA[index][0]);
                            p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                            p.getPackets().modifyText("Bob", 241, 3);
                            p.getPackets().modifyText("There you go, happy doing business with you!", 241, 4);
                            p.getPackets().animateInterface(9827, 241, 2);
                            p.getPackets().sendChatboxInterface2(241);
                            p.getPackets().sendMessage("You pay Bob his fee and he repairs your " + (string)DATA[index][3] + ".");
                            break;
                        }
                    }
                    break;

                case 105:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("On second thoughts, no thanks.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 106;
                    break;

                case 106:
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("Ok, but don't expect my prices to change anytime soon!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 107:
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("Hello there " + p.getLoginDetails().getUsername() + ", what can i do for you?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 108;
                    break;

                case 108:
                    p.getPackets().modifyText("Could you please repair my Barrow item?", 230, 2);
                    p.getPackets().modifyText("I'm interested in buying an axe.", 230, 3);
                    p.getPackets().modifyText("Nevermind.", 230, 4);
                    p.getPackets().sendChatboxInterface2(230);
                    newStatus = 109;
                    break;

                case 109:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you please repair my Barrow item?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 110;
                    break;

                case 110:
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("Certainly! Show me the item and i'll see what i can do.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 111:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'm interested in buying an axe.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 112;
                    break;

                case 112:
                    p.getPackets().sendNPCHead(BOB_ID, 241, 2);
                    p.getPackets().modifyText("Bob", 241, 3);
                    p.getPackets().modifyText("What a coincidence! Axes are my speciality!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 113;
                    break;

                case 113:
                    //TODO open bob's shop
                    break;

                case 114:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Nevermind.", 64, 4);
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