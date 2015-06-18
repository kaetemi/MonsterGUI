using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using SimpleJSON;

// This file specifies all game abilities and upgrades, and handles parsing and reading of all game state responses.

namespace MonsterGUI
{
	/// <summary>
	/// Numeric values for abilities
	/// </summary>
	enum Abilities
	{
		Invalid = 0,

		Attack = 1,
		ChangeLane = 2,
		Respawn = 3,
		ChangeTarget = 4,

		StartAbility = 5,
		MoraleBooster = 5, // IncreaseDamage
		GoodLuckCharm = 6, // IncreaseCritPercentage
		Medics = 7, // Heal
		MetalDetector = 8, // IncreaseGoldDropped
		Cooldown = 9, // DecreaseCooldowns

		Nuke = 10,
		ClusterBomb = 11,
		Napalm = 12,

		StartItem = 13,
		Revive = 13,
		CrippleSpawner = 14,
 		CrippleMonster = 15,
		MaximizeElement = 16,
		GoldRain = 17,
		IncreaseCritPercentagePermanently = 18,
		IncreaseHPPermanently = 19,
		GoldForDamage = 20,
		GodMode = 21,
		GiveGold = 22, 
		StealHealth = 23,
		ReflectDamage = 24,
		RandomItem = 25,
		Wormhole = 26,
		ClearCool = 27,

		Nb = 28,
		Max = 64
	}

	enum EnemyType
	{
		None = -1,
		Spawner = 0,
		Creep = 1,
		Boss = 2,
		MiniBoss = 3,
		Treasure = 4
	}

	// g_TuningData
	enum UpgradeOption
	{
		LightArmor = 0,
		AutoFireCannon = 1,
		ArmorPiercingRound = 2,
		ElementalFire = 3,
		ElementalWater = 4,
		ElementalAir = 5,
		ElementalEarth = 6,
		LuckyShot = 7,
		HeavyArmor = 8,
		AdvancedTargeting = 9,
		ExplosiveRounds = 10,
		Medics = 11,
		MoraleBooster = 12,
		GoodLuckCharms = 13,
		MetalDetector = 14,
		DecreaseCooldowns = 15,
		TacticalNuke = 16,
		ClusterBomb = 17,
		Napalm = 18,
		BossLoot = 19,
		EnergyShields = 20,
		FarmingEquipment = 21,
		Railgun = 22,
		PersonalTraining = 23,
		AFKEquipment = 24,
		NewMouseButton = 25,
		HPUpgrade5 = 26,
		DPSUpgrade5 = 27,
		ClickUpgrade5 = 28,

		Nb = 29,
		Max = 64
	}

	enum UpgradeType
	{
		HitPoints = 0,
		DPS = 1,
		ClickDamage = 2,
		DamageMultiplier_Fire = 3,
		DamageMultiplier_Water = 4,
		DamageMultiplier_Air = 5,
		DamageMultiplier_Earth = 6,
		DamageMultiplier_Crit = 7,
		PurchaseAbility = 8,
		BossLootDropPercentage = 9,
		Nb = 10
	}

	struct PlayerData
	{
		public double Hp;
		public double Gold;

		public int CurrentLane;
		public int Target;

		public int TimeDied;

		public ulong ActiveAbilitiesBitfield;
	}

	struct Enemy
	{
		public double Hp;
		public double MaxHp;

		public EnemyType Type;

		// type
		// hp
		// gold
		// etc
	}

	struct Lane
	{
		// dps
		// enemies
		// active player abilities
		// etc

		public void Init()
		{
			// NOTE: Fixed array sizes as we are accessing from multiple threads without locking
			Enemies = new Enemy[4]; 
		}

		public Enemy[] Enemies;
		public double ActivePlayerAbilityGoldPerClick;
		public UpgradeOption Element;
	}

	struct GameData
	{
		public void Init()
		{
			// NOTE: Fixed array sizes as we are accessing from multiple threads without locking
			Lanes = new Lane[3];
			for (int i = 0; i < Lanes.Length; ++i)
				Lanes[i].Init();
		}

		public long Level;

		public Lane[] Lanes;

		public long Timestamp;
		public long TimestampLevelStart;

	/*	"timestamp": 1434212164,
			"status": 2,
			"timestamp_game_start": 1434211782,
			"timestamp_level_start": 1434212163*/
	}

	struct Stats
	{

		public int NumActivePlayers;
		public long NumClicks;

		/*	"num_players": 1500,
			"num_mobs_killed": "355",
			"num_towers_killed": "118",
			"num_minibosses_killed": "24",
			"num_bosses_killed": "4",
			"num_clicks": "24690630687",
			"num_abilities_activated": "180",
			"num_ability_items_activated": "4298",
			"num_active_players": 1213,
			"time_simulating": 2.7457846948787088,
			"time_saving": 28.79324529773146*/
	}

