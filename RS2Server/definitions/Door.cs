using RS2.Server.model;

namespace RS2.Server.definitions
{
    public class Door
    {
        public enum DefaultStatus
        {
            OPEN,
            CLOSED
        }

        public DefaultStatus defaultStatus; //public for deserialization process
        public int openDoorId;
        public int closedDoorId;
        public Location closedDoorLocation;
        public Location openDoorLocation;
        public int openDirection;
        public int closedDirection;
        public bool closable;
        private bool instantClose;
        private bool blockForOtherPlayers; // Used for things like warrior guild/quest doors
        private bool doorOpen;
        private long lastChangeTime;

        public Door()
        {
        }

        public Location getOpenDoorLocation()
        {
            return openDoorLocation;
        }

        public bool isInstantClose()
        {
            return instantClose;
        }

        public bool isBlockForOtherPlayers()
        {
            return blockForOtherPlayers;
        }

        public int getOpenDoorId()
        {
            return openDoorId;
        }

        public int getClosedDoorId()
        {
            return closedDoorId;
        }

        public int getOpenDirection()
        {
            return openDirection;
        }

        public int getClosedDirection()
        {
            return closedDirection;
        }

        public void setDoorOpen(bool doorOpen)
        {
            this.doorOpen = doorOpen;
        }

        public bool isDoorOpen()
        {
            return doorOpen;
        }

        public void setClosedDoorLocation(Location closedDoorLocation)
        {
            this.closedDoorLocation = closedDoorLocation;
        }

        public Location getClosedDoorLocation()
        {
            return closedDoorLocation;
        }

        public Location getDoorLocation()
        {
            return doorOpen ? openDoorLocation : closedDoorLocation;
        }

        public void setLastChangeTime(long lastChangeTime)
        {
            this.lastChangeTime = lastChangeTime;
        }

        public long getLastChangeTime()
        {
            return lastChangeTime;
        }

        public bool isClosable()
        {
            return closable;
        }

        public DefaultStatus getDefaultStatus()
        {
            return defaultStatus;
        }
    }
}