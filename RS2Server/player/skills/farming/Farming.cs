using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.player.skills.woodcutting;
using RS2.Server.util;
using System;

namespace RS2.Server.player.skills.farming
{
    internal class Farming : FarmingData
    {
        public Farming()
        {
        }

        public static bool interactWithPatch(Player p, int objectId, int objectX, int objectY, int seedId)
        {
            for (int i = 0; i < PATCHES.Length; i++)
            {
                if (objectId == (int)PATCHES[i][1])
                {
                    Location patchLocation = new Location(objectX, objectY, 0);
                    int j = i;
                    int[] data = Farming.getPatchDistances(i, objectX, objectY);
                    AreaEvent interactWithPatchAreaEvent = new AreaEvent(p, data[0], data[1], data[2], data[3]);
                    interactWithPatchAreaEvent.setAction(() =>
                    {
                        tendToPatch(p, patchLocation, seedId, j);
                    });
                    Server.registerCoordinateEvent(interactWithPatchAreaEvent);
                    return true;
                }
            }
            return false;
        }

        protected static void tendToPatch(Player p, Location patchLocation, int item, int i)
        {
            Patch patch = null;
            i = getPatchIndex(patchLocation, i);
            if (i == -1)
            {
                return;
            }
            patch = Server.getGlobalObjects().getFarmingPatches().patchExists(p, i);
            if (patch == null)
            {
                patch = new Patch(p.getLoginDetails().getUsername(), (PatchType)PATCHES[i][0], i, (int)PATCHES[i][7], (int)PATCHES[i][6]);
                patch.setConfigArray(WEEDS_CONFIG);
                Server.getGlobalObjects().getFarmingPatches().addPatch(patch);
                rakePatch(p, patch);
            }
            else
            {
                if (!patch.patchOccupied() && (item == -1 || item == TOOLS[0]))
                {
                    rakePatch(p, patch);
                }
                else if (patch.isBlankPatch() && item != -1)
                {
                    plantCrop(p, patch, item);
                }
                else if (patch.isFullyGrown() && (item == -1 || item == TOOLS[3]))
                {
                    harvestCrop(p, patch);
                }
                else if (patch.isFruitTree() && patch.getStatus() >= 6 && patch.getStatus() <= 11)
                {
                    harvestFruit(p, patch);
                }
                else if ((patch.isTree() || patch.isFruitTree()) && patch.getStatus() == patch.checkHealthStatus() && !patch.isHealthChecked())
                {
                    checkTreeHealth(p, patch);
                }
                else if ((patch.isTree() || patch.isFruitTree()) && patch.getStatus() == patch.chopStatus() && patch.isHealthChecked())
                {
                    chopTree(p, patch);
                }
                else if ((patch.isTree() || patch.isFruitTree()) && patch.getStatus() == patch.stumpStatus() && patch.isHealthChecked() && item == TOOLS[3])
                {
                    digUpTree(p, patch);
                }
                else if (patch.isFruitTree() && patch.getStatus() == patch.stumpStatus() && !patch.isHealthChecked())
                {
                    p.getPackets().sendMessage("You must check the tree's health before you can chop it down.");
                }
                else
                {
                    p.getPackets().sendMessage("Nothing interesting happens.");
                }
            }
        }

        private static void harvestFruit(Player p, Patch patch)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            p.setLastAnimation(new Animation(2282));
            p.setTemporaryAttribute("unmovable", true);
            patch.setWeeding(true);
            Event harvestFruitEvent = new Event(1700);
            harvestFruitEvent.setAction(() =>
            {
                if (patch.getStatus() == patch.chopStatus() || p.isDisconnected() || p.isDestroyed() || p.isDead() || p.getTemporaryAttribute("teleporting") != null)
                {
                    harvestFruitEvent.stop();
                    return;
                }
                p.setLastAnimation(new Animation(2282));
                string s = patch.getSeedIndex() == 41 ? "leaf " : "";
                Event harvestingFruitEvent = new Event(800);
                harvestingFruitEvent.setAction(() =>
                {
                    harvestingFruitEvent.stop();
                    p.getPackets().sendMessage("You pick " + (string)SEEDS[patch.getSeedIndex()][9] + " " + (string)SEEDS[patch.getSeedIndex()][7] + s + " from the tree.");
                    p.getSkills().addXp(Skills.SKILL.FARMING, (double)SEEDS[patch.getSeedIndex()][8]);
                    p.getInventory().addItemOrGround((int)SEEDS[patch.getSeedIndex()][2], 1);
                    patch.setStatus(patch.getStatus() - 1);
                    if (patch.getStatus() == 5)
                    { // We have taken all the fruit (it is 5 after we lower the status above)
                        patch.setStatus(13); // Chop option
                        p.removeTemporaryAttribute("unmovable");
                        patch.setWeeding(false);
                    }
                    setConfig(p, patch);
                });
                Server.registerEvent(harvestingFruitEvent);
            });
            Server.registerEvent(harvestFruitEvent);
        }

