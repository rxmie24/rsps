using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;
using System;

namespace RS2.Server.player
{
    internal class Bank
    {
        private Player player;
        public static int MAX_BANK_SLOTS = 496;
        private bool banking = false;
        private Item[] bank = new Item[MAX_BANK_SLOTS];
        private bool withdrawAsNote = false;
        private int setNewPinAttempt = 0;
        private int[] pinSeq;
        private int pinStatus = 0;
        private int[] tempPin1 = new int[4];
        private int[] tempPin2 = new int[4];
        private bool pinCorrect;
        private int[] bankPin;
        private int recoveryDaysRequired = 3;
        private bool bankPinRemoved;
        public bool changingPin = false;
        private long lastPinChange;
        private long lastDeletionRequest;
        public int openStatus = 0;
        private int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //private int[] tabAmounts = new int[11];
        private int currentTab = 10;

        private int lastXAmount = 1;
        public static string[] MESSAGES = { "Now click the SECOND digit.", "Time for the THIRD digit.", "Finally, the FOURTH digit.", "Finally, the FOURTH digit." };
        public static int[] ASTERIK = { 21, 22, 23, 24 };

        public Bank(Player player)
        {
            for (int i = 0; i < MAX_BANK_SLOTS; i++)
            {
                bank[i] = new Item(-1, 0);
            }
            this.player = player;
        }

        public string getBankPin()
        {
            if (bankPin == null) return "";
            string bankPinStr = "";
            foreach (int pinNumber in bankPin)
                bankPinStr += pinNumber.ToString();
            return bankPinStr;
        }

        public void setBankPin(string pin)
        {
            if (pin == "") return;

            for (int i = 0; i < 4; i++)
                bankPin[i] = Convert.ToInt32(pin[i]);
        }

        public void openBank()
        {
            player.getWalkingQueue().resetWalkingQueue();
            player.getPackets().clearMapFlag();
            //int items = 20 * 1024^0 + 3 * 1024^1 + 4 * 1024^2; // Used with config2 1246
            if (!banking)
            {
                if (!pinCorrect && bankPin != null)
                {
                    if (isPinPending())
                    {
                        verifyPin(false);
                        return;
                    }
                    if (!isPinPending())
                    {
                        pinStatus = 0;
                        tempPin1 = new int[4];
                        openEnterPin();
                        return;
                    }
                }
                player.getPackets().sendConfig(563, 4194304);
                player.getPackets().sendConfig(1248, -2013265920);
                player.getPackets().sendConfig(1249, lastXAmount);
                player.getPackets().sendBankOptions();
                refreshBank();
                player.getPackets().displayInventoryInterface(763);
                player.getPackets().displayInterface(762);
                player.getPackets().showChildInterface(762, 18, false);
                player.getPackets().showChildInterface(762, 19, false);
                player.getPackets().showChildInterface(762, 23, false);
                //setTabConfig();
                banking = true;
            }
        }

        private void setTabConfig()
        {
            /*int config = 0;
            config += tabAmounts[2];
            config += tabAmounts[3] * 1024;
            config += tabAmounts[4] * 1048576;
            player.getPackets().sendConfig(1246, config);
            config = 0;
            config += tabAmounts[5];
            config += tabAmounts[6] * 1024;
            config += tabAmounts[7] * 1048576;
            player.getPackets().sendConfig(1247, config);
            config = -2013265920;
            config += tabAmounts[8];
            config += tabAmounts[9] * 1024;
            player.getPackets().sendConfig(1248, config);*/
        }

        //20 * 1024^0 + 3 * 1024^1 + 4 * 1024^2
        public void openBank(bool dialogue, int x, int y)
        {
            openStatus = 0;
            changingPin = false;
            AreaEvent openBankAreaEvent = new AreaEvent(player, x - 1, y - 1, x + 1, y + 1);
            openBankAreaEvent.setAction(() =>
            {
                if (dialogue)
                {
                    //Dialogue.doDialogue(player, 1); //TODO: Make bank Dialogue for  clicking on booths.
                    return;
                }
                openBank();
            });
            Server.registerCoordinateEvent(openBankAreaEvent);
        }

