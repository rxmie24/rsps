using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.events
{
    internal class CoordinateEvent : Event
    {
        private Player player;
        private Location location;
        private Location oldLocation;
        private int failedAttempts = 0;
        private bool reached = false;

        public CoordinateEvent(Player player, Location location)
            : base(0)
        {
            this.player = player;
            this.location = location;
            this.oldLocation = location;
            if (player != null)
            {
                player.setDistanceEvent(this);
            }
        }

        public bool hasReached()
        {
            return reached;
        }

        public void setReached(bool reached)
        {
            this.reached = reached;
        }

        public Player getPlayer()
        {
            return player;
        }

        public int getFailedAttempts()
        {
            return failedAttempts;
        }

        public void incrementFailedAttempts()
        {
            failedAttempts++;
        }

        public void setOldLocation(Location location)
        {
            this.oldLocation = location;
        }

        public Location getOldLocation()
        {
            return this.oldLocation;
        }

        public Location getTargetLocation()
        {
            return this.location;
        }

        public void setPlayerNull()
        {
            if (player != null)
            {
                player.setDistanceEvent(null);
            }
        }
    }
}