        private static void chopFruitTree(Player p, Patch patch)
        {
            if (!Woodcutting.hasAxe(p))
            {
                p.getPackets().sendMessage("You don't have an axe.");
                return;
            }
            patch.setWeeding(true); // prevents it from growing which makes me rage
            p.setLastAnimation(new Animation(Woodcutting.getAxeAnimation(p)));
            p.setTemporaryAttribute("harvesting", true);
            Event chopFruitTreeEvent = new Event(2550);
            chopFruitTreeEvent.setAction(() =>
            {
                if (p.getTemporaryAttribute("harvesting") != null)
                {
                    patch.setStatus(patch.stumpStatus());
                    setConfig(p, patch);
                }
                chopFruitTreeEvent.stop();
                patch.setWeeding(false);
            });
            Server.registerEvent(chopFruitTreeEvent);
        }

        private static void digUpTree(Player p, Patch patch)
        {
            p.setLastAnimation(new Animation(830));
            Event digUpTreeEvent = new Event(1000);
            digUpTreeEvent.setAction(() =>
            {
                patch.setStatus(2);
                patch.setConfigArray(WEEDS_CONFIG);
                setConfig(p, patch);
                p.getPackets().sendMessage("You dig up the tree.");
                digUpTreeEvent.stop();
            });
            Server.registerEvent(digUpTreeEvent);
        }

        private static void chopTree(Player p, Patch patch)
        {
            if (patch.isFruitTree())
            {
                chopFruitTree(p, patch);
                return;
            }
            if (!Woodcutting.hasAxe(p))
            {
                p.getPackets().sendMessage("You don't have an axe.");
                return;
            }
            if (!hasLevelToCutTree(p, patch))
            {
                p.getPackets().sendMessage("You will recieve no logs from this tree, due to your Woodcutting level.");
            }
            Tree newTree = new Tree(0, 0, null, (int)SEEDS[patch.getSeedIndex()][2], 0, (string)SEEDS[patch.getSeedIndex()][7], (double)SEEDS[patch.getSeedIndex()][11], 0);
            p.setTemporaryAttribute("cuttingTree", newTree);
            p.setLastAnimation(new Animation(Woodcutting.getAxeAnimation(p)));
            p.getPackets().sendMessage("You begin to swing your axe at the tree..");
            long delay = getCutTime(p, patch);
            bool canRecieveLog = hasLevelToCutTree(p, patch);
            Event chopTreeEvent = new Event(delay);
            chopTreeEvent.setAction(() =>
            {
                long timeSinceLastAnimation = Environment.TickCount;
                if (!Woodcutting.hasAxe(p))
                {
                    p.getPackets().sendMessage("You don't have an axe.");
                    Woodcutting.resetWoodcutting(p);
                    chopTreeEvent.stop();
                    return;
                }
                if (p.getTemporaryAttribute("cuttingTree") == null)
                {
                    Woodcutting.resetWoodcutting(p);
                    chopTreeEvent.stop();
                    return;
                }
                Tree tree = (Tree)p.getTemporaryAttribute("cuttingTree");
                if (!newTree.Equals(tree))
                {
                    chopTreeEvent.stop();
                    return;
                }
                if (canRecieveLog)
                {
                    string s = tree.getLog() == 1521 ? "an" : "a";
                    if (p.getInventory().addItem(tree.getLog()))
                    {
                        p.getSkills().addXp(Skills.SKILL.WOODCUTTING, tree.getXp());
                        p.getPackets().sendMessage("You cut down " + s + " " + tree.getName() + " log.");
                    }
                    else
                    {
                        p.getPackets().sendChatboxInterface(210);
                        p.getPackets().modifyText("Your inventory is too full to carry any logs.", 210, 1);
                        p.setLastAnimation(new Animation(65535));
                        chopTreeEvent.stop();
                        return;
                    }
                }
                if (Misc.random(canRecieveLog ? 2 : 0) == 0)
                {
                    p.setLastAnimation(new Animation(65535));
                    patch.setStatus(patch.getConfigLength() - 1);
                    setConfig(p, patch);
                    chopTreeEvent.stop();
                    return;
                }
                if (Environment.TickCount - timeSinceLastAnimation >= 2550)
                {
                    p.setLastAnimation(new Animation(Woodcutting.getAxeAnimation(p)));
                    timeSinceLastAnimation = Environment.TickCount;
                }
            });
            Server.registerEvent(chopTreeEvent);
        }