        public void openPinSettings(int i)
        {
            pinStatus = 0;
            setNewPinAttempt = 0;
            tempPin1 = new int[4];
            player.getPackets().showChildInterface(14, 89, false); // "Yes i really want to do that" text.
            player.getPackets().showChildInterface(14, 91, false); // "No forget i asked" text
            player.getPackets().showChildInterface(14, 87, false); // Big black square with red bars
            if (!pinCorrect && bankPin != null)
            {
                openStatus = 1;
                if (isPinPending())
                {
                    verifyPin(false);
                    return;
                }
                if (!isPinPending())
                {
                    openEnterPin();
                    return;
                }
            }
            if (i == 1 && !isPinPending())
            {
                player.getPackets().showChildInterface(14, 60, false);
                player.getPackets().showChildInterface(14, 65, false);
                player.getPackets().showChildInterface(14, 61, false);
                player.getPackets().modifyText("You have a PIN", 14, 69);
                player.getPackets().modifyText("Players are reminded", 14, 42);
                player.getPackets().modifyText("that they should NEVER tell", 14, 43);
                player.getPackets().modifyText("anyone their bank PINs or", 14, 44);
                player.getPackets().modifyText("password, nor should they", 14, 45);
                player.getPackets().modifyText("ever enter their PINs on any", 14, 46);
                player.getPackets().modifyText("website form.", 14, 47);
                player.getPackets().modifyText("", 14, 48);
                player.getPackets().modifyText("", 14, 49);
                player.getPackets().modifyText("", 14, 50);
            }
            else if (i == 2)
            {
                changingPin = false;
                player.getPackets().modifyText("No changes made.", 14, 42);
                player.getPackets().modifyText("", 14, 43);
                player.getPackets().modifyText("", 14, 44);
                player.getPackets().modifyText("", 14, 45);
                player.getPackets().modifyText("", 14, 46);
                player.getPackets().modifyText("", 14, 47);
                player.getPackets().modifyText("", 14, 48);
                player.getPackets().modifyText("", 14, 49);
                player.getPackets().modifyText("", 14, 50);
            }
            else if (i == 3)
            {
                player.getPackets().modifyText("Those numbers did not", 14, 42);
                player.getPackets().modifyText("match.", 14, 43);
                player.getPackets().modifyText("Your PIN has not been set,", 14, 45);
                player.getPackets().modifyText("please try again if you wish", 14, 46);
                player.getPackets().modifyText("to set a new PIN.", 14, 47);
                player.getPackets().modifyText("", 14, 48);
                player.getPackets().modifyText("", 14, 49);
                player.getPackets().modifyText("", 14, 50);
            }
            else if (i == 4)
            {
                bankPin = null;
                lastPinChange = 0;
                lastDeletionRequest = 0;
                player.getPackets().showChildInterface(14, 65, false);
                player.getPackets().showChildInterface(14, 60, true);
                player.getPackets().showChildInterface(14, 61, true);
                player.getPackets().modifyText("The PIN has been cancelled", 14, 42);
                player.getPackets().modifyText("and will NOT be set.", 14, 43);
                player.getPackets().modifyText("", 14, 44);
                player.getPackets().modifyText("You still do not have a Bank", 14, 45);
                player.getPackets().modifyText("PIN.", 14, 46);
                player.getPackets().modifyText("", 14, 47);
                player.getPackets().modifyText("", 14, 48);
                player.getPackets().modifyText("", 14, 49);
                player.getPackets().modifyText("", 14, 50);
            }
            else
                if (i == 5 || isPinPending())
                {
                    player.getPackets().modifyText("PIN coming soon", 14, 69);
                    player.getPackets().showChildInterface(14, 65, true);
                    player.getPackets().showChildInterface(14, 60, false);
                    player.getPackets().showChildInterface(14, 61, false);
                    player.getPackets().showChildInterface(14, 64, false);
                    player.getPackets().showChildInterface(14, 62, false);
                    player.getPackets().showChildInterface(14, 63, false);
                    player.getPackets().modifyText("You have requested that a", 14, 42);
                    player.getPackets().modifyText("PIN be set on your bank", 14, 43);
                    player.getPackets().modifyText("account. This will take effect", 14, 44);
                    player.getPackets().modifyText("in another " + (recoveryDaysRequired - TimeSpan.FromTicks(lastPinChange).Days) + " days.", 14, 45);
                    player.getPackets().modifyText("", 14, 46);
                    player.getPackets().modifyText("If you wish to cancel this", 14, 47);
                    player.getPackets().modifyText("PIN, please use the button", 14, 48);
                    player.getPackets().modifyText("on the left.", 14, 49);
                    player.getPackets().modifyText("", 14, 50);
                }
                else if (i == 6 && changingPin)
                {
                    changingPin = false;
                    player.getPackets().modifyText("Those numbers did not", 14, 42);
                    player.getPackets().modifyText("match.", 14, 43);
                    player.getPackets().modifyText("", 14, 44);
                    player.getPackets().modifyText("Your pin has NOT been", 14, 45);
                    player.getPackets().modifyText("changed, please try again", 14, 46);
                    player.getPackets().modifyText("If you wish to set a new PIN", 14, 47);
                    player.getPackets().modifyText("", 14, 48);
                    player.getPackets().modifyText("", 14, 49);
                    player.getPackets().modifyText("", 14, 50);
                }
                else if (i == 7 && changingPin)
                {
                    changingPin = false;
                    lastDeletionRequest = 0;
                    player.getPackets().showChildInterface(14, 60, false);
                    player.getPackets().showChildInterface(14, 65, false);
                    player.getPackets().showChildInterface(14, 61, false);
                    player.getPackets().modifyText("You have a PIN", 14, 69);
                    player.getPackets().modifyText("Your PIN has been changed", 14, 42);
                    player.getPackets().modifyText("to the number you entered.", 14, 43);
                    player.getPackets().modifyText("This takes effect", 14, 44);
                    player.getPackets().modifyText("immediately.", 14, 45);
                    player.getPackets().modifyText("", 14, 46);
                    player.getPackets().modifyText("If you cannot remember that", 14, 47);
                    player.getPackets().modifyText("new number, we STRONGLY", 14, 48);
                    player.getPackets().modifyText("advise you to delete the PIN", 14, 49);
                    player.getPackets().modifyText("now.", 14, 50);
                }
                else if (i == 8)
                {
                    lastDeletionRequest = 0;
                    player.getPackets().showChildInterface(14, 60, false);
                    player.getPackets().showChildInterface(14, 65, false);
                    player.getPackets().showChildInterface(14, 61, false);
                    player.getPackets().modifyText("You have a PIN", 14, 69);
                    player.getPackets().modifyText("The requested removal of", 14, 42);
                    player.getPackets().modifyText("your PIN has been cancelled.", 14, 43);
                    player.getPackets().modifyText("Your PIN will NOT be deleted.", 14, 44);
                    player.getPackets().modifyText("", 14, 45);
                    player.getPackets().modifyText("If it wasn't you that tried to", 14, 46);
                    player.getPackets().modifyText("delete it, someone else may", 14, 47);
                    player.getPackets().modifyText("have been on your account.", 14, 48);
                    player.getPackets().modifyText("- Please consider changing", 14, 49);
                    player.getPackets().modifyText("your password immediately!", 14, 50);
                }
                else if (i == 9 && bankPinRemoved)
                {
                    bankPin = null;
                    bankPinRemoved = false;
                    player.getPackets().modifyText("Your Bank PIN has now been", 14, 42);
                    player.getPackets().modifyText("deleted.", 14, 43);
                    player.getPackets().modifyText("", 14, 44);
                    player.getPackets().modifyText("This means that there is no", 14, 45);
                    player.getPackets().modifyText("PIN protection on your bank", 14, 46);
                    player.getPackets().modifyText("account.", 14, 47);
                    player.getPackets().modifyText("", 14, 48);
                    player.getPackets().modifyText("", 14, 49);
                    player.getPackets().modifyText("", 14, 50);
                    player.getPackets().sendMessage("Your bank PIN has been successfully deleted.");
                }
            if (bankPin == null)
            {
                tempPin2 = new int[4];
                player.getPackets().modifyText("No PIN set", 14, 69);
                player.getPackets().showChildInterface(14, 62, false);
                player.getPackets().showChildInterface(14, 63, false);
                player.getPackets().showChildInterface(14, 64, false);
                player.getPackets().showChildInterface(14, 65, false);
                player.getPackets().showChildInterface(14, 60, true);
                player.getPackets().showChildInterface(14, 61, true);
            }
            player.getPackets().modifyText(recoveryDaysRequired + " days", 14, 71);
            player.getPackets().displayInterface(14);
        }

