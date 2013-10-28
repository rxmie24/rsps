using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;
using System;

namespace RS2.Server.player.skills.slayer
{
    internal class Slayer : SlayerData
    {
        public Slayer()
        {
        }

        public static bool talkToMaster(Player p, Npc npc)
        {
            for (int i = 0; i < SLAYER_MASTERS.Length; i++)
            {
                if (npc.getId() == (int)SLAYER_MASTERS[i][0])
                {
                    int j = i;
                    p.setEntityFocus(npc.getClientIndex());
                    AreaEvent talkToMasterAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
                    talkToMasterAreaEvent.setAction(() =>
                    {
                        p.setTemporaryAttribute("slayerMaster", j);
                        npc.setFaceLocation(p.getLocation());
                        p.setFaceLocation(npc.getLocation());
                        p.setEntityFocus(65535);
                        doDialogue(p, 1000);
                    });
                    Server.registerCoordinateEvent(talkToMasterAreaEvent);
                    return true;
                }
            }
            return false;
        }

        private static void displayTip(Player p, int index)
        {
            if (p.getSlayerTask() != null)
            {
                p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                p.getPackets().modifyText("They can be found somewhere on karamja m8", 242, 4);
                p.getPackets().modifyText("want Vannaka in Edgeville to assign you a task?", 242, 5);
                p.getPackets().animateInterface(9827, 242, 2);
                p.getPackets().sendChatboxInterface2(242);
                return;
            }
            p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
            p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
            p.getPackets().modifyText("You don't have a task, speak to a Slayer master to get one.", 241, 4);
            p.getPackets().animateInterface(9827, 241, 2);
            p.getPackets().sendChatboxInterface2(241);
        }

        private static string getTask(Player p, int index)
        {
            object[][] data = getMasterData(index);
            if (data == null)
            {
                p.getPackets().closeInterfaces();
                p.getPackets().sendMessage("An error occured, please talk to the Slayer master again.");
                return "";
            }
            int monsterIdx = (int)Misc.random(data.Length - 1);
            for (; ; )
            {
                string name = (string)data[monsterIdx][0];
                if (taskEnabled(p, name))
                {
                    break;
                }
                else
                {
                    monsterIdx = (int)Misc.random(data.Length - 1);
                    break;
                }
            }
            int monsterAmt = (int)SLAYER_MASTERS[index][3] + Misc.random((int)SLAYER_MASTERS[index][4]);
            SlayerTask task = new SlayerTask(index, monsterIdx, monsterAmt);
            p.setSlayerTask(task);
            return (string)(monsterAmt + " " + data[monsterIdx][0]);
        }