        public static int getCutTime(Player p, Patch patch)
        {
            int standardTime = (int)SEEDS[patch.getSeedIndex()][12];
            int randomTime = standardTime + Misc.random(standardTime);
            int axeTime = WoodcuttingData.AXE_DELAY[Woodcutting.getUsedAxe(p)];
            int finalTime = randomTime -= axeTime;
            if (finalTime <= 1000 || finalTime <= 0)
            {
                finalTime = 1000;
            }
            return finalTime;
        }

        private static bool hasLevelToCutTree(Player p, Patch patch)
        {
            if (patch.isFruitTree())
            {
                return true;
            }
            return p.getSkills().getGreaterLevel(Skills.SKILL.WOODCUTTING) >= (int)SEEDS[patch.getSeedIndex()][10];
        }

        private static void checkTreeHealth(Player p, Patch patch)
        {
            patch.setHealthChecked(true);
            if (patch.isFruitTree())
            {
                patch.setStatus(11);
            }
            else
            {
                patch.setStatus(patch.getConfigLength() - 2);
            }
            p.getPackets().sendMessage("You check the health of the tree.");
            setConfig(p, patch);
            p.getSkills().addXp(Skills.SKILL.FARMING, (double)SEEDS[patch.getSeedIndex()][5]);
        }

        private static void harvestCrop(Player p, Patch patch)
        {
            if (patch.getPatchType().Equals(PatchType.VEGATABLE) || patch.getPatchType().Equals(PatchType.VEGATABLE_1))
            {
                if (!p.getInventory().hasItem(TOOLS[3]))
                {
                    p.getPackets().sendMessage("You need a spade to harvest your crops.");
                    return;
                }
            }
            PatchType patchType = patch.getPatchType();
            int emote = (patchType.Equals(PatchType.HERB) || patchType.Equals(PatchType.FLOWER)) ? 2282 : 830;
            int delay = patchType.Equals(PatchType.HERB) ? 2250 : 1500;
            int amount = patchType.Equals(PatchType.FLOWER) ? 1 : (int)PATCHES[patch.getPatchIndex()][8] + Misc.random(10);
            string s = patchType.Equals(PatchType.FLOWER) ? "flower patch" : patchType.Equals(PatchType.HERB) ? "herb patch" : "allotment";
            string s1 = patchType.Equals(PatchType.FLOWER) || patchType.Equals(PatchType.HERB) ? "pick" : "harvest";
            p.setLastAnimation(new Animation(emote));
            p.setTemporaryAttribute("harvesting", true);
            Event startHarvestCropEvent = new Event(delay);
            startHarvestCropEvent.setAction(() =>
            {
                int i = 1;
                if (p.getTemporaryAttribute("harvesting") == null)
                {
                    startHarvestCropEvent.stop();
                    return;
                }
                p.setLastAnimation(new Animation(emote));
                Event doHarvestCropEvent = new Event(800);
                doHarvestCropEvent.setAction(() =>
                {
                    doHarvestCropEvent.stop();
                    p.getSkills().addXp(Skills.SKILL.FARMING, (double)SEEDS[patch.getSeedIndex()][5]);
                    p.getInventory().addItemOrGround((int)SEEDS[patch.getSeedIndex()][2], 1);
                    p.getPackets().sendMessage("You " + s1 + " " + (string)SEEDS[patch.getSeedIndex()][8] + " " + (string)SEEDS[patch.getSeedIndex()][7] + " from the " + s + " .");
                    if (i++ >= amount)
                    {
                        p.getPackets().sendMessage("The patch has been cleared.");
                        patch.setStatus(2);
                        patch.setConfigArray(WEEDS_CONFIG);
                        setConfig(p, patch);
                    }
                });
                Server.registerEvent(doHarvestCropEvent);
                if (i >= amount)
                {
                    startHarvestCropEvent.stop();
                    return;
                }
            });
            Server.registerEvent(startHarvestCropEvent);
        }

