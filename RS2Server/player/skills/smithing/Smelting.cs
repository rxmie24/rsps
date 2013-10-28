using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.util;

namespace RS2.Server.player.skills.smithing
{
    internal class Smelting : SmithingData
    {
        public Smelting()
        {
        }

        public static void displaySmeltOptions(Player p)
        {
            int gfxChild = 4;
            int textChild = 16;
            string s = "<br><br><br><br>";
            for (int i = 0; i < BARS.Length; i++)
            {
                bool enabled = p.getSkills().getGreaterLevel(Skills.SKILL.SMITHING) >= SMELT_LEVELS[i];
                string colour = enabled ? "<col=000000>" : "<col=A00000>";
                p.getPackets().itemOnInterface(311, gfxChild, 130, BARS[i]);
                p.getPackets().modifyText(s + colour + BAR_NAMES[i], 311, textChild);
                gfxChild++;
                textChild += 4;
            }
            p.getPackets().sendChatboxInterface(311);
        }

        public static void smeltOre(Player p, int buttonId, bool newSmelt, int amount)
        {
            p.getPackets().closeInterfaces();
            int index = -1;
            if (!newSmelt && p.getTemporaryAttribute("smeltingBar") == null)
            {
                return;
            }
            if (newSmelt)
            {
                resetSmelting(p);
                index = getBarToMake(buttonId);
                if (index == -1)
                {
                    return;
                }
                if (amount == -1)
                {
                    amount = getAmount(buttonId);
                }
                if (amount == -2)
                {
                    p.getPackets().displayEnterAmount();
                    p.setTemporaryAttribute("interfaceVariable", new EnterVariable(311, buttonId));
                    return;
                }
                p.setTemporaryAttribute("smeltingBar", new BarToSmelt(index, BARS[index], SMELT_LEVELS[index], SMELT_XP[index], amount, SMELT_ORES[index], SMELT_ORE_AMT[index], BAR_NAMES[index]));
            }
            BarToSmelt bar = (BarToSmelt)p.getTemporaryAttribute("smeltingBar");
            if (!canSmelt(p, bar))
            {
                resetSmelting(p);
                return;
            }
            for (int i = 0; i < bar.getOre().Length; i++)
            {
                if (bar.getOreAmount()[i] != 0)
                {
                    for (int j = 0; j < bar.getOreAmount()[i]; j++)
                    {
                        if (!p.getInventory().deleteItem(bar.getOre()[i], 1))
                        {
                            resetSmelting(p);
                            return;
                        }
                    }
                }
            }
            string s = bar.getOre()[1] == 0 ? "ore" : "ore and coal";
            p.getPackets().sendMessage("You place the " + bar.getName().ToLower() + " " + s + " into the furnace..");
            p.setLastAnimation(new Animation(3243));
            Event SmeltBarEvent = new Event(1800);
            SmeltBarEvent.setAction(() =>
            {
                SmeltBarEvent.stop();
                if (p.getTemporaryAttribute("smeltingBar") == null)
                {
                    resetSmelting(p);
                    return;
                }
                BarToSmelt myBar = (BarToSmelt)p.getTemporaryAttribute("smeltingBar");
                if (!bar.Equals(myBar))
                {
                    resetSmelting(p);
                    return;
                }
                bool dropIron = false;
                if (bar.getIndex() == 2)
                {
                    if (!wearingForgingRing(p))
                    {
                        if (Misc.random(1) == 0)
                        {
                            p.getPackets().sendMessage("You accidentally drop the iron ore deep into the furnace..");
                            dropIron = true;
                        }
                    }
                    else
                    {
                        lowerForgingRing(p);
                    }
                }
                if (!dropIron)
                {
                    string s1 = bar.getIndex() == 2 || bar.getIndex() == 8 ? "an" : "a";
                    p.getPackets().sendMessage("You retrieve " + s1 + " " + bar.getName().ToLower() + " bar from the furnace..");
                    p.getInventory().addItem(bar.getBarId());
                    p.getSkills().addXp(Skills.SKILL.SMITHING, bar.getXp());
                }
                bar.decreaseAmount();
                if (bar.getAmount() >= 1)
                {
                    smeltOre(p, -1, false, 1);
                }
                if (bar.getAmount() <= 0)
                {
                    resetSmelting(p);
                }
            });
            Server.registerEvent(SmeltBarEvent);
        }

        private static bool canSmelt(Player p, BarToSmelt bar)
        {
            if (bar == null || bar.getAmount() <= 0)
            {
                return false;
            }
            if (p.getSkills().getGreaterLevel(Skills.SKILL.SMITHING) < bar.getLevel())
            {
                p.getPackets().sendMessage("You need a Smithing level of " + bar.getLevel() + " to make that bar.");
                return false;
            }
            for (int i = 0; i < bar.getOre().Length; i++)
            {
                if (bar.getOreAmount()[i] != 0)
                {
                    if (!p.getInventory().hasItemAmount(bar.getOre()[i], bar.getOreAmount()[i]))
                    {
                        p.getPackets().sendMessage("You don't have enough ore to make that bar.");
                        return false;
                    }
                }
            }
            return true;
        }

        private static int getAmount(int buttonId)
        {
            for (int i = 13; i <= 48; i++)
            {
                if (buttonId == i)
                {
                    return SMELT_BUTTON_AMOUNT[i - 13];
                }
            }
            return 0;
        }

        private static int getBarToMake(int buttonId)
        {
            int counter = 0, barIndex = 0;
            for (int i = 13; i <= 48; i++)
            {
                if (counter == 4)
                {
                    barIndex++;
                    counter = 0;
                }
                if (buttonId == i)
                {
                    return barIndex;
                }
                counter++;
            }
            return -1;
        }

        private static void lowerForgingRing(Player p)
        {
            p.setForgeCharge(p.getForgeCharge() - 1);
            if (p.getForgeCharge() <= 0)
            {
                p.setForgeCharge(40);
                p.getPackets().sendMessage("Your Ring of forging has shattered!");
                p.getEquipment().getSlot(ItemData.EQUIP.RING).setItemId(-1);
                p.getEquipment().getSlot(ItemData.EQUIP.RING).setItemAmount(0);
                p.getPackets().refreshEquipment();
            }
        }

        private static bool wearingForgingRing(Player p)
        {
            return p.getEquipment().getItemInSlot(ItemData.EQUIP.RING) == 2568;
        }

        public static void setAmountToZero(Player p)
        {
            if (p.getTemporaryAttribute("smeltingBar") != null)
            {
                BarToSmelt bar = (BarToSmelt)p.getTemporaryAttribute("smeltingBar");
                bar.setAmount(0);
            }
        }

        public static bool wantToSmelt(Player p, int item)
        {
            for (int i = 0; i < SMELT_ORES.Length; i++)
            {
                for (int j = 0; j < SMELT_ORES[i].Length; j++)
                {
                    if (SMELT_ORES[i][j] != 0)
                    {
                        if (item == SMELT_ORES[i][j])
                        {
                            displaySmeltOptions(p);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void resetSmelting(Player p)
        {
            p.setTemporaryAttribute("smeltingBar", null);
        }
    }
}