using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;
using System;

namespace RS2.Server.net
{
    internal class NpcUpdate
    {
        /**
         * Prevent instance creation.
         */

        private NpcUpdate()
        {
        }

        public static byte[] XLATE_DIRECTION_TO_CLIENT = new byte[] { 1, 2, 4, 7, 6, 5, 3, 0 };

        /**
         * Update the specified player's Npcs.
         * @param p
         */

        public static void update(Player player)
        {
            //Creates a list of new players in area. [This only happens once.. no rebuilding like all runescape 2 servers.]
            player.getLocalEnvironment().updateNpcsInArea();
            //Attempt to skip a pointless update if possible.
            //Any current npcs on screen got a update for me, if no we just return this whole method.
            bool hasAppearanceUpdate = player.getLocalEnvironment().getSeenNpcs().Exists(new Predicate<Npc>(delegate(Npc n) { return n.getUpdateFlags().isUpdateRequired(); }));
            //No new pending npcs and no npcs pending to get removed from screen.
            bool hasAddRemoveUpdate = (player.getLocalEnvironment().getNewNpcs().Count > 0 || player.getLocalEnvironment().getRemovedNpcs().Count > 0);
            //no updates.. exit.
            if (!hasAppearanceUpdate && !hasAddRemoveUpdate) return;

            PacketBuilder mainPacket = new PacketBuilder().setId(32).setSize(Packet.Size.VariableShort).initBitAccess();
            PacketBuilder updateBlock = new PacketBuilder().setSize(Packet.Size.Bare);
            mainPacket.addBits(8, player.getLocalEnvironment().getSeenNpcs().Count);

            //Send information of all the npcs in our own location.
            foreach (Npc n in player.getLocalEnvironment().getSeenNpcs())
            {
                if (player.getLocalEnvironment().getRemovedNpcs().Contains(n))
                {
                    mainPacket.addBits(1, 1); //update required.
                    mainPacket.addBits(2, 3); //delete npc.
                    continue;
                }
                else
                {
                    if (n.getSprites().getPrimarySprite() == -1)
                    {
                        if (n.getUpdateFlags().isUpdateRequired())
                        {
                            mainPacket.addBits(1, 1);
                            mainPacket.addBits(2, 0);
                        }
                        else
                        {
                            mainPacket.addBits(1, 0);
                        }
                    }
                    else if (n.getSprites().getSecondarySprite() == -1)
                    {
                        mainPacket.addBits(1, 1);
                        mainPacket.addBits(2, 1);
                        mainPacket.addBits(3, XLATE_DIRECTION_TO_CLIENT[n.getSprites().getPrimarySprite()]);
                        mainPacket.addBits(1, n.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                    }
                    else
                    {
                        mainPacket.addBits(1, 1);
                        mainPacket.addBits(2, 2);
                        mainPacket.addBits(3, n.getSprites().getPrimarySprite());
                        mainPacket.addBits(3, n.getSprites().getSecondarySprite());
                        mainPacket.addBits(1, n.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                    }
                    if (n.getUpdateFlags().isUpdateRequired())
                        appendUpdateBlock(n, updateBlock);
                }
            }

            //Send information of all the new npcs in our own location.
            foreach (Npc n in player.getLocalEnvironment().getNewNpcs())
            {
                int y = n.getLocation().getY() - player.getLocation().getY();
                if (y < 0)
                {
                    y += 32;
                }
                int x = n.getLocation().getX() - player.getLocation().getX();
                if (x < 0)
                {
                    x += 32;
                }
                mainPacket.addBits(15, n.getIndex());
                mainPacket.addBits(1, 1);
                mainPacket.addBits(3, n.getFaceDirection());
                mainPacket.addBits(1, n.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                mainPacket.addBits(5, y < 0 ? y + 32 : y);
                mainPacket.addBits(14, n.getId());
                mainPacket.addBits(5, x < 0 ? x + 32 : x);

                if (n.getUpdateFlags().isUpdateRequired())
                    appendUpdateBlock(n, updateBlock);
            }

            /**
             * Done with with all our updates.. fine to refine our environment lists.
             * Remove npcs who either moved away from our location or hidden / dead etc..
             * Mix new npcs with old npcs into one npclist.
             * Clear new npc list, for more new npcs again
             */
            player.getLocalEnvironment().organizeNpcs();

            if (updateBlock.getLength() >= 3)
            {
                mainPacket.addBits(15, 32767);
            }
            mainPacket.finishBitAccess();
            mainPacket.addBytes(updateBlock.toPacket().getData());
            player.getConnection().SendPacket(mainPacket.toPacket());
        }

        private static void appendUpdateBlock(Npc Npc, PacketBuilder updateBlock)
        {
            int mask = 0x0;
            if (Npc.getUpdateFlags().isHit2UpdateRequired())
            {
                mask |= 0x2;
            }
            if (Npc.getUpdateFlags().isEntityFocusUpdateRequired())
            {
                mask |= 0x4;
            }
            if (Npc.getUpdateFlags().isAnimationUpdateRequired())
            {
                mask |= 0x10;
            }
            if (Npc.getUpdateFlags().isForceTextUpdateRequired())
            {
                mask |= 0x20;
            }
            if (Npc.getUpdateFlags().isHitUpdateRequired())
            {
                mask |= 0x40;
            }
            if (Npc.getUpdateFlags().isGraphicsUpdateRequired())
            {
                mask |= 0x80;
            }
            if (Npc.getUpdateFlags().isFaceLocationUpdateRequired())
            {
                mask |= 0x200;
            }

            if (mask >= 0x100)
            {
                mask |= 0x8;
                updateBlock.addLEShort(mask);

                //updateBlock.addByte((byte)(mask & 0xFF));
                //updateBlock.addByte((byte)(mask >> 8));
            }
            else
            {
                updateBlock.addByte((byte)(mask & 0xFF));
            }

            if (Npc.getUpdateFlags().isHitUpdateRequired())
            {
                appendHitUpdate(Npc, updateBlock);
            }
            if (Npc.getUpdateFlags().isHit2UpdateRequired())
            {
                appendHit2Update(Npc, updateBlock);
            }
            if (Npc.getUpdateFlags().isAnimationUpdateRequired())
            {
                appendAnimationUpdate(Npc, updateBlock);
            }
            if (Npc.getUpdateFlags().isEntityFocusUpdateRequired())
            {
                appendEntityFocusUdate(Npc, updateBlock);
            }
            if (Npc.getUpdateFlags().isGraphicsUpdateRequired())
            {
                appendGraphicsUpdate(Npc, updateBlock);
            }
            //0x1
            if (Npc.getUpdateFlags().isForceTextUpdateRequired())
            {
                appendForceTextUpdate(Npc, updateBlock);
            }
            //0x100
            if (Npc.getUpdateFlags().isFaceLocationUpdateRequired())
            {
                appendFaceLocationUpdate(Npc, updateBlock);
            }
        }

        private static void appendFaceLocationUpdate(Npc Npc, PacketBuilder updateBlock)
        {
            Location loc = Npc.getFaceLocation();
            int x = loc.getX();
            int y = loc.getY();
            updateBlock.addShortA(x = 2 * x + 1);
            updateBlock.addUShort(y = 2 * y + 1);
        }

        private static void appendHitUpdate(Npc Npc, PacketBuilder updateBlock)
        {
            NpcData npcDef = NpcData.forId(Npc.getId());
            int ratio = 1;
            if (npcDef != null)
                ratio = Npc.getHp() * 255 / npcDef.getHitpoints();
            else
                Misc.WriteError("Missing npcDef npcId: " + Npc.getId());
            updateBlock.addByte((byte)Npc.getHits().getHitDamage1());
            updateBlock.addByteC((int)Npc.getHits().getHitType1()); //TODO: <- check
            updateBlock.addByteS((byte)ratio);
        }

        private static void appendAnimationUpdate(Npc Npc, PacketBuilder updateBlock)
        {
            Animation anim = Npc.getLastAnimation();
            if (anim != null)
            {
                updateBlock.addUShort(anim.getId());
                updateBlock.addByte((byte)anim.getDelay());
            }
        }

        private static void appendHit2Update(Npc Npc, PacketBuilder updateBlock)
        {
            updateBlock.addByteC((byte)Npc.getHits().getHitDamage2());
            updateBlock.addByteS((byte)Npc.getHits().getHitType2());
        }

        private static void appendGraphicsUpdate(Npc Npc, PacketBuilder updateBlock)
        {
            Graphics gfx = Npc.getLastGraphics();
            if (gfx != null)
            {
                updateBlock.addShortA(gfx.getId());
                updateBlock.addLEInt(gfx.getHeight() << 16 + gfx.getDelay());
            }
        }

        private static void appendForceTextUpdate(Npc Npc, PacketBuilder updateBlock)
        {
            object forceText = Npc.getTemporaryAttribute("forceText");
            if (forceText != null)
            {
                updateBlock.addString((string)forceText);
            }
        }

        private static void appendEntityFocusUdate(Npc Npc, PacketBuilder updateBlock)
        {
            int entityFocus = Npc.getEntityFocus();
            if (entityFocus != -1)
                updateBlock.addShortA(entityFocus);
        }
    }
}