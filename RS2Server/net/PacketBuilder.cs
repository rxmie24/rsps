using RS2.Server.util;
using System;

namespace RS2.Server.net
{
    internal class PacketBuilder
    {
        /**
	     * Default capacity
	     */
        private static int DEFAULT_SIZE = 32;
        /**
         * The payload buffer
         */
        private byte[] payload;
        /**
         * Current number of bytes used in the buffer
         */
        public int curLength;
        /**
         * ID of the packet
         */
        private byte id;
        /**
         * Current index into the buffer by bits
         */
        public int bitPosition = 0;
        private Packet.Size size = Packet.Size.Fixed;

        /**
         * Bitmasks for <code>addBits()</code>
         */

        private static uint[] bitmasks = {
		    0, 0x1, 0x3, 0x7,
		    0xf, 0x1f, 0x3f, 0x7f,
		    0xff, 0x1ff, 0x3ff, 0x7ff,
		    0xfff, 0x1fff, 0x3fff, 0x7fff,
		    0xffff, 0x1ffff, 0x3ffff, 0x7ffff,
		    0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
		    0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff,
		    0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff,
		    0xffffffff
	    };

        /**
         * Constructs a packet builder with no data and an initial capacity
         * of <code>DEFAULT_SIZE</code>.
         *
         * @see DEFAULT_SIZE
         */

        public PacketBuilder()
            : this(DEFAULT_SIZE) { }

        public byte[] getPayload()
        {
            return payload;
        }

        /**
         * Constructs a packet builder with no data and an initial capacity
         * of <code>capacity</code>.
         *
         * @param capacity The initial capacity of the buffer
         */

        public PacketBuilder(int capacity)
        {
            payload = new byte[capacity];
        }

        public PacketBuilder(byte[] array)
        {
            payload = array;
        }

        /**
         * Ensures that the buffer is at least <code>minimumBytes</code> bytes.
         *
         * @param minimumCapacity The size needed
         */

        private void ensureCapacity(int minimumCapacity)
        {
            if (minimumCapacity >= payload.Length)
                expandCapacity(minimumCapacity);
        }

        /**
         * Expands the buffer to the specified size.
         *
         * @param minimumCapacity The minimum capacity to which to expand
         * @see java.lang.AbstractStringBuilder#expandCapacity(int)
         */

        private void expandCapacity(int minimumCapacity)
        {
            int newCapacity = (payload.Length + 1) * 2;
            if (newCapacity < 0)
            {
                newCapacity = Int32.MaxValue;
            }
            else if (minimumCapacity > newCapacity)
            {
                newCapacity = minimumCapacity;
            }
            byte[] newPayload = new byte[newCapacity];
            try
            {
                while (curLength > payload.Length)
                    curLength--;
                Array.Copy(payload, 0, newPayload, 0, curLength);
            }
            catch (Exception e)
            {
                Misc.WriteError("expandCapacity, msg=" + e.Message);
            }
            payload = newPayload;
        }

        /**
         * Sets this packet as bare. A bare packet will contain only the payload
         * data, rather than having the standard packet header prepended.
         *
         * @param bare Whether this packet is to be sent bare
         */

        public PacketBuilder setSize(Packet.Size size)
        {
            this.size = size;
            return this;
        }

        /**
         * Sets the ID for this packet.
         *
         * @param id The ID of the packet
         */

        public PacketBuilder setId(byte id)
        {
            this.id = id;
            return this;
        }

        public PacketBuilder initBitAccess()
        {
            bitPosition = curLength * 8;
            return this;
        }

        public void setCurLength(int curLength)
        {
            this.curLength = curLength;
        }

        public PacketBuilder finishBitAccess()
        {
            curLength = (bitPosition + 7) / 8;
            return this;
        }

        /**
         * TODO needs a proper description.
         */

        public PacketBuilder addBits(int numBits, int value)
        {
            int bytePos = bitPosition >> 3;
            int bitOffset = 8 - (bitPosition & 7);
            bitPosition += numBits;
            curLength = (bitPosition + 7) / 8;
            ensureCapacity(curLength);
            for (; numBits > bitOffset; bitOffset = 8)
            {
                payload[bytePos] &= (byte)~bitmasks[bitOffset];	 // mask out the desired area
                payload[bytePos++] |= (byte)((value >> (numBits - bitOffset)) & bitmasks[bitOffset]);

                numBits -= bitOffset;
            }
            if (numBits == bitOffset)
            {
                payload[bytePos] &= (byte)~bitmasks[bitOffset];
                payload[bytePos] |= (byte)(value & bitmasks[bitOffset]);
            }
            else
            {
                payload[bytePos] &= (byte)~(bitmasks[numBits] << (bitOffset - numBits));
                payload[bytePos] |= (byte)((value & bitmasks[numBits]) << (bitOffset - numBits));
            }
            return this;
        }

        /**
         * Adds a <code>byte</code> to the data buffer. The size of this
         * packet will grow by one byte.
         *
         * @param val		   The <code>byte</code> value to add
         * @param checkCapacity Whether the buffer capacity should be checked
         * @return A reference to this object
         */

        private PacketBuilder addByte(byte val, bool checkCapacity)
        {
            if (checkCapacity)
                ensureCapacity(curLength + 1);
            payload[curLength++] = (byte)val;
            return this;
        }

        /**
         * Adds a <code>byte</code> to the data buffer. The size of this
         * packet will grow by one byte.
         *
         * @param val The <code>byte</code> value to add
         * @return A reference to this object
         */

        public PacketBuilder addByte(byte val)
        {
            return addByte(val, true);
        }

        public PacketBuilder addByteC(int val)
        {
            return addByte((byte)-val);
        }

