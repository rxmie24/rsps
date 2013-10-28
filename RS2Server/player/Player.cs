using RS2.Server.clans;
using RS2.Server.combat;
using RS2.Server.definitions;
using RS2.Server.events;
using RS2.Server.minigames.barrows;
using RS2.Server.minigames.duelarena;
using RS2.Server.minigames.fightcave;
using RS2.Server.model;
using RS2.Server.net;
using RS2.Server.npc;
using RS2.Server.player.skills.farming;
using RS2.Server.player.skills.prayer;
using RS2.Server.player.skills.runecrafting;
using RS2.Server.player.skills.slayer;
using RS2.Server.util;
using System;
using System.Collections.Generic;

namespace RS2.Server.player
{
    internal class Player : Entity
    {
        private Connection connection;
        private LoginDetails loginDetails;
        private int playerRights;
        private bool hd; //high definition game.
        private int runEnergy;
        private bool disconnected;

        private bool chat, split, mouse, aid, achievementDiaryTab, autoRetaliate, vengeance, paidAgilityArena;
        private int magicType, forgeCharge, superAntipoisonCycles, antifireCycles;
        private long teleblockTime, lastVengeanceTime;
        private double lastHit, prayerDrainRate;
        private int smallPouchAmount, mediumPouchAmount, largePouchAmount, giantPouchAmount;
        private int poisonAmount, specialAmount, skullCycles, recoilCharges, slayerPoints, defenderWave;
        private int barrowTunnel, barrowKillCount;
        private bool[] barrowBrothersKilled;
        private string[] removedSlayerTasks;
        private DwarfCannon cannon;
        private SlayerTask slayerTask;
        private GESession grandExchangeSession;
        private TradeSession tradeSession;
        private ShopSession shopSession;
        private FightCaveSession fightCave;
        private DuelSession duelSession;
        private int agilityArenaStatus;
        private bool taggedLastAgilityPillar;

        private object distanceEvent;
        private List<Player> tradeRequests;
        private List<Player> duelRequests;

        private Clan clan;
        private Bank bank;
        private Inventory inventory;
        private Equipment equipment;
        private Friends friends;
        private ForceMovement forceMovement;
        private Prayers prayers;
        private Skills skills;
        private AttackStyle attackStyle;
        private Combat.CombatType lastCombatType;
        private Packets packets;
        private LocalEnvironment localEnvironment;
        private Appearance appearance;
        private AppearanceUpdateFlags updateFlags;
        private ChatMessage lastChatMessage;
        private WalkingQueue walkingQueue;
        private SpecialAttack specialAttack;

        private bool clientActive = false;

        public Player(Connection connection)
        {
            this.connection = connection; //without this, new Packets(this); wouldn't function.
            if (connection != null)
                loginDetails = connection.getLoginDetails();
            appearance = new Appearance();
            follow = new Follow(this);
            bank = new Bank(this);
            inventory = new Inventory(this);
            equipment = new Equipment(this);
            friends = new Friends(this);
            prayers = new Prayers(this);
            skills = new Skills(this);
            attackStyle = new AttackStyle();
            packets = new Packets(this);
            localEnvironment = new LocalEnvironment(this);
            updateFlags = new AppearanceUpdateFlags(this);
            walkingQueue = new WalkingQueue(this);
            specialAttack = new SpecialAttack(this);
            chat = true;
            split = false;
            mouse = true;
            aid = false;
            magicType = 1;
            achievementDiaryTab = false;
            forgeCharge = 40;
            smallPouchAmount = 0;
            mediumPouchAmount = 0;
            largePouchAmount = 0;
            giantPouchAmount = 0;
            defenderWave = 0;
            autoRetaliate = false;
            vengeance = false;
            lastVengeanceTime = 0;
            poisonAmount = 0;
            specialAmount = 100;
            skullCycles = 0;
            recoilCharges = 40;
            barrowTunnel = -1;
            barrowKillCount = 0;
            barrowBrothersKilled = new bool[6];
            slayerPoints = 0;
            removedSlayerTasks = new string[4];
            for (int i = 0; i < removedSlayerTasks.Length; i++)
            {
                removedSlayerTasks[i] = "-";
            }
            agilityArenaStatus = 0;
            taggedLastAgilityPillar = false;
            paidAgilityArena = false;
            teleblockTime = 0;
            lastHit = -1;
            prayerDrainRate = 0;
            superAntipoisonCycles = 0;
            antifireCycles = 0;
            tradeRequests = new List<Player>();
            duelRequests = new List<Player>();
        }

