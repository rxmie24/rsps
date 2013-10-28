using RS2.Server.definitions;
using RS2.Server.minigames.duelarena;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace RS2.Server
{
    internal class LoginHandler
    {
        /**
         * Players to load, at this point they are just connections.
         */
        private Queue<Connection> playersToLoad;

        /**
         * Players to save.
         */
        private Queue<Player> playersToSave;

        public enum ReturnCode
        {
            DISPLAY_ADVERTISEMENT = 1,
            LOGIN_OK = 2,
            INVALID_PASSWORD = 3,
            BANNED = 4,
            ALREADY_ONLINE = 5,
            GAME_UPDATED_RELOAD = 6,
            WORLD_FULL = 7,
            LOGIN_SERVER_OFFLINE = 8,
            LOGIN_LIMIT_EXCEEDED = 9,
            BAD_SESSION_ID = 10,
            FORCE_CHANGE_PASSWORD = 11,
            MEMBERS_WORLD = 12,
            COULD_NOT_COMPLETE = 13,
            UPDATE_IN_PROGRESS = 14
            /*
             * Anything else is 'unexpected response, please try again'.
             */
        }

        public LoginHandler()
        {
            this.playersToLoad = new Queue<Connection>();
            this.playersToSave = new Queue<Player>();
            new Thread(loadSaveThread).Start();
        }

        public void doLogin(Connection connection)
        {
            if (connection == null)
                return;

            switch (connection.loginStage)
            {
                case 0: //attempts to login, could also be update server.
                case 2: //login server
                case 4: //login server
                    attemptPlayerLogin(connection);
                    break;

                case 1: //update server [odd numbers]
                case 3: //update server [odd numbers]
                case 5: //update server [odd numbers]
                    updateServer(connection);
                    break;
            }
        }

        public void loadSaveThread()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(30);
                }
                catch (ThreadInterruptedException)
                {
                    forceSaveAllPlayers();
                    break;
                }

                lock (playersToLoad)
                {
                    if (playersToLoad.Count > 0)
                    {
                        Connection connection = null;
                        while (playersToLoad.Count > 0)
                        {
                            connection = playersToLoad.Dequeue();
                            if (connection != null)
                            {
                                ReturnCode returnCode = loadPlayer(connection);
                                PacketBuilder pb = new PacketBuilder().setSize(Packet.Size.Bare);
                                int slot = -1;
                                if (returnCode == ReturnCode.LOGIN_OK)
                                {
                                    slot = Server.register(connection);
                                    if (slot == -1)
                                    {
                                        returnCode = ReturnCode.WORLD_FULL;
                                    }
                                }
                                pb.addByte((byte)returnCode);
                                if (returnCode == ReturnCode.LOGIN_OK)
                                {
                                    pb.addByte((byte)connection.getPlayer().getRights()); // rights
                                    pb.addByte((byte)0); //1
                                    pb.addByte((byte)0);//Flagged, will genrate mouse packets
                                    pb.addByte((byte)0); //3
                                    pb.addByte((byte)0); //4
                                    pb.addByte((byte)0); //5
                                    pb.addByte((byte)0); // Generates packets
                                    pb.addUShort(slot);//PlayerID
                                    pb.addByte((byte)1); // membership flag #1?..this one enables all GE boxes
                                    pb.addByte((byte)1); // membership flag #2?
                                    connection.SendPacket(pb.toPacket());
                                    connection.getPlayer().getPackets().sendMapRegion();
                                    connection.getPlayer().setActive(true);
                                    Console.WriteLine("Loaded " + connection.getPlayer().getLoginDetails().getUsername() + "'s game: returncode = " + returnCode + ".");
                                }
                                else
                                {
                                    connection.SendPacket(pb.toPacket());
                                }
                            }
                        }
                    }
                }
                lock (playersToSave)
                {
                    if (playersToSave.Count > 0)
                    {
                        Player p = null;
                        while (playersToSave.Count > 0)
                        {
                            p = playersToSave.Dequeue();
                            if (p != null)
                            {
                                if (savePlayer(p))
                                {
                                    Console.WriteLine("Saved " + p.getLoginDetails().getUsername() + "'s game.");
                                }
                                else
                                {
                                    Console.WriteLine("Could not save " + p.getLoginDetails().getUsername() + "'s game.");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void forceSaveAllPlayers()
        {
            // save ALL games
            Console.WriteLine("Saving all games...");
            int saved = 0;
            int total = 0;
            foreach (Player p in Server.players)
            {
                total++;
                if (savePlayer(p))
                {
                    Console.WriteLine("Saved " + p.getLoginDetails().getUsername() + "'s game.");
                    saved++;
                }
                else
                {
                    Console.WriteLine("Could not save " + p.getLoginDetails().getUsername() + "'s game.");
                }
            }
            if (total == 0)
                Console.WriteLine("No games to save.");
            else
                Console.WriteLine("Saved " + (saved / total * 100) + "% of games (" + saved + "/" + total + ").");
        }

        public ReturnCode loadPlayer(Connection connection)
        {
            if (connection == null)
                return ReturnCode.COULD_NOT_COMPLETE;
            LoginDetails loginDetails = connection.getLoginDetails();
            if (loginDetails == null || loginDetails.getUsername() == "" || loginDetails.getLongName() == 0)
                return ReturnCode.INVALID_PASSWORD;//ReturnCode.INVALID_PASSWORD;

            foreach (char c in loginDetails.getUsername().ToCharArray())
            {
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                    return ReturnCode.INVALID_PASSWORD;
            }
            Player createdPlayer = new Player(connection);
            connection.setPlayer(createdPlayer); //player finally created.
            createdPlayer.setLoginDetails(loginDetails);

            if (!File.Exists(Misc.getServerPath() + @"\accounts\" + loginDetails.getUsername() + ".xml"))
            {
                createdPlayer.setRights(2); //all new users admins atm (change later).
                createdPlayer.setLocation(new Location(2323, 3174, 0));
                return ReturnCode.LOGIN_OK; //new user.
            }
            //Yeah reading XML files is a bit homo.
            try
            {
                int temp;
                long lTemp;
                string username = createdPlayer.getLoginDetails().getUsername().ToLower();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Misc.getServerPath() + @"\accounts\" + username + ".xml");

                XmlNode xmlNode = xmlDocument.SelectSingleNode("/Player/Login/Password");
                if (xmlNode == null) return ReturnCode.INVALID_PASSWORD; //no password node.
                if (createdPlayer.getLoginDetails().getPassword() != xmlNode.InnerText)
                    return ReturnCode.INVALID_PASSWORD;

                XmlNode loginElement = xmlDocument.SelectSingleNode("/Player/Login");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNodeList childs = loginElement.ChildNodes;

                    foreach (XmlElement element in childs)
                    {
                        switch (element.Name)
                        {
                            case "Rights":
                                if (!int.TryParse(element.InnerText, out temp))
                                    temp = 0;
                                createdPlayer.setRights(temp);
                                break;

                            case "BankPin":
                                if (element.InnerText == "") continue;
                                createdPlayer.getBank().setBankPin(element.InnerText);
                                break;
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/Position");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNodeList childs = loginElement.ChildNodes;
                    Location location = new Location();
                    foreach (XmlElement element in childs)
                    {
                        switch (element.Name)
                        {
                            case "X":
                                if (!int.TryParse(element.InnerText, out temp))
                                {
                                    createdPlayer.setLocation(Constants.HOME_SPAWN_LOCATION);
                                    break;
                                }
                                location.setX(temp);
                                break;

                            case "Y":
                                if (!int.TryParse(element.InnerText, out temp))
                                {
                                    createdPlayer.setLocation(Constants.HOME_SPAWN_LOCATION);
                                    break;
                                }
                                location.setY(temp);
                                break;

                            case "Z":
                                if (!int.TryParse(element.InnerText, out temp))
                                    temp = 0;
                                location.setZ(temp);
                                break;
                        }
                    }
                    createdPlayer.setLocation(location);
                    if (Location.atDuelArena(createdPlayer.getLocation()))
                        DuelSession.teleportDuelArenaHome(createdPlayer);
                }

                xmlNode = xmlDocument.SelectSingleNode("/Player/Settings/RunEnergy");
                if (xmlNode == null)
                {
                    temp = 100;
                }
                else
                {
                    if (!int.TryParse(xmlNode.InnerText, out temp))
                        temp = 100;
                }
                createdPlayer.setRunEnergyLoad(temp);

                loginElement = xmlDocument.SelectSingleNode("/Player/Settings/PrivacySettings");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNodeList childs = loginElement.ChildNodes;
                    Friends.STATUS publicStatus = Friends.STATUS.ON, privateStatus = Friends.STATUS.ON, tradeStatus = Friends.STATUS.ON;

                    foreach (XmlElement element in childs)
                    {
                        switch (element.Name)
                        {
                            case "Public":
                                publicStatus = (Friends.STATUS)Enum.Parse(typeof(Friends.STATUS), element.InnerText, true);
                                break;

                            case "Private":
                                privateStatus = (Friends.STATUS)Enum.Parse(typeof(Friends.STATUS), element.InnerText, true);
                                break;

                            case "Trade":
                                tradeStatus = (Friends.STATUS)Enum.Parse(typeof(Friends.STATUS), element.InnerText, true);
                                break;
                        }
                    }
                    createdPlayer.getFriends().setPrivacyOptions(publicStatus, privateStatus, tradeStatus);
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/Friends");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNodeList childs = loginElement.ChildNodes; //number of Friends

                    foreach (XmlElement element in childs)
                    {
                        if (element.Name == "Friend")
                        {
                            if (long.TryParse(element.InnerText, out lTemp))
                                createdPlayer.getFriends().getFriendsList().Add(lTemp);
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/Ignores");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNodeList childs = loginElement.ChildNodes; //number of Friends

                    foreach (XmlElement element in childs)
                    {
                        if (element.Name == "Ignore")
                        {
                            if (long.TryParse(element.InnerText, out lTemp))
                                createdPlayer.getFriends().getIgnoresList().Add(lTemp);
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/Stats");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNode skillNode;
                    foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                    {
                        skillNode = loginElement.SelectSingleNode("/Player/Stats/" + skill.ToString());

                        foreach (XmlElement element in skillNode.ChildNodes)
                        { //CurrentLevel/XP
                            switch (element.Name)
                            {
                                case "CurrentLevel":
                                    if (!int.TryParse(element.InnerText, out temp))
                                    {
                                        if (skill != Skills.SKILL.HITPOINTS)
                                            createdPlayer.getSkills().setCurLevel(skill, 1);
                                        else
                                            createdPlayer.getSkills().setCurLevel(skill, 10);
                                        break;
                                    }
                                    createdPlayer.getSkills().setCurLevel(skill, temp);
                                    break;

                                case "XP":
                                    if (!int.TryParse(element.InnerText, out temp))
                                    {
                                        if (skill != Skills.SKILL.HITPOINTS)
                                            createdPlayer.getSkills().setXp(skill, 0);
                                        else
                                            createdPlayer.getSkills().setXp(skill, 1184);
                                        break;
                                    }
                                    createdPlayer.getSkills().setXp(skill, temp);
                                    break;
                            }
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/EquipmentItems");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    XmlNode skillNode;

                    foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
                    {
                        if (equip == ItemData.EQUIP.NOTHING) continue;
                        skillNode = loginElement.SelectSingleNode("/Player/EquipmentItems/" + equip.ToString());
                        if (skillNode == null) continue;
                        int id = -1, amount = 0;
                        foreach (XmlElement element in skillNode.ChildNodes)
                        {
                            switch (element.Name)
                            {
                                case "Id":
                                    if (!int.TryParse(element.InnerText, out id))
                                        id = -1;
                                    break;

                                case "Amount":
                                    if (!int.TryParse(element.InnerText, out amount))
                                        amount = 0;
                                    break;
                            }
                        }
                        if (id != -1)
                        {
                            createdPlayer.getEquipment().getEquipment()[(int)equip].setItemId(id);
                            createdPlayer.getEquipment().getEquipment()[(int)equip].setItemAmount(amount);
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/InventoryItems");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    int slot = -1, id = 0, amount = 0;
                    foreach (XmlElement itemElement in loginElement.ChildNodes) //each item.
                    {
                        foreach (XmlElement itemDef in itemElement.ChildNodes) //each item.
                        {
                            switch (itemDef.Name)
                            {
                                case "Slot":
                                    if (!int.TryParse(itemDef.InnerText, out slot))
                                        slot = -1;
                                    if (slot < 0 || slot > Inventory.MAX_INVENTORY_SLOTS) slot = -1;
                                    break;

                                case "Id":
                                    if (!int.TryParse(itemDef.InnerText, out id))
                                        slot = -1;
                                    break;

                                case "Amount":
                                    if (!int.TryParse(itemDef.InnerText, out amount))
                                        slot = -1;
                                    break;
                            }
                        }
                        if (slot != -1)
                        {
                            createdPlayer.getInventory().getItems()[slot].setItemId(id);
                            createdPlayer.getInventory().getItems()[slot].setItemAmount(amount);
                        }
                    }
                }

                loginElement = xmlDocument.SelectSingleNode("/Player/BankItems");
                if (loginElement != null && loginElement.HasChildNodes)
                {
                    int slot = -1, id = 0, amount = 0;
                    foreach (XmlElement itemElement in loginElement.ChildNodes) //each item.
                    {
                        foreach (XmlElement itemDef in itemElement.ChildNodes) //each item.
                        {
                            switch (itemDef.Name)
                            {
                                case "Slot":
                                    if (!int.TryParse(itemDef.InnerText, out slot))
                                        slot = -1;
                                    if (slot < 0 || slot > Inventory.MAX_INVENTORY_SLOTS) slot = -1;
                                    break;

                                case "Id":
                                    if (!int.TryParse(itemDef.InnerText, out id))
                                        slot = -1;
                                    break;

                                case "Amount":
                                    if (!int.TryParse(itemDef.InnerText, out amount))
                                        slot = -1;
                                    break;
                            }
                        }
                        if (slot != -1)
                        {
                            createdPlayer.getBank().getBank()[slot].setItemId(id);
                            createdPlayer.getBank().getBank()[slot].setItemAmount(amount);
                        }
                    }
                }

                return ReturnCode.LOGIN_OK; //new user.
            }
            catch (Exception e)
            {
                Misc.WriteError(e.Message);
                return ReturnCode.COULD_NOT_COMPLETE;
            }
        }

        public bool savePlayer(Player p)
        {
            if (p == null) return false;
            try
            {
                string username = p.getLoginDetails().getUsername().ToLower();

                /* Character saving code goes here */
                XmlTextWriter writer = new XmlTextWriter(Misc.getServerPath() + @"\accounts\" + username + ".xml", null);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Player");
                writer.WriteStartElement("Login");
                writer.WriteElementString("Password", p.getLoginDetails().getPassword());
                writer.WriteElementString("Rights", p.getRights().ToString());
                writer.WriteElementString("BankPin", p.getBank().getBankPin());
                writer.WriteEndElement();

                writer.WriteStartElement("Position");
                writer.WriteElementString("X", p.getLocation().getX().ToString());
                writer.WriteElementString("Y", p.getLocation().getY().ToString());
                writer.WriteElementString("Z", p.getLocation().getZ().ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Settings");
                writer.WriteElementString("RunEnergy", p.getRunEnergy().ToString());
                writer.WriteStartElement("PrivacySettings");
                writer.WriteElementString("Public", p.getFriends().getPrivacyOption(0).ToString());
                writer.WriteElementString("Private", p.getFriends().getPrivacyOption(1).ToString());
                writer.WriteElementString("Trade", p.getFriends().getPrivacyOption(2).ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Friends");
                foreach (long friend in p.getFriends().getFriendsList())
                    writer.WriteElementString("Friend", friend.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Ignores");
                foreach (long ignore in p.getFriends().getIgnoresList())
                    writer.WriteElementString("Ignore", ignore.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Stats");
                foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                {
                    writer.WriteStartElement(skill.ToString()); //skill name.
                    writer.WriteElementString("CurrentLevel", p.getSkills().getCurLevel(skill).ToString());
                    writer.WriteElementString("XP", p.getSkills().getXp(skill).ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                Item item;
                writer.WriteStartElement("EquipmentItems");
                foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
                {
                    if (equip == ItemData.EQUIP.NOTHING) continue;
                    item = p.getEquipment().getSlot(equip);
                    if (item.getItemId() == -1) continue; //empty slot.

                    writer.WriteStartElement(equip.ToString());
                    writer.WriteElementString("Id", item.getItemId().ToString());
                    writer.WriteElementString("Amount", item.getItemAmount().ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("InventoryItems");
                for (int i = 0; i < Inventory.MAX_INVENTORY_SLOTS; i++)
                {
                    item = p.getInventory().getSlot(i);
                    if (item.getItemId() == -1) continue; //empty slot.

                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Slot", i.ToString());
                    writer.WriteElementString("Id", item.getItemId().ToString());
                    writer.WriteElementString("Amount", item.getItemAmount().ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("BankItems");
                for (int i = 0; i < Bank.MAX_BANK_SLOTS; i++)
                {
                    item = p.getBank().getSlot(i);
                    if (item.getItemId() == -1) continue; //empty slot.

                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Slot", i.ToString());
                    writer.WriteElementString("Id", item.getItemId().ToString());
                    writer.WriteElementString("Amount", item.getItemAmount().ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                //Write the XML to file and close the writer
                writer.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void addSavePlayer(Player p)
        {
            //start attempting to save the account.
            lock (playersToSave)
            {
                playersToSave.Enqueue(p);
            }
        }

        /**
         * Checks if the Player is the update checker and returns true is update was sent.
         * Used to disconnect sockets faster then 15 timeout disconnect.
         * @param p The Player which the frame should be created for.
         */

        public static bool removableConnection(Connection connection)
        {
            if (connection == null || connection.socket == null || !connection.socket.Connected)
                return true;

            return connection.loginStage == 255;
        }

        private void attemptPlayerLogin(Connection connection)
        {
            if (connection == null)
                return;

            if (connection.loginStage == 0) //Attempt login or update server.
            {
                Packet fill_2 = fillStream(connection, 2);
                if (fill_2 == null)
                    return;

                int connectionType = fill_2.readByte();

                if (connectionType == 15)
                { //it's update server
                    connection.loginStage = 1;
                    updateServer(connection);
                    return;
                }
                else if (connectionType == 255)
                {
                    connection.SendPacket(new PacketBuilder()
                        .setSize(Packet.Size.Bare)
                        .addBytes(Misc.WORLD_LIST_DATA).toPacket());
                    connection.loginStage = 5;
                    updateServer(connection);
                    return;
                }
                else if (connectionType != 14)
                {
                    connection.loginStage = 255; //255 is used as fail.
                    return;
                }

                Random random = new Random();
                long serverSessionKey = ((long)(random.NextDouble() * 99999999D) << 32)
                    + (long)(random.NextDouble() * 99999999D);

                int longPlayerName = fill_2.readByte();

                PacketBuilder s1Response = new PacketBuilder();
                s1Response.setSize(Packet.Size.Bare).addByte((byte)0).addLong(serverSessionKey);
                connection.SendPacket(s1Response.toPacket());
                connection.loginStage = 2;
                attemptPlayerLogin(connection);
            }
            else if (connection.loginStage == 2)
            {
                Packet fill_1 = fillStream(connection, 1);
                if (fill_1 == null)
                    return;

                int loginType = fill_1.readByte();

                if (loginType != 16 && loginType != 18 && loginType != 14)
                {
                    connection.loginStage = 255; //255 is used as fail.
                    return;
                }
                connection.loginStage = 4;
                attemptPlayerLogin(connection);
            }
            else if (connection.loginStage == 4)
            {
                Packet fill_2 = fillStream(connection, 2);
                if (fill_2 == null)
                    return;

                int loginPacketSize = fill_2.readUShort();
                int loginEncryptPacketSize = loginPacketSize - (36 + 1 + 1 + 2);

                if (loginEncryptPacketSize <= 0)
                {
                    connection.loginStage = 255;
                    return;
                }
                Packet fill_loginPacketSize = fillStream(connection, loginPacketSize);
                if (fill_loginPacketSize == null)
                    return;

                int clientVersion = fill_loginPacketSize.readInt();

                if (clientVersion != 530)
                {
                    connection.loginStage = 255;
                    return;
                }

                byte junk1 = fill_loginPacketSize.readByte();
                byte lowMem = fill_loginPacketSize.readByte(); //0 is this still low mem ver?
                byte zero = fill_loginPacketSize.readByte();
                byte b1 = fill_loginPacketSize.readByte();
                ushort s1 = fill_loginPacketSize.readUShort();
                ushort s2 = fill_loginPacketSize.readUShort();
                byte b2 = fill_loginPacketSize.readByte();

                for (int i = 0; i < 24; i++)
                {
                    int cacheIDX = fill_loginPacketSize.readByte();
                }
                string appletSettings = fill_loginPacketSize.readRS2String(); //EkKmok3kJqOeN6D3mDdihco3oPeYN2KFy6W5--vZUbNA
                int someInt1 = fill_loginPacketSize.readInt();
                int someInt2 = fill_loginPacketSize.readInt();
                ushort short1 = fill_loginPacketSize.readUShort();

                for (int i = 0; i < 28; i++)
                {
                    int crcOfClientClasses = fill_loginPacketSize.readInt();
                }

                int junk2 = fill_loginPacketSize.readByte();
                int encryption = fill_loginPacketSize.readByte();

                if (encryption != 10 && encryption != 64)
                {
                    connection.loginStage = 255;
                    return;
                }

                long clientSessionKey = fill_loginPacketSize.readLong();
                long serverSessionKey = fill_loginPacketSize.readLong();
                LoginDetails loginDetails = new LoginDetails();
                loginDetails.setLongName(fill_loginPacketSize.readLong()); //must start a 225.
                loginDetails.setUsername(Misc.longToPlayerName(loginDetails.getLongName()).ToLower().Replace("_", " ").Trim());
                loginDetails.setPassword(fill_loginPacketSize.readRS2String());

                Console.WriteLine("Attempting to login with Username: " + loginDetails.getUsername() + " Password: " + loginDetails.getPassword());
                connection.setLoginDetails(loginDetails);
                //start attempting to login the account.
                lock (playersToLoad)
                {
                    playersToLoad.Enqueue(connection);
                }

                connection.loginStage = 6;
            }
        }

        private void updateServer(Connection connection)
        {
            if (connection == null)
                return;

            try
            {
                if (connection.loginStage == 1)
                {
                    Packet fill_3 = fillStream(connection, 3);
                    if (fill_3 == null) //really is 5, but we guess first 2 could be login server and not updateServer.
                        return;

                    PacketBuilder u1Response = new PacketBuilder();
                    u1Response.setSize(Packet.Size.Bare).addByte((byte)0);
                    connection.SendPacket(u1Response.toPacket());
                    connection.loginStage = 3;
                    updateServer(connection);
                }
                else if (connection.loginStage == 3)
                {
                    Packet fill_8 = fillStream(connection, 8);
                    if (fill_8 == null)
                        return;

                    PacketBuilder ukeys = new PacketBuilder();
                    ukeys.setSize(Packet.Size.Bare).addBytes(Misc.UPDATE_KEYS);
                    connection.SendPacket(ukeys.toPacket());
                    connection.loginStage = 5;
                    updateServer(connection);
                }
                else if (connection.loginStage == 5)
                {
                    Packet fill_1 = fillStream(connection, 1);
                    if (fill_1 == null)
                        return;
                    //this is some unknown/not useful packet sent by client useful for quick disconnection.
                    connection.loginStage = 255;
                }
            }
            catch (Exception exception)
            {
                Misc.WriteError(exception.Message);
            }
        }

        /**
         * Check and read any incoming bytes.
         * @param p The Player which the frame should be created for.
         * @param forceRead How many bytes to read from the buffer.
         */

        private Packet fillStream(Connection connection, int forceRead)
        {
            if (connection == null)
                return null;

            if (connection.chuckedRawPackets.Count() < forceRead)
                return null;

            PacketBuilder pckt = new PacketBuilder();
            pckt.setSize(Packet.Size.Bare).addBytes(connection.chuckedRawPackets.GetRange(0, forceRead).ToArray(), 0, forceRead);
            connection.chuckedRawPackets.RemoveRange(0, forceRead); //delete read data.
            return pckt.toPacket();
        }
    }
}