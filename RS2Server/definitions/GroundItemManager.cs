using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player;
using System.Collections.Generic;

namespace RS2.Server.definitions
{
    internal class GroundItemManager
    {
        private List<GroundItem> items;

        public GroundItemManager()
        {
            items = new List<GroundItem>();
        }

        public void newEntityDrop(GroundItem item)
        {
            lock (items)
            {
                items.Add(item);
            }
            if (item.getOwner() != null && !item.getOwner().isDestroyed())
            {
                item.getOwner().getPackets().createGroundItem(item);
            }
            Event showItemDropGloballyEvent = new Event(60000); //60 seconds to show dropped item to all players.
            showItemDropGloballyEvent.setAction(() =>
            {
                newGlobalItem(item);
                showItemDropGloballyEvent.stop();
            });
            Server.registerEvent(showItemDropGloballyEvent);
        }

        public bool addToStack(int id, int amount, Location location, Player p)
        {
            if (!ItemData.forId(id).isStackable())
            {
                return false;
            }
            foreach (GroundItem i in items)
            {
                if (i.getLocation().Equals(location) && i.getItemId() == id)
                {
                    if (!i.isGlobal() && i.getOwner().Equals(p))
                    {
                        i.setItemAmount(i.getItemAmount() + amount);
                        p.getPackets().clearGroundItem(i);
                        p.getPackets().createGroundItem(i);
                        return true;
                    }
                }
            }
            return false;
        }

        private void newGlobalItem(GroundItem item)
        {
            if (item == null)
            {
                return;
            }
            item = itemExists(item);
            if (item != null)
            {
                item.setGlobal(true);
                foreach (Player p in Server.getPlayerList())
                {
                    if (p == null || (item.getDefinition().isPlayerBound() && !item.getOwner().Equals(p)))
                    {
                        continue;
                    }
                    if (p.getLocation().withinDistance(item.getLocation(), 60))
                    {
                        if (item.getOwner() != null)
                        {
                            p.getPackets().createGroundItem2(item);
                        }
                        else
                        {
                            p.getPackets().createGroundItem(item);
                        }
                    }
                }
                if (!item.getDefinition().isPlayerBound())
                {
                    item.setOwner(null);
                }
                GroundItem i = item;
                if (!item.isRespawn())
                {
                    Event removeGlobalItemEvent = new Event(60000);
                    removeGlobalItemEvent.setAction(() =>
                    {
                        clearGlobalItem(i);
                        removeGlobalItemEvent.stop();
                    });
                    Server.registerEvent(removeGlobalItemEvent);
                }
            }
        }

        public void newWorldItem(GroundItem item)
        {
            lock (items)
            {
                items.Add(item);
            }
            item.setOwner(null);
            newGlobalItem(item);
        }

        public bool deleteItem(int id, Location location)
        {
            GroundItem item = itemExists(location, id);
            if (item != null)
            {
                clearGlobalItem(item);
                return true;
            }
            return false;
        }

        public void pickupItem(Player p, int id, Location location)
        {
            GroundItem item = itemExists(location, id);
            if (item != null && p.getSprites().getPrimarySprite() == -1 && p.getSprites().getSecondarySprite() == -1)
            {
                if (item.getDefinition().isPlayerBound() && !item.getOwner().Equals(p))
                {
                    return;
                }
                if (!p.getInventory().addItem(item.getItemId(), item.getItemAmount()))
                {
                    return;
                }
                clearGlobalItem(item);
                if (item.isRespawn())
                {
                    GroundItem i = item;
                    Event itemRespawnEvent = new Event(60000);
                    itemRespawnEvent.setAction(() =>
                    {
                        GroundItem respawn = new GroundItem(i.getItemId(), i.getItemAmount(), i.getLocation(), null);
                        respawn.setRespawn(true);
                        respawn.setGlobal(true);
                        newGlobalItem(respawn);
                        itemRespawnEvent.stop();
                    });
                }
            }
        }

        public void refreshGlobalItems(Player p)
        {
            if (Location.inFightCave(p.getLocation()))
            {
                return;
            }
            foreach (GroundItem i in items)
            {
                if (i != null)
                {
                    if ((i.isGlobal() && ((i.getOwner() != null && i.getOwner().Equals(p))))  // is your item after shown to other players (only shows for you)
                            || (!i.isGlobal() && i.getOwner() != null && i.getOwner().Equals(p)) // is your item before shown to other players
                            || (i.getOwner() == null && i.isGlobal()))
                    { // is nobodys item after shown to other players (applies to 'regular' drops)
                        if (p.getLocation().withinDistance(i.getLocation(), 60))
                        {
                            p.getPackets().clearGroundItem(i);
                            p.getPackets().createGroundItem(i);
                        }
                    }
                }
            }
        }

        public void clearGlobalItem(GroundItem item)
        {
            lock (items)
            {
                items.Remove(item);
            }
            if (item != null)
            {
                foreach (Player p in Server.getPlayerList())
                {
                    if (p == null)
                    {
                        continue;
                    }
                    if (p.getLocation().withinDistance(item.getLocation(), 60))
                    {
                        p.getPackets().clearGroundItem(item);
                    }
                }
            }
        }

        private GroundItem itemExists(GroundItem item)
        {
            foreach (GroundItem i in items)
            {
                if (i.Equals(item))
                {
                    return i;
                }
            }
            return null;
        }

        public GroundItem itemExists(Location l, int id)
        {
            foreach (GroundItem i in items)
            {
                if (i.getLocation().Equals(l) && i.getItemId() == id)
                {
                    return i;
                }
            }
            return null;
        }
    }
}