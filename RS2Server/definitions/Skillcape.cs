using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class Skillcape
    {
        /**
 * Handles a skill cape emote: checks appropriate levels,
 * finds the correct animation + graphic, etc.
 * @param player
 */

        public static bool emote(Player player)
        {
            Skills.SKILL skill = Skills.SKILL.ATTACK;
            int skillcapeAnimation = -1, skillcapeGraphic = -1;
            Item cape = player.getEquipment().getSlot(ItemData.EQUIP.CAPE);
            if (cape.getItemId() <= 0)
            {
                return false;
            }
            bool didEmote = true;
            switch (cape.getItemId())
            {
                /*
                 * Attack cape.
                 */
                case 9747:
                case 9748:
                    skill = Skills.SKILL.ATTACK;
                    skillcapeAnimation = 4959;
                    skillcapeGraphic = 823;
                    break;
                /*
                 * Defense cape.
                 */
                case 9753:
                case 9754:
                    skill = Skills.SKILL.DEFENCE;
                    skillcapeAnimation = 4961;
                    skillcapeGraphic = 824;
                    break;
                /*
                 * Strength cape.
                 */
                case 9750:
                case 9751:
                    skill = Skills.SKILL.STRENGTH;
                    skillcapeAnimation = 4981;
                    skillcapeGraphic = 828;
                    break;
                /*
                 * Hitpoints cape.
                 */
                case 9768:
                case 9769:
                    skill = Skills.SKILL.HITPOINTS;
                    skillcapeAnimation = 4971;
                    skillcapeGraphic = 833;
                    break;
                /*
                 * Ranging cape.
                 */
                case 9756:
                case 9757:
                    skill = Skills.SKILL.RANGE;
                    skillcapeAnimation = 4973;
                    skillcapeGraphic = 832;
                    break;
                /*
                 * Prayer cape.
                 */
                case 9759:
                case 9760:
                    skill = Skills.SKILL.PRAYER;
                    skillcapeAnimation = 4979;
                    skillcapeGraphic = 829;
                    break;
                /*
                 * Magic cape.
                 */
                case 9762:
                case 9763:
                    skill = Skills.SKILL.MAGIC;
                    skillcapeAnimation = 4939;
                    skillcapeGraphic = 813;
                    break;
                /*
                 * Cooking cape.
                 */
                case 9801:
                case 9802:
                    skill = Skills.SKILL.COOKING;
                    skillcapeAnimation = 4955;
                    skillcapeGraphic = 821;
                    break;
                /*
                 * Woodcutting cape.
                 */
                case 9807:
                case 9808:
                    skill = Skills.SKILL.WOODCUTTING;
                    skillcapeAnimation = 4957;
                    skillcapeGraphic = 822;
                    break;
                /*
                 * Fletching cape.
                 */
                case 9783:
                case 9784:
                    skill = Skills.SKILL.FLETCHING;
                    skillcapeAnimation = 4937;
                    skillcapeGraphic = 812;
                    break;
                /*
                 * Fishing cape.
                 */
                case 9798:
                case 9799:
                    skill = Skills.SKILL.FISHING;
                    skillcapeAnimation = 4951;
                    skillcapeGraphic = 819;
                    break;
                /*
                 * Firemaking cape.
                 */
                case 9804:
                case 9805:
                    skill = Skills.SKILL.FIREMAKING;
                    skillcapeAnimation = 4975;
                    skillcapeGraphic = 831;
                    break;
                /*
                 * Crafting cape.
                 */
                case 9780:
                case 9781:
                    skill = Skills.SKILL.CRAFTING;
                    skillcapeAnimation = 4949;
                    skillcapeGraphic = 818;
                    break;
                /*
                 * Smithing cape.
                 */
                case 9795:
                case 9796:
                    skill = Skills.SKILL.SMITHING;
                    skillcapeAnimation = 4943;
                    skillcapeGraphic = 815;
                    break;
                /*
                 * Mining cape.
                 */
                case 9792:
                case 9793:
                    skill = Skills.SKILL.MINING;
                    skillcapeAnimation = 4941;
                    skillcapeGraphic = 814;
                    break;
                /*
                 * Herblore cape.
                 */
                case 9774:
                case 9775:
                    skill = Skills.SKILL.HERBLORE;
                    skillcapeAnimation = 4969;
                    skillcapeGraphic = 835;
                    break;
                /*
                 * Agility cape.
                 */
                case 9771:
                case 9772:
                    skill = Skills.SKILL.AGILITY;
                    skillcapeAnimation = 4977;
                    skillcapeGraphic = 830;
                    break;
                /*
                 * Thieving cape.
                 */
                case 9777:
                case 9778:
                    skill = Skills.SKILL.THIEVING;
                    skillcapeAnimation = 4965;
                    skillcapeGraphic = 826;
                    break;
                /*
                 * Slayer cape.
                 */
                case 9786:
                case 9787:
                    skill = Skills.SKILL.SLAYER;
                    skillcapeAnimation = 4937;//need animation
                    skillcapeGraphic = 812;//need graphic
                    break;
                /*
                 * Farming cape.
                 */
                case 9810:
                case 9811:
                    skill = Skills.SKILL.FARMING;
                    skillcapeAnimation = 4963;
                    skillcapeGraphic = 825;
                    break;
                /*
                 * Runecraft cape.
                 */
                case 9765:
                case 9766:
                    skill = Skills.SKILL.RUNECRAFTING;
                    skillcapeAnimation = 4947;
                    skillcapeGraphic = 817;
                    break;
                /*
                 * Hunter's cape
                 */
                case 9948:
                case 9949:
                    skill = Skills.SKILL.HUNTER;
                    skillcapeAnimation = 5158;
                    skillcapeGraphic = 907;
                    break;
                /*
                 * Construct. cape.
                 */
                case 9789:
                case 9790:
                    skill = Skills.SKILL.CONSTRUCTION;
                    skillcapeAnimation = 4953;
                    skillcapeGraphic = 820;
                    break;
                /*
                 * Summoning cape.
                 */
                case 12169:
                case 12170:
                    skill = Skills.SKILL.SUMMONING;
                    skillcapeAnimation = 8525;
                    skillcapeGraphic = 1515;
                    break;
                /*
                 * Quest cape.
                 */
                case 9813:
                    skillcapeAnimation = 4945;
                    skillcapeGraphic = 816;
                    player.setLastAnimation(new Animation(skillcapeAnimation));
                    player.setLastGraphics(new Graphics(skillcapeGraphic));
                    return true;

                default:
                    didEmote = false;
                    break;
            }
            if (player.getSkills().getMaxLevel(skill) == 99)
            {
                player.setLastAnimation(new Animation(skillcapeAnimation));
                player.setLastGraphics(new Graphics(skillcapeGraphic));
            }
            else
            {
                didEmote = false;
            }
            return didEmote;
        }
    }
}