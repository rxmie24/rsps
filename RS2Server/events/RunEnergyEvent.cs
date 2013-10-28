using RS2.Server.player;

namespace RS2.Server.events
{
    internal class RunEnergyEvent : Event
    {
        public RunEnergyEvent()
            : base(2000) { }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p == null) continue; //this happens on events, pretty frequently
                if ((p.getWalkingQueue().isRunToggled() || p.getWalkingQueue().isRunning()) && p.getSprites().getSecondarySprite() != -1)
                {
                    continue;
                }
                else
                {
                    if (p.getRunEnergy() < 100)
                    {
                        p.setRunEnergy(p.getRunEnergy() + 1);
                    }
                }
            }
        }
    }
}