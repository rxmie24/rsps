using RS2.Server.definitions;
using RS2.Server.model;
using RS2.Server.player;
using System;

namespace RS2.Server.net
{
    internal class PlayerUpdate
    {
        /**
         * Prevent instance creation.
         */

        private PlayerUpdate()
        {
        }

        /**
         * Update the specified player.
         * @param p
         */

        public static void update(Player player)
        {
            //Creates a list of new players in area. [This only happens once.. no rebuilding like all runescape 2 servers.]
            player.getLocalEnvironment().updatePlayersInArea();

            //Attempt to skip a pointless update if possible.
            //Any current users on screen got a update for me.
            bool hasAppearanceUpdate = player.getLocalEnvironment().getSeenPlayers().Exists(new Predicate<Player>(delegate(Player p) { return p.getUpdateFlags().hasAnyUpdate(); }));
            //No new pending players and no players pending to get removed from screen.
            bool hasAddRemoveUpdate = (player.getLocalEnvironment().getNewPlayers().Count > 0 || player.getLocalEnvironment().getRemovedPlayers().Count > 0);
            //no updates.. exit.

            if (!hasAppearanceUpdate && !hasAddRemoveUpdate && !player.getUpdateFlags().hasAnyUpdate() && !player.getUpdateFlags().didMapRegionChange() && player.getConnection().getPingCount() < 7) return;

            player.getConnection().resetPingCount();

            if (player.getUpdateFlags().didMapRegionChange())
                player.getPackets().sendMapRegion();

            PacketBuilder mainPacket = new PacketBuilder().setId(225).setSize(Packet.Size.VariableShort).initBitAccess();
            PacketBuilder updateBlock = new PacketBuilder().setSize(Packet.Size.Bare);

            if (player.getUpdateFlags().isTeleporting())
            { //teleport
                mainPacket.addBits(1, 1);
                mainPacket.addBits(2, 3);
                mainPacket.addBits(7, player.getLocation().getLocalY(player.getUpdateFlags().getLastRegion())); //currentX
                mainPacket.addBits(1, 1);
                mainPacket.addBits(2, player.getLocation().getZ()); //heightLevel
                mainPacket.addBits(1, player.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                mainPacket.addBits(7, player.getLocation().getLocalX(player.getUpdateFlags().getLastRegion())); //currentY
            }
            else
            {
                if (player.getSprites().getPrimarySprite() == -1)
                { //no movement
                    mainPacket.addBits(1, player.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                    if (player.getUpdateFlags().isUpdateRequired())
                        mainPacket.addBits(2, 0);
                }
                else
                { //movement.
                    mainPacket.addBits(1, 1);
                    if (player.getSprites().getSecondarySprite() == -1)
                    { //not running
                        mainPacket.addBits(2, 1);
                        mainPacket.addBits(3, player.getSprites().getPrimarySprite()); //walk
                        mainPacket.addBits(1, player.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                    }
                    else
                    {
                        mainPacket.addBits(2, 2);
                        mainPacket.addBits(1, 1);
                        mainPacket.addBits(3, player.getSprites().getPrimarySprite()); //walk
                        mainPacket.addBits(3, player.getSprites().getSecondarySprite()); //run
                        mainPacket.addBits(1, player.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                    }
                }
            }
            if (player.getUpdateFlags().isUpdateRequired())
                appendUpdateBlock(player, updateBlock, false); //update my own updates.

            mainPacket.addBits(8, player.getLocalEnvironment().getSeenPlayers().Count); //All players I've seen already (not new players)

            //Send information of all the players in our own location.
            foreach (Player p in player.getLocalEnvironment().getSeenPlayers())
            {
                if (player.getLocalEnvironment().getRemovedPlayers().Contains(p))
                {
                    mainPacket.addBits(1, 1); //update required.
                    mainPacket.addBits(2, 3); //delete player.
                    continue;
                }
                else if (p.getSprites().getPrimarySprite() == -1)
                {
                    if (p.getUpdateFlags().isUpdateRequired())
                    {
                        mainPacket.addBits(1, 1); //update required.
                        mainPacket.addBits(2, 0); //finish
                    }
                    else
                    {
                        mainPacket.addBits(1, 0); //no update required, either region changed or no movement change.
                    }
                }
                else if (p.getSprites().getPrimarySprite() != -1 && p.getSprites().getSecondarySprite() == -1)
                {
                    mainPacket.addBits(1, 1); //update required.
                    mainPacket.addBits(2, 1); //update just walk direction sprite
                    mainPacket.addBits(3, p.getSprites().getPrimarySprite());
                    mainPacket.addBits(1, p.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                }
                else if (p.getSprites().getPrimarySprite() != -1 && p.getSprites().getSecondarySprite() != -1)
                { //Bit 2 = 2, updates both sprites.
                    mainPacket.addBits(1, 1); //update required.
                    mainPacket.addBits(2, 2); //update both walk & run sprites.
                    mainPacket.addBits(1, 1);
                    mainPacket.addBits(3, p.getSprites().getPrimarySprite());
                    mainPacket.addBits(3, p.getSprites().getSecondarySprite());
                    mainPacket.addBits(1, p.getUpdateFlags().isUpdateRequired() ? 1 : 0);
                }
                if (p.getUpdateFlags().isUpdateRequired())
                    appendUpdateBlock(p, updateBlock, false);
            }

            //Send information of all the new players in our own location.
            foreach (Player p in player.getLocalEnvironment().getNewPlayers())
            {
                mainPacket.addBits(11, p.getIndex()); //playerId of new player.

                int yPos = p.getLocation().getY() - player.getLocation().getY();
                int xPos = p.getLocation().getX() - player.getLocation().getX();

                mainPacket.addBits(1, 1);
                mainPacket.addBits(5, xPos < 0 ? xPos + 32 : xPos);
                mainPacket.addBits(3, p.getWalkingQueue().getLastDirection());
                mainPacket.addBits(1, 1);
                mainPacket.addBits(5, yPos < 0 ? yPos + 32 : yPos);
                appendUpdateBlock(p, updateBlock, true); //force appearance update.
            }

            /**
             * Done with with all our updates.. fine to refine our environment lists.
             * Remove players who either moved away from our location or plain old disconnected.
             * Mix new players with old players into one playerlist.
             * Clear new players list, for more new players again
             */
            player.getLocalEnvironment().organizePlayers();

            if (updateBlock.getLength() > 0)
                mainPacket.addBits(11, 2047); //2047 max players in server,area.
            mainPacket.finishBitAccess();
            if (updateBlock.getLength() > 0)
                mainPacket.addBytes(updateBlock.toPacket().getData());
            if (player.getConnection() != null)
                player.getConnection().SendPacket(mainPacket.toPacket());
        }

        private static void appendUpdateBlock(Player p, PacketBuilder updateBlock, bool forceAppearance)
        {
            int mask = 0x0;

            AppearanceUpdateFlags flags = p.getUpdateFlags();
            if (flags.isChatTextUpdateRequired())
            {
                mask |= 0x80;
            }
            if (flags.isHitUpdateRequired())
            {
                mask |= 0x1;
            }
            if (flags.isEntityFocusUpdateRequired())
            {
                mask |= 0x2;
            }
            if (flags.isAppearanceUpdateRequired() || forceAppearance)
            {
                mask |= 0x4;
            }
            if (flags.isAnimationUpdateRequired())
            {
                mask |= 0x8;
            }
            if (flags.isForceTextUpdateRequired())
            {
                mask |= 0x20;
            }
            if (flags.isFaceLocationUpdateRequired())
            {
                mask |= 0x40;
            }
            if (flags.isGraphicsUpdateRequired())
            {
                mask |= 0x100;
            }
            if (flags.isHit2UpdateRequired())
            {
                mask |= 0x200;
            }
            if (flags.isForceMovementRequired())
            {
                mask |= 0x400; //mask |= 0x800;
            }

            if (mask >= 0x100) //0x100=256 [full byte], so use two bytes.
            {
                mask |= 0x10;
                updateBlock.addLEShort(mask);

                //updateBlock.addByte((byte)(mask & 0xFF));
                //updateBlock.addByte((byte)(mask >> 8));
            }
            else
            {
                updateBlock.addByte((byte)(mask & 0xFF));
            }

            if (flags.isChatTextUpdateRequired())
            {
                appendChatTextUpdate(p, updateBlock);
            }
            if (flags.isHitUpdateRequired())
            {
                appendHitUpdate(p, updateBlock);
            }
            if (flags.isAnimationUpdateRequired())
            {
                appendAnimationUpdate(p, updateBlock);
            }
            if (flags.isAppearanceUpdateRequired() || forceAppearance)
            {
                appendAppearanceUpdate(p, updateBlock);
            }
            if (flags.isEntityFocusUpdateRequired())
            {
                appendFaceEntityUpdate(p, updateBlock);
            }
            if (flags.isForceMovementRequired())
            {
                appendForceMovement(p, updateBlock);
            }
            if (flags.isForceTextUpdateRequired())
            {
                appendForceTextUpdate(p, updateBlock);
            }
            if (flags.isHit2UpdateRequired())
            {
                appendHit2Update(p, updateBlock);
            }
            if (flags.isGraphicsUpdateRequired())
            {
                appendGraphicsUpdate(p, updateBlock);
            }
            if (flags.isFaceLocationUpdateRequired())
            {
                appendFaceLocationUpdate(p, updateBlock);
            }
        }

        private static void appendForceMovement(Player p, PacketBuilder updateBlock)
        {
            Location myLocation = p.getUpdateFlags().getLastRegion();
            ForceMovement fm = p.getForceMovement();
            updateBlock.addByteC(fm.getX1());
            updateBlock.addByte((byte)(fm.getY1()));
            updateBlock.addByteA(fm.getX2());
            updateBlock.addByte((byte)fm.getY2());
            updateBlock.addLEShort(fm.getSpeed1());
            updateBlock.addLEShort(fm.getSpeed2());
            updateBlock.addByteC(fm.getDirection());
        }

        private static void appendUnknownMask(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addByteC(1);
            updateBlock.addLEShort(65465);
            updateBlock.addByteA(21);
            updateBlock.addUShort(434454);
        }

        private static void appendFaceLocationUpdate(Player p, PacketBuilder updateBlock)
        {
            int x = p.getFaceLocation().getX();
            int y = p.getFaceLocation().getY();
            updateBlock.addUShort(x = 2 * x + 1);
            updateBlock.addUShortA(y = 2 * y + 1);
        }

        private static void appendForceTextUpdate(Player p, PacketBuilder updateBlock)
        {
            object forceText = p.getTemporaryAttribute("forceText");
            if (forceText != null)
            {
                updateBlock.addString((string)forceText);
            }
        }

        private static void appendFaceEntityUpdate(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addShortA(p.getEntityFocus());
        }

        private static void appendHit2Update(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addByte((byte)p.getHits().getHitDamage2());
            updateBlock.addByteS((byte)p.getHits().getHitType2());
        }

        private static void appendHitUpdate(Player p, PacketBuilder updateBlock)
        {
            int ratio = p.getSkills().getCurLevel(Skills.SKILL.HITPOINTS) * 255 / p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS);
            if (p.getSkills().getCurLevel(Skills.SKILL.HITPOINTS) > p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS))
            {
                ratio = p.getSkills().getMaxLevel(3) * 255 / p.getSkills().getCurLevel(Skills.SKILL.HITPOINTS);
            }
            if (p.getHits().getHitDamage1() < 128) //damage can be either addByte [for damage less then 128 otherwise UShort]
                updateBlock.addByte((byte)p.getHits().getHitDamage1());
            else
                updateBlock.addUShort(p.getHits().getHitDamage1() + 0x8000);
            updateBlock.addByteA((byte)p.getHits().getHitType1());
            updateBlock.addByteS(ratio);
        }

        private static void appendGraphicsUpdate(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addLEShort(p.getLastGraphics().getId());
            updateBlock.addInt2(p.getLastGraphics().getHeight() << 16 + p.getLastGraphics().getDelay());
        }

        private static void appendAnimationUpdate(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addUShort(p.getLastAnimation().getId());
            updateBlock.addByte((byte)p.getLastAnimation().getDelay());
        }

        private static void appendChatTextUpdate(Player p, PacketBuilder updateBlock)
        {
            updateBlock.addLEShort((p.getLastChatMessage().getColour() << 8) + p.getLastChatMessage().getEffect());
            updateBlock.addByte((byte)p.getRights());
            byte[] chatStr = p.getLastChatMessage().getPacked();
            updateBlock.addByte((byte)(chatStr.Length));
            updateBlock.addBytesReverse(chatStr, chatStr.Length, 0);
            //}
        }

        private static void appendAppearanceUpdate(Player p, PacketBuilder updateBlock)
        {
            PacketBuilder playerProps = new PacketBuilder().setSize(Packet.Size.Bare);

            Appearance app = p.getAppearance();
            playerProps.addByte((byte)(app.getGender() & 0xFF));
            if ((app.getGender() & 0x2) == 2)
            {
                playerProps.addByte((byte)0);
                playerProps.addByte((byte)0);
            }
            playerProps.addByte((byte)p.getPrayers().getPkIcon());
            playerProps.addByte((byte)p.getPrayers().getHeadIcon());
            if (!app.isInvisible())
            {
                if (!app.isNpc())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (p.getEquipment().getItemInSlot((ItemData.EQUIP)i) != -1)
                        {
                            playerProps.addUShort(32768 + p.getEquipment().getSlot((ItemData.EQUIP)i).getDefinition().getEquipId());
                        }
                        else
                        {
                            playerProps.addByte((byte)0);
                        }
                    }
                    if (p.getEquipment().getItemInSlot(ItemData.EQUIP.CHEST) != -1)
                    {
                        playerProps.addUShort(32768 + p.getEquipment().getSlot(ItemData.EQUIP.CHEST).getDefinition().getEquipId());
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.AMULET));
                    }
                    if (p.getEquipment().getItemInSlot(ItemData.EQUIP.SHIELD) != -1)
                    {
                        playerProps.addUShort(32768 + p.getEquipment().getSlot(ItemData.EQUIP.SHIELD).getDefinition().getEquipId());
                    }
                    else
                    {
                        playerProps.addByte((byte)0);
                    }
                    Item chest = p.getEquipment().getSlot(ItemData.EQUIP.CHEST);
                    if (chest != null && chest.getDefinition() != null)
                    {
                        if (!ItemData.isFullBody(chest.getDefinition()))
                        {
                            playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.WEAPON));
                        }
                        else
                        {
                            playerProps.addByte((byte)0);
                        }
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.WEAPON));
                    }
                    if (p.getEquipment().getItemInSlot(ItemData.EQUIP.LEGS) != -1)
                    {
                        playerProps.addUShort(32768 + p.getEquipment().getSlot(ItemData.EQUIP.LEGS).getDefinition().getEquipId());
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.SHIELD));
                    }
                    Item hat = p.getEquipment().getSlot(ItemData.EQUIP.HAT);
                    if (hat != null && hat.getDefinition() != null)
                    {
                        if (!ItemData.isFullHat(hat.getDefinition()) && !ItemData.isFullMask(hat.getDefinition()))
                        {
                            playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.HAT));
                        }
                        else
                        {
                            playerProps.addByte((byte)0);
                        }
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.HAT));
                    }
                    if (p.getEquipment().getItemInSlot(ItemData.EQUIP.HANDS) != -1)
                    {
                        playerProps.addUShort(32768 + p.getEquipment().getSlot(ItemData.EQUIP.HANDS).getDefinition().getEquipId());
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.CHEST));
                    }
                    if (p.getEquipment().getItemInSlot(ItemData.EQUIP.FEET) != -1)
                    {
                        playerProps.addUShort(32768 + p.getEquipment().getSlot(ItemData.EQUIP.FEET).getDefinition().getEquipId());
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(6));
                    }
                    if (hat != null && hat.getDefinition() != null)
                    {
                        if (!ItemData.isFullMask(hat.getDefinition()))
                        {
                            playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.CAPE));
                        }
                        else
                        {
                            playerProps.addByte((byte)0);
                        }
                    }
                    else
                    {
                        playerProps.addUShort(0x100 + app.getLook(ItemData.EQUIP.CAPE));
                    }
                }
                else
                {
                    playerProps.addUShort(-1);
                    playerProps.addUShort(app.getNpcId());
                    playerProps.addByte((byte)255);
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    playerProps.addByte((byte)0);
                }
            }
            foreach (int colour in app.getColoursArray())
            {
                playerProps.addByte((byte)colour);
            }
            playerProps.addUShort(p.getEquipment().getStandWalkAnimation());
            playerProps.addLong(p.getLoginDetails().getLongName());
            playerProps.addByte((byte)p.getSkills().getCombatLevel());
            playerProps.addUShort(0);
            playerProps.addByte((byte)0);
            updateBlock.addByteA((byte)(playerProps.getLength() & 0xFF));
            updateBlock.addBytes(playerProps.toPacket().getData(), 0, playerProps.getLength());
        }
    }
}