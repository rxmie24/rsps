using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.minigames.duelarena
{
    internal class DuelSession
    {
        /**
	     * STATUSES :
	     * 0 - open interface / stake items / set rules
	     * 1 - one player has accepted
	     * 2 - both players on confirmation interface
	     * 3 - one player has accepted
	     * 4 - both players accepted
	     * 5 - duel countdown
	     * 6 - able to attack
	     * 7 - end of duel
	     * 8 - displayed winning screen
	     */

        private Player player;
        private Player p2;
        private Player winner;
        private Item[] items = new Item[28];
        private Item[] winnings;
        private int status;
        private int rules; //Possible 29 bits.

        public enum RULE
        {
            NOTHING = 0x0,
            NO_FORFEIT = 0x1,
            NO_MOVEMENT = 0x2,
            NO_RANGE = 0x10,
            NO_MELEE = 0x20,
            NO_MAGIC = 0x40,
            NO_DRINKS = 0x80,
            NO_FOOD = 0x100,
            NO_PRAYER = 0x200,
            OBSTACLES = 0x400,
            FUN_WEAPONS = 0x1000,
            NO_SPECIAL_ATTACKS = 0x2000,
            HAT = 0x4000,
            CAPE = 0x8000,
            AMULET = 0x10000,
            WEAPON = 0x20000,
            BODY = 0x40000,
            SHIELD = 0x80000,
            LEGS = 0x200000,
            GLOVES = 0x800000,
            BOOTS = 0x1000000,
            RING = 0x4000000,
            ARROWS = 0x8000000,
            SUMMONING = 0x10000000
        }

        private static string[] DUEL_TEXT = {
		    //DURING
            "",
		    "You cannot forfeit the duel.",
		    "You cannot move.",
		    "You cannot use Ranged attacks.", // "You cannot have no Ranged, no Melee and no Magic - how would you fight?
		    "You cannot use Melee attacks.",
		    "You cannot use Magic attacks.",
		    "You cannot use drinks.",
		    "You cannot use food.",
		    "You cannot use Prayer.",
		    "There will be obstacles in the arena.",
		    "You can only use 'fun weapons.'", // if neither player has a fun weapon, it says "Fun weapons is selected, but neither player has a fun weapon".
		    "You cannot use special attacks.",
		    //END OF DURING
		    //BEFORE THE DUEL
		    //ALWAYS SAYS - BOOSTED STATS WILL BE RESTORED
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.", // DURING - You can't use two-handed weapons, like bows.
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Some worn items will be taken off.",
		    "Summoning familiars can assist you in battle.", // DURING
	    };

        private static int[] DUEL_TEXT_VAR = {
            0,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    1,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    0,
		    1,
	    };

        public DuelSession(Player p, Player p2)
        {
            this.player = p;
            this.p2 = p2;
            this.rules = 0;
            this.status = 0;
            player.getDuelRequests().Clear();
            openInterface();
        }

        private void openInterface()
        {
            player.getPackets().modifyText("", 631, 28);
            player.getPackets().modifyText("unlimited!", 631, 92);
            player.getPackets().modifyText("unlimited!", 631, 93);
            player.getPackets().modifyText(p2.getLoginDetails().getUsername(), 631, 25);
            player.getPackets().modifyText("" + p2.getSkills().getCombatLevel(), 631, 27);
            player.getPackets().sendConfig(286, 0);
            configureDuel();
            refreshDuel();
        }

        public void configureDuel()
        {
            player.getPackets().displayInventoryInterface(336);
            player.getPackets().displayInterface(631);
            player.getPackets().setRightClickOptions(1278, (336 * 65536), 0, 27);// should be 1086?
            object[] opts1 = new object[] { "Stake X", "Stake All", "Stake 10", "Stake 5", "Stake 1", -1, 0, 7, 4, 93, 22020096 };
            player.getPackets().sendClientScript2(189, 150, opts1, "IviiiIsssss");
            player.getPackets().setRightClickOptions(1278, (631 * 65536) + 104, 0, 27);
            player.getPackets().setRightClickOptions(1278, (631 * 65536) + 103, 0, 27);
            player.getPackets().setRightClickOptions(2360446, (336 * 65536), 0, 27);
        }

        public void refreshDuel()
        {
            player.getPackets().sendItems(-1, 64209, 93, player.getInventory().getItems());
            player.getPackets().sendItems(-1, -70135, 134, items);
            p2.getPackets().sendItems(-2, -70135, 134, items);
        }

        public void toggleDuelRules(int buttonId)
        {
            if (buttonId == 102)
            {
                acceptDuel();
                return;
            }
            if (buttonId == 107)
            {
                declineDuel();
                return;
            }
            RULE rule = getArrayId(buttonId);
            if (rule == RULE.NOTHING)
                return;

            //Compute the bitwse dule rules configuration packet data.
            if (ruleEnabled(rule))
            { //rule is set, unset it.
                setRule(rule, false); //off
                p2.getDuel().setRule(rule, false);
            }
            else
            { //rule is unset, set it.
                setRule(rule, true); //on
                p2.getDuel().setRule(rule, true);
            }

            if (ruleEnabled(RULE.WEAPON))
            {
                player.getPackets().sendMessage("You will not be able to use two-handed weapons, such as bows.");
                p2.getPackets().sendMessage("You will not be able to use two-handed weapons, such as bows.");
            }
            if (ruleEnabled(RULE.ARROWS))
            {
                player.getPackets().sendMessage("You will not be able to use any weapon which uses arrows.");
                p2.getPackets().sendMessage("You will not be able to use any weapon which uses arrows.");
            }

            if (rules > (int)RULE.NOTHING)
            {
                player.getPackets().sendConfig(286, rules);
                p2.getPackets().sendConfig(286, rules);
            }
            else
            {
                player.getPackets().sendConfig(286, (int)RULE.NOTHING);
                p2.getPackets().sendConfig(286, (int)RULE.NOTHING);
            }

            resetStatus();
        }

        public void declineDuel()
        {
            status = 0;
            p2.getDuel().setStatus(0);
            player.getPackets().sendMessage("You decline the stake and duel options.");
            p2.getPackets().sendMessage("Other player declined the stake and duel options.");
            giveBack();
            p2.getDuel().giveBack();
            player.setTemporaryAttribute("challengeUpdate", true);
            p2.setTemporaryAttribute("challengeUpdate", true);
            p2.getPackets().closeInterfaces();
            player.getPackets().closeInterfaces();
        }

        private void giveBack()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (!player.getInventory().addItem(items[i].getItemId(), items[i].getItemAmount()))
                    {
                        Misc.WriteError("Possible stake dupe " + player.getLoginDetails().getUsername());
                    }
                }
            }
        }

        public void acceptDuel()
        {
            if (status == 0)
            {
                if (stakingOverMaxAmount())
                {
                    player.getPackets().sendMessage("You or your opponent dosen't have enough room for the stake.");
                    return;
                }
                if (notEnoughRoom())
                {
                    player.getPackets().sendMessage("You don't have enough inventory space for the stake and/or duel options.");
                    return;
                }
                if (p2.getDuel().notEnoughRoom())
                {
                    player.getPackets().sendMessage("Other player dosen't have enough inventory space for the stake and/or duel options.");
                    return;
                }
                if (ruleEnabled(RULE.NO_RANGE) && ruleEnabled(RULE.NO_MELEE) && ruleEnabled(RULE.NO_MAGIC))
                {
                    player.getPackets().modifyText("You cannot have no Ranged, no Melee and no Magic - how would you fight?", 631, 28);
                    p2.getPackets().modifyText("You cannot have no Ranged, no Melee and no Magic - how would you fight?", 631, 28);
                    return;
                }
                this.status = 1;
            }
            if (status == 1)
            {
                player.getPackets().modifyText("Waiting for other player...", 631, 28);
                p2.getPackets().modifyText("Other player has accepted...", 631, 28);
                if (p2.getDuel().getStatus() == 1)
                {
                    displayConfirmation();
                    p2.getDuel().displayConfirmation();
                }
                return;
            }
            if (status == 2 && p2.getDuel().getStatus() == 2)
            {
                player.getPackets().modifyText("Waiting for other player...", 626, 45);
                p2.getPackets().modifyText("Other player has accepted...", 626, 45);
                status = 3;
                return;
            }
            if (p2.getDuel().getStatus() == 3)
                status = 3;

            if (status == 3 && p2.getDuel().getStatus() == 3)
            {
                status = 4;
                p2.getDuel().setStatus(4);
                player.getPackets().softCloseInterfaces();
                p2.getPackets().softCloseInterfaces();
                player.getPackets().sendMessage("You accept the stake and duel options.");
                p2.getPackets().sendMessage("You accept the stake and duel options.");
                if (ruleEnabled(RULE.NO_MOVEMENT))
                {
                    player.setTemporaryAttribute("unmovable", true);
                    p2.setTemporaryAttribute("unmovable", true);
                }
                Location[] teleports = getArenaTeleport();
                int random = Misc.random(1);
                player.teleport(random == 0 ? teleports[0] : teleports[1]);
                p2.teleport(random == 0 ? teleports[1] : teleports[0]);
                player.getPackets().setArrowOnEntity(10, p2.getIndex());
                p2.getPackets().setArrowOnEntity(10, player.getIndex());
                resetPlayerVariables(true);
                removeWornItems();
                p2.getDuel().removeWornItems();
                beginCountdown(false);
                p2.getDuel().beginCountdown(true);
            }
        }

        public void removeWornItems()
        {
            ItemData.EQUIP[] slot = {ItemData.EQUIP.HAT, ItemData.EQUIP.CAPE, ItemData.EQUIP.AMULET,
                                     ItemData.EQUIP.WEAPON, ItemData.EQUIP.CHEST, ItemData.EQUIP.SHIELD,
                                     ItemData.EQUIP.LEGS, ItemData.EQUIP.HANDS, ItemData.EQUIP.FEET,
                                     ItemData.EQUIP.RING, ItemData.EQUIP.ARROWS};
            RULE[] rule = {RULE.HAT, RULE.CAPE, RULE.AMULET, RULE.WEAPON, RULE.BODY, RULE.SHIELD,
                           RULE.LEGS, RULE.GLOVES, RULE.BOOTS, RULE.RING, RULE.ARROWS};
            for (int j = 0; j < rule.Length; j++)
            {
                if (ruleEnabled(rule[j]))
                {
                    player.getEquipment().unequipItem(slot[j]);
                }
            }
        }

        private void beginCountdown(bool b)
        {
            Event beginCountdownEvent = new Event(1000);
            beginCountdownEvent.setAction(() =>
            {
                beginCountdownEvent.stop();
                countdown();
            });
            Server.registerEvent(beginCountdownEvent);
        }

        private void countdown()
        {
            status = 5;
            int countdownStart = 3; //start countdown at 3.
            Event countdownEvent = new Event(1000);
            countdownEvent.setAction(() =>
            {
                if (countdownStart > 0)
                {
                    player.setForceText("" + countdownStart--);
                }
                else
                {
                    player.setForceText("Fight!");
                    status = 6;
                    countdownEvent.stop();
                }
            });
            Server.registerEvent(countdownEvent);
        }

        private Location[] getArenaTeleport()
        {
            int arenaChoice = Misc.random(2);
            Location[] locations = new Location[2];
            int[] arenaBoundariesX = { 3337, 3367, 3336 };
            int[] arenaBoundariesY = { 3246, 3227, 3208 };
            int[] maxOffsetX = { 14, 14, 16 };
            int[] maxOffsetY = { 10, 10, 10 };
            int finalX = arenaBoundariesX[arenaChoice] + Misc.random(maxOffsetX[arenaChoice]);
            int finalY = arenaBoundariesY[arenaChoice] + Misc.random(maxOffsetY[arenaChoice]);
            locations[0] = new Location(finalX, finalY, 0);
            if (ruleEnabled(RULE.NO_MOVEMENT))
            {
                int direction = Misc.random(1);
                if (direction == 0)
                {
                    finalX--;
                }
                else
                {
                    finalY++;
                }
            }
            else
            {
                finalX = arenaBoundariesX[arenaChoice] + Misc.random(maxOffsetX[arenaChoice]);
                finalY = arenaBoundariesY[arenaChoice] + Misc.random(maxOffsetY[arenaChoice]);
            }
            locations[1] = new Location(finalX, finalY, 0);
            return locations;
        }

        public int getExtraSlots()
        {
            int slots = 0;
            ItemData.EQUIP[] slot = { ItemData.EQUIP.HAT, ItemData.EQUIP.CAPE, ItemData.EQUIP.AMULET,
                                      ItemData.EQUIP.WEAPON, ItemData.EQUIP.CHEST, ItemData.EQUIP.SHIELD,
                                      ItemData.EQUIP.LEGS, ItemData.EQUIP.HANDS, ItemData.EQUIP.FEET,
                                      ItemData.EQUIP.RING, ItemData.EQUIP.ARROWS};
            RULE[] rule = {RULE.HAT, RULE.CAPE, RULE.AMULET, RULE.WEAPON, RULE.BODY, RULE.SHIELD,
                           RULE.LEGS, RULE.GLOVES, RULE.BOOTS, RULE.RING, RULE.ARROWS};

            for (int j = 0; j < rule.Length; j++)
            {
                if (ruleEnabled(rule[j]))
                {
                    slots += player.getEquipment().getSlot(slot[j]).getItemId() > 0 ? 1 : 0;
                }
            }
            return slots;
        }

        private void displayConfirmation()
        {
            this.status = 2;
            int duringDuelOffset = 33;
            int beforeDuelOffset = 41;
            bool wornItemWarning = false;
            for (int i = 28; i <= 44; i++)
            {
                if (i != 32)
                {
                    player.getPackets().modifyText("", 626, i);
                }
            }
            player.getPackets().modifyText("Modified stats will be restored.", 626, beforeDuelOffset);
            beforeDuelOffset++;
            if (beforeDuelOffset == 42)
            {
                beforeDuelOffset = 28;
            }
            int ruleCount = 0;
            foreach (RULE rule in Enum.GetValues(typeof(RULE)))
            {
                if (ruleEnabled(rule))
                {
                    if (rule == RULE.NOTHING) break; //no rules were set.
                    if (rule == RULE.WEAPON)
                    {
                        player.getPackets().modifyText("You can't use two-handed weapons, like bows.", 626, duringDuelOffset);
                        duringDuelOffset++;
                    }
                    if (DUEL_TEXT_VAR[ruleCount] == 0)
                    {
                        if (rule >= RULE.HAT && rule <= RULE.ARROWS)
                        {
                            if (wornItemWarning)
                            {
                                continue;
                            }
                            else
                            {
                                wornItemWarning = true;
                            }
                        }
                        player.getPackets().modifyText(DUEL_TEXT[ruleCount], 626, beforeDuelOffset);
                        beforeDuelOffset++;
                        if (beforeDuelOffset == 42)
                        {
                            beforeDuelOffset = 28;
                        }
                    }
                    else
                    {
                        if (DUEL_TEXT_VAR[ruleCount] == 1)
                        {
                            player.getPackets().modifyText(DUEL_TEXT[ruleCount], 626, duringDuelOffset);
                            duringDuelOffset++;
                        }
                    }
                }
                ruleCount++;
            }
            p2.getPackets().modifyText("", 626, 45); // Accepted text.
            if (getAmountOfItems() > 0)
            {
                player.getPackets().modifyText("", 626, 25); // 'Absoloutely nothing!' text.
                p2.getPackets().modifyText("", 626, 26); // 'Absoloutely nothing!' text.
            }
            player.getPackets().displayInterface(626);
        }

        public void resetPlayerVariables(bool justThisPlayer)
        {
            player.setTarget(null);
            player.setAttacker(null);
            player.getSpecialAttack().resetSpecial();
            player.setLastkiller(null);
            player.setEntityFocus(65535);
            player.setDead(false);
            player.clearKillersHits();
            player.setVengeance(false);
            player.setFrozen(false);
            player.setSkullCycles(0);
            player.setLastVengeanceTime(0);
            player.setTeleblockTime(0);
            player.setPoisonAmount(0);
            player.removeTemporaryAttribute("willDie");
            player.removeTemporaryAttribute("unmovable");
            player.removeTemporaryAttribute("teleblocked");
            foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                player.getSkills().setCurLevel(skill, player.getSkills().getMaxLevel(skill));
            player.getPackets().sendSkillLevels();
            Prayer.deactivateAllPrayers(player);
            if (!justThisPlayer)
            {
                p2.setTarget(null);
                p2.setAttacker(null);
                p2.getSpecialAttack().resetSpecial();
                p2.setLastkiller(null);
                p2.setEntityFocus(65535);
                p2.setDead(false);
                p2.clearKillersHits();
                p2.setVengeance(false);
                p2.setFrozen(false);
                p2.setSkullCycles(0);
                p2.setLastVengeanceTime(0);
                p2.setTeleblockTime(0);
                p2.setPoisonAmount(0);
                p2.removeTemporaryAttribute("willDie");
                p2.removeTemporaryAttribute("unmovable");
                p2.removeTemporaryAttribute("teleblocked");
                foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                    p2.getSkills().setCurLevel(skill, p2.getSkills().getMaxLevel(skill));
                p2.getPackets().sendSkillLevels();
                Prayer.deactivateAllPrayers(p2);
            }
        }

        public static void teleportDuelArenaHome(Player player)
        {
            player.teleport(new Location(3360 + Misc.random(19), 3274 + Misc.random(3), 0));
        }

        public void finishDuel(bool lost, bool forfeit)
        {
            status = 8;
            p2.getDuel().setStatus(8);
            if (forfeit)
            {
                p2.getPackets().sendMessage(player.getLoginDetails().getUsername() + " has forfeited the duel!");
            }
            if (lost)
            {
                p2.getDuel().setWinner(p2);
                p2.getDuel().setWinnings(items);
                p2.getPackets().sendMessage("Well done! You have defeated " + player.getLoginDetails().getUsername() + "!");
                player.getPackets().sendMessage("Oh dear you are dead!");
            }
            teleportDuelArenaHome(player);
            teleportDuelArenaHome(p2);
            p2.getPackets().setArrowOnEntity(10, -1);
            player.getPackets().setArrowOnEntity(10, -1);
            p2.removeTemporaryAttribute("unmovable");
            player.removeTemporaryAttribute("unmovable");
            p2.getPackets().modifyText("" + player.getSkills().getCombatLevel(), 634, 22);
            p2.getPackets().modifyText(player.getLoginDetails().getUsername(), 634, 23);
            object[] opts1 = new object[] { "", "", "", "", "", -1, 0, 6, 6, 136, 41549857 };
            p2.getPackets().displayInterface(634);
            if (p2.getDuel().getWinnings() != null)
            {
                p2.getPackets().setRightClickOptions(1026, 41549857, 0, 35);
                p2.getPackets().sendClientScript2(566, 149, opts1, "IviiiIsssss");
                p2.getPackets().sendItems(-1, 64000, 136, p2.getDuel().getWinnings());
            }
            resetPlayerVariables(false);
            player.setTemporaryAttribute("challengeUpdate", true);
            p2.setTemporaryAttribute("challengeUpdate", true);
            if (lost)
            {
                player.getPackets().closeInterfaces();
                p2.getDuel().giveBack();
            }
        }

        public void resetStatus()
        {
            if (status == 1 || p2.getDuel().getStatus() == 1)
            {
                status = 0;
                p2.getDuel().setStatus(0);
                player.getPackets().modifyText("", 631, 28);
                p2.getPackets().modifyText("", 631, 28);
            }
        }

        public void recieveWinnings(Player p)
        {
            if (!winner.Equals(p))
            {
                Misc.WriteError(p.getLoginDetails().getUsername() + " tried to claim stake winnings that weren't his.");
                return;
            }
            if (status != 8)
            {
                return;
            }
            for (int i = 0; i < winnings.Length; i++)
            {
                if (winnings[i] != null)
                {
                    if (!player.getInventory().addItem(winnings[i].getItemId(), winnings[i].getItemAmount()))
                    {
                        Misc.WriteError("Possible stake winnings dupe " + player.getLoginDetails().getUsername());
                    }
                    else
                    {
                        winnings[i] = null;
                    }
                }
            }
        }

        private RULE getArrayId(int buttonId)
        {
            int[] buttons = { 37, 38, 45, 46, 29, 30, 31, 32, 33, 34, 39, 40, 41, 42, 43, 44, 47, 48, 35, 36, 50, 51, 57, 58, 59, 61, 62, 63, 64, 67, 66, 65, 60, 52, 53 };
            RULE[] rule = { RULE.NO_FORFEIT,         RULE.NO_FORFEIT,         RULE.NO_MOVEMENT, RULE.NO_MOVEMENT,
                            RULE.NO_RANGE,           RULE.NO_RANGE,           RULE.NO_MELEE,    RULE.NO_MELEE,
                            RULE.NO_MAGIC,           RULE.NO_MAGIC,           RULE.NO_DRINKS,   RULE.NO_DRINKS,
                            RULE.NO_FOOD,            RULE.NO_FOOD,            RULE.NO_PRAYER,   RULE.NO_PRAYER,
                            RULE.OBSTACLES,          RULE.OBSTACLES,          RULE.FUN_WEAPONS, RULE.FUN_WEAPONS,
                            RULE.NO_SPECIAL_ATTACKS, RULE.NO_SPECIAL_ATTACKS, RULE.HAT,         RULE.CAPE,
                            RULE.AMULET,             RULE.WEAPON,             RULE.BODY,        RULE.SHIELD,
                            RULE.LEGS,               RULE.GLOVES,             RULE.BOOTS,       RULE.RING,
                            RULE.ARROWS,             RULE.SUMMONING,          RULE.SUMMONING };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttonId == buttons[i])
                {
                    return rule[i];
                }
            }
            return RULE.NOTHING;
        }

        public void forfeitDuel(int x, int y)
        {
            // if (status == 5 || status == 4) {
            AreaEvent forfeitDuelAreaEvent = new AreaEvent(player, x - 1, y - 1, x + 1, y + 1);
            forfeitDuelAreaEvent.setAction(() =>
            {
                if (ruleEnabled(RULE.NO_FORFEIT))
                {
                    player.getPackets().sendMessage("You cannot forfeit this duel!");
                    return;
                }
                player.getPackets().modifyText("Forfeit duel?", 228, 1);
                player.getPackets().modifyText("Yes", 228, 2);
                player.getPackets().modifyText("No", 228, 3);
                player.getPackets().sendChatboxInterface2(228);
            });
            Server.registerCoordinateEvent(forfeitDuelAreaEvent);
            //}
        }

        public bool stakeItem(int slot, int amount)
        {
            int itemId = player.getInventory().getItemInSlot(slot);
            bool stackable = ItemData.forId(itemId).isStackable();
            int stakeSlot = findItem(itemId);
            if (amount <= 0 || itemId == -1 || status > 2)
            {
                return false;
            }
            if (ItemData.forId(itemId).isPlayerBound())
            {
                player.getPackets().sendMessage("You cannot stake that item.");
                return false;
            }
            if (!stackable)
            {
                stakeSlot = findFreeSlot();
                if (stakeSlot == -1)
                {
                    player.getPackets().sendMessage("An error occured whilst trying to find free a stake slot.");
                    //theoretically this should never happen since if there are no slots available, you should have no inventory items.
                    return false;
                }
                if (amount > player.getInventory().getItemAmount(itemId))
                {
                    amount = player.getInventory().getItemAmount(itemId);
                }
                for (int i = 0; i < amount; i++)
                {
                    stakeSlot = findFreeSlot();
                    if (!player.getInventory().deleteItem(itemId) || stakeSlot == -1)
                    {
                        break;
                    }
                    items[stakeSlot] = new Item(itemId, 1);
                }
                resetStatus();
                refreshDuel();
                return true;
            }
            else if (stackable)
            {
                stakeSlot = findItem(itemId);
                if (stakeSlot == -1)
                {
                    stakeSlot = findFreeSlot();
                    if (stakeSlot == -1)
                    {
                        player.getPackets().sendMessage("An error occured whilst trying to find free a stake slot.");
                        return false;
                    }
                }
                if (amount > player.getInventory().getAmountInSlot(slot))
                    amount = player.getInventory().getAmountInSlot(slot);

                if (player.getInventory().deleteItem(itemId, amount))
                {
                    if (items[stakeSlot] == null)
                    {
                        items[stakeSlot] = new Item(itemId, amount);
                    }
                    else
                    {
                        if (items[stakeSlot].getItemId() == itemId)
                        {
                            items[stakeSlot].setItemId(itemId);
                            items[stakeSlot].setItemAmount(items[stakeSlot].getItemAmount() + amount);
                        }
                    }
                    resetStatus();
                    refreshDuel();
                    return true;
                }
            }
            return false;
        }

        public void removeItem(int slot, int amount)
        {
            if (status > 2 || items[slot] == null)
            {
                return;
            }
            int itemId = getItemInSlot(slot);
            int tradeSlot = findItem(itemId);
            bool stackable = ItemData.forId(itemId).isStackable();
            if (tradeSlot == -1)
            {
                Misc.WriteError("user tried to remove non-existing item from duel! " + player.getLoginDetails().getUsername());
                return;
            }
            if (amount > getItemAmount(itemId))
                amount = getItemAmount(itemId);

            if (!stackable)
            {
                for (int i = 0; i < amount; i++)
                {
                    tradeSlot = findItem(itemId);
                    if (player.getInventory().addItem(itemId, amount))
                    {
                        items[tradeSlot].setItemAmount(getAmountInSlot(tradeSlot) - amount);
                        if (getAmountInSlot(tradeSlot) <= 0)
                        {
                            items[tradeSlot] = null;
                        }
                    }
                }
                resetStatus();
                refreshDuel();
            }
            else
            {
                tradeSlot = findItem(itemId);
                if (player.getInventory().addItem(itemId, amount))
                {
                    items[tradeSlot].setItemAmount(getAmountInSlot(tradeSlot) - amount);
                    if (getAmountInSlot(tradeSlot) <= 0)
                    {
                        items[tradeSlot] = null;
                    }
                    p2.getPackets().tradeWarning(tradeSlot);
                }
            }
            resetStatus();
            refreshDuel();
        }

        public Item[] getWinnings()
        {
            return winnings;
        }

        public void setWinnings(Item[] items)
        {
            Item[] winnings = new Item[28];
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() > 0)
                    {
                        winnings[j] = items[i];
                        j++;
                    }
                }
            }
            this.winnings = winnings;
        }

        public void setWinner(Player p)
        {
            this.winner = p;
        }

        public Player getWinner()
        {
            return winner;
        }

        public bool ruleEnabled(RULE i)
        {
            int bits = (int)i;
            return (rules & bits) == bits;
        }

        public void setRule(RULE rule, bool set)
        {
            if (set)
                rules |= (int)rule;
            else
                rules &= ~(int)rule;
        }

        public int getStatus()
        {
            return status;
        }

        public void setStatus(int status)
        {
            this.status = status;
        }

        public int getTotalFreeSlots()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    j++;
                }
            }
            return j;
        }

        public int findFreeSlot()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool hasItem(int itemId)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool hasItemAmount(int itemId, int amount)
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        j += items[i].getItemAmount();
                    }
                }
            }
            return j >= amount;
        }

        public int findItem(int itemId)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int getItemAmount(int itemId)
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() == itemId)
                    {
                        j += items[i].getItemAmount();
                    }
                }
            }
            return j;
        }

        public int getAmountOfItems()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() > -1)
                    {
                        j++;
                    }
                }
            }
            return j;
        }

        public bool stakingOverMaxAmount()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    int id = items[i].getItemId();
                    // if you're both staking the item
                    long l1 = (long)p2.getDuel().getItemAmount(id) + (long)items[i].getItemAmount();
                    // you're staking the item, p2 has it in inventory
                    long l2 = (long)p2.getInventory().getItemAmount(id) + (long)items[i].getItemAmount();
                    if (l1 > int.MaxValue || l2 > int.MaxValue)
                    {
                        return true;
                    }
                }
            }
            for (int i = 0; i < p2.getDuel().getItems().Length; i++)
            {
                if (p2.getDuel().getSlot(i) != null)
                {
                    // p2 is staking the item, you have it in inventory
                    long l3 = (long)p2.getDuel().getSlot(i).getItemAmount() + (long)player.getInventory().getItemAmount(p2.getDuel().getSlot(i).getItemId());
                    if (l3 > int.MaxValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool notEnoughRoom()
        {
            int j = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    if (items[i].getItemId() > -1)
                    {
                        if (items[i].getDefinition().isStackable())
                        {
                            if (player.getInventory().hasItem(items[i].getItemId()))
                            {
                                continue;
                            }
                        }
                        j++;
                    }
                }
            }
            for (int i = 0; i < p2.getDuel().getItems().Length; i++)
            {
                if (p2.getDuel().getSlot(i) != null)
                {
                    if (p2.getDuel().getSlot(i).getItemId() > -1)
                    {
                        if (p2.getDuel().getSlot(i).getDefinition().isStackable())
                        {
                            if (player.getInventory().hasItem(p2.getDuel().getSlot(i).getItemId()) || hasItem(p2.getDuel().getSlot(i).getItemId()))
                            {
                                continue;
                            }
                        }
                        j++;
                    }
                }
            }
            j += getExtraSlots();
            return j > player.getInventory().getTotalFreeSlots();
        }

        private Item[] getItems()
        {
            return items;
        }

        public int getAmountInSlot(int slot)
        {
            return items[slot].getItemAmount();
        }

        public int getItemInSlot(int slot)
        {
            return items[slot].getItemId();
        }

        public Item getSlot(int slot)
        {
            return items[slot];
        }

        public Player getPlayer2()
        {
            return p2;
        }
    }
}