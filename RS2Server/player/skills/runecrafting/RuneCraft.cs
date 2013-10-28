using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;
using System;

namespace RS2.Server.player.skills.runecrafting
{
    internal class RuneCraft : RuneCraftData
    {
        public RuneCraft()
        {
        }

        public static bool wantToRunecraft(Player p, int Object, int x, int y)
        {
            for (int i = 0; i < ALTARS.Length; i++)
            {
                if (Object == ALTARS[i])
                {
                    craftRunes(p, i, x, y);
                    faceAltar(p, i);
                    return true;
                }
            }
            return false;
        }

        public static bool fillPouch(Player p, POUCHES pouch)
        {
            if (pouch != POUCHES.SMALL_POUCH && pouch != POUCHES.MEDIUM_POUCH && pouch != POUCHES.LARGE_POUCH && pouch != POUCHES.GIANT_POUCH)
            {
                return false;
            }
            int amount = 0;
            int pouchAmount = 0;
            int invenEss = p.getInventory().getItemAmount(ESSENCE);
            switch (pouch)
            {
                case POUCHES.SMALL_POUCH:
                    amount = 3;
                    pouchAmount = p.getSmallPouchAmount();
                    break;

                case POUCHES.MEDIUM_POUCH:
                    amount = 6;
                    pouchAmount = p.getMediumPouchAmount();
                    break;

                case POUCHES.LARGE_POUCH:
                    amount = 9;
                    pouchAmount = p.getLargePouchAmount();
                    break;

                case POUCHES.GIANT_POUCH:
                    amount = 12;
                    pouchAmount = p.getGiantPouchAmount();
                    break;
            }
            amount = (amount - pouchAmount);
            if (invenEss <= 0)
            {
                return true;
            }
            if (amount >= invenEss)
            {
                amount = invenEss;
            }
            if (amount <= 0)
            {
                p.getPackets().sendMessage("This pouch is full.");
                return true;
            }
            for (int i = 0; i < amount; i++)
            {
                if (!p.getInventory().deleteItem(ESSENCE))
                {
                    p.getPackets().sendMessage("An error occured whilst deleting essence.");
                    return true;
                }
            }
            setAddPouchAmount(p, pouch, amount);
            return true;
        }

        public static bool emptyPouch(Player p, POUCHES pouch)
        {
            if (pouch != POUCHES.SMALL_POUCH && pouch != POUCHES.MEDIUM_POUCH && pouch != POUCHES.LARGE_POUCH && pouch != POUCHES.GIANT_POUCH)
            {
                return false;
            }
            int freeSpace = p.getInventory().getTotalFreeSlots();
            int amount = 0;
            switch (pouch)
            {
                case POUCHES.SMALL_POUCH:
                    amount = p.getSmallPouchAmount();
                    break;

                case POUCHES.MEDIUM_POUCH:
                    amount = p.getMediumPouchAmount();
                    break;

                case POUCHES.LARGE_POUCH:
                    amount = p.getLargePouchAmount();
                    break;

                case POUCHES.GIANT_POUCH:
                    amount = p.getGiantPouchAmount();
                    break;
            }
            if (amount >= freeSpace)
            {
                amount = freeSpace;
            }
            for (int i = 0; i < amount; i++)
            {
                p.getInventory().addItem(ESSENCE);
            }
            setRemovePouchAmount(p, pouch, amount);
            return true;
        }

        private static void setAddPouchAmount(Player p, POUCHES pouch, int amount)
        {
            switch (pouch)
            {
                case POUCHES.SMALL_POUCH:
                    p.setSmallPouchAmount(p.getSmallPouchAmount() + amount);
                    break;

                case POUCHES.MEDIUM_POUCH:
                    p.setMediumPouchAmount(p.getMediumPouchAmount() + amount);
                    break;

                case POUCHES.LARGE_POUCH:
                    p.setLargePouchAmount(p.getLargePouchAmount() + amount);
                    break;

                case POUCHES.GIANT_POUCH:
                    p.setGiantPouchAmount(p.getGiantPouchAmount() + amount);
                    break;
            }
        }

        private static void setRemovePouchAmount(Player p, POUCHES pouch, int amount)
        {
            switch (pouch)
            {
                case POUCHES.SMALL_POUCH:
                    p.setSmallPouchAmount(p.getSmallPouchAmount() - amount);
                    break;

                case POUCHES.MEDIUM_POUCH:
                    p.setMediumPouchAmount(p.getMediumPouchAmount() - amount);
                    break;

                case POUCHES.LARGE_POUCH:
                    p.setLargePouchAmount(p.getLargePouchAmount() - amount);
                    break;

                case POUCHES.GIANT_POUCH:
                    p.setGiantPouchAmount(p.getGiantPouchAmount() - amount);
                    break;
            }
        }

        private static void faceAltar(Player p, int i)
        {
            p.setFaceLocation(new Location(ALTAR_COORDS[i][0], ALTAR_COORDS[i][1], p.getLocation().getZ()));
        }

