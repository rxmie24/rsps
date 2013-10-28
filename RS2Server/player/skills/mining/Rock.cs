using RS2.Server.model;

namespace RS2.Server.player.skills.mining
{
    internal class Rock
    {
        private int rockIndex;
        private Location rockLocation;
        private int ore;
        private int level;
        private double xp;
        private ushort rockId;
        private bool continueMine;
        private string name;

        public Rock(int index, ushort rockId, Location loc, int ore, int level, string name, double xp)
        {
            this.rockIndex = index;
            this.rockLocation = loc;
            this.ore = ore;
            this.rockId = rockId;
            this.level = level;
            this.xp = xp;
            this.name = name;
            this.continueMine = index == 0 || index == 9;
        }

        public ushort getRockId()
        {
            return rockId;
        }

        public double getXp()
        {
            return xp;
        }

        public int getOre()
        {
            return ore;
        }

        public int getLevel()
        {
            return level;
        }

        public bool isContinueMine()
        {
            return continueMine;
        }

        public int getRockIndex()
        {
            return rockIndex;
        }

        public Location getRockLocation()
        {
            return rockLocation;
        }

        public string getName()
        {
            return name;
        }
    }
}