        private static void plantCrop(Player p, Patch patch, int seedId)
        {
            if (p.getTemporaryAttribute("unmovable") != null)
            {
                return;
            }
            for (int i = 0; i < SEEDS.Length; i++)
            {
                if (seedId == (int)SEEDS[i][1])
                {
                    PatchType type = patch.getPatchType();
                    bool sapling = type.Equals(PatchType.TREE) || type.Equals(PatchType.FRUIT_TREE);
                    string s = sapling ? "sapling" : "seeds.";
                    string s1 = s.Equals("sapling") ? patch.getSeedIndex() == 33 || patch.getSeedIndex() == 38 || patch.getSeedIndex() == 40 ? "an " : "a " : "";
                    if (!patch.getPatchType().Equals(PatchType.VEGATABLE) && !patch.getPatchType().Equals(PatchType.VEGATABLE_1))
                    {
                        if (!patch.getPatchType().Equals((PatchType)SEEDS[i][0]))
                        {
                            string s2 = !((PatchType)SEEDS[i][0]).Equals(PatchType.FRUIT_TREE) && !((PatchType)SEEDS[i][0]).Equals(PatchType.TREE) ? "seed" : "tree";
                            p.getPackets().sendMessage("This type of " + s2 + " cannot be planted here.");
                            return;
                        }
                    }
                    else if (patch.getPatchType().Equals(PatchType.VEGATABLE) || patch.getPatchType().Equals(PatchType.VEGATABLE_1))
                    {
                        if (!SEEDS[i][0].Equals(PatchType.VEGATABLE) && !SEEDS[i][0].Equals(PatchType.VEGATABLE_1))
                        {
                            p.getPackets().sendMessage("This type of seed  cannot be planted here.");
                            return;
                        }
                    }
                    int[] data = getDataForPatch(patch, i);
                    if (data == null)
                    {
                        return;
                    }
                    patch.setSeedIndex(i);
                    if (p.getSkills().getGreaterLevel(Skills.SKILL.FARMING) < (int)SEEDS[patch.getSeedIndex()][6])
                    {
                        p.getPackets().sendMessage("You need a Farming level of " + (int)SEEDS[patch.getSeedIndex()][6] + " to plant " + s1 + "" + (string)SEEDS[patch.getSeedIndex()][7] + " seeds.");
                        return;
                    }
                    int seedAmount = (int)PATCHES[patch.getPatchIndex()][8];
                    if (!p.getInventory().hasItemAmount((int)SEEDS[i][1], seedAmount))
                    {
                        p.getPackets().sendMessage("This patch requires " + seedAmount + " seeds.");
                        return;
                    }
                    if (!sapling)
                    {
                        if (!p.getInventory().hasItem(TOOLS[1]))
                        {
                            p.getPackets().sendMessage("You need a seed dibber to plant seeds.");
                            return;
                        }
                    }
                    else
                    {
                        if (!p.getInventory().hasItem(TOOLS[2]))
                        {
                            p.getPackets().sendMessage("You need a trowel to transfer the sapling from the pot to a farming patch.");
                            return;
                        }
                        if (!p.getInventory().hasItem(TOOLS[3]))
                        {
                            p.getPackets().sendMessage("You need a spade to plant the salping.");
                            return;
                        }
                    }
                    int j = i;
                    p.setLastAnimation(new Animation(2291));
                    p.setTemporaryAttribute("unmovable", true);
                    Event plantCropEvent = new Event(1000);
                    plantCropEvent.setAction(() =>
                    {
                        plantCropEvent.stop();
                        if (p.getInventory().deleteItem((int)SEEDS[j][1], seedAmount))
                        {
                            if (sapling)
                            {
                                p.getInventory().addItemOrGround(TOOLS[4]);
                            }
                            patch.setStatus(0);
                            patch.setConfigArray(data);
                            patch.setTimeToGrow((long)SEEDS[j][3]);
                            setConfig(p, patch);
                            p.removeTemporaryAttribute("unmovable");
                            string prefix = seedAmount > 1 ? "" + seedAmount : "a";
                            string suffix = seedAmount > 1 ? "seeds." : "seed.";
                            if ((patch.getPatchType().Equals(PatchType.HERB) && (patch.getSeedIndex() == 20 || patch.getSeedIndex() == 21)))
                            {
                                prefix = "an";
                            }
                            string message = sapling ? "You plant the " + (string)SEEDS[patch.getSeedIndex()][7] + " sapling." : "You plant " + prefix + " " + (string)SEEDS[patch.getSeedIndex()][7] + " " + suffix;
                            p.getPackets().sendMessage(message);
                            p.getSkills().addXp(Skills.SKILL.FARMING, (double)SEEDS[patch.getSeedIndex()][4]);
                        }
                    });
                    Server.registerEvent(plantCropEvent);
                    break;
                }
            }
        }

