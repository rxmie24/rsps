using RS2.Server.player;

namespace RS2.Server.events
{
    internal class SkullCycleEvent : Event
    {
        public SkullCycleEvent()
            : base(60000) { }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null)
                {
                    if (p.isSkulled() && !p.isDead())
                        p.setSkullCycles(p.getSkullCycles() - 1);
                }
            }
        }
    }
}