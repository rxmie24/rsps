namespace RS2.Server.player.skills.crafting
{
    internal class CraftItem
    {
        private int craftType;
        private int craftItem;
        private string message;
        private double xp;
        private int level;
        private int finishedItem;
        private int amount;

        public CraftItem(int craftType, int craftItem, int amount, double xp, int finishedItem, string message, int level)
        {
            this.craftType = craftType;
            this.craftItem = craftItem;
            this.amount = amount;
            this.xp = xp;
            this.finishedItem = finishedItem;
            this.message = message;
            this.level = level;
        }

        public int getAmount()
        {
            return amount;
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }

        public int getCraftType()
        {
            return craftType;
        }

        public int getCraftItem()
        {
            return craftItem;
        }

        public string getMessage()
        {
            return message;
        }

        public double getXp()
        {
            return xp;
        }

        public int getLevel()
        {
            return level;
        }

        public int getFinishedItem()
        {
            return finishedItem;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }
    }
}