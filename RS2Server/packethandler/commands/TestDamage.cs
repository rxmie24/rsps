using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class TestDamage : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Test Damage command]: ::testdmg dmg  ::testdmg 2000");
                return;
            }

            int dmg = 0;
            Hits.Hit hit;
            if (!int.TryParse(arguments[0], out dmg))
            {
                dmg = 0;
                hit = new Hits.Hit(dmg, Hits.HitType.NO_DAMAGE);
            }
            else
            {
                hit = new Hits.Hit(dmg, Hits.HitType.NORMAL_DAMAGE);
            }

            player.getHits().setHit1(hit);
            player.getUpdateFlags().setHitUpdateRequired(true);
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}