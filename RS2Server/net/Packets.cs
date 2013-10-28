using RS2.Server.clans;
using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.grandexchange;
using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;
using System;

namespace RS2.Server.net
{
    internal class Packets
    {
        private Player player;
        private Connection connection;
        private int count = 0;

        public Packets(Player player)
        {
            this.player = player;
            this.connection = player.getConnection();
        }

        public void sendLogin()
        {
            sendWindowPane(548);
            sendSkillLevels();
            sendMessage("Welcome to C# RS2 Server.");
            sendEnergy();
            sendConfig(173, 0);
            refreshInventory();
            refreshEquipment();
            sendPlayerOption("Follow", 3, 0);
            sendPlayerOption("Trade with", 4, 0);
            player.getFriends().refresh();
            player.refresh();
            player.getEquipment().setWeapon();
            setPrivacyOptions();
        }

        public void sendMessage(string message)
        {
            connection.SendPacket(new PacketBuilder().setId(70).setSize(Packet.Size.VariableByte)
                .addString(message).toPacket());
        }

        public void sendSkillLevels()
        {
            foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                sendSkillLevel(skill);
        }

        public void sendSkillLevel(Skills.SKILL skill)
        {
            connection.SendPacket(new PacketBuilder().setId(38)
                .addByteA((byte)player.getSkills().getCurLevel(skill))
                .addInt1((int)player.getSkills().getXp(skill))
                .addByte((byte)skill).toPacket());
        }

        public void sendMapRegion()
        {
            player.getUpdateFlags().setLastRegion((Location)player.getLocation().Clone());
            if (player.getLocation().getX() >= 19000)
            {
                sendFightCaveMapdata();
                return;
            }
            PacketBuilder pb = new PacketBuilder().setId(162).setSize(Packet.Size.VariableShort);
            bool forceSend = true;
            if (((((player.getLocation().getRegionX() / 8) == 48) || ((player.getLocation().getRegionX() / 8) == 49)) && ((player.getLocation().getRegionY() / 8) == 48))
                || (((player.getLocation().getRegionX() / 8) == 48) && ((player.getLocation().getRegionY() / 8) == 148)))
            {
                forceSend = false;
            }
            pb.addShortA(player.getLocation().getLocalX());

            for (int xCalc = (player.getLocation().getRegionX() - 6) / 8; xCalc <= ((player.getLocation().getRegionX() + 6) / 8); xCalc++)
            {
                for (int yCalc = (player.getLocation().getRegionY() - 6) / 8; yCalc <= ((player.getLocation().getRegionY() + 6) / 8); yCalc++)
                {
                    int region = yCalc + (xCalc << 8);
                    if (forceSend || ((yCalc != 49) && (yCalc != 149) && (yCalc != 147) && (xCalc != 50) && ((xCalc != 49) || (yCalc != 47))))
                    {
                        int[] mapData = MapData.getData(region);
                        if (mapData == null)
                        {
                            pb.addInt2(0);
                            pb.addInt2(0);
                            pb.addInt2(0);
                            pb.addInt2(0);
                        }
                        else
                        {
                            pb.addInt2(mapData[0]);
                            pb.addInt2(mapData[1]);
                            pb.addInt2(mapData[2]);
                            pb.addInt2(mapData[3]);
                        }
                    }
                }
            }
            pb.addByteS(player.getLocation().getZ());
            pb.addUShort(player.getLocation().getRegionX());
            pb.addShortA(player.getLocation().getRegionY());
            pb.addShortA(player.getLocation().getLocalY());
            connection.SendPacket(pb.toPacket());
            Server.getGroundItems().refreshGlobalItems(player);
            Server.getGlobalObjects().refreshGlobalObjects(player);
        }

        private int lastX = 0, lastY = 0;

