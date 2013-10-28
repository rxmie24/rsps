namespace RS2.Server.player.skills.herblore
{
    internal class Potion
    {
        private int unfinished;
        private int finished;
        private int secondary;
        private int level;
        private double xp;
        private int amount;

        public Potion(int finished, int unfinished, int secondary, int level, double xp, int amount)
        {
            this.finished = finished;
            this.unfinished = unfinished;
            this.secondary = secondary;
            this.level = level;
            this.xp = xp;
            this.amount = amount;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }

        public int getUnfinished()
        {
            return unfinished;
        }

        public int getFinished()
        {
            return finished;
        }

        public int getSecondary()
        {
            return secondary;
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
    }
}