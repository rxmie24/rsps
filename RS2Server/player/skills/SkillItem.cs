namespace RS2.Server.player.skills
{
    internal class SkillItem
    {
        private int finishedItem;
        private int itemOne;
        private int itemTwo;
        private int level;
        private int skill;
        private double xp;
        private int amount;

        public SkillItem(int finished, int itemOne, int itemTwo, int level, int skill, double xp, int amount)
        {
            this.finishedItem = finished;
            this.itemOne = itemOne;
            this.itemTwo = itemTwo;
            this.level = level;
            this.skill = skill;
            this.xp = xp;
            this.amount = amount;
        }

        public void decreaseAmount()
        {
            this.amount = amount - 1;
        }

        public int getAmount()
        {
            return amount;
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

        public int getLevel()
        {
            return level;
        }

        public int getSkill()
        {
            return skill;
        }

        public double getXp()
        {
            return xp;
        }
    }
}