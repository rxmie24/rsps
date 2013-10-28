using RS2.Server.events;
using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.firemaking
{
    internal class Firemaking : FiremakingData
    {
        public Firemaking()
        {
        }

        public static bool isFiremaking(Player p, int itemUsed, int usedWith, int usedSlot, int withSlot)
        {
            int itemOne = itemUsed;
            int itemTwo = usedWith;
            int slotType;
            int slot;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    itemOne = usedWith;
                    itemTwo = itemUsed;
                }
                for (int j = 0; j < LOGS.Length; j++)
                {
                    if (itemOne == LOGS[j] && itemTwo == TINDERBOX)
                    {
                        slotType = itemUsed == TINDERBOX ? 1 : usedWith == TINDERBOX ? 2 : -1;
                        slot = slotType == 1 ? withSlot : usedSlot;
                        lightFire(p, j, false, slot);
                        return true;
                    }
                }
                for (int j = 0; j < COLOURED_LOGS.Length; j++)
                {
                    if (itemOne == COLOURED_LOGS[j] && itemTwo == TINDERBOX)
                    {
                        slotType = itemUsed == TINDERBOX ? 1 : usedWith == TINDERBOX ? 2 : -1;
                        slot = slotType == 1 ? withSlot : usedSlot;
                        lightFire(p, j, true, slot);
                        return true;
                    }
                }
                for (int j = 0; j < OTHER_ITEMS.Length; j++)
                {
                    if (itemOne == OTHER_ITEMS[j][0] && itemTwo == TINDERBOX)
                    {
                        slotType = itemUsed == TINDERBOX ? 1 : usedWith == TINDERBOX ? 2 : -1;
                        slot = slotType == 1 ? withSlot : usedSlot;
                        lightLightSource(p, j, slot);
                        return true;
                    }
                }
                //ground fire log lighting not item on item, so below is useless looping.
                if (withSlot == -1 || usedSlot == -1) continue;
                for (int j = 0; j < LOGS.Length; j++)
                {
                    for (int k = 0; k < FIRELIGHTERS.Length; k++)
                    {
                        if (itemOne == LOGS[j] && itemTwo == FIRELIGHTERS[k])
                        {
                            useFirelighter(p, j, k, usedSlot, withSlot);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static void lightLightSource(Player p, int j, int slot)
        {
            if (p.getInventory().replaceItemSlot(OTHER_ITEMS[j][0], OTHER_ITEMS[j][1], slot))
            {
                p.getPackets().sendMessage("You ignite the light source.");
            }
        }

        private static void useFirelighter(Player p, int j, int k, int usedSlot, int withSlot)
        {
            if (p.getInventory().deleteItem(FIRELIGHTERS[k], usedSlot, 1))
            {
                p.getInventory().replaceItemSlot(LOGS[j], COLOURED_LOGS[k], withSlot);
                p.getPackets().sendMessage("You cover the log in the strange goo..it changes colour.");
            }
            else
            {
                if (p.getInventory().deleteItem(FIRELIGHTERS[k], withSlot, 1))
                {
                    p.getInventory().replaceItemSlot(LOGS[j], COLOURED_LOGS[k], usedSlot);
                    p.getPackets().sendMessage("You cover the log in the strange goo..it changes colour.");
                }
            }
        }

        private static void lightGroundFire(Player p, int logIndex, bool colouredFire)
        {
            lightFire(p, logIndex, colouredFire, -1);
        }

        private static void lightFire(Player p, int logIndex, bool colouredFire, int slot)
        {
            //if (!World.getInstance().getObjectLocations().tileAvailable(new Location(p.getLocation().getX() - 1, p.getLocation().getY(), p.getLocation().getZ()))) {
            //return;
            //}
            // TODO clip this
            p.getPackets().closeInterfaces();
            int log = colouredFire ? COLOURED_LOGS[logIndex] : LOGS[logIndex];

            if (!canMakeFire(p, logIndex, colouredFire))
                return;

            if (slot != -1)
            {
                //item in inventory wasn't a log (you may have swapped it/banked it quickly).
                if (!p.getInventory().deleteItem(log, slot, 1))
                    return;

                Server.getGroundItems().newEntityDrop(new GroundItem(log, 1, (Location)p.getLocation().Clone(), p));
            }
            else
            { //light fire using logs already placed on ground.
                //check if there is a log on the ground in our current location, if not we can't make fire.
                GroundItem gi = Server.getGroundItems().itemExists(p.getLocation(), log);
                if (gi == null)
                    return; //so we quit here.
            }

            int delay = getDelay(p, logIndex);
            p.getWalkingQueue().resetWalkingQueue();
            p.getPackets().clearMapFlag();
            if (delay == START_DELAY)
                p.setLastAnimation(new Animation(733));
            p.getPackets().sendMessage("You attempt to light the logs.");
            ushort fireObject = colouredFire ? COLOURED_FIRES[logIndex] : FIRE_OBJECT;
            p.setTemporaryAttribute("unmovable", true);
            Event lightFireEvent = new Event(delay);
            lightFireEvent.setAction(() =>
            {
                lightFireEvent.stop();
                p.setTemporaryAttribute("lastFiremake", Environment.TickCount);
                p.setLastAnimation(new Animation(65535));
                p.getWalkingQueue().forceWalk(-1, 0);
                Event finishFireEvent = new Event(1000);
                finishFireEvent.setAction(() =>
                {
                    finishFireEvent.stop();
                    Location fireLocation = new Location((p.getLocation().getX() + 1), p.getLocation().getY(), p.getLocation().getZ());
                    if (Server.getGroundItems().deleteItem(log, fireLocation))
                    {
                        p.getPackets().sendMessage("The fire catches and the logs begin to burn.");
                        p.setFaceLocation(fireLocation);
                        p.getSkills().addXp(Skills.SKILL.FIREMAKING, FIRE_XP[logIndex]);
                        Server.getGlobalObjects().newFire(p, fireObject, fireLocation);
                    }
                    p.removeTemporaryAttribute("unmovable");
                });
                Server.registerEvent(finishFireEvent);
            });
            Server.registerEvent(lightFireEvent);
        }

        private static int getDelay(Player p, int index)
        {
            long lastFireTime = 0;
            if (p.getTemporaryAttribute("lastFiremake") != null)
            {
                lastFireTime = (int)p.getTemporaryAttribute("lastFiremake");
                if (Environment.TickCount - lastFireTime < 2500)
                {
                    return 1000;
                }
            }
            return START_DELAY;
        }

        private static bool canMakeFire(Player p, int logIndex, bool colouredLog)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return false;
            }
            if (Server.getGlobalObjects().fireExists(p.getLocation()))
            {
                p.getPackets().sendMessage("You cannot light a fire here.");
                return false;
            }
            if ((p.getSkills().getGreaterLevel(Skills.SKILL.FIREMAKING) < FIRE_LEVEL[logIndex]) && !colouredLog)
            {
                p.getPackets().sendMessage("You need a Firemaking level of " + FIRE_LEVEL[logIndex] + " to light this log.");
                return false;
            }
            if (!p.getInventory().hasItem(TINDERBOX))
            {
                p.getPackets().sendMessage("You need a tinderbox if you intend on actually make a fire!");
                return false;
            }
            return true;
        }
    }
}