using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.barrows;
using RS2.Server.minigames.warriorguild;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.cooking;
using RS2.Server.player.skills.crafting;
using RS2.Server.player.skills.farming;
using RS2.Server.player.skills.firemaking;
using RS2.Server.player.skills.fletching;
using RS2.Server.player.skills.herblore;
using RS2.Server.player.skills.magic;
using RS2.Server.player.skills.prayer;
using RS2.Server.player.skills.runecrafting;
using RS2.Server.player.skills.slayer;
using RS2.Server.player.skills.smithing;
using RS2.Server.player.skills.woodcutting;
using RS2.Server.util;
using System;

namespace RS2.Server.packethandler
{
    internal class ItemInteract : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.EQUIP:
                    handleEquipItem(player, packet);
                    break;

                case PacketHandlers.PacketId.ITEM_ON_ITEM:
                    handleItemOnItem(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_CLICK:
                    handleInvenClickItem(player, packet);
                    break;

                case PacketHandlers.PacketId.ITEM_ON_OBJECT:
                    handleItemOnObject(player, packet);
                    break;

                case PacketHandlers.PacketId.ITEM_ON_GROUND_ITEM:
                    handleItemOnGroundItem(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_OPERATE:
                    handleOperateItem(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_DROP:
                    handleDropItem(player, packet);
                    break;

                case PacketHandlers.PacketId.PICKUP:
                    handlePickupItem(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_SWAP_SLOT:
                    handleSwapSlot(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_SWAP_SLOT2:
                    handleSwapSlot2(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_RIGHT_CLICK_OPTION1:
                    handleRightClickOne(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_RIGHT_CLICK_OPTION2:
                    handleRightClickTwo(player, packet);
                    break;

                case PacketHandlers.PacketId.INV_EXAMINE_ITEM:
                    handleExamineItem(player, packet);
                    break;
            }
        }

        private void handleEquipItem(Player player, Packet packet)
        {
            int item = packet.readLEShort();
            int slot = packet.readShortA();
            int interfaceId = packet.readInt();
            if (slot > 28 || slot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                //player.getPackets().closeInterfaces();
                if (RuneCraft.emptyPouch(player, (RuneCraftData.POUCHES)player.getInventory().getItemInSlot(slot)))
                {
                    return;
                }
                player.getEquipment().equipItem(player.getInventory().getItemInSlot(slot), slot);
            }
        }

        private void handleItemOnItem(Player player, Packet packet)
        {
            int itemSlot = packet.readUShort();
            int unused = packet.readInt();
            int withSlot = packet.readLEShort();
            int unused2 = packet.readInt();
            int itemUsed = packet.readLEShortA();
            int usedWith = packet.readLEShortA();
            if (itemSlot > 28 || itemSlot < 0 || withSlot > 28 || withSlot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            if (player.getInventory().getSlot(itemSlot).getItemId() == itemUsed && player.getInventory().getSlot(withSlot).getItemId() == usedWith)
            {
                if (Fletching.isFletching(player, itemUsed, usedWith))
                {
                    return;
                }
                else if (Herblore.doingHerblore(player, itemUsed, usedWith))
                {
                    return;
                }
                else if (Herblore.mixDoses(player, itemUsed, usedWith, itemSlot, withSlot))
                {
                    return;
                }
                else if (Crafting.wantsToCraft(player, itemUsed, usedWith))
                {
                    return;
                }
                else if (Firemaking.isFiremaking(player, itemUsed, usedWith, itemSlot, withSlot))
                {
                    return;
                }
                else if (Farming.plantSapling(player, itemUsed, usedWith))
                {
                    return;
                }
                else
                {
                    player.getPackets().sendMessage("Nothing interesting happens.");
                }
            }
        }

        private void handleInvenClickItem(Player player, Packet packet)
        {
            int slot = packet.readLEShortA();
            int item = packet.readShortA();
            int childId = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            if (slot > 28 || slot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                player.getPackets().closeInterfaces();
                if (Consumables.isEating(player, player.getInventory().getItemInSlot(slot), slot))
                    return;
                else if (Herblore.idHerb(player, player.getInventory().getItemInSlot(slot)))
                    return;
                else if (RuneCraft.fillPouch(player, (RuneCraftData.POUCHES)player.getInventory().getItemInSlot(slot)))
                    return;
                else if (Prayer.wantToBury(player, player.getInventory().getItemInSlot(slot), slot))
                    return;
                else if (Teleport.useTeletab(player, player.getInventory().getItemInSlot(slot), slot))
                    return;
                else if (FarmingAmulet.showOptions(player, player.getInventory().getItemInSlot(slot)))
                    return;

                switch (item)
                {
                    case 4155: // Slayer gem
                        Slayer.doDialogue(player, 1051);
                        break;

                    case 6: // Dwarf multicannon
                        if (player.getCannon() != null)
                        {
                            player.getPackets().sendMessage("You already have a cannon set up!");
                            break;
                        }
                        player.setCannon(new DwarfCannon(player));
                        break;

                    case 5073: // Nest with seeds.
                    case 5074: // Nest with jewellery.
                        Woodcutting.randomNestItem(player, item);
                        break;

                    case 952: // Spade
                        player.setLastAnimation(new Animation(830));
                        if (Barrows.enterCrypt(player))
                        {
                            player.getPackets().sendMessage("You've broken into a crypt!");
                            break;
                        }
                        player.getPackets().sendMessage("You find nothing.");
                        break;
                }
            }
        }

        private void handleItemOnObject(Player player, Packet packet)
        {
            int objectX = packet.readShortA();
            int item = packet.readUShort();
            int objectY = packet.readLEShort();
            int slot = packet.readUShort();
            int interfaceId = packet.readLEShort();
            int child = packet.readUShort();
            int objectId = packet.readShortA();
            if (slot > 28 || slot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            Console.WriteLine("Item on object = " + objectId + " " + objectX + " " + objectY);
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            player.setFaceLocation(new Location(objectX, objectY, player.getLocation().getZ()));
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                if (Crafting.wantsToCraftOnObject(player, player.getInventory().getItemInSlot(slot), objectId))
                {
                    return;
                }
                else if (Farming.interactWithPatch(player, objectId, objectX, objectY, player.getInventory().getItemInSlot(slot)))
                {
                    return;
                }
                else if (WarriorGuild.useAnimator(player, player.getInventory().getItemInSlot(slot), objectId, objectX, objectY))
                {
                    return;
                }
                if (player.getInventory().getItemInSlot(slot) == 7936)
                {
                    if (RuneCraft.wantToRunecraft(player, objectId, objectX, objectY))
                    {
                        return;
                    }
                    if (RuneCraft.useTalisman(player, objectId, objectX, objectY))
                    {
                        return;
                    }
                }
                switch (objectId)
                {
                    case 6: // Cannon:
                        DwarfCannon cannon = player.getCannon();
                        Location l = new Location(objectX, objectY, player.getLocation().getZ());
                        if (cannon == null || (cannon != null & !l.withinDistance(cannon.getLocation(), 2)))
                        {
                            player.getPackets().sendMessage("This isn't your cannon!");
                            break;
                        }
                        cannon.loadCannon();
                        break;

                    case 36781: // Lumbridge fountain.
                    case 24214:	// Fountain in east Varrock.
                    case 24265:	// Varrock main fountain.
                    case 11661:	// Falador waterpump.
                    case 11759:	// Falador south fountain.
                    case 879:	// Camelot fountains.
                    case 29529:	// Sink.
                    case 874:	// Sink.
                        if (FillVial.fillingVial(player, new Location(objectX, objectY, player.getLocation().getZ())) && player.getInventory().getItemInSlot(slot) == 229)
                        {
                            break;
                        }
                        break;

                    case 2728: // Range in Catherby
                        if (Cooking.isCooking(player, player.getInventory().getItemInSlot(slot), false, -1, -1))
                        {
                            break;
                        }
                        break;

                    case 2732: // Fire
                        if (Cooking.isCooking(player, player.getInventory().getItemInSlot(slot), true, objectX, objectY))
                        {
                            break;
                        }
                        break;

                    case 36956: // Lumbridge furnace
                    case 11666: // Falador furnace
                        if (Smelting.wantToSmelt(player, player.getInventory().getItemInSlot(slot)))
                        {
                            break;
                        }
                        else if (Crafting.wantsToCraftOnObject(player, player.getInventory().getItemInSlot(slot), objectId))
                        {
                            break;
                        }
                        break;

                    case 2783: // Anvil
                        if (Smithing.wantToSmithOnAnvil(player, player.getInventory().getItemInSlot(slot), new Location(objectX, objectY, player.getLocation().getZ())))
                        {
                            break;
                        }
                        break;

                    default:
                        player.getPackets().sendMessage("Nothing interesting happens.");
                        break;
                }
            }
        }

        private void handleItemOnGroundItem(Player player, Packet packet)
        {
            int objectX = packet.readLEShortA();
            int itemSlot = packet.readLEShort();
            int itemIdInInventory = packet.readLEShort();
            int itemIdOnGround = packet.readLEShort();
            int objectY = packet.readLEShortA();
            int interfaceId = packet.readLEShort();
            int child = packet.readUShort();

            if (itemSlot > 28 || itemSlot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
                return;

            if (Firemaking.isFiremaking(player, itemIdInInventory, itemIdOnGround, itemSlot, -1))
                return;
            else
                player.getPackets().sendMessage("Nothing interesting happens.");
        }

        private void handleOperateItem(Player player, Packet packet)
        {
            int item = packet.readShortA();
            int slot = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            int childId = packet.readLEShort();
            if (slot < 0 || slot > 13 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            ItemData.EQUIP equipSlot = (ItemData.EQUIP)slot;
            if (player.getEquipment().getItemInSlot(equipSlot) == item)
            {
                SkillHandler.resetAllSkills(player);
                player.getPackets().closeInterfaces();
                if (JewelleryTeleport.useJewellery(player, player.getEquipment().getItemInSlot(equipSlot), slot, true))
                {
                    return;
                }
                else
                    if (equipSlot == ItemData.EQUIP.CAPE && Skillcape.emote(player))
                    {
                        return;
                    }
                player.getPackets().sendMessage("This item isn't operable.");
            }
        }

        private void handleDropItem(Player player, Packet packet)
        {
            int item = packet.readShortA();
            int slot = packet.readShortA();
            int interfaceId = packet.readLEShort();
            int childId = packet.readUShort();
            if (slot > 28 || slot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                player.getPackets().closeInterfaces();
                if (ItemData.isPlayerBound(player.getInventory().getItemInSlot(slot)))
                {
                    Item[] items = { new Item(player.getInventory().getItemInSlot(slot), 1) };
                    //player.getPackets().sendItems(94, 0, 93, items);
                    player.getPackets().modifyText("Are you sure you want to destroy this item?", 94, 3); // Title
                    //player.getPackets().modifyText("Yes", 94, 4); // Yes
                    //player.getPackets().modifyText("No", 94, 5); // No
                    player.getPackets().modifyText("", 94, 10); // Line 1
                    player.getPackets().modifyText("If you wish to get another Fire cape, you must", 94, 11); // Line 2
                    player.getPackets().modifyText("complete the Fight cave minigame again.", 94, 12); // Line 3
                    player.getPackets().modifyText("Fire Cape", 94, 13); // Item name
                    player.getPackets().sendChatboxInterface(94);
                    return;
                }
                int id = player.getInventory().getItemInSlot(slot);
                int amt = player.getInventory().getAmountInSlot(slot);
                GroundItem i = new GroundItem(id, amt, new Location(player.getLocation().getX(), player.getLocation().getY(), player.getLocation().getZ()), player);
                if (player.getInventory().deleteItem(id, slot, amt))
                {
                    if (!Server.getGroundItems().addToStack(id, amt, player.getLocation(), player))
                    {
                        Server.getGroundItems().newEntityDrop(i);
                    }
                }
            }
        }

        private void handlePickupItem(Player player, Packet packet)
        {
            int x = packet.readLEShort();
            int id = packet.readUShort();
            int y = packet.readLEShortA();
            Location l = new Location(x, y, player.getLocation().getZ());
            SkillHandler.resetAllSkills(player);
            if (x < 1000 || y < 1000 | id < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            player.getPackets().closeInterfaces();
            if (player.getLocation().Equals(l))
            {
                Server.getGroundItems().pickupItem(player, id, player.getLocation());
                return;
            }
            CoordinateEvent pickupItemCoordinateEvent = new CoordinateEvent(player, l);
            pickupItemCoordinateEvent.setAction(() =>
            {
                Server.getGroundItems().pickupItem(player, id, player.getLocation());
            });
            Server.registerCoordinateEvent(pickupItemCoordinateEvent);
        }

        private void handleSwapSlot(Player player, Packet packet)
        {
            int oldSlot = packet.readUShort();
            int childId = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            int newSlot = packet.readShortA();
            int swapType = packet.readByteS();
            int oldItem = player.getInventory().getItemInSlot(oldSlot);
            int oldAmount = player.getInventory().getAmountInSlot(oldSlot);
            int newItem = player.getInventory().getItemInSlot(newSlot);
            int newAmount = player.getInventory().getAmountInSlot(newSlot);
            if (oldSlot > 28 || oldSlot < 0 || newSlot > 28 || oldSlot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            switch (interfaceId)
            {
                case 149:
                    if (swapType == 0 && childId == 0)
                    {
                        player.getInventory().getSlot(oldSlot).setItemId(newItem);
                        player.getInventory().getSlot(oldSlot).setItemAmount(newAmount);
                        player.getInventory().getSlot(newSlot).setItemId(oldItem);
                        player.getInventory().getSlot(newSlot).setItemAmount(oldAmount);
                    }
                    break;

                default:
                    Misc.WriteError("UNHANDLED ITEM SWAP 1 : interface = " + interfaceId);
                    break;
            }
            //No need to update the screen since the client does it for us!
        }

        private void handleSwapSlot2(Player player, Packet packet)
        {
            int interfaceId = packet.readLEShort();
            int child = packet.readUShort();
            int newSlot = packet.readLEShort();
            int interface2 = packet.readUShort();
            int child2 = packet.readUShort();
            int oldSlot = packet.readLEShort();
            int oldItem = player.getInventory().getItemInSlot(oldSlot);
            int oldAmount = player.getInventory().getAmountInSlot(oldSlot);
            int newItem = player.getInventory().getItemInSlot(newSlot);
            int newAmount = player.getInventory().getAmountInSlot(newSlot);
            if (oldSlot > 28 || oldSlot < 0 || newSlot > 28 || oldSlot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            switch (interfaceId)
            {
                case 621: // Shop.
                case 763: // Bank.
                case 336: // Duel
                    player.getInventory().getSlot(oldSlot).setItemId(newItem);
                    player.getInventory().getSlot(oldSlot).setItemAmount(newAmount);
                    player.getInventory().getSlot(newSlot).setItemId(oldItem);
                    player.getInventory().getSlot(newSlot).setItemAmount(oldAmount);
                    break;

                default:
                    Misc.WriteError("UNHANDLED ITEM SWAP 2 : interface = " + interfaceId);
                    break;
            }
            //No need to update the screen since the client does it for us!
            player.getPackets().refreshInventory();
        }

        private void handleRightClickOne(Player player, Packet packet)
        {
            int childId = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            int item = packet.readLEShortA();
            int slot = packet.readLEShortA();
            if (slot > 28 || slot < 0 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                player.getPackets().closeInterfaces();
                if (interfaceId == 149 && childId == 0)
                {
                    if (Herblore.emptyPotion(player, player.getInventory().getItemInSlot(slot), slot))
                    {
                        return;
                    }
                    else if (JewelleryTeleport.useJewellery(player, player.getInventory().getItemInSlot(slot), slot, false))
                    {
                        return;
                    }
                }
            }
        }

        private void handleRightClickTwo(Player player, Packet packet)
        {
            int childId = packet.readLEShort();
            int interfaceId = packet.readLEShort();
            int slot = packet.readLEShort();
            int item = packet.readLEShort();
            if (slot < 0 || slot > 28 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            if (player.getInventory().getItemInSlot(slot) == item)
            {
                player.getPackets().closeInterfaces();
                switch (player.getInventory().getItemInSlot(slot))
                {
                    case 5509: // Small pouch.
                        player.getPackets().sendMessage("There is " + player.getSmallPouchAmount() + " Pure essence in your small pouch. (holds 3).");
                        break;

                    case 5510: // Medium pouch.
                        player.getPackets().sendMessage("There is " + player.getMediumPouchAmount() + " Pure essence in your medium pouch. (holds 6).");
                        break;

                    case 5512: // Large pouch.
                        player.getPackets().sendMessage("There is " + player.getLargePouchAmount() + " Pure essence in your large pouch. (holds 9).");
                        break;

                    case 5514: // Giant pouch.
                        player.getPackets().sendMessage("There is " + player.getGiantPouchAmount() + " Pure essence in your giant pouch. (holds 12).");
                        break;
                }
            }
        }

        private void handleExamineItem(Player player, Packet packet)
        {
            int item = packet.readLEShortA();
            if (item < 0 || item > Constants.MAX_ITEMS)
            {
                return;
            }
            string examine = ItemData.forId(item).getExamine();
            player.getPackets().sendMessage(examine);
        }
    }
}