        private static void rakePatch(Player p, Patch patch)
        {
            if (patch.isBlankPatch())
            {
                p.getPackets().sendMessage("This patch is clear of weeds.");
                return;
            }
            if (!p.getInventory().hasItem(TOOLS[0]))
            {
                p.getPackets().sendMessage("You need a rake to clear the weeds from this patch.");
                return;
            }
            p.setLastAnimation(new Animation(2273));
            patch.setWeeding(true);
            p.setTemporaryAttribute("harvesting", true);
            Event rakePatchEvent = new Event(1300);
            rakePatchEvent.setAction(() =>
            {
                if (p.isDestroyed() || p.isDisconnected() || p.getTemporaryAttribute("harvesting") == null)
                {
                    rakePatchEvent.stop();
                    patch.setWeeding(false);
                    return;
                }
                if (!p.getInventory().hasItem(TOOLS[0]))
                {
                    p.getPackets().sendMessage("You need a rake to clear the weeds from this patch.");
                    patch.setWeeding(false);
                    rakePatchEvent.stop();
                    return;
                }
                p.getInventory().addItemOrGround(6055);
                p.setLastAnimation(new Animation(2273));
                setConfig(p, patch);
                patch.setStatus(patch.getStatus() + 1);
                if (patch.getStatus() >= 3)
                {
                    p.getPackets().sendMessage("You clear the weeds from the patch, this patch is now suitable for farming.");
                    patch.setHasWeeds(false);
                    patch.setWeeding(false);
                    patch.setStatus(2);
                    setConfig(p, patch);
                    p.setLastAnimation(new Animation(65535));
                    rakePatchEvent.stop();
                    return;
                }
            });
            Server.registerEvent(rakePatchEvent);
        }

        public static bool regrowWeeds(Patch patch)
        {
            bool shouldRemoveFromList = false;
            if (!patch.patchOccupied())
            {
                if (Misc.random(4) != 0 || patch.isWeeding())
                {
                    return false;
                }
                Player owner = Server.getPlayerForName(patch.getOwnerName());
                patch.setHasWeeds(true);
                patch.setStatus(patch.getStatus() - 1);
                if (patch.getStatus() <= -1)
                {
                    patch.setStatus(0);
                    shouldRemoveFromList = true;
                }
                if (owner != null)
                {
                    setConfig(owner, patch);
                }
            }
            return shouldRemoveFromList;
        }