        public void sendFightCaveMapdata()
        {
            lastX = lastX == 0 ? 2413 : (player.getLocation().getX() - (20000 + (200 * player.getIndex())));
            lastY = lastY == 0 ? 5116 : (player.getLocation().getY() - 20000);
            PacketBuilder pb = new PacketBuilder().setId(214).setSize(Packet.Size.VariableShort);
            pb.addUShortA(player.getLocation().getLocalX());
            pb.addUShortA(player.getLocation().getRegionX());
            pb.addByteS(player.getLocation().getZ());
            pb.addUShortA(player.getLocation().getLocalY());
            pb.initBitAccess();
            for (int height = 0; height < 4; height++)
            {
                for (int xCalc = ((lastX >> 3) - 6); xCalc <= ((lastX >> 3) + 6); xCalc++)
                {
                    for (int yCalc = ((lastY >> 3) - 6); yCalc <= ((lastY >> 3) + 6); yCalc++)
                    {
                        int region = yCalc / 8 + (xCalc / 8 << 8);
                        if (height == player.getLocation().getZ() && region == 9551)
                        {
                            pb.addBits(1, 1);
                            pb.addBits(26, (xCalc << 14) | (yCalc << 3) | (0 << 1) | (0 << 24));
                        }
                        else
                        {
                            pb.addBits(1, 0);
                        }
                    }
                }
            }
            pb.finishBitAccess();
            int[] sent = new int[4 * 13 * 13];
            int sentIndex = 0;
            for (int xCalc = (((lastX >> 3) - 6) / 8); xCalc <= (((lastX >> 3) + 6) / 8); xCalc++)
            {
            outer:
                for (int yCalc = (((lastY >> 3) - 6) / 8); yCalc <= (((lastY >> 3) + 6) / 8); yCalc++)
                {
                    int region = yCalc + (xCalc << 8);
                    if (region != 9551)
                    {
                        continue;
                    }
                    for (int i = 0; i < sentIndex; i++)
                    {
                        if (sent[i] == region)
                        {
                            goto outer;
                        }
                    }
                    sent[sentIndex] = region;
                    sentIndex++;
                    int[] mapData = MapData.getData(region);
                    if (mapData == null)
                    {
                        pb.addInt2(0);
                        pb.addInt2(0);
                        pb.addInt2(0);
                        pb.addInt2(0);
                    }
                    else
                    {
                        pb.addInt2(mapData[0]);
                        pb.addInt2(mapData[1]);
                        pb.addInt2(mapData[2]);
                        pb.addInt2(mapData[3]);
                    }
                }
            }
            pb.addUShort(player.getLocation().getRegionY());
            connection.SendPacket(pb.toPacket());
        }

        public void playSoundEffect(int soundId, byte repeatAmount, ushort delayToStart)
        {
            connection.SendPacket(new PacketBuilder().setId(172)
                .addUShort(soundId)
                .addByte(repeatAmount)
                .addUShort(delayToStart).toPacket());
        }

        public void sendItems(int interfaceId, int childId, int type, Item[] inventory)
        {
            PacketBuilder pb = new PacketBuilder().setId(105).setSize(Packet.Size.VariableShort);
            pb.addInt((interfaceId << 16) + childId);
            pb.addUShort(type);
            pb.addUShort(inventory.Length);
            for (int i = 0; i < inventory.Length; i++)
            {
                Item item = inventory[i];
                int id = -1, amount = 0;
                if (inventory[i] != null)
                {
                    id = item.getItemId();
                    amount = item.getItemAmount();
                }
                if (amount > 254)
                {
                    pb.addByteS(255);
                    pb.addInt(amount);
                }
                else
                {
                    pb.addByteS(amount);
                }
                pb.addUShort(id + 1);
            }
            connection.SendPacket(pb.toPacket());
        }

        public void refreshInventory()
        {
            sendItems(149, 0, 93, player.getInventory().getItems());
        }

        public void refreshEquipment()
        {
            sendItems(387, 28, 94, player.getEquipment().getEquipment());
        }

        public void updateGEProgress(GEItem offer)
        {
            connection.SendPacket(new PacketBuilder().setId(116)
                .addByte((byte)offer.getSlot())
                .addByte((byte)offer.getProgress())
                .addUShort(offer.getDisplayItem())
                .addInt(offer.getPriceEach())
                .addInt(offer.getTotalAmount())
                .addInt(offer.getAmountTraded())
                .addInt(offer.getTotalAmount() * offer.getPriceEach()).toPacket());
        }

        public void resetGESlot(int slot)
        {
            connection.SendPacket(new PacketBuilder().setId(116)
                .addByte((byte)slot)
                .addByte((byte)0)
                .addUShort(0)
                .addInt(0)
                .addInt(0)
                .addInt(0)
                .addInt(0).toPacket());
        }

        public void sendBankOptions()
        {
            setRightClickOptions(1278, (762 * 65536) + 73, 0, 496);
            setRightClickOptions(2360446, (763 * 65536), 0, 27);
            sendBlankClientScript(239, 1451);
        }

