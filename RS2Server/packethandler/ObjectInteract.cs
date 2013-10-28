using RS2.Server.definitions;
using RS2.Server.definitions.areas;
using RS2.Server.minigames.agilityarena;
using RS2.Server.minigames.barrows;
using RS2.Server.minigames.fightcave;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.player.skills;
using RS2.Server.player.skills.agility;
using RS2.Server.player.skills.crafting;
using RS2.Server.player.skills.farming;
using RS2.Server.player.skills.mining;
using RS2.Server.player.skills.runecrafting;
using RS2.Server.player.skills.smithing;
using RS2.Server.player.skills.thieving;
using RS2.Server.player.skills.woodcutting;
using System;

namespace RS2.Server.packethandler
{
    internal class ObjectInteract : PacketHandler
    {
        public void handlePacket(Player player, Packet packet)
        {
            switch (packet.getPacketId())
            {
                case PacketHandlers.PacketId.OBJECT_FIRST_CLICK:
                    handleFirstClickObject(player, packet);
                    break;

                case PacketHandlers.PacketId.OBJECT_SECOND_CLICK:
                    handleSecondClickObject(player, packet);
                    break;

                case PacketHandlers.PacketId.OBJECT_THIRD_CLICK:
                    handleThirdClickObject(player, packet);
                    break;

                case PacketHandlers.PacketId.OBJECT_FOURTH_CLICK:
                    handleFourthClickObject(player, packet);
                    break;

                case PacketHandlers.PacketId.OBJECT_EXAMINE:
                    handleExamineObject(player, packet);
                    break;
            }
        }

