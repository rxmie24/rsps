using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;

namespace RS2.Server.minigames.barrows
{
    internal class Barrows : BarrowsData
    {
        public Barrows()
        {
        }

        /*
         * The config to remove roofs is 1270
         * The door is 6713
         *
         * Random door configs
         * CONFIG = 452    0
           CONFIG = 452    32
           CONFIG = 452    96
           CONFIG = 452    16480
           CONFIG = 452    278624
           CONFIG = 452    802912
           CONFIG = 452    2900064
           CONFIG = 452    2637920
           CONFIG = 452    2638944
           CONFIG = 452    2640992
           CONFIG = 452    2645088
           CONFIG = 452    2653280
           CONFIG = 452    2649184
         */

        public static bool enterCrypt(Player p)
        {
            for (int i = 0; i < MOUND_COORDS.Length; i++)
            {
                for (int j = 0; j < MOUND_COORDS[i].Length; j++)
                {
                    if (p.getLocation().inArea(MOUND_COORDS[i][0], MOUND_COORDS[i][1], MOUND_COORDS[i][2], MOUND_COORDS[i][3]) && p.getLocation().getZ() == 0)
                    {
                        p.teleport(new Location(STAIR_COORDS[i][0], STAIR_COORDS[i][1], 3));
                        if (p.getBarrowTunnel() == -1)
                        {
                            p.setBarrowTunnel(Misc.random(5));
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool leaveCrypt(Player player, int stairs, int x, int y)
        {
            if (stairs != 6707 && stairs != 6703 && stairs != 6702 && stairs != 6704 && stairs != 6705 && stairs != 6706)
            {
                return false;
            }
            Player p = player;
            Location stairLocation;
            Location moundLocation;
            int cryptIndex = getCryptIndex(p);
            if (cryptIndex == -1)
            {
                return false;
            }
            stairLocation = new Location(STAIR_COORDS[cryptIndex][0], STAIR_COORDS[cryptIndex][1], 3);
            moundLocation = new Location(MOUND_COORDS[cryptIndex][0] + Misc.random(3), MOUND_COORDS[cryptIndex][1] + Misc.random(3), 0);
            if (p.getLocation().Equals(stairLocation))
            {
                p.setFaceLocation(new Location(x, y, 3));
                p.teleport(moundLocation);
                return true;
            }
            CoordinateEvent teleportMoundCoordinateEvent = new CoordinateEvent(p, stairLocation);
            teleportMoundCoordinateEvent.setAction(() =>
            {
                p.teleport(moundLocation);
            });
            Server.registerCoordinateEvent(teleportMoundCoordinateEvent);
            return true;
        }

        public static void removeBrotherFromGame(Player p)
        {
            foreach (Npc n in Server.getNpcList())
            {
                if (n != null)
                {
                    if (n.getId() >= 2025 && n.getId() <= 2030)
                    {
                        if (n.getOwner() == null || n.getOwner().Equals(p) || n.getOwner().isDestroyed())
                        {
                            if (!n.isDead())
                            {
                                n.setHidden(true);
                                Server.getNpcList().Remove(n); //TODO: save? idk yet
                            }
                        }
                    }
                }
            }
        }

        public static bool tryOpenCoffin(Player player, int objectId)
        {
            if (objectId != 6823 && objectId != 6771 && objectId != 6821 && objectId != 6773 && objectId != 6822 && objectId != 6772)
            {
                return false;
            }
            Player p = player;
            int cryptIndex = getCryptIndex(p);
            if (cryptIndex == -1)
            {
                return false;
            }
            AreaEvent openCoffinAreaEvent = new AreaEvent(p, COFFIN_AREA[cryptIndex][0], COFFIN_AREA[cryptIndex][1], COFFIN_AREA[cryptIndex][2], COFFIN_AREA[cryptIndex][3]);
            openCoffinAreaEvent.setAction(() =>
            {
                openCoffin(p, objectId);
            });
            Server.registerCoordinateEvent(openCoffinAreaEvent);
            return true;
        }

        public static bool openCoffin(Player p, int objectId)
        {
            if (objectId != 6823 && objectId != 6771 && objectId != 6821 && objectId != 6773 && objectId != 6822 && objectId != 6772)
            {
                return false;
            }
            int cryptIndex = getCryptIndex(p);
            if (cryptIndex == -1)
            {
                return false;
            }
            if (p.getBarrowBrothersKilled(cryptIndex))
            {
                p.getPackets().sendMessage("You don't find anything.");
                return true;
            }
            if (p.getBarrowTunnel() == cryptIndex)
            {
                p.getPackets().modifyText("You find a hidden tunnel, do you want to enter?", 210, 1);
                p.getPackets().sendChatboxInterface(210);
                p.setTemporaryAttribute("barrowTunnel", 1);
                return true;
            }
            foreach (Npc n in Server.getNpcList())
            {
                if (n.getId() == BROTHER_ID[cryptIndex])
                {
                    if (n.getOwner().Equals(p))
                    {
                        p.getPackets().sendMessage("You don't find anything.");
                        return true;
                    }
                }
            }
            Npc npc = new Npc(BROTHER_ID[cryptIndex]);
            npc.setLocation(p.getLocation());
            npc.setEntityFocus(p.getClientIndex());
            npc.setOwner(p);
            npc.setTarget(p);
            npc.setCombatTurns(npc.getAttackSpeed());
            Server.getNpcList().Add(npc);
            p.getPackets().setArrowOnEntity(1, npc.getClientIndex());
            return true;
        }

        public static int getCryptIndex(Player p)
        {
            if (p.getLocation().inArea(3567, 9701, 3580, 9711))
            {
                return VERAC;
            }
            else if (p.getLocation().inArea(3548, 9709, 3561, 9721))
            {
                return DHAROK;
            }
            else if (p.getLocation().inArea(3549, 9691, 3562, 9706))
            {
                return AHRIM;
            }
            else if (p.getLocation().inArea(3532, 9698, 3546, 9710))
            {
                return GUTHAN;
            }
            else if (p.getLocation().inArea(3544, 9677, 3559, 9689))
            {
                return KARIL;
            }
            else if (p.getLocation().inArea(3563, 9680, 3577, 9694))
            {
                return TORAG;
            }
            return -1;
        }

        public static void verifyEnterTunnel(Player p)
        {
            p.getPackets().closeInterfaces();
            if (p.getTemporaryAttribute("barrowTunnel") != null)
            {
                if ((int)p.getTemporaryAttribute("barrowTunnel") == 2)
                {
                    p.teleport(new Location(3568, 9712, 0));
                    p.removeTemporaryAttribute("barrowTunnel");
                    return;
                }
            }
            p.getPackets().sendChatboxInterface(228);
            p.getPackets().modifyText("Yeah, I'm fearless!", 228, 2);
            p.getPackets().modifyText("No way, that looks scary!", 228, 3);
            p.setTemporaryAttribute("barrowTunnel", 2);
        }

        public static void openChest(Player player)
        {
            Player p = player;
            if (p.getLocation().getZ() != 0 || p.getTemporaryAttribute("lootedBarrowChest") != null)
            {
                return;
            }
            if (!p.getLocation().inArea(3551, 9694, 3552, 9694))
            {
                AreaEvent openChestAreaEvent = new AreaEvent(p, 3551, 9694, 3552, 9694);
                openChestAreaEvent.setAction(() =>
                {
                    openChest(p);
                });
                Server.registerCoordinateEvent(openChestAreaEvent);
                return;
            }
            for (int i = 0; i < 6; i++)
            {
                if (!p.getBarrowBrothersKilled(i))
                {
                    foreach (Npc n in Server.getNpcList())
                    {
                        if (n != null)
                        {
                            if (n.getId() == BROTHER_ID[i])
                            {
                                if (n.getOwner().Equals(p))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    Npc npc = new Npc(BROTHER_ID[i]);
                    npc.setLocation(p.getLocation());
                    npc.setEntityFocus(p.getClientIndex());
                    npc.setOwner(p);
                    npc.setTarget(p);
                    npc.setCombatTurns(npc.getAttackSpeed());
                    Server.getNpcList().Add(npc);
                    p.getPackets().setArrowOnEntity(1, npc.getClientIndex());
                    return;
                }
            }
            p.getPackets().sendMessage("You begin to lift open the massive chest...");
            p.setLastAnimation(new Animation(833));

            Event rewardEarthQuakeEvent = new Event(1000);
            rewardEarthQuakeEvent.setAction(() =>
            {
                rewardEarthQuakeEvent.stop();
                p.getPackets().sendMessage("..You loot the chest and the tomb begins to shake!");
                p.getPackets().createObject(6775, new Location(3551, 9695, 0), 0, 10);
                getBarrowReward(p);
                startEarthQuake(p);
            });
            Server.registerEvent(rewardEarthQuakeEvent);
        }

        protected static void getBarrowReward(Player p)
        {
            int barrowChance = Misc.random(BARROWS_CHANCE);
            int killCount = p.getBarrowKillCount();
            if (barrowChance == 0)
            {
                int reward = BARROW_REWARDS[Misc.random(BARROW_REWARDS.Length - 1)];
                p.getInventory().addItemOrGround(reward);
            }
            if (Misc.random(20) == 0)
            {
                p.getInventory().addItemOrGround(1149); // Dragon med helm.
            }
            else if (Misc.random(15) == 0)
            {
                int halfKey = Misc.random(1) == 0 ? 985 : 987;
                p.getInventory().addItemOrGround(halfKey); // Half key.
            }
            if (Misc.random(3) == 0 || p.getBarrowTunnel() == KARIL)
            { // Bolt racks.
                int amount = getAmountOfReward(4740, killCount);
                p.getInventory().addItemOrGround(4740, amount);
            }
            if (Misc.random(3) == 0)
            { // Blood runes
                int amount = getAmountOfReward(565, killCount);
                p.getInventory().addItemOrGround(565, amount);
            }
            if (Misc.random(2) == 0)
            { // Death runes
                int amount = getAmountOfReward(560, killCount);
                p.getInventory().addItemOrGround(560, amount);
            }
            if (Misc.random(1) == 0)
            { // Chaos runes
                int amount = getAmountOfReward(562, killCount);
                p.getInventory().addItemOrGround(562, amount);
            }
            if (Misc.random(1) == 0)
            { // Coins
                int amount = getAmountOfReward(995, killCount);
                p.getInventory().addItemOrGround(995, amount);
            }
            if (Misc.random(1) == 0)
            {
                int amount = getAmountOfReward(558, killCount); // Mind runes.
                p.getInventory().addItemOrGround(558, amount);
            }
        }

        private static int getAmountOfReward(int item, int killCount)
        {
            int amount = 0;
            for (int i = 0; i < OTHER_REWARDS.Length; i++)
            {
                if (OTHER_REWARDS[i] == item)
                {
                    for (int j = 0; j < REWARD_KILLCOUNT[i].Length; j++)
                    {
                        if (killCount >= REWARD_KILLCOUNT[i][j])
                        {
                            amount = REWARD_AMOUNT[i][j];
                        }
                    }
                    if (amount < MINIMUM_AMOUNT[i])
                    {
                        amount = MINIMUM_AMOUNT[i];
                    }
                    break;
                }
            }
            return Misc.random(amount);
        }

        public static void startEarthQuake(Player player)
        {
            Player p = player;
            p.getPackets().newEarthquake(4, 4, 4, 4, 4);
            p.setTemporaryAttribute("lootedBarrowChest", true);
            p.getPackets().createObject(6775, new Location(3551, 9695, 0), 0, 10);
            p.setBarrowKillCount(0);
            p.getPackets().sendConfig(453, 0);
            Event takeEarthQuakeDamageEvent = new Event(3000 + Misc.random(10000)); //every 3-13 seconds.
            takeEarthQuakeDamageEvent.setAction(() =>
            {
                if (p.getTemporaryAttribute("lootedBarrowChest") == null)
                {
                    takeEarthQuakeDamageEvent.stop();
                    return;
                }
                p.setLastGraphics(new Graphics(405));
                p.setLastAnimation(new Animation(p.getDefenceAnimation()));
                p.hit(7 + Misc.random(15)); //take 7-15 damage
                takeEarthQuakeDamageEvent.setTick(8000 + Misc.random(20000)); //reset damage time to 8-20 seconds.
            });
            Server.registerEvent(takeEarthQuakeDamageEvent);
        }

        public static void killBrother(Player p, int id)
        {
            for (int i = 0; i < BROTHER_ID.Length; i++)
            {
                if (id == BROTHER_ID[i])
                {
                    p.setBarrowBrothersKilled(i, true);
                    p.setBarrowKillCount(p.getBarrowKillCount() + 1);
                    p.getPackets().modifyText("Kill Count: " + p.getBarrowKillCount(), 24, 0);
                    break;
                }
            }
        }

        public static bool openTunnelDoor(Player player, int doorId, int x, int y)
        {
            if (doorId < 6716 || (doorId > 6731 && doorId < 6735) || doorId > 6750)
            {
                return false;
            }
            int index = getDoorIndex(doorId, x, y);
            int index2 = getOtherDoor(x, y); // index of the door next to the one you clicked.
            if (index == -1 || index2 == -1)
            {
                return false;
            }
            bool betweenDoors = player.getTemporaryAttribute("betweenDoors") != null;
            Location clickedDoor = new Location(DOOR_LOCATION[index][0], DOOR_LOCATION[index][1], 0);
            Location otherDoor = new Location(DOOR_LOCATION[index2][0], DOOR_LOCATION[index2][1], 0);
            int openDoorId = DOOR_OPEN_DIRECTION[index][0];
            int openDoorId2 = DOOR_OPEN_DIRECTION[index2][0];
            int openDirection = DOOR_OPEN_DIRECTION[index][2];
            int newX = openDirection == 1 ? x + 1 : openDirection == 2 ? x : openDirection == 3 ? x - 1 : openDirection == 4 ? x : x;
            int newY = openDirection == 1 ? y : openDirection == 2 ? y + 1 : openDirection == 3 ? y : openDirection == 4 ? y - 1 : y;
            int newX2 = openDirection == 1 ? DOOR_LOCATION[index2][0] + 1 : openDirection == 2 ? DOOR_LOCATION[index2][0] : openDirection == 3 ? DOOR_LOCATION[index2][0] - 1 : openDirection == 4 ? DOOR_LOCATION[index2][0] : DOOR_LOCATION[index2][0];
            int newY2 = openDirection == 1 ? DOOR_LOCATION[index2][1] : openDirection == 2 ? DOOR_LOCATION[index2][1] + 1 : openDirection == 3 ? DOOR_LOCATION[index2][1] : openDirection == 4 ? DOOR_LOCATION[index2][1] - 1 : DOOR_LOCATION[index2][1];
            int[] doorStandCoordinates = getDoorCoordinates(player, index, index2, betweenDoors);
            int[] walkDirections = getWalkDirections(player, index, index2, betweenDoors);
            player.setFaceLocation(clickedDoor);
            AreaEvent doorsWalkAreaEvent = new AreaEvent(player, doorStandCoordinates[0], doorStandCoordinates[1], doorStandCoordinates[2] + 1, doorStandCoordinates[3] + 1);
            doorsWalkAreaEvent.setAction(() =>
            {
                player.setTemporaryAttribute("unmovable", true);

                Event forceWalkDoorEvent = new Event(800);
                forceWalkDoorEvent.setAction(() =>
                {
                    player.getWalkingQueue().resetWalkingQueue();
                    foreach (Player p in Server.getPlayerList())
                    { //change door for all logged in players? uhh what?
                        p.getPackets().removeObject(clickedDoor, openDoorId == 6713 ? 4 : 3, 0);
                        p.getPackets().removeObject(otherDoor, openDoorId2 == 6732 ? 3 : 4, 0);
                        p.getPackets().createObject(openDoorId, new Location(newX, newY, 0), DOOR_OPEN_DIRECTION[index][1], 0);
                        p.getPackets().createObject(openDoorId2, new Location(newX2, newY2, 0), DOOR_OPEN_DIRECTION[index2][1], 0);
                    }
                    player.getWalkingQueue().forceWalk(walkDirections[0], walkDirections[1]);
                    forceWalkDoorEvent.stop();
                });
                Server.registerEvent(forceWalkDoorEvent);
                Event betweenDoorsEvent = new Event(betweenDoors ? 2200 : 1900);
                betweenDoorsEvent.setAction(() =>
                {
                    int face = openDirection == 3 ? 0 : openDirection == 4 ? 3 : openDirection == 2 ? 1 : 2;
                    foreach (Player p in Server.getPlayerList())
                    {
                        p.getPackets().removeObject(new Location(newX, newY, 0), openDoorId == 6713 ? 4 : 3, 0);
                        p.getPackets().removeObject(new Location(newX2, newY2, 0), openDoorId2 == 6732 ? 3 : 4, 0);
                        p.getPackets().createObject(DOORS[index], clickedDoor, face, 0);
                        p.getPackets().createObject(DOORS[index2], otherDoor, face, 0);
                    }
                    player.removeTemporaryAttribute("unmovable");
                    if (!betweenDoors)
                    {
                        player.getPackets().sendConfig(1270, 1);
                        player.setTemporaryAttribute("betweenDoors", true);
                    }
                    else
                    {
                        player.getPackets().sendConfig(1270, 0);
                        player.removeTemporaryAttribute("betweenDoors");
                    }
                    betweenDoorsEvent.stop();
                });
                Server.registerEvent(betweenDoorsEvent);
            });
            Server.registerCoordinateEvent(doorsWalkAreaEvent);
            return true;
        }

        private static int[] getWalkDirections(Player p, int index, int index2, bool betweenDoors)
        {
            int openDirection = DOOR_OPEN_DIRECTION[index][2];
            int[] direction = new int[2];
            if (openDirection == 0)
            {
                /*Nothing*/
            }
            else if (openDirection == 1)
            { // doors open east.
                direction[0] = betweenDoors ? +1 : -1;
                direction[1] = 0;
            }
            else if (openDirection == 2)
            { // doors open north.
                direction[0] = 0;
                direction[1] = betweenDoors ? +1 : -1;
            }
            else if (openDirection == 3)
            { // doors open west.
                direction[0] = betweenDoors ? -1 : +1;
                direction[1] = 0;
            }
            else if (openDirection == 4)
            { // doors open south.
                direction[0] = 0;
                direction[1] = betweenDoors ? -1 : +1;
            }
            return direction;
        }

        /**
         * Returns the coordinates a player must be stood on to use a door, this varies
         * due to the direction of the doors and which side of the door you are on.
         */

        private static int[] getDoorCoordinates(Player p, int index, int index2, bool betweenDoors)
        {
            int openDirection = DOOR_OPEN_DIRECTION[index][2];
            int doorX = DOOR_LOCATION[index][0];
            int doorY = DOOR_LOCATION[index][1];
            int otherDoorX = DOOR_LOCATION[index2][0];
            int otherDoorY = DOOR_LOCATION[index2][1];
            int[] coordinates = new int[4];
            coordinates[0] = getLowest(doorX, otherDoorX);
            coordinates[1] = getLowest(doorY, otherDoorY);
            coordinates[2] = getHighest(doorX, otherDoorX);
            coordinates[3] = getHighest(doorY, otherDoorY);
            if (!betweenDoors)
            { // Player isn't between doors, and is 1 coord from the door.
                if (openDirection == 3)
                {
                    coordinates[0] -= 1;
                }
                else if (openDirection == 4)
                {
                    coordinates[1] -= 1;
                }
            }
            return coordinates;
        }

        public static void prayerDrainEvent(Player p)
        {
            Event prayerDrainEvent = new Event(5000 + Misc.random(5000));
            prayerDrainEvent.setAction(() =>
            {
                if (p.getTemporaryAttribute("atBarrows") == null)
                {
                    prayerDrainEvent.stop();
                    return;
                }
                int currentPrayer = p.getSkills().getCurLevel(Skills.SKILL.PRAYER);
                int maxLevel = p.getSkills().getMaxLevel(Skills.SKILL.PRAYER);
                int levelBy10 = currentPrayer - (maxLevel / 6);
                if (currentPrayer > 0)
                {
                    p.getSkills().setCurLevel(Skills.SKILL.PRAYER, levelBy10);
                    p.getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
                }
                int[] array = p.getLocation().getZ() == 0 ? HEADS[1] : HEADS[0];
                int head = array[Misc.random(array.Length - 1)];
                int slot = Misc.random(5);
                if (slot == 0)
                {
                    slot = 1;
                }
                p.getPackets().itemOnInterface(24, slot, 100, head);
                p.getPackets().animateInterface(2085, 24, slot);
                prayerDrainEvent.setTick(5000 + Misc.random(15000));

                Event animateEvent = new Event(4000);
                animateEvent.setAction(() =>
                {
                    p.getPackets().itemOnInterface(24, slot, 100, -1);
                    p.getPackets().animateInterface(-1, 24, slot);
                    animateEvent.stop();
                });
                Server.registerEvent(animateEvent);
            });
            Server.registerEvent(prayerDrainEvent);
        }

        private static int getLowest(int i, int j)
        {
            return i > j ? j : i;
        }

        private static int getHighest(int i, int j)
        {
            return i > j ? i : j;
        }

        private static int getOtherDoor(int x, int y)
        {
            for (int i = 0; i < DOOR_LOCATION.Length; i++)
            {
                if ((x == DOOR_LOCATION[i][0] && y + 1 == DOOR_LOCATION[i][1]) ||
                    (x + 1 == DOOR_LOCATION[i][0] && y == DOOR_LOCATION[i][1]) ||
                    (x == DOOR_LOCATION[i][0] && y - 1 == DOOR_LOCATION[i][1]) ||
                    (x - 1 == DOOR_LOCATION[i][0] && y == DOOR_LOCATION[i][1]))
                {
                    return i;
                }
            }
            return -1;
        }

        private static int getDoorIndex(int doorId, int x, int y)
        {
            for (int i = 0; i < DOORS.Length; i++)
            {
                if (doorId == DOORS[i])
                {
                    if (x == DOOR_LOCATION[i][0] && y == DOOR_LOCATION[i][1])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static bool betweenDoors(Player p)
        {
            for (int i = 0; i < DB.Length; i++)
            {
                if (p.getLocation().inArea(DB[i][0], DB[i][1], DB[i][2], DB[i][3]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}