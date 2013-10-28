using RS2.Server.player.skills.cooking;
using RS2.Server.player.skills.crafting;
using RS2.Server.player.skills.fishing;
using RS2.Server.player.skills.fletching;
using RS2.Server.player.skills.herblore;
using RS2.Server.player.skills.mining;
using RS2.Server.player.skills.smithing;
using RS2.Server.player.skills.woodcutting;

namespace RS2.Server.player.skills
{
    internal class SkillHandler
    {
        public static int SKILLCAPE_PRICE = 250000;

        public static void resetAllSkills(Player p)
        {
            Fletching.setFletchItem(p, null);
            Herblore.setHerbloreItem(p, null);
            Cooking.setCookingItem(p, null);
            Mining.resetMining(p);
            Smithing.resetSmithing(p);
            Woodcutting.resetWoodcutting(p);
            Fishing.resetFishing(p);
            Crafting.resetCrafting(p);
            p.removeTemporaryAttribute("harvesting");
        }
    }
}