        public static bool taskEnabled(Player p, string monsterName)
        {
            for (int i = 0; i < p.getRemovedSlayerTasks().Length; i++)
            {
                if (monsterName.Equals(p.getRemovedSlayerTasks()[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static object[][] getMasterData(int index)
        {
            object[][] data = null;
            switch (index)
            {
                case 0:
                    data = TURAEL_TASKS;
                    break;

                case 1:
                    data = MAZCHNA_TASKS;
                    break;

                case 2:
                    data = VANNAKA_TASKS;
                    break;

                case 3:
                    data = CHAELDAR_TASKS;
                    break;

                case 4:
                    data = DURADEL_TASKS;
                    break;
            }
            return data;
        }

        public static void checkSlayerKill(Player p, Npc npc)
        {
            if (p.getSlayerTask() == null)
            {
                return;
            }
            SlayerTask task = p.getSlayerTask();
            object[][] data = getMasterData(task.getMasterIndex());
            for (int i = 1; i < data[task.getMonsterIndex()].Length; i++)
            {
                if (npc.getId() == (int)data[task.getMonsterIndex()][i])
                {
                    if (npc.getKiller().Equals(p))
                    {
                        p.getSkills().addXp(Skills.SKILL.SLAYER, npc.getMaxHp());//(double) data[task.getMonsterIndex()][1]);
                        checkIfCompleteTask(p, task);
                        break;
                    }
                }
            }
        }

        private static void checkIfCompleteTask(Player p, SlayerTask task)
        {
            task.setAmount(task.getAmount() - 1);
            if (task.getAmount() <= 0)
            {
                doDialogue(p, 1062);
                p.getPackets().sendMessage("You have completed your Slayer task, please return to a Slayer master.");
                p.setSlayerTask(null);
            }
        }

        public static bool openSlayerShop(Player p, Npc npc)
        {
            int id = npc.getId();
            if (id != 8273 && id != 1597 && id != 8274 && id != 1598 && id != 8275 || p.isDead())
            {
                return false;
            }
            p.setEntityFocus(npc.getClientIndex());
            AreaEvent openSlayerShopAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
            openSlayerShopAreaEvent.setAction(() =>
            {
                p.setFaceLocation(npc.getLocation());
                npc.setFaceLocation(p.getLocation());
                p.setShopSession(new ShopSession(p, 2));
            });
            Server.registerCoordinateEvent(openSlayerShopAreaEvent);
            return true;
        }

        public static bool openPointsInterface(Player p, Npc npc)
        {
            int id = npc.getId();
            if (id != 8273 && id != 1597 && id != 8274 && id != 1598 && id != 8275 || p.isDead())
            {
                return false;
            }
            p.setEntityFocus(npc.getClientIndex());
            AreaEvent openPointsInterfaceAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
            openPointsInterfaceAreaEvent.setAction(() =>
            {
                p.setEntityFocus(65535);
                npc.setFaceLocation(p.getLocation());
                displayPoints(p, 1);
            });
            Server.registerCoordinateEvent(openPointsInterfaceAreaEvent);
            return true;
        }

        protected static void displayPoints(Player p, int status)
        {
            switch (status)
            {
                case 1:
                    int[] childIds = { 35, 30, 31, 32 };
                    for (int i = 0; i < p.getRemovedSlayerTasks().Length; i++)
                    {
                        p.getPackets().modifyText(p.getRemovedSlayerTasks()[i], 161, childIds[i]);
                    }
                    p.getPackets().modifyText(" " + p.getSlayerPoints(), 161, 19);
                    p.getPackets().modifyText("(" + REASSIGN_POINTS + " points)", 161, 26);
                    p.getPackets().modifyText("(" + PERM_REMOVE_POINTS + " points)", 161, 27);
                    p.getPackets().displayInterface(161);
                    break;

                case 2:
                    p.getPackets().modifyText("" + p.getSlayerPoints(), 163, 18);
                    p.getPackets().displayInterface(163);
                    break;

                case 3:
                    p.getPackets().modifyText("" + p.getSlayerPoints(), 164, 20);
                    p.getPackets().modifyText("(" + BUY_XP_POINTS + " points)", 164, 32);
                    p.getPackets().modifyText("(" + BUY_RING_POINTS + " points)", 164, 33);
                    p.getPackets().modifyText("(" + BUY_DART_POINTS + " points)", 164, 34);
                    p.getPackets().modifyText("(" + BUY_BOLT_POINTS + " points)", 164, 35);
                    p.getPackets().modifyText("(" + BUY_ARROW_POINTS + " points)", 164, 36);
                    p.getPackets().displayInterface(164);
                    break;
            }
        }

        public static void handlePointsInterface(Player p, int interfaceId, int buttonId)
        {
            int currentPoints = p.getSlayerPoints();
            int j = 0;
            switch (interfaceId)
            {
                case 161:
                    switch (buttonId)
                    {
                        case 14: // "Learn" button.
                            //displayPoints(p, 2);
                            p.getPackets().sendMessage("That option is unavailable.");
                            break;

                        case 15: // "Buy" button.
                            displayPoints(p, 3);
                            break;

                        case 23: // reassign current mission.
                            if (currentPoints < REASSIGN_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + REASSIGN_POINTS + " Slayer points to reassign your Slayer task.");
                                break;
                            }
                            if (p.getSlayerTask() == null)
                            {
                                p.getPackets().sendMessage("You are not currently assigned a Slayer task.");
                                break;
                            }
                            SlayerTask task = p.getSlayerTask();
                            p.setTemporaryAttribute("slayerMaster", task.getMasterIndex());
                            p.getPackets().closeInterfaces();
                            p.setSlayerPoints(currentPoints - REASSIGN_POINTS);
                            p.getPackets().sendMessage("You trade " + REASSIGN_POINTS + " Slayer points and recieve a new Slayer task.");
                            doDialogue(p, 1007);
                            break;

                        case 24: // Permanently remove current task.
                            if (currentPoints < PERM_REMOVE_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + PERM_REMOVE_POINTS + " Slayer points to be able to ignore a task.");
                                break;
                            }
                            if (p.getSlayerTask() == null)
                            {
                                p.getPackets().sendMessage("You are not currently assigned a Slayer task.");
                                break;
                            }
                            bool freeSpace = false;
                            for (int i = 0; i < p.getRemovedSlayerTasks().Length; i++)
                            {
                                if (p.getRemovedSlayerTasks()[i].ToLower().Equals("-"))
                                {
                                    SlayerTask taskk = p.getSlayerTask();
                                    object[][] data = getMasterData(taskk.getMasterIndex());
                                    p.setSlayerPoints(currentPoints - PERM_REMOVE_POINTS);
                                    p.getRemovedSlayerTasks()[i] = (string)data[taskk.getMonsterIndex()][0];
                                    p.getPackets().sendMessage("You trade " + PERM_REMOVE_POINTS + " Slayer points to prevent future " + (string)data[taskk.getMonsterIndex()][0] + " Slayer tasks.");
                                    p.getPackets().sendMessage("Your Slayer task has been removed, please speak to a Slayer master for a new one.");
                                    p.setSlayerTask(null);
                                    freeSpace = true;
                                    sortRemovedTasks(p);
                                    break;
                                }
                            }
                            if (!freeSpace)
                            {
                                p.getPackets().sendMessage("You have reached the limit of Slayer tasks which can be removed.");
                                break;
                            }
                            displayPoints(p, 1);
                            break;

                        case 36: // Removed task 1.
                            j = 0;
                            cancelRemovedTask(p, j);
                            break;

                        case 37: // Removed task 2.
                            j = 1;
                            cancelRemovedTask(p, j);
                            break;

                        case 38: // Removed task 3.
                            j = 2;
                            cancelRemovedTask(p, j);
                            break;

                        case 39: // Removed task 4.
                            j = 3;
                            cancelRemovedTask(p, j);
                            break;
                    }
                    break;

                case 163:
                    switch (buttonId)
                    {
                        case 14: // "Assignment" button.
                            displayPoints(p, 1);
                            break;

                        case 15: // "Buy" button.
                            displayPoints(p, 3);
                            break;
                    }
                    break;

                case 164:
                    switch (buttonId)
                    {
                        case 17: // "Assignment" button.
                            displayPoints(p, 1);
                            break;

                        case 16: // "Learn" button.
                            //displayPoints(p, 2);
                            p.getPackets().sendMessage("That option is unavailable.");
                            break;

                        case 24: // Buy Slayer XP.
                            if (currentPoints < BUY_XP_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + BUY_XP_POINTS + " Slayer points to buy Slayer XP.");
                                break;
                            }
                            p.getPackets().closeInterfaces();
                            p.setSlayerPoints(currentPoints - BUY_XP_POINTS);
                            p.getPackets().sendMessage("You trade " + BUY_XP_POINTS + " of your Slayer points for 10,000 Slayer XP.");
                            p.getSkills().addXp(Skills.SKILL.SLAYER, 10000);
                            break;

                        case 26: // Buy Slaying ring.
                            if (currentPoints < BUY_RING_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + BUY_RING_POINTS + " Slayer points to buy a Ring of Slaying.");
                                break;
                            }
                            if (p.getInventory().addItem(13281))
                            {
                                p.getPackets().closeInterfaces();
                                p.setSlayerPoints(currentPoints - BUY_RING_POINTS);
                                p.getPackets().sendMessage("You trade " + BUY_RING_POINTS + " of your Slayer points for a Ring of Slaying.");
                            }
                            else
                            {
                                p.getPackets().sendMessage("You do not have enough inventory space to purchase this.");
                            }
                            break;

                        case 28: // Buy 250 slayer dart casts.
                            if (currentPoints < BUY_DART_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + BUY_DART_POINTS + " Slayer points to buy 250 Slayer dart casts.");
                                break;
                            }
                            if (p.getInventory().getTotalFreeSlots() >= 2)
                            {
                                p.getPackets().closeInterfaces();
                                p.setSlayerPoints(currentPoints - BUY_DART_POINTS);
                                p.getInventory().addItem(560, 250);
                                p.getInventory().addItem(558, 1000);
                                p.getPackets().sendMessage("You trade " + BUY_DART_POINTS + " of your Slayer points for 250 Slayer dart casts.");
                            }
                            else
                            {
                                p.getPackets().sendMessage("You do not have enough inventory space to purchase this.");
                            }
                            break;

                        case 37: // Buy 250 broad bolts
                            if (currentPoints < BUY_BOLT_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + BUY_BOLT_POINTS + " Slayer points to buy 250 Broad bolts.");
                                break;
                            }
                            if (p.getInventory().addItem(13280, 250))
                            {
                                p.getPackets().closeInterfaces();
                                p.setSlayerPoints(currentPoints - BUY_BOLT_POINTS);
                                p.getPackets().sendMessage("You trade " + BUY_BOLT_POINTS + " of your Slayer points for 250 Broad bolts.");
                            }
                            else
                            {
                                p.getPackets().sendMessage("You do not have enough inventory space to purchase this.");
                            }
                            break;

                        case 39: // Buy 250 Broad arrows
                            if (currentPoints < BUY_ARROW_POINTS)
                            {
                                p.getPackets().sendMessage("You need atleast " + BUY_ARROW_POINTS + " Slayer points to buy 250 Broad arrows.");
                                break;
                            }
                            if (p.getInventory().addItem(4160, 250))
                            {
                                p.getPackets().closeInterfaces();
                                p.setSlayerPoints(currentPoints - BUY_ARROW_POINTS);
                                p.getPackets().sendMessage("You trade " + BUY_ARROW_POINTS + " of your Slayer points for 250 Broad arrows.");
                            }
                            else
                            {
                                p.getPackets().sendMessage("You do not have enough inventory space to purchase this.");
                            }
                            break;
                    }
                    break;
            }
        }