        /**
         * Called roughly every 500ms.
         */

        public void tick()
        {
            if (this.inCombat())
            {
                Combat.combatLoop(this);
            }
            if (getFollow().getFollowing() != null)
            {
                getFollow().followEntity();
            }
        }

        public void setConnection(Connection connection)
        {
            this.connection = connection;
        }

        public Connection getConnection()
        {
            return connection;
        }

        public void setLoginDetails(LoginDetails loginDetails)
        {
            this.loginDetails = loginDetails;
        }

        public LoginDetails getLoginDetails()
        {
            return loginDetails;
        }

        public bool isActive()
        {
            return clientActive;
        }

        public void setActive(bool clientActive)
        {
            this.clientActive = clientActive;
        }

        public int getWorld()
        {
            return Constants.WORLD;
        }

        public int getRights()
        {
            return playerRights;
        }

        public void setRights(int playerRights)
        {
            this.playerRights = playerRights;
        }

        public void setHd(bool b)
        {
            this.hd = b;
        }

        public bool isHd()
        {
            return hd;
        }

        public void setRunEnergyLoad(int runEnergy)
        {
            this.runEnergy = runEnergy;
        }

        public void setRunEnergy(int runEnergy)
        {
            this.runEnergy = runEnergy;
            packets.sendEnergy();
        }

        public int getRunEnergy()
        {
            return this.runEnergy;
        }

        public bool isDisconnected()
        {
            return disconnected;
        }

        public void setDisconnected(bool disconnected)
        {
            this.disconnected = disconnected;
        }

        public List<Player> getTradeRequests()
        {
            return tradeRequests;
        }