        public PacketBuilder addByteA(int i)
        {
            return addByte((byte)(i + 128));
        }

        public PacketBuilder addByteS(int val)
        {
            return addByte((byte)(128 - val));
        }

        /**
         * Adds a <code>short</code> to the data stream. The size of this
         * packet will grow by two bytes.
         *
         * @param val The <code>short</code> value to add
         * @return A reference to this object
         */

        public PacketBuilder addUShort(int val)
        {
            ensureCapacity(curLength + 2);
            addByte((byte)(val >> 8), false);
            addByte((byte)val, false);
            return this;
        }

        public PacketBuilder addLEShort(int val)
        {
            ensureCapacity(curLength + 2);
            addByte((byte)val, false);
            addByte((byte)(val >> 8), false);
            return this;
        }

        public PacketBuilder addUShortA(int i)
        {
            ensureCapacity(curLength + 2);
            addByte((byte)(i + 128), false);
            addByte((byte)(i >> 8), false);
            return this;
        }

        public PacketBuilder addShortA(int i)
        {
            ensureCapacity(curLength + 2);
            addByte((byte)(i >> 8), false);
            addByte((byte)(i + 128), false);
            return this;
        }

        public PacketBuilder setShort(int val, int offset)
        {
            payload[offset++] = (byte)(val >> 8);
            payload[offset++] = (byte)val;
            if (curLength < offset + 2)
                curLength += 2;
            return this;
        }

        /**
         * Adds a <code>3 bytes</code> to the data stream. The size of this
         * packet will grow by 3 bytes.
         *
         * @param val The <code>3 bytes</code> value to add
         * @return A reference to this object
         */

        public PacketBuilder addThreeBytes(int val)
        {
            ensureCapacity(curLength + 3);
            addByte((byte)(val >> 16), false);
            addByte((byte)(val >> 8), false);
            addByte((byte)val, false);
            return this;
        }

        /**
         * Adds a <code>int</code> to the data stream. The size of this
         * packet will grow by four bytes.
         *
         * @param val The <code>int</code> value to add
         * @return A reference to this object
         */

        public PacketBuilder addInt(int val)
        {
            ensureCapacity(curLength + 4);
            addByte((byte)(val >> 24), false);
            addByte((byte)(val >> 16), false);
            addByte((byte)(val >> 8), false);
            addByte((byte)val, false);
            return this;
        }

        public PacketBuilder addInt1(int val)
        {
            ensureCapacity(curLength + 4);
            addByte((byte)(val >> 8), false);
            addByte((byte)val, false);
            addByte((byte)(val >> 24), false);
            addByte((byte)(val >> 16), false);
            return this;
        }

        public PacketBuilder addInt2(int val)
        {
            ensureCapacity(curLength + 4);
            addByte((byte)(val >> 16), false);
            addByte((byte)(val >> 24), false);
            addByte((byte)val, false);
            addByte((byte)(val >> 8), false);
            return this;
        }

        public PacketBuilder addLEInt(int val)
        {
            ensureCapacity(curLength + 4);
            addByte((byte)val, false);
            addByte((byte)(val >> 8), false);
            addByte((byte)(val >> 16), false);
            addByte((byte)(val >> 24), false);
            return this;
        }

        /**
         * Adds a <code>long</code> to the data stream. The size of this
         * packet will grow by eight bytes.
         *
         * @param val The <code>long</code> value to add
         * @return A reference to this object
         */

        public PacketBuilder addLong(long val)
        {
            addInt((int)(val >> 32));
            addInt((int)(val & -1L));
            return this;
        }

        public PacketBuilder addLELong(long val)
        {
            addLEInt((int)(val & -1L));
            addLEInt((int)(val >> 32));
            return this;
        }

        public PacketBuilder addString(string s)
        {
            ensureCapacity(curLength + s.Length + 1);
            System.Text.Encoding.ASCII.GetBytes(s).CopyTo(payload, curLength);
            curLength += s.Length;
            payload[curLength++] = 0;
            return this;
        }

        /**
         * Adds the contents of <code>byte</code> array <code>data</code>
         * to the packet. The size of this packet will grow by the length of
         * the provided array.
         *
         * @param data The bytes to add to this packet
         * @return A reference to this object
         */

        public PacketBuilder addBytes(byte[] data)
        {
            return addBytes(data, 0, data.Length);
        }

        /**
         * Adds the contents of <code>byte</code> array <code>data</code>,
         * starting at index <code>offset</code>. The size of this packet will
         * grow by <code>len</code> bytes.
         *
         * @param data   The bytes to add to this packet
         * @param offset The index of the first byte to append
         * @param len	The number of bytes to append
         * @return A reference to this object
         */

        public PacketBuilder addBytes(byte[] data, int offset, int len)
        {
            int newLength = curLength + len;
            ensureCapacity(newLength);
            Array.Copy(data, offset, payload, curLength, len);
            curLength = newLength;
            return this;
        }

        public PacketBuilder addBytesA(byte[] data, int i, int j)
        {
            for (int k = j; k < j + i; k++)
            {
                addByte((byte)(data[k] - 128));
            }
            return this;
        }

        public void addBytesReverse(byte[] aByte, int i, int j)
        {
            for (int k = (j + i) - 1; k >= j; k--)
                addByte((byte)aByte[k]);
        }

        public int getLength()
        {
            return curLength;
        }

        /**
         * Returns a <code>Packet</code> object for the data contained
         * in this builder.
         *
         * @return A <code>Packet</code> object
         */

        public Packet toPacket()
        {
            byte[] data = new byte[curLength];
            Array.Copy(payload, 0, data, 0, curLength);
            return new Packet(null, id, data, size);
        }
    }
}