        public void handleEnterPin(int buttonId)
        {
            if ((bankPin == null && !isPinPending()) || changingPin)
            {
                if (pinStatus == 4 && setNewPinAttempt == 1)
                {
                    return;
                }
                player.getPackets().modifyText(MESSAGES[pinStatus], 13, 32);
                player.getPackets().modifyText("*", 13, ASTERIK[pinStatus]);
                if (setNewPinAttempt == 0 && pinSeq != null)
                    tempPin1[pinStatus] = (pinSeq[buttonId - 1]);
                else if (setNewPinAttempt == 1 && pinSeq != null)
                    tempPin2[pinStatus] = (pinSeq[buttonId - 1]);
                pinStatus++;
                if (pinStatus == 4 && setNewPinAttempt == 0)
                {
                    setNewPinAttempt++;
                    pinStatus = 0;
                    player.getPackets().modifyText("Now please enter that number again!", 13, 28);
                    player.getPackets().modifyText("First click the FIRST digit.", 13, 32);
                    for (int i = 21; i <= 24; i++)
                    {
                        player.getPackets().modifyText("?", 13, i);
                    }
                }
                else
                    if (pinStatus == 4 && setNewPinAttempt == 1)
                    {
                        for (int i = 0; i < tempPin1.Length; i++)
                        {
                            if (tempPin1[i] != tempPin2[i])
                            {
                                player.getPackets().sendMessage("The two PIN numbers you entered did not match.");
                                openPinSettings(changingPin ? 6 : 3);
                                return;
                            }
                        }
                        if (bankPin == null && !changingPin)
                        {
                            lastPinChange = DateTime.Now.Ticks;
                        }
                        bankPin = tempPin1;
                        pinCorrect = true;
                        openPinSettings(changingPin ? 7 : 5);
                        return;
                    }
                scrambleNumbers();
            }
            else
            {
                if (pinStatus >= 4)
                {
                    return;
                }
                player.getPackets().modifyText(MESSAGES[pinStatus], 13, 32);
                player.getPackets().modifyText("*", 13, ASTERIK[pinStatus]);
                tempPin1[pinStatus] = (pinSeq[buttonId - 1]);
                pinStatus++;
                scrambleNumbers();
                if (pinStatus == 4)
                {
                    for (int i = 0; i < tempPin1.Length; i++)
                    {
                        if (tempPin1[i] != bankPin[i])
                        {
                            player.getPackets().closeInterfaces();
                            player.getPackets().modifyText("The PIN number you entered was incorrect.", 210, 1);
                            player.getPackets().sendChatboxInterface2(210);
                            return;
                        }
                    }
                    pinCorrect = true;
                    player.getPackets().sendMessage("You have correctly entered your PIN.");
                    if (lastDeletionRequest != 0)
                    {
                        openPinSettings(8);
                        return;
                    }
                    if (openStatus == 0)
                    {
                        openBank();
                    }
                    else
                    {
                        openPinSettings(1);
                    }
                }
            }
        }