        private static void craftRunes(Player p, int i, int x, int y)
        {
            AreaEvent craftRunesAreaEvent = new AreaEvent(p, x - 3, y - 3, x + 3, y + 3);
            craftRunesAreaEvent.setAction(() =>
            {
                if (p.getTemporaryAttribute("lastRunecraftTime") != null)
                {
                    if (Environment.TickCount - (int)p.getTemporaryAttribute("lastRunecraftTime") < 500)
                    {
                        return;
                    }
                }
                if (!p.getInventory().hasItem(ESSENCE))
                {
                    p.getPackets().sendMessage("You have no Pure essence.");
                    return;
                }
                if (p.getSkills().getGreaterLevel(Skills.SKILL.RUNECRAFTING) < CRAFT_LEVEL[i])
                {
                    p.getPackets().sendMessage("You need a Runecrafting level of " + CRAFT_LEVEL[i] + " to craft " + ItemData.forId(RUNES[i]).getName() + "s.");
                    return;
                }
                p.setLastAnimation(new Animation(791));
                Event craftRunesEvent = new Event(250);
                craftRunesEvent.setAction(() =>
                {
                    craftRunesEvent.stop();
                    int amount = p.getInventory().getItemAmount(ESSENCE);
                    for (int j = 0; j < amount; j++)
                    {
                        if (!p.getInventory().deleteItem(ESSENCE))
                        {
                            p.getPackets().sendMessage("An error occured whilst deleting essence from your inventory.");
                            return;
                        }
                    }
                    int multiply = 1;
                    for (int j = 0; j < MULTIPLY_LEVELS[i].Length; j++)
                    {
                        if (p.getSkills().getGreaterLevel(Skills.SKILL.RUNECRAFTING) >= MULTIPLY_LEVELS[i][j])
                        {
                            multiply++;
                        }
                    }
                    string s = amount > 1 || (amount == 1 && multiply > 1) ? "s." : ".";
                    string s1 = amount > 1 || (amount == 1 && multiply > 1) ? "" : "a ";
                    if (p.getInventory().addItem(RUNES[i], amount * multiply))
                    {
                        p.getSkills().addXp(Skills.SKILL.RUNECRAFTING, (CRAFT_XP[i] * amount));
                        p.getPackets().sendMessage("You craft the essence into " + s1 + ItemData.forId(RUNES[i]).getName() + s);
                    }
                    p.setTemporaryAttribute("lastRunecraftTime", Environment.TickCount);
                });
            });
            Server.registerCoordinateEvent(craftRunesAreaEvent);
        }

        public static void enterAltar(Player p, int i)
        {
            if (i == 13)
            {
                return;
            }
            if (i == 12)
            {
                p.getPackets().sendMessage("This altar is currently unavailable due to mapdata issues.");
                return;
            }
            p.teleport(new Location(RUIN_TELEPORT[i][0], RUIN_TELEPORT[i][1], 0));
        }

        public static bool enterRift(Player p, int objectId, int x, int y)
        {
            for (int i = 0; i < ABYSS_DOORWAYS.Length; i++)
            {
                if (objectId == ABYSS_DOORWAYS[i])
                {
                    if (i == 13)
                    {
                        return true;
                    }
                    int j = i;
                    CoordinateEvent enterRiftCoordinateEvent = new CoordinateEvent(p, new Location(x, y, 0));
                    enterRiftCoordinateEvent.setAction(() =>
                    {
                        if (j == 12)
                        {
                            p.getPackets().sendMessage("This altar is currently unavailable due to mapdata issues.");
                            return;
                        }
                        p.teleport(new Location(ALTAR_COORDS[j][0], (ALTAR_COORDS[j][1] + 3), 0));
                        faceAltar(p, j);
                    });
                    Server.registerCoordinateEvent(enterRiftCoordinateEvent);
                    return true;
                }
            }
            return false;
        }

        public static void toggleRuin(Player p, int id, bool show)
        {
            int index = getTiaraIndex(id);
            if (index == -1)
            {
                return;
            }
            int ruinId = show ? RUINS2[index] : RUINS[index];
            int face = index == 6 ? 2 : index == 7 ? 3 : 0;
            p.getPackets().createObject((ruinId), new Location(RUIN_COORDS[index][0], RUIN_COORDS[index][1], 0), face, 10);
        }

        public static int getTiaraIndex(int id)
        {
            for (int j = 0; j < TIARAS.Length; j++)
            {
                if (id == TIARAS[j])
                {
                    return j;
                }
            }
            return -1;
        }

