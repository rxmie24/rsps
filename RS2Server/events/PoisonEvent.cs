using RS2.Server.model;
using RS2.Server.player;
using RS2.Server.util;

namespace RS2.Server.events
{
    internal class PoisonEvent : Event
    {
        private Entity target;

        public PoisonEvent(Entity target, int poisonAmount)
            : base(30000 + Misc.random(30000))
        {
            this.target = target;
            initialize(poisonAmount);
        }

        private void initialize(int poisonAmount)
        {
            if (target is Player)
            {
                if (((Player)target).getSuperAntipoisonCycles() > 0)
                {
                    stop();
                    return;
                }
                ((Player)target).getPackets().sendMessage("You have been poisoned!");
            }
            target.setPoisonAmount(poisonAmount);
        }

        public override void runAction()
        {
            if (!target.isPoisoned() || target.isDead())
            {
                stop();
                return;
            }
            if (target is Player)
            {
                ((Player)target).getPackets().closeInterfaces();
            }
            target.hit(target.getPoisonAmount(), Hits.HitType.POISON_DAMAGE);
            if (Misc.random(200) >= 100)
            {
                target.setPoisonAmount(target.getPoisonAmount() - 1);
            }
            if (Misc.random(10) == 0)
            {
                target.setPoisonAmount(0);
                stop();
                return;
            }
            setTick(30000 + Misc.random(30000));
        }
    }
}