	/// <summary>
	/// See TechTreeExample.txt
	/// </summary>
	struct TechTree
	{
		public void Init()
		{
			// NOTE: Fixed array sizes as we are accessing from multiple threads without locking
			Upgrades = new Upgrade[(int)UpgradeOption.Max];
			AbilityItems = new int[(int)Abilities.Max];
		}

		public struct Upgrade
		{
			public int Level;
			public double CostForNextLevel;
		}

		public Upgrade[] Upgrades;

		public ulong UnlockedAbilitiesBitfield;

		public int[] AbilityItems;

		public double CritPercentage;
		public long BadgePoints;

	}

	struct TuningData
	{
		public void Init()
		{
			// NOTE: Fixed array sizes as we are accessing from multiple threads without locking
			Upgrades = new Upgrade[(int)UpgradeOption.Max];
			for (int i = 0; i < Upgrades.Length; ++i)
				Upgrades[i].Type = UpgradeType.Nb;
		}

		public struct PlayerStruct
		{
			public double Hp;
			public double Dps;
			public double DamagePerClick;
			public double DamageMultiplierFire;
			public double DamageMultiplierCrit;
			public double CritPercentage;
			public double LootChance;
		}

		public struct Upgrade
		{
			public double Multiplier;
			public UpgradeType Type;
			public int RequiredUpgrade;
			public int RequiredUpgradeLevel;
		};

		public PlayerStruct Player;
		public Upgrade[] Upgrades;
	}

	public partial class MainWindow
	{
		// Game state data
		PlayerData playerData = new PlayerData();
		GameData gameData = new GameData();
		Stats stats = new Stats();
		TechTree techTree = new TechTree();
		TuningData tuningData = new TuningData();

		volatile bool getPlayerNames = false;
		bool getSteamId = false;
		bool getTuningData = false;

		string steamId = "";
		string personaName = "";

		/// <summary>
		/// App init
		/// </summary>
		private void getStateInit()
		{
			resultTokenDetailsDelegate = new JsonCallback(resultTokenDetails);
			resultPlayerNamesDelegate = new JsonCallback(resultPlayerNames);
			resultGameDataDelegate = new JsonCallback(resultGameData);
			resultTuningDataDelegate = new JsonCallback(resultTuningData);
			gameData.Init();
			techTree.Init();
			tuningData.Init();
		}

