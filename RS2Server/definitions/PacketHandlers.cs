using RS2.Server.net;
using RS2.Server.packethandler;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic;

namespace RS2.Server.definitions
{
    internal class PacketHandlers
    {
        private static Dictionary<PacketId, PacketHandler> handlers = new Dictionary<PacketId, PacketHandler>();
        /* sspoke: these packetSizes are extremely important,
        * they were designed to predict packet size's from
        * the client by decrypted command otherwise it would be
        * impossible because there is 2 kinds of packets
        * 1) Variable size packet (these size can be read from packet
        * 2) Normal packet (these packet's size is just taken from table below)
        * Because thats how jagex client was programmed
        * no way around it.
        */
        public static int[] packetSizes = new int[256];

        public enum PacketId : byte
        {
            CLOSE = 184,
            ACTIONBUTTON = 155,
            ACTIONBUTTON2 = 10,
            ACTIONBUTTON3 = 132,
            IDLE = 245,
            MOVE_CAMERA = 21,
            PING = 93,
            FOCUS = 22,
            CLICK_MOUSE = 75,
            WINDOW_TYPE = 243,
            SOUND_SETTINGS = 98,
            COMMAND = 44,
            PUBLIC = 237,
            CLAN_CHAT = 104,
            ADD_FRIEND = 120,
            DELETE_FRIEND = 57,
            ADD_IGNORE = 34,
            DELETE_IGNORE = 213,
            SEND_PM = 201,
            CLAN_RANKS = 188,
            CLAN_KICK = 162,
            PRIVACY_SETTINGS = 157,
            WALK_1 = 215,
            WALK_2 = 39,
            WALK_3 = 77,
            ATTACK_PLAYER = 68,
            FOLLOW = 71,
            TRADE = 180,
            MAGIC_ON_PLAYER = 195,
            NPC_FIRST_CLICK = 3,
            NPC_SECOND_CLICK = 78,
            NPC_THIRD_CLICK = 148,
            NPC_FOURTH_CLICK = 30,
            NPC_FIFTH_CLICK = 218,
            NPC_EXAMINE = 72,
            MAGIC_ON_NPC = 239,
            ITEM_ON_NPC = 115,
            OBJECT_FIRST_CLICK = 254,
            OBJECT_SECOND_CLICK = 194,
            OBJECT_THIRD_CLICK = 84,
            OBJECT_FOURTH_CLICK = 247,
            OBJECT_EXAMINE = 94,
            EQUIP = 55,
            ITEM_ON_ITEM = 27,
            INV_CLICK = 156,
            ITEM_ON_OBJECT = 134,
            ITEM_ON_GROUND_ITEM = 101,
            INV_OPERATE = 206,
            INV_DROP = 135,
            PICKUP = 66,
            INV_SWAP_SLOT = 231,
            INV_SWAP_SLOT2 = 79,
            INV_RIGHT_CLICK_OPTION1 = 161,
            INV_RIGHT_CLICK_OPTION2 = 153,
            INV_EXAMINE_ITEM = 92,
            ENTER_AMOUNT = 23,
            ENTER_TEXT = 244,
            INTERFACE_CLICK_1 = 81,
            INTERFACE_CLICK_2 = 196,
            INTERFACE_CLICK_3 = 124,
            INTERFACE_CLICK_4 = 199,
            INTERFACE_CLICK_5 = 234,
            INTERFACE_CLICK_6 = 168,
            INTERFACE_CLICK_7 = 166,
            INTERFACE_CLICK_8 = 64,
            INTERFACE_CLICK_9 = 53,
            INTERFACE_CLICK_10 = 223,
            GE_SEARCH = 111,
            REIGON_LOADING = 110,
            PACKET_COUNT = 177,
            CLIENT_VARIABLE_30 = 137,
            DODGY_PACKET = 0,
        };

