using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;
using System;
using System.Collections.Generic;

namespace RS2.Server.player
{
    internal class DwarfCannon
    {
        private Player p;
        private Location cannonLocation;
        private Location fakeCannonLocation;
        private long setupTime;
        private int constructionStage;
        private int cannonballs;
        private bool stopCannon;
        private int direction;
        private bool firing;
        private static int MAX_CANNONBALLS = 30;
        private static int CANNONBALL_ID = 2;
        private List<Npc> npcsInArea;

        private static int[] CANNON_PIECES = {
		    6, 8, 10, 12
	    };

        private static string[] CONSTRUCTION_MESSAGE = {
		    "Cannon base", "Stand", "Barrels", "Furnace"
	    };

        private static int[] CANNON_OBJECTS = {
		    7, 8, 9, 6
	    };

        public DwarfCannon(Player player)
        {
            p = player;
            cannonLocation = p.getLocation();
            fakeCannonLocation = new Location(cannonLocation.getX() + 1, cannonLocation.getY() + 1, cannonLocation.getY());
            firing = false;
            cannonballs = 0;
            constructionStage = 0;
            direction = 0;
            setNpcsInArea();
            newCannon();
        }

        private void setNpcsInArea()
        {
            npcsInArea = new List<Npc>();
            foreach (Npc n in Server.getNpcList())
            {
                Location l1 = n.getMinimumCoords();
                Location l2 = n.getMaximumCoords();
                if (cannonLocation.inArea(l1.getX() - 8, l1.getY() - 8, l2.getX() + 8, l2.getY() + 8))
                {
                    npcsInArea.Add(n);
                }
            }
        }

        public void newCannon()
        {
            setupTime = Environment.TickCount;
            Event setupNewCannonEvent = new Event(1000);
            setupNewCannonEvent.setAction(() =>
            {
                string prefixMsg = (constructionStage == 0) ? "You place the " : "You add the ";
                string suffixMsg = (constructionStage == 0) ? " on the ground." : ".";
                if (p.getInventory().deleteItem(CANNON_PIECES[constructionStage]))
                {
                    p.getPackets().createObject(CANNON_OBJECTS[constructionStage], cannonLocation, 0, 10);
                    p.getPackets().sendMessage(prefixMsg + CONSTRUCTION_MESSAGE[constructionStage] + suffixMsg);
                    if (++constructionStage >= 4)
                    {
                        constructionStage--;
                        setupNewCannonEvent.stop();
                        return;
                    }
                    p.setLastAnimation(new Animation(827));
                }
                else
                {
                    setupNewCannonEvent.stop();
                }
            });
            Server.registerEvent(setupNewCannonEvent);
        }

        public void fireCannon()
        {
            if (firing)
            {
                loadCannon();
                return;
            }

            firing = true;
            int cannonTurnAnimation = 515;
            Event attemptFireCannonEvent = new Event(1000);
            attemptFireCannonEvent.setAction(() =>
            {
                if (!firing)
                {
                    attemptFireCannonEvent.stop();
                    return;
                }
                p.getPackets().newObjectAnimation(cannonLocation, cannonTurnAnimation);
                Event fireCannonEvent = new Event(600);
                fireCannonEvent.setAction(() =>
                {
                    if (!firing)
                    {
                        fireCannonEvent.stop();
                        return;
                    }
                    if (stopCannon && cannonTurnAnimation == 514)
                    {
                        cannonTurnAnimation = 514;
                        fireCannonEvent.stop();
                        firing = false;
                        return;
                    }
                    if (!stopCannon)
                    {
                        if (checkHitTarget())
                            checkCannonballs();
                    }
                    if (direction++ == 7)
                        direction = 0;
                    if (++cannonTurnAnimation > 521)
                        cannonTurnAnimation = 514;
                    fireCannonEvent.stop();
                });
                Server.registerEvent(fireCannonEvent);
            });
            Server.registerEvent(attemptFireCannonEvent);
        }