        public void displayEnterAmount()
        {
            object[] o = { "Enter amount:" };
            sendClientScript(108, o, "s");
        }

        public void displayEnterText(string s)
        {
            object[] o = { s };
            sendClientScript(109, o, "s");
        }

        public void tradeWarning(int slot)
        {
            object[] opt = new object[] { slot, 7, 4, 21954593 };
            sendClientScript(143, opt, "Iiii");
        }

        public void sendEnergy()
        {
            connection.SendPacket(new PacketBuilder().setId(234)
                .addByte((byte)player.getRunEnergy()).toPacket());
        }

        public void setPrivacyOptions()
        {
            connection.SendPacket(new PacketBuilder().setId(232)
                .addByte((byte)player.getFriends().getPrivacyOption(0))
                .addByte((byte)player.getFriends().getPrivacyOption(1))
                .addByte((byte)player.getFriends().getPrivacyOption(2)).toPacket());
        }

        public void sendPlayerOption(string option, int slot, int pos)
        {
            connection.SendPacket(new PacketBuilder().setId(44).setSize(Packet.Size.VariableByte)
                .addUShortA(65535)
                .addByte((byte)pos)
                .addByte((byte)slot)
                .addString(option).toPacket());
        }

        public void sendSentPrivateMessage(long name, string text)
        {
            connection.SendPacket(new PacketBuilder().setId(71).setSize(Packet.Size.VariableByte)
                .addLong(name)
                .addBytes(Misc.textPack(text)).toPacket());
        }

        public void setFriendsListStatus()
        {
            connection.SendPacket(new PacketBuilder().setId(197)
                .addByte((byte)2).toPacket());
        }

        public void sendReceivedPrivateMessage(long name, int rights, string message, byte[] packed)
        {
            int messageCounter = player.getFriends().getNextUniqueId();
            //byte[] bytes = new byte[message.getBytes().length];
            //Misc.textPack(bytes, message);
            //Misc.method251(bytes, 0, 1, message.length(), message.getBytes());
            connection.SendPacket(new PacketBuilder().setId(0).setSize(Packet.Size.VariableByte)
                .addLong(name)
                .addUShort(0) // used with the message counter below
                .addThreeBytes(messageCounter)
                //.addBytes(new byte[] { (byte)((messageCounter << 16) & 0xFF), (byte)((messageCounter << 8) & 0xFF), (byte)(messageCounter & 0xFF) })
                .addByte((byte)rights)
                .addBytes(packed).toPacket());
        }

        public void sendFriend(long name, int world)
        {
            Clan c = Server.getClanManager().getClanByOwner(player.getLoginDetails().getUsername());
            Clan.ClanRank clanRank = Clan.ClanRank.FRIEND;
            if (c != null)
                clanRank = c.getUserRank(Misc.longToPlayerName(name));

            PacketBuilder pb = new PacketBuilder().setId(62).setSize(Packet.Size.VariableByte)
                .addLong(name)
                .addUShort(world)
                .addByte((byte)clanRank);
            if (world != 0)
            {
                if (world == player.getWorld())
                    pb.addString("Online");
                else
                    pb.addString("Server " + world);
            }
            connection.SendPacket(pb.toPacket());
        }

        public void sendIgnores(long[] names)
        {
            PacketBuilder pb = new PacketBuilder().setId(126).setSize(Packet.Size.VariableShort);
            foreach (long name in names)
            {
                pb.addLong(name);
            }
            connection.SendPacket(pb.toPacket());
        }

        public void newClanMessage(Clan c, ChatMessage chatMessage)
        {
            PacketBuilder pb = new PacketBuilder();
            pb.setId(54).setSize(Packet.Size.VariableByte);
            pb.addLong(chatMessage.getPlayer().getLoginDetails().getLongName());
            pb.addByte((byte)1); // dummy
            pb.addLong(Misc.playerNameToLong(c.getClanName()));
            pb.addUShort(0); // some message counter bs
            string message = chatMessage.getChatText();
            int messageCounter = player.getFriends().getNextUniqueId();
            pb.addThreeBytes(messageCounter);
            pb.addByte((byte)chatMessage.getPlayer().getRights());
            pb.addBytes(chatMessage.getPacked());
            connection.SendPacket(pb.toPacket());
        }

