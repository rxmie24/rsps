using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.magic
{
    internal class Lunar
    {
        public Lunar()
        {
        }

        public static void castLunarSpell(Player p, int id)
        {
            if (p.getMagicType() != 3)
            {
                return;
            }
            switch (id)
            {
                case 14: // Vengeance:
                    vengeance(p, id);
                    break;
            }
        }

        private static void vengeance(Player p, int id)
        {
            if (id == 14)
            { // Normal vengeance
                if (!p.getInventory().hasItemAmount(560, 2) || !p.getInventory().hasItemAmount(557, 10) || !p.getInventory().hasItemAmount(9075, 4))
                {
                    p.getPackets().sendMessage("You do not have enough runes to cast Vengeance!");
                    return;
                }
                if (p.hasVengeance())
                {
                    p.getPackets().sendMessage("You have already filled yourself with vengeance.");
                    return;
                }
                else
                {
                    if (Environment.TickCount - p.getLastVengeanceTime() <= 30000)
                    {
                        p.getPackets().sendMessage("You cannot cast this spell yet.");
                        return;
                    }
                }
                p.getInventory().deleteItem(560, 2);
                p.getInventory().deleteItem(557, 10);
                p.getInventory().deleteItem(9075, 4);
                p.setLastAnimation(new Animation(4410));
                p.setLastGraphics(new Graphics(726, 0, 80));
                p.setLastVengeanceTime(Environment.TickCount);
                p.setVengeance(true);
            }
        }
    }
}