        public static bool wearingTiara(Player p)
        {
            for (int i = 0; i < TIARAS.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.HAT) == TIARAS[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool enterViaTiara(Player p, int Object, int x, int y)
        {
            if (Object < 7104 || Object > 7124)
            {
                return false;
            }
            for (int i = 0; i < RUINS2.Length; i++)
            {
                if (Object == RUINS2[i])
                {
                    int j = i;
                    AreaEvent enterViaTiaraAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 4, y + 4);
                    enterViaTiaraAreaEvent.setAction(() =>
                    {
                        if (wearingTiara(p))
                        {
                            enterAltar(p, j);
                        }
                    });
                    Server.registerCoordinateEvent(enterViaTiaraAreaEvent);
                    return true;
                }
            }
            return false;
        }

        public static bool useTalisman(Player p, int Object, int x, int y)
        {
            if (Object < 2452 || Object > 2461)
            {
                return false;
            }
            for (int i = 0; i < RUINS.Length; i++)
            {
                if (Object == RUINS[i])
                {
                    int j = i;
                    AreaEvent useTalismanAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 4, y + 4);
                    useTalismanAreaEvent.setAction(() =>
                    {
                        if (hasTalisman(p, j))
                        {
                            enterAltar(p, j);
                        }
                    });
                    Server.registerCoordinateEvent(useTalismanAreaEvent);
                    return true;
                }
            }
            return false;
        }

        private static bool hasTalisman(Player p, int i)
        {
            for (int j = 0; j < TALISMANS.Length; j++)
            {
                if (p.getInventory().hasItem(TALISMANS[j]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool leaveAltar(Player p, int Object, int x, int y)
        {
            if (Object < 2465 || Object > 2475)
            {
                return false;
            }
            for (int i = 0; i < PORTALS.Length; i++)
            {
                if (Object == PORTALS[i])
                {
                    if (i != 6)
                    {
                        if (x != PORTAL_COORDS[i][0] && y != PORTAL_COORDS[i][1])
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (i == 6 && (x != 2163 && y != 4833) && (x != 2142 && y != 4854) && (x != 2121 && y != 4833) && (x != 2142 && y != 4812))
                        {
                            return false;
                        }
                    }
                    int j = i;
                    AreaEvent leaveAltarAreaEvent = new AreaEvent(p, x - 1, y - 1, x + 1, y + 1);
                    leaveAltarAreaEvent.setAction(() =>
                    {
                        teleportOutOfAltar(p, j);
                    });
                    Server.registerCoordinateEvent(leaveAltarAreaEvent);
                    return true;
                }
            }
            return false;
        }

        private static void teleportOutOfAltar(Player p, int i)
        {
            int x = RUIN_COORDS[i][0] + 1;
            int y = RUIN_COORDS[i][1] - 1;
            p.teleport(new Location(x, y, 0));
        }

        public static Location teleportInner()
        {
            int i = Misc.random(ABYSS_TELEPORT_INNER.Length - 1);
            return new Location(ABYSS_TELEPORT_INNER[i][0], ABYSS_TELEPORT_INNER[i][1], 0);
        }

        public static Location teleportOuter()
        {
            int i = Misc.random(ABYSS_TELEPORT_OUTER.Length - 1);
            return new Location(ABYSS_TELEPORT_OUTER[i][0], ABYSS_TELEPORT_OUTER[i][1], 0);
        }

        public static void teleportToEssMine(Player p, Npc n)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            p.setTemporaryAttribute("unmovable", true);
            n.setLastGraphics(new Graphics(108));
            Event teleportToEssMineEvent = new Event(600);
            teleportToEssMineEvent.setAction(() =>
            {
                int i = 0;
                i++;
                if (i == 1)
                {
                    p.setLastGraphics(new Graphics(110));
                    n.setForceText("Senventior disthine molenko!");
                }
                else if (i == 2)
                {
                    teleportToEssMineEvent.stop();
                    Event doTeleportToEssMineEvent = new Event(300);
                    doTeleportToEssMineEvent.setAction(() =>
                    {
                        p.teleport(getRandomMineLocation());
                        p.removeTemporaryAttribute("unmovable");
                        doTeleportToEssMineEvent.stop();
                    });
                }
            });
            Server.registerEvent(teleportToEssMineEvent);
        }

        protected static Location getRandomMineLocation()
        {
            switch (Misc.random(4))
            {
                case 0: return new Location(2920 + Misc.random(2), 4845 + Misc.random(4), 0);
                case 1: return new Location(2897 + Misc.random(2), 4849 + Misc.random(4), 0);
                case 3: return new Location(2897 + Misc.random(2), 4808 + Misc.random(3), 0);
                case 4: return new Location(2929 + Misc.random(1), 4807 + Misc.random(6), 0);
            }
            return new Location(2909 + Misc.random(3), 4830 + Misc.random(4), 0);
        }

        public static void leaveEssMine(Player p, Location loc)
        {
            AreaEvent leaveEssMineAreaEvent = new AreaEvent(p, loc.getX() - 1, loc.getY() - 1, loc.getX() + 1, loc.getY() + 1);
            leaveEssMineAreaEvent.setAction(() =>
            {
                p.teleport(new Location(2340 + Misc.random(1), 3155 + Misc.random(1), 0));
            });
            Server.registerCoordinateEvent(leaveEssMineAreaEvent);
        }
    }
}