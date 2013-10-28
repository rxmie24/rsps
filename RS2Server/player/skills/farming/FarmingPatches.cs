using RS2.Server.events;
using System;
using System.Collections.Generic;

namespace RS2.Server.player.skills.farming
{
    internal class FarmingPatches
    {
        private List<Patch> patches;
        private List<Patch> removedPatches;

        public FarmingPatches()
        {
            patches = new List<Patch>();
            removedPatches = new List<Patch>();
        }

        public void processPatches()
        {
            /*
             * If something has to be removed from the ArrayList in this loop, use it.remove() not patches.remove()
             * or else we end up with a ConcurrentModificationException and it'll break out the loop :$.
             */
            Event processPatchesEvent = new Event(3000);
            processPatchesEvent.setAction(() =>
            {
                foreach (Patch patch in patches)
                {
                    if (patch == null)
                    {
                        removedPatches.Add(patch);
                    }
                    else if (patch.isSapling())
                    {
                        if (Farming.growSapling(patch))
                        {
                            removedPatches.Add(patch);
                        }
                    }
                    else if (!patch.patchOccupied())
                    {
                        if (Farming.regrowWeeds(patch))
                        { // true if we should remove from the list
                            removedPatches.Add(patch);
                        }
                    }
                    else if (!patch.isFullyGrown() && patch.patchOccupied())
                    {
                        long currentTime = Environment.TickCount;
                        long lastUpdatedTime = patch.getLastUpdate();
                        int delay = (int)(patch.getTimeToGrow() / patch.getConfigLength());
                        //if (currentTime - lastUpdatedTime >= delay) {
                        Farming.growPatch(patch);
                        //}
                    }
                }
                patches.RemoveAll(new Predicate<Patch>(delegate(Patch x) { return removedPatches.Contains(x); }));
            });
            Server.registerEvent(processPatchesEvent);
        }

        public Patch[] getPatchesForPlayer(Player p, int min, int max)
        {
            int i = 0;
            Patch[] array = new Patch[4];
            for (int j = min; j <= max; j++)
            {
                Patch patch = patchExists(p, j);
                array[i++] = patch;
            }
            return array;
        }

        public Patch patchExists(Player p, int index)
        {
            foreach (Patch patch in patches)
            {
                if (patch.getOwnerName().Equals(p.getLoginDetails().getUsername()))
                {
                    if (patch.getPatchIndex() == index)
                    {
                        return patch;
                    }
                }
            }
            return null;
        }

        public void removePatch(Patch patch)
        {
            lock (patches)
            {
                patches.Remove(patch);
            }
        }

        public void addPatch(Patch patch)
        {
            lock (patches)
            {
                patches.Add(patch);
            }
        }
    }
}