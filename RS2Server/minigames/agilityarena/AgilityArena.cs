using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.agility;
using RS2.Server.util;

namespace RS2.Server.minigames.agilityarena
{
    internal class AgilityArena : AgilityData
    {
        private static int currentPillar;

        public AgilityArena()
        {
            startArena();
        }

        private void startArena()
        {
            Event startArenaEvent = new Event(0);
            startArenaEvent.setAction(() =>
            {
                currentPillar = Misc.random(AGILITY_ARENA_PILLARS.Length - 1);
                updateArrow();
                startArenaEvent.setTick(30000 + Misc.random(30000));
            });
            Server.registerEvent(startArenaEvent);
        }

        public static void tagPillar(Player p, int pillarIndex)
        {
            AreaEvent tagPillarAreaEvent = new AreaEvent(p, AGILITY_ARENA_PILLARS[pillarIndex][1] - 1, AGILITY_ARENA_PILLARS[pillarIndex][2] - 1, AGILITY_ARENA_PILLARS[pillarIndex][1] + 1, AGILITY_ARENA_PILLARS[pillarIndex][2] + 1);
            tagPillarAreaEvent.setAction(() =>
            {
                if (p.getLocation().getZ() == 3)
                {
                    p.setFaceLocation(new Location(AGILITY_ARENA_PILLARS[pillarIndex][1], AGILITY_ARENA_PILLARS[pillarIndex][2], 3));
                    if (pillarIndex != currentPillar)
                    {
                        p.getPackets().sendMessage("You can only get a ticket when the flashing arrow is above the pillar!");
                        return;
                    }
                    if (p.isTaggedLastAgilityPillar())
                    {
                        p.getPackets().sendMessage("You have already tagged this pillar, wait until the arrow moves again.");
                        return;
                    }
                    int currentStatus = p.getAgilityArenaStatus();
                    if (currentStatus == 0)
                    {
                        p.getPackets().sendConfig(309, 4);
                        p.getPackets().sendMessage("You get tickets by tagging more than one pillar in a row. Tag the next pillar!");
                    }
                    else
                    {
                        p.getInventory().addItem(2996);
                        p.getPackets().sendMessage("You recieve an Agility Arena ticket!");
                    }
                    p.setAgilityArenaStatus(currentStatus == 0 ? 1 : 1);
                    p.setTaggedLastAgilityPillar(true);
                }
            });
            Server.registerCoordinateEvent(tagPillarAreaEvent);
        }

        public static void updatePillarForPlayer(Player p)
        {
            int[] pillarVars = AGILITY_ARENA_PILLARS[currentPillar];
            p.getPackets().setArrowOnPosition(pillarVars[1], pillarVars[2], 80);
        }

