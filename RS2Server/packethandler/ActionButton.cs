using RS2.Server.clans;
using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.definitions.areas;
using RS2.Server.grandexchange;
using RS2.Server.minigames.agilityarena;
using RS2.Server.minigames.barrows;
using RS2.Server.minigames.warriorguild;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills.cooking;
using RS2.Server.player.skills.crafting;
using RS2.Server.player.skills.farming;
using RS2.Server.player.skills.fletching;
using RS2.Server.player.skills.herblore;
using RS2.Server.player.skills.magic;
using RS2.Server.player.skills.prayer;
using RS2.Server.player.skills.slayer;
using RS2.Server.player.skills.smithing;
using System;

namespace RS2.Server.packethandler
{
    internal class ActionButton : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.CLOSE:
                    handleCloseButton(player, packet);
                    break;

                case PacketHandlers.PacketId.ACTIONBUTTON:
                case PacketHandlers.PacketId.ACTIONBUTTON2:
                    handleActionButton(player, packet);
                    break;

                case PacketHandlers.PacketId.ACTIONBUTTON3:
                    handleActionButton3(player, packet);
                    break;
            }
        }

        private void handleActionButton3(Player player, Packet packet)
        {
            int id = packet.readUShort();
            int interfaceId = packet.readUShort();
            int junk = packet.readLEShort();

            int logType = (int)(player.getTemporaryAttribute("fletchType") == null ? -1 : (int)player.getTemporaryAttribute("fletchType")); // Bows (Fletching).
            int ammoType = (int)(player.getTemporaryAttribute("ammoType") == null ? -1 : (int)player.getTemporaryAttribute("ammoType")); // Arrows (Fletching).
            int boltType = (int)(player.getTemporaryAttribute("ammoType2") == null ? -1 : (int)player.getTemporaryAttribute("ammoType2")); // Bolts (Fletching).
            int xbowType = (int)(player.getTemporaryAttribute("bowType2") == null ? -1 : (int)player.getTemporaryAttribute("bowType2")); // Xbows (Fletching).
            int bowType = (int)(player.getTemporaryAttribute("bowType") == null ? -1 : (int)player.getTemporaryAttribute("bowType")); // Longbow/Shortbow stringing (Fletching).
            int grindItem = (int)(player.getTemporaryAttribute("herbloreGrindItem") == null ? -1 : (int)player.getTemporaryAttribute("herbloreGrindItem")); // item to be grinded (Herblore)
            bool stringingBow = (bool)(player.getTemporaryAttribute("stringingBow") == null ? false : (bool)player.getTemporaryAttribute("stringingBow")); // Stringing bow/xbow (Fletching)
            int unfinishedPotion = (int)(player.getTemporaryAttribute("unfinishedPotion") == null ? -1 : (int)player.getTemporaryAttribute("unfinishedPotion")); // unfinished potion to make (Herblore)
            int completePotion = (int)(player.getTemporaryAttribute("completePotion") == null ? -1 : (int)player.getTemporaryAttribute("completePotion")); // unfinished potion to make (Herblore)
            int cookItem = (int)(player.getTemporaryAttribute("meatItem") == null ? -1 : (int)player.getTemporaryAttribute("meatItem")); // item to cook (Cooking)
            int dialogueStatus = (int)(player.getTemporaryAttribute("dialogue") == null ? -1 : (int)player.getTemporaryAttribute("dialogue")); // Dialogue status
            int craftType = (int)(player.getTemporaryAttribute("craftType") == null ? -1 : (int)player.getTemporaryAttribute("craftType")); // 'Category' of item to craft
            int leatherCraft = (int)(player.getTemporaryAttribute("leatherCraft") == null ? -1 : (int)player.getTemporaryAttribute("leatherCraft")); // Type of leather item to craft (high lvl hides)
            int boltTips = (int)(player.getTemporaryAttribute("boltTips") == null ? -1 : (int)player.getTemporaryAttribute("boltTips")); // Type of bolt tips to cut
            JewelleryTeleport.JewellerySlot js = (JewelleryTeleport.JewellerySlot)player.getTemporaryAttribute("jewelleryTeleport") == null ? null : (JewelleryTeleport.JewellerySlot)player.getTemporaryAttribute("jewelleryTeleport");
            Console.WriteLine("ACTIONBUTTON-3 " + id);
            if (JewelleryTeleport.teleport(player, id, js))
            {
                return;
            }
            switch (id)
            {
                case 2:
                    if (player.getDuel() != null)
                    {
                        if (player.getDuel().getStatus() == 6)
                        {
                            player.getDuel().finishDuel(true, true);
                            player.getPackets().sendMessage("You climb through the trapdoor and forfeit the duel.");
                            break;
                        }
                        break;
                    }
                    else
                        if (player.getTemporaryAttribute("barrowTunnel") != null)
                        {
                            Barrows.verifyEnterTunnel(player);
                            return;
                        }
                        else if (dialogueStatus == 1005)
                        {
                            Slayer.doDialogue(player, 1006);
                            break;
                        }
                        else if (dialogueStatus == 1009)
                        {
                            Slayer.doDialogue(player, 1010);
                            break;
                        }
                        else if (dialogueStatus == 1002)
                        {
                            Slayer.doDialogue(player, 1013);
                            break;
                        }
                        else if (dialogueStatus == 1017)
                        {
                            Slayer.doDialogue(player, 1019);
                            break;
                        }
                        else if (dialogueStatus == 1029)
                        {
                            Slayer.doDialogue(player, 1029);
                            break;
                        }
                        else if (dialogueStatus == 1053)
                        {
                            Slayer.doDialogue(player, 1055);
                            break;
                        }
                        else if (dialogueStatus > 1000)
                        {
                            Slayer.doDialogue(player, 1006);
                            break;
                        }
                        else if (dialogueStatus == 7)
                        {
                            AgilityArena.doDialogue(player, 7);
                            break;
                        }
                        else if (dialogueStatus == 29)
                        {
                            AgilityArena.doDialogue(player, 29);
                            break;
                        }
                        else if (dialogueStatus == 37)
                        {
                            AgilityArena.doDialogue(player, 37);
                            break;
                        }
                        else if (dialogueStatus == 46)
                        {
                            AgilityArena.doDialogue(player, 46);
                            break;
                        }
                        else if (dialogueStatus == 79)
                        {
                            WarriorGuild.talkToKamfreena(player, 79);
                            break;
                        }
                        else if (dialogueStatus == 103)
                        {
                            BrokenBarrows.showBobDialogue(player, 103);
                            break;
                        }
                        else if (dialogueStatus == 109)
                        {
                            BrokenBarrows.showBobDialogue(player, 109);
                            break;
                        }
                        else if (dialogueStatus == 159)
                        {
                            HomeArea.showAliDialogue(player, 159);
                            break;
                        }
                        else if (dialogueStatus == 207)
                        {
                            AlKharid.showAliDialogue(player, 207);
                            break;
                        }
                        else if (dialogueStatus == 242)
                        {
                            BoatOptions.showBentleyDialogue(player, 242);
                            break;
                        }
                        else if (dialogueStatus == 450)
                        {
                            FarmingAmulet.displayAllotmentOptions(player);
                            break;
                        }
                        else if (dialogueStatus == 451)
                        {
                            FarmingAmulet.teleportToPatch(player, 0);
                            break;
                        }
                        else if (dialogueStatus == 452)
                        {
                            FarmingAmulet.teleportToPatch(player, 4);
                            break;
                        }
                        else if (dialogueStatus == 453)
                        {
                            FarmingAmulet.teleportToPatch(player, 8);
                            break;
                        }
                    break;

                case 13:
                    if (logType != -1)
                    {
                        MakeBows.cutLog(player, 1, logType, 0, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pie dish
                        Clay.craftClay(player, 5, craftType, 1, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 8));
                        break;
                    }
                    else if (craftType == 6)
                    {// Crossbow string
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 2));
                        break;
                    }
                    break;

                case 12:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 5, logType, 0, stringingBow, true);
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 1, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pie dish
                        Clay.craftClay(player, 10, craftType, 1, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 1, 4, leatherCraft, true); // Vambraces
                        break;
                    }
                    else if (craftType == 6)
                    {// Bowstring
                        Spinning.craftSpinning(player, 1, 1, true);
                        break;
                    }
                    break;

                case 11:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 10, logType, 0, stringingBow, true);
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 1, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pie dish
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(306, 1));
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 5, 4, leatherCraft, true); // Vambraces
                        break;
                    }
                    else if (craftType == 6)
                    {// Bowstring
                        Spinning.craftSpinning(player, 5, 1, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Tiara
                        Silver.newSilverItem(player, 1, 121, true);
                        break;
                    }
                    break;

                case 10:
                    if (logType == 0)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 0));
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 5, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pot
                        Clay.craftClay(player, 1, craftType, 0, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 10, 4, leatherCraft, true); // Vambraces
                        break;
                    }
                    else if (craftType == 6)
                    {// Bowstring
                        Spinning.craftSpinning(player, 10, 1, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Tiara
                        Silver.newSilverItem(player, 5, 121, true);
                        break;
                    }
                    break;

                case 17:
                    if (logType != -1)
                    {
                        MakeBows.cutLog(player, 1, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay Bowl
                        Clay.craftClay(player, 5, craftType, 2, true);
                    }
                    break;

                case 16:
                    if (logType != -1)
                    {
                        MakeBows.cutLog(player, 5, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay Bowl
                        Clay.craftClay(player, 10, craftType, 2, true);
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 1, 8, leatherCraft, true); // Chaps
                        break;
                    }
                    else if (craftType == 6)
                    {// Crossbow string
                        Spinning.craftSpinning(player, 1, 2, true);
                        break;
                    }
                    break;

                case 15:
                    if (logType != -1)
                    {
                        MakeBows.cutLog(player, 10, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay Bowl
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(306, 2));
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 5, 8, leatherCraft, true); // Chaps
                        break;
                    }
                    else if (craftType == 6)
                    {// Crossbow string
                        Spinning.craftSpinning(player, 5, 2, true);
                        break;
                    }
                    break;

                case 14:
                    if (craftType == 1 || craftType == 2)
                    { // Clay pie dish
                        Clay.craftClay(player, 1, craftType, 1, true);
                        break;
                    }
                    else if (logType == 0 && !stringingBow)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 1));
                        break;
                    }
                    else if (logType > 0)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 1));
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 10, 8, leatherCraft, true); // Chaps
                        break;
                    }
                    else if (craftType == 6)
                    {// Crossbow string
                        Spinning.craftSpinning(player, 10, 2, true);
                        break;
                    }
                    break;

                case 9:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 1, logType, 2, false, true);
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 10, logType, 1, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pot
                        Clay.craftClay(player, 5, craftType, 0, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 4));
                        break;
                    }
                    else if (craftType == 6)
                    {// Bowstring
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 1));
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Tiara
                        Silver.newSilverItem(player, 10, 121, true);
                        break;
                    }
                    break;

                case 8:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 5, logType, 2, false, true);
                        break;
                    }
                    else if (logType > 0)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 1));
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pot
                        Clay.craftClay(player, 10, craftType, 0, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 1, 0, leatherCraft, true); // Body
                        break;
                    }
                    else if (craftType == 6)
                    {// Ball of wool
                        Spinning.craftSpinning(player, 1, 0, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Tiara
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(303, 121));
                        break;
                    }
                    else if (dialogueStatus < 1000)
                    {
                        Dialogue.doDialogue(player, dialogueStatus);
                        break;
                    }
                    break;

                case 7:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 10, logType, 2, false, true);
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 1, logType, 0, stringingBow, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay pot
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(306, 0));
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 5, 0, leatherCraft, true); // Body
                        break;
                    }
                    else if (craftType == 6)
                    {// Ball of wool
                        Spinning.craftSpinning(player, 5, 0, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Unholy symbol
                        Silver.newSilverItem(player, 1, 120, true);
                        break;
                    }
                    else if (dialogueStatus > 1000)
                    {
                        Slayer.doDialogue(player, dialogueStatus);
                        break;
                    }
                    else if (dialogueStatus < 1000)
                    {
                        Dialogue.doDialogue(player, dialogueStatus);
                        break;
                    }
                    break;

                case 6:
                    if (logType == 0 && !stringingBow)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 2));
                        break;
                    }
                    else if (stringingBow && bowType != -1)
                    {
                        MakeBows.cutLog(player, 1, logType, bowType, true, true);
                        break;
                    }
                    else if (boltTips > -1)
                    {
                        MakeAmmo.makeBoltTip(player, boltTips, 1, true);
                        break;
                    }
                    else if (xbowType != -1)
                    {
                        MakeXbow.createXbow(player, 1, xbowType, stringingBow, true);
                        break;
                    }
                    else if (grindItem != -1)
                    {
                        Herblore.grindIngredient(player, 1, true);
                        break;
                    }
                    else if (unfinishedPotion != -1)
                    {
                        Herblore.makeUnfinishedPotion(player, 1, true);
                        break;
                    }
                    else if (completePotion != -1)
                    {
                        Herblore.completePotion(player, 1, true);
                        break;
                    }
                    else if (cookItem != -1)
                    {
                        Cooking.cookItem(player, 1, true, player.getTemporaryAttribute("cookingFireLocation") != null);
                        break;
                    }
                    else if (logType > 0)
                    {
                        MakeBows.cutLog(player, 5, logType, 0, false, true);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        Leather.craftDragonHide(player, 10, 0, leatherCraft, true); // Body
                        break;
                    }
                    else if (craftType == 6)
                    {// Ball of wool
                        Spinning.craftSpinning(player, 10, 0, true);
                        break;
                    }
                    else if (craftType >= 50 && craftType <= 60)
                    { // Cut gem
                        Jewellery.cutGem(player, craftType, 1, true);
                        break;
                    }
                    else if (craftType >= 100 && craftType <= 110)
                    { // String amulet
                        Jewellery.stringAmulet(player, craftType, 1, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Unholy symbol
                        Silver.newSilverItem(player, 5, 120, true);
                        break;
                    }
                    else if (dialogueStatus > 1000)
                    {
                        Slayer.doDialogue(player, dialogueStatus);
                        break;
                    }
                    else if (dialogueStatus == 7)
                    {
                        AgilityArena.doDialogue(player, 41);
                        break;
                    }
                    else if (dialogueStatus == 242)
                    {
                        BoatOptions.showBentleyDialogue(player, 246);
                        break;
                    }
                    else if (dialogueStatus == 451 || dialogueStatus == 452 || dialogueStatus == 453)
                    {
                        FarmingAmulet.showOptions(player, 12622);
                        break;
                    }
                    else if (dialogueStatus < 1000)
                    {
                        Dialogue.doDialogue(player, dialogueStatus);
                        break;
                    }
                    break;

                case 5:
                    if (ammoType != -1)
                    {
                        MakeAmmo.createAmmo(player, 1, ammoType, false, true);
                        break;
                    }
                    else if (stringingBow && bowType != -1)
                    {
                        MakeBows.cutLog(player, 5, logType, bowType, true, true);
                        break;
                    }
                    else if (boltType != -1)
                    {
                        MakeAmmo.createAmmo(player, 1, boltType, true, true);
                        break;
                    }
                    else if (xbowType != -1)
                    {
                        MakeXbow.createXbow(player, 5, xbowType, stringingBow, true);
                        break;
                    }
                    else if (grindItem != -1)
                    {
                        Herblore.grindIngredient(player, 5, true);
                        break;
                    }
                    else if (unfinishedPotion != -1)
                    {
                        Herblore.makeUnfinishedPotion(player, 5, true);
                        break;
                    }
                    else if (completePotion != -1)
                    {
                        Herblore.completePotion(player, 5, true);
                        break;
                    }
                    else if (cookItem != -1)
                    {
                        Cooking.cookItem(player, 5, true, player.getTemporaryAttribute("cookingFireLocation") != null);
                        break;
                    }
                    else if (logType != -1)
                    {
                        MakeBows.cutLog(player, 10, logType, 0, false, true);
                        break;
                    }
                    else if (dialogueStatus == 1002)
                    {
                        Slayer.doDialogue(player, 1024);
                        break;
                    }
                    else if (dialogueStatus == 1053)
                    {
                        Slayer.doDialogue(player, 1061);
                        break;
                    }
                    else if (dialogueStatus > 1000)
                    {
                        Slayer.doDialogue(player, dialogueStatus);
                        break;
                    }
                    else if (dialogueStatus == 7)
                    {
                        AgilityArena.doDialogue(player, 34);
                        break;
                    }
                    else if (leatherCraft != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 0));
                        break;
                    }
                    else if (craftType == 6)
                    {// Ball of wool
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(304, 0));
                        break;
                    }
                    else if (craftType >= 50 && craftType <= 60)
                    { // Cut gem
                        Jewellery.cutGem(player, craftType, 5, true);
                        break;
                    }
                    else if (craftType >= 100 && craftType <= 110)
                    { // String amulet
                        Jewellery.stringAmulet(player, craftType, 5, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Unholy symbol
                        Silver.newSilverItem(player, 10, 120, true);
                        break;
                    }
                    else if (dialogueStatus == 46)
                    {
                        AgilityArena.doDialogue(player, 54);
                        break;
                    }
                    else if (dialogueStatus == 242)
                    {
                        BoatOptions.showBentleyDialogue(player, 245);
                        break;
                    }
                    else if (dialogueStatus == 451)
                    {
                        FarmingAmulet.teleportToPatch(player, 3);
                        break;
                    }
                    else if (dialogueStatus == 452)
                    {
                        FarmingAmulet.teleportToPatch(player, 7);
                        break;
                    }
                    else if (dialogueStatus == 453)
                    {
                        FarmingAmulet.teleportToPatch(player, 11);
                        break;
                    }
                    else if (dialogueStatus == 450)
                    {
                        player.getPackets().closeInterfaces();
                        break;
                    }
                    else if (dialogueStatus != -1 && dialogueStatus < 1000)
                    {
                        Dialogue.doDialogue(player, dialogueStatus);
                        break;
                    }
                    else if (boltTips > -1)
                    {
                        MakeAmmo.makeBoltTip(player, boltTips, 5, true);
                        break;
                    }
                    break;

                case 4:
                    if (ammoType != -1)
                    {
                        MakeAmmo.createAmmo(player, 5, ammoType, false, true);
                        break;
                    }
                    else if (boltType != -1)
                    {
                        MakeAmmo.createAmmo(player, 5, boltType, true, true);
                        break;
                    }
                    else if (xbowType != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 0));
                        break;
                    }
                    else if (stringingBow && bowType != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 1));
                        break;
                    }
                    else if (grindItem != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 2));
                        break;
                    }
                    else if (unfinishedPotion != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 3));
                        break;
                    }
                    else if (completePotion != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 4));
                        break;
                    }
                    else if (cookItem != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 5));
                        break;
                    }
                    else if (logType != -1)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 0));
                        break;
                    }
                    else if (craftType >= 50 && craftType <= 60)
                    { // Cut gem
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, craftType));
                        break;
                    }
                    else if (craftType >= 100 && craftType <= 110)
                    { // String amulet
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, craftType));
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Unholy symbol
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(303, 120));
                        break;
                    }
                    else if (dialogueStatus == 1053)
                    {
                        Slayer.doDialogue(player, 1059);
                        break;
                    }
                    else if (dialogueStatus == 1002)
                    {
                        Slayer.doDialogue(player, 1025);
                        break;
                    }
                    else if (dialogueStatus == 7)
                    {
                        AgilityArena.doDialogue(player, 24);
                        break;
                    }
                    else if (dialogueStatus == 46)
                    {
                        AgilityArena.doDialogue(player, 51);
                        break;
                    }
                    else if (dialogueStatus == 109)
                    {
                        BrokenBarrows.showBobDialogue(player, 114);
                        break;
                    }
                    else if (dialogueStatus == 140 || dialogueStatus == 141)
                    {
                        player.getPackets().closeInterfaces();
                        break;
                    }
                    else if (dialogueStatus == 159)
                    {
                        HomeArea.showAliDialogue(player, 177);
                        break;
                    }
                    else if (dialogueStatus == 242)
                    {
                        BoatOptions.showBentleyDialogue(player, 244);
                        break;
                    }
                    else if (boltTips > -1)
                    { // Cut bolt tips
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(309, 6));
                        break;
                    }
                    else if (dialogueStatus == 451)
                    {
                        FarmingAmulet.teleportToPatch(player, 2);
                        break;
                    }
                    else if (dialogueStatus == 452)
                    {
                        FarmingAmulet.teleportToPatch(player, 6);
                        break;
                    }
                    else if (dialogueStatus == 450)
                    {
                        FarmingAmulet.displayFruitTreeOptions(player);
                        break;
                    }
                    else if (dialogueStatus == 453)
                    {
                        FarmingAmulet.teleportToPatch(player, 10);
                        break;
                    }
                    break;

                case 3:
                    if (player.getDuel() != null)
                    {
                        player.getPackets().closeChatboxInterface();
                        break;
                    }
                    else if (ammoType != -1)
                    {
                        MakeAmmo.createAmmo(player, 10, ammoType, false, true);
                        break;
                    }
                    else if (boltType != -1)
                    {
                        MakeAmmo.createAmmo(player, 10, boltType, true, true);
                        break;
                    }
                    else if (stringingBow && bowType != -1)
                    {
                        MakeBows.cutLog(player, player.getInventory().getItemAmount(1777), logType, bowType, true, true);
                        break;
                    }
                    else if (xbowType != -1)
                    {
                        MakeXbow.createXbow(player, player.getInventory().getItemAmount(9438), xbowType, stringingBow, true);
                        break;
                    }
                    else if (grindItem != -1)
                    {
                        Herblore.grindIngredient(player, 28, true);
                        break;
                    }
                    else if (unfinishedPotion != -1)
                    {
                        Herblore.makeUnfinishedPotion(player, 28, true);
                        break;
                    }
                    else if (completePotion != -1)
                    {
                        Herblore.completePotion(player, 28, true);
                        break;
                    }
                    else if (cookItem != -1)
                    {
                        Cooking.cookItem(player, 28, true, player.getTemporaryAttribute("cookingFireLocation") != null);
                        break;
                    }
                    else if (bowType != -1 && logType != -1)
                    {
                        MakeBows.cutLog(player, 10, logType, bowType, false, true);
                        break;
                    }
                    else if (craftType >= 50 && craftType <= 60)
                    { // Cut gem
                        Jewellery.cutGem(player, craftType, 27, true);
                        break;
                    }
                    else if (craftType >= 100 && craftType <= 110)
                    { // String amulet
                        Jewellery.stringAmulet(player, craftType, 27, true);
                        break;
                    }
                    else if (craftType >= 120 && craftType <= 130)
                    { // Unholy symbol
                        Silver.newSilverItem(player, 27, 120, true);
                        break;
                    }
                    else if (dialogueStatus == 1017)
                    {
                        Slayer.doDialogue(player, 1017);
                        break;
                    }
                    else if (dialogueStatus == 1053)
                    {
                        Slayer.doDialogue(player, 1057);
                        break;
                    }
                    else if (dialogueStatus == 1002)
                    {
                        Slayer.doDialogue(player, 1021);
                        break;
                    }
                    else if (dialogueStatus > 1000)
                    {
                        Slayer.doDialogue(player, dialogueStatus);
                        break;
                    }
                    else if (dialogueStatus == 7)
                    {
                        AgilityArena.doDialogue(player, 17);
                        break;
                    }
                    else if (dialogueStatus == 29)
                    {
                        AgilityArena.doDialogue(player, 33);
                        break;
                    }
                    else if (dialogueStatus == 37)
                    {
                        AgilityArena.doDialogue(player, 39);
                        break;
                    }
                    else if (dialogueStatus == 46)
                    {
                        AgilityArena.doDialogue(player, 48);
                        break;
                    }
                    else if (dialogueStatus == 79)
                    {
                        WarriorGuild.talkToKamfreena(player, 80);
                        break;
                    }
                    else if (dialogueStatus == 103)
                    {
                        BrokenBarrows.showBobDialogue(player, 105);
                        break;
                    }
                    else if (dialogueStatus == 109)
                    {
                        BrokenBarrows.showBobDialogue(player, 111);
                        break;
                    }
                    else if (dialogueStatus == 159)
                    {
                        HomeArea.showAliDialogue(player, 162);
                        break;
                    }
                    else if (dialogueStatus == 207)
                    {
                        AlKharid.showAliDialogue(player, 210);
                        break;
                    }
                    else if (dialogueStatus == 242)
                    {
                        BoatOptions.showBentleyDialogue(player, 243);
                        break;
                    }
                    else if (boltTips > -1)
                    {
                        MakeAmmo.makeBoltTip(player, boltTips, player.getInventory().getItemAmount((int)FletchingData.GEMS[boltTips][0]), true);
                        break;
                    }
                    else if (dialogueStatus == 451)
                    {
                        FarmingAmulet.teleportToPatch(player, 1);
                        break;
                    }
                    else if (dialogueStatus == 450)
                    {
                        FarmingAmulet.displayTreeOptions(player);
                        break;
                    }
                    else if (dialogueStatus == 452)
                    {
                        FarmingAmulet.teleportToPatch(player, 5);
                        break;
                    }
                    else if (dialogueStatus == 453)
                    {
                        FarmingAmulet.teleportToPatch(player, 9);
                        break;
                    }
                    break;

                case 21:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 1, logType, 3, false, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay plant pot
                        Clay.craftClay(player, 5, craftType, 3, true);
                        break;
                    }
                    break;

                case 20:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 5, logType, 3, false, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay plant pot
                        Clay.craftClay(player, 10, craftType, 3, true);
                        break;
                    }
                    break;

                case 19:
                    if (logType == 0)
                    {
                        MakeBows.cutLog(player, 10, logType, 3, false, true);
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay plant pot
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(306, 3));
                        break;
                    }
                    break;

                case 18:
                    if (logType == 0)
                    {
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(305, 3));
                        break;
                    }
                    else if (craftType == 1 || craftType == 2)
                    { // Clay Bowl
                        Clay.craftClay(player, 1, craftType, 2, true);
                        break;
                    }
                    break;

                case 22:
                    if (craftType == 1 || craftType == 2)
                    { // Clay plant pot
                        Clay.craftClay(player, 1, craftType, 3, true);
                        break;
                    }
                    break;

                case 26:
                    if (craftType == 1 || craftType == 2)
                    { // Clay lid
                        Clay.craftClay(player, 1, craftType, 4, true);
                        break;
                    }
                    break;

                case 25:
                    if (craftType == 1 || craftType == 2)
                    { // Clay lid
                        Clay.craftClay(player, 5, craftType, 4, true);
                        break;
                    }
                    break;

                case 24:
                    if (craftType == 1 || craftType == 2)
                    { // Clay lid
                        Clay.craftClay(player, 10, craftType, 4, true);
                        break;
                    }
                    break;

                case 23:
                    if (craftType == 1 || craftType == 2)
                    { // Clay lid
                        player.getPackets().displayEnterAmount();
                        player.setTemporaryAttribute("interfaceVariable", new EnterVariable(306, 4));
                        break;
                    }
                    break;

                case 1:
                    switch (id)
                    {
                        case 1:
                            if (dialogueStatus == 1002)
                            {
                                Slayer.doDialogue(player, dialogueStatus);
                                break;
                            }
                            else if (dialogueStatus == 1053)
                            {
                                Slayer.doDialogue(player, dialogueStatus);
                                break;
                            }
                            break;
                    }
                    break;

                default:
                    Console.WriteLine("ACTIONBUTTON3 = " + id);
                    break;
            }
            if (dialogueStatus == -1)
            {
                player.getPackets().closeInterfaces();
            }
        }

        private void handleCloseButton(Player player, Packet packet)
        {
            if (player.getTrade() != null)
            {
                player.getTrade().decline();
            }
            if (player.getDuel() != null)
            {
                if (player.getDuel().getStatus() < 4)
                {
                    player.getDuel().declineDuel();
                    return;
                }
                else
                    if (player.getDuel().getStatus() == 8 && player.getDuel().getWinner().Equals(player))
                    {
                        player.getDuel().recieveWinnings(player);
                    }
            }
            player.getPackets().closeInterfaces();
        }

        private void handleActionButton(Player player, Packet packet)
        {
            int interfaceId = packet.readUShort();
            ushort buttonId = packet.readUShort();
            ushort buttonId2 = 0;
            if (packet.getLength() >= 6)
            {
                buttonId2 = packet.readUShort();
            }
            if (buttonId2 == 65535)
            {
                buttonId2 = 0;
            }
            Console.WriteLine("button = " + interfaceId + " " + buttonId + " " + buttonId2);
            switch (interfaceId)
            {
                case 389: // GE Item Search
                    if (player.getGESession() != null)
                    {
                        if (player.getGESession().getCurrentOffer() != null)
                        {
                            if (player.getGESession().getCurrentOffer() is BuyOffer)
                            {
                                player.getPackets().sendInterface(0, 752, 6, 137); // Removes the item search
                            }
                        }
                    }
                    break;

                case 374: // Tzhaar fight pits viewing orb
                    Server.getMinigames().getFightPits().useOrb(player, buttonId);
                    break;

                case 107: // GE Sell inventory.
                    if (player.getGESession() == null)
                    {
                        break;
                    }
                    switch (buttonId)
                    {
                        case 18: // Offer
                            player.getGESession().offerSellItem(buttonId2);
                            break;
                    }
                    break;

                case 105: // GE interface
                    if (player.getGESession() == null)
                    {
                        break;
                    }
                    switch (buttonId)
                    {
                        case 209: // Collect-notes
                            player.getGESession().collectSlot1(true);
                            break;

                        case 203: // Abort offer
                            player.getGESession().abortOffer();
                            break;

                        case 31: // Sell, box 1
                            player.getGESession().newSellOffer(0);
                            break;

                        case 30: // Buy, box 1
                            player.getGESession().newBuyOffer(0);
                            break;

                        case 18: // Check status, box 1
                            player.getGESession().checkOffer(0);
                            break;

                        case 46: // Buy, box 2
                            player.getGESession().newBuyOffer(1);
                            break;

                        case 47: // Sell, box 2
                            player.getGESession().newSellOffer(1);
                            break;

                        case 34: // Check status, box 2
                            player.getGESession().checkOffer(1);
                            break;

                        case 62: // Buy, box 3
                            player.getGESession().newBuyOffer(2);
                            break;

                        case 63: // Sell, box 3
                            player.getGESession().newSellOffer(2);
                            break;

                        case 50: // Check status, box 3
                            player.getGESession().checkOffer(2);
                            break;

                        case 81: // Buy, box 4
                            player.getGESession().newBuyOffer(3);
                            break;

                        case 82: // Sell, box 4
                            player.getGESession().newSellOffer(3);
                            break;

                        case 69: // Check status, box 4
                            player.getGESession().checkOffer(3);
                            break;

                        case 100: // Buy, box 5
                            player.getGESession().newBuyOffer(4);
                            break;

                        case 101: // Sell, box 5
                            player.getGESession().newSellOffer(4);
                            break;

                        case 88: // Check status, box 5
                            player.getGESession().checkOffer(4);
                            break;

                        case 119: // Buy, box 6
                            player.getGESession().newBuyOffer(5);
                            break;

                        case 120: // Sell, box 6
                            player.getGESession().newSellOffer(5);
                            break;

                        case 107: // Check status, box 6
                            player.getGESession().checkOffer(5);
                            break;

                        case 127: // The "back" button
                            player.getPackets().closeInterfaces();
                            player.setGESession(new GESession(player));
                            break;

                        case 194: // Search for item
                            player.getGESession().openItemSearch();
                            break;

                        case 159: // Increment amount by 1
                            player.getGESession().incrementAmount(1);
                            break;

                        case 157: // Decrease amount by 1
                            player.getGESession().decreaseAmount(1);
                            break;

                        case 162: // Increase amount by +1
                            player.getGESession().incrementAmount(1);
                            break;

                        case 164: // Increase amount by +10
                            player.getGESession().incrementAmount(10);
                            break;

                        case 166: // Increase amount by +100
                            player.getGESession().incrementAmount(100);
                            break;

                        case 168: // Increase amount by +1k (or All if selling)
                            player.getGESession().incrementAmount(1000);
                            break;

                        case 170: //custom enter quantity
                            player.getPackets().displayEnterAmount();
                            player.setTemporaryAttribute("interfaceVariable", new EnterVariable(105, 0));
                            break;

                        case 185: //custom enter per price
                            player.getPackets().displayEnterAmount();
                            player.setTemporaryAttribute("interfaceVariable", new EnterVariable(105, 1));
                            break;

                        case 177: // Set price to minimum
                            player.getGESession().setPrice(0);
                            break;

                        case 180: // Set price to medium
                            player.getGESession().setPrice(1);
                            break;

                        case 183: // Set price to maximum
                            player.getGESession().setPrice(2);
                            break;

                        case 171: // Set price -1
                            player.getGESession().setPrice(3);
                            break;

                        case 173: // Set price +1
                            player.getGESession().setPrice(4);
                            break;

                        case 190: // Confirm offer
                            player.getGESession().confirmOffer();
                            break;
                    }
                    break;

                case 161: // Slayer points interfaces
                case 163:
                case 164:
                    Slayer.handlePointsInterface(player, interfaceId, buttonId);
                    break;

                case 675: // Craft jewellery:
                    player.getPackets().displayEnterAmount();
                    player.setTemporaryAttribute("interfaceVariable", new EnterVariable(675, buttonId));
                    break;

                case 154: // Craft normal leather.
                    Leather.craftNormalLeather(player, buttonId, 1, true);
                    break;

                case 542: // Craft glass.
                    switch (buttonId)
                    {
                        case 40: // Make 1 beer glass.
                            Glass.craftGlass(player, 1, 0, true);
                            break;

                        case 41: // Make 1 candle lantern.
                            Glass.craftGlass(player, 1, 1, true);
                            break;

                        case 42: // Make 1 oil lamp.
                            Glass.craftGlass(player, 1, 2, true);
                            break;

                        case 38: // Make 1 vial.
                            Glass.craftGlass(player, 1, 3, true);
                            break;

                        case 44: // Make 1 Fishbowl
                            Glass.craftGlass(player, 1, 4, true);
                            break;

                        case 39: // Make 1 orb.
                            Glass.craftGlass(player, 1, 5, true);
                            break;

                        case 43: // Make 1 lantern lens
                            Glass.craftGlass(player, 1, 6, true);
                            break;

                        case 45: // Make 1 dorgeshuun light orb.
                            Glass.craftGlass(player, 1, 7, true);
                            break;
                    }
                    break;

                case 271: // Prayer tab.
                    if (!Prayer.canUsePrayer(player, buttonId))
                    {
                        Prayer.deactivateAllPrayers(player);
                        break;
                    }
                    switch (buttonId)
                    {
                        case 5: // Thick skin.
                            Prayer.togglePrayer(player, 1, 1);
                            break;

                        case 15: // Rock skin.
                            Prayer.togglePrayer(player, 1, 2);
                            break;

                        case 31: // Steel skin.
                            Prayer.togglePrayer(player, 1, 3);
                            break;

                        case 7: // Burst of strength.
                            Prayer.togglePrayer(player, 2, 1);
                            break;

                        case 17: // Superhuman strength.
                            Prayer.togglePrayer(player, 2, 2);
                            break;

                        case 33: // Ultimate strength.
                            Prayer.togglePrayer(player, 2, 3);
                            break;

                        case 9: // Clarity of thought.
                            Prayer.togglePrayer(player, 3, 1);
                            break;

                        case 19: // Improved reflexes.
                            Prayer.togglePrayer(player, 3, 2);
                            break;

                        case 35: // Incredible reflexes.
                            Prayer.togglePrayer(player, 3, 3);
                            break;

                        case 37: // Magic protect.
                            Prayer.togglePrayer(player, 4, 1);
                            break;

                        case 39: // Ranged protect.
                            Prayer.togglePrayer(player, 4, 2);
                            break;

                        case 41: // Melee protect.
                            Prayer.togglePrayer(player, 4, 3);
                            break;

                        case 47: // Retribution.
                            Prayer.togglePrayer(player, 4, 4);
                            break;

                        case 49: // Redemption.
                            Prayer.togglePrayer(player, 4, 5);
                            break;

                        case 51: // Smite.
                            Prayer.togglePrayer(player, 4, 6);
                            break;

                        case 55: // Chivalry.
                            Prayer.togglePrayer(player, 5, 1);
                            break;

                        case 57: // Piety.
                            Prayer.togglePrayer(player, 5, 2);
                            break;

                        case 25: // Protect item.
                            Prayer.togglePrayer(player, 6, 1);
                            break;

                        case 21: // Rapid restore
                            Prayer.togglePrayer(player, 7, 1);
                            break;

                        case 23: // Rapid heal.
                            Prayer.togglePrayer(player, 7, 2);
                            break;

                        case 11: // Sharp eye.
                            Prayer.togglePrayer(player, 8, 1);
                            break;

                        case 27: // Hawk Eye.
                            Prayer.togglePrayer(player, 8, 2);
                            break;

                        case 43: // Eagle Eye.
                            Prayer.togglePrayer(player, 8, 3);
                            break;

                        case 13: // Mystic will.
                            Prayer.togglePrayer(player, 9, 1);
                            break;

                        case 29: // Mystic Lore.
                            Prayer.togglePrayer(player, 9, 2);
                            break;

                        case 45: // Mystic Might.
                            Prayer.togglePrayer(player, 9, 3);
                            break;
                    }
                    break;

                case 90: // Staff attack interface.
                    switch (buttonId)
                    {
                        case 5: // Select spell (Magic XP)
                            MagicData.configureSelectSpellInterface(player);
                            break;

                        case 9: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            MagicData.cancelAutoCast(player, true);
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 388: // Ancient magic autocast select spell.
                    switch (buttonId)
                    {
                        case 0: // Smoke rush.
                            MagicData.setAutoCastSpell(player, 16, 8, true);
                            break;

                        case 1: // Shadow rush.
                            MagicData.setAutoCastSpell(player, 17, 12, true);
                            break;

                        case 2: // Blood rush.
                            MagicData.setAutoCastSpell(player, 18, 4, true);
                            break;

                        case 3: // Ice rush.
                            MagicData.setAutoCastSpell(player, 19, 0, true);
                            break;

                        case 4: // Smoke burst.
                            MagicData.setAutoCastSpell(player, 20, 10, true);
                            break;

                        case 5: // Shadow burst.
                            MagicData.setAutoCastSpell(player, 21, 14, true);
                            break;

                        case 6: // Blood burst.
                            MagicData.setAutoCastSpell(player, 22, 6, true);
                            break;

                        case 7: // Ice burst.
                            MagicData.setAutoCastSpell(player, 23, 2, true);
                            break;

                        case 8: // Smoke blitz.
                            MagicData.setAutoCastSpell(player, 24, 9, true);
                            break;

                        case 9: // Shadow blitz.
                            MagicData.setAutoCastSpell(player, 25, 13, true);
                            break;

                        case 10: // Blood blitz.
                            MagicData.setAutoCastSpell(player, 26, 5, true);
                            break;

                        case 11: // Ice blitz.
                            MagicData.setAutoCastSpell(player, 27, 1, true);
                            break;

                        case 12: // Smoke barrage.
                            MagicData.setAutoCastSpell(player, 28, 11, true);
                            break;

                        case 13: // Shadow barrage.
                            MagicData.setAutoCastSpell(player, 29, 15, true);
                            break;

                        case 14: // Blood barrage.
                            MagicData.setAutoCastSpell(player, 30, 7, true);
                            break;

                        case 15: // Ice barrage.
                            MagicData.setAutoCastSpell(player, 31, 3, true);
                            break;

                        case 16: // Cancel.
                            MagicData.cancelAutoCast(player, false);
                            break;
                    }
                    break;

                case 406: // Void knight mace autocast select spell.
                    switch (buttonId)
                    {
                        case 0: // Crumble undead.
                            MagicData.setAutoCastSpell(player, 32, 22, false);
                            break;

                        case 1: // Guthix claws.
                            MagicData.setAutoCastSpell(player, 34, 42, false);
                            break;

                        case 2: // Wind wave.
                            MagicData.setAutoCastSpell(player, 12, 45, false);
                            break;

                        case 3: // Water wave.
                            MagicData.setAutoCastSpell(player, 13, 48, false);
                            break;

                        case 4: // Earth wave.
                            MagicData.setAutoCastSpell(player, 14, 52, false);
                            break;

                        case 5: // Fire wave.
                            MagicData.setAutoCastSpell(player, 15, 55, false);
                            break;

                        case 6: // Cancel.
                            MagicData.cancelAutoCast(player, false);
                            break;
                    }
                    break;

                case 310: // Slayer staff autocast select spell.
                    switch (buttonId)
                    {
                        case 0: // Crumble undead.
                            MagicData.setAutoCastSpell(player, 32, 22, false);
                            break;

                        case 1: // Slayer dart.
                            MagicData.setAutoCastSpell(player, 33, 31, false);
                            break;

                        case 2: // Wind wave.
                            MagicData.setAutoCastSpell(player, 12, 45, false);
                            break;

                        case 3: // Water wave.
                            MagicData.setAutoCastSpell(player, 13, 48, false);
                            break;

                        case 4: // Earth wave.
                            MagicData.setAutoCastSpell(player, 14, 52, false);
                            break;

                        case 5: // Fire wave.
                            MagicData.setAutoCastSpell(player, 15, 55, false);
                            break;

                        case 6: // Cancel.
                            MagicData.cancelAutoCast(player, false);
                            break;
                    }
                    break;

                case 319: // Normal magic autocast select spell.
                    switch (buttonId)
                    {
                        case 0: // Wind strike.
                            MagicData.setAutoCastSpell(player, 0, 1, false);
                            break;

                        case 1: // Water strike.
                            MagicData.setAutoCastSpell(player, 1, 4, false);
                            break;

                        case 2: // Earth strike.
                            MagicData.setAutoCastSpell(player, 2, 6, false);
                            break;

                        case 3: // Fire strike.
                            MagicData.setAutoCastSpell(player, 3, 8, false);
                            break;

                        case 4: // Wind bolt.
                            MagicData.setAutoCastSpell(player, 4, 10, false);
                            break;

                        case 5: // Water bolt.
                            MagicData.setAutoCastSpell(player, 5, 14, false);
                            break;

                        case 6: // Earth bolt.
                            MagicData.setAutoCastSpell(player, 6, 17, false);
                            break;

                        case 7: // Fire bolt.
                            MagicData.setAutoCastSpell(player, 7, 20, false);
                            break;

                        case 8: // Wind blast.
                            MagicData.setAutoCastSpell(player, 8, 24, false);
                            break;

                        case 9: // Water blast.
                            MagicData.setAutoCastSpell(player, 9, 27, false);
                            break;

                        case 10: // Earth blast.
                            MagicData.setAutoCastSpell(player, 10, 33, false);
                            break;

                        case 11: // Fire blast.
                            MagicData.setAutoCastSpell(player, 11, 38, false);
                            break;

                        case 12: // Wind wave.
                            MagicData.setAutoCastSpell(player, 12, 45, false);
                            break;

                        case 13: // Water wave.
                            MagicData.setAutoCastSpell(player, 13, 48, false);
                            break;

                        case 14: // Earth wave.
                            MagicData.setAutoCastSpell(player, 14, 52, false);
                            break;

                        case 15: // Fire wave.
                            MagicData.setAutoCastSpell(player, 15, 55, false);
                            break;

                        case 16: // Cancel.
                            MagicData.cancelAutoCast(player, false);
                            break;
                    }
                    break;

                case 182: // Logout tab.
                    player.getPackets().logout();
                    break;

                case 261: // Settings tab.
                    switch (buttonId)
                    {
                        case 16: // Display settings.
                            player.getPackets().displayInterface(742);
                            break;

                        case 18: // Audio settings.
                            player.getPackets().displayInterface(743);
                            break;

                        case 3: // Run toggle.
                            if (!player.getWalkingQueue().isRunToggled())
                            {
                                player.getWalkingQueue().setRunToggled(true);
                                player.getPackets().sendConfig(173, 1);
                            }
                            else
                            {
                                player.getWalkingQueue().setRunToggled(false);
                                player.getPackets().sendConfig(173, 0);
                            }
                            break;

                        case 4: // Chat effect toggle.
                            if (!player.isChatEffectsEnabled())
                            {
                                player.setChatEffectsEnabled(true);
                                player.getPackets().sendConfig(171, 0);
                            }
                            else
                            {
                                player.setChatEffectsEnabled(false);
                                player.getPackets().sendConfig(171, 1);
                            }
                            break;

                        case 5: // Split private chat toggle.
                            if (!player.isPrivateChatSplit())
                            {
                                player.setPrivateChatSplit(true);
                                player.getPackets().sendConfig(287, 1);
                            }
                            else
                            {
                                player.setPrivateChatSplit(false);
                                player.getPackets().sendConfig(287, 0);
                            }
                            break;

                        case 7: // Accept aid toggle.
                            if (!player.isAcceptAidEnabled())
                            {
                                player.setAcceptAidEnabled(true);
                                player.getPackets().sendConfig(427, 1);
                            }
                            else
                            {
                                player.setAcceptAidEnabled(false);
                                player.getPackets().sendConfig(427, 0);
                            }
                            break;

                        case 6: // Mouse buttons toggle.
                            if (!player.isMouseTwoButtons())
                            {
                                player.setMouseTwoButtons(true);
                                player.getPackets().sendConfig(170, 0);
                            }
                            else
                            {
                                player.setMouseTwoButtons(false);
                                player.getPackets().sendConfig(170, 1);
                            }
                            break;
                    }
                    break;

                case 589: // Clan chat
                    if (buttonId == 9)
                    {
                        foreach (long friend in player.getFriends().getFriendsList())
                        {
                            player.getPackets().sendFriend(friend, player.getFriends().getWorld(friend));
                        }
                        Server.getClanManager().openClanSetup(player);
                        break;
                    }
                    break;

                case 590: // Clan chat setup
                    Clan clan = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
                    if (clan == null)
                    {
                        break;
                    }
                    switch (buttonId)
                    {
                        case 22: // Clan name
                            player.getPackets().displayEnterText("Enter clan name :");
                            player.setTemporaryAttribute("interfaceVariable", new EnterVariable(590, 0));
                            break;

                        case 23: // "Who can enter chat" - anyone.
                            clan.setEnterRights(Clan.ClanRank.NO_RANK);
                            player.getPackets().modifyText(clan.getRankString(clan.getEnterRights()), 590, 23);
                            break;

                        case 24: // "Who can talk in chat" - anyone.
                            clan.setTalkRights(Clan.ClanRank.NO_RANK);
                            player.getPackets().modifyText(clan.getRankString(clan.getTalkRights()), 590, 24);
                            break;

                        case 26: // "Who can share loot" - anyone.
                            clan.setLootRights(Clan.ClanRank.NO_RANK);
                            player.getPackets().modifyText(clan.getRankString(clan.getLootRights()), 590, 26);
                            break;
                    }
                    break;

                case 763: // Bank inventory
                    switch (buttonId)
                    {
                        case 0: // Deposit 1.
                            player.getBank().deposit(buttonId2, 1);
                            player.getBank().refreshBank();
                            break;
                    }
                    break;

                case 762: // Bank
                    switch (buttonId)
                    {
                        case 73: // withdraw 1.
                            player.getBank().withdraw(buttonId2, 1);
                            player.getBank().refreshBank();
                            break;

                        case 16: // Note item.
                            player.getBank().asNote();
                            break;

                        case 41: // first (main) bank tab
                            player.getBank().setCurrentTab(10);
                            break;

                        case 39: // first bank tab
                            player.getBank().setCurrentTab(2);
                            break;

                        case 37: // second bank tab
                            player.getBank().setCurrentTab(3);
                            break;

                        case 35: // third bank tab
                            player.getBank().setCurrentTab(4);
                            break;

                        case 33: // fourth bank tab
                            player.getBank().setCurrentTab(5);
                            break;

                        case 31: // fifth bank tab
                            player.getBank().setCurrentTab(6);
                            break;

                        case 29: // sixth bank tab
                            player.getBank().setCurrentTab(7);
                            break;

                        case 27: // seventh bank tab
                            player.getBank().setCurrentTab(8);
                            break;

                        case 25: // eighth bank tab
                            player.getBank().setCurrentTab(9);
                            break;
                    }
                    break;

                case 626: // Stake duel confirmation interface.
                    if (buttonId == 53)
                    {
                        if (player.getDuel() != null)
                        {
                            player.getDuel().acceptDuel();
                            break;
                        }
                    }
                    break;

                case 631: // Stake duel first interface.
                    if (player.getDuel() != null)
                    {
                        if (buttonId == 103)
                        {
                            player.getDuel().removeItem(buttonId2, 1);
                            break;
                        }
                        else
                        {
                            player.getDuel().toggleDuelRules(buttonId);
                            break;
                        }
                    }
                    break;

                case 387: // Equipment tab.
                    switch (buttonId)
                    {
                        case 55: // Character display.
                            player.getEquipment().displayEquipmentScreen();
                            break;

                        case 52: // Items kept on death.
                            ProtectedItems.displayItemsInterface(player);
                            break;
                    }
                    break;

                case 274: // Quest tab.
                    switch (buttonId)
                    {
                        case 3: // Achievment diary toggle.
                            player.getPackets().sendTab(85, 259);
                            player.setAchievementDiaryTab(true);
                            break;
                    }
                    break;

                case 259: // Achievment diary tab.
                    switch (buttonId)
                    {
                        case 8: // Quest tab toggle.
                            player.getPackets().sendTab(85, 274);
                            player.setAchievementDiaryTab(false);
                            break;
                    }
                    break;

                case 620: // Shop interface.
                    if (player.getShopSession() == null)
                    {
                        return;
                    }
                    switch (buttonId)
                    {
                        case 26: // Player stock tab.
                            player.getShopSession().openPlayerShop();
                            break;

                        case 25: // Main stock tab.
                            player.getShopSession().openMainShop();
                            break;

                        case 23: // Value (main stock)
                        case 24: // Value (player stock)
                            player.getShopSession().valueItem(buttonId2, false);
                            break;
                    }
                    break;

                case 621: // Shop inventory.
                    if (player.getShopSession() == null)
                    {
                        return;
                    }
                    switch (buttonId)
                    {
                        case 0: // Value (player stock)
                            player.getShopSession().valueItem(buttonId2, true);
                            break;
                    }
                    break;

                case 192: // Normal Magic tab.
                    switch (buttonId)
                    {
                        case 0: // Home Teleport.
                            Teleport.homeTeleport(player);
                            break;

                        case 15: // Varrock teleport.
                            Teleport.teleport(player, 0);
                            break;

                        case 18: // Lumbridge teleport.
                            Teleport.teleport(player, 1);
                            break;

                        case 21: // Falador teleport.
                            Teleport.teleport(player, 2);
                            break;

                        case 23: // POH teleport.
                            player.getPackets().sendMessage("This teleport is unavailable.");
                            break;

                        case 26: // Camelot teleport.
                            Teleport.teleport(player, 3);
                            break;

                        case 32: // Ardougne teleport.
                            Teleport.teleport(player, 4);
                            break;

                        case 37: // Watchtower teleport.
                            player.getPackets().sendMessage("This teleport is unavailable.");
                            //Teleport.teleport(player, 5);
                            break;

                        case 44: // Trollheim teleport.
                            Teleport.teleport(player, 6);
                            break;

                        case 47: // Ape Atoll teleport.
                            player.getPackets().sendMessage("This teleport is unavailable.");
                            break;

                        case 58: // Charge.
                            MagicCombat.castCharge(player);
                            break;
                    }
                    break;

                case 193: // Ancient magic tab.
                    switch (buttonId)
                    {
                        case 20: // Paddewwa teleport.
                            Teleport.teleport(player, 7);
                            break;

                        case 21: // Senntisten teleport.
                            Teleport.teleport(player, 8);
                            break;

                        case 22: // Kharyrll teleport.
                            Teleport.teleport(player, 9);
                            break;

                        case 23: // Lassar teleport.
                            Teleport.teleport(player, 10);
                            break;

                        case 24: // Dareeyak teleport.
                            Teleport.teleport(player, 11);
                            break;

                        case 25: // Carrallanger teleport.
                            Teleport.teleport(player, 12);
                            break;

                        case 27: // Ghorrock teleport.
                            Teleport.teleport(player, 14);
                            break;

                        case 26: // Annakarl teleport.
                            Teleport.teleport(player, 13);
                            break;

                        case 28: // Ancients Home teleport.
                            Teleport.homeTeleport(player);
                            break;
                    }
                    break;

                case 13: // Bank pin buttons.
                    if (buttonId == 29)
                    {
                        player.getBank().forgotPin();
                        break;
                    }
                    player.getBank().handleEnterPin(buttonId);
                    break;

                case 14: // Bank pin settings.
                    switch (buttonId)
                    {
                        case 60: // Set new bank pin.
                            player.getBank().displayFirstConfirmation();
                            break;

                        case 61: // Change recovery delay.
                            player.getBank().changePinDelay();
                            break;

                        case 91: // "No, I might forget it!".
                            if (player.getBank().isPinPending())
                            {
                                player.getBank().cancelPendingPin();
                                break;
                            }
                            player.getBank().openPinSettings(2);
                            break;

                        case 89: // Yes i want to set a pin.
                            if (player.getBank().isPinPending())
                            {
                                player.getBank().verifyPin(true);
                                break;
                            }
                            player.getBank().openEnterPin();
                            break;

                        case 65: // Cancel pin that's pending.
                            player.getBank().openPinSettings(4);
                            break;

                        case 62: // Change pin.
                            player.getBank().changePin();
                            break;

                        case 63: // Delete pin.
                            player.getBank().deletePin();
                            break;
                    }
                    break;

                case 464: // Emote tab.
                    Emotes.emote(player, buttonId);
                    break;

                case 320: // Skills Tab.
                    SkillMenu.display(player, buttonId);
                    break;

                case 499: // Skill menu side menu.
                    SkillMenu.subMenu(player, buttonId);
                    break;

                case 336: // Trade/duel inventory - trade 1.
                    if (player.getTrade() != null)
                    {
                        player.getTrade().tradeItem(buttonId2, 1);
                        break;
                    }
                    if (player.getDuel() != null)
                    {
                        player.getDuel().stakeItem(buttonId2, 1);
                        break;
                    }
                    break;

                case 335: // Trade interface.
                    if (player.getTrade() == null)
                    {
                        break;
                    }
                    switch (buttonId)
                    {
                        case 16: // Accept trade.
                            player.getTrade().accept();
                            break;

                        case 18: // Decline trade.
                            player.getTrade().decline();
                            break;

                        case 30: // Offer 1
                            player.getTrade().removeItem(buttonId2, 1);
                            break;
                    }
                    break;

                case 334: // Trade confirmation.
                    if (player.getTrade() == null)
                    {
                        break;
                    }
                    switch (buttonId)
                    {
                        case 21: // Decline trade.
                            player.getTrade().decline();
                            break;

                        case 20: // Accept trade.
                            player.getTrade().accept();
                            break;
                    }
                    break;

                case 750: // Run button
                    if (!player.getWalkingQueue().isRunToggled())
                    {
                        player.getWalkingQueue().setRunToggled(true);
                        player.getPackets().sendConfig(173, 1);
                    }
                    else
                    {
                        player.getWalkingQueue().setRunToggled(false);
                        player.getPackets().sendConfig(173, 0);
                    }
                    break;

                case 667: // Equipment/bonuses interface.
                    if (buttonId == 14)
                    {
                        player.getEquipment().unequipItem((ItemData.EQUIP)buttonId2);
                        break;
                    }
                    break;

                case 771: // Character design interface.
                    ConfigureAppearance.sortButton(player, buttonId);
                    break;

                case 311: // Smelt interface.
                    Smelting.smeltOre(player, buttonId, true, -1);
                    break;

                case 300: // Bar smithing interface.
                    Smithing.smithItem(player, buttonId, 1, true);
                    break;

                case 92: // Unarmed attack interface.
                    switch (buttonId)
                    {
                        case 24: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 85: // Spear attack interface.
                    switch (buttonId)
                    {
                        case 8: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 24: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 93: // Whip attack interface.
                    switch (buttonId)
                    {
                        case 8: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 24: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 89: // Dagger attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 82: // Longsword/scimitar attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 78: // Claw attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 81: // Godsword attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 88: // Mace attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 76: // Granite maul attack interface.
                    switch (buttonId)
                    {
                        case 8: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 24: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 77: // Bow attack interface.
                    switch (buttonId)
                    {
                        case 11: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            break;

                        case 27: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 75: // Battleaxe attack interface.
                    switch (buttonId)
                    {
                        case 10: // Special attack.
                            player.getSpecialAttack().toggleSpecBar();
                            player.getSpecialAttack().dragonBattleaxe();
                            break;

                        case 26: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 91: // Thrown weapon
                    switch (buttonId)
                    {
                        case 24: // Auto retaliate.
                            player.toggleAutoRetaliate();
                            break;

                        default:
                            AttackInterface.configureButton(player, interfaceId, buttonId);
                            break;
                    }
                    break;

                case 430: // Lunar interface
                    switch (buttonId)
                    {
                        case 14: // Vengeance
                            Lunar.castLunarSpell(player, buttonId);
                            break;
                    }
                    break;

                case 102: // Items on death interface
                    if (buttonId == 18)
                        player.getPackets().sendMessage("You will keep this item if you should you die.");
                    else
                        player.getPackets().sendMessage("You will lose this item if you should you die.");
                    break;

                default:
                    if (interfaceId != 548 && interfaceId != 751)
                    {
                        Console.WriteLine("Unhandled ActionButton : " + interfaceId + " " + buttonId + " " + buttonId2);
                    }
                    break;
            }
        }
    }
}