        public void cancelPendingPin()
        {
            if (bankPin == null || !isPinPending())
            {
                return;
            }
            pinCorrect = true;
            openPinSettings(4);
        }

        public void changePin()
        {
            if (bankPin == null || changingPin || isPinPending())
            {
                return;
            }
            tempPin2 = new int[4];
            changingPin = true;
            player.getPackets().showChildInterface(14, 87, true);
            player.getPackets().showChildInterface(14, 89, true);
            player.getPackets().showChildInterface(14, 91, true);
            player.getPackets().modifyText("Do you really wish to change your Bank PIN?", 14, 73);
            player.getPackets().modifyText("Yes, I am ready for a new one.", 14, 89);
            player.getPackets().modifyText("No thanks, I'll stick to my current one.", 14, 91);
        }

        public void deletePin()
        {
            if (bankPin == null || isPinPending())
            {
                return;
            }
            bankPinRemoved = true;
            player.getPackets().showChildInterface(14, 87, true);
            player.getPackets().showChildInterface(14, 89, true);
            player.getPackets().showChildInterface(14, 91, true);
            player.getPackets().modifyText("Do you really wish to delete your Bank PIN?", 14, 73);
            player.getPackets().modifyText("Yes, I don't need a PIN anymore.", 14, 89);
            player.getPackets().modifyText("No thanks, I'd rather keep the extra security.", 14, 91);
        }

