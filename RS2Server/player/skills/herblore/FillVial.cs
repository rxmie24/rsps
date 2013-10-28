using RS2.Server.events;
using RS2.Server.model;
using System;

namespace RS2.Server.player.skills.herblore
{
    internal class FillVial : HerbloreData
    {
        public FillVial()
        {
        }

        // TODO make this use an AreaEvent so itll work from a distance.
        /**
         * Will fill vials in a continuous motion from a water source.
         */

        public static bool fillingVial(Player p, Location loc)
        {
            if (!p.getInventory().hasItem(VIAL) || !p.getLocation().withinDistance(loc, 2))
            {
                return true;
            }
            if (p.getTemporaryAttribute("fillVialTimer") != null)
            {
                long lastFillTime = (int)p.getTemporaryAttribute("fillVialTimer");
                if (Environment.TickCount - lastFillTime < 600)
                {
                    return true;
                }
            }
            p.setTemporaryAttribute("fillingVials", true);
            p.setFaceLocation(loc);

            Event fillVialEvent = new Event(500);
            fillVialEvent.setAction(() =>
            {
                int amountFilled = 0;
                string s = amountFilled == 1 ? "vial" : "vials";
                if (p.getTemporaryAttribute("fillingVials") == null || !p.getLocation().withinDistance(loc, 2) || !p.getInventory().hasItem(229))
                {
                    p.setLastAnimation(new Animation(65535));
                    if (amountFilled > 0)
                    {
                        p.getPackets().sendMessage("You fill up the " + s + " with water.");
                    }
                    fillVialEvent.stop();
                    return;
                }
                if (p.getInventory().replaceSingleItem(VIAL, VIAL_OF_WATER))
                {
                    p.setLastAnimation(new Animation(832));
                    amountFilled++;
                    p.setTemporaryAttribute("fillVialTimer", Environment.TickCount);
                }
                else
                {
                    if (amountFilled > 0)
                    {
                        p.setLastAnimation(new Animation(65535));
                        p.getPackets().sendMessage("You fill up the " + s + " with water.");
                    }
                    fillVialEvent.stop();
                }
            });
            Server.registerEvent(fillVialEvent);
            return true;
        }
    }
}