        public static void growPatch(Patch patch)
        {
            if (patch.isTree())
            {
                if (patch.getStatus() == patch.checkHealthStatus())
                {
                    if (!patch.isHealthChecked())
                    {
                        return;
                    }
                }
                else if (patch.getStatus() == patch.chopStatus())
                {
                    return;
                }
                else if (patch.getStatus() == patch.stumpStatus())
                {
                    patch.setStatus(patch.chopStatus());
                }
                else
                {
                    patch.setStatus(patch.getStatus() + 1);
                }
            }
            else if (patch.isFruitTree())
            {
                if (patch.isWeeding())
                {
                    return;
                }
                if (patch.getStatus() == patch.checkHealthStatus())
                {
                    if (!patch.isHealthChecked())
                    {
                        return;
                    }
                }
                if (patch.isHealthChecked())
                {
                    if (patch.getStatus() == patch.checkHealthStatus() - 1)
                    { // all fruit
                        return;
                    }
                }
                if (patch.getStatus() == patch.stumpStatus())
                {
                    patch.setStatus(patch.chopStatus());
                }
                else if (patch.getStatus() == patch.chopStatus())
                {
                    patch.setStatus(6);
                }
                else
                {
                    patch.setStatus(patch.getStatus() + 1);
                }
            }
            else
            {
                patch.setStatus(patch.getStatus() + 1);
            }
            Player owner = Server.getPlayerForName(patch.getOwnerName());
            if (owner != null)
            {
                setConfig(owner, patch);
            }
            patch.setLastUpdate(Environment.TickCount);
        }

        public static void setConfig(Player p, Patch patch)
        {
            int[] bounds = getIndexBoundaries(patch);
            Patch[] patches = Server.getGlobalObjects().getFarmingPatches().getPatchesForPlayer(p, bounds[0], bounds[1]);
            int config = 0;
            for (int i = 0; i < patches.Length; i++)
            {
                if (patches[i] != null && !patches[i].isSapling())
                {
                    config += (patches[i].getConfigElement(patches[i].getStatus()) * patches[i].getMultiplyer());
                }
            }
            p.getPackets().sendConfig(patch.getConfigId(), config);
        }

