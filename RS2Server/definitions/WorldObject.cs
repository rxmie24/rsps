using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class WorldObject
    {
        private bool spawnedObject;
        private ushort originalId;
        private int secondaryId;
        private Location location;
        private int restoreDelay;
        private int objectHealth;
        private bool secondForm;
        private int face;
        private Player owner;
        private int objectType;
        private int sizeX;
        private int sizeY;
        private int type;
        private bool deleteObject;

        public WorldObject(ushort originalId, int secondaryId, int face, Location location, int delay, int health)
        {
            this.originalId = originalId;
            this.secondaryId = secondaryId;
            this.location = location;
            this.face = face;
            this.restoreDelay = delay;
            this.objectHealth = health;
            this.secondForm = false;
            this.type = 10; // default
            this.objectType = 0;
        }

        public WorldObject(Location location, int face, int type, bool deleteObject)
        {
            this.location = location;
            this.face = face;
            this.type = type; // default
            this.objectType = 0;
            this.deleteObject = deleteObject;
        }

        /*
         * Used for fires
         */

        public WorldObject(ushort id, Location location, int objectType)
        {
            this.originalId = id;
            this.location = location;
            this.face = 0;
            this.type = 10;
            this.objectType = objectType;
        }

        public WorldObject(ushort id, Location location, int face, int type)
        {
            this.originalId = id;
            this.location = location;
            this.face = face;
            this.type = type;
            this.objectType = 0;
        }

        public WorldObject(ushort id, int secondary, Location location, int face, int type)
        {
            this.originalId = id;
            this.secondaryId = secondary;
            this.location = location;
            this.face = face;
            this.type = type;
            this.objectType = 0;
        }

        public WorldObject(ushort id, Location location, int face, int type, bool spawned)
        {
            this.originalId = id;
            this.location = location;
            this.face = face;
            this.type = type;
            this.objectType = 0;
            this.spawnedObject = true;
        }

        public int getFace()
        {
            return face;
        }

        public bool isSecondForm()
        {
            return secondForm;
        }

        public void setSecondForm(bool secondForm)
        {
            this.secondForm = secondForm;
        }

        public ushort getOriginalId()
        {
            return originalId;
        }

        public int getSecondaryId()
        {
            return secondaryId;
        }

        public Location getLocation()
        {
            return location;
        }

        public int getRestoreDelay()
        {
            return restoreDelay;
        }

        public void setObjectHealth(int objectHealth)
        {
            this.objectHealth = objectHealth;
        }

        public int getObjectHealth()
        {
            return objectHealth;
        }

        public void setOwner(Player owner)
        {
            this.owner = owner;
        }

        public Player getOwner()
        {
            return owner;
        }

        public void setFire(bool isFire)
        {
            this.objectType = 1;
        }

        public bool isFire()
        {
            return objectType == 1;
        }

        public bool isCannon()
        {
            return objectType == 2;
        }

        public void setSizeX(int sizeX)
        {
            this.sizeX = sizeX;
        }

        public int getSizeX()
        {
            return sizeX;
        }

        public void setSizeY(int sizeY)
        {
            this.sizeY = sizeY;
        }

        public int getSizeY()
        {
            return sizeY;
        }

        public int getType()
        {
            return type;
        }

        public void setType(int type)
        {
            this.type = type;
        }

        public bool isSpawnedObject()
        {
            return spawnedObject;
        }

        public void setSpawnedObject(bool spawned)
        {
            this.spawnedObject = spawned;
        }

        public void setRestore(int restore)
        {
            this.restoreDelay = restore;
        }

        public void setSecondaryId(int secondary)
        {
            this.secondaryId = secondary;
        }

        public bool shouldDeleteObject()
        {
            return deleteObject;
        }
    }
}