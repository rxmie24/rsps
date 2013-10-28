using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.npc;
using RS2.Server.player;

namespace RS2.Server.packethandler.commands
{
    internal class Info : Command
    {
        public void execute(Player player, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                player.getPackets().sendMessage("[Info command]: ::info npcId (example ::info 1)");
                return;
            }

            int npcId = 0;

            if (!int.TryParse(arguments[0], out npcId))
            {
                player.getPackets().sendMessage("[Info command]: ::info npcId (example ::info 1)");
                return;
            }
            if (npcId < 0 || npcId > NpcData.getTotalNpcDefinitions())
                return;

            player.getPackets().sendMessage("ATT = " + (int)CombatFormula.getMeleeAttack(player) + " DEF = " + (int)CombatFormula.getMeleeDefence(player, player) + " SPEC = " + (int)CombatFormula.getMeleeAttack(player) * CombatFormula.getSpecialAttackBonus(player.getEquipment().getItemInSlot(ItemData.EQUIP.WEAPON)));
            player.getPackets().sendMessage("NPC ATT = " + (int)CombatFormula.getNPCMeleeAttack(new Npc(npcId)) + " NPC DEF = " + (int)CombatFormula.getNPCMeleeDefence(new Npc(npcId)));
        }

        public int minimumRightsNeeded()
        {
            return 0;
        }
    }
}