        public void verifyPin(bool verified)
        {
            int daysLeft = recoveryDaysRequired - TimeSpan.FromTicks(lastPinChange).Days;
            if (!verified)
            {
                player.getPackets().showChildInterface(14, 87, true);
                player.getPackets().showChildInterface(14, 89, true);
                player.getPackets().showChildInterface(14, 91, true);
                player.getPackets().modifyText("A PIN will be set on your account in another " + daysLeft + " days.", 14, 73);
                player.getPackets().modifyText("Yes, I asked for this. I want this PIN.", 14, 89);
                player.getPackets().modifyText("No, I didn't ask for this. Cancel it.", 14, 91);
                player.getPackets().displayInterface(14);
                return;
            }
            pinCorrect = true;
            player.getPackets().sendMessage("Your PIN is still pending.");
            if (openStatus == 0)
            {
                openBank();
            }
            else
            {
                openPinSettings(1);
            }
        }

        public void openEnterPin()
        {
            if (bankPinRemoved)
            {
                openPinSettings(9);
                return;
            }
            if (bankPin == null || changingPin)
            {
                player.getPackets().showChildInterface(13, 29, false);
            }
            else if (bankPin != null && !changingPin)
            {
                player.getPackets().setRightClickOptions(2, 851997, -1, -1);
            }
            if (lastDeletionRequest != 0)
            {
                int daysLeft = recoveryDaysRequired - TimeSpan.FromTicks(lastDeletionRequest).Days;
                player.getPackets().modifyText("Your Bank PIN will be deleted in another " + daysLeft + " days.", 13, 31);
            }
            else
            {
                player.getPackets().modifyText("Bank of " + player.getLoginDetails().getUsername(), 13, 31);
            }
            scrambleNumbers();
            player.getPackets().displayInterface(13);
        }

        public void changePinDelay()
        {
            string s = bankPin == null ? "But you" : "";
            string s1 = bankPin == null ? "haven't got one..." : "";
            recoveryDaysRequired = recoveryDaysRequired == 3 ? 7 : 3;
            player.getPackets().modifyText(recoveryDaysRequired + " days", 14, 71);
            player.getPackets().modifyText("Your recovery delay has", 14, 42);
            player.getPackets().modifyText("now been set to " + recoveryDaysRequired + " days.", 14, 43);
            player.getPackets().modifyText("", 14, 44);
            player.getPackets().modifyText("You would have to wait this", 14, 45);
            player.getPackets().modifyText("long to delete your PIN if", 14, 46);
            player.getPackets().modifyText("you forgot it. " + s, 14, 47);
            player.getPackets().modifyText(s1, 14, 48);
        }

        public void displayFirstConfirmation()
        {
            player.getPackets().showChildInterface(14, 87, true);
            player.getPackets().showChildInterface(14, 89, true);
            player.getPackets().showChildInterface(14, 91, true);
            player.getPackets().modifyText("Do you really wish to set a PIN on your bank account?", 14, 73);
            player.getPackets().modifyText("Yes, I really want a bank PIN. I will never forget it!", 14, 89);
            player.getPackets().modifyText("No, I might forget it!", 14, 91);
        }