        public PacketHandlers()
        {
            setPacketSizes();
            PacketHandler temp;

            temp = new ActionButton();
            handlers.Add(PacketId.CLOSE, temp); //Close
            handlers.Add(PacketId.ACTIONBUTTON, temp); //Actionbutton #1
            handlers.Add(PacketId.ACTIONBUTTON2, temp); //Actionbutton #2
            handlers.Add(PacketId.ACTIONBUTTON3, temp); //Actionbutton #3

            temp = new ClientAction();
            handlers.Add(PacketId.IDLE, temp); //Idle
            handlers.Add(PacketId.MOVE_CAMERA, temp); //Move camera
            handlers.Add(PacketId.PING, temp); //Ping
            handlers.Add(PacketId.FOCUS, temp); //Window focus
            handlers.Add(PacketId.CLICK_MOUSE, temp); //Mouse
            handlers.Add(PacketId.WINDOW_TYPE, temp); //Window type
            handlers.Add(PacketId.SOUND_SETTINGS, temp); //Toggle sound (sounds)

            temp = new Command();
            handlers.Add(PacketId.COMMAND, temp); //Commands

            temp = new Communication();
            handlers.Add(PacketId.ADD_FRIEND, temp); //Add friend
            handlers.Add(PacketId.DELETE_FRIEND, temp); //Delete friend
            handlers.Add(PacketId.ADD_IGNORE, temp); //Add ignore
            handlers.Add(PacketId.DELETE_IGNORE, temp); //Delete ignore
            handlers.Add(PacketId.SEND_PM, temp); //Send PM
            handlers.Add(PacketId.CLAN_RANKS, temp); //Set clan ranks
            handlers.Add(PacketId.CLAN_CHAT, temp); //Join clan chat
            handlers.Add(PacketId.PUBLIC, temp); //Public chat
            handlers.Add(PacketId.CLAN_KICK, temp); //Clan chat kick
            handlers.Add(PacketId.PRIVACY_SETTINGS, temp); //public/private/trade chat settings

            temp = new Walk();
            handlers.Add(PacketId.WALK_1, temp); //Walk
            handlers.Add(PacketId.WALK_2, temp); //Walk
            handlers.Add(PacketId.WALK_3, temp); //Walk

            temp = new PlayerInteract();
            handlers.Add(PacketId.ATTACK_PLAYER, temp); //Attack
            handlers.Add(PacketId.FOLLOW, temp); //Follow
            handlers.Add(PacketId.TRADE, temp); //Trade
            handlers.Add(PacketId.MAGIC_ON_PLAYER, temp); //Magic on player

            temp = new NPCInteract();
            handlers.Add(PacketId.NPC_FIRST_CLICK, temp); //Attack
            handlers.Add(PacketId.NPC_SECOND_CLICK, temp); //Second click
            handlers.Add(PacketId.NPC_THIRD_CLICK, temp); //Third click
            handlers.Add(PacketId.NPC_FOURTH_CLICK, temp); //Fourth click
            handlers.Add(PacketId.NPC_FIFTH_CLICK, temp); //Fifth click
            handlers.Add(PacketId.NPC_EXAMINE, temp); //Examine Npc
            handlers.Add(PacketId.MAGIC_ON_NPC, temp); //Magic on NPC
            handlers.Add(PacketId.ITEM_ON_NPC, temp); //Item on NPC

            temp = new ObjectInteract();
            handlers.Add(PacketId.OBJECT_FIRST_CLICK, temp); //First click
            handlers.Add(PacketId.OBJECT_SECOND_CLICK, temp); //Second click
            handlers.Add(PacketId.OBJECT_THIRD_CLICK, temp); //Third click
            handlers.Add(PacketId.OBJECT_FOURTH_CLICK, temp); //Fourth click
            handlers.Add(PacketId.OBJECT_EXAMINE, temp); //object examine

            temp = new ItemInteract();
            handlers.Add(PacketId.EQUIP, temp); //Equip item
            handlers.Add(PacketId.ITEM_ON_ITEM, temp); //Item used on item
            handlers.Add(PacketId.INV_CLICK, temp); //Inventory click item
            handlers.Add(PacketId.ITEM_ON_OBJECT, temp); //Item used on object
            handlers.Add(PacketId.ITEM_ON_GROUND_ITEM, temp); //Item used on ground item.
            handlers.Add(PacketId.INV_OPERATE, temp); //Operate option
            handlers.Add(PacketId.INV_DROP, temp); //Drop item
            handlers.Add(PacketId.PICKUP, temp); //Pick up item
            handlers.Add(PacketId.INV_SWAP_SLOT, temp); //Swap item slot 1
            handlers.Add(PacketId.INV_SWAP_SLOT2, temp); //Swap item slot 2
            handlers.Add(PacketId.INV_RIGHT_CLICK_OPTION1, temp); //Right click option 1
            handlers.Add(PacketId.INV_RIGHT_CLICK_OPTION2, temp); //Right click option 2
            handlers.Add(PacketId.INV_EXAMINE_ITEM, temp); //Inventory item examine

            temp = new InterfaceOptions();
            handlers.Add(PacketId.ENTER_AMOUNT, temp); //Enter amount
            handlers.Add(PacketId.ENTER_TEXT, temp); // Enter text
            handlers.Add(PacketId.INTERFACE_CLICK_1, temp); //Option 1
            handlers.Add(PacketId.INTERFACE_CLICK_2, temp); //Option 2
            handlers.Add(PacketId.INTERFACE_CLICK_3, temp); //Option 3
            handlers.Add(PacketId.INTERFACE_CLICK_4, temp); //Option 4
            handlers.Add(PacketId.INTERFACE_CLICK_5, temp); //Option 5
            handlers.Add(PacketId.INTERFACE_CLICK_6, temp); //Option 6
            handlers.Add(PacketId.INTERFACE_CLICK_7, temp); //Option 7
            handlers.Add(PacketId.INTERFACE_CLICK_8, temp); //Option 8
            handlers.Add(PacketId.INTERFACE_CLICK_9, temp); //Option 9
            handlers.Add(PacketId.INTERFACE_CLICK_10, temp); //Option 9
            handlers.Add(PacketId.GE_SEARCH, temp); //Grand exchange search

            temp = new Unused();
            handlers.Add(PacketId.REIGON_LOADING, temp); //Region loading, varied size
            handlers.Add(PacketId.PACKET_COUNT, temp); //Sent after any click which increases 'count' in getPackets
            handlers.Add(PacketId.CLIENT_VARIABLE_30, temp); //Sent if a client variable is 30..unknown
            handlers.Add(PacketId.DODGY_PACKET, temp); //Caused by a dodgy packet

            Console.WriteLine("Binded packetHandlers");
        }

