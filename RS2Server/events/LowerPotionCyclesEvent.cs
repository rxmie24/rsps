using RS2.Server.player;

namespace RS2.Server.events
{
    internal class LowerPotionCyclesEvent : Event
    {
        public LowerPotionCyclesEvent()
            : base(15000) { }

        public override void runAction()
        {
            foreach (Player p in Server.getPlayerList())
            {
                if (p != null && !p.isDead())
                {
                    int antifireCycle = p.getAntifireCycles();
                    int antipoisonCycle = p.getSuperAntipoisonCycles();
                    if (antifireCycle > 0)
                    {
                        if (antifireCycle == 2)
                        {
                            p.getPackets().sendMessage("Your resistance to dragonfire is about to run out!");
                        }
                        else if (antifireCycle == 1)
                        {
                            p.getPackets().sendMessage("Your resistance to dragonfire has run out!");
                        }
                        p.setAntifireCycles(antifireCycle - 1);
                    }
                    if (antipoisonCycle > 0)
                    {
                        p.setSuperAntipoisonCycles(antipoisonCycle - 1);
                    }
                }
            }
        }
    }
}