        private void handleFirstClickObject(Player player, Packet packet)
        {
            int objectX = packet.readLEShort();
            ushort objectId = (ushort)packet.readShortA();
            int objectY = packet.readUShort();
            if (objectX < 1000 || objectY < 1000 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            Console.WriteLine("First object click = " + objectId + " " + objectX + " " + objectY);
            if (RuneCraft.wantToRunecraft(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (RuneCraft.enterRift(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (RuneCraft.enterViaTiara(player, objectId, objectX, objectY))
            {
                player.setFaceLocation(new Location(objectX, objectY, player.getLocation().getZ()));
                return;
            }
            else if (RuneCraft.leaveAltar(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Barrows.leaveCrypt(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Barrows.tryOpenCoffin(player, objectId))
            {
                return;
            }
            else if (Barrows.openTunnelDoor(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Thieving.wantToThieveChest(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Agility.doAgility(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Farming.interactWithPatch(player, objectId, objectX, objectY, -1))
            {
                return;
            }
            else if (Server.getGlobalObjects().getDoors().useDoor(player, objectId, objectX, objectY, player.getLocation().getZ()))
            {
                return;
            }
            else if (LaddersAndStairs.useObject(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 1))
            {
                return;
            }
            else if (WildernessObelisks.useWildernessObelisk(player, objectId, new Location(objectX, objectY, player.getLocation().getZ())))
            {
                return;
            }
            if (player.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            player.setFaceLocation(new Location(objectX, objectY, player.getLocation().getZ()));
            switch (objectId)
            {
                case 2492: // essence mine portals
                    RuneCraft.leaveEssMine(player, new Location(objectX, objectY, player.getLocation().getZ()));
                    break;

                case 5959:
                case 5960:
                    Wilderness.handleLever(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()));
                    break;

                case 733: // Wilderness web
                    Wilderness.slashWeb(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()));
                    break;

                case 28089: // GE desk
                    Server.getGrandExchange().clickDesk(player, objectX, objectY, 1);
                    break;

                case 9359: // Tzhaar main exit
                    TzHaar.exitTzhaar(player);
                    break;

                case 31284: // Tzhaar entrance
                    TzHaar.enterTzhaar(player);
                    break;

                case 9357: // Fight cave exit
                    FightCave.exitCave(player, objectX, objectY);
                    break;

                case 9356: // Fight cave entrance
                    FightCave.enterCave(player);
                    break;

                case 9391: // Tzhaar fight pits viewing orb
                    Server.getMinigames().getFightPits().useOrb(player, -1);
                    break;

                case 9369: // Tzhaar pits main entrance
                case 9368: // Tzhaar pits game door
                    Server.getMinigames().getFightPits().useDoor(player, objectId);
                    break;

                case 3617: // Agility arena ladder
                    AgilityArena.enterArena(player, objectX, objectY);
                    break;

                case 3618:
                    if (Location.atAgilityArena(player.getLocation()))
                    {
                        AgilityArena.exitArena(player, objectX, objectY);
                    }
                    break;

                case 6: // Dwarf multicannon
                    DwarfCannon cannon = player.getCannon();
                    Location l = new Location(objectX, objectY, player.getLocation().getZ());
                    if (cannon == null || (cannon != null & !l.withinDistance(cannon.getLocation(), 2)))
                    {
                        player.getPackets().sendMessage("This isn't your cannon!");
                        break;
                    }
                    cannon.fireCannon();
                    break;

                case 7: //Cannon base only
                case 8: //Cannon stand
                case 9: //Cannon barrels
                    DwarfCannon cannonPickup = player.getCannon();
                    Location cannonLocation = new Location(objectX, objectY, player.getLocation().getZ());
                    if (cannonPickup == null || (cannonPickup != null & !cannonLocation.withinDistance(cannonPickup.getLocation(), 2)))
                    {
                        player.getPackets().sendMessage("This isn't your cannon!");
                        break;
                    }
                    cannonPickup.pickupCannon();
                    break;

                case 11601: // Clay oven
                    player.getPackets().modifyText("Please use the item on the oven.", 210, 1);
                    player.getPackets().sendChatboxInterface(210);
                    break;

                case 10284: // Barrows chest
                    Barrows.openChest(player);
                    break;

                case 4483: // Castle wars bank chest.
                case 21301: // Neitiznot bank chest
                    player.getBank().openBank(false, objectX, objectY);
                    break;

                case 1276: // Normal tree
                case 1278: // Normal tree
                case 2409: // Normal tree
                case 1277: // Normal tree with but different coloured stump
                case 3034: // Normal tree with dark stump
                case 3033: // Normal tree with dark stump
                case 10041: // Normal tree
                case 1282: // Dead tree
                case 1283: // Dead tree
                case 1284: // Dead tree
                case 1285: // Dead tree
                case 1286: // Dead tree
                case 1289: // Dead tree
                case 1290: // Dead tree
                case 1365: // Dead tree
                case 1383: // Dead tree
                case 1384: // Dead tree
                case 1291: // Dead tree
                case 3035: // Dead tree
                case 3036: // Dead tree
                case 1315: // Evergreen
                case 1316: // Evergreen
                case 1318: // Snowy Evergreen
                case 1319: // Snowy Evergreen
                case 1330: // Snow covered tree
                case 1331: // Snow covered tree
                case 1332: // Snow covered tree
                case 3879: // Evergreen from elf land
                case 3881: // Evergreen from elf land (slightly bigger than one above)
                case 3882: // Evergreen from elf land (slightly bigger than one above)
                case 3883: // Small Evergreen from elf land
                case 1280: // Normal tree orange stump
                case 14309: // PC game island tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 0, true);
                    break;

                case 1281: // Normal Oak tree
                case 3037: // Oak tree dark stump
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 1, true);
                    break;

                case 1308: // Normal Willow tree
                case 5551: // Normal Willow tree
                case 5552: // Normal Willow tree
                case 5553: // Normal Willow tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 2, true);
                    break;

                case 2023: // Achey tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 3, true);
                    break;

                case 9036: // Normal Teak tree
                case 15062: // Normal Teak tree (same as above?)
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 4, true);
                    break;

                case 1307: // Normal Maple tree
                case 4674:// Exactly same as above
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 5, true);
                    break;

                case 2289: // Normal Hollow tree
                case 4060: // Normal Hollow tree (bigger than above)
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 6, true);
                    break;

                case 9034: // Normal Mahogany tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 7, true);
                    break;

                case 21273: // Normal Arctic pine
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 8, true);
                    break;

                case 28951: // Normal Eucalyptus tree
                case 28952: // Normal Eucalyptus tree (smaller)
                case 28953: // Normal Eucalyptus tree (smallest)
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 9, true);
                    break;

                case 1309: // Yew tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 10, true);
                    break;

                case 1306: // Normal Magic tree
                    Woodcutting.tryCutTree(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 11, true);
                    break;

                case 3203: // Duel arena trapdoors.
                    if (player.getDuel() != null)
                    {
                        player.getDuel().forfeitDuel(objectX, objectY);
                        break;
                    }
                    break;

                case 7152: // Abyss tendrils.
                case 7144:
                    AbyssObstacles.chopTendrils(player, objectX, objectY);
                    break;

                case 7147: // Abyss tunnel.
                    AbyssObstacles.useAgilityTunnel(player, objectX, objectY);
                    break;

                case 7146: // Abyss eyes.
                case 7150:
                    AbyssObstacles.passEyes(player, objectX, objectY);
                    break;

                case 7151: // Abyss boil.
                case 7145:
                    AbyssObstacles.burnBoil(player, objectX, objectY);
                    break;

                case 7153: // Abyss mining rock.
                case 7143:
                    AbyssObstacles.mineRock(player, objectX, objectY);
                    break;

                case 2213: // Catherby bank booth.
                case 11402: // Varrock bank booth.
                case 11758: // Falador bank booth.
                case 36786: // Lumbridge bank booth.
                case 35647: // Al-Kharid bank booth.
                case 25808: // Seers bank booth.
                case 34752: // Ardougne bank booth.
                case 26972: // Edgeville bank booth.
                case 29085: // Ooglog bank booth.
                    player.getBank().openBank(true, objectX, objectY);
                    break;

                case 2491: // Essence rock
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 0, true);
                    break;

                case 11954: // Iron rocks
                case 11955:
                case 11956:
                case 14856:
                case 14857:
                case 14858:
                case 31071:
                case 31072:
                case 31073:
                case 32441:
                case 32442:
                case 32443:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 6, true);
                    break;

                case 11948: // Silver rocks
                case 11949:
                case 11950:
                case 11165:
                case 11186:
                case 11187:
                case 11188:
                case 31074:
                case 31075:
                case 31076:
                case 32444:
                case 32445:
                case 32446:
                case 15579:
                case 15580:
                case 15581:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 8, true);
                    break;

                case 15504: // Clay rocks
                case 15503:
                case 15505:
                case 11189:
                case 11190:
                case 11191:
                case 31062:
                case 31063:
                case 31064:
                case 32429:
                case 32430:
                case 32431:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 1, true);
                    break;

                case 11960: // Copper rocks
                case 11961:
                case 11962:
                case 11936:
                case 11937:
                case 11938:
                case 31080:
                case 31081:
                case 31082:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 2, true);
                    break;

