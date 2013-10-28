using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.minigames.warriorguild
{
    internal class WarriorGuild : WarriorGuildData
    {
        public WarriorGuild()
        {
        }

        public static bool useAnimator(Player p, int itemId, int objectId, int objectX, int objectY)
        {
            if (objectId != ANIMATOR_ID)
            {
                return false;
            }
            int animatorIndex = -1;
            for (int i = 0; i < ANIMATOR_LOCATIONS.Length; i++)
            {
                if (objectX == ANIMATOR_LOCATIONS[i][0] && objectY == ANIMATOR_LOCATIONS[i][1])
                {
                    animatorIndex = i;
                    break;
                }
            }
            if (animatorIndex == -1)
            {
                return false;
            }
            p.setTemporaryAttribute("warriorGuildAnimator", animatorIndex);
            CoordinateEvent createAnimatedArmorCoordinateEvent = new CoordinateEvent(p, new Location(ANIMATOR_LOCATIONS[animatorIndex][0], (ANIMATOR_LOCATIONS[animatorIndex][1] + 1), 0));
            createAnimatedArmorCoordinateEvent.setAction(() =>
            {
                int armourIndex = hasArmour(p, itemId);
                if (armourIndex != -1)
                {
                    createdAnimatedArmour(p, armourIndex);
                }
            });
            Server.registerCoordinateEvent(createAnimatedArmorCoordinateEvent);
            return true;
        }

        protected static void createdAnimatedArmour(Player p, int index)
        {
            if (p.getTemporaryAttribute("warriorGuildAnimator") == null)
            {
                return;
            }
            p.setLastAnimation(new Animation(827));
            p.setTemporaryAttribute("unmovable", true);
            for (int i = 0; i < ARMOUR_SETS[index].Length; i++)
            {
                p.getInventory().deleteItem(ARMOUR_SETS[index][i]);
            }
            p.getPackets().sendChatboxInterface(211);
            p.getPackets().modifyText("You place the armour onto the platform where it", 211, 1);
            p.getPackets().modifyText("dissapears...", 211, 2);
            int animatorIndex = (int)p.getTemporaryAttribute("warriorGuildAnimator");
            Event createAnimatedArmourEvent = new Event(1500);
            int createAnimatedArmourCounter = 0;
            Npc npc = null;
            createAnimatedArmourEvent.setAction(() =>
            {
                if (createAnimatedArmourCounter == 0)
                {
                    p.getPackets().sendChatboxInterface(211);
                    p.getPackets().modifyText("The animator hums, something appears to be working.", 211, 1);
                    p.getPackets().modifyText("You stand back.", 211, 2);
                    createAnimatedArmourEvent.setTick(500);
                }
                else if (createAnimatedArmourCounter == 1)
                {
                    p.getWalkingQueue().forceWalk(0, +3);
                    createAnimatedArmourEvent.setTick(2000);
                }
                else if (createAnimatedArmourCounter == 2)
                {
                    createAnimatedArmourEvent.setTick(500);
                    Location minCoords = new Location(2849, 3534, 0);
                    Location maxCoords = new Location(2861, 3545, 0);
                    npc = new Npc(ANIMATED_ARMOUR[index]);
                    npc.setMinimumCoords(minCoords);
                    npc.setMaximumCoords(maxCoords);
                    npc.setLocation(new Location(ANIMATOR_LOCATIONS[animatorIndex][0], ANIMATOR_LOCATIONS[animatorIndex][1], 0));
                    npc.setWalkType(WalkType.STATIC);
                    npc.setForceText("I'm ALIVE!");
                    npc.setLastAnimation(new Animation(4166));
                    npc.setEntityFocus(p.getClientIndex());
                    npc.setOwner(p);
                    npc.setTarget(p);
                    p.getPackets().setArrowOnEntity(1, npc.getClientIndex());
                    Server.getNpcList().Add(npc);
                }
                else
                {
                    p.setEntityFocus(npc.getClientIndex());
                    p.getPackets().softCloseInterfaces();
                    createAnimatedArmourEvent.stop();
                    p.removeTemporaryAttribute("unmovable");
                    npc.getFollow().setFollowing(p);
                }
                createAnimatedArmourCounter++;
            });
            Server.registerEvent(createAnimatedArmourEvent);
        }

        protected static int hasArmour(Player p, int itemId)
        {
            int itemIndex = -1;
            for (int i = 0; i < ARMOUR_SETS.Length; i++)
            {
                for (int j = 0; j < ARMOUR_SETS[i].Length; j++)
                {
                    if (itemId == ARMOUR_SETS[i][j])
                    {
                        itemIndex = i;
                        for (int k = 0; k < ARMOUR_SETS[i].Length; k++)
                        {
                            if (!p.getInventory().hasItem(ARMOUR_SETS[i][k]))
                            {
                                p.getPackets().sendMessage("You do not have a complete set of " + ARMOUR_TYPE[i] + " armour!");
                                return -1;
                            }
                        }
                        break;
                    }
                }
            }
            return itemIndex;
        }

        public static bool talkToWarriorGuildNPC(Player p, Npc n, int slot)
        {
            if (n.getId() != 4289)
            {
                return false;
            }
            p.setEntityFocus(n.getClientIndex());
            int npcX = n.getLocation().getX();
            int npcY = n.getLocation().getY();
            AreaEvent talkToWarriorGuildNPCAreaEvent = new AreaEvent(p, npcX - 1, npcY - 1, npcX + 1, npcY + 1);
            talkToWarriorGuildNPCAreaEvent.setAction(() =>
            {
                p.setFaceLocation(n.getLocation());
                p.setEntityFocus(65535);
                switch (n.getId())
                {
                    case 4289: // Kamfreena
                        Dialogue.doDialogue(p, 77);
                        break;
                }
            });
            Server.registerCoordinateEvent(talkToWarriorGuildNPCAreaEvent);
            return true;
        }

        private static int getDefenderStatus(Player p)
        {
            int currentDefender = -1;
            for (int i = 0; i < DEFENDERS.Length; i++)
            {
                if (p.getInventory().hasItem(DEFENDERS[i]) || p.getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD) == DEFENDERS[i])
                {
                    currentDefender = i;
                }
            }
            if (currentDefender >= -1 && currentDefender != 6)
            {
                currentDefender++;
            }
            return currentDefender;
        }

        public static void talkToKamfreena(Player p, int status)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            switch (status)
            {
                case 77:
                    p.getPackets().sendNPCHead(4289, 241, 2);
                    p.getPackets().modifyText("Kamfreena", 241, 3);
                    p.getPackets().modifyText("Hello! Can I help you?.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 78;
                    break;

                case 78:
                    p.getPackets().modifyText("I'd like to kill some Cyclops please.", 228, 2);
                    p.getPackets().modifyText("Never mind, sorry to bother you.", 228, 3);
                    p.getPackets().sendChatboxInterface(228);
                    newStatus = 79;
                    break;

                case 79:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like to kill some Cyclops please.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 81;
                    break;

                case 80:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Never mind, sorry to bother you.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 81:
                    if (!p.getInventory().hasItemAmount(8851, 100))
                    {
                        p.getPackets().sendNPCHead(4289, 242, 2);
                        p.getPackets().modifyText("Kamfreena", 242, 3);
                        p.getPackets().modifyText("You require a minimum of 100 tokens in order", 242, 4);
                        p.getPackets().modifyText("to be able to enter the Cyclops' room.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                        break;
                    }
                    else
                    {
                        int currentDefenderStatus = getDefenderStatus(p);
                        int lastDefenderStatus = p.getDefenderWave();
                        p.setDefenderWave(currentDefenderStatus);
                        string s = currentDefenderStatus != lastDefenderStatus ? " now " : " ";
                        p.getPackets().sendNPCHead(4289, 242, 2);
                        p.getPackets().modifyText("Kamfreena", 242, 3);
                        p.getPackets().modifyText("Very well. The Cyclops will" + s + "drop:", 242, 4);
                        p.getPackets().modifyText(ItemData.forId(DEFENDERS[currentDefenderStatus]).getName() + ".", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                        newStatus = 82;
                    }
                    break;

                case 82:
                    if (p.getDefenderWave() < 6)
                    {
                        p.getPackets().sendNPCHead(4289, 242, 2);
                        p.getPackets().modifyText("Kamfreena", 242, 3);
                        p.getPackets().modifyText("Be sure to speak to me once you have retrieved one", 242, 4);
                        p.getPackets().modifyText("if you wish to advance!", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead(4289, 242, 2);
                        p.getPackets().modifyText("Kamfreena", 242, 3);
                        p.getPackets().modifyText("Since Rune is the highest Defender available, you don't", 242, 4);
                        p.getPackets().modifyText("need to speak to me once you have retrieved one.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }
    }
}