namespace RS2.Server.player.skills.fletching
{
    internal class Ammo
    {
        private int finishedItem;
        private int itemOne;
        private int itemTwo;
        private double xp;
        private int level;
        private int itemType;
        private int amount;
        private bool bolt;

        public Ammo(int finished, int one, int two, double xp, int level, int itemType, bool isBolt, int amount)
        {
            this.finishedItem = finished;
            this.itemOne = one;
            this.itemTwo = two;
            this.xp = xp;
            this.level = level;
            this.itemType = itemType;
            bolt = isBolt;
            this.amount = amount;
        }

        public int getAmount()
        {
            return amount;
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }

        public int getFinishedItem()
        {
            return finishedItem;
        }

        public int getItemOne()
        {
            return itemOne;
        }

        public int getItemTwo()
        {
            return itemTwo;
        }

        public double getXp()
        {
            return xp;
        }

        public int getLevel()
        {
            return level;
        }

        public int getItemType()
        {
            return itemType;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }

        public bool isBolt()
        {
            return bolt;
        }
    }
}