        public void loadCannon()
        {
            if (cannonballs >= MAX_CANNONBALLS)
            {
                p.getPackets().sendMessage("The cannon cannot hold any more ammo.");
                return;
            }
            int ballsInInven = p.getInventory().getItemAmount(CANNONBALL_ID);
            if (ballsInInven == 0)
            {
                p.getPackets().sendMessage("You don't have any cannonballs to restock the cannon with!");
                return;
            }
            int difference = MAX_CANNONBALLS - cannonballs;
            int amountToLoad = difference;
            if (ballsInInven < (difference))
            {
                amountToLoad = ballsInInven;
            }
            if (p.getInventory().deleteItem(CANNONBALL_ID, amountToLoad))
            {
                cannonballs += difference;
                p.getPackets().sendMessage("You load the cannon with " + amountToLoad + " cannonballs.");
                stopCannon = false;
            }
        }

        public void pickupCannon()
        {
            if (p.getInventory().getTotalFreeSlots() < (constructionStage + (cannonballs > 0 ? 1 : 0)))
            {
                p.getPackets().sendMessage("You don't have enough room to pick up the cannon.");
                return;
            }
            for (int i = 0; i <= constructionStage; i++)
            {
                if (!p.getInventory().addItem(CANNON_PIECES[i]))
                {
                    return;
                }
            }
            firing = false;
            p.getInventory().addItem(2, cannonballs);
            p.getPackets().removeObject(cannonLocation, 0, 10);
            p.getPackets().sendMessage("You pick up the cannon.");
            p.setCannon(null);
        }

        private void checkCannonballs()
        {
            cannonballs--;
            if (cannonballs <= 0)
            {
                p.getPackets().sendMessage("Your cannon has run out of ammo!");
                stopCannon = true;
            }
        }

        protected bool checkHitTarget()
        {
            int cannonX = fakeCannonLocation.getX();
            int cannonY = fakeCannonLocation.getY();
            Npc[] npcsToAttack = new Npc[npcsInArea.Count];
            bool hit = false;
            foreach (Npc n in Server.getNpcList())
            {
                hit = false;
                Location l = n.getLocation();
                if (n == null || n.isHidden() || n.isDead() || !n.getLocation().withinDistance(fakeCannonLocation, 8))
                {
                    continue;
                }
                switch (direction)
                {
                    case 0: // North
                        hit = l.inArea(cannonX - 1, cannonY, cannonX + 1, cannonY + 8);
                        break;

                    case 1: // North east
                        break;

                    case 2: // East:
                        hit = l.inArea(cannonX, cannonY - 1, cannonX + 8, cannonY + 1);
                        break;

                    case 3: // South east
                        break;

                    case 4: // South
                        hit = l.inArea(cannonX - 1, cannonY - 8, cannonX + 1, cannonY);
                        break;

                    case 5: // South west
                        break;

                    case 6: // West
                        hit = l.inArea(cannonX - 8, cannonY - 1, cannonX, cannonY + 1);
                        break;

                    case 7: // North west
                        break;
                }
                if (hit)
                {
                    Npc npc = n;
                    p.getPackets().sendProjectile(fakeCannonLocation, n.getLocation(), 30, 53, 50, 38, 38, 40, n);
                    Event doCannonHitEvent = new Event(1000);
                    doCannonHitEvent.setAction(() =>
                    {
                        doCannonHitEvent.stop();
                        int damage = Misc.random(30);
                        p.getSkills().addXp(Skills.SKILL.RANGE, damage * 2);
                        npc.hit(damage);
                        npc.setLastAnimation(new Animation(npc.getDefenceAnimation()));
                    });
                    Server.registerEvent(doCannonHitEvent);
                    return true;
                }
            }
            return false;
        }

        public Location getLocation()
        {
            return cannonLocation;
        }
    }
}