        public void updateClan(Clan c)
        {
            PacketBuilder pb = new PacketBuilder();
            pb.setId(55).setSize(Packet.Size.VariableShort);
            pb.addLong(Misc.playerNameToLong(c.getClanOwner()));
            pb.addLong(Misc.playerNameToLong(c.getClanName()));
            pb.addByte((byte)c.getKickRights());
            pb.addByte((byte)c.getUserList().Count);
            foreach (ClanUser list in c.getUserList())
            {
                Player p = list.getClanMember();
                pb.addLong(p.getLoginDetails().getLongName());
                pb.addUShort(p.getWorld());
                int rights = Convert.ToInt32(list.getClanRights());
                pb.addByte((byte)rights);
                pb.addString("Server " + p.getWorld());
            }
            connection.SendPacket(pb.toPacket());
        }

        public void resetClanInterface()
        {
            connection.SendPacket(new PacketBuilder().setId(55).setSize(Packet.Size.VariableShort)
                .addLong(0).toPacket());
        }

        public void logout()
        {
            if (player.getFightCave() == null)
            {
                if (!Combat.isXSecondsSinceCombat(player, player.getLastAttacked(), 10000))
                {
                    sendMessage("You must have been out of combat for 10 seconds before you may log out.");
                    return;
                }
            }
            else
                if (player.getFightCave() != null)
                {
                    if (!player.getFightCave().isGamePaused())
                    {
                        sendMessage("You will logout at the end of this wave and your progress will be saved.");
                        sendMessage("Die, or leave via the cave exit to quit the fight cave.");
                        player.getFightCave().setGamePaused(true);
                        return;
                    }
                }
            connection.SendPacket(new PacketBuilder().setId(86).toPacket());
            player.setDisconnected(true);
        }

        public void forceLogout()
        {
            connection.SendPacket(new PacketBuilder().setId(86).toPacket());
            player.setDisconnected(true);
        }

        public void setArrowOnEntity(int type, int id)
        {
            PacketBuilder pb = new PacketBuilder().setId(217);
            int offset = pb.curLength;
            pb.addByte((byte)type); // 10 player, 1 npc
            pb.addByte((byte)((id < 32768) ? 0 : 1)); // arrowtype
            pb.addUShort(id);
            pb.curLength += 3;
            pb.addUShort(65535);
            for (int i = (pb.curLength - offset); i < 9; i++)
            {
                pb.addByte((byte)0);
            }
            connection.SendPacket(pb.toPacket());
        }

        public void setArrowOnPosition(int x, int y, int height)
        {
            PacketBuilder pb = new PacketBuilder().setId(217);
            int offset = pb.curLength;
            pb.addByte((byte)2);
            pb.addByte((byte)0);
            pb.addUShort(x);
            pb.addUShort(y);
            pb.addByte((byte)height);
            pb.addUShort(65535);
            for (int i = (pb.curLength - offset); i < 9; i++)
            {
                pb.addByte((byte)0);
            }
            connection.SendPacket(pb.toPacket());
        }

        public void newEarthquake(int i, int j, int k, int l, int i1)
        {
            connection.SendPacket(new PacketBuilder().setId(27)
                .addUShort(count++)
                .addByte((byte)i)
                .addByte((byte)j)
                .addByte((byte)k)
                .addByte((byte)l)
                .addUShort(i1).toPacket());
        }

        public void resetCamera()
        {
            connection.SendPacket(new PacketBuilder().setId(24)
                .addUShort(count++).toPacket());
        }

        public void newStillGraphics(Location loc, Graphics graphics, byte tilesAway = 0)
        {
            sendCoords(new Location(loc.getX(), loc.getY(), player.getLocation().getZ()));
            connection.SendPacket(new PacketBuilder().setId(17)
                .addByte(tilesAway) //tiles away  (X >> 4 + Y & 7)
                .addUShort(graphics.getId()) // graphic id
                .addByte((byte)graphics.getHeight()) //height of the spell above it's basic place, i think it's written in pixels 100 pixels higher
                .addUShort(graphics.getDelay()).toPacket()); ;//Time before casting the graphic
        }

        public void setMinimapStatus(int setting)
        {
            connection.SendPacket(new PacketBuilder().setId(192)
                .addByte((byte)setting).toPacket());
        }

        public void newSystemUpdate(int time)
        {
            connection.SendPacket(new PacketBuilder().setId(85).addUShort(time * 50 / 30).toPacket());
        }