        public static bool plantSapling(Player p, int itemUsed, int usedWith)
        {
            if (itemUsed != TOOLS[4] && usedWith != TOOLS[4])
            {
                return false;
            }
            int itemOne = itemUsed;
            int itemTwo = usedWith;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    itemOne = usedWith;
                    itemTwo = itemUsed;
                }
                for (int j = 0; j < SAPLING_DATA.Length; j++)
                {
                    if (itemOne == (int)SAPLING_DATA[j][0] && itemTwo == TOOLS[4])
                    {
                        if (!p.getInventory().hasItem(TOOLS[2]))
                        {
                            p.getPackets().sendMessage("You don't have a trowel.");
                            return true;
                        }
                        if (!p.getInventory().hasItem(TOOLS[4]))
                        {
                            p.getPackets().sendMessage("You need a plant pot filled with compost to do this.");
                            return true;
                        }
                        string s = j == 0 ? "" : "seed";
                        if (p.getInventory().deleteItem((int)SAPLING_DATA[j][0]))
                        {
                            Patch patch = new Patch(p.getLoginDetails().getUsername(), j);
                            Server.getGlobalObjects().getFarmingPatches().addPatch(patch);
                            p.getInventory().replaceSingleItem(TOOLS[4], (int)SAPLING_DATA[j][1]);
                            p.getPackets().sendMessage("You place the " + (string)SAPLING_DATA[j][3] + " " + s + " into the plant pot and cover it with soil...now wait.");
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static int[] getPatchDistances(int index, int objectX, int objectY)
        {
            int[] data = new int[4];
            if (index >= 0 && index <= 7)
            { // Vegatable
                data[0] = objectX - 1;
                data[1] = objectY - 1;
                data[2] = objectX + 1;
                data[3] = objectY + 1;
            }
            else if (index >= 8 && index <= 15)
            { // Herb & flower
                data[0] = objectX - 1;
                data[1] = objectY - 1;
                data[2] = objectX + 2;
                data[3] = objectY + 2;
            }
            else if (index >= 12 && index <= 15)
            { // Flower
                data[0] = objectX - 1;
                data[1] = objectY - 1;
                data[2] = objectX + 2;
                data[3] = objectY + 2;
            }
            else if (index >= 16 && index <= 19)
            { // Trees
                data[0] = objectX - 1;
                data[1] = objectY - 1;
                data[2] = objectX + 3;
                data[3] = objectY + 3;
            }
            else if (index >= 20 && index <= 23)
            { // Fruit Trees
                data[0] = objectX - 1;
                data[1] = objectY - 1;
                data[2] = objectX + 2;
                data[3] = objectY + 2;
            }
            return data;
        }

        private static int getPatchIndex(Location patchLocation, int i)
        {
            if (patchLocation.inArea((int)PATCHES[i][2], (int)PATCHES[i][3], (int)PATCHES[i][4], (int)PATCHES[i][5]))
            {
                return i;
            }
            return -1;
        }

        private static int[] getIndexBoundaries(Patch patch)
        {
            int[] indexes = new int[2];
            switch (patch.getConfigId())
            {
                case 504: // Catherby + Draynor allotments
                    indexes[0] = 0;
                    indexes[1] = 3;
                    break;

                case 505: // Ardougne + Canifis allotments
                    indexes[0] = 4;
                    indexes[1] = 7;
                    break;

                case 515: // Herb patches
                    indexes[0] = 8;
                    indexes[1] = 11;
                    break;

                case 508: // Flower patches
                    indexes[0] = 12;
                    indexes[1] = 15;
                    break;

                case 502: // Tree patches
                    indexes[0] = 16;
                    indexes[1] = 19;
                    break;

                case 503: // Fruit tree patches
                    indexes[0] = 20;
                    indexes[1] = 23;
                    break;
            }
            return indexes;
        }

        public static void refreshPatches(Player p)
        {
            int[] configs = { 502, 503, 504, 505, 508, 515 };
            for (int i = 0; i < configs.Length; i++)
            {
                Patch patch = new Patch(p.getLoginDetails().getUsername(), 0);
                patch.setConfig(configs[i]);
                setConfig(p, patch);
            }
        }

        public static bool growSapling(Patch patch)
        {
            Player owner = Server.getPlayerForName(patch.getOwnerName());

            if (owner != null)
            {
                if (owner.getInventory().replaceSingleItem((int)SAPLING_DATA[patch.getPatchIndex()][1], (int)SAPLING_DATA[patch.getPatchIndex()][2]))
                {
                    return true;
                }
                else if (owner.getBank().findItem((int)SAPLING_DATA[patch.getPatchIndex()][1]) != -1)
                {
                    Item item = owner.getBank().getSlot(owner.getBank().findItem((int)SAPLING_DATA[patch.getPatchIndex()][1]));
                    if (item.getItemId() == (int)SAPLING_DATA[patch.getPatchIndex()][1])
                    {
                        item.setItemId((int)SAPLING_DATA[patch.getPatchIndex()][2]);
                        owner.getBank().refreshBank();
                        return true;
                    }
                }
            }
            return true;
        }

        private static int[] getDataForPatch(Patch patch, int seed)
        {
            PatchType type = patch.getPatchType();
            if (type.Equals(PatchType.VEGATABLE))
            { // Draynor + Catherby
                return ALLOTMENT_PATCH_CONFIGS[seed];
            }
            else
                if (type.Equals(PatchType.VEGATABLE_1))
                { // Canifis + Ardougne
                    return ALLOTMENT_PATCH_CONFIGS[seed];
                }
                else
                    if (type.Equals(PatchType.HERB))
                    {
                        return HERB_PATCH_CONFIGS[seed - 14];
                    }
                    else
                        if (type.Equals(PatchType.FLOWER))
                        {
                            return FLOWER_PATCH_CONFIGS[seed - 28];
                        }
                        else
                            if (type.Equals(PatchType.TREE))
                            {
                                return TREE_PATCH_CONFIGS[seed - 33];
                            }
                            else
                                if (type.Equals(PatchType.FRUIT_TREE))
                                {
                                    return FRUIT_TREE_PATCH_CONFIGS[seed - 38];
                                }
            return null;
        }
    }
}