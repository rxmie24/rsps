using RS2.Server.player;

namespace RS2.Server.events
{
    internal class SystemUpdateEvent : Event
    {
        private int time = 180;

        public SystemUpdateEvent()
            : base(1000) { }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    p.getPackets().newSystemUpdate(time);
                }
            }
            if (time-- <= 0)
            {
                this.stop();
                foreach (Player p in Server.getPlayerList())
                {
                    p.getPackets().forceLogout();
                }
            }
        }
    }
}