        public void sendWindowPane(int pane)
        {
            connection.SendPacket(new PacketBuilder().setId(145)
            .addUShortA(pane)
            .addByteA((byte)0)
            .addUShortA(count++).toPacket());
        }

        public void moveChildInterface(int interfaceId, int child, int x, int y)
        {
            //TODO
            /*connection.SendPacket(new PacketBuilder().setId(84)
                .addInt2((x * 65536) + y)
                .addInt2((interfaceId * 65536) + child)
                .addShortA(count++).toPacket());*/
        }

        public void modifyText(string message, int interfaceId, int childId)
        {
            connection.SendPacket(new PacketBuilder().setId(171).setSize(Packet.Size.VariableShort)
                .addInt2((interfaceId << 16) + childId)
                .addString(message)
                .addShortA(count++).toPacket());
        }

        public void sendCoords(Location location)
        {
            int regionX = player.getUpdateFlags().getLastRegion().getRegionX();
            int regionY = player.getUpdateFlags().getLastRegion().getRegionY();
            connection.SendPacket(new PacketBuilder().setId(26)
                .addByteC((byte)(location.getX() - ((regionX - 6) * 8)))
                .addByte((byte)(location.getY() - ((regionY - 6) * 8))).toPacket());
        }

        public void sendProjectileCoords(Location location)
        {
            int regionX = player.getUpdateFlags().getLastRegion().getRegionX();
            int regionY = player.getUpdateFlags().getLastRegion().getRegionY();
            connection.SendPacket(new PacketBuilder().setId(26)
                .addByteC((byte)(location.getX() - ((regionX - 6) * 8) - 3))
                .addByte((byte)(location.getY() - ((regionY - 6) * 8) - 2)).toPacket());
        }

        public void createObject(int objectId, Location loc, int face, int type)
        {
            sendCoords(new Location(loc.getX(), loc.getY(), player.getLocation().getZ()));
            connection.SendPacket(new PacketBuilder().setId(179)
                .addByteA((byte)((type << 2) + (face & 3)))
                .addByte((byte)0)
                .addShortA(objectId).toPacket());
        }

        public void newObjectAnimation(Location loc, int anim)
        {
            sendCoords(new Location(loc.getX(), loc.getY(), player.getLocation().getZ()));
            int type = 10;
            int x = loc.getX();
            int face = 0;
            if (anim == 497)
            { // Agility ropeswing
                face = x == 2551 ? 4 : x == 3005 ? 2 : 0;
            }
            connection.SendPacket(new PacketBuilder().setId(20)
                .addByteS(0)
                .addByteS((byte)((type << 2) + (face & 3)))
                .addLEShort(anim).toPacket());
        }

        public void removeObject(Location loc, int face, int type)
        {
            sendCoords(new Location(loc.getX(), loc.getY(), player.getLocation().getZ()));
            connection.SendPacket(new PacketBuilder().setId(195)
                .addByteC((byte)((type << 2) + (face & 3)))
                .addByte((byte)0).toPacket());
        }

        public void createGroundItem(GroundItem item)
        {
            if (item != null)
            {
                sendCoords(item.getLocation());
                connection.SendPacket(new PacketBuilder().setId(33)
                    .addLEShort(item.getItemId())
                    .addByte((byte)0)
                    .addShortA(item.getItemAmount()).toPacket());
            }
        }

        public void createGroundItem2(GroundItem item)
        {
            if (item != null)
            {
                sendCoords(item.getLocation());
                connection.SendPacket(new PacketBuilder().setId(135)
                    .addUShortA(item.getOwner().getIndex())
                    .addByteC((byte)0)
                    .addLEShort(item.getItemAmount())
                    .addLEShort(item.getItemId()).toPacket());
            }
        }

        public void clearGroundItem(GroundItem item)
        {
            if (item != null)
            {
                sendCoords(item.getLocation());
                connection.SendPacket(new PacketBuilder().setId(240)
                    .addByte((byte)0)
                    .addUShort(item.getItemId()).toPacket());
            }
        }

        public void sendProjectile(Location source, Location dest, int startSpeed, int gfx, int angle, int startHeight, int endHeight, int speed, Entity lockon)
        {
            sendProjectileCoords(source);
            connection.SendPacket(new PacketBuilder().setId(16)
                 .addByte((byte)((byte)angle))
                 .addByte((byte)((byte)(source.getX() - dest.getX()) * -1))
                 .addByte((byte)((byte)(source.getY() - dest.getY()) * -1))
                 .addUShort(lockon is Player ? (-lockon.getClientIndex() - 1) : lockon.getClientIndex() + 1)
                 .addUShort(gfx)
                 .addByte((byte)startHeight)
                 .addByte((byte)endHeight)
                 .addUShort(startSpeed)
                 .addUShort(speed)
                 .addByte((byte)((byte)gfx == 53 ? 0 : 16))//arch..0 if cannon
                 .addByte((byte)64).toPacket());
        }

