using RS2.Server.definitions.areas;
using RS2.Server.minigames.agilityarena;
using RS2.Server.minigames.barrows;
using RS2.Server.minigames.warriorguild;
using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class Dialogue
    {
        public Dialogue()
        {
        }

        public static void doDialogue(Player p, int status)
        {
            if (status > 0 && status < 76)
            {
                AgilityArena.doDialogue(p, status);
            }
            else if (status > 76 && status < 100)
            {
                WarriorGuild.talkToKamfreena(p, status);
            }
            else if (status > 100 && status < 125)
            {
                BrokenBarrows.showBobDialogue(p, status);
            }
            else if (status > 155 && status < 200)
            {
                HomeArea.showAliDialogue(p, status);
            }
            else if (status > 200 && status < 235)
            {
                AlKharid.showAliDialogue(p, status);
            }
            else if (status > 239 && status < 270)
            {
                BoatOptions.showBentleyDialogue(p, status);
            }
            else if (status > 279 && status < 300)
            {
                BoatOptions.showCanifisSailorDialogue(p, status);
            }
            else if (status > 299 && status < 330)
            {
                BoatOptions.showJarvaldDialogue(p, status);
            }
            else if (status > 339 && status < 360)
            {
                BoatOptions.showSquireDialogue(p, status);
            }
            else if (status > 370 && status < 400)
            {
                BoatOptions.showArnorDialogue(p, status);
            }
            else if (status > 410 && status < 430)
            {
                BoatOptions.showCaptainBarnabyDialogue(p, status);
            }
        }
    }
}