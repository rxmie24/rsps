using RS2.Server.model;

namespace RS2.Server.player.skills.woodcutting
{
    internal class Tree
    {
        private int treeIndex;
        private Location treeLocation;
        private int log;
        private int level;
        private double xp;
        private ushort treeId;
        private string name;
        private int distance;

        public Tree(int index, ushort treeId, Location loc, int log, int level, string name, double xp, int distance)
        {
            this.treeIndex = index;
            this.treeLocation = loc;
            this.log = log;
            this.treeId = treeId;
            this.level = level;
            this.xp = xp;
            this.name = name;
            this.distance = distance;
        }

        public int getTreeIndex()
        {
            return treeIndex;
        }

        public Location getTreeLocation()
        {
            return treeLocation;
        }

        public int getLog()
        {
            return log;
        }

        public int getLevel()
        {
            return level;
        }

        public double getXp()
        {
            return xp;
        }

        public ushort getTreeId()
        {
            return treeId;
        }

        public string getName()
        {
            return name;
        }

        public int getDistance()
        {
            return distance;
        }
    }
}