using RS2.Server.model;

namespace RS2.Server.player.skills.fishing
{
    internal class Spot
    {
        private int[] fish;
        private int[] level;
        private int spotindex;
        private int spotId;
        private double[] fishingXp;
        private Location spotLocation;
        private int secondaryItem;
        private int primaryItem;
        private string primaryName;
        private string secondaryName;
        private bool secondOption;

        public Spot(int[] spotFish, int[] spotLevel, int spotindex, int spotId, double[] spotXp, Location spotLocation, int primaryItem, int secondaryItem, string primaryName, string secondaryName, bool secondOption)
        {
            this.fish = spotFish;
            this.level = spotLevel;
            this.spotindex = spotindex;
            this.spotId = spotId;
            this.fishingXp = spotXp;
            this.spotLocation = spotLocation;
            this.secondaryItem = secondaryItem;
            this.primaryItem = primaryItem;
            this.primaryName = primaryName;
            this.secondaryName = secondaryName;
            this.secondOption = secondOption;
        }

        public int getSpotId()
        {
            return spotId;
        }

        public double[] getFishingXp()
        {
            return fishingXp;
        }

        public Location getSpotLocation()
        {
            return spotLocation;
        }

        public int[] getFish()
        {
            return fish;
        }

        public int[] getLevel()
        {
            return level;
        }

        public int getSpotindex()
        {
            return spotindex;
        }

        public int getSecondaryItem()
        {
            return secondaryItem;
        }

        public int getPrimaryItem()
        {
            return primaryItem;
        }

        public string getPrimaryName()
        {
            return primaryName;
        }

        public string getSecondaryName()
        {
            return secondaryName;
        }

        public bool isSecondOption()
        {
            return secondOption;
        }
    }
}