        public void updateArrow()
        {
            int[] pillarVars = AGILITY_ARENA_PILLARS[currentPillar];
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (Location.atAgilityArena(p.getLocation()))
                    {
                        p.getPackets().setArrowOnPosition(pillarVars[1], pillarVars[2], 80);
                        if (p.getAgilityArenaStatus() > 0)
                        {
                            if (!p.isTaggedLastAgilityPillar())
                            {
                                p.setAgilityArenaStatus(0);
                                p.getPackets().sendConfig(309, 0);
                            }
                        }
                        p.setTaggedLastAgilityPillar(false);
                    }
                }
            }
        }

        public static void enterArena(Player p, int objectX, int objectY)
        {
            CoordinateEvent enterArenaCoordinateEvent = new CoordinateEvent(p, new Location(2809, 3193, 0));
            enterArenaCoordinateEvent.setAction(() =>
            {
                if (!p.hasPaidAgilityArena())
                {
                    p.getPackets().sendMessage("You must pay Cap'n Izzy the entrance fee before you can enter the Agility Arena.");
                    return;
                }
                p.setLastAnimation(new Animation(827));
                Event teleportArenaEvent = new Event(1000);
                teleportArenaEvent.setAction(() =>
                {
                    p.setPaidAgilityArena(false);
                    p.teleport(new Location(2805, 9589, 3));
                    teleportArenaEvent.stop();
                });
                Server.registerEvent(teleportArenaEvent);
            });
            Server.registerCoordinateEvent(enterArenaCoordinateEvent);
        }

        public static void exitArena(Player p, int objectX, int objectY)
        {
            CoordinateEvent exitArenaCoordinateEvent = new CoordinateEvent(p, new Location(2805, 9589, 3));
            exitArenaCoordinateEvent.setAction(() =>
            {
                p.setLastAnimation(new Animation(828));
                Event exitArenaEvent = new Event(1000);
                exitArenaEvent.setAction(() =>
                {
                    p.teleport(new Location(2809, 3193, 0));
                    exitArenaEvent.stop();
                });
                Server.registerEvent(exitArenaEvent);
            });
            Server.registerCoordinateEvent(exitArenaCoordinateEvent);
        }

        public static bool dialogue(Player p, Npc npc, bool rightClickPay)
        {
            if ((npc.getId() != 1055 && npc.getId() != 437) || (rightClickPay && npc.getId() != 437))
            {
                return false;
            }
            p.setEntityFocus(npc.getClientIndex());
            AreaEvent dialogueAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
            dialogueAreaEvent.setAction(() =>
            {
                npc.setFaceLocation(p.getLocation());
                int status = npc.getId() == 1055 ? 43 : 1;
                if (rightClickPay)
                {
                    if (!p.getInventory().hasItemAmount(995, AGILITY_ARENA_PRICE))
                    {
                        p.getPackets().sendMessage("You don't have enough money to pay the entrance fee.");
                        return;
                    }
                    status = 29;
                }
                doDialogue(p, status);
            });
            Server.registerCoordinateEvent(dialogueAreaEvent);
            return true;
        }

        public static void doDialogue(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            switch (status)
            {
                case 1:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Ahoy Cap'n!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 2;
                    break;

                case 2:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("Ahoy there!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 3;
                    break;

                case 3:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("Avast ye scurvy swabs!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 4;
                    break;

                case 4:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Huh?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 5;
                    break;

                case 5:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("Don't mind me parrot, he's Cracked Jenny's Tea Cup!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 6;
                    break;

                case 6:
                    p.getPackets().sendChatboxInterface2(235);
                    p.getPackets().modifyText("What is this place?", 235, 2);
                    p.getPackets().modifyText("What do i do in the arena?", 235, 3);
                    p.getPackets().modifyText("I'd like to use the Agility Arena, please.", 235, 4);
                    p.getPackets().modifyText("Could you sell me a Skillcape of Agility?.", 235, 5);
                    p.getPackets().modifyText("See you later.", 235, 6);
                    newStatus = 7;
                    break;

                case 7:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("What is this place?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 8;
                    break;

                case 8:
                    p.getPackets().sendNPCHead(437, 242, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 242, 3);
                    p.getPackets().modifyText("This, me hearty, is the entrance to the Brimhaven", 242, 4);
                    p.getPackets().modifyText("Agility Arena!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 9;
                    break;

                case 9:
                    p.getPackets().sendNPCHead(437, 242, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 242, 3);
                    p.getPackets().modifyText("I were diggin for buried treasure when I found it!", 242, 4);
                    p.getPackets().modifyText("Amazed I was! It was a sight to behold!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 10;
                    break;

                case 10:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("Buried treasure!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 11;
                    break;

                case 11:
                    p.getPackets().sendNPCHead(437, 242, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 242, 3);
                    p.getPackets().modifyText("It were the biggest thing i'd ever seen! It must've been", 242, 4);
                    p.getPackets().modifyText("atleast a league from side to side!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 12;
                    break;

                case 12:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("It made me list, I were that excited!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 13;
                    break;

                case 13:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("Get on with it!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 14;
                    break;

                case 14:
                    p.getPackets().sendNPCHead(437, 244, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 244, 3);
                    p.getPackets().modifyText("I'd found a huge cave with all these platforms. I reckon", 244, 4);
                    p.getPackets().modifyText("it be an ancient civilisation that made it. I had to be", 244, 5);
                    p.getPackets().modifyText("mighty careful as there was these traps everywhere!", 244, 6);
                    p.getPackets().modifyText("Dangerous it was!", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 15;
                    break;

                case 15:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("Danger! Danger!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 16;
                    break;

                case 16:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("Entrance is only " + AGILITY_ARENA_PRICE.ToString("#,##0") + " coins!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 6;
                    break;

                case 17:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("What do I do in the arena?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 18;
                    break;

                case 18:
                    p.getPackets().sendNPCHead(437, 244, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 244, 3);
                    p.getPackets().modifyText("Well, me hearty, it's simple. Ye can cross between two", 244, 4);
                    p.getPackets().modifyText("platforms by using the traps or obstacles strung across", 244, 5);
                    p.getPackets().modifyText("'em. Try and make your way to the pillar that is", 244, 6);
                    p.getPackets().modifyText("indicated by the flashing arrow.", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 19;
                    break;

                case 19:
                    p.getPackets().sendNPCHead(437, 243, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 243, 3);
                    p.getPackets().modifyText("Ye receive tickets for tagging more than one pillar in a", 243, 4);
                    p.getPackets().modifyText("row. So ye won't get a ticket from the first pillar but", 243, 5);
                    p.getPackets().modifyText("ye will for every platform ye tag in a row after that.", 243, 6);
                    p.getPackets().animateInterface(9827, 243, 2);
                    p.getPackets().sendChatboxInterface2(243);
                    newStatus = 20;
                    break;

                case 20:
                    p.getPackets().sendNPCHead(437, 244, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 244, 3);
                    p.getPackets().modifyText("If ye miss a platform ye will miss out on the next ticket", 244, 4);
                    p.getPackets().modifyText("so try and get every platform you can! When ye be", 244, 5);
                    p.getPackets().modifyText("done, take the tickets to Jackie over there and she'll", 244, 6);
                    p.getPackets().modifyText("exchange them for experience or items.", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 21;
                    break;

                case 21:
                    p.getPackets().sendNPCHead(4535, 242, 1);
                    p.getPackets().modifyText("Parrot", 242, 3);
                    p.getPackets().modifyText("Tag when green light means tickets!", 242, 4);
                    p.getPackets().modifyText("Tag when red light means green light!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 1);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 22;
                    break;

                case 22:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("Thanks me hearty!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 23;
                    break;

                case 23:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Thanks!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 6;
                    break;

                case 24:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like to use the Agility Arena, please.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 25;
                    break;

                case 25:
                    string message = "";
                    if (p.hasPaidAgilityArena())
                    {
                        message = "Ye've already paid, so down ye goes...";
                    }
                    else
                    {
                        message = "Aye, Entrance be " + AGILITY_ARENA_PRICE.ToString("#,##0") + " coins.";
                        newStatus = 26;
                    }
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText(message, 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 26:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("Pieces of eight!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 27;
                    break;

                case 27:
                    p.getPackets().sendNPCHead(437, 242, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 242, 3);
                    p.getPackets().modifyText("A word of warning me hearty! There are dangerous", 242, 4);
                    p.getPackets().modifyText("traps down there!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 28;
                    break;

                case 28:
                    if (!p.getInventory().hasItemAmount(995, AGILITY_ARENA_PRICE))
                    {
                        p.getPackets().sendPlayerHead(64, 2);
                        p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                        p.getPackets().modifyText("I don't have enough money on me at the moment..", 64, 4);
                        p.getPackets().animateInterface(9827, 64, 2);
                        p.getPackets().sendChatboxInterface2(64);
                        newStatus = 31;
                        break;
                    }
                    p.getPackets().modifyText("Ok, here's " + AGILITY_ARENA_PRICE.ToString("#,##0") + " coins.", 228, 2);
                    p.getPackets().modifyText("Never mind.", 228, 3);
                    p.getPackets().sendChatboxInterface(228);
                    newStatus = 29;
                    break;

                case 29:
                    if (p.getInventory().deleteItem(995, AGILITY_ARENA_PRICE))
                    {
                        p.setPaidAgilityArena(true);
                    }
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Ok, here's " + AGILITY_ARENA_PRICE.ToString("#,##0") + " coins.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 30;
                    break;

                case 30:
                    p.getPackets().sendMessage("You pay Cap'n Izzy " + AGILITY_ARENA_PRICE.ToString("#,##0") + " coins.");
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("May the wind be in ye sails!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 31:
                    p.getPackets().sendNPCHead(4535, 241, 1);
                    p.getPackets().modifyText("Parrot", 241, 3);
                    p.getPackets().modifyText("*Squawk*", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 32;
                    break;

                case 32:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("No coins, no entrance!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 33:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Never mind.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 34:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could you sell me a Skillcape of Agility?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 35;
                    break;

                case 35:
                    p.getPackets().sendNPCHead(437, 244, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 244, 3);
                    if (p.getSkills().getMaxLevel(Skills.SKILL.AGILITY) < 99)
                    {
                        p.getPackets().modifyText("Unfortunatly not. I may only sell the Skillcape of Agility", 244, 4);
                        p.getPackets().modifyText("to those that have conquered the obstacles of Runescape,", 244, 5);
                        p.getPackets().modifyText("can climb like a cat and run like the wind! which err..", 244, 6);
                        p.getPackets().modifyText("isnt you. Is there anything else?", 244, 7);
                        newStatus = 6;
                    }
                    else
                    {
                        p.getPackets().modifyText("Indeed! You have reached level 99 Agility and have", 244, 4);
                        p.getPackets().modifyText("become a master of dexterity. It would be a pleasure", 244, 5);
                        p.getPackets().modifyText("to sell you an Agility skillcape and hood for a sum of", 244, 6);
                        p.getPackets().modifyText(SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins.", 244, 7);
                        newStatus = 36;
                    }
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    break;

                case 36:
                    p.getPackets().modifyText("I'll pay " + SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins.", 228, 2);
                    p.getPackets().modifyText(SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " is a crazy price!", 228, 3);
                    p.getPackets().sendChatboxInterface(228);
                    newStatus = 37;
                    break;

                case 37:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'll pay " + SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 38;
                    break;

                case 38:
                    if (p.getInventory().getTotalFreeSlots() < 2)
                    {
                        p.getPackets().sendNPCHead(437, 241, 2);
                        p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                        p.getPackets().modifyText("Ye require 2 free inventory spaces!", 241, 4);
                        p.getPackets().animateInterface(9827, 241, 2);
                        p.getPackets().sendChatboxInterface2(241);
                        break;
                    }
                    if (p.getInventory().deleteItem(995, SkillHandler.SKILLCAPE_PRICE))
                    {
                        int cape = p.getSkills().hasMultiple99s() ? 9772 : 9771;
                        int hood = 9773;
                        p.getInventory().addItem(cape);
                        p.getInventory().addItem(hood);
                        p.getPackets().sendNPCHead(437, 242, 2);
                        p.getPackets().modifyText("Cap'n Izzy No-Beard", 242, 3);
                        p.getPackets().modifyText("One Agility Skillcape & hood.", 242, 4);
                        p.getPackets().modifyText("Wear it with pride.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead(437, 241, 2);
                        p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                        p.getPackets().modifyText("Ye don't have enough coins!", 241, 4);
                        p.getPackets().animateInterface(9827, 241, 2);
                        p.getPackets().sendChatboxInterface2(241);
                    }
                    break;

                case 39:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText(SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " is a crazy price!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 40;
                    break;

                case 40:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("I'm sure ye will change your mind eventually..", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 41:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("See you later.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 42;
                    break;

                case 42:
                    p.getPackets().sendNPCHead(437, 241, 2);
                    p.getPackets().modifyText("Cap'n Izzy No-Beard", 241, 3);
                    p.getPackets().modifyText("Aye, goodbye " + p.getLoginDetails().getUsername() + ".", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                /*
                 * NOW TALKING TO JACKIE THE FRUIT
                 */

                case 43:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Ahoy there!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 44;
                    break;

                case 44:
                    p.getPackets().sendNPCHead(1055, 241, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 241, 3);
                    p.getPackets().modifyText("Ahoy!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 45;
                    break;

                case 45:
                    p.getPackets().sendChatboxInterface2(232);
                    p.getPackets().modifyText("What is this place?", 232, 2);
                    p.getPackets().modifyText("What do you do?", 232, 3);
                    p.getPackets().modifyText("I'd like to trade in my tickets, please.", 232, 4);
                    p.getPackets().modifyText("See you later.", 232, 5);
                    newStatus = 46;
                    break;

                case 46:
                    p.getPackets().sendNPCHead(1055, 241, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 241, 3);
                    p.getPackets().modifyText("Welcome to the Brimhaven Agility Arena!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 47;
                    break;

                case 47:
                    p.getPackets().sendNPCHead(1055, 242, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 242, 3);
                    p.getPackets().modifyText("If ye want to know more, talk to Cap'n Izzy, after", 242, 4);
                    p.getPackets().modifyText("all... he did find it!", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 45;
                    break;

                case 48:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("What do you do?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 49;
                    break;

                case 49:
                    p.getPackets().sendNPCHead(1055, 244, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 244, 3);
                    p.getPackets().modifyText("I be the jack o' tickets. I exchange the tickets ye", 244, 4);
                    p.getPackets().modifyText("Collect in the Agility arena for more stuff. Ye can", 244, 5);
                    p.getPackets().modifyText("obtain more Agility experience or items ye won't", 244, 6);
                    p.getPackets().modifyText("find anywhere else!", 244, 7);
                    p.getPackets().animateInterface(9827, 244, 2);
                    p.getPackets().sendChatboxInterface2(244);
                    newStatus = 50;
                    break;

                case 50:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Sounds good!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 45;
                    break;

                case 51:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like to trade in my tickets, please.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 52;
                    break;

                case 52:
                    p.getPackets().sendNPCHead(1055, 241, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 241, 3);
                    p.getPackets().modifyText("Aye, ye be on the right track.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 53;
                    break;

                case 53:
                    p.getPackets().displayInterface(6);
                    break;

                case 54:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("See you later.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 55;
                    break;

                case 55:
                    p.getPackets().sendNPCHead(1055, 241, 2);
                    p.getPackets().modifyText("Pirate Jackie the Fruit", 241, 3);
                    p.getPackets().modifyText("Goodbye.", 241, 4);
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