        public void scrambleNumbers()
        {
            pinSeq = new int[10];
            int[] id = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            numbers = id;
            int j = -1;
            for (int i = 0; i <= 9; i++)
            {
                int num = -1;
                for (; ; )
                {
                    num = Misc.random(numbers.Length - 1);
                    if (numbers[num] == -1)
                    {
                        continue;
                    }
                    else
                    {
                        j = numbers[num];
                        numbers[num] = -1;
                        break;
                    }
                }
                player.getPackets().moveChildInterface(13, 13, 0, 0);
                player.getPackets().modifyText("" + j, 13, (i + 11));
                pinSeq[i] = j;
            }
        }

        public bool isPinPending()
        {
            if (lastPinChange == 0)
            {
                return false;
            }
            int hours = recoveryDaysRequired == 7 ? 168 : 72;
            return TimeSpan.FromTicks(lastPinChange).Hours < hours;
        }

        public void forgotPin()
        {
            int daysLeft = recoveryDaysRequired - TimeSpan.FromTicks(lastDeletionRequest).Days;
            player.getPackets().closeInterfaces();

            if (lastDeletionRequest == 0)
            {
                lastDeletionRequest = DateTime.Now.Ticks;
                daysLeft = recoveryDaysRequired - TimeSpan.FromTicks(lastDeletionRequest).Days;
                player.getPackets().modifyText("Since you don't know your PIN, it will be deleted in another " + daysLeft, 212, 1);
                player.getPackets().modifyText("days. If you wish to cancel this change, you may do so by entering", 212, 2);
                player.getPackets().modifyText("your PIN correctly next time you attempt to use your Bank.", 212, 3);
                player.getPackets().sendChatboxInterface2(212);
            }
            else
            {
                player.getPackets().modifyText("You have already requested that your PIN be deleted.", 211, 1);
                player.getPackets().modifyText("This will take effect after another " + daysLeft + " days.", 211, 2);
                player.getPackets().sendChatboxInterface2(211);
            }
        }

        public void refreshBank()
        {
            player.getPackets().sendItems(-1, 64000, 95, bank);
            player.getPackets().sendItems(-1, 64000, 93, player.getInventory().getItems());
            player.getPackets().refreshInventory();
        }

        /*public void sendTabConfig() {
            int config = 0;
            config += getItemsInTab(p, 2);
            config += getItemsInTab(p, 3) * 1024;
            config += getItemsInTab(p, 4) * 1048576;
            player.getPackets().sendConfig(1246, config);
            config = 0;
            config += getItemsInTab(p, 5);
            config += getItemsInTab(p, 6) * 1024;
            config += getItemsInTab(p, 7) * 1048576;
            player.getPackets().sendConfig(1247, config);
            config = -2013265920;
            config += getItemsInTab(p, 8);
            config += getItemsInTab(p, 9) * 1024;
            player.getPackets().sendConfig(1248, config);
        }*/

        public void asNote()
        {
            if (player.getTemporaryAttribute("withdrawNote") == null)
            {
                player.setTemporaryAttribute("withdrawNote", (bool)true);
                withdrawAsNote = true;
                return;
            }
            if ((bool)player.getTemporaryAttribute("withdrawNote"))
            {
                player.setTemporaryAttribute("withdrawNote", (bool)false);
            }
            else
            {
                player.setTemporaryAttribute("withdrawNote", (bool)true);
            }
            withdrawAsNote = (bool)player.getTemporaryAttribute("withdrawNote");
        }

        public void deposit(int inventorySlot, int itemAmount)
        {
            if (!banking)
                return;
            Inventory inv = player.getInventory();
            int item = inv.getItemInSlot(inventorySlot);
            int bankSlot = findItem(item);
            bool itemNoted = ItemData.forId(item).isNoted();
            bool itemStackable = ItemData.forId(item).isStackable();

            itemAmount = Math.Min(itemAmount, inv.getItemAmount(item));

            if (inv.getItemInSlot(inventorySlot) == item && item != -1)
            {
                if (!inv.deleteItem(item, itemAmount))
                    return;

                if (itemNoted)
                {
                    bankSlot = findItem(ItemData.getUnNotedItem(item));
                }
                else
                {
                    bankSlot = findItem(item);
                }

                if (bankSlot == -1)
                {
                    bankSlot = findFreeSlot();
                    if (bankSlot == -1)
                    {
                        player.getPackets().sendMessage("Your bank is full!");
                        return;
                    }
                }

                if (itemNoted)
                {
                    bank[bankSlot].setItemId(ItemData.getUnNotedItem(item));
                }
                else
                {
                    bank[bankSlot].setItemId(item);
                }
                bank[bankSlot].setItemAmount(bank[bankSlot].getItemAmount() + itemAmount);
                refreshBank();
            }
        }

