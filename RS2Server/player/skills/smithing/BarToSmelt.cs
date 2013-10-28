namespace RS2.Server.player.skills.smithing
{
    internal class BarToSmelt
    {
        private int index;
        private int barId;
        private int level;
        private double xp;
        private int amount;
        private int[] ores;
        private int[] oreAmount;
        private string name;

        public BarToSmelt(int index, int barId, int level, double xp, int amount, int[] ore, int[] oreAmount, string name)
        {
            this.index = index;
            this.barId = barId;
            this.level = level;
            this.xp = xp;
            this.amount = amount;
            this.ores = ore;
            this.oreAmount = oreAmount;
            this.name = name;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }

        public int getIndex()
        {
            return index;
        }

        public int getBarId()
        {
            return barId;
        }

        public int getLevel()
        {
            return level;
        }

        public double getXp()
        {
            return xp;
        }

        public int getAmount()
        {
            return amount;
        }

        public int[] getOre()
        {
            return ores;
        }

        public int[] getOreAmount()
        {
            return oreAmount;
        }

        public void setAmount(int i)
        {
            this.amount = i;
        }

        public string getName()
        {
            return name;
        }
    }
}