        /**
         * Handles a packet.
         * @param session
         * @param p
         */

        public static void handlePacket(Player player, Packet p)
        {
            if (p.getSize() != Packet.Size.Bare)
            {
                PacketHandler handler = null;
                if (handlers.TryGetValue(p.getPacketId(), out handler))
                    handler.handlePacket(player, p);
                else
                    Misc.WriteError("Unhandled packet: " + p + ".");
                //if(p.getId() != 93 && p.getId() != 75 && p.getId() != 22)
                //Console.WriteLine(p.ToString());
            }
        }

        /**
          * Set all the packet sizes.
          * <p>Sets every packet size, -3 stands for a packet that doesn't exist or
          * is undocumented. -1 tells the server the size is going to be sent from
          * the client.
          */

        public void setPacketSizes()
        {
            /*
             * If the packet size is -3 it is either undocumented or unused id.
             */
            packetSizes[0] = 0;
            packetSizes[1] = -3;
            packetSizes[2] = -3;
            packetSizes[3] = 2; // Attack NPC
            packetSizes[4] = -3;
            packetSizes[5] = -3;
            packetSizes[6] = -3;
            packetSizes[7] = -3;
            packetSizes[8] = -3;
            packetSizes[9] = -3;
            packetSizes[10] = 4; // Actionbuttons #2
            packetSizes[11] = -3;
            packetSizes[12] = -3;
            packetSizes[13] = -3;
            packetSizes[14] = -3;
            packetSizes[15] = -3;
            packetSizes[16] = -3;
            packetSizes[17] = -3;
            packetSizes[18] = -3;
            packetSizes[19] = -3;
            packetSizes[20] = -3;
            packetSizes[21] = 4; // Camera
            packetSizes[22] = 1; // Focus
            packetSizes[23] = 4; // Enter amount
            packetSizes[24] = -3;
            packetSizes[25] = -3;
            packetSizes[26] = -3;
            packetSizes[27] = 16; // Item on item
            packetSizes[28] = -3;
            packetSizes[29] = -3;
            packetSizes[30] = 2; // Fourth click NPC (trade slayermaster).
            packetSizes[31] = -3;
            packetSizes[32] = -3;
            packetSizes[33] = -3;
            packetSizes[34] = 8; // Add ignore
            packetSizes[35] = -3;
            packetSizes[36] = -3;
            packetSizes[37] = -3;
            packetSizes[38] = -3;
            packetSizes[39] = -1; // Walk
            packetSizes[40] = -3;
            packetSizes[41] = -3;
            packetSizes[42] = -3;
            packetSizes[43] = -3;
            packetSizes[44] = -1; // Command
            packetSizes[45] = -3;
            packetSizes[46] = -3;
            packetSizes[47] = -3;
            packetSizes[48] = -3;
            packetSizes[49] = -3;
            packetSizes[50] = -3;
            packetSizes[51] = -3;
            packetSizes[52] = -3;
            packetSizes[53] = 6; // Interface option #9
            packetSizes[54] = -3;
            packetSizes[55] = 8; // Equip item
            packetSizes[56] = -3;
            packetSizes[57] = 8; // Delete friend
            packetSizes[58] = -3;
            packetSizes[59] = -3;
            packetSizes[60] = -3;
            packetSizes[61] = -3;
            packetSizes[62] = -3;
            packetSizes[63] = -3;
            packetSizes[64] = 6; // Interface option #8
            packetSizes[65] = -3;
            packetSizes[66] = 6;  // Pick up item
            packetSizes[67] = -3;
            packetSizes[68] = 2; // Attack player
            packetSizes[69] = -3;
            packetSizes[70] = -3;
            packetSizes[71] = 2; // Follow Player
            packetSizes[72] = 2; // Examine Npc
            packetSizes[73] = -3;
            packetSizes[74] = -3;
            packetSizes[75] = 6; // Mouse click
            packetSizes[76] = -3;
            packetSizes[77] = -1; // Walk
            packetSizes[78] = 2; // Second click NPC
            packetSizes[79] = 12; // Swapping inventory places in shop, bank and duel
            packetSizes[80] = -3;
            packetSizes[81] = 8; // Unequip item
            packetSizes[82] = -3;
            packetSizes[83] = -3;
            packetSizes[84] = 6; // Object third click
            packetSizes[85] = -3;
            packetSizes[86] = -3;
            packetSizes[87] = -3;
            packetSizes[88] = -3;
            packetSizes[89] = -3;
            packetSizes[90] = -3;
            packetSizes[91] = -3;
            packetSizes[92] = 2; // Inventory item examine.
            packetSizes[93] = 0; // Ping
            packetSizes[94] = 2; //Examine object.
            packetSizes[95] = -3;
            packetSizes[96] = -3;
            packetSizes[97] = -3;
            packetSizes[98] = 4; // Toggle sound setting
            packetSizes[99] = -3;
            packetSizes[100] = -3;
            packetSizes[101] = 14; //use item on ground item
            packetSizes[102] = -3;
            packetSizes[103] = -3;
            packetSizes[104] = 8; // Join clan chat
            packetSizes[105] = -3;
            packetSizes[106] = 2; // Follow player
            packetSizes[107] = -3;
            packetSizes[108] = -3;
            packetSizes[109] = -3;
            packetSizes[110] = 0; // Region loading, size varies
            packetSizes[111] = 2; // Grand Exchange item search
            packetSizes[112] = -3;
            packetSizes[113] = -3;
            packetSizes[114] = -3;
            packetSizes[115] = 10; // Use item on npc
            packetSizes[116] = -3;
            packetSizes[117] = -3;
            packetSizes[118] = -3;
            packetSizes[119] = -3;
            packetSizes[120] = 8; // Add friend
            packetSizes[121] = -3;
            packetSizes[122] = -3;
            packetSizes[123] = -3;
            packetSizes[124] = 6; //Interface option #3
            packetSizes[125] = -3;
            packetSizes[126] = -3;
            packetSizes[127] = -3;
            packetSizes[128] = -3;
            packetSizes[129] = -3;
            packetSizes[130] = -3;
            packetSizes[131] = -3;
            packetSizes[132] = 6; // Actionbuttons #3
            packetSizes[133] = -3; //4th player option .. (not sure)
            packetSizes[134] = 14; // Item on object
            packetSizes[135] = 8; // Drop item
            packetSizes[136] = -3;
            packetSizes[137] = 7; // Unknown, nothing major
            packetSizes[138] = -3;
            packetSizes[139] = -3;
            packetSizes[140] = -3;
            packetSizes[141] = -3;
            packetSizes[142] = -3;
            packetSizes[143] = -3;
            packetSizes[144] = -3;
            packetSizes[145] = -3;
            packetSizes[146] = -3;
            packetSizes[147] = -3;
            packetSizes[148] = 2; // Third click NPC
            packetSizes[149] = -3;
            packetSizes[150] = -3;
            packetSizes[151] = -3;
            packetSizes[152] = -3;
            packetSizes[153] = 8; // Inventory click item #2 (check RC pouch)
            packetSizes[154] = -3;
            packetSizes[155] = 6; // Actionbutton
            packetSizes[156] = 8; // Inventory click item (food etc)
            packetSizes[157] = 10; // Privacy options
            packetSizes[158] = -3;
            packetSizes[159] = -3;
            packetSizes[160] = -3;
            packetSizes[161] = 8; // Item right click option #1 (rub/empty)
            packetSizes[162] = 8; // Clan chat kick
            packetSizes[163] = -3;
            packetSizes[164] = -3;
            packetSizes[165] = -3;
            packetSizes[166] = 6; // Interface option #7
            packetSizes[167] = -3;
            packetSizes[168] = 6; // Interface option #6
            packetSizes[169] = -3;
            packetSizes[170] = -3;
            packetSizes[171] = -3;
            packetSizes[172] = -3;
            packetSizes[173] = -3;
            packetSizes[174] = -3;
            packetSizes[175] = -3;
            packetSizes[176] = -3;
            packetSizes[177] = 2; // Junk, no real purpose
            packetSizes[178] = -3;
            packetSizes[179] = -3;
            packetSizes[180] = 2; // Trade player.
            packetSizes[181] = -3;
            packetSizes[182] = -3;
            packetSizes[183] = -3;
            packetSizes[184] = 7; // Close interface
            packetSizes[185] = -3;
            packetSizes[186] = -3;
            packetSizes[187] = -3;
            packetSizes[188] = 9; // Clan ranks
            packetSizes[189] = -3;
            packetSizes[190] = -3;
            packetSizes[191] = -3;
            packetSizes[192] = -3;
            packetSizes[193] = -3;
            packetSizes[194] = 6; // Object second click
            packetSizes[195] = 8; // Magic on player
            packetSizes[196] = 6; // Interface option #2
            packetSizes[197] = -3;
            packetSizes[198] = -3;
            packetSizes[199] = 6; //Interface option #4
            packetSizes[200] = -3;
            packetSizes[201] = -1; // Send PM
            packetSizes[202] = -3;
            packetSizes[203] = -3;
            packetSizes[204] = -3;
            packetSizes[205] = -3;
            packetSizes[206] = 8; // Operate item
            packetSizes[207] = -3;
            packetSizes[208] = -3;
            packetSizes[209] = -3;
            packetSizes[210] = -3;
            packetSizes[211] = -3;
            packetSizes[212] = -3;
            packetSizes[213] = 8; // Delete ignore
            packetSizes[214] = -3;
            packetSizes[215] = -1; // Walk
            packetSizes[216] = -3;
            packetSizes[217] = -3;
            packetSizes[218] = 2; // Fifth click NPC
            packetSizes[219] = -3;
            packetSizes[220] = -3;
            packetSizes[221] = -3;
            packetSizes[222] = -3;
            packetSizes[223] = -3;
            packetSizes[224] = -3;
            packetSizes[225] = -3;
            packetSizes[226] = -3;
            packetSizes[227] = -3;
            packetSizes[228] = -3;
            packetSizes[229] = -3;
            packetSizes[230] = -3;
            packetSizes[231] = 9; // Swap item slot
            packetSizes[232] = -3;
            packetSizes[233] = -3;
            packetSizes[234] = 6; //Interface option #5
            packetSizes[235] = -3;
            packetSizes[236] = -3;
            packetSizes[237] = -1; // Public chat
            packetSizes[238] = -3;
            packetSizes[239] = 8; // Magic on NPC
            packetSizes[240] = -3;
            packetSizes[241] = -3;
            packetSizes[242] = -3;
            packetSizes[243] = 6; // Screen type (fullscreen, small HD etc)
            packetSizes[244] = 8; // Enter text
            packetSizes[245] = 0; // Idle logout
            packetSizes[246] = -3;
            packetSizes[247] = 6; // Object 4th option
            packetSizes[248] = -3;
            packetSizes[249] = -3;
            packetSizes[250] = -3;
            packetSizes[251] = -3;
            packetSizes[252] = -3;
            packetSizes[253] = -3;
            packetSizes[254] = 6; // First click object
            packetSizes[255] = -3;
        }
    }
}