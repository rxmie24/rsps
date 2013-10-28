using RS2.Server.clans;
using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills.cooking;
using RS2.Server.player.skills.crafting;
using RS2.Server.player.skills.fletching;
using RS2.Server.player.skills.herblore;
using RS2.Server.player.skills.smithing;
using RS2.Server.util;
using System;

namespace RS2.Server.packethandler
{
    internal class InterfaceOptions : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.ENTER_AMOUNT:
                    handleEnterAmount(player, packet);
                    break;

                case PacketHandlers.PacketId.ENTER_TEXT:
                    handleEnterText(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_1:
                    handleClickOne(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_2:
                    handleClickTwo(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_3:
                    handleClickThree(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_4:
                    handleClickFour(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_5:
                    handleClickFive(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_6:
                    handleClickSix(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_7:
                    handleClickSeven(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_8:
                    handleClickEight(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_9:
                    handleClickNine(player, packet);
                    break;

                case PacketHandlers.PacketId.INTERFACE_CLICK_10:
                    handleClickTen(player, packet);
                    break;

                case PacketHandlers.PacketId.GE_SEARCH:
                    handleGeSearch(player, packet);
                    break;
            }
        }

        private void handleEnterAmount(Player player, Packet packet)
        {
            if (player.getTemporaryAttribute("interfaceVariable") == null)
            {
                player.getPackets().sendMessage("An error occured, please try again.");
                return;
            }
            EnterVariable var = (EnterVariable)player.getTemporaryAttribute("interfaceVariable");
            int craftType = (int)(player.getTemporaryAttribute("craftType") == null ? -1 : (int)player.getTemporaryAttribute("craftType")); // 'Category' of item to craft
            int amount = packet.readInt();
            switch (var.getInterfaceId())
            {
                case 675: // Jewellery crafting
                    Jewellery.makeJewellery(player, var.getSlot(), amount, true);
                    break;

                case 304: // Dragonhide crafting.
                    int leatherType = (int)(player.getTemporaryAttribute("leatherCraft") == null ? -1 : (int)player.getTemporaryAttribute("leatherCraft")); // Type of leather to craft.
                    switch (var.getSlot())
                    {
                        case 0:
                            if (leatherType != -1)
                            {
                                Leather.craftDragonHide(player, 1, 0, leatherType, true); // Body
                                break;
                            }
                            else if (craftType == 6)
                            { // Ball of wool
                                Spinning.craftSpinning(player, amount, 0, true);
                                break;
                            }
                            break;

                        case 1:
                            if (craftType == 6)
                            { // Bowstring
                                Spinning.craftSpinning(player, amount, 1, true);
                                break;
                            }
                            break;

                        case 2:
                            if (craftType == 6)
                            { // Crossbow string
                                Spinning.craftSpinning(player, amount, 2, true);
                                break;
                            }
                            break;

                        case 4:
                            Leather.craftDragonHide(player, 1, 4, leatherType, true); // Vambraces
                            break;

                        case 8:
                            Leather.craftDragonHide(player, 1, 8, leatherType, true); // Chaps
                            break;
                    }
                    break;

                case 303:
                    switch (var.getSlot())
                    {
                        case 120: // Unholy symbol
                            Silver.newSilverItem(player, amount, var.getSlot(), true);
                            break;

                        case 121: // Tiara
                            Silver.newSilverItem(player, amount, var.getSlot(), true);
                            break;
                    }
                    break;

                case 154: // Craft normal leather.
                    if (var.getSlot() >= 28 && var.getSlot() <= 34)
                    {
                        Leather.craftNormalLeather(player, var.getSlot(), amount, true);
                        break;
                    }
                    break;

                case 542: // Glassblowing.
                    switch (var.getSlot())
                    {
                        case 40: // Make X beer glass.
                            Glass.craftGlass(player, amount, 0, true);
                            break;

                        case 41: // Make X candle lantern.
                            Glass.craftGlass(player, amount, 1, true);
                            break;

                        case 42: // Make X oil lamp.
                            Glass.craftGlass(player, amount, 2, true);
                            break;

                        case 38: // Make X vial.
                            Glass.craftGlass(player, amount, 3, true);
                            break;

                        case 44: // Make X Fishbowl
                            Glass.craftGlass(player, amount, 4, true);
                            break;

                        case 39: // Make X orb.
                            Glass.craftGlass(player, amount, 5, true);
                            break;

                        case 43: // Make X lantern lens
                            Glass.craftGlass(player, amount, 6, true);
                            break;

                        case 45: // Make X dorgeshuun light orb.
                            Glass.craftGlass(player, amount, 7, true);
                            break;
                    }
                    break;

                case 763: // Bank inventory - X.
                    player.getBank().setLastXAmount(amount);
                    player.getBank().deposit(var.getSlot(), amount);
                    player.getBank().refreshBank();
                    break;

                case 762: // Bank - X.
                    player.getBank().setLastXAmount(amount);
                    player.getBank().withdraw(var.getSlot(), amount);
                    player.getBank().refreshBank();
                    break;

                case 336: // Trade/stake inventory - trade X.
                    if (player.getTrade() != null)
                    {
                        player.getTrade().tradeItem(var.getSlot(), amount);
                        break;
                    }
                    if (player.getDuel() != null)
                    {
                        player.getDuel().stakeItem(var.getSlot(), amount);
                        break;
                    }
                    break;

                case 631:
                    if (player.getDuel() != null)
                    {
                        player.getDuel().removeItem(var.getSlot(), amount);
                        break;
                    }
                    break;

                case 335: // Trade/stake interface - remove X.
                    player.getTrade().removeItem(var.getSlot(), amount);
                    break;

                case 620: // Shop - buy X.
                    player.getShopSession().buyItem(var.getSlot(), amount);
                    break;

                case 105: //Grand Exchange custom quantity and price inputs.
                    switch (var.getSlot())
                    {
                        case 0: //custom enter quantity
                            player.getGESession().setCustomAmount(amount);
                            break;

                        case 1: //custom enter per price
                            player.getGESession().setCustomPrice(amount);
                            break;
                    }
                    break;

                case 305: // What would you like to make? - 4 options
                    if (player.getTemporaryAttribute("fletchType") == null)
                    {
                        return;
                    }
                    int logType = (int)player.getTemporaryAttribute("fletchType");
                    switch (var.getSlot())
                    {
                        case 0:
                            MakeBows.cutLog(player, amount, logType, 0, (player.getTemporaryAttribute("stringingBow") == null ? false : (bool)player.getTemporaryAttribute("stringingBow")), true);
                            break;

                        case 1:
                            MakeBows.cutLog(player, amount, logType, 1, (player.getTemporaryAttribute("stringingBow") == null ? false : (bool)player.getTemporaryAttribute("stringingBow")), true);
                            break;

                        case 2:
                            MakeBows.cutLog(player, amount, 0, 2, false, true);
                            break;

                        case 3:
                            MakeBows.cutLog(player, amount, 0, 3, false, true);
                            break;
                    }
                    break;

                case 306: // What would you like to make? - 5 options
                    switch (var.getSlot())
                    {
                        case 0:
                            Clay.craftClay(player, amount, craftType, var.getSlot(), true);
                            break;

                        case 1:
                            Clay.craftClay(player, amount, craftType, var.getSlot(), true);
                            break;

                        case 2:
                            Clay.craftClay(player, amount, craftType, var.getSlot(), true);
                            break;

                        case 3:
                            Clay.craftClay(player, amount, craftType, var.getSlot(), true);
                            break;

                        case 4:
                            Clay.craftClay(player, amount, craftType, var.getSlot(), true);
                            break;
                    }
                    break;

                case 309: // What would you like to make - 1 option
                    if (var.getSlot() >= 50 && var.getSlot() <= 60)
                    { // Cut gem
                        Jewellery.cutGem(player, craftType, amount, true);
                        break;
                    }
                    else if (var.getSlot() >= 100 && var.getSlot() <= 110)
                    { // String amulet
                        Jewellery.stringAmulet(player, craftType, amount, true);
                        break;
                    }
                    switch (var.getSlot())
                    {
                        case 0:
                            MakeXbow.createXbow(player, amount, (int)(player.getTemporaryAttribute("bowType2") == null ? -1 : (int)player.getTemporaryAttribute("bowType2")), (bool)(player.getTemporaryAttribute("stringingBow") == null ? false : (bool)player.getTemporaryAttribute("stringingBow")), true);
                            break;

                        case 1:
                            MakeBows.cutLog(player, amount, (int)(player.getTemporaryAttribute("fletchType") == null ? -1 : (int)player.getTemporaryAttribute("fletchType")), (int)(player.getTemporaryAttribute("bowType") == null ? -1 : (int)player.getTemporaryAttribute("bowType")), true, true);
                            break;

                        case 2:
                            Herblore.grindIngredient(player, amount, true);
                            break;

                        case 3:
                            Herblore.makeUnfinishedPotion(player, amount, true);
                            break;

                        case 4:
                            Herblore.completePotion(player, amount, true);
                            break;

                        case 5:
                            Cooking.cookItem(player, amount, true, player.getTemporaryAttribute("cookingFireLocation") != null);
                            break;

                        case 6:
                            MakeAmmo.makeBoltTip(player, (int)(player.getTemporaryAttribute("boltTips") == null ? -1 : (int)player.getTemporaryAttribute("boltTips")), amount, true);
                            break;
                    }
                    break;

                case 311: // Smelting interface
                    switch (var.getSlot())
                    {
                        case 13: // Bronze
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 17: // Blurite
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 21: // Iron
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 25: // Silver
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 29: // Steel
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 33: // Gold
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 37: // Mithril
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 41: // Adamant
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;

                        case 45: // Rune
                            Smelting.smeltOre(player, var.getSlot(), true, amount);
                            break;
                    }
                    break;

                case 300: // Smithing interface.
                    if (player.getTemporaryAttribute("smithingItem") == null)
                    {
                        return;
                    }
                    SmithBar item = (SmithBar)player.getTemporaryAttribute("smithingItem");
                    if (item != null)
                    {
                        item.setAmount(amount);
                    }
                    Smithing.smithItem(player, var.getSlot(), amount, false);
                    break;
            }
            player.removeTemporaryAttribute("interfaceVariable");
        }

        private void handleEnterText(Player player, Packet packet)
        {
            if (player.getTemporaryAttribute("interfaceVariable") == null)
            {
                player.getPackets().sendMessage("An error occured, please try again.");
                return;
            }
            long textAsLong = packet.readLong();
            EnterVariable var = (EnterVariable)player.getTemporaryAttribute("interfaceVariable");
            switch (var.getInterfaceId())
            {
                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan != null)
                    {
                        clan.setClanName(Misc.longToPlayerName(textAsLong));
                        Server.getClanManager().updateClan(clan);
                        player.getPackets().modifyText(Misc.formatPlayerNameForDisplay(clan.getClanName()), 590, 22);
                        break;
                    }
                    player.getPackets().sendMessage("Please set up a clan channel before trying to change the name.");
                    break;
            }
            player.removeTemporaryAttribute("interfaceVariable");
        }

        private void handleClickOne(Player player, Packet packet)
        {
            int slot = packet.readShortA();
            int item = packet.readUShort();
            int childId = packet.readUShort();
            int interfaceId = packet.readUShort();
            if (slot < 0 || slot > 28 || player.isDead())
            {
                return;
            }
            Console.WriteLine("Click One Slot = " + slot);
            player.getPackets().closeInterfaces();
            Console.WriteLine("InterfaceOption 1: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 387: // Unequip item
                    if (slot < 14 && player.getEquipment().getItemInSlot((ItemData.EQUIP)slot) == item)
                    {
                        player.getEquipment().unequipItem((ItemData.EQUIP)slot);
                    }
                    break;
            }
        }

        private void handleClickTwo(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 2: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 105: // GE Interface
                    switch (child)
                    {
                        case 209: // "Collect" and "Collect-items" option
                            player.getGESession().collectSlot1(false);
                            break;

                        case 211: // Left box "Collect" option (coins)
                            player.getGESession().collectSlot2();
                            break;
                    }
                    break;

                case 154: // Craft normal leather.
                    Leather.craftNormalLeather(player, child, 5, true);
                    break;

                case 542: // Glassblowing.
                    switch (child)
                    {
                        case 40: // Make 5 beer glass.
                            Glass.craftGlass(player, 5, 0, true);
                            break;

                        case 41: // Make 5 candle lantern.
                            Glass.craftGlass(player, 5, 1, true);
                            break;

                        case 42: // Make 5 oil lamp.
                            Glass.craftGlass(player, 5, 2, true);
                            break;

                        case 38: // Make 5 vial.
                            Glass.craftGlass(player, 5, 3, true);
                            break;

                        case 44: // Make 5 Fishbowl
                            Glass.craftGlass(player, 5, 4, true);
                            break;

                        case 39: // Make 5 orb.
                            Glass.craftGlass(player, 5, 5, true);
                            break;

                        case 43: // Make 5 lantern lens
                            Glass.craftGlass(player, 5, 6, true);
                            break;

                        case 45: // Make 5 dorgeshuun light orb.
                            Glass.craftGlass(player, 5, 7, true);
                            break;
                    }
                    break;

                case 763: // Bank inventory - 5.
                    player.getBank().deposit(slot, 5);
                    player.getBank().refreshBank();
                    break;

                case 762: // Bank - 5.
                    player.getBank().withdraw(slot, 5);
                    player.getBank().refreshBank();
                    break;

                case 336: // Trade/stake inventory - trade 5.
                    if (player.getTrade() != null)
                    {
                        player.getTrade().tradeItem(slot, 5);
                        break;
                    }
                    if (player.getDuel() != null)
                    {
                        player.getDuel().stakeItem(slot, 5);
                        break;
                    }
                    break;

                case 631: // Duel interface - remove 5
                    if (player.getDuel() != null)
                    {
                        player.getDuel().removeItem(slot, 5);
                        break;
                    }
                    break;

                case 335: // Trade interface - remove 5.
                    player.getTrade().removeItem(slot, 5);
                    break;

                case 620: // Shop - buy 1.
                    player.getShopSession().buyItem(slot, 1);
                    break;

                case 621: // Shop - sell 1.
                    player.getShopSession().sellItem(slot, 1);
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - any friends.
                            clan.setEnterRights(Clan.ClanRank.FRIEND);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - any friends.
                            clan.setTalkRights(Clan.ClanRank.FRIEND);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 26: // "Who can share loot" - any friends.
                            clan.setLootRights(Clan.ClanRank.FRIEND);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickThree(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 3: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 154: // Craft normal leather.
                    Leather.craftNormalLeather(player, child, player.getInventory().getItemAmount(1741), true);
                    break;

                case 542: // Glassblowing.
                    int totalGlass = player.getInventory().getItemAmount(1775);
                    switch (child)
                    {
                        case 40: // Make all beer glass.
                            Glass.craftGlass(player, totalGlass, 0, true);
                            break;

                        case 41: // Make all candle lantern.
                            Glass.craftGlass(player, totalGlass, 1, true);
                            break;

                        case 42: // Make all oil lamp.
                            Glass.craftGlass(player, totalGlass, 2, true);
                            break;

                        case 38: // Make all vial.
                            Glass.craftGlass(player, totalGlass, 3, true);
                            break;

                        case 44: // Make all Fishbowl
                            Glass.craftGlass(player, totalGlass, 4, true);
                            break;

                        case 39: // Make all orb.
                            Glass.craftGlass(player, totalGlass, 5, true);
                            break;

                        case 43: // Make all lantern lens
                            Glass.craftGlass(player, totalGlass, 6, true);
                            break;

                        case 45: // Make all dorgeshuun light orb.
                            Glass.craftGlass(player, totalGlass, 7, true);
                            break;
                    }
                    break;

                case 763: // Bank inventory - 10.
                    player.getBank().deposit(slot, 10);
                    player.getBank().refreshBank();
                    break;

                case 762: // Bank - 10.
                    player.getBank().withdraw(slot, 10);
                    player.getBank().refreshBank();
                    break;

                case 336: // Trade/stake inventory - trade 10.
                    if (player.getTrade() != null)
                    {
                        player.getTrade().tradeItem(slot, 10);
                        break;
                    }
                    if (player.getDuel() != null)
                    {
                        player.getDuel().stakeItem(slot, 10);
                        break;
                    }
                    break;

                case 335: // Trade interface - remove 10.
                    player.getTrade().removeItem(slot, 10);
                    break;

                case 631: // Duel interface - remove 10.
                    if (player.getDuel() != null)
                    {
                        player.getDuel().removeItem(slot, 5);
                        break;
                    }
                    break;

                case 620: // Shop - buy 5.
                    player.getShopSession().buyItem(slot, 5);
                    break;

                case 621: // Shop - sell 5.
                    player.getShopSession().sellItem(slot, 5);
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - recruit.
                            clan.setEnterRights(Clan.ClanRank.RECRUIT);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - recruit.
                            clan.setTalkRights(Clan.ClanRank.RECRUIT);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 26: // "Who can share loot" - recruit.
                            clan.setLootRights(Clan.ClanRank.RECRUIT);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickFour(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 4: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 763: // Bank inventory - Custom amount.
                    player.getBank().deposit(slot, player.getBank().getLastXAmount());
                    player.getBank().refreshBank();
                    break;

                case 762: // Bank - Custom amount.
                    player.getBank().withdraw(slot, player.getBank().getLastXAmount());
                    player.getBank().refreshBank();
                    break;

                case 154: // Craft normal leather.
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(154, child));
                    break;

                case 542: // Glassblowing.
                    switch (child)
                    {
                        case 40: // Make X beer glass.
                        case 41: // Make X candle lantern.
                        case 42: // Make X oil lamp.
                        case 38: // Make X vial.
                        case 44: // Make X Fishbowl
                        case 39: // Make X orb.
                        case 43: // Make X lantern lens
                        case 45: // Make X dorgeshuun light orb.
                            player.getPackets().displayEnterAmount();
                            player.setTemporaryAttribute("interfaceVariable", new EnterVariable(542, child));
                            break;
                    }
                    break;

                case 336: // Trade/stake inventory - trade all.
                    if (player.getTrade() != null)
                    {
                        player.getTrade().tradeItem(slot, player.getInventory().getItemAmount(player.getInventory().getItemInSlot(slot)));
                        break;
                    }
                    if (player.getDuel() != null)
                    {
                        player.getDuel().stakeItem(slot, player.getInventory().getItemAmount(player.getInventory().getItemInSlot(slot)));
                        break;
                    }
                    break;

                case 335: // Trade interface - remove all.
                    player.getTrade().removeItem(slot, player.getTrade().getItemAmount(player.getTrade().getItemInSlot(slot)));
                    break;

                case 631: // Duel interface - remove All
                    if (player.getDuel() != null)
                    {
                        player.getDuel().removeItem(slot, player.getDuel().getItemAmount(player.getDuel().getItemInSlot(slot)));
                        break;
                    }
                    break;

                case 620: // Shop - buy 10.
                    player.getShopSession().buyItem(slot, 10);
                    break;

                case 621: // Shop - sell 10.
                    player.getShopSession().sellItem(slot, 10);
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - corporal.
                            clan.setEnterRights(Clan.ClanRank.CORPORAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - corporal.
                            clan.setTalkRights(Clan.ClanRank.CORPORAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - corporal.
                            clan.setKickRights(Clan.ClanRank.CORPORAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - corporal.
                            clan.setLootRights(Clan.ClanRank.CORPORAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickFive(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 5: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 763: // Bank inventory - X.
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                    break;

                case 762: // Bank - X.
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                    break;

                case 336: // Trade inventory - trade X.
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                    break;

                case 335: // Trade interface - remove X.
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                    break;

                case 631: // Duel interface - remove All
                    if (player.getDuel() != null)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                        break;
                    }
                    break;

                case 620: // Shop - buy X/buy 50.
                    if (player.getShopSession().isInMainStock())
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(interfaceId, slot));
                    }
                    else
                    {
                        player.getShopSession().buyItem(slot, 50);
                    }
                    break;

                case 621: // Shop - Sell 50.
                    player.getShopSession().sellItem(slot, 50);
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - sergeant.
                            clan.setEnterRights(Clan.ClanRank.SERGEANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - sergeant.
                            clan.setTalkRights(Clan.ClanRank.SERGEANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - sergeant.
                            clan.setKickRights(Clan.ClanRank.SERGEANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - sergeant.
                            clan.setLootRights(Clan.ClanRank.SERGEANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickSix(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 6: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 763: // Bank inventory - All.
                    player.getBank().deposit(slot, player.getInventory().getItemAmount(player.getInventory().getItemInSlot(slot)));
                    player.getBank().refreshBank();
                    break;

                case 762: // Bank - All.
                    player.getBank().withdraw(slot, player.getBank().getAmountInSlot(slot));
                    player.getBank().refreshBank();
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - lieutenant.
                            clan.setEnterRights(Clan.ClanRank.LIEUTENANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - lieutenant.
                            clan.setTalkRights(Clan.ClanRank.LIEUTENANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - lieutenant.
                            clan.setKickRights(Clan.ClanRank.LIEUTENANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - lieutenant.
                            clan.setLootRights(Clan.ClanRank.LIEUTENANT);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickSeven(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 7: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 762: // Bank - All but one.
                    player.getBank().withdraw(slot, player.getBank().getAmountInSlot(slot) - 1);
                    player.getBank().refreshBank();
                    break;

                case 336: // Trade inventory - trade all.
                    //player.getTrade().lendItem(slot);
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - captain.
                            clan.setEnterRights(Clan.ClanRank.CAPTAIN);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - captain.
                            clan.setTalkRights(Clan.ClanRank.CAPTAIN);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - captain.
                            clan.setKickRights(Clan.ClanRank.CAPTAIN);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - captain.
                            clan.setLootRights(Clan.ClanRank.CAPTAIN);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickEight(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 8: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - general.
                            clan.setEnterRights(Clan.ClanRank.GENERAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - general.
                            clan.setTalkRights(Clan.ClanRank.GENERAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - general.
                            clan.setKickRights(Clan.ClanRank.GENERAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - general.
                            clan.setLootRights(Clan.ClanRank.GENERAL);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickNine(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 9: interfaceId: " + interfaceId);
            switch (interfaceId)
            {
                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        player.getPackets().sendMessage("Please create your clan chat before changing settings.");
                        break;
                    }
                    switch (child)
                    {
                        case 23: // "Who can enter chat" - only me/owner.
                            clan.setEnterRights(Clan.ClanRank.OWNER);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - only me/owner.
                            clan.setTalkRights(Clan.ClanRank.OWNER);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 25: // // "Who can kick in chat" - only me/owner.
                            clan.setKickRights(Clan.ClanRank.OWNER);
                            player.getPackets().modifyText(clan.getRankString(clan.getKickRights()), 590, 25);
                            break;

                        case 26: // "Who can share loot" - only me/owner.
                            clan.setLootRights(Clan.ClanRank.OWNER);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;
            }
        }

        private void handleClickTen(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            int child = packet.readUShort();
            int slot = packet.readUShort();
            Console.WriteLine("InterfaceOption 10: interfaceId: " + interfaceId);
        }

        private void handleGeSearch(Player player, Packet packet)
        {
            int item = packet.readUShort();
            if (item < 0 || item > 16000)
            {
                return;
            }
            if (player.getGESession() == null)
            {
                // TODO close the search interface
                return;
            }
            player.getGESession().updateSearchItem(item);
        }
    }
}