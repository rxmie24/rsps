using RS2.Server.model;
using RS2.Server.player;

namespace RS2.Server.definitions
{
    internal class AttackInterface
    {
        public static void configureButton(Player p, int interfaceId, int button)
        {
            AttackStyle av = p.getAttackStyle();
            switch (interfaceId)
            {
                case 92: // Unarmed attack interface.
                    switch (button)
                    {
                        case 2: // Punch (Attack XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(0);
                            break;

                        case 3: // Kick (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(1);
                            break;

                        case 4: // Block (Defence XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 93: // Whip attack interface.
                    switch (button)
                    {
                        case 2: // Flick (Attack XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(0);
                            break;

                        case 3: // Lash (Shared XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.CONTROLLED);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(1);
                            break;

                        case 4: // Deflect (Defence XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 89: // Dagger attack interface.
                    switch (button)
                    {
                        case 2: // Stab (Attack XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(0);
                            break;

                        case 3: // Lunge (Strength XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(1);
                            break;

                        case 4: // Slash (Strength XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(2);
                            break;

                        case 5: // Block (Defence XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 82: // Longsword/scimitar attack interface.
                    switch (button)
                    {
                        case 2: // Chop (Attack XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(0);
                            break;

                        case 3: // Slash (Strength XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(1);
                            break;

                        case 4: // Smash (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;

                        case 5: // Block (Defence XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 78: // Claw attack interface.
                    switch (button)
                    {
                        case 2: // Chop (Attack XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(0);
                            break;

                        case 5: // Slash (Strength XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(1);
                            break;

                        case 4: // Lunge (Shared XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.CONTROLLED);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(2);
                            break;

                        case 3: // Block (Defence XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 81: // Godsword attack interface.
                    switch (button)
                    {
                        case 2: // Chop (Attack XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(0);
                            break;

                        case 3: // Slash (Strength XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(1);
                            break;

                        case 4: // Lunge (Shared XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.CONTROLLED);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(2);
                            break;

                        case 5: // Block (Defence XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 88: // Mace attack interface.
                    switch (button)
                    {
                        case 2: // Pound (Attack XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(0);
                            break;

                        case 3: // Pummel (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(1);
                            break;

                        case 4: // Spike (Shared XP) - Stab
                            av.setSkill(AttackStyle.CombatSkill.CONTROLLED);
                            av.setStyle(AttackStyle.CombatStyle.STAB);
                            av.setSlot(2);
                            break;

                        case 5: // Block (Defence XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 76: // Granite maul attack interface.
                    switch (button)
                    {
                        case 2: // Pound (Attack XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(0);
                            break;

                        case 4: // Pummel (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(1);
                            break;

                        case 3: // Block (Defence XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 77: // Bow attack interface.
                    switch (button)
                    {
                        case 2: // Accurate (Range XP) - Accurate
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_ACCURATE);
                            av.setSlot(0);
                            break;

                        case 4: // Rapid (Range XP) - Rapid
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_RAPID);
                            av.setSlot(1);
                            break;

                        case 3: // Longrange (Range XP) - Defensive
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_DEFENSIVE);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 75: // Battleaxe attack interface.
                    switch (button)
                    {
                        case 2: // Chop (Attack XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(0);
                            break;

                        case 5: // Hack (Strength XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(1);
                            break;

                        case 4: // Smash (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;

                        case 3: // Block (Defence XP) - Slash
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.SLASH);
                            av.setSlot(3);
                            break;
                    }
                    break;

                case 91: // Thrown weapon
                    switch (button)
                    {
                        case 2: // Accurate (Range XP) - Accurate
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_ACCURATE);
                            av.setSlot(0);
                            break;

                        case 4: // Rapid (Range XP) - Rapid
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_RAPID);
                            av.setSlot(1);
                            break;

                        case 3: // Longrange (Range XP) - Defensive
                            av.setSkill(AttackStyle.CombatSkill.RANGE);
                            av.setStyle(AttackStyle.CombatStyle.RANGE_DEFENSIVE);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 85: // Spear
                    switch (button)
                    {
                        case 2: // Bash (Attack XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(0);
                            break;

                        case 4: // Pound (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(1);
                            break;

                        case 3: // Block (Defense XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;
                    }
                    break;

                case 90: // Staff interface
                    switch (button)
                    {
                        case 2: // Bash (Attack XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.ACCURATE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(0);
                            break;

                        case 4: // Pound (Strength XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.AGGRESSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(1);
                            break;

                        case 3: // Focus (Defense XP) - Crush
                            av.setSkill(AttackStyle.CombatSkill.DEFENSIVE);
                            av.setStyle(AttackStyle.CombatStyle.CRUSH);
                            av.setSlot(2);
                            break;
                    }
                    break;
            }
        }

        public static void setButtonForAttackStyle(Player p, int interfaceId)
        {
            if (interfaceId == -1)
            {
                return;
            }
            AttackStyle av = p.getAttackStyle();
            AttackStyle.CombatSkill type = av.getSkill();
            AttackStyle.CombatStyle type2 = av.getStyle();
            int slot = av.getSlot();
            switch (interfaceId)
            {
                case 92: // Unarmed
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 93: // Whip attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.CONTROLLED) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 89: // Dagger attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if ((type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.STAB)) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if ((type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.SLASH)))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 82: // Longsword/scimitar attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.SLASH) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.CRUSH))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 78: // Claw attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.CONTROLLED))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 81: // Godsword attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.CONTROLLED))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 88: // Mace attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.CONTROLLED))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 76: // Granite maul attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 77: // Bow attack interface.
                    if (type2.Equals(AttackStyle.CombatStyle.RANGE_ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type2.Equals(AttackStyle.CombatStyle.RANGE_RAPID) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type2.Equals(AttackStyle.CombatStyle.RANGE_DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 75: // Battleaxe attack interface.
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.SLASH) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) && type2.Equals(AttackStyle.CombatStyle.CRUSH))
                    {
                        p.getPackets().sendConfig(43, 2);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 3);
                        av.setSlot(3);
                    }
                    break;

                case 91: // Thrown weapon
                    if (type2.Equals(AttackStyle.CombatStyle.RANGE_ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type2.Equals(AttackStyle.CombatStyle.RANGE_RAPID) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type2.Equals(AttackStyle.CombatStyle.RANGE_DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 85: // Spear
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;

                case 90: // Staff interface
                    if (type.Equals(AttackStyle.CombatSkill.ACCURATE) || slot == 0)
                    {
                        p.getPackets().sendConfig(43, 0);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.AGGRESSIVE) || slot == 1)
                    {
                        p.getPackets().sendConfig(43, 1);
                    }
                    else if (type.Equals(AttackStyle.CombatSkill.DEFENSIVE) || slot == 2 || slot == 3)
                    {
                        p.getPackets().sendConfig(43, 2);
                        av.setSlot(2);
                    }
                    break;
            }
        }
    }
}