using System;

namespace RS2.Server.player.skills.farming
{
    public class Patch : FarmingData
    {
        private int status;
        private int index;
        private int configId;
        private int[] configArray;
        private long timeToGrow;
        private long lastUpdate;
        public string ownerName;
        private int seedIndex;
        private int multiplyer;
        private PatchType patchType;
        private bool weeds;
        private bool sapling;
        private bool healthChecked;
        private bool weeding;

        public Patch(string owner, int index)
        {
            this.ownerName = owner;
            this.index = index;
            this.sapling = true;
            this.status = 0;
        }

        public Patch(string owner, PatchType type, int index, int multiplyer, int config)
        {
            this.weeds = true;
            this.status = 0;
            this.lastUpdate = Environment.TickCount;
            this.ownerName = owner;
            this.patchType = type;
            this.index = index;
            this.multiplyer = multiplyer;
            this.configId = config;
        }

        public Patch(string owner, PatchType type, int index, int configId, int[] configArray, long timeToGrow, int crop, int multiplyer)
        {
            this.ownerName = owner;
            this.patchType = type;
            this.index = index;
            this.configId = configId;
            this.configArray = configArray;
            this.timeToGrow = timeToGrow;
            this.multiplyer = multiplyer;
            this.weeds = true;
            this.status = 0;
        }

        public void setStatus(int status)
        {
            this.status = status;
        }

        public int getStatus()
        {
            return status;
        }

        public string getOwnerName()
        {
            return ownerName;
        }

        public int getPatchIndex()
        {
            return index;
        }

        public int getConfigId()
        {
            return configId;
        }

        public void setConfig(int i)
        {
            this.configId = i;
        }

        public int getConfigElement(int status)
        {
            return configArray[status];
        }

        public long getTimeToGrow()
        {
            return timeToGrow;
        }

        public void setTimeToGrow(long time)
        {
            this.timeToGrow = time;
        }

        public int getConfigLength()
        {
            return configArray.Length;
        }

        public bool isFullyGrown()
        {
            if (!patchOccupied())
            {
                return false;
            }
            if (!isFruitTree())
            {
                return status >= configArray.Length - 1;
            }
            return false;
        }

        public int getMultiplyer()
        {
            return multiplyer;
        }

        public PatchType getPatchType()
        {
            return patchType;
        }

        public void setConfigArray(int[] configArray)
        {
            this.configArray = configArray;
        }

        public bool hasWeeds()
        {
            return weeds;
        }

        public void setHasWeeds(bool b)
        {
            this.weeds = b;
        }

        public long getLastUpdate()
        {
            return lastUpdate;
        }

        public void setLastUpdate(long time)
        {
            this.lastUpdate = time;
        }

        public bool patchOccupied()
        {
            return configArray.Length != 3;
        }

        public void setSeedIndex(int seedIndex)
        {
            this.seedIndex = seedIndex;
        }

        public int getSeedIndex()
        {
            return seedIndex;
        }

        public bool isSapling()
        {
            return sapling;
        }

        public bool isFruitTree()
        {
            return patchType.Equals(PatchType.FRUIT_TREE);
        }

        public bool isTree()
        {
            return patchType.Equals(PatchType.TREE);
        }

        public void setHealthChecked(bool healthChecked)
        {
            this.healthChecked = healthChecked;
        }

        public bool isHealthChecked()
        {
            return healthChecked;
        }

        public bool isStump()
        {
            return false;
        }

        public bool isBlankPatch()
        {
            return !weeds && configArray.Length == 3 && status == 2;
        }

        public void setWeeding(bool b)
        {
            this.weeding = b;
        }

        public bool isWeeding()
        {
            return weeding;
        }

        public int checkHealthStatus()
        {
            if (isTree())
            {
                return configArray.Length - 3;
            }
            else if (isFruitTree())
            {
                return configArray.Length - 3;
            }
            return 0;
        }

        public int chopStatus()
        {
            if (isTree())
            {
                return configArray.Length - 2;
            }
            else if (isFruitTree())
            {
                return configArray.Length - 2;
            }
            return 0;
        }

        public int stumpStatus()
        {
            if (isTree())
            {
                return configArray.Length - 1;
            }
            else if (isFruitTree())
            {
                return configArray.Length - 1;
            }
            return 0;
        }
    }
}