        private static void cancelRemovedTask(Player p, int j)
        {
            if (p.getRemovedSlayerTasks()[j].Equals("-"))
            {
                return;
            }
            p.getPackets().sendMessage("You may now recieve " + p.getRemovedSlayerTasks()[j] + " Slayer tasks again.");
            p.setRemovedSlayerTask(j, "-");
            sortRemovedTasks(p);
            displayPoints(p, 1);
        }

        private static void sortRemovedTasks(Player p)
        {
            String[] tasks = p.getRemovedSlayerTasks();
            String[] tasks2 = new String[4];
            int j = 0;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].Equals("-"))
                {
                    tasks2[j] = tasks[i];
                    j++;
                }
            }
            for (int i = j; i < tasks2.Length; i++)
            {
                tasks2[i] = "-";
            }
            p.setRemovedSlayerTask(tasks2);
        }

        public static void doDialogue(Player p, int dialogueStatus)
        {
            int newStatus = -1;
            p.getPackets().softCloseInterfaces();
            if (dialogueStatus > 1050)
            {
                doGemDialogue(p, dialogueStatus);
            }
            if (p.getTemporaryAttribute("slayerMaster") == null)
            {
                return;
            }
            int index = (int)p.getTemporaryAttribute("slayerMaster");
            switch (dialogueStatus)
            {
                case 1000:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
                    p.getPackets().modifyText("Hello, what can i help you with?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 1001;
                    break;

                case 1001:
                    p.getPackets().modifyText("I need another Slayer assignment.", 238, 1);
                    p.getPackets().modifyText("Could i see your supplies?", 238, 2);
                    p.getPackets().modifyText("I'd like to discuss Slayer points.", 238, 3);
                    p.getPackets().modifyText("I'd like a Slayer Skillcape.", 238, 4);
                    p.getPackets().modifyText("Er...nothing...", 238, 5);
                    p.getPackets().sendChatboxInterface2(238);
                    newStatus = 1002;
                    break;

                case 1002: // New assignment
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I need a new assignment.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1003;
                    break;

                case 1003: // Say they should go to a new master depending on lvl
                    if (p.getSlayerTask() != null)
                    {
                        SlayerTask task = p.getSlayerTask();
                        object[][] data = getMasterData(task.getMasterIndex());
                        if (index > 0)
                        {
                            p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 243, 2);
                            p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 243, 3);
                            p.getPackets().modifyText("You are still hunting " + (string)data[task.getMonsterIndex()][0] + ", finish that task then", 243, 4);
                            p.getPackets().modifyText("come back to see me, if you are struggling with your task", 243, 5);
                            p.getPackets().modifyText("then go and see Turael in Burthorpe.", 243, 6);
                            p.getPackets().animateInterface(9827, 243, 2);
                            p.getPackets().sendChatboxInterface2(243);
                            break;
                        }
                        else
                        {
                            p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                            p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                            p.getPackets().modifyText("You are still hunting " + (string)data[task.getMonsterIndex()][0] + ", although i suppose", 242, 4);
                            p.getPackets().modifyText("i could change it for you.", 242, 5);
                            p.getPackets().animateInterface(9827, 242, 2);
                            p.getPackets().sendChatboxInterface2(242);
                            newStatus = 1016;
                            break;
                        }
                    }
                    else
                    {
                        if (index == 4)
                        {
                            p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                            p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                            p.getPackets().modifyText("Excellent, you're doing great, your new task is to kill", 242, 4);
                            p.getPackets().modifyText(getTask(p, index), 242, 5);
                            p.getPackets().animateInterface(9827, 242, 2);
                            p.getPackets().sendChatboxInterface2(242);
                            newStatus = 1008;
                            break;
                        }
                        if (p.getSkills().getCombatLevel() >= (int)SLAYER_MASTERS[index + 1][1])
                        {
                            p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                            p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                            p.getPackets().modifyText("You're actually very strong. Are you sure you don't", 242, 4);
                            p.getPackets().modifyText("want " + (string)SLAYER_MASTERS[index + 1][2] + " in " + (string)SLAYER_MASTERS[index + 1][7] + " to assign you a task?", 242, 5);
                            p.getPackets().animateInterface(9827, 242, 2);
                            p.getPackets().sendChatboxInterface2(242);
                            newStatus = 1004;
                            break;
                        }
                    }
                    break;

                case 1004:
                    p.getPackets().modifyText("No, that's ok, i'll get a task from you", 557, 2);
                    p.getPackets().modifyText("Oh, ok then, i'll go talk to " + (string)SLAYER_MASTERS[index + 1][2], 557, 3);
                    p.getPackets().sendChatboxInterface2(557);
                    newStatus = 1005;
                    break;

                case 1005:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Oh ok then, i'll go talk to " + (string)SLAYER_MASTERS[index + 1][2] + ".", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 1006:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("No, that's ok, i'll take a task from you.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1007;
                    break;

                case 1007:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("Excellent, you're doing great, your new task is to kill", 242, 4);
                    p.getPackets().modifyText(getTask(p, index), 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 1008;
                    break;

                case 1008:
                    p.getPackets().modifyText("Got any tips for me?.", 557, 2);
                    p.getPackets().modifyText("Okay, great!", 557, 3);
                    p.getPackets().sendChatboxInterface2(557);
                    newStatus = 1009;
                    break;

                case 1009:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Okay, great!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1011;
                    break;

                case 1010:
                    displayTip(p, index);
                    newStatus = 1012;
                    break;

                case 1011:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("Good luck, don't forget to come back when you need a", 242, 4);
                    p.getPackets().modifyText("new assignment.", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    break;

                case 1012:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Thanks for the help!", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 1013:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Could i see your supplies?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1014;
                    break;

                case 1014:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
                    p.getPackets().modifyText("Of course, i have a wide selection of Slayer equipment!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 1015;
                    break;

                case 1015:
                    p.setShopSession(new ShopSession(p, 2));
                    break;

                case 1016:
                    p.getPackets().modifyText("No thankyou, i will stick with my current task", 557, 2);
                    p.getPackets().modifyText("I'd like a new task", 557, 3);
                    p.getPackets().sendChatboxInterface2(557);
                    newStatus = 1017;
                    break;

                case 1017:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like a new task from you please.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1018;
                    break;

                case 1018:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("Your new task is to kill", 242, 4);
                    p.getPackets().modifyText(getTask(p, index), 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 1008;
                    break;

                case 1019:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I think i'll stick with my current task.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1020;
                    break;

                case 1020:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
                    p.getPackets().modifyText("As you wish, good luck!", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    break;

                case 1021:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like to discuss Slayer points.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1022;
                    break;

                case 1022:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
                    p.getPackets().modifyText("As you wish.", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 1023;
                    break;

                case 1023:
                    displayPoints(p, 1);
                    break;

                case 1024:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Er...Nothing, sorry for wasting your time.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 1025:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("I'd like a Slayer Skillcape.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1026;
                    break;

                case 1026:
                    if (index != 4)
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("I don't sell the Slayer Skillcape, you should", 242, 4);
                        p.getPackets().modifyText("go and speak with Duradel in Shilo village.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                        newStatus = 1027;
                        break;
                    }
                    if (p.getSkills().getMaxLevel(Skills.SKILL.SLAYER) < 99)
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("Ha! you're not a Master Slayer!, you must have reached", 242, 4);
                        p.getPackets().modifyText("a Slayer level of 99 to purchase the Slayer Skillcape.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("I will sell you a Slayer Skillcape and hood", 242, 4);
                        p.getPackets().modifyText("for a sum of " + SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                        newStatus = 1028;
                    }
                    break;

                case 1027:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Thankyou for the help, i shall speak with Duradel.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 1028:
                    p.getPackets().modifyText("I will gladly pay " + SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins.", 557, 2);
                    p.getPackets().modifyText("I've changed my mind.", 557, 3);
                    p.getPackets().sendChatboxInterface2(557);
                    newStatus = 1029;
                    break;

                case 1029:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText(SkillHandler.SKILLCAPE_PRICE.ToString("#,##0") + " coins seems a fair price.", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1030;
                    break;

                case 1030:
                    if (p.getInventory().getTotalFreeSlots() < 2)
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("Unfortunatly i can't sell to you, since you", 242, 4);
                        p.getPackets().modifyText("don't seem to have enough inventory room.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                        break;
                    }
                    if (p.getInventory().deleteItem(995, SkillHandler.SKILLCAPE_PRICE))
                    {
                        int cape = p.getSkills().hasMultiple99s() ? 9787 : 9786;
                        int hood = 9788;
                        p.getInventory().addItem(cape);
                        p.getInventory().addItem(hood);
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("There you go, one Slayer Skillcape & hood.", 242, 4);
                        p.getPackets().modifyText("Wear it with pride young slayer.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("I don't give these things out lightly you know!", 242, 4);
                        p.getPackets().modifyText("Get more money then speak to me again.", 242, 5);
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

        private static void doGemDialogue(Player p, int dialogueStatus)
        {
            int newStatus = -1;
            int index = -1;
            for (int i = 0; i < SLAYER_MASTERS.Length; i++)
            {
                if (p.getSkills().getCombatLevel() >= (int)SLAYER_MASTERS[i][1])
                {
                    index = i;
                }
            }
            switch (dialogueStatus)
            {
                case 1051:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 241, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 241, 3);
                    p.getPackets().modifyText("Hello there " + p.getLoginDetails().getUsername() + "! what can i help you with?", 241, 4);
                    p.getPackets().animateInterface(9827, 241, 2);
                    p.getPackets().sendChatboxInterface2(241);
                    newStatus = 1052;
                    break;

                case 1052:
                    p.getPackets().modifyText("How am i doing so far?.", 238, 1);
                    p.getPackets().modifyText("Who are you?", 238, 2);
                    p.getPackets().modifyText("Where are you?", 238, 3);
                    p.getPackets().modifyText("Got any tips for me?", 238, 4);
                    p.getPackets().modifyText("Nevermind.", 238, 5);
                    p.getPackets().sendChatboxInterface2(238);
                    newStatus = 1053;
                    break;

                case 1053:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("How am i doing so far?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1054;
                    break;

                case 1054:
                    if (p.getSlayerTask() != null)
                    {
                        SlayerTask task = p.getSlayerTask();
                        object[][] data = getMasterData(task.getMasterIndex());
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("You're currently assigned to kill " + (string)data[task.getMonsterIndex()][0] + "; only", 242, 4);
                        p.getPackets().modifyText(task.getAmount() + " more to go.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    else
                    {
                        p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                        p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                        p.getPackets().modifyText("You're aren't assigned anything to kill, perhaps", 242, 4);
                        p.getPackets().modifyText("you should come and see me soon.", 242, 5);
                        p.getPackets().animateInterface(9827, 242, 2);
                        p.getPackets().sendChatboxInterface2(242);
                    }
                    newStatus = 1052;
                    break;

                case 1055:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Who are you?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1056;
                    break;

                case 1056:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("My name is " + (string)SLAYER_MASTERS[index][2] + ", i am the Slayer", 242, 4);
                    p.getPackets().modifyText("master most suited to your combat level.", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 1052;
                    break;

                case 1057:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Where are you?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1058;
                    break;

                case 1058:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("I am in " + (string)SLAYER_MASTERS[index][5], 242, 4);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][6], 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    newStatus = 1052;
                    break;

                case 1059:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Do you have any tips for me?", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    newStatus = 1060;
                    break;

                case 1060:
                    displayTip(p, index);
                    newStatus = 1052;
                    break;

                case 1061:
                    p.getPackets().sendPlayerHead(64, 2);
                    p.getPackets().modifyText(p.getLoginDetails().getUsername(), 64, 3);
                    p.getPackets().modifyText("Nevermind, sorry for bothering you..", 64, 4);
                    p.getPackets().animateInterface(9827, 64, 2);
                    p.getPackets().sendChatboxInterface2(64);
                    break;

                case 1062:
                    p.getPackets().sendNPCHead((int)SLAYER_MASTERS[index][0], 242, 2);
                    p.getPackets().modifyText((string)SLAYER_MASTERS[index][2], 242, 3);
                    p.getPackets().modifyText("You have completed your Slayer task, please return", 242, 4);
                    p.getPackets().modifyText("to a Slayer master for a new assignment.", 242, 5);
                    p.getPackets().animateInterface(9827, 242, 2);
                    p.getPackets().sendChatboxInterface2(242);
                    break;
            }
            if (newStatus != -1)
            {
                p.setTemporaryAttribute("dialogue", newStatus);
            }
        }
    }
}