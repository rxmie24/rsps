using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.util;

namespace RS2.Server.player.skills.fishing
{
    internal class Fishing : FishingData
    {
        public Fishing()
        {
        }

        public static bool wantToFish(Player p, Npc npc, bool secondOption)
        {
            for (int i = 0; i < SPOT_IDS.Length; i++)
            {
                if (npc.getId() == SPOT_IDS[i])
                {
                    p.setFaceLocation(npc.getLocation());

                    AreaEvent startFishingAreaEvent = new AreaEvent(p, npc.getLocation().getX() - 1, npc.getLocation().getY() - 1, npc.getLocation().getX() + 1, npc.getLocation().getY() + 1);
                    startFishingAreaEvent.setAction(() =>
                    {
                        startFishing(p, i, npc, true, secondOption);
                    });
                    Server.registerCoordinateEvent(startFishingAreaEvent);
                    return true;
                }
            }
            return false;
        }

        private static void startFishing(Player p, int i, Npc npc, bool newFish, bool secondOption)
        {
            if (!newFish && p.getTemporaryAttribute("fishingSpot") == null)
            {
                return;
            }
            if (newFish)
            {
                int j = secondOption ? 1 : 0;
                int[] fish = secondOption ? SECOND_SPOT_FISH[i] : FIRST_SPOT_FISH[i];
                int[] level = secondOption ? SECOND_SPOT_LEVEL[i] : FIRST_SPOT_LEVEL[i];
                double[] xp = secondOption ? SECOND_SPOT_XP[i] : FIRST_SPOT_XP[i];
                p.setTemporaryAttribute("fishingSpot", new Spot(fish, level, i, SPOT_IDS[i], xp, npc.getLocation(), PRIMARY_ITEM[i][j], SECONDARY_ITEM[i][j], PRIMARY_NAME[i][j], SECONDARY_NAME[i][j], secondOption));
            }
            Spot fishingSpot = (Spot)p.getTemporaryAttribute("fishingSpot");
            int k = fishingSpot.isSecondOption() ? 1 : 0;
            int index = getFishToAdd(p, fishingSpot);
            if (!canFish(p, fishingSpot, null, index))
            {
                resetFishing(p);
                p.setLastAnimation(new Animation(65535));
                return;
            }
            if (newFish)
            {
                p.getPackets().sendMessage("You attempt to catch a fish...");
                p.setLastAnimation(new Animation(FISHING_ANIMATION[i][k]));
            }
            string name = fishingSpot.isSecondOption() ? SECOND_CATCH_NAME[fishingSpot.getSpotindex()][index] : FIRST_CATCH_NAME[fishingSpot.getSpotindex()][index];
            string s = fishingSpot.getSpotindex() == 1 && !fishingSpot.isSecondOption() ? "some" : "a";
            Event doFishingEvent = new Event(getFishingDelay(p, fishingSpot));
            doFishingEvent.setAction(() =>
            {
                doFishingEvent.stop();
                if (p.getTemporaryAttribute("fishingSpot") == null)
                {
                    resetFishing(p);
                    p.setLastAnimation(new Animation(65535));
                    return;
                }
                Spot fishingSpot2 = (Spot)p.getTemporaryAttribute("fishingSpot");
                if (!canFish(p, fishingSpot, fishingSpot2, index))
                {
                    resetFishing(p);
                    p.setLastAnimation(new Animation(65535));
                    return;
                }
                p.getPackets().closeInterfaces();
                p.getInventory().deleteItem(fishingSpot2.getSecondaryItem());
                p.setLastAnimation(new Animation(FISHING_ANIMATION2[fishingSpot2.getSpotindex()][k]));
                p.getPackets().sendMessage("You catch " + s + " " + name + ".");
                if (p.getInventory().addItem(fishingSpot2.getFish()[index]))
                {
                    p.getSkills().addXp(Skills.SKILL.FISHING, fishingSpot2.getFishingXp()[index]);
                }
                startFishing(p, i, null, false, secondOption);
            });
            Server.registerEvent(doFishingEvent);
        }

        private static long getFishingDelay(Player p, Spot fishingSpot)
        {
            int[] time = fishingSpot.isSecondOption() ? SECOND_SPOT_TIME : FIRST_SPOT_TIME;
            int[] minTime = fishingSpot.isSecondOption() ? SECOND_SPOT_MINTIME : FIRST_SPOT_MINTIME;
            int baseTime = time[fishingSpot.getSpotindex()];
            int min = minTime[fishingSpot.getSpotindex()];
            int finalDelay = baseTime -= Misc.random(min);
            return finalDelay;
        }

        protected static int getFishToAdd(Player p, Spot fishingSpot)
        {
            int fishingLevel = p.getSkills().getGreaterLevel(Skills.SKILL.FISHING);
            int[] canCatch = new int[fishingSpot.getFish().Length];
            int j = 0;
            for (int i = 0; i < fishingSpot.getFish().Length; i++)
            {
                if (fishingLevel >= fishingSpot.getLevel()[i])
                {
                    canCatch[j] = fishingSpot.getFish()[i];
                    j++;
                }
            }
            int[] canCatch2 = new int[j];
            for (int i = 0; i < canCatch.Length; i++)
            {
                if (canCatch[i] > 0)
                {
                    canCatch2[i] = canCatch[i];
                }
            }
            int fish = Misc.random(canCatch2.Length - 1);
            for (int i = 0; i < fishingSpot.getFish().Length; i++)
            {
                if (fish == fishingSpot.getFish()[i])
                {
                    return i;
                }
            }
            return Misc.random(canCatch2.Length - 1);
        }

        public static bool canFish(Player p, Spot fishingSpot, Spot fishingSpot2, int index)
        {
            if (p == null || fishingSpot == null)
            {
                return false;
            }
            if (!p.getLocation().withinDistance(fishingSpot.getSpotLocation(), 2))
            {
                return false;
            }
            if (fishingSpot2 != null)
            {
                if (!fishingSpot.Equals(fishingSpot2))
                {
                    return false;
                }
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.FISHING) < fishingSpot.getLevel()[index])
            {
                p.getPackets().sendMessage("You need a fishing level of " + fishingSpot.getLevel()[index] + " to fish here.");
                return false;
            }
            if (fishingSpot.getPrimaryItem() != -1)
            {
                if (!p.getInventory().hasItem(fishingSpot.getPrimaryItem()))
                {
                    p.getPackets().sendMessage("You need " + fishingSpot.getPrimaryName() + " to fish here.");
                    return false;
                }
            }
            if (fishingSpot.getSecondaryItem() != -1)
            {
                if (!p.getInventory().hasItem(fishingSpot.getSecondaryItem()))
                {
                    p.getPackets().sendMessage("You need " + fishingSpot.getSecondaryName() + " to fish here.");
                    return false;
                }
            }
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendChatboxInterface(210);
                p.getPackets().modifyText("Your inventory is too full to catch any more fish.", 210, 1);
                return false;
            }
            return true;
        }

        public static void resetFishing(Player p)
        {
            p.removeTemporaryAttribute("fishingSpot");
        }
    }
}