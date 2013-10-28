using RS2.Server.player;
using System;

namespace RS2.Server.packethandler.commands
{
    internal class Uptime : Command
    {
        public void execute(Player player, string[] arguments)
        {
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount - Server.serverStartupTime); //milliseconds lol.
            player.getPackets().sendMessage("Server been running for: " + (int)uptime.TotalDays + " days, " + uptime.Hours + " hours, " + uptime.Minutes + " minutes, " + uptime.Seconds + " seconds, " + uptime.Milliseconds + " milliseconds.");
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}