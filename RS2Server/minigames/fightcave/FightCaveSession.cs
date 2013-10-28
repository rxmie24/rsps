using RS2.Server.combat;
using RS2.Server.events;
using RS2.Server.model;
using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.player.skills.prayer;
using RS2.Server.util;
using System;

namespace RS2.Server.minigames.fightcave
{
    internal class FightCaveSession
    {
        private Player p;
        private byte currentWave;
        private byte mobAmount;
        private bool completed;
        private bool healersSpawned;
        private bool gamePaused;

        public FightCaveSession(Player player)
        {
            this.p = player;
            this.currentWave = 0;
            this.mobAmount = 0;
            this.completed = false;
            this.healersSpawned = false;
            this.gamePaused = false;
            startGame();
        }

        private void startGame()
        {
            Event startFightCaveGameEvent = new Event(3000);
            startFightCaveGameEvent.setAction(() =>
            {
                if (completed)
                {
                    startFightCaveGameEvent.stop();
                    return;
                }
                if (mobAmount > 0 || currentWave > 63)
                {
                    return;
                }
                if (gamePaused && currentWave != 63)
                {
                    startFightCaveGameEvent.stop();
                    p.getPackets().forceLogout();
                    return;
                }
                if (currentWave == 62)
                {
                    startFightCaveGameEvent.setTick(8000);
                    currentWave++;
                    showJadMessage();
                    return;
                }
                else if (currentWave < 62)
                {
                    currentWave++;
                }
                int[] mobs = decryptWave(currentWave);
                int amount = 0;
                for (int i = 0; i < mobs.Length; i++)
                {
                    if (mobs[i] > 0)
                    {
                        Npc npc = new Npc(mobs[i]);
                        Location minCoords = new Location(((20000 + 2363) + (200 * p.getIndex())), 25051, 0);
                        Location maxCoords = new Location(((20000 + 2430) + (200 * p.getIndex())), 25123, 0);
                        npc.setMinimumCoords(minCoords);
                        npc.setMaximumCoords(maxCoords);
                        npc.setLocation(new Location((20000 + 2387) + (200 * p.getIndex()) + Misc.random(22), 20000 + 5069 + Misc.random(33), 0));
                        npc.setEntityFocus(p.getClientIndex());
                        npc.setOwner(p);
                        npc.setTarget(p);
                        npc.getFollow().setFollowing(p);
                        Server.getNpcList().Add(npc);
                        amount++;
                    }
                }
                mobAmount = (byte)amount;
            });
            Server.registerEvent(startFightCaveGameEvent);
        }

        private void showJadMessage()
        {
            p.getPackets().sendNPCHead(2617, 241, 1);
            p.getPackets().modifyText("TzHaar-Mej-Jal", 241, 3);
            p.getPackets().modifyText("Look out, here comes TzTok-Jad!", 241, 4);
            p.getPackets().animateInterface(9827, 241, 1);
            p.getPackets().sendChatboxInterface2(241);
        }

