namespace RS2.Server.player.skills.fletching
{
    internal class Bow
    {
        private int finishedItem;
        private int logType;
        private double xp;
        private int level;
        private int amount;
        private int itemType;
        private bool stringing;

        public Bow(int finished, int logType, int itemType, double xp, int level, int amount, bool stringing)
        {
            this.finishedItem = finished;
            this.logType = logType;
            this.itemType = itemType;
            this.xp = xp;
            this.level = level;
            this.amount = amount;
            this.stringing = stringing;
        }

        public bool isStringing()
        {
            return stringing;
        }

        public int getAmount()
        {
            return amount;
        }

        public void decreaseAmount()
        {
            this.amount = (amount - 1);
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }

        public int getFinishedItem()
        {
            return finishedItem;
        }

        public int getLogType()
        {
            return logType;
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
    }
}