        public void withdraw(int bankSlot, int amount)
        {
            if (!banking)
            {
                return;
            }
            int item = getItemInSlot(bankSlot);
            if (item == -1)
            {
                return;
            }
            Inventory inv = player.getInventory();
            int amountToRemove = amount;
            int itemToRemove = item;
            bool stackable = ItemData.forId(item).isStackable();
            if (amountToRemove > bank[bankSlot].getItemAmount())
            {
                amountToRemove = bank[bankSlot].getItemAmount();
            }
            if (withdrawAsNote)
            {
                itemToRemove = ItemData.getNotedItem(itemToRemove);
                if (itemToRemove == item || stackable)
                {
                    itemToRemove = item;
                    player.getPackets().sendMessage("That item can't be withdrawn as a note.");
                }
            }
            if (amount > 1 && !stackable && !withdrawAsNote)
            {
                for (int i = 0; i < amount; i++)
                {
                    if (!inv.addItem(itemToRemove))
                    {
                        break;
                    }
                    bank[bankSlot].setItemAmount(bank[bankSlot].getItemAmount() - 1);
                    if (bank[bankSlot].getItemAmount() <= 0)
                    {
                        bank[bankSlot].setItemId(-1);
                        bank[bankSlot].setItemAmount(0);
                        /*tabAmounts[currentTab]--;
                        currentTab = 10; // TODO remove once force tab sswitching is fixed
                        */
                        arrangeBank();
                        break;
                    }
                }
                refreshBank();
                return;
            }
            else if (amount == 1 || stackable || withdrawAsNote)
            {
                if (!inv.addItem(itemToRemove, amountToRemove))
                {
                    return;
                }
                bank[bankSlot].setItemAmount(bank[bankSlot].getItemAmount() - amountToRemove);
                if (bank[bankSlot].getItemAmount() <= 0)
                {
                    bank[bankSlot].setItemId(-1);
                    bank[bankSlot].setItemAmount(0);
                    //tabAmounts[currentTab]--;
                    //currentTab = 10; // TODO remove once force tab switching is fixed
                    arrangeBank();
                }
                refreshBank();
                return;
            }
        }

        public void arrangeBank()
        {
            Item[] oldData = bank;
            bank = new Item[MAX_BANK_SLOTS];
            int ptr = 0;
            for (int i = 0; i < MAX_BANK_SLOTS; i++)
            {
                if (oldData[i].getItemId() != -1)
                {
                    bank[ptr++] = oldData[i];
                }
            }
            for (int i = ptr; i < MAX_BANK_SLOTS; i++)
            {
                bank[i] = new Item(-1, 0);
            }
        }

        public void setCurrentTab(int tab)
        {
            this.currentTab = tab;
        }

        public void setBanking(bool b)
        {
            this.banking = b;
        }

        public int findFreeSlot()
        {
            for (int i = 0; i < MAX_BANK_SLOTS; i++)
            {
                if (bank[i].getItemId() == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        public int findItem(int itemId)
        {
            for (int i = 0; i < MAX_BANK_SLOTS; i++)
            {
                if (bank[i].getItemId() == itemId)
                {
                    return i;
                }
            }
            return -1;
        }

        public int getAmountInSlot(int slot)
        {
            return bank[slot].getItemAmount();
        }

        public int getItemInSlot(int slot)
        {
            return bank[slot].getItemId();
        }

        public Item getSlot(int slot)
        {
            return bank[slot];
        }

        public Item[] getBank()
        {
            return bank;
        }

        public bool isBanking()
        {
            return banking;
        }

        public void setLastXAmount(int i)
        {
            this.lastXAmount = i;
            player.getPackets().sendConfig(1249, lastXAmount);
        }

        public int getLastXAmount()
        {
            return lastXAmount;
        }
    }
}