        public void clearMapFlag()
        {
            connection.SendPacket(new PacketBuilder().setId(153).toPacket());
        }

        /* TODO: Clean up Client Scripts, duplicates */

        public void sendBlankClientScript(int id)
        {
            connection.SendPacket(new PacketBuilder().setId(115).setSize(Packet.Size.VariableShort)
                .addUShort(0)
                .addString("")
                .addInt(id).toPacket());
        }

        public void sendBlankClientScript(int id, string s)
        {
            connection.SendPacket(new PacketBuilder().setId(115).setSize(Packet.Size.VariableShort)
                .addUShort(0)
                .addString(s)
                .addInt(id).toPacket());
        }

        public void sendBlankClientScript(int id2, int id)
        {
            connection.SendPacket(new PacketBuilder().setId(115).setSize(Packet.Size.VariableShort)
                .addUShort(id2)
                .addString("")
                .addInt(id).toPacket());
        }

        public void sendClientScript(int id, object[] parameters, string types)
        {
            if (parameters.Length != types.Length)
            {
                Misc.WriteError("params size should be the same as types length");
                return;
            }
            PacketBuilder packet = new PacketBuilder().setId(115).setSize(Packet.Size.VariableShort)
            .addUShort(count++)
            .addString(types);
            int idx = 0;
            for (int i = types.Length - 1; i >= 0; i--)
            {
                if (types[i] == 's')
                    packet.addString((string)parameters[idx]);
                else
                    packet.addInt((int)parameters[idx]);
                idx++;
            }
            packet.addInt(id);
            connection.SendPacket(packet.toPacket());
        }

        public void sendClientScript2(int id2, int id, object[] parameters, string types)
        {
            if (parameters.Length != types.Length)
            {
                Misc.WriteError("params size should be the same as types length");
                return;
            }
            PacketBuilder packet = new PacketBuilder().setId(115).setSize(Packet.Size.VariableShort)
            .addUShort(count++)
            .addString(types);
            int idx = 0;
            for (int i = types.Length - 1; i >= 0; i--)
            {
                if (types[i] == 's')
                {
                    packet.addString((string)parameters[idx]);
                }
                else
                {
                    packet.addInt((int)parameters[idx]);
                }
                idx++;
            }
            packet.addInt(id);
            connection.SendPacket(packet.toPacket());
        }

        /* End of Client Scripts */

        public void setRightClickOptions(int set, int inter, int off, int len)
        {
            connection.SendPacket(new PacketBuilder().setId(165)
                .addLEShort(count++)
                .addLEShort(len)
                .addInt(inter)
                .addShortA(off)
                .addInt1(set).toPacket());
        }

        public void sendConfig1(int id, int value)
        {
            connection.SendPacket(new PacketBuilder().setId(60)
                .addShortA(id)
                .addByteC(value).toPacket());
        }

        public void sendConfig2(int id, int value)
        {
            connection.SendPacket(new PacketBuilder().setId(226)
                .addInt(value)
                .addShortA(id).toPacket());
        }

        public void sendConfig(int id, int value)
        {
            if (value < 128 && value > -128)
            {
                sendConfig1(id, value);
            }
            else
            {
                sendConfig2(id, value);
            }
        }

        public void sendInterface(int showId, int windowId, int interfaceId, int childId)
        {
            int id = windowId * 65536 + interfaceId;
            connection.SendPacket(new PacketBuilder().setId(155)
                .addByte((byte)showId)
                .addInt2(id)
                .addShortA(count++)
                .addUShort(childId).toPacket());
        }

        public void showChildInterface(int interfaceId, int childId, bool show)
        {
            connection.SendPacket(new PacketBuilder().setId(21)
                .addByteC(show ? 0 : 1)
                .addUShort(count++)
                .addLEInt((interfaceId << 16) + childId).toPacket());
        }

        public void sendNPCHead(int npcID, int interfaceID, int childID)
        {
            connection.SendPacket(new PacketBuilder().setId(73)
                .addShortA(npcID)
                .addLEInt((interfaceID << 16) + childID)
                .addLEShort(count++).toPacket());
        }

