namespace RS2.Server.player.skills.fletching
{
    internal class Fletching : FletchingData
    {
        public Fletching()
        {
        }

        public static bool isFletching(Player p, int itemUsed, int usedWith)
        {
            int itemOne = itemUsed;
            int itemTwo = usedWith;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    itemOne = usedWith;
                    itemTwo = itemUsed;
                }
                for (int j = 0; j < LOGS.Length; j++)
                {
                    if (itemOne == LOGS[j] && itemTwo == KNIFE)
                    {
                        MakeBows.displayBowOptions(p, j, false, -1);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("fletchType", j);
                        return true;
                    }
                }
                for (int j = 0; j < UNSTRUNG_SHORTBOW.Length; j++)
                {
                    if (itemOne == UNSTRUNG_SHORTBOW[j] && itemTwo == BOWSTRING)
                    {
                        MakeBows.displayBowOptions(p, j, true, 0);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("fletchType", j);
                        p.setTemporaryAttribute("bowType", 0);
                        p.setTemporaryAttribute("stringingBow", (bool)true);
                        return true;
                    }
                }
                for (int j = 0; j < UNSTRUNG_LONGBOW.Length; j++)
                {
                    if (itemOne == UNSTRUNG_LONGBOW[j] && itemTwo == BOWSTRING)
                    {
                        MakeBows.displayBowOptions(p, j, true, 1);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("fletchType", j);
                        p.setTemporaryAttribute("bowType", 1);
                        p.setTemporaryAttribute("stringingBow", (bool)true);
                        return true;
                    }
                }
                for (int j = 0; j < ARROWHEAD.Length; j++)
                {
                    if (itemOne == ARROWHEAD[j] && itemTwo == HEADLESS_ARROW)
                    {
                        MakeAmmo.displayAmmoInterface(p, j, false, false);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("ammoType", j);
                        return true;
                    }
                }
                for (int j = 0; j < FEATHERLESS_BOLT.Length; j++)
                {
                    if (itemOne == FEATHERLESS_BOLT[j] && itemTwo == FEATHER)
                    {
                        MakeAmmo.displayAmmoInterface(p, j, true, true);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("ammoType2", j);
                        return true;
                    }
                }
                for (int k = 0; k < BOLT_TIPS.Length; k++)
                {
                    if (itemOne == HEADLESS_BOLT[k] && itemTwo == BOLT_TIPS[k])
                    {
                        MakeAmmo.displayAmmoInterface(p, k, false, true);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("ammoType2", k + 8);
                        return true;
                    }
                }
                for (int j = 0; j < FEATHERLESS_BOLT.Length; j++)
                {
                    for (int k = 0; k < BOLT_TIPS.Length; k++)
                    {
                        if (itemOne == FEATHERLESS_BOLT[j] && itemTwo == BOLT_TIPS[k])
                        {
                            setFletchItem(p, null);
                            p.getPackets().sendMessage("You must add Feathers to a bolt before you can add a tip.");
                            return true;
                        }
                    }
                }
                for (int j = 0; j < XBOW_LIMB.Length; j++)
                {
                    if (itemOne == XBOW_LIMB[j] && itemTwo == CROSSBOW_STOCK[0])
                    {
                        MakeXbow.displayOptionInterface(p, j, false);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("bowType2", j);
                        return true;
                    }
                }
                for (int j = 0; j < UNFINISHED_XBOW.Length; j++)
                {
                    if (itemOne == UNFINISHED_XBOW[j] && itemTwo == XBOW_STRING)
                    {
                        MakeXbow.displayOptionInterface(p, j, true);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("bowType2", j);
                        p.setTemporaryAttribute("stringingBow", (bool)true);
                        return true;
                    }
                }
                for (int j = 0; j < GEMS.Length; j++)
                {
                    if (itemOne == (int)GEMS[j][0] && itemTwo == 1755)
                    {
                        MakeAmmo.displayGemOptions(p, j);
                        setFletchItem(p, null);
                        p.setTemporaryAttribute("boltTips", j);
                        return true;
                    }
                }
                if (itemOne == ARROW_SHAFTS && itemTwo == FEATHER)
                {
                    MakeAmmo.displayAmmoInterface(p, 0, true, false);
                    setFletchItem(p, null);
                    p.setTemporaryAttribute("ammoType", 7);
                    return true;
                }
            }
            return false;
        }

        public static void setFletchItem(Player p, object a)
        {
            if (a == null)
            {
                resetAllFletchVariables(p);
                p.removeTemporaryAttribute("fletchItem");
                return;
            }
            p.setTemporaryAttribute("fletchItem", (object)a);
        }

        private static void resetAllFletchVariables(Player p)
        {
            p.removeTemporaryAttribute("fletchType");
            p.removeTemporaryAttribute("ammoType");
            p.removeTemporaryAttribute("bowType");
            p.removeTemporaryAttribute("ammoType2");
            p.removeTemporaryAttribute("bowType2");
            p.removeTemporaryAttribute("stringingBow");
            p.removeTemporaryAttribute("boltTips");
        }

        public static object getFletchItem(Player p)
        {
            return (object)p.getTemporaryAttribute("fletchItem");
        }
    }
}