        public void teleFromCave(bool quit)
        {
            p.teleport(new Location(2439, 5169, 0));
            Server.removeAllPlayersNPCs(p);
            Event teleFromCaveEvent = new Event(600);
            teleFromCaveEvent.setAction(() =>
            {
                teleFromCaveEvent.stop();
                string s = "You have defeated TzTok-Jad, I am most impressed!";
                string s1 = "Please accept this gift as a reward.";
                if (quit)
                {
                    if (currentWave > 1)
                    {
                        s = "Well done in the cave, here, take TokKul as a reward.";
                        s1 = null;
                        p.getInventory().addItemOrGround(6529, getTokkulReward());
                    }
                    else
                    {
                        s = "Well I suppose you tried... better luck next time.";
                        s1 = null;
                    }
                }
                else
                {
                    p.getInventory().addItemOrGround(6570);
                    p.getInventory().addItemOrGround(6529, 16064);
                }
                if (s1 != null)
                {
                    p.getPackets().sendNPCHead(2617, 242, 1);
                    p.getPackets().modifyText("TzHaar-Mej-Jal", 242, 3);
                    p.getPackets().modifyText(s, 242, 4);
                    p.getPackets().modifyText(s1, 242, 5);
                    p.getPackets().animateInterface(9827, 242, 1);
                    p.getPackets().sendChatboxInterface2(242);
                }
                else
                {
                    p.getPackets().sendNPCHead(2617, 241, 1);
                    p.getPackets().modifyText("TzHaar-Mej-Jal", 241, 3);
                    p.getPackets().modifyText(s, 241, 4);
                    p.getPackets().animateInterface(9827, 241, 1);
                    p.getPackets().sendChatboxInterface2(241);
                }
                p.clearKillersHits();
                p.setLastAttackType(1);
                p.setLastAttack(0);
                p.setTarget(null);
                p.setAttacker(null);
                p.getSkills().setCurLevel(Skills.SKILL.HITPOINTS, p.getSkills().getMaxLevel(Skills.SKILL.HITPOINTS));
                p.getPackets().sendSkillLevel(Skills.SKILL.HITPOINTS);
                p.setSkullCycles(0);
                p.setEntityFocus(65535);
                p.getSpecialAttack().resetSpecial();
                p.getEquipment().setWeapon();
                p.setLastkiller(null);
                Combat.resetCombat(p, 1);
                p.setDead(false);
                p.setLastVengeanceTime(0);
                p.setVengeance(false);
                p.removeTemporaryAttribute("willDie");
                p.setFrozen(false);
                p.removeTemporaryAttribute("unmovable");
                p.setAntifireCycles(0);
                p.setSuperAntipoisonCycles(0);
                Prayer.deactivateAllPrayers(p);
                p.setTeleblockTime(0);
                p.removeTemporaryAttribute("teleblocked");
                p.removeTemporaryAttribute("autoCastSpell");
                foreach (Skills.SKILL skill in Enum.GetValues(typeof(Skills.SKILL)))
                    p.getSkills().setCurLevel(skill, p.getSkills().getMaxLevel(skill));
                p.getPackets().sendSkillLevels();
                p.setFightCave(null);
            });
            Server.registerEvent(teleFromCaveEvent);
        }

        public void resumeGame()
        {
            gamePaused = false;
            Location instanceLocation = new Location((20000 + 2413) + (200 * p.getIndex()), 20000 + 5116, 0);
            p.teleport(instanceLocation);
            p.getPackets().sendNPCHead(2617, 242, 1);
            p.getPackets().modifyText("TzHaar-Mej-Jal", 242, 3);
            p.getPackets().modifyText("Welcome back to the fight cave, JalYt.", 242, 4);
            p.getPackets().modifyText("Pepare to fight for your life!", 242, 5);
            p.getPackets().animateInterface(9827, 242, 1);
            p.getPackets().sendChatboxInterface2(242);
            startGame();
        }

        private int getTokkulReward()
        {
            return (int)((currentWave * 6) * (currentWave * 0.34));
        }

        public int[] decryptWave(int waveId)
        {
            int[] mobs = new int[6];
            int mobsAdded = 0;
            if (waveId / 63 > 0)
            {
                for (int i = 0; i <= waveId / 63; i++)
                {
                    mobs[mobsAdded++] = 2745;
                    waveId -= 63;
                }
            }
            if (waveId / 31 > 0)
            {
                for (int i = 0; i <= waveId / 31; i++)
                {
                    mobs[mobsAdded++] = 2743;//+misc.random(1);
                    waveId -= 31;
                }
            }
            if (waveId / 15 > 0)
            {
                for (int i = 0; i <= waveId / 15; i++)
                {
                    mobs[mobsAdded++] = 2741;//+misc.random(1);
                    waveId -= 15;
                }
            }
            if (waveId / 7 > 0)
            {
                for (int i = 0; i <= waveId / 7; i++)
                {
                    mobs[mobsAdded++] = 2739;//+misc.random(1);
                    waveId -= 7;
                }
            }
            if (waveId / 3 > 0)
            {
                for (int i = 0; i <= waveId / 3; i++)
                {
                    mobs[mobsAdded++] = 2736;//+misc.random(1);
                    waveId -= 3;
                }
            }
            if (waveId > 0)
            {
                for (int i = 0; i <= waveId; i++)
                {
                    mobs[mobsAdded++] = 2734;//+misc.random(1);
                    waveId--;
                }
            }
            return mobs;
        }

        public byte getMobAmount()
        {
            return mobAmount;
        }

        public void decreaseMobAmount(bool killedJad)
        {
            this.mobAmount--;
            if (killedJad)
            {
                completed = true;
                teleFromCave(false);
            }
        }

        public void setPlayer(Player p)
        {
            this.p = p;
        }

        public bool isHealersSpawned()
        {
            return healersSpawned;
        }

        public void setHealersSpawned(bool healersSpawned)
        {
            this.healersSpawned = healersSpawned;
        }

        public bool isGamePaused()
        {
            return gamePaused;
        }

        public void setGamePaused(bool gamePaused)
        {
            this.gamePaused = gamePaused;
        }
    }
}