        public void sendPlayerHead(int interfaceID, int childID)
        {
            connection.SendPacket(new PacketBuilder().setId(66)
                .addUShortA(count++)
                .addInt1((interfaceID << 16) + childID).toPacket());
        }

        public void animateInterface(int animID, int interfaceID, int childID)
        {
            connection.SendPacket(new PacketBuilder().setId(36)
                .addInt2((interfaceID << 16) + childID)
                .addLEShort(animID)
                .addShortA(count++).toPacket());
        }

        public void itemOnInterface(int interfaceId, int childId, int size, int item)
        {
            connection.SendPacket(new PacketBuilder().setId(50)
                .addInt(size)
                .addInt2((interfaceId << 16) + childId)
                .addUShortA(item)
                .addLEShort(count++).toPacket());
        }

        public void sendCloseInterface(int windowId, int position)
        {
            connection.SendPacket(new PacketBuilder().setId(149)
                .addUShort(count++)
                .addInt((windowId << 16) + position).toPacket());
        }

        public void closeInterfaces()
        {
            if (player.isHd())
            {
                sendCloseInterface(746, 6); // Main
                sendCloseInterface(746, 5); // Main
                sendCloseInterface(752, 12); // Chatbox1
                sendCloseInterface(752, 11); // Chatbox2
                sendCloseInterface(746, 76); // Inventory
            }
            else
            {
                //sendCloseInterface(752, 6); // Chatbox 3
                sendCloseInterface(752, 12); // Chatbox1
                sendCloseInterface(752, 11); // Chatbox2
                sendCloseInterface(548, 11); // Main
                sendCloseInterface(548, 80); // Inventory
            }
            player.getBank().setBanking(false);
            player.setShopSession(null);
            player.setTrade(null);
            player.removeTemporaryAttribute("dialogue");
            player.removeTemporaryAttribute("jewelleryTeleport");
            if (player.getGESession() != null)
            {
                if (player.getGESession().getCurrentOffer() != null)
                {
                    if (player.getGESession().getCurrentOffer() is BuyOffer)
                    {
                        sendInterface(0, 752, 6, 137); // Removes the item search
                    }
                }
            }
            player.setGESession(null);
            if (player.getDuel() != null)
            {
                if (player.getDuel().getStatus() < 4 || player.getDuel().getStatus() >= 8)
                {
                    player.setDuelSession(null);
                }
            }
        }

        public void configureGameScreen(int windowType)
        {
            bool resetVariables = false;
            bool achievementDiary = player.isAchievementDiaryTab();
            int magicInterface = player.getMagicType() == 2 ? 193 : player.getMagicType() == 3 ? 430 : 192;
            int lastWindowType = (int)(player.getTemporaryAttribute("lastWindowType") == null ? -1 : (int)player.getTemporaryAttribute("lastWindowType"));
            if (lastWindowType == windowType)
            {
                return;
            }
            if (windowType == 0 || windowType == 1)
            {
                resetVariables = player.isHd();
                player.setHd(false);
                sendWindowPane(548);
                sendTab(14, 751); // Chat options
                sendTab(75, 752); // Chatbox
                sendTab(70, 748); // HP bar
                sendTab(71, 749); // Prayer bar
                sendTab(72, 750); // Energy bar
                //sendTab(67, 747); // Summoning bar
                sendInterface(1, 752, 8, 137); // Username on chat
                sendTab(83, 92); // Attack tab
                sendTab(84, 320); // Skill tab
                sendTab(85, 274); // Quest tab
                sendTab(86, 149); // Inventory tab
                sendTab(87, 387); // Equipment tab
                sendTab(88, 271); // Prayer tab
                sendTab(89, 192); // Magic tab
                sendTab(91, 550); // Friend tab
                sendTab(92, 551); // Ignore tab
                sendTab(93, 589); // Clan tab
                sendTab(94, 261); // Setting tab
                sendTab(95, 464); // Emote tab
                sendTab(96, 187); // Music tab
                sendTab(97, 182); // Logout tab
                sendTab(10, 754); // PM split chat
            }
            else if (windowType == 2 || windowType == 3)
            {
                resetVariables = !player.isHd();
                player.setHd(true);
                sendWindowPane(746);
                sendTab(23, 751); // Chat options
                sendTab(70, 752); // Chatbox
                sendInterface(1, 752, 8, 137);
                sendTab(13, 748); // HP bar
                sendTab(14, 749); // Prayer bar
                sendTab(15, 750); // Energy bar
                //sendTab(16, 747); // Summoning bar
                sendTab(93, 92); // Attack tab
                sendTab(94, 320); // Skill tab
                sendTab(95, achievementDiary ? 259 : 274); // Quest tab
                sendTab(96, 149); // Inventory tab
                sendTab(97, 387); // Equipment tab
                sendTab(98, 271); // Prayer tab
                sendTab(99, magicInterface); // Magic tab
                sendTab(101, 550); // Friend tab
                sendTab(102, 551); // Ignore tab
                sendTab(103, 589); // Clan tab
                sendTab(104, 261); // Setting tab
                sendTab(105, 464); // Emote tab
                sendTab(106, 187); // Music tab
                sendTab(107, 182); // Logout tab
                sendTab(71, 754); // PM split chat
            }
            player.setTemporaryAttribute("lastWindowType", windowType);
            if (resetVariables)
            {
                player.removeTemporaryAttribute("inMulti");
                player.removeTemporaryAttribute("atDuelArea");
                player.removeTemporaryAttribute("atBarrows");
                player.removeTemporaryAttribute("inWild");
                player.removeTemporaryAttribute("atGodwars");
                player.removeTemporaryAttribute("atAgilityArena");
                player.removeTemporaryAttribute("snowInterface");
                player.getEquipment().setWeapon();
            }
            if (player.getTemporaryAttribute("sendLogin") == null)
            {
                sendLogin();
                player.setTemporaryAttribute("sendLogin", true);
            }
        }

