using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.woodcutting
{
    internal class Woodcutting : WoodcuttingData
    {
        public Woodcutting()
        {
        }

        public static void tryCutTree(Player p, ushort treeId, Location treeLocation, int i, bool newCut)
        {
            int index = getTreeIndex(treeId);
            if (index == -1)
            {
                return;
            }
            int s = TREE_SIZE[index];
            int dis = s;
            AreaEvent tryCutTreeAreaEvent = new AreaEvent(p, treeLocation.getX() - s, treeLocation.getY() - s, treeLocation.getX() + s, treeLocation.getY() + s);
            tryCutTreeAreaEvent.setAction(() =>
            {
                cutTree(p, treeId, treeLocation, i, newCut, dis);
            });
            Server.registerCoordinateEvent(tryCutTreeAreaEvent);
        }

        public static int getTreeIndex(int id)
        {
            for (int i = 0; i < TREES.Length; i++)
            {
                if (id == TREES[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static void cutTree(Player p, ushort treeId, Location treeLocation, int i, bool newCut, int distance)
        {
            if (!newCut && p.getTemporaryAttribute("cuttingTree") == null)
            {
                return;
            }
            if (newCut)
            {
                if (i == 10 || i == 11)
                { // Magic or Yew tree.
                    if (!Server.getGlobalObjects().objectExists(treeId, treeLocation))
                    {
                        //	misc.WriteError(p.getUsername() + " tried to cut a non existing Magic or Yew tree!");
                        //	return;
                    }
                }
                Tree newTree = new Tree(i, treeId, treeLocation, LOGS[i], LEVEL[i], TREE_NAME[i], XP[i], distance);
                p.setTemporaryAttribute("cuttingTree", newTree);
            }
            Tree treeToCut = (Tree)p.getTemporaryAttribute("cuttingTree");
            if (!canCut(p, treeToCut, null))
            {
                resetWoodcutting(p);
                return;
            }
            if (newCut)
            {
                p.setLastAnimation(new Animation(getAxeAnimation(p)));
                p.setFaceLocation(treeLocation);
                p.getPackets().sendMessage("You begin to swing your axe at the tree..");
            }
            int delay = getCutTime(p, treeToCut.getTreeIndex());
            Event cutTreeEvent = new Event(delay);
            cutTreeEvent.setAction(() =>
            {
                cutTreeEvent.stop();
                if (p.getTemporaryAttribute("cuttingTree") == null)
                {
                    resetWoodcutting(p);
                    return;
                }
                Tree tree = (Tree)p.getTemporaryAttribute("cuttingTree");
                if (!canCut(p, treeToCut, tree))
                {
                    resetWoodcutting(p);
                    return;
                }
                Server.getGlobalObjects().lowerHealth(tree.getTreeId(), tree.getTreeLocation());
                if (!Server.getGlobalObjects().originalObjectExists(tree.getTreeId(), tree.getTreeLocation()))
                {
                    resetWoodcutting(p);
                    p.setLastAnimation(new Animation(65535));
                }
                if (p.getInventory().addItem(tree.getLog()))
                {
                    p.getPackets().closeInterfaces();
                    int index = tree.getTreeIndex();
                    string s = index == 1 || index == 3 || index == 8 ? "an" : "a";
                    p.getSkills().addXp(Skills.SKILL.WOODCUTTING, tree.getXp());
                    if (index == 6)
                    {
                        p.getPackets().sendMessage("You retrieve some Hollow bark from the tree.");
                    }
                    else
                    {
                        p.getPackets().sendMessage("You cut down " + s + " " + tree.getName() + " log.");
                    }
                    if (Misc.random(3) == 0)
                    {
                        int nestId = Misc.random(10) == 0 ? 5073 : 5074;
                        GroundItem g = new GroundItem(nestId, 1, new Location(p.getLocation().getX(), p.getLocation().getY(), p.getLocation().getZ()), p);
                        Server.getGroundItems().newEntityDrop(g);
                        p.getPackets().sendMessage("Something falls out of the tree and lands at your feet.");
                    }
                }
                cutTree(p, tree.getTreeId(), tree.getTreeLocation(), tree.getTreeIndex(), false, tree.getDistance());
            });
            Server.registerEvent(cutTreeEvent);
            if (delay >= 2550)
            {
                Event treeCuttingAnimationEvent = new Event(2550);
                int time = delay;
                treeCuttingAnimationEvent.setAction(() =>
                {
                    time -= 2550;
                    if (time <= 0)
                    {
                        treeCuttingAnimationEvent.stop();
                    }
                    Tree tree = (Tree)p.getTemporaryAttribute("cuttingTree");
                    if (!canCut(p, treeToCut, tree))
                    {
                        treeCuttingAnimationEvent.stop();
                        return;
                    }
                    p.setFaceLocation(treeLocation);
                    p.setLastAnimation(new Animation(getAxeAnimation(p)));
                });
                Server.registerEvent(treeCuttingAnimationEvent);
            }
        }

        private static bool canCut(Player p, Tree tree, Tree tree2)
        {
            if (tree == null || p == null || !Server.getGlobalObjects().originalObjectExists(tree.getTreeId(), tree.getTreeLocation()))
            {
                return false;
            }
            if (!p.getLocation().withinDistance(tree.getTreeLocation(), tree.getDistance()))
            {
                return false;
            }
            if (tree2 != null)
            {
                if (!tree.Equals(tree2))
                {
                    return false;
                }
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.WOODCUTTING) < tree.getLevel())
            {
                p.getPackets().sendMessage("You need a Woodcutting level of " + tree.getLevel() + " to cut that tree.");
                return false;
            }
            if (!hasAxe(p))
            {
                p.getPackets().sendMessage("You need an axe to cut down a tree!");
                return false;
            }
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendChatboxInterface(210);
                p.getPackets().modifyText("Your inventory is too full to carry any logs.", 210, 1);
                return false;
            }
            return true;
        }

        public static int getCutTime(Player p, int i)
        {
            int standardTime = TREE_DELAY[i];
            int randomTime = standardTime + Misc.random(standardTime);
            int axeTime = AXE_DELAY[getUsedAxe(p)];
            int finalTime = randomTime -= axeTime;
            if (finalTime <= 1000 || finalTime <= 0)
            {
                finalTime = 1000;
            }
            return finalTime;
        }

        public static bool hasAxe(Player p)
        {
            for (int i = 0; i < AXES.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == AXES[i] || p.getInventory().hasItem(AXES[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static void randomNestItem(Player p, int nest)
        {
            if (p.getInventory().findFreeSlot() == -1)
            {
                p.getPackets().sendMessage("You need atleast 1 free inventory spot to search through a nest.");
                return;
            }
            int[] items = nest == 5073 ? NEST_SEEDS : NEST_JEWELLERY;
            int reward = items[Misc.random(items.Length - 1)];
            if (Misc.random(4) == 0 && nest == 5073)
            {
                p.getPackets().sendMessage("You thought you saw something, but the nest is actually empty.");
                reward = 0;
            }
            if (nest == 5073)
            {
                if (Misc.random(50) == 0)
                {
                    reward = 1645;
                }
            }
            if (p.getInventory().replaceSingleItem(nest, 6693))
            {
                if (reward != 0)
                {
                    p.getInventory().addItem(reward);
                }
            }
        }

        public static int getUsedAxe(Player p)
        {
            int index = -1;
            for (int i = 0; i < AXES.Length; i++)
            {
                if (p.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON) == AXES[i] || p.getInventory().hasItem(AXES[i]))
                {
                    index = i;
                }
            }
            return index;
        }

        public static int getAxeAnimation(Player p)
        {
            int axe = getUsedAxe(p);
            if (axe == -1)
            {
                return 65535;
            }
            return AXE_ANIMATION[axe];
        }

        public static void resetWoodcutting(Player p)
        {
            p.removeTemporaryAttribute("cuttingTree");
        }
    }
}