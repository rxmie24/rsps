using RS2.Server.definitions;
using RS2.Server.player;
using RS2.Server.util;
using System;
using System.Collections.Generic; //List/Queue
using System.Net.Sockets;

namespace RS2.Server.net
{
    internal class Connection
    {
        public byte[] buffer;
        private object chuckedPackets_lock = new object(); //for thread safety
        public List<Byte> chuckedRawPackets;
        public System.Net.Sockets.Socket socket;
        private Queue<Packet> queuedPackets;
        public byte loginStage = 0;
        private int pingCount = 0;
        private LoginDetails loginDetails;
        private Player player;

        //public Cryption inStreamDecryption = null, outStreamDecryption = null;

        public Connection()
        {
            chuckedRawPackets = new List<Byte>();
            queuedPackets = new Queue<Packet>();
        }

        public void setLoginDetails(LoginDetails loginDetails)
        {
            this.loginDetails = loginDetails;
        }

        public LoginDetails getLoginDetails()
        {
            return loginDetails;
        }

        public void setPlayer(Player player)
        {
            this.player = player;
        }

        public Player getPlayer()
        {
            return player;
        }

        public void setPinged()
        {
            pingCount++;
        }

        public int getPingCount()
        {
            return pingCount;
        }

        public void resetPingCount()
        {
            pingCount = 0;
        }

        public void appendChuckedPackets(int amountOfBytes)
        {
            lock (chuckedPackets_lock)
            {
                //TODO: Fix later, not very efficent why clone the data if it exists in buffer >.>
                byte[] chuckedPacket = new byte[amountOfBytes];
                Buffer.BlockCopy(buffer, 0, chuckedPacket, 0, amountOfBytes);
                chuckedRawPackets.AddRange(chuckedPacket);
            }
        }

        public void processQueuedPackets(Object threadContext)
        {
            lock (queuedPackets)
            {
                if (player == null) //no player why even bother?.
                    return;
                if (queuedPackets.Count == 0)
                    return;
                try //Exception occurs when it hits zero elements.
                {
                    Packet p = null;
                    while (queuedPackets.Count > 0)
                    {
                        p = queuedPackets.Dequeue();
                        if (p != null && player != null)
                            PacketHandlers.handlePacket(player, p);
                    }
                }
                catch (InvalidOperationException) { }
            }
        }

        public void addPacketToQueue(Packet p)
        {
            lock (queuedPackets)
            {
                queuedPackets.Enqueue(p);
            }
        }

        public void packetDecoder()
        {
            if (chuckedRawPackets.Count >= 1 && player != null)
            {
                int bytesRead = 0;
                //get opcode
                byte id = chuckedRawPackets[0];
                //id -= inStreamDecryption.getNextKey() & 0xff; //ISSAC (Not used.. seems like it was removed).
                bytesRead++;
                int packetSize = PacketHandlers.packetSizes[id];
                if (packetSize == -1)
                {
                    //variable length packet
                    if (chuckedRawPackets.Count >= 2)
                    {
                        packetSize = chuckedRawPackets[1];
                        bytesRead++;
                    }
                    else
                        return;
                }
                else if (packetSize < 0)
                {
                    packetSize = ((chuckedRawPackets.Count - bytesRead) < 0 ? 0 : chuckedRawPackets.Count - bytesRead);
                    Misc.WriteError("Unknown length - id: " + id + ", guessed to be size: " + packetSize + ".");
                }
                if (packetSize > (chuckedRawPackets.Count - bytesRead))
                {
                    /*
                     * Packet hasn't fully arrived yet.
                     * There are less available bytes that the packet's size wants to request.
                     */
                    return;
                }
                Packet p;
                if (packetSize > 0)
                {
                    lock (chuckedPackets_lock) //Multiple PacketDecoders.. RemoeRange and GetRange same time not good, better lock.
                    {
                        byte[] payload = chuckedRawPackets.GetRange(bytesRead, packetSize).ToArray();
                        p = new Packet(player, id, payload);
                    }
                }
                else
                {
                    p = new Packet(player, id, new byte[0]);
                }

                lock (chuckedPackets_lock)
                { //Possible to remove packets, while other packets coming in, so better lock.
                    chuckedRawPackets.RemoveRange(0, packetSize + bytesRead); //+2 for packetSize
                }

                addPacketToQueue(p);
                packetDecoder(); //try again.. maybe more possible packets left.
            }
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                if (socket.Connected)
                {
                    lock (socket)
                    {
                        if (packet == null)
                            return;

                        int dataLength = packet.getLength();
                        //This is where ISSAC encryption would have been applied to each packet, so it won't ever mess up.                        }
                        byte[] buffer = new byte[dataLength + (int)packet.getSize()];
                        buffer[0] = packet.getId();
                        if (packet.getSize() == Packet.Size.VariableByte)
                        {
                            if (dataLength > 255)  //trying to send more data then we can represent with 8 bits!
                                Misc.WriteError("Tried to send packet length " + dataLength + " in 8 bits [pid=" + packet.getId() + "]");
                            buffer[1] = ((byte)dataLength);
                        }
                        else if (packet.getSize() == Packet.Size.VariableShort)
                        {
                            if (dataLength > 65535)	 //trying to send more data then we can represent with 16 bits!
                                Misc.WriteError("Tried to send packet length " + dataLength + " in 16 bits [pid=" + packet.getId() + "]");
                            //requires 3+dataLength bytes (packetId [1] + dataLength Short [2] + dataLength[x]
                            buffer[1] = ((byte)(dataLength >> 8));
                            buffer[2] = ((byte)dataLength);
                        }
                        packet.getData().CopyTo(buffer, (int)packet.getSize());
                        //we use a blocking mode send, no async on the outgoing
                        //since this is primarily a multithreaded application, shouldn't cause problems to send in blocking mode
                        socket.Send(buffer, SocketFlags.None);
                    }
                }
            }
            catch (Exception e)
            {
                Misc.WriteError(e.Message);
            }
        }
    }
}