        public void softCloseInterfaces()
        {
            if (player.isHd())
            {
                sendCloseInterface(746, 6); // Main
                sendCloseInterface(746, 5); // Main
                sendCloseInterface(752, 12); // Chatbox1
                sendCloseInterface(752, 11); // Chatbox2
                //sendCloseInterface(752, 6); // Chatbox 3
                sendCloseInterface(746, 76); // Inventory
            }
            else
            {
                sendCloseInterface(752, 12); // Chatbox1
                sendCloseInterface(752, 11); // Chatbox2
                //sendCloseInterface(752, 6); // Chatbox 3
                sendCloseInterface(548, 11); // Main
                sendCloseInterface(548, 80); // Inventory
            }
            player.removeTemporaryAttribute("dialogue");
        }

        public void sendChatboxInterface(int childId)
        {
            sendInterface(0, 752, 11, childId);
        }

        public void sendChatboxInterface2(int childId)
        {
            sendInterface(0, 752, 12, childId);
        }

        public void closeChatboxInterface()
        {
            sendCloseInterface(752, 12); // Chatbox1
            sendCloseInterface(752, 11); // Chatbox2
            //sendCloseInterface(752, 6); // Chatbox 3
        }

        public void displayInterface(int id)
        {
            if (player.isHd())
            {
                sendInterface(0, 746, id == 499 ? 5 : 6, id);
                return;
            }
            sendInterface(0, 548, 11, id);
        }

        public void displayInventoryInterface(int childId)
        {
            if (player.isHd())
            {
                sendInterface(0, 746, 76, childId);
                return;
            }
            sendInterface(0, 548, 80, childId);
        }

        public void sendTab(int tabId, int childId)
        {
            if (player.isHd())
            {
                sendInterface(1, childId == 137 ? 752 : 746, tabId, childId);
                return;
            }
            sendInterface(1, childId == 137 ? 752 : 548, tabId, childId);
        }

        public void sendOverlay(int i)
        {
            if (player.isHd())
            {
                sendInterface(1, 746, 3, i);
                return;
            }
            sendInterface(1, 548, 5, i);
        }

        public void sendRemoveOverlay()
        {
            if (player.isHd())
            {
                sendCloseInterface(746, 3);
                return;
            }
            sendCloseInterface(548, 5);
        }

        public void displayMultiIcon()
        {
            if (player.isHd())
            {
                sendInterface(1, 746, 17, 745);
            }
            else
            {
                sendInterface(1, 548, 7, 745);
            }
            showChildInterface(745, 1, true);
        }

        public void removeMultiIcon()
        {
            if (player.isHd())
            {
                sendCloseInterface(746, 17);
                return;
            }
            sendCloseInterface(548, 7);
        }
    }
}