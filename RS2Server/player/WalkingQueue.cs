using RS2.Server.model;
using System;

namespace RS2.Server.player
{
    internal class WalkingQueue
    {
        public struct Waypoint
        {
            public int x;
            public int y;
            public int dir;

            public Waypoint(int x, int y, int dir)
            {
                this.x = x;
                this.y = y;
                this.dir = dir;
            }
        }

        public static sbyte[] DIRECTION_DELTA_X = new sbyte[] { -1, 0, 1, -1, 1, -1, 0, 1 };
        public static sbyte[] DIRECTION_DELTA_Y = new sbyte[] { 1, 1, 1, 0, 0, -1, -1, -1 };

        public static int MAX_WALKING_WAYPOINTS = 50;
        public Waypoint[] walkingQueue = new Waypoint[MAX_WALKING_WAYPOINTS];
        private int waypointWriteOffset = 0; //location where it can write next waypoint in array.
        private int waypointReadOffset = 0; //location where it is currently reading the waypoint in array.
        private Player player;

        private bool running = false;
        private bool runToggled = false;

        private int lastDirection;

        public WalkingQueue(Player player)
        {
            this.player = player;
            for (int i = 0; i < MAX_WALKING_WAYPOINTS; i++)
                walkingQueue[i] = new Waypoint(0, 0, -1);
            this.lastDirection = 6;
            resetWalkingQueue();
        }

        public bool isRunToggled()
        {
            return runToggled;
        }

        public void setRunToggled(bool runToggled)
        {
            this.runToggled = runToggled;
        }

        public void setIsRunning(bool running)
        {
            this.running = running;
        }

        public bool isRunning()
        {
            return running;
        }

        public void setLastDirection(int lastDirection)
        {
            this.lastDirection = lastDirection;
        }

        public int getLastDirection()
        {
            return lastDirection;
        }

        public void addToWalkingQueue(int x, int y)
        {
            int diffX = x - walkingQueue[waypointWriteOffset - 1].x;
            int diffY = y - walkingQueue[waypointWriteOffset - 1].y;
            int maxStepDistance = Math.Max(Math.Abs(diffX), Math.Abs(diffY));

            for (int i = 0; i < maxStepDistance; i++)
            {
                if (diffX < 0)
                    diffX++;
                else if (diffX > 0)
                    diffX--;

                if (diffY < 0)
                    diffY++;
                else if (diffY > 0)
                    diffY--;

                addStepToWalkingQueue(x - diffX, y - diffY);
            }
        }

        public void addStepToWalkingQueue(int x, int y)
        {
            int diffX = x - walkingQueue[waypointWriteOffset - 1].x;
            int diffY = y - walkingQueue[waypointWriteOffset - 1].y;
            int dir = direction(diffX, diffY);
            if (waypointWriteOffset >= MAX_WALKING_WAYPOINTS)
            {
                return;
            }
            if (dir != -1)
            {
                walkingQueue[waypointWriteOffset].x = x;
                walkingQueue[waypointWriteOffset].y = y;
                walkingQueue[waypointWriteOffset++].dir = dir;
            }
        }

        public void resetWalkingQueue()
        {
            walkingQueue[0].x = player.getLocation().getLocalX();
            walkingQueue[0].y = player.getLocation().getLocalY();
            walkingQueue[0].dir = -1;
            waypointReadOffset = waypointWriteOffset = 1;
        }

        public void forceWalk(int x, int y)
        {
            resetWalkingQueue();
            addToWalkingQueue(player.getLocation().getLocalX() + x, player.getLocation().getLocalY() + y);
        }

        public bool hasNextStep()
        {
            return waypointReadOffset < waypointWriteOffset;
        }

        private int getNextWalkingDirection()
        {
            if (waypointReadOffset == waypointWriteOffset)
            {
                return -1;
            }
            int dir = walkingQueue[waypointReadOffset++].dir;
            if (dir == -1) return -1;
            int xdiff = WalkingQueue.DIRECTION_DELTA_X[dir];
            int ydiff = WalkingQueue.DIRECTION_DELTA_Y[dir];
            player.getLocation().setX(player.getLocation().getX() + xdiff);
            player.getLocation().setY(player.getLocation().getY() + ydiff);
            lastDirection = dir;
            return dir;
        }

        public void getNextPlayerMovement()
        {
            //Does a walking action such as new player movement direction (if any) or new coordinates when teleporting.
            player.getUpdateFlags().setDidMapRegionChange(false);
            player.getUpdateFlags().setTeleporting(false);
            player.getSprites().setSprites(-1, -1);

            if (player.getTeleportTo() != null)
            {
                player.getUpdateFlags().setDidMapRegionChange(true);

                Location lastRegion = player.getUpdateFlags().getLastRegion();
                if (lastRegion != null)
                {
                    int rX = player.getTeleportTo().getLocalX(lastRegion);
                    int rY = player.getTeleportTo().getLocalY(lastRegion);
                    if (rX >= 2 * 8 && rX < 11 * 8 && rY >= 2 * 8 && rY < 11 * 8)
                        player.getUpdateFlags().setDidMapRegionChange(false);
                }
                if (player.getUpdateFlags().didMapRegionChange())
                    player.getUpdateFlags().setLastRegion(player.getTeleportTo());

                player.setLocation(player.getTeleportTo());

                player.setDistanceEvent(null);
                resetWalkingQueue();
                player.getUpdateFlags().setTeleporting(true);
                player.resetTeleportTo();
            }
            else
            {
                if (player.getUpdateFlags().getLastRegion() == null)
                    return;
                Location oldLocation = (Location)player.getLocation().Clone();
                int walkDir = getNextWalkingDirection();
                int runDir = -1;
                if (running || runToggled)
                {
                    if (player.getRunEnergy() > 0)
                    {
                        runDir = getNextWalkingDirection();
                        if (runDir != -1)
                            player.setRunEnergy(player.getRunEnergy() - 1);
                    }
                    else
                    {
                        if (runToggled)
                        {
                            player.getPackets().sendConfig(173, 0);
                            runToggled = running = false;
                        }
                        running = false;
                    }
                }

                Location lastRegion = player.getUpdateFlags().getLastRegion();

                int rX = oldLocation.getLocalX(lastRegion);
                int rY = oldLocation.getLocalY(lastRegion);

                if ((rX < 2 * 8 || rX >= 11 * 8 || rY < 2 * 8 || rY >= 11 * 8))
                {
                    player.getUpdateFlags().setDidMapRegionChange(true);
                    if (walkDir != -1) waypointReadOffset--;
                    if (runDir != -1) waypointReadOffset--;
                    walkDir = -1;
                    runDir = -1;
                    player.setLocation(oldLocation);
                }
                player.getSprites().setSprites(walkDir, runDir);
            }
        }

        public static int direction(int dx, int dy)
        {
            if (dx < 0)
            {
                if (dy < 0)
                    return 5; //south-west
                else if (dy > 0)
                    return 0; //north-west
                else
                    return 3; //west
            }
            else if (dx > 0)
            {
                if (dy < 0)
                    return 7; //south-east
                else if (dy > 0)
                    return 2; //north-east
                else
                    return 4; //east
            }
            else
            {
                if (dy < 0)
                    return 6; //south
                else if (dy > 0)
                    return 1; //north
                else
                    return -1; //no change.
            }
        }
    }
}