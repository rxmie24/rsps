namespace RS2.Server.player.skills.crafting
{
    internal class Crafting : CraftingData
    {
        public Crafting()
        {
        }

        public static bool wantsToCraft(Player p, int itemUsed, int usedWith)
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
                if (itemOne == MOLTEN_GLASS && itemTwo == GLASSBLOWING_PIPE)
                {
                    resetCrafting(p);
                    Glass.displayGlassOption(p);
                    return true;
                }
                for (int j = 0; j < TANNED_HIDE.Length; j++)
                {
                    if (itemOne == TANNED_HIDE[j] && itemTwo == NEEDLE)
                    {
                        resetCrafting(p);
                        Leather.openLeatherInterface(p, j);
                        return true;
                    }
                }
                for (int j = 0; j < GEMS.Length; j++)
                {
                    if (itemOne == CHISEL && itemTwo == (int)GEMS[j][0])
                    {
                        resetCrafting(p);
                        Jewellery.showCutGemOption(p, j);
                        return true;
                    }
                }
                for (int j = 0; j < AMULETS.Length; j++)
                {
                    if (itemOne == BALL_OF_WOOL && itemTwo == (int)AMULETS[j][0])
                    {
                        resetCrafting(p);
                        Jewellery.showStringAmulet(p, j);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool wantsToCraftOnObject(Player p, int item, int craftObject)
        {
            if (craftObject == CLAY_TABLE)
            {
                if (item == CLAY)
                {
                    resetCrafting(p);
                    Clay.displayClayOptions(p, 1);
                    p.setTemporaryAttribute("craftType", 1);
                    return true;
                }
                else
                {
                    for (int j = 0; j < CLAY_ITEMS.Length; j++)
                    {
                        if (item == (int)CLAY_ITEMS[j][0])
                        {
                            p.getPackets().sendMessage("This item must now be baked in a clay oven.");
                            return true;
                        }
                    }
                }
            }
            else if (craftObject == CLAY_OVEN)
            {
                if (craftObject == CLAY)
                {
                    p.getPackets().sendMessage("This clay must be moulded into an item first.");
                    return true;
                }
                for (int j = 0; j < CLAY_ITEMS.Length; j++)
                {
                    if (item == (int)CLAY_ITEMS[j][0])
                    {
                        resetCrafting(p);
                        Clay.displayClayOptions(p, 2);
                        p.setTemporaryAttribute("craftType", 2);
                        return true;
                    }
                }
            }
            else if (craftObject == 11666)
            { // Furnace
                if (item == GOLD_BAR)
                {
                    Jewellery.displayJewelleryInterface(p);
                    return true;
                }
                else if (item == SILVER_BAR)
                {
                    Silver.displaySilverOptions(p);
                    return true;
                }
            }
            return false;
        }

        public static void resetCrafting(Player p)
        {
            p.removeTemporaryAttribute("craftType");
            p.removeTemporaryAttribute("craftItem");
            p.removeTemporaryAttribute("leatherType");
        }
    }
}