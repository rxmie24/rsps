using RS2.Server.player;
using System.Collections.Generic;

namespace RS2.Server.packethandler.commands
{
    internal static class CommandManager
    {
        private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        static CommandManager()
        {
            commands.Add("t", new Test());

            commands.Add("testgfx", new TestStillGraphics());
            commands.Add("testdmg", new TestDamage());
            commands.Add("uptime", new Uptime());
            commands.Add("players", new Players());
            commands.Add("yell", new Yell());
            commands.Add("item", new Pickup());
            commands.Add("gfx", new Graphic());
            commands.Add("emote", new Animation());
            commands.Add("tele", new Teleport());
            commands.Add("reloadladderxml", new ReloadLadderXml());
            commands.Add("inter", new Interface());
            commands.Add("under", new UnderGround());
            commands.Add("above", new AboveGround());
            commands.Add("height", new Height());
            commands.Add("coords", new Coordinates());
            commands.Add("bank", new Bank());
            commands.Add("npc", new SpawnNpc());
            commands.Add("shop", new OpenShop());
            commands.Add("update", new SystemUpdate());
            commands.Add("obj", new SpawnObject());
            commands.Add("char", new CharacterAppearance());
            commands.Add("empty", new EmptyInventory());
            commands.Add("setlevel", new SetLevel());
            commands.Add("kick", new Kick());
            commands.Add("kickall", new KickAll());
            commands.Add("maxhit", new MaxHit());
            commands.Add("master", new Master());
            commands.Add("pnpc", new PlayerAsNpc());
            commands.Add("clientsideobjectdump", new ClientSideObjectDump());
            commands.Add("config", new Config());
            commands.Add("switch", new SwitchMagic());
            commands.Add("info", new Info());
            commands.Add("spec", new RestoreSpecialAttack());
            commands.Add("value", new InventoryValue());
        }

        public static void execute(Player player, string command)
        {
            string name = "";
            string[] arguments = new string[0];
            int hasCommand = command.IndexOf(' ');
            if (hasCommand > -1)
            {
                name = command.Substring(0, hasCommand);
                arguments = command.Substring(hasCommand + 1).Split(' ');
            }
            else
            {
                name = command;
            }
            name = name.ToLower();
            Command handleCommand;
            if (commands.TryGetValue(name, out handleCommand))
            {
                if (player.getRights() >= handleCommand.minimumRightsNeeded())
                {
                    handleCommand.execute(player, arguments);
                }
            }
        }
    }
}