using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;
using System;

namespace RS2.Server.player.skills.thieving
{
    internal class Thieving : ThievingData
    {
        public Thieving()
        {
        }

        public static bool wantToThieveNpc(Player p, Npc npc)
        {
            for (int i = 0; i < NPCS.Length; i++)
            {
                for (int j = 0; j < NPCS[i].Length; j++)
                {
                    if (npc.getId() == NPCS[i][j])
                    {
                        thieveNpc(p, npc, i);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void thieveNpc(Player p, Npc npc, int index)
        {
            AreaEvent thieveNpcAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
            thieveNpcAreaEvent.setAction(() =>
            {
                if (!canThieveNpc(p, npc, index))
                {
                    return;
                }
                p.setFaceLocation(npc.getLocation());
                p.setLastAnimation(new Animation(881));
                p.getPackets().sendMessage("You attempt to pick the " + NPC_NAMES[index] + " pocket...");
                p.setTemporaryAttribute("lastPickPocket", Environment.TickCount);

                Event thieveNpcEvent = new Event(1000);
                thieveNpcEvent.setAction(() =>
                {
                    thieveNpcEvent.stop();
                    if (!p.getLocation().withinDistance(npc.getLocation(), 2))
                    {
                        return;
                    }
                    if (successfulThieve(p, index, false))
                    {
                        int rewardIndex = Misc.random(NPC_REWARD[index].Length - 1);
                        int reward = NPC_REWARD[index][rewardIndex];
                        int rewardAmount = NPC_REWARD_AMOUNT[index][rewardIndex];
                        if (index == 7)
                        { // Master farmer.
                            if (Misc.random(15) == 0)
                            {
                                reward = HERB_SEEDS[Misc.random(HERB_SEEDS.Length - 1)];
                            }
                        }
                        p.getSkills().addXp(Skills.SKILL.THIEVING, NPC_XP[index]);
                        p.getInventory().addItem(reward, rewardAmount);
                        p.getPackets().sendMessage("You pick the " + NPC_NAMES[index] + " pocket.");
                    }
                    else
                    {
                        p.getWalkingQueue().resetWalkingQueue();
                        p.getPackets().sendMessage("You fail to pick the " + NPC_NAMES[index] + " pocket.");
                        p.getPackets().sendMessage("You've been stunned!");
                        npc.setForceText("What do you think you're doing?");
                        p.setTemporaryAttribute("unmovable", true);
                        p.setTemporaryAttribute("stunned", true);
                        p.setLastGraphics(new Graphics(80, 0, 100));
                        p.setLastAnimation(new Animation(p.getDefenceAnimation()));
                        p.hit(1);
                        npc.setFaceLocation(p.getLocation());
                        Event removeStunEvent = new Event(5000);
                        removeStunEvent.setAction(() =>
                        {
                            removeStunEvent.stop();
                            p.removeTemporaryAttribute("unmovable");
                            p.removeTemporaryAttribute("stunned");
                            p.setLastGraphics(new Graphics(65535));
                        });
                        Server.registerEvent(removeStunEvent);
                    }
                });
                Server.registerEvent(thieveNpcEvent);
            });
            Server.registerCoordinateEvent(thieveNpcAreaEvent);
        }

        private static bool successfulThieve(Player p, int index, bool stall)
        {
            int thieveLevel = p.getSkills().getCurLevel(Skills.SKILL.THIEVING);
            int levelNeeded = stall ? STALL_LVL[index] : NPC_LVL[index];
            int difference = thieveLevel - levelNeeded;
            if ((difference > 6 && index >= 12))
            {
                difference = 6;
            }
            if ((difference > 14 && index < 12))
            {
                difference = 14;
            }
            int chance = difference < 3 ? 1 : (int)(difference * 0.9);
            return Misc.random(chance) != 0;
        }

        private static bool canThieveNpc(Player p, Npc npc, int index)
        {
            if (p == null || npc == null || npc.isDead() || npc.isHidden() || npc.isDestroyed() || p.isDead() || p.isDestroyed())
            {
                return false;
            }
            if (!p.getLocation().withinDistance(npc.getLocation(), 2))
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.THIEVING) < NPC_LVL[index])
            {
                p.getPackets().sendMessage("You need a Thieving level of " + NPC_LVL[index] + " to rob this Npc.");
                p.setFaceLocation(npc.getLocation());
                return false;
            }
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendMessage("You need a free inventory space for any potential loot.");
                return false;
            }
            if (p.getTemporaryAttribute("stunned") != null)
            {
                return false;
            }
            if (p.getTemporaryAttribute("lastPickPocket") != null)
            {
                if (Environment.TickCount - (int)p.getTemporaryAttribute("lastPickPocket") < 1500)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool wantToThieveStall(Player p, ushort id, int x, int y)
        {
            for (int i = 0; i < STALLS.Length; i++)
            {
                for (int j = 0; j < STALLS[i].Length; j++)
                {
                    if (id == STALLS[i][j])
                    {
                        thieveStall(p, i, id, x, y);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool wantToThieveChest(Player p, ushort id, int x, int y)
        {
            for (int i = 0; i < CHESTS.Length; i++)
            {
                if (id == CHESTS[i])
                {
                    thieveChest(p, i, id, x, y);
                    return true;
                }
            }
            return false;
        }

        private static void thieveChest(Player p, int index, ushort chestId, int x, int y)
        {
            AreaEvent thieveChestAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 1, y + 1);
            thieveChestAreaEvent.setAction(() =>
            {
                p.setFaceLocation(new Location(x, y, p.getLocation().getZ()));
                if (!canThieveChest(p, index, chestId, x, y))
                {
                    return;
                }
                p.getPackets().sendMessage("You attempt to pick the chest lock..");
                p.setLastAnimation(new Animation(833));
                Event thieveChestEvent = new Event(1000);
                thieveChestEvent.setAction(() =>
                {
                    thieveChestEvent.stop();
                    if (Misc.random(5) == 0)
                    {
                        p.hit(p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS) / 10);
                        p.setForceText("Ouch!");
                        p.getPackets().sendMessage("You activate a trap whilst trying to pick the lock!");
                        return;
                    }
                    if (Server.getGlobalObjects().originalObjectExists(chestId, new Location(x, y, 0)))
                    {
                        Server.getGlobalObjects().lowerHealth(chestId, new Location(x, y, 0));
                        for (int i = 0; i < CHEST_REWARD[index].Length; i++)
                        {
                            p.getInventory().addItem(CHEST_REWARD[index][i], CHEST_REWARD_AMOUNTS[index][i]);
                        }
                        p.getSkills().addXp(Skills.SKILL.THIEVING, CHEST_XP[index]);
                        p.getPackets().sendMessage("You successfully pick the lock and loot the chest!");
                    }
                });
                Server.registerEvent(thieveChestEvent);
            });
            Server.registerCoordinateEvent(thieveChestAreaEvent);
        }

        protected static bool canThieveChest(Player p, int index, ushort chestId, int x, int y)
        {
            if (p == null || p.isDead() || p.isDestroyed() || p.isDisconnected())
            {
                return false;
            }
            if (!Server.getGlobalObjects().objectExists(chestId, new Location(x, y, 0)))
            {
                Misc.WriteError(p.getLoginDetails().getUsername() + " tried to steal from a non existing chest!");
                return false;
            }
            if (p.getInventory().getTotalFreeSlots() < CHEST_REWARD[index].Length)
            {
                p.getPackets().sendMessage("You don't have enough free inventory space for the loot from that chest.");
                return false;
            }
            if (!Server.getGlobalObjects().originalObjectExists(chestId, new Location(x, y, 0)))
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.THIEVING) < CHEST_LVL[index])
            {
                p.getPackets().sendMessage("You need a Thieving level of " + CHEST_LVL[index] + " to steal from this chest.");
                return false;
            }
            return true;
        }

        private static void thieveStall(Player p, int index, ushort stallId, int x, int y)
        {
            int stallFace = Server.getGlobalObjects().getFace(stallId, new Location(x, y, 0));
            int[] areaCoords = new int[4];
            if (index == 2 || index == 4 || index == 5 || index == 12 || index == 13)
            { // Ape atoll stalls
                areaCoords[0] = x - 1;
                areaCoords[1] = y - 1;
                areaCoords[2] = x + 1;
                areaCoords[3] = y + 1;
            }
            else
            {
                areaCoords = getAreaCoords(stallFace, x, y);
            }
            AreaEvent thieveStallAreaEvent = new AreaEvent(p, areaCoords[0], areaCoords[1], areaCoords[2], areaCoords[3]);
            thieveStallAreaEvent.setAction(() =>
            {
                if (!canThieveStall(p, index, stallId, x, y))
                {
                    return;
                }
                p.setLastAnimation(new Animation(833));
                Event thieveStallEvent = new Event(1000);
                thieveStallEvent.setAction(() =>
                {
                    thieveStallEvent.stop();
                    if (Server.getGlobalObjects().originalObjectExists(stallId, new Location(x, y, 0)))
                    {
                        Server.getGlobalObjects().lowerHealth(stallId, new Location(x, y, 0));
                        p.getSkills().addXp(Skills.SKILL.THIEVING, STALL_XP[index]);
                        int rewardIndex = Misc.random(STALL_REWARD[index].Length - 1);
                        int reward = STALL_REWARD[index][rewardIndex];
                        if (index == 7)
                        { // Seed stall
                            if (Misc.random(15) == 0)
                            {
                                reward = HERB_SEEDS[Misc.random(HERB_SEEDS.Length - 1)];
                            }
                        }
                        else if (index == 13)
                        { // Scimitar stall
                            if (Misc.random(75) == 0)
                            {
                                reward = 1333; // Rune scimitar.
                            }
                            else if (Misc.random(250) == 0)
                            {
                                reward = 4587; // Dragon scimitar.
                            }
                        }
                        int amount = Misc.random(STALL_REWARD_AMOUNTS[index][rewardIndex]);
                        if (amount <= 0)
                        {
                            amount = 1;
                        }
                        p.getInventory().addItem(reward, amount);
                    }
                });
                Server.registerEvent(thieveStallEvent);
            });
            Server.registerCoordinateEvent(thieveStallAreaEvent);
        }

        private static int[] getAreaCoords(int face, int x, int y)
        {
            int[] coords = new int[4];
            switch (face)
            {
                case 0:
                case 3:
                    coords[0] = x - 1;
                    coords[1] = y - 1;
                    coords[2] = x + 2;
                    coords[3] = y + 2;
                    break;

                case 1:
                case 2:
                    coords[0] = x - 1;
                    coords[1] = y - 1;
                    coords[2] = x + 2;
                    coords[3] = y + 2;
                    break;
            }
            return coords;
        }

        private static bool canThieveStall(Player p, int index, ushort stallId, int x, int y)
        {
            if (p == null || p.isDead() || p.isDestroyed() || p.isDisconnected())
            {
                return false;
            }
            if (!Server.getGlobalObjects().objectExists(stallId, new Location(x, y, 0)))
            {
                Misc.WriteError(p.getLoginDetails().getUsername() + " tried to steal from a non existing stall!");
                return false;
            }
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendMessage("You need a free inventory space for any potential loot.");
                return false;
            }
            if (!Server.getGlobalObjects().originalObjectExists(stallId, new Location(x, y, 0)))
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.THIEVING) < STALL_LVL[index])
            {
                p.getPackets().sendMessage("You need a Thieving level of " + STALL_LVL[index] + " to steal from this stall.");
                return false;
            }
            return true;
        }
    }
}