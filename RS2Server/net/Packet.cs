using RS2.Server.definitions;
using RS2.Server.player;
using System.Text;

namespace RS2.Server.net
{
    internal class Packet
    {
        public enum Size : int { Bare, Fixed, VariableByte, VariableShort };

        /**
         * The associated IO session
         */
        private Player player;
        /**
         * The ID of the packet
         */
        private byte pID;
        /**
         * The length of the payload
         */
        private int pLength;
        /**
         * The payload
         */
        private byte[] pData;

        /**
         * The current index into the payload buffer for reading
         */
        public int caret = 0;
        /**
         * Whether this packet is without the standard packet header
         */
        private Size size = Size.Fixed;

        public Packet(Player player, byte pID, byte[] pData, Size s)
        {
            this.player = player;
            this.pID = pID;
            this.pData = pData;
            this.pLength = pData.Length;
            this.size = s;
        }

        /**
         * Creates a new packet with the specified parameters. The packet
         * is considered not to be a bare packet.
         *
         * @param session The session to associate with the packet
         * @param pID	 The ID of the packet
         * @param pData   The payload the packet
         */

        public Packet(Player player, byte pID, byte[] pData)
            : this(player, pID, pData, Size.Fixed) { }

        /**
         * Returns the IO session associated with the packet, if any.
         *
         * @return The <code>IoSession</code> object, or <code>null</code>
         *		 if none.
         */

        public Player getSession()
        {
            return player;
        }

        public Size getSize()
        {
            return size;
        }

        /**
         * Returns the packet ID.
         *
         * @return The packet ID
         */

        public byte getId()
        {
            return pID;
        }

        public PacketHandlers.PacketId getPacketId()
        {
            return (PacketHandlers.PacketId)pID;
        }

        /**
         * Returns the length of the payload of this packet.
         *
         * @return The length of the packet's payload
         */

        public int getLength()
        {
            return pLength;
        }

        /**
         * Returns the entire payload data of this packet.
         *
         * @return The payload <code>byte</code> array
         */

        public byte[] getData()
        {
            return pData;
        }

        /**
         * Returns the remaining payload data of this packet.
         *
         * @return The payload <code>byte</code> array
         */

        public byte[] getRemainingData()
        {
            byte[] data = new byte[pLength - caret];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = pData[i + caret];
            }
            caret += data.Length;
            return data;
        }

        /**
         * Reads the next <code>byte</code> from the payload.
         *
         * @return A <code>byte</code>
         */

        public byte readByte()
        { //regular unsigned byte.
            return pData[caret++];
        }

        public int readByteC() //read a unsigned byte as [negivate unsigned byte [0 to -255] strange encryption]
        {
            return (int)-readByte();
        }

        public int readByteS() //read the unsigned byte to signed byte [byte greater then 128 befores negivate]
        {
            return (int)(128 - readByte());
        }

        public int readByteA()  //read the unsigned byte to signed byte [byte greater then 128 befores positive]
        {
            return (int)(readByte() - 128);
        }

        /**
         * Reads the next <code>short</code> from the payload.
         *
         * @return A <code>short</code>
         */

        public ushort readUShort()
        {
            //hiByte << 8 | loByte
            return (ushort)((pData[caret++] << 8) | pData[caret++]);
        }

        public short readLEShort() //read signed short
        {
            int unsignedShort = (pData[caret++] + (pData[caret++] << 8));
            if (unsignedShort > 0x7FFF) //if unsigned short is more then signed short's max.
                unsignedShort -= 0x10000; //then we convert unsigned short to signed short.

            return (short)unsignedShort;
        }

        public ushort readUShortA()
        {
            //[max value=65535] with [127,255], 0xFF's make it impossible to ever be negivate, so best be ushort
            return (ushort)((pData[caret++] - 128 & 0xFF) + ((pData[caret++] & 0xFF) << 8));
        }

        public short readLEShortA()
        {
            int unsignedShortA = ((pData[caret++] - 128 & 0xFF)) + ((pData[caret++] & 0xFF) << 8);
            if (unsignedShortA > 0x7FFF)
                unsignedShortA -= 0x10000;
            return (short)unsignedShortA;
        }

        public ushort readShortA()
        {
            //[max value=65535] with [255,127], 0xFF's make it impossible to ever be negivate, so best be ushort
            return (ushort)(((pData[caret++] & 0xFF) << 8) + (pData[caret++] - 128 & 0xFF));
        }

        public short readLEShortA3()
        {
            int unsignedShortA = ((pData[caret++] & 0xff) << 8) + (pData[caret++] - 128 & 0xff);
            if (unsignedShortA > 0x7FFF)
            {
                unsignedShortA -= 0x10000;
            }
            return (short)unsignedShortA;
        }

        /**
         * Reads the next <code>int</code> from the payload.
         *
         * @return An <code>int</code>
         */

        public int readInt()
        {
            return ((pData[caret++] & 0xff) << 24)
                 | ((pData[caret++] & 0xff) << 16)
                 | ((pData[caret++] & 0xff) << 8)
                 | (pData[caret++] & 0xff);
        }

        /**
         * Reads the next <code>long</code> from the payload.
         *
         * @return A <code>long</code>
         */

        public long readLong()
        {
            return ((long)(pData[caret++] & 0xFF) << 56)
                 | ((long)(pData[caret++] & 0xFF) << 48)
                 | ((long)(pData[caret++] & 0xFF) << 40)
                 | ((long)(pData[caret++] & 0xFF) << 32)
                 | ((long)(pData[caret++] & 0xFF) << 24)
                 | ((long)(pData[caret++] & 0xFF) << 16)
                 | ((long)(pData[caret++] & 0xFF) << 8)
                 | ((byte)(pData[caret++] & 0xFF));
        }

        /**
         * Reads a string of the specified length from the payload.
         *
         * @param length The length of the string to be read
         * @return A <code>String</code>
         */

        public string readString(int length)
        {
            string rv = new string(Encoding.ASCII.GetChars(pData, caret, length));
            caret += length;
            return rv;
        }

        /**
         * Reads the string which is formed by the unread portion
         * of the payload.
         *
         * @return A <code>String</code>
         */

        public string readString()
        {
            return readString(pLength - caret);
        }

        public string readRS2String()
        {
            int start = caret;
            while (pData[caret++] != 0) ;
            return new string(Encoding.ASCII.GetChars(pData, start, caret - start - 1));
        }

        public void readBytes(byte[] buf, int off, int len)
        {
            for (int i = 0; i < len; i++)
                buf[off + i] = pData[caret++];
        }

        /**
         * Skips the specified number of bytes in the payload.
         *
         * @param x The number of bytes to be skipped
         */

        public void skip(int x)
        {
            caret += x;
        }

        public int remaining()
        {
            return pData.Length - caret;
        }

        /**
         * Returns this packet in string form.
         *
         * @return A <code>String</code> representing this packet
         */

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[id=" + pID + ",len=" + pLength + ",data=");
            for (int x = 0; x < pLength; x++)
            {
                sb.Append(byteToHex(pData[x], true) + " ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        private static string byteToHex(byte b, bool forceLeadingZero)
        {
            StringBuilder sb = new StringBuilder();
            if (b / 16 > 0 || forceLeadingZero)
                sb.Append(hex[b / 16]);
            sb.Append(hex[b % 16]);
            return sb.ToString();
        }

        private static char[] hex = "0123456789ABCDEF".ToCharArray();
    }
}