                case 11959: // Tin rocks
                case 11958:
                case 11957:
                case 11933:
                case 11934:
                case 11935:
                case 31077:
                case 31078:
                case 31079:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 3, true);
                    break;

                case 11930: // Coal rocks
                case 11931:
                case 11932:
                case 14850:
                case 14851:
                case 14852:
                case 31068:
                case 31069:
                case 31070:
                case 32426:
                case 32427:
                case 32428:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 9, true);
                    break;

                case 11951: // Gold rocks
                case 11952:
                case 11953:
                case 11183:
                case 11184:
                case 11185:
                case 31065:
                case 31066:
                case 31067:
                case 32432:
                case 32433:
                case 32434:
                case 15576:
                case 15577:
                case 15578:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 10, true);
                    break;

                case 11945: // Mithril rocks
                case 11946:
                case 11947:
                case 11942:
                case 11943:
                case 11944:
                case 14853:
                case 14854:
                case 14855:
                case 31086:
                case 31087:
                case 31088:
                case 32438:
                case 32439:
                case 32440:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 11, true);
                    break;

                case 11963: // Adamant rocks
                case 11964:
                case 11965:
                case 11939:
                case 11940:
                case 11941:
                case 14862:
                case 14863:
                case 14864:
                case 31083:
                case 31084:
                case 31085:
                case 32435:
                case 32436:
                case 32437:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 12, true);
                    break;

                case 14859: // Rune rocks
                case 14860:
                case 14861:
                    Mining.tryMineRock(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 13, true);
                    break;

                case 11552: // Empty rocks
                case 11553:
                case 11554:
                case 11555:
                case 11556:
                case 31059:
                case 31060:
                case 31061:
                case 14832:
                case 14833:
                case 14834:
                case 33400:
                case 33401:
                case 33402:
                case 15582:
                case 15583:
                case 15584:
                    Mining.displayEmptyRockMessage(player, new Location(objectX, objectY, player.getLocation().getZ()));
                    break;

                case 23271: // Wilderness ditch
                    Wilderness.crossDitch(player, objectX, objectY);
                    break;
            }
        }

        private void handleSecondClickObject(Player player, Packet packet)
        {
            int objectY = packet.readLEShortA();
            int objectX = packet.readLEShort();
            ushort objectId = packet.readUShort();
            Console.WriteLine("Second object click = " + objectId + " " + objectX + " " + objectY);
            if (player.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            if (objectX < 1000 || objectY < 1000 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            player.setFaceLocation(new Location(objectX, objectY, player.getLocation().getZ()));
            if (Thieving.wantToThieveStall(player, objectId, objectX, objectY))
            {
                return;
            }
            else if (Farming.interactWithPatch(player, objectId, objectX, objectY, -1))
            {
                return;
            }
            else if (LaddersAndStairs.useObject(player, objectId, new Location(objectX, objectY, player.getLocation().getZ()), 2))
            {
                return;
            }
            switch (objectId)
            {
                case 28089: // GE desk
                    Server.getGrandExchange().clickDesk(player, objectX, objectY, 2);
                    break;

                case 25824: // Spinning wheel (Seers)
                case 36970: // Spinning wheel (Lumbridge
                    Spinning.displaySpinningInterface(player);
                    break;

                case 6: // Dwarf multicannon
                    DwarfCannon cannon = player.getCannon();
                    Location l = new Location(objectX, objectY, player.getLocation().getZ());
                    if (cannon == null || (cannon != null & !l.Equals(cannon.getLocation())))
                    {
                        player.getPackets().sendMessage("This isn't your cannon!");
                        break;
                    }
                    cannon.pickupCannon();
                    break;

                case 11666: // Falador furnace
                case 36956: // Lumbridge furnace
                    Smelting.displaySmeltOptions(player);
                    break;

                case 11959: // Tin rocks
                case 11958:
                case 11957:
                case 11933:
                case 11934:
                case 11935:
                case 31077:
                case 31078:
                case 31079:
                    Mining.prospectRock(player, objectX, objectY, "tin");
                    break;

                case 11960: // Copper rocks
                case 11961:
                case 11962:
                case 11936:
                case 11937:
                case 11938:
                case 31080:
                case 31081:
                case 31082:
                    Mining.prospectRock(player, objectX, objectY, "copper");
                    break;

                case 15504: // Clay rocks
                case 15503:
                case 15505:
                case 11189:
                case 11190:
                case 11191:
                case 31062:
                case 31063:
                case 31064:
                case 32429:
                case 32430:
                case 32431:
                    Mining.prospectRock(player, objectX, objectY, "clay");
                    break;

                case 11948: // Silver rocks
                case 11949:
                case 11950:
                case 11165:
                case 11186:
                case 11187:
                case 11188:
                case 31074:
                case 31075:
                case 31076:
                case 32444:
                case 32445:
                case 32446:
                case 15579:
                case 15580:
                case 15581:
                    Mining.prospectRock(player, objectX, objectY, "silver");
                    break;

                case 11930: // Coal rocks
                case 11931:
                case 11932:
                case 14850:
                case 14851:
                case 14852:
                case 31068:
                case 31069:
                case 31070:
                case 32426:
                case 32427:
                case 32428:
                    Mining.prospectRock(player, objectX, objectY, "coal");
                    break;

                case 11945: // Mithril rocks
                case 11946:
                case 11947:
                case 11942:
                case 11943:
                case 11944:
                case 14853:
                case 14854:
                case 14855:
                case 31086:
                case 31087:
                case 31088:
                case 32438:
                case 32439:
                case 32440:
                    Mining.prospectRock(player, objectX, objectY, "mithril");
                    break;

                case 11954: // Iron rocks
                case 11955:
                case 11956:
                case 14856:
                case 14857:
                case 14858:
                case 31071:
                case 31072:
                case 31073:
                case 32441:
                case 32442:
                case 32443:
                    Mining.prospectRock(player, objectX, objectY, "iron");
                    break;

                case 14859: // Rune rocks
                case 14860:
                case 14861:
                    Mining.prospectRock(player, objectX, objectY, "runite");
                    break;

                case 11951: // Gold rocks
                case 11952:
                case 11953:
                case 11183:
                case 11184:
                case 11185:
                case 31065:
                case 31066:
                case 31067:
                case 32432:
                case 32433:
                case 32434:
                case 15576:
                case 15577:
                case 15578:
                    Mining.prospectRock(player, objectX, objectY, "gold");
                    break;

                case 11963: // Adamant rocks
                case 11964:
                case 11965:
                case 11939:
                case 11940:
                case 11941:
                case 14862:
                case 14863:
                case 14864:
                case 31083:
                case 31084:
                case 31085:
                case 32435:
                case 32436:
                case 32437:
                    Mining.prospectRock(player, objectX, objectY, "adamantite");
                    break;

                case 11552: // Empty rocks
                case 11553:
                case 11554:
                case 11555:
                case 11556:
                case 31059:
                case 31060:
                case 31061:
                case 14832:
                case 14833:
                case 14834:
                case 33400:
                case 33401:
                case 33402:
                case 15582:
                case 15583:
                case 15584:
                    Mining.displayEmptyRockMessage(player, new Location(objectX, objectY, player.getLocation().getZ()));
                    break;

                case 2491: // Rune essence
                    Mining.prospectRock(player, objectX, objectY, "Rune essence");
                    break;

                case 27663: // Duel arena bank chest.
                case 2213:  // Catherby bank booth.
                case 11402: // Varrock bank booth.
                case 11758: // Falador bank booth.
                case 36786: // Lumbridge bank booth.
                case 35647: // Al-Kharid bank booth.
                case 25808: // Seers bank booth.
                case 34752: // Ardougne bank booth.
                case 26972: // Edgeville bank booth.
                case 29085: // Ooglog bank booth.
                    player.getBank().openBank(false, objectX, objectY);
                    break;
            }
        }

        private void handleThirdClickObject(Player player, Packet packet)
        {
            short id = packet.readLEShortA();
            int y = packet.readLEShortA();
            int x = packet.readLEShort();
            if (player.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            if (x < 1000 || id < 0 || y < 1000 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            player.setFaceLocation(new Location(x, y, player.getLocation().getZ()));
            Console.WriteLine("Third object click = " + id + " " + x + " " + y);
            if (LaddersAndStairs.useObject(player, id, new Location(x, y, player.getLocation().getZ()), 3))
            {
                return;
            }
            switch (id)
            {
                case 28089: // GE desk
                    Server.getGrandExchange().clickDesk(player, x, y, 3);
                    break;
            }
        }

        private void handleFourthClickObject(Player player, Packet packet)
        {
            int y = packet.readLEShort();
            int x = packet.readLEShortA();
            ushort id = packet.readUShort();
            if (player.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            if (x < 1000 || id < 0 || y < 1000 || player.isDead() || player.getTemporaryAttribute("cantDoAnything") != null)
            {
                return;
            }
            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            player.setFaceLocation(new Location(x, y, player.getLocation().getZ()));
            Console.WriteLine("Fourth object click = " + id + " " + x + " " + y);
            switch (id)
            {
                case 28089: // GE desk
                    Server.getGrandExchange().clickDesk(player, x, y, 3);
                    break;
            }
        }

        private void handleExamineObject(Player player, Packet packet)
        {
            ushort id = (ushort)packet.readLEShortA();

            if (player.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }

            SkillHandler.resetAllSkills(player);
            player.getPackets().closeInterfaces();
            player.getPackets().sendMessage("[Id: " + id + "] " + ObjectData.forId(id).getExamine());
        }
    }
}