		/// <summary>
		/// User GO
		/// </summary>
		private void getStateGo()
		{
			steamId = "";

			getPlayerNames = true;
			getSteamId = true;
			getTuningData = true;

			abilitiesIntfs = new System.Windows.Forms.Label[8];
			abilitiesIntfs[(int)Abilities.MoraleBooster - (int)Abilities.StartAbility] = moraleBoosterIntf;
			abilitiesIntfs[(int)Abilities.GoodLuckCharm - (int)Abilities.StartAbility] = goodLuckCharmIntf;
			abilitiesIntfs[(int)Abilities.Medics - (int)Abilities.StartAbility] = medicsIntf;
			abilitiesIntfs[(int)Abilities.MetalDetector - (int)Abilities.StartAbility] = metalDetectorIntf;
			abilitiesIntfs[(int)Abilities.Cooldown - (int)Abilities.StartAbility] = coolDownIntf;
			abilitiesIntfs[(int)Abilities.Nuke - (int)Abilities.StartAbility] = tacticalNukeIntf;
			abilitiesIntfs[(int)Abilities.ClusterBomb - (int)Abilities.StartAbility] = clusterBombIntf;
			abilitiesIntfs[(int)Abilities.Napalm - (int)Abilities.StartAbility] = napalmIntf;

			itemsIntfs = new System.Windows.Forms.Label[15];
			itemsIntfs[(int)(int)Abilities.Revive - (int)Abilities.StartItem] = resurrIntf;
			itemsIntfs[(int)(int)Abilities.CrippleSpawner - (int)Abilities.StartItem] = crippleSpawnIntf;
			itemsIntfs[(int)(int)Abilities.CrippleMonster - (int)Abilities.StartItem] = crppleMonstIntf;
			itemsIntfs[(int)(int)Abilities.MaximizeElement - (int)Abilities.StartItem] = maxEleIntf;
			itemsIntfs[(int)(int)Abilities.GoldRain - (int)Abilities.StartItem] = rainGoldIntf;
			itemsIntfs[(int)(int)Abilities.IncreaseCritPercentagePermanently - (int)Abilities.StartItem] = critPermIntf;
			itemsIntfs[(int)(int)Abilities.IncreaseHPPermanently - (int)Abilities.StartItem] = hpPermIntf;
			itemsIntfs[(int)(int)Abilities.GoldForDamage - (int)Abilities.StartItem] = throwGoldIntf;
			itemsIntfs[(int)(int)Abilities.GodMode - (int)Abilities.StartItem] = godModeIntf;
			itemsIntfs[(int)(int)Abilities.GiveGold - (int)Abilities.StartItem] = treasureIntf;
			itemsIntfs[(int)(int)Abilities.StealHealth - (int)Abilities.StartItem] = stealHpIntf;
			itemsIntfs[(int)(int)Abilities.ReflectDamage - (int)Abilities.StartItem] = reflctDmgIntf;
			itemsIntfs[(int)(int)Abilities.RandomItem - (int)Abilities.StartItem] = giveRandomIntf;
			itemsIntfs[(int)(int)Abilities.Wormhole - (int)Abilities.StartItem] = skipLevelIntf;
			itemsIntfs[(int)(int)Abilities.ClearCool - (int)Abilities.StartItem] = clearCoolIntf;

			itemsCounts = new System.Windows.Forms.Label[15];
			itemsCounts[(int)(int)Abilities.Revive - (int)Abilities.StartItem] = resurrCount;
			itemsCounts[(int)(int)Abilities.CrippleSpawner - (int)Abilities.StartItem] = crpplSpawnCount;
			itemsCounts[(int)(int)Abilities.CrippleMonster - (int)Abilities.StartItem] = crippleMonstCount;
			itemsCounts[(int)(int)Abilities.MaximizeElement - (int)Abilities.StartItem] = maxEleCount;
			itemsCounts[(int)(int)Abilities.GoldRain - (int)Abilities.StartItem] = rainGoldCount;
			itemsCounts[(int)(int)Abilities.IncreaseCritPercentagePermanently - (int)Abilities.StartItem] = critPermCount;
			itemsCounts[(int)(int)Abilities.IncreaseHPPermanently - (int)Abilities.StartItem] = hpPermCount;
			itemsCounts[(int)(int)Abilities.GoldForDamage - (int)Abilities.StartItem] = throwGoldCount;
			itemsCounts[(int)(int)Abilities.GodMode - (int)Abilities.StartItem] = godModeCount;
			itemsCounts[(int)(int)Abilities.GiveGold - (int)Abilities.StartItem] = treasureCount;
			itemsCounts[(int)(int)Abilities.StealHealth - (int)Abilities.StartItem] = stealHpCount;
			itemsCounts[(int)(int)Abilities.ReflectDamage - (int)Abilities.StartItem] = reflectDmgCount;
			itemsCounts[(int)(int)Abilities.RandomItem - (int)Abilities.StartItem] = giveRandomCount;
			itemsCounts[(int)(int)Abilities.Wormhole - (int)Abilities.StartItem] = skipLevelCount;
			itemsCounts[(int)(int)Abilities.ClearCool - (int)Abilities.StartItem] = clearCoolCount;

			upgradeIntf[(int)UpgradeType.HitPoints] = upgrStatHP;
			upgradeIntf[(int)UpgradeType.DPS] = upgrStatDPS;
			upgradeIntf[(int)UpgradeType.ClickDamage] = upgrStatDmg;
			upgradeIntf[(int)UpgradeType.DamageMultiplier_Fire] = upgrStatFire;
			upgradeIntf[(int)UpgradeType.DamageMultiplier_Water] = upgrStatWater;
			upgradeIntf[(int)UpgradeType.DamageMultiplier_Air] = upgrStatAir;
			upgradeIntf[(int)UpgradeType.DamageMultiplier_Earth] = upgrStatEarth;
			upgradeIntf[(int)UpgradeType.DamageMultiplier_Crit] = upgrStatCrit;
			upgradeIntf[(int)UpgradeType.BossLootDropPercentage] = upgrStatLoot;
			upgradeIntf[(int)UpgradeType.HitPoints] = upgrStatHP;

			playerData = new PlayerData();
			gameData = new GameData();
			gameData.Init();
			stats = new Stats();
			techTree = new TechTree();
			techTree.Init();
			tuningData = new TuningData();
			tuningData.Init();
			printPlayerData();
			printGameData();
			printTechTree();
			processTechTree(); // Copy some data from print over for use
		}

		private void critDamage(double value)
		{
			// It seems this is sent to the client to increment a global counter which is then
			// decremented every time the user clicks to do a critic using the calculated crit 
			// amount, until the counter reaches zero again.
			// Not useful here.
		}

