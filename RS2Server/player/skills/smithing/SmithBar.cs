namespace RS2.Server.player.skills.smithing
{
    internal class SmithBar
    {
        private int barType;
        private int barAmount;
        private int level;
        private double xp;
        private int finishedItem;
        private int finishedItemAmount;
        private int amount;

        public SmithBar(int barType, int barAmount, int level, double xp, int finishedItem, int finishedItemAmount, int amount)
        {
            this.barType = barType;
            this.barAmount = barAmount;
            this.level = level;
            this.xp = xp;
            this.finishedItem = finishedItem;
            this.finishedItemAmount = finishedItemAmount;
            this.amount = amount;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }

        public int getBarType()
        {
            return barType;
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }

        public int getBarAmount()
        {
            return barAmount;
        }

        public int getLevel()
        {
            return level;
        }

        public double getXp()
        {
            return xp;
        }

        public int getFinishedItem()
        {
            return finishedItem;
        }

        public int getFinishedItemAmount()
        {
            return finishedItemAmount;
        }

        public int getAmount()
        {
            return amount;
        }
    }
}