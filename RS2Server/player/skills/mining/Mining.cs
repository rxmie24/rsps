using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.mining
{
    internal class Mining : MiningData
    {
        public Mining()
        {
        }

        public static void tryMineRock(Player p, ushort rockId, Location rockLocation, int i, bool newMine)
        {
            if (rockId != 2491)
            {
                AreaEvent mineRockAreaEvent = new AreaEvent(p, rockLocation.getX() - 1, rockLocation.getY() - 1, rockLocation.getX() + 1, rockLocation.getY() + 1);
                mineRockAreaEvent.setAction(() =>
                {
                    mineRock(p, rockId, rockLocation, i, newMine);
                });
                Server.registerCoordinateEvent(mineRockAreaEvent);
            }
            else
            {
                AreaEvent mineRuneEssenceAreaEvent = new AreaEvent(p, rockLocation.getX() - 1, rockLocation.getY() - 1, rockLocation.getX() + 5, rockLocation.getY() + 5);
                mineRuneEssenceAreaEvent.setAction(() =>
                {
                    mineRock(p, rockId, rockLocation, i, newMine);
                });
                Server.registerCoordinateEvent(mineRuneEssenceAreaEvent);
            }
        }

        public static void mineRock(Player p, ushort rockId, Location rockLocation, int i, bool newMine)
        {
            if (!newMine && p.getTemporaryAttribute("miningRock") == null)
            {
                return;
            }
            if (newMine)
            {
                if (!Server.getGlobalObjects().objectExists(rockId, rockLocation))
                {
                    //misc.WriteError(p.getUsername() + " tried to mine a non existing rock!");
                    //return;
                }
                Rock newRock = new Rock(i, rockId, rockLocation, ORES[i], ROCK_LEVEL[i], NAME[i], ROCK_XP[i]);
                p.setTemporaryAttribute("miningRock", newRock);
            }
            Rock rockToMine = (Rock)p.getTemporaryAttribute("miningRock");
            bool essRock = rockToMine.getRockIndex() == 0;
            if (!canMine(p, rockToMine, null))
            {
                resetMining(p);
                return;
            }
            if (newMine)
            {
                string s = essRock ? "You begin to mine Essence.." : "You swing your pick at the rock..";
                p.getPackets().sendMessage(s);
            }
            p.getPackets().closeInterfaces();
            p.setLastAnimation(new Animation(getPickaxeAnimation(p)));
            p.setFaceLocation(rockLocation);
            int delay = getMineTime(p, rockToMine.getRockIndex());
            Event mineRockEvent = new Event(delay);
            mineRockEvent.setAction(() =>
            {
                mineRockEvent.stop(); // Stop the event no matter what
                if (p.getTemporaryAttribute("miningRock") == null)
                {
                    return;
                }
                Rock rock = (Rock)p.getTemporaryAttribute("miningRock");
                if (!canMine(p, rockToMine, rock))
                {
                    return;
                }
                if (!essRock)
                {
                    Server.getGlobalObjects().lowerHealth(rock.getRockId(), rock.getRockLocation());
                    if (!Server.getGlobalObjects().originalObjectExists(rock.getRockId(), rock.getRockLocation()))
                    {
                        resetMining(p);
                        stopAllOtherMiners(p, rock);
                        p.setLastAnimation(new Animation(65535));
                        mineRockEvent.stop();
                    }
                }
                bool addGem = (!essRock && Misc.random(getGemChance(p)) == 0) ? true : false;
                if (p.getInventory().addItem(addGem ? randomGem() : rock.getOre()))
                {
                    p.getSkills().addXp(Skills.SKILL.MINING, rock.getXp());
                    if (addGem)
                    {
                        p.getPackets().sendMessage("You manage to mine a sparkling gem!");
                    }
                    else
                    {
                        if (!essRock)
                        {
                            p.getPackets().sendMessage("You manage to mine some " + rock.getName() + ".");
                        }
                    }
                }
                if (rock.isContinueMine())
                {
                    mineRock(p, rock.getRockId(), rock.getRockLocation(), rock.getRockIndex(), false);
                }
            });
            Server.registerEvent(mineRockEvent);
            if (delay >= 9000 && !rockToMine.isContinueMine())
            {
                Event mineMoreRockEvent = new Event(9000);
                mineMoreRockEvent.setAction(() =>
                {
                    mineMoreRockEvent.stop();
                    Rock rock = (Rock)p.getTemporaryAttribute("miningRock");
                    if (!canMine(p, rockToMine, rock))
                    {
                        return;
                    }
                    p.setFaceLocation(rock.getRockLocation());
                    p.setLastAnimation(new Animation(getPickaxeAnimation(p)));
                });
                Server.registerEvent(mineMoreRockEvent);
            }
        }

        private static int getGemChance(Player p)
        {
            for (int i = 0; i < GLORY_AMULETS.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.AMULET) == GLORY_AMULETS[i])
                {
                    return 30;
                }
            }
            return 40;
        }

        public static int getMineTime(Player p, int i)
        {
            int standardTime = ROCK_TIME[i];
            int randomTime = standardTime + Misc.random(standardTime);
            int pickaxeTime = PICKAXE_TIME[getUsedPickaxe(p)];
            int finalTime = randomTime -= pickaxeTime;
            if (finalTime <= 800 || finalTime <= 0)
            {
                finalTime = 800;
            }
            if (i == 0)
            {
                finalTime = standardTime;
            }
            return finalTime;
        }

        public static void prospectRock(Player p, int x, int y, string s)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            AreaEvent prospectRockAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 1, y + 1);
            prospectRockAreaEvent.setAction(() =>
            {
                p.getPackets().sendMessage("You examine the rock for ores...");
                p.setTemporaryAttribute("unmovable", true);
                Event prospectResultEvent = new Event(2000);
                prospectResultEvent.setAction(() =>
                {
                    p.getPackets().sendMessage("This rock contains " + s + ".");
                    p.removeTemporaryAttribute("unmovable");
                    prospectResultEvent.stop();
                });
                Server.registerEvent(prospectResultEvent);
            });
            Server.registerCoordinateEvent(prospectRockAreaEvent);
        }

        private static void stopAllOtherMiners(Player player, Rock rock)
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null && player.getLocation().withinDistance(p.getLocation(), 5) && !p.Equals(player))
                {
                    if (p.getTemporaryAttribute("miningRock") != null)
                    {
                        Rock otherPlayerRock = (Rock)p.getTemporaryAttribute("miningRock");
                        if (otherPlayerRock.getRockLocation().Equals(rock.getRockLocation()))
                        {
                            p.setLastAnimation(new Animation(65535));
                        }
                    }
                }
            }
        }

        private static bool canMine(Player p, Rock rock, Rock rock2)
        {
            if (rock == null || p == null || !Server.getGlobalObjects().originalObjectExists(rock.getRockId(), rock.getRockLocation()))
            {
                return false;
            }
            if (rock.getRockIndex() != 0)
            {
                if (!p.getLocation().withinDistance(rock.getRockLocation(), 2))
                {
                    return false;
                }
            }
            else
            {
                // is rune ess rock
                if (!p.getLocation().inArea(rock.getRockLocation().getX() - 1, rock.getRockLocation().getY() - 1, rock.getRockLocation().getX() + 5, rock.getRockLocation().getY() + 5))
                {
                    return false;
                }
            }
            if (rock2 != null)
            {
                if (!rock.Equals(rock2))
                {
                    return false;
                }
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.MINING) < rock.getLevel())
            {
                p.getPackets().sendMessage("You need a Mining level of " + rock.getLevel() + " to mine that rock.");
                return false;
            }
            if (!hasPickaxe(p))
            {
                p.getPackets().sendMessage("You need a pickaxe to mine a rock!");
                return false;
            }
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendChatboxInterface(210);
                p.getPackets().modifyText("Your inventory is too full to carry any ore.", 210, 1);
                return false;
            }
            return true;
        }

        public static bool hasPickaxe(Player p)
        {
            for (int i = 0; i < PICKAXES.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == PICKAXES[i] || p.getInventory().hasItem(PICKAXES[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static int getPickaxeAnimation(Player p)
        {
            int pickaxe = getUsedPickaxe(p);
            if (pickaxe == -1)
            {
                return 65535;
            }
            return PICKAXE_ANIMS[pickaxe];
        }

        private static int getUsedPickaxe(Player p)
        {
            int index = -1;
            for (int i = 0; i < PICKAXES.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == PICKAXES[i] || p.getInventory().hasItem(PICKAXES[i]))
                {
                    index = i;
                }
            }
            return index;
        }

        public static int randomGem()
        {
            if (Misc.random(5000) == 0)
            {
                return 1631; // Dragonstone
            }
            if (Misc.random(15000) == 0)
            {
                return 6571; // Onyx
            }
            return GEMS[Misc.random(GEMS.Length - 1)];
        }

        public static void displayEmptyRockMessage(Player player, Location rockLocation)
        {
            AreaEvent displayEmptyRockMessageAreaEvent = new AreaEvent(player, rockLocation.getX() - 1, rockLocation.getY() - 1, rockLocation.getX() + 1, rockLocation.getY() + 1);
            displayEmptyRockMessageAreaEvent.setAction(() =>
            {
                player.getPackets().sendMessage("There is currently no ore available from this rock.");
            });
        }

        public static void resetMining(Player p)
        {
            p.removeTemporaryAttribute("miningRock");
        }
    }
}