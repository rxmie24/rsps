using RS2.Server.player;

namespace RS2.Server.events
{
    internal class AreaEvent : Event
    {
        private int x;
        private int y;
        private int x1;
        private int y2;
        private Player p;

        public AreaEvent(Player player, int x, int y, int x1, int y2)
            : base(0)
        {
            this.p = player;
            this.x = x;
            this.y = y;
            this.x1 = x1;
            this.y2 = y2;
            if (player != null)
            {
                player.setDistanceEvent(this);
            }
        }

        public bool inArea()
        {
            return p.getLocation().inArea(x, y, x1, y2);
        }

        public Player getPlayer()
        {
            return p;
        }

        public void setDistanceEventNull()
        {
            if (p != null)
            {
                p.setDistanceEvent(null);
            }
        }
    }
}