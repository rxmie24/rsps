using RS2.Server.model;

namespace RS2.Server.player
{
    internal class ConfigureAppearance
    {
        /*
         * colours
         * 0 = top
         * 1 = hair
         * 2 = pants
         * 3 = boots
         * 4 = skin
         *
         * looks
         * 0 = hair
         */

        public ConfigureAppearance()
        {
        }

        public static void openInterface(Player p)
        {
            p.getPackets().displayInterface(771);
            p.getPackets().sendPlayerHead(771, 79);
            p.getPackets().animateInterface(9804, 771, 79);
            p.getPackets().sendConfig(1262, p.getAppearance().getGender() == 2 ? 1 : 8);
            p.getAppearance().setTemporaryAppearance(new Appearance());
            p.getAppearance().getTemporaryAppearance().setColoursArray(p.getAppearance().getColoursArray());
            p.getAppearance().getTemporaryAppearance().setLookArray(p.getAppearance().getLookArray());
            p.getAppearance().getTemporaryAppearance().setGender(p.getAppearance().getGender());
            p.removeTemporaryAttribute("hairToggle");
        }

        public static void sortButton(Player p, int button)
        {
            Appearance temp = p.getAppearance().getTemporaryAppearance();
            if (button == 362)
            {
                p.setAppearance(p.getAppearance().getTemporaryAppearance());
                p.getPackets().closeInterfaces();
                return;
            }
            if ((button == 52 && temp.getGender() == 1) || (button == 49 && temp.getGender() == 0))
            {
                p.getPackets().sendConfig(1262, temp.getGender() == 0 ? 1 : 8);
                temp.setGender(temp.getGender() == 0 ? 1 : 0);
                return;
            }
            if (button == 93)
            {
                temp.setLook(0, getHairStyle(p, false));
                return;
            }
            if (button == 93)
            {
                temp.setLook(0, getHairStyle(p, true));
                return;
            }
            if (button >= 151 && button <= 158)
            {
                temp.setColour(4, getSkinColour(p, button));
                return;
            }
            if (button >= 189 && button <= 217)
            {
                temp.setColour(2, getTorsoColour(p, button));
                return;
            }
        }

        private static int getHairStyle(Player p, bool decrease)
        {
            int oldHair = p.getAppearance().getLook(0);
            int newHair = oldHair;
            if (p.getTemporaryAttribute("hairToggle") == null)
            {
                p.setTemporaryAttribute("hairToggle", 0);
                return 0;
            }
            newHair = (int)p.getTemporaryAttribute("hairToggle");
            newHair = decrease ? -1 : +1;
            if (newHair == 11)
            {
                newHair = 186;//
            }
            else if (newHair == 12)
            {
                newHair = 188;//
            }
            else if (newHair == 13)
            {
                newHair = 190;//
            }
            else if (newHair == 14)
            {
                newHair = 192;//
            }
            else if (newHair == 15)
            {
                newHair = 194;//
            }
            else if (newHair == 16)
            {
                newHair = 196; //
            }
            else if (newHair == 17)
            {
                //17 to 25 dump from rs
            }

            p.setTemporaryAttribute("hairToggle", newHair);
            return newHair;
        }

        private static int getSkinColour(Player p, int button)
        {
            int[] buttons = { 151, 152, 153, 154, 155, 156, 157, 158 };
            int[] skins = { 10, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (button == buttons[i])
                {
                    return skins[i];
                }
            }
            return p.getAppearance().getColour(4);
        }

        private static int getTorsoColour(Player p, int button)
        {
            int[] buttons = { 189, 190, 191, 192, 193, 194, 195, 196 };
            int[] colours = { 10, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (button == buttons[i])
                {
                    return colours[i];
                }
            }
            return p.getAppearance().getColour(2);
        }
    }
}