        public bool wantsToTrade(Player p2)
        {
            foreach (Player p in tradeRequests)
            {
                if (p != null)
                {
                    if (p.Equals(p2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void newTradeRequest(Player p2)
        {
            if (wantsToTrade(p2))
            {
                return;
            }
            tradeRequests.Add(p2);
        }

        public List<Player> getDuelRequests()
        {
            return duelRequests;
        }

        public bool wantsToDuel(Player p2)
        {
            foreach (Player p in duelRequests)
            {
                if (p != null)
                {
                    if (p.Equals(p2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void newDuelRequest(Player p2)
        {
            if (wantsToDuel(p2))
            {
                return;
            }
            duelRequests.Add(p2);
        }

        public Clan getClan()
        {
            return clan;
        }

        public void setClan(Clan clan)
        {
            this.clan = clan;
        }

        public Bank getBank()
        {
            return bank;
        }

        public Inventory getInventory()
        {
            return inventory;
        }

        public Equipment getEquipment()
        {
            return equipment;
        }

        public Friends getFriends()
        {
            return friends;
        }

        public Prayers getPrayers()
        {
            return prayers;
        }

        public Skills getSkills()
        {
            return skills;
        }

        public AttackStyle getAttackStyle()
        {
            return attackStyle;
        }

        public void setLastCombatType(Combat.CombatType lastCombatType)
        {
            this.lastCombatType = lastCombatType;
        }

        public Combat.CombatType getLastCombatType()
        {
            return lastCombatType;
        }

        public Packets getPackets()
        {
            return packets;
        }

        public LocalEnvironment getLocalEnvironment()
        {
            return localEnvironment;
        }

        public Appearance getAppearance()
        {
            return appearance;
        }

        public void setAppearance(Appearance newAppearance)
        {
            this.appearance = newAppearance;
            updateFlags.setAppearanceUpdateRequired(true);
        }

        public AppearanceUpdateFlags getUpdateFlags()
        {
            return updateFlags;
        }

        public ChatMessage getLastChatMessage()
        {
            return lastChatMessage;
        }

        public void setLastChatMessage(ChatMessage msg)
        {
            lastChatMessage = msg;
            updateFlags.setChatTextUpdateRequired(true);
        }

        public ForceMovement getForceMovement()
        {
            return forceMovement;
        }

        public void setForceMovement(ForceMovement movement)
        {
            this.forceMovement = movement;
            updateFlags.setForceMovementRequired(true);
        }

        public WalkingQueue getWalkingQueue()
        {
            return walkingQueue;
        }

        public SpecialAttack getSpecialAttack()
        {
            return specialAttack;
        }

        public GESession getGESession()
        {
            return grandExchangeSession;
        }

        public void setGESession(GESession geSession)
        {
            this.grandExchangeSession = geSession;
        }

        public TradeSession getTrade()
        {
            return tradeSession;
        }

        public void setTrade(TradeSession tradeSession)
        {
            this.tradeSession = tradeSession;
        }

        public ShopSession getShopSession()
        {
            return shopSession;
        }

        public void setShopSession(ShopSession shopSession)
        {
            this.shopSession = shopSession;
        }

        public FightCaveSession getFightCave()
        {
            return fightCave;
        }

        public void setFightCave(FightCaveSession fightCave)
        {
            this.fightCave = fightCave;
        }

        public DuelSession getDuel()
        {
            return duelSession;
        }

        public void setDuelSession(DuelSession duelSession)
        {
            this.duelSession = duelSession;
        }

        public override void setHp(int newHp)
        {
            skills.setCurLevel(Skills.SKILL.HITPOINTS, newHp);
        }

        public override int getHp()
        {
            return skills.getCurLevel(Skills.SKILL.HITPOINTS);
        }

        public override int getMaxHp()
        {
            return skills.getMaxLevel(Skills.SKILL.HITPOINTS);
        }

        public override void heal(int amount)
        {
            if (isDead())
            {
                return;
            }
            if ((skills.getCurLevel(Skills.SKILL.HITPOINTS) + amount) > (skills.getMaxLevel(Skills.SKILL.HITPOINTS)))
            {
                skills.setCurLevel(Skills.SKILL.HITPOINTS, skills.getMaxLevel(Skills.SKILL.HITPOINTS));
                packets.sendSkillLevel(Skills.SKILL.HITPOINTS);
                return;
            }
            skills.setCurLevel(Skills.SKILL.HITPOINTS, skills.getCurLevel(Skills.SKILL.HITPOINTS) + amount);
            packets.sendSkillLevel(Skills.SKILL.HITPOINTS);
        }

        public override bool isDestroyed()
        {
            return !Server.getPlayerList().Contains(this);
        }

        public override int getMaxHit()
        {
            return (int)CombatFormula.getPlayerMaxHit(this, 0);
        }

        public double getMaxHit(int strengthModifier)
        {
            return CombatFormula.getPlayerMaxHit(this, strengthModifier);
        }

        public override void hit(int damage)
        {
            if (duelSession != null)
            {
                if (duelSession.getStatus() >= 8)
                {
                    return;
                }
            }
            hit(damage, damage == 0 ? Hits.HitType.NO_DAMAGE : Hits.HitType.NORMAL_DAMAGE);
        }

        public override void hit(int damage, Hits.HitType type)
        {
            if (isDead())
            {
                damage = 0;
                type = Hits.HitType.NO_DAMAGE;
            }
            bool redemption = prayers.getHeadIcon() == PrayerData.REDEMPTION;
            byte maxHp = (byte)skills.getMaxLevel(Skills.SKILL.HITPOINTS);
            byte newHp = (byte)(skills.getMaxLevel(Skills.SKILL.HITPOINTS) - damage);
            if (redemption)
            {
                if (newHp >= 1 && newHp <= maxHp * 0.10)
                {
                    setLastGraphics(new Graphics(436, 0, 0));
                    packets.sendMessage("Using your prayer skill, you heal yourself.");
                    skills.setCurLevel(Skills.SKILL.PRAYER, 0);
                    packets.sendSkillLevel(Skills.SKILL.PRAYER);
                    heal((int)(skills.getMaxLevel(Skills.SKILL.PRAYER) * 0.25));
                }
            }
            if (duelSession != null)
            {
                if (duelSession.getStatus() >= 8)
                    return;
            }
            else
            {
                if (newHp >= 1 && newHp <= maxHp * 0.10 && !redemption)
                {
                    if (equipment.getItemInSlot(ItemData.EQUIP.RING) == 2570)
                    {
                        teleport(new Location(3221 + Misc.random(1), 3217 + Misc.random(3), 0));
                        packets.sendMessage("Your ring of life shatters whilst teleporting you to safety.");
                        equipment.getSlot(ItemData.EQUIP.RING).setItemId(-1);
                        equipment.getSlot(ItemData.EQUIP.RING).setItemAmount(0);
                        packets.refreshEquipment();
                        queuedHits.Clear();
                        Combat.resetCombat(this, 1);
                        return;
                    }
                }
            }
            bool damageOverZero = damage > 0;
            if (damage > skills.getCurLevel(Skills.SKILL.HITPOINTS))
            {
                damage = skills.getCurLevel(Skills.SKILL.HITPOINTS);
            }
            if (damageOverZero && damage == 0)
            {
                type = Hits.HitType.NO_DAMAGE;
            }
            if (!updateFlags.isHitUpdateRequired())
            {
                getHits().setHit1(new Hits.Hit(damage, type));
                updateFlags.setHitUpdateRequired(true);
            }
            else
            {
                if (!updateFlags.isHit2UpdateRequired())
                {
                    getHits().setHit2(new Hits.Hit(damage, type));
                    updateFlags.setHit2UpdateRequired(true);
                }
                else
                {
                    lock (queuedHits)
                    {
                        queuedHits.Enqueue(new Hits.Hit(damage, type));
                    }
                    return;
                }
            }
            skills.setCurLevel(Skills.SKILL.HITPOINTS, skills.getCurLevel(Skills.SKILL.HITPOINTS) - damage);
            if (skills.getCurLevel(Skills.SKILL.HITPOINTS) <= 0)
            {
                skills.setCurLevel(Skills.SKILL.HITPOINTS, 0);
                if (!isDead())
                {
                    Server.registerEvent(new DeathEvent(this));
                    setDead(true);
                }
            }
            packets.sendSkillLevel(Skills.SKILL.HITPOINTS);
        }

        public override void dropLoot()
        {
            Entity killer = this.getKiller();
            Player klr = killer is Npc ? null : (Player)killer;
            if (klr == null)
            {
                klr = this;
            }
            int amountToKeep = isSkulled() ? 0 : 3;
            if (prayers.isProtectItem())
            {
                amountToKeep = isSkulled() ? 1 : 4;
            }

            /*
             * Meh wanted to make a much more effient system.
            //price of item, [item, object[]=(inventory/euqipment)]
            //Inventory Items
            SortedDictionary<int, object[]> valueSortedItems = new SortedDictionary<int, object[]>();
            Item item;
            int price = 0;
            for(int i = 0; i < Inventory.MAX_INVENTORY_SLOTS; i++) {
                item = inventory.getSlot(i);
                if(item.getItemId() != -1 && item.getItemAmount() > 0) { //is real item.
                    //price of item stacked
                    price = item.getItemAmount() * item.getDefinition().getPrice().getMaximumPrice();
                    valueSortedItems.Add(price, new object[] {item, 0});
                }
            }
            //Equipment Items
            for(int i = 0; i < 14; i++) {
                item = equipment.getSlot(i);
                if(item.getItemId() != -1 && item.getItemAmount() > 0) { //is real item.
                    //price of item stacked
                    price = item.getItemAmount() * item.getDefinition().getPrice().getMaximumPrice();
                    valueSortedItems.Add(price, new object[] {item, 1});
                }
            }*/

            int[] protectedItems = new int[amountToKeep];
            bool[] saved = new bool[amountToKeep];
            if (protectedItems.Length > 0)
            {
                protectedItems[0] = ProtectedItems.getProtectedItem1(this)[0];
            }
            if (protectedItems.Length > 1)
            {
                protectedItems[1] = ProtectedItems.getProtectedItem2(this)[0];
            }
            if (protectedItems.Length > 2)
            {
                protectedItems[2] = ProtectedItems.getProtectedItem3(this)[0];
            }
            if (protectedItems.Length > 3)
            {
                protectedItems[3] = ProtectedItems.getProtectedItem4(this)[0];
            }
            bool save = false;
            for (int i = 0; i < 28; i++)
            {
                save = false;
                Item item = inventory.getSlot(i);
                if (item.getItemId() > 0)
                {
                    for (int j = 0; j < protectedItems.Length; j++)
                    {
                        if (amountToKeep > 0 && protectedItems[j] > 0)
                        {
                            if (!saved[j] && !save)
                            {
                                if (item.getItemId() == protectedItems[j] && item.getItemAmount() == 1)
                                {
                                    saved[j] = true;
                                    save = true;
                                }
                                if (item.getItemId() == protectedItems[j] && item.getItemAmount() > 1)
                                {
                                    item.setItemAmount(item.getItemAmount() - 1);
                                    saved[j] = true;
                                    save = true;
                                }
                            }
                        }
                    }
                    if (!save)
                    {
                        int itemId = item.getItemId();
                        if (itemId >= 4708 && itemId <= 4759)
                        {
                            itemId = BrokenBarrows.getBrokenId(itemId);
                        }
                        GroundItem gi = new GroundItem(itemId, item.getItemAmount(), this.getLocation(), item.getDefinition().isPlayerBound() ? this : klr);
                        Server.getGroundItems().newEntityDrop(gi);
                    }
                }
            }
            inventory.clearAll();
            saved = new bool[amountToKeep];
            foreach (ItemData.EQUIP equip in Enum.GetValues(typeof(ItemData.EQUIP)))
            {
                if (equip == ItemData.EQUIP.NOTHING) continue;
                save = false;
                Item item = this.getEquipment().getSlot(equip);
                if (item.getItemId() > 0)
                {
                    for (int j = 0; j < protectedItems.Length; j++)
                    {
                        if (amountToKeep > 0 && protectedItems[j] > -1)
                        {
                            if (!saved[j] && !save)
                            {
                                if (item.getItemId() == protectedItems[j] && item.getItemAmount() == 1)
                                {
                                    saved[j] = true;
                                    save = true;
                                }
                                if (item.getItemId() == protectedItems[j] && item.getItemAmount() > 1)
                                {
                                    item.setItemAmount(item.getItemAmount() - 1);
                                    saved[j] = true;
                                    save = true;
                                }
                            }
                        }
                    }
                    if (!save)
                    {
                        int itemId = item.getItemId();
                        if (itemId >= 4708 && itemId <= 4759)
                        {
                            itemId = BrokenBarrows.getBrokenId(itemId);
                        }
                        GroundItem gi = new GroundItem(itemId, item.getItemAmount(), this.getLocation(), item.getDefinition().isPlayerBound() ? this : klr);
                        Server.getGroundItems().newEntityDrop(gi);
                    }
                }
            }
            equipment.clearAll();
            GroundItem bones = new GroundItem(526, 1, this.getLocation(), klr);
            Server.getGroundItems().newEntityDrop(bones);
            inventory.setProtectedItems(protectedItems);
        }

        public override int getAttackAnimation()
        {
            return !this.appearance.isNpc() ? Animations.getAttackAnimation(this) : NpcData.forId(this.appearance.getNpcId()).getAttackAnimation();
        }

        public override int getAttackSpeed()
        {
            if (getMiasmicEffect() > 0)
            {
                return Animations.getAttackSpeed(this) * 2;
            }
            return Animations.getAttackSpeed(this);
        }

        public override int getDeathAnimation()
        {
            return !this.appearance.isNpc() ? 7185 : NpcData.forId(this.appearance.getNpcId()).getDeathAnimation();
        }

        public override int getDefenceAnimation()
        {
            return !this.appearance.isNpc() ? Animations.getDefenceAnimation(this) : NpcData.forId(this.appearance.getNpcId()).getDefenceAnimation();
        }

        public override int getHitDelay()
        {
            return Animations.getPlayerHitDelay(this);
        }

        public void refresh()
        {
            getFriends().login();
            getPackets().sendConfig(171, !chat ? 1 : 0);
            getPackets().sendConfig(287, split ? 1 : 0);
            if (split)
            {
                getPackets().sendBlankClientScript(83, "s");
            }
            getPackets().sendConfig(170, !mouse ? 1 : 0);
            getPackets().sendConfig(427, aid ? 1 : 0);
            getPackets().sendConfig(172, !autoRetaliate ? 1 : 0);
            if (magicType != 1)
            {
                getPackets().sendTab(isHd() ? 99 : 89, magicType == 2 ? 193 : 430);
            }
            if (achievementDiaryTab)
            {
                getPackets().sendTab(isHd() ? 95 : 85, 259);
            }
            RuneCraft.toggleRuin(this, getEquipment().getItemInSlot(ItemData.EQUIP.HAT), RuneCraft.wearingTiara(this));
            getSpecialAttack().setSpecialAmount(specialAmount);
            setPoisonAmount(poisonAmount);
            if (poisonAmount > 0)
            {
                Server.registerEvent(new PoisonEvent((Entity)this, poisonAmount));
            }
            if (teleblockTime > 0)
            {
                if (teleblockTime > Environment.TickCount)
                {
                    long delay = teleblockTime - Environment.TickCount;
                    setTemporaryAttribute("teleblocked", true);
                    Event removeTeleBlockEvent = new Event(delay);
                    removeTeleBlockEvent.setAction(() =>
                    {
                        removeTeleBlockEvent.stop();
                        removeTemporaryAttribute("teleblocked");
                        teleblockTime = 0;
                    });
                    Server.registerEvent(removeTeleBlockEvent);
                }
            }
            Farming.refreshPatches(this);
            getEquipment().refreshBonuses();
            if (fightCave != null)
            {
                fightCave.setPlayer(this);
                fightCave.resumeGame();
            }
            setSkullCycles(skullCycles); // This method updates the appearance, so have this last.
        }

        public int getAgilityArenaStatus()
        {
            return agilityArenaStatus;
        }

        public void setAgilityArenaStatus(int agilityArenaStatus)
        {
            this.agilityArenaStatus = agilityArenaStatus;
        }

        public bool isAchievementDiaryTab()
        {
            return achievementDiaryTab;
        }

        public bool isMouseTwoButtons()
        {
            return mouse;
        }

        public bool isChatEffectsEnabled()
        {
            return chat;
        }

        public bool isPrivateChatSplit()
        {
            return split;
        }

        public bool isAcceptAidEnabled()
        {
            return aid;
        }

        public void setMouseTwoButtons(bool mouse)
        {
            this.mouse = mouse;
        }

        public void setChatEffectsEnabled(bool chat)
        {
            this.chat = chat;
        }

        public void setPrivateChatSplit(bool split)
        {
            this.split = split;
            if (split)
            {
                getPackets().sendBlankClientScript(83, "s");
            }
        }

        public void setAcceptAidEnabled(bool aid)
        {
            this.aid = aid;
        }

        public int getMagicType()
        {
            return magicType;
        }

        public void setMagicType(int magicType)
        {
            this.magicType = magicType;
        }

        public int getDefenderWave()
        {
            return defenderWave;
        }

        public void setDefenderWave(int defenderWave)
        {
            this.defenderWave = defenderWave;
        }

        public void setAchievementDiaryTab(bool b)
        {
            this.achievementDiaryTab = b;
        }

        public int getSmallPouchAmount()
        {
            return smallPouchAmount;
        }

        public void setSmallPouchAmount(int smallPouchAmount)
        {
            this.smallPouchAmount = smallPouchAmount;
        }

        public int getMediumPouchAmount()
        {
            return mediumPouchAmount;
        }

        public void setMediumPouchAmount(int mediumPouchAmount)
        {
            this.mediumPouchAmount = mediumPouchAmount;
        }

        public int getLargePouchAmount()
        {
            return largePouchAmount;
        }

        public void setLargePouchAmount(int largePouchAmount)
        {
            this.largePouchAmount = largePouchAmount;
        }

        public int getGiantPouchAmount()
        {
            return giantPouchAmount;
        }

        public void setGiantPouchAmount(int giantPouchAmount)
        {
            this.giantPouchAmount = giantPouchAmount;
        }

        public int getSkullCycles()
        {
            return skullCycles;
        }

        public void renewSkull()
        {
            setSkullCycles(20);
        }

        public bool isSkulled()
        {
            return skullCycles > 0;
        }

        public void setSkullCycles(int amt)
        {
            this.skullCycles = amt;
            getPrayers().setPkIcon(isSkulled() ? 0 : -1);
        }

        public void setAutoRetaliate(bool autoRetaliate)
        {
            this.autoRetaliate = autoRetaliate;
        }

        public void toggleAutoRetaliate()
        {
            this.autoRetaliate = !autoRetaliate;
            getPackets().sendConfig(172, !autoRetaliate ? 1 : 0);
        }

        public override bool isAutoRetaliating()
        {
            return autoRetaliate;
        }

        public void setSpecialAmount(int specialAmount)
        {
            this.specialAmount = specialAmount;
        }

        public void setBarrowTunnel(int barrowTunnel)
        {
            this.barrowTunnel = barrowTunnel;
        }

        public int getBarrowTunnel()
        {
            return barrowTunnel;
        }

        public void setBarrowKillCount(int i)
        {
            this.barrowKillCount = i;
            if (barrowKillCount > 9999)
            {
                barrowKillCount = 9999;
            }
        }

        public int getBarrowKillCount()
        {
            return barrowKillCount;
        }

        public void setBarrowBrothersKilled(int i, bool b)
        {
            this.barrowBrothersKilled[i] = b;
        }

        public bool getBarrowBrothersKilled(int i)
        {
            return barrowBrothersKilled[i];
        }

        public void setRecoilCharges(int i)
        {
            this.recoilCharges = i;
        }

        public int getRecoilCharges()
        {
            return recoilCharges;
        }

        public void setVengeance(bool vengeance)
        {
            this.vengeance = vengeance;
        }

        public bool hasVengeance()
        {
            return vengeance;
        }

        public void setCannon(DwarfCannon cannon)
        {
            this.cannon = cannon;
        }

        public DwarfCannon getCannon()
        {
            return cannon;
        }

        public void setSlayerTask(SlayerTask task)
        {
            this.slayerTask = task;
        }

        public SlayerTask getSlayerTask()
        {
            return slayerTask;
        }

        public int getSlayerPoints()
        {
            return slayerPoints;
        }

        public void setSlayerPoints(int i)
        {
            this.slayerPoints = i;
        }

        public void setRemovedSlayerTask(int index, string monster)
        {
            this.removedSlayerTasks[index] = monster;
        }

        public string[] getRemovedSlayerTasks()
        {
            return removedSlayerTasks;
        }

        public void setRemovedSlayerTask(string[] tasks)
        {
            removedSlayerTasks = tasks;
        }

        public bool isTaggedLastAgilityPillar()
        {
            return taggedLastAgilityPillar;
        }

        public void setTaggedLastAgilityPillar(bool b)
        {
            taggedLastAgilityPillar = b;
        }

        public void setPaidAgilityArena(bool paidAgilityArena)
        {
            this.paidAgilityArena = paidAgilityArena;
        }

        public bool hasPaidAgilityArena()
        {
            return paidAgilityArena;
        }

        public double getLastHit()
        {
            return lastHit;
        }

        public void setLastHit(double hit)
        {
            this.lastHit = hit;
        }

        public long getTeleblockTime()
        {
            return teleblockTime;
        }

        public void setTeleblockTime(long l)
        {
            teleblockTime = l;
        }

        public int getForgeCharge()
        {
            return forgeCharge;
        }

        public void setForgeCharge(int forgeCharge)
        {
            this.forgeCharge = forgeCharge;
        }

        public void setTzhaarSkull()
        {
            getPrayers().setPkIcon(1);
        }

        public long getLastVengeanceTime()
        {
            return lastVengeanceTime;
        }

        public void setLastVengeanceTime(long time)
        {
            this.lastVengeanceTime = time;
        }

        public void decreasePrayerPoints(double modification)
        {
            int lvlBefore = getSkills().getCurLevel(Skills.SKILL.PRAYER);
            prayerDrainRate += modification;

            if (getSkills().getCurLevel(Skills.SKILL.PRAYER) > 0 && prayerDrainRate > 1)
            {
                int drainedInteger = (int)Math.Floor(prayerDrainRate);
                prayerDrainRate -= drainedInteger;
                getSkills().setCurLevel(Skills.SKILL.PRAYER, getSkills().getCurLevel(Skills.SKILL.PRAYER) - drainedInteger);
                getPackets().sendSkillLevel(Skills.SKILL.PRAYER);
            }
        }

        public void setSuperAntipoisonCycles(int superAntipoisonCycles)
        {
            this.superAntipoisonCycles = superAntipoisonCycles;
        }

        public int getSuperAntipoisonCycles()
        {
            return superAntipoisonCycles;
        }

        public void setAntifireCycles(int antifireCycles)
        {
            this.antifireCycles = antifireCycles;
        }

        public int getAntifireCycles()
        {
            return antifireCycles;
        }

        public object getDistanceEvent()
        {
            return distanceEvent;
        }

        public void setDistanceEvent(object distanceEvent)
        {
            this.distanceEvent = distanceEvent;
        }
    }
}