		private void decodePlayerData(JSONNode json)
		{
			JSONNode hp = json["hp"];
			JSONNode currentLane = json["current_lane"];
			JSONNode target = json["target"];
			JSONNode gold = json["gold"];
			JSONNode activeAbilitiesBitfield = json["active_abilities_bitfield"];
			JSONNode critDamage = json["crit_damage"];
			JSONNode timeDied = json["time_died"];
			JSONNode activeAbilities = json["active_abilities"];
			JSONNode loot = json["loot"];

			if (hp != null) playerData.Hp = double.Parse(hp.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
			if (gold != null) playerData.Gold = double.Parse(gold.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
			if (currentLane != null) playerData.CurrentLane = Convert.ToInt32(currentLane.Value, CultureInfo.InvariantCulture);
			if (target != null) playerData.Target = Convert.ToInt32(target.Value, CultureInfo.InvariantCulture);
			if (activeAbilitiesBitfield != null) playerData.ActiveAbilitiesBitfield = Convert.ToUInt64(activeAbilitiesBitfield.Value, CultureInfo.InvariantCulture);
			if (timeDied != null) playerData.TimeDied = Convert.ToInt32(timeDied.Value, CultureInfo.InvariantCulture);

			if (loot != null)
			{
				refreshUpgrades = true;
			}

			if (critDamage != null)
			{
				double v = double.Parse(critDamage.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (v != 0)
				{
					this.critDamage(v);
				}
			}

			--waitForNewPlayerData;
			printPlayerData();
		}

		static string[] laneNumbers = new string[] {
			"❶ ② ③",
			"① ❷ ③",
			"① ② ❸",
		};

		static string[] targetNumbers = new string[] {
			"❶ ② ③ ④",
			"① ❷ ③ ④",
			"① ② ❸ ④",
			"① ② ③ ❹",
		};
		
		/// <summary>
		/// Display all player specific data to screen (stuff in playerData changed)
		/// </summary>
		private void printPlayerData()
		{
			hpLabel.Text = playerData.Hp.ToString();
			goldLabel.Text = playerData.Gold.ToString();
			currentLaneLabel.Text = laneNumbers[playerData.CurrentLane];//(playerData.CurrentLane + 1).ToString();
			targetLabel.Text = targetNumbers[playerData.Target];
			deadAliveText.Text = (playerData.TimeDied == 0) ? "Alive" : "Dead";
			printPlayerTech();
		}

		static string[] elementIcons = new string[] {
			"🔥", // Fire
			"🌊", // Water
			"💨", // Air
			"🌴", // Earth
		};
		private void printGameTree()
		{
			int bestElement = bestElementLevel();
			string res = "";
			for (int i = 0; i < gameData.Lanes.Length; ++i)
			{
				if (gameData.Lanes[i].Element >= UpgradeOption.ElementalFire && gameData.Lanes[i].Element <= UpgradeOption.ElementalEarth)
				{
					int laneEleLevel = techTree.Upgrades[(int)gameData.Lanes[i].Element].Level;
					if (laneEleLevel >= bestElement)
					{
						res += "【" + elementIcons[(int)gameData.Lanes[i].Element - (int)UpgradeOption.ElementalFire] + "】 ";
					}
					else
					{
						res += elementIcons[(int)gameData.Lanes[i].Element - (int)UpgradeOption.ElementalFire] + " ";
					}
				}
			}
			elementText.Text = res;
		}

		void decodeTechTree(JSONNode json)
		{
			JSONNode unlockedAbilitiesBitfield = json["unlocked_abilities_bitfield"];
			JSONNode abilityItems = json["ability_items"];
			JSONNode upgrades = json["upgrades"];
			JSONNode critPercentage = json["crit_percentage"];
			JSONNode badgePoints = json["badge_points"];

			if (critPercentage != null) this.techTree.CritPercentage = double.Parse(critPercentage.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
			if (badgePoints != null) this.techTree.BadgePoints = Convert.ToInt64(badgePoints.Value, CultureInfo.InvariantCulture);

			// "level": 20,
			// "cost_for_next_level": 9094947010
			ulong hasUpgrade = 0;
			if (upgrades != null)
			{
				foreach (JSONNode upgrade in upgrades.Childs)
				{
					JSONNode upgrade_ = upgrade["upgrade"];
					JSONNode level = upgrade["level"];
					JSONNode costForNextLevel = upgrade["cost_for_next_level"];
					if (upgrade_ != null && level != null && costForNextLevel != null)
					{
						int upgradeI = Convert.ToInt32(upgrade_.Value, CultureInfo.InvariantCulture);
						if (upgradeI < techTree.Upgrades.Length)
						{
							techTree.Upgrades[upgradeI].Level = Convert.ToInt32(level.Value, CultureInfo.InvariantCulture);
							techTree.Upgrades[upgradeI].CostForNextLevel = double.Parse(costForNextLevel.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
							hasUpgrade |= (1UL << upgradeI);
						}
					}
				}
			}
			for (int i = 0; i < techTree.Upgrades.Length; ++i)
			{
				if ((hasUpgrade & (1UL << i)) == 0)
				{
					techTree.Upgrades[i].Level = 0;
					techTree.Upgrades[i].CostForNextLevel = double.MaxValue;
				}
			}

			if (unlockedAbilitiesBitfield != null) techTree.UnlockedAbilitiesBitfield = Convert.ToUInt64(unlockedAbilitiesBitfield.Value, CultureInfo.InvariantCulture);
			ulong hasAbilityItem = 0;
			if (abilityItems != null)
			{
				foreach (JSONNode abilityItem in abilityItems.Childs)
				{
					JSONNode ability = abilityItem["ability"];
					JSONNode quantity = abilityItem["quantity"];
					if (ability != null && quantity != null)
					{
						int abilityI = Convert.ToInt32(ability.Value, CultureInfo.InvariantCulture);
						int quantityI = Convert.ToInt32(quantity.Value, CultureInfo.InvariantCulture);
						if (abilityI < techTree.AbilityItems.Length)
						{
							techTree.AbilityItems[abilityI] = quantityI;
							hasAbilityItem |= (1UL << abilityI);
						}
					}
				}
			}
			for (int i = 0; i < techTree.AbilityItems.Length; ++i)
			{
				if ((hasAbilityItem & (1UL << i)) == 0)
					techTree.AbilityItems[i] = 0;
			}

			printTechTree();
			processTechTree(); // Copy some data from print over for use
		}

		string[] printTechTreeBases = new string[(int)UpgradeType.Nb];
		void printTuningData()
		{
			printTechTreeBases[(int)UpgradeType.HitPoints] = (tuningData.Player.Hp / 1000.0).ToString() + "k";
			printTechTreeBases[(int)UpgradeType.DPS] = tuningData.Player.DamagePerClick.ToString();
			printTechTreeBases[(int)UpgradeType.ClickDamage] = tuningData.Player.DamagePerClick.ToString();
			printTechTreeBases[(int)UpgradeType.DamageMultiplier_Crit] = tuningData.Player.DamageMultiplierCrit.ToString() + "x";
			printTechTreeBases[(int)UpgradeType.BossLootDropPercentage] = tuningData.Player.LootChance.ToString() + "x";
		}

		double[] techTreeUpgradeMultipliers = new double[(int)UpgradeType.Nb];
		void processTechTree()
		{
			if (printTechTreeBases[(int)UpgradeType.ClickDamage] == null)
				return;

			// Just copy so they can be used in thread
			for (int i = 0; i < printTechTreeMultipliers.Length; ++i)
			{
				techTreeUpgradeMultipliers[i] = printTechTreeMultipliers[i];
			}

			waitForUpgradeData = false;
		}

		System.Windows.Forms.Label[] upgradeIntf = new System.Windows.Forms.Label[(int)UpgradeType.Nb];
		double[] printTechTreeMultipliers = new double[(int)UpgradeType.Nb];
		string[] printTechTreeLevels = new string[(int)UpgradeType.Nb];
		/// <summary>
		/// Display all tech tree data on screen (stuff in techTree changed) (must call processTechTree after)
		/// </summary>
		void printTechTree()
		{
			if (printTechTreeBases[(int)UpgradeType.ClickDamage] == null)
				return;

			for (int i = 0; i < (int)UpgradeType.Nb; ++i)
			{
				printTechTreeMultipliers[i] = 1.0;
				printTechTreeLevels[i] = "";
			}
			printTechTreeMultipliers[(int)UpgradeType.DPS] = 0.0;
			for (int i = 0; i < tuningData.Upgrades.Length; ++i) if (tuningData.Upgrades[i].Type < UpgradeType.Nb)
			{
				double totalMultiplier = (double)techTree.Upgrades[i].Level * tuningData.Upgrades[i].Multiplier;
				printTechTreeMultipliers[(int)tuningData.Upgrades[i].Type] += totalMultiplier;
				printTechTreeLevels[(int)tuningData.Upgrades[i].Type] += " " + techTree.Upgrades[i].Level.ToString();
			}
			for (int i = 0; i < upgradeIntf.Length; ++i) if (upgradeIntf[i] != null && !string.IsNullOrEmpty(printTechTreeLevels[i]))
			{
				upgradeIntf[i].Text = Math.Round(printTechTreeMultipliers[i], i == (int)UpgradeType.BossLootDropPercentage ? 2 : 1).ToString(CultureInfo.InvariantCulture) 
					+ "x" + printTechTreeBases[i] + " (" + printTechTreeLevels[i].Substring(1) + ")";
				bool makeBold = printUpgradingType[i];
				if (makeBold != upgradeIntf[i].Font.Bold)
					upgradeIntf[i].Font = new System.Drawing.Font(upgradeIntf[i].Font, makeBold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
			}
			double dmgBase = tuningData.Player.DamagePerClick;
			double mulBase = printTechTreeMultipliers[(int)UpgradeType.ClickDamage];
			double mulEle = Math.Max(printTechTreeMultipliers[(int)UpgradeType.DamageMultiplier_Air],
				Math.Max(printTechTreeMultipliers[(int)UpgradeType.DamageMultiplier_Earth],
				Math.Max(printTechTreeMultipliers[(int)UpgradeType.DamageMultiplier_Fire],
				printTechTreeMultipliers[(int)UpgradeType.DamageMultiplier_Water])));
			double mulCrit = printTechTreeMultipliers[(int)UpgradeType.DamageMultiplier_Crit];
			double dpc = dmgBase * mulBase;
			double dpcEle = dpc * mulEle;
			double dpcCrit = dpc * mulCrit;
			dpcDisp.Text = Math.Round(dpc).ToString();
			dpcEleDisp.Text = Math.Round(dpcEle).ToString();
			dpcCritDisp.Text = Math.Round(dpcCrit).ToString();

			printPlayerTech();
			printGameTree();
		}

		int itemCount(Abilities ability)
		{
			return techTree.AbilityItems[(int)ability];
		}

		bool hasPurchasedAbility(Abilities ability)
		{
			if (ability >= Abilities.StartItem)
			{
				return techTree.AbilityItems[(int)ability] > 0;
			}
			else
			{
				ulong abbit = 1UL << (int)ability;
				return (techTree.UnlockedAbilitiesBitfield & abbit) == abbit;
			}
		}

		bool isAbilityCoolingDown(Abilities ability)
		{
			ulong abbit = 1UL << (int)ability;
			return (playerData.ActiveAbilitiesBitfield & abbit) == abbit;
		}

		System.Windows.Forms.Label[] abilitiesIntfs;
		System.Windows.Forms.Label[] itemsIntfs;
		System.Windows.Forms.Label[] itemsCounts;
		void printPlayerTech()
		{
			// medicsText.Text = ((playerData.ActiveAbilitiesBitfield & AbilitiesBitfield.Medics) == AbilitiesBitfield.Medics) ? "Cooldown Active" : "Available";
			for (int i = 0; i < abilitiesIntfs.Length; ++i)
			{
				int ab = i + (int)Abilities.StartAbility;
				ulong abbit = (1UL << ab);
				abilitiesIntfs[i].Enabled = (playerData.ActiveAbilitiesBitfield & abbit) != abbit;
				abilitiesIntfs[i].Visible = (techTree.UnlockedAbilitiesBitfield & abbit) == abbit;
			}
			int xpos = 6;
			for (int i = 0; i < itemsIntfs.Length; ++i)
			{
				int ab = i + (int)Abilities.StartItem;
				ulong abbit = (1UL << ab);
				itemsIntfs[i].Enabled = (playerData.ActiveAbilitiesBitfield & abbit) != abbit;
				itemsIntfs[i].Visible = (techTree.AbilityItems[ab] > 0);
				itemsCounts[i].Text = techTree.AbilityItems[ab].ToString();
				itemsCounts[i].Enabled = (playerData.ActiveAbilitiesBitfield & abbit) != abbit;
				itemsCounts[i].Visible = (techTree.AbilityItems[ab] > 0);
				if (techTree.AbilityItems[ab] > 0)
				{
					itemsIntfs[i].Location = new System.Drawing.Point(xpos, itemsIntfs[i].Location.Y);
					itemsCounts[i].Location = new System.Drawing.Point(xpos, itemsCounts[i].Location.Y);
					xpos += 46;
				}
			}
		}

		private JsonCallback resultTokenDetailsDelegate;
		private void resultTokenDetails(JSONNode json)
		{
			JSONNode steamid = json["steamid"];
			JSONNode personaName = json["persona_name"];

			if (steamid != null) steamId = steamid.Value;
			if (personaName != null) this.personaName = personaName.Value;

			Console.WriteLine("steamid: " + steamId);
		}

		private JsonCallback resultTuningDataDelegate;
		private void resultTuningData(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode jsonNode = response["json"];
			if (jsonNode == null)
				return;
			// Console.WriteLine(jsonNode.Value);
			// System.IO.File.WriteAllText("TuningDataExample.txt", jsonNode.Value);
			JSONNode tuningData = JSON.Parse(jsonNode.Value);

			JSONNode player = tuningData["player"];
			if (player != null)
			{
				JSONNode hp = player["hp"].Value;
				JSONNode dps = player["dps"].Value;
				JSONNode damagePerClick = player["damage_per_click"].Value;
				JSONNode damageMultiplierFire = player["damage_multiplier_fire"].Value;
				JSONNode damageMultiplierCrit = player["damage_multiplier_crit"].Value;
				JSONNode critPercentage = player["crit_percentage"].Value;
				JSONNode lootChance = player["loot_chance"].Value;

				if (hp != null) this.tuningData.Player.Hp = double.Parse(hp.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (dps != null) this.tuningData.Player.Dps = double.Parse(dps.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (damagePerClick != null) this.tuningData.Player.DamagePerClick = double.Parse(damagePerClick.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (damageMultiplierFire != null) this.tuningData.Player.DamageMultiplierFire = double.Parse(damageMultiplierFire.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (damageMultiplierCrit != null) this.tuningData.Player.DamageMultiplierCrit = double.Parse(damageMultiplierCrit.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (critPercentage != null) this.tuningData.Player.CritPercentage = double.Parse(critPercentage.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				if (lootChance != null) this.tuningData.Player.LootChance = double.Parse(lootChance.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
			}

			JSONNode upgrades = tuningData["upgrades"];
			if (upgrades != null)
			{
				foreach (KeyValuePair<string, JSONNode> upgrade in (upgrades as JSONClass))
				{
					int id = Convert.ToInt32(upgrade.Key, CultureInfo.InvariantCulture);
					if (id < this.tuningData.Upgrades.Length)
					{
						JSONNode multiplier = upgrade.Value["multiplier"];
						JSONNode type = upgrade.Value["type"];
						JSONNode requiredUpgrade = upgrade.Value["required_upgrade"];
						JSONNode requiredUpgradeLevel = upgrade.Value["required_upgrade_level"];

						if (multiplier != null) this.tuningData.Upgrades[id].Multiplier = double.Parse(multiplier.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
						if (type != null) this.tuningData.Upgrades[id].Type = (UpgradeType)Convert.ToInt32(type.Value, CultureInfo.InvariantCulture);
						if (requiredUpgrade != null) this.tuningData.Upgrades[id].RequiredUpgrade = Convert.ToInt32(requiredUpgrade.Value, CultureInfo.InvariantCulture);
						if (requiredUpgradeLevel != null) this.tuningData.Upgrades[id].RequiredUpgradeLevel = Convert.ToInt32(requiredUpgradeLevel.Value, CultureInfo.InvariantCulture);
					}
				}
			}

			printTuningData();
			waitForTuningData = false;
		}

		private JsonCallback resultPlayerNamesDelegate;
		private void resultPlayerNames(JSONNode json)
		{
			// System.Windows.Forms.MessageBox.Show("hello");
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode names = response["names"];
			if (names == null)
				return;
			playerList.Items.Clear();
			foreach (JSONNode child in names.Childs)
			{
				JSONNode name = child["name"];
				if (name != null)
				{
					playerList.Items.Add(name.Value);
				}
			}
			playerGroup.Text = "Players (" + playerList.Items.Count + ")";
			this.Text = "MonsterGUI.exe" + (string.IsNullOrEmpty(personaName) ? "" : (" - " + personaName));
		}

		private JsonCallback resultGameDataDelegate;
		/// <summary>
		/// Parse game data
		/// </summary>
		/// <param name="json"></param>
		private void resultGameData(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode gameData = response["game_data"];
			if (gameData != null)
			{
				JSONNode level = gameData["level"];
				JSONNode timestamp = gameData["timestamp"];
				JSONNode timestampLevelStart = gameData["timestamp_level_start"];
				JSONNode lanes = gameData["lanes"];

				if (level != null) this.gameData.Level = Convert.ToInt64(level.Value, CultureInfo.InvariantCulture) + 1;
				if (timestamp != null) this.gameData.Timestamp = Convert.ToInt64(timestamp.Value, CultureInfo.InvariantCulture);
				if (timestampLevelStart != null) this.gameData.TimestampLevelStart = Convert.ToInt64(timestampLevelStart.Value, CultureInfo.InvariantCulture);
				
				if (lanes != null)
				{
					int i = 0;
					foreach (JSONNode lane in lanes.Childs)
					{
						JSONNode activePlayerAbilityGoldPerClick = lane["active_player_ability_gold_per_click"];
						JSONNode enemies = lane["enemies"];
						JSONNode element = lane["element"];

						if (activePlayerAbilityGoldPerClick != null) this.gameData.Lanes[i].ActivePlayerAbilityGoldPerClick = double.Parse(activePlayerAbilityGoldPerClick.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
						if (element != null) this.gameData.Lanes[i].Element = (UpgradeOption)(Convert.ToInt32(element.Value, CultureInfo.InvariantCulture) - 1 + (int)UpgradeOption.ElementalFire);

						if (enemies != null)
						{
							int j = 0;
							foreach (JSONNode enemy in enemies.Childs)
							{
								JSONNode hp = enemy["hp"];
								JSONNode maxHp = enemy["max_hp"];
								JSONNode type = enemy["type"];

								if (hp != null) this.gameData.Lanes[i].Enemies[j].Hp = double.Parse(hp.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
								if (maxHp != null) this.gameData.Lanes[i].Enemies[j].MaxHp = double.Parse(maxHp.Value.ToUpperInvariant(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
								if (type != null) this.gameData.Lanes[i].Enemies[j].Type = (EnemyType)Convert.ToInt32(type.Value, CultureInfo.InvariantCulture);

								++j;
								if (j > this.gameData.Lanes[i].Enemies.Length)
									break;
							}
							for (; j < this.gameData.Lanes[i].Enemies.Length; ++j)
								this.gameData.Lanes[i].Enemies[j].Type = EnemyType.None;
						}

						++i;
						if (i > this.gameData.Lanes.Length) 
							break;
					}
				}
			}
			JSONNode stats = response["stats"];
			if (stats != null)
			{
				JSONNode numActivePlayers = stats["num_active_players"];
				JSONNode numClicks = stats["num_clicks"];

				if (numActivePlayers != null) this.stats.NumActivePlayers = Convert.ToInt32(numActivePlayers.Value, CultureInfo.InvariantCulture);
				if (numClicks != null) this.stats.NumClicks = Convert.ToInt64(numClicks.Value, CultureInfo.InvariantCulture);
			}
			if (gameData != null || stats != null)
				printGameData();
		}

		long lastTimestamp;
		long lastNumClicks;
		/// <summary>
		/// Display all game data on screen (stuff in gameData and stats changed)
		/// </summary>
		private void printGameData()
		{
			levelText.Text = gameData.Level.ToString() + " (" + (gameData.Timestamp - gameData.TimestampLevelStart).ToString() + "s)";
			activePlayersText.Text = stats.NumActivePlayers.ToString();
			lane1Gold.Text = Math.Round(gameData.Lanes[0].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(0) ? "" : " (no enemies)");
			lane2Gold.Text = Math.Round(gameData.Lanes[1].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(1) ? "" : " (no enemies)");
			lane3Gold.Text = Math.Round(gameData.Lanes[2].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(2) ? "" : " (no enemies)");
			clicksNumText.Text = stats.NumClicks.ToString();
			long timestampDiff = gameData.Timestamp - lastTimestamp;
			if (timestampDiff > 0)
			{
				long clickDiff = stats.NumClicks - lastNumClicks;
				long cps = clickDiff / timestampDiff;
				cpsText.Text = cps.ToString();
				lastNumClicks = stats.NumClicks;
				lastTimestamp = gameData.Timestamp;
			}
			bool bossMonster = false;
			for (int i = 0; i < 3; ++i)
			{
				if (bossMonsterOnLane(i))
				{
					try { bossLaneText.Text = laneNumbers[i] + " (" + Math.Round(gameData.Lanes[i].Enemies[0].Hp * 100.0 / gameData.Lanes[i].Enemies[0].MaxHp) + " %)"; }
					catch { bossLaneText.Text = laneNumbers[i]; }
					bossMonster = true;
					break;
				}
			}
			if (!bossMonster) for (int i = 0; i < 3; ++i)
			{
				if (treasureMonsterOnLane(i))
				{
					bossLaneText.Text = laneNumbers[i] + " (treasure)";
					bossMonster = true;
					break;
				}
			}
			if (!bossMonster)
			{
				bossLaneText.Text = "";
			}
			printGameTree();
		}

		/// <summary>
		/// Thread which runs GetGameData every second
		/// </summary>
		private void getStateThread()
		{
			// Tech Tree: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetPlayerData/v0001/?gameid=XXXX&steamid=XXXXXXXXXXXXXXX&include_tech_tree=1
			// Player List: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetPlayerNames/v0001/?gameid=XXXX
			// Game Data: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetGameData/v0001/?gameid=37559&include_stats=1

			WebClient wc = new TimeoutWebClient();
			wc.Headers.Add("Accept-Charset", "utf-8");
			while (running)
			{
				int startTick = System.Environment.TickCount;
				try
				{
					if (getSteamId && !string.IsNullOrEmpty(accessToken))
					{
						StringBuilder url = new StringBuilder();
						url.Append("https://steamapi-a.akamaihd.net/ISteamUserOAuth/GetTokenDetails/v1/?access_token=");
						url.Append(accessToken);
						Invoke(enableDelegate, getStateStatus, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultTokenDetailsDelegate, json);
						getSteamId = false;
					}
					else if (getTuningData && !string.IsNullOrEmpty(accessToken))
					{
						// https://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetTuningData/v0001/?game_type=1&gameid=41671&access_token=***
						StringBuilder url = new StringBuilder();
						url.Append("https://");
						url.Append(host);
						url.Append("GetTuningData/v0001/?game_type=1&gameid=");
						url.Append(room);
						url.Append("&access_token=");
						url.Append(accessToken);
						Invoke(enableDelegate, getStateStatus, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultTuningDataDelegate, json);
						getTuningData = false;
					}
					else if (getPlayerNames)
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetPlayerNames/v0001/?gameid=");
						url.Append(room);
						Invoke(enableDelegate, getStateStatus, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPlayerNamesDelegate, json);
						getPlayerNames = false;
					}
					else
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetGameData/v0001/?gameid=");
						url.Append(room);
						url.Append("&include_stats=1");
						Invoke(enableDelegate, getStateStatus, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultGameDataDelegate, json);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				if (!exiting) Invoke(enableDelegate, getStateStatus, false);
				int endTick = System.Environment.TickCount;
				int toSleep = 1000 - (endTick - startTick);
				if (toSleep > 0) System.Threading.Thread.Sleep(toSleep);
			}
			wc.Dispose();
			Invoke(endedThreadDelegate);
		}
	}
}
