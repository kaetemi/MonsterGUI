﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using SimpleJSON;

namespace MonsterGUI
{
	enum Abilities
	{
		Click = 1,
		SwitchLane = 2,
		Respawn = 3,
		SwitchTarget = 4,
		MoraleBooster = 5,
		GoodLuck = 6,
		Medics = 7,
		MetalDetector = 8,
		Cooldown = 9, 
		Nuke = 10,
		ClusterBomb = 11,
		Napalm = 12,
		Revive = 13,
		CrippleSpawner = 14,
 		CrippleMonster = 15,
		MaximizeElement = 16,
		GoldRain = 17,
		Crit = 18,
		//
		//
		GodMode = 21,
		//
		//
		ReflectDamage = 24
	}

	enum AbilitiesBitfield
	{
		Medics = 1 << Abilities.Medics,
		MetalDetector = 1 << Abilities.MetalDetector,
		GoldRain = 1 << Abilities.GoldRain
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

	struct PlayerData
	{
		public decimal Hp;
		public decimal Gold;

		public int CurrentLane;
		public int Target;

		public int TimeDied;

		public AbilitiesBitfield ActiveAbilitiesBitfield;
	}

	struct Enemy
	{
		public decimal Hp;
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
			Enemies = new Enemy[4]; 
		}

		public Enemy[] Enemies;
		public decimal ActivePlayerAbilityGoldPerClick;
	}

	struct GameData
	{
		public void Init()
		{
			Lanes = new Lane[3];
			for (int i = 0; i < Lanes.Length; ++i)
				Lanes[i].Init();
		}

		public int Level;

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

	public partial class MainWindow
	{
		PlayerData playerData = new PlayerData();
		GameData gameData = new GameData();
		Stats stats = new Stats();

		volatile bool getPlayerNames = false;

		private void getStateInit()
		{
			resultPlayerNamesDelegate = new JsonCallback(resultPlayerNames);
			resultGameDataDelegate = new JsonCallback(resultGameData);
			gameData.Init();
		}

		private void getStateGo()
		{
			getPlayerNames = true;

			playerData = new PlayerData();
			gameData = new GameData();
			gameData.Init();
			stats = new Stats();
			printPlayerData();
			printGameData();
		}

		private void critDamage(long value)
		{
			// ... Do something?
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

			if (hp != null) playerData.Hp = Convert.ToDecimal(hp.Value, CultureInfo.InvariantCulture);
			if (gold != null) playerData.Gold = Convert.ToDecimal(gold.Value, CultureInfo.InvariantCulture);
			if (currentLane != null) playerData.CurrentLane = Convert.ToInt32(currentLane.Value, CultureInfo.InvariantCulture);
			if (target != null) playerData.Target = Convert.ToInt32(target.Value, CultureInfo.InvariantCulture);
			if (activeAbilitiesBitfield != null) playerData.ActiveAbilitiesBitfield = (AbilitiesBitfield)Convert.ToInt32(activeAbilitiesBitfield.Value, CultureInfo.InvariantCulture);
			if (timeDied != null) playerData.TimeDied = Convert.ToInt32(timeDied.Value, CultureInfo.InvariantCulture);

			if (critDamage != null)
			{
				long v = Convert.ToInt64(critDamage.Value, CultureInfo.InvariantCulture);
				if (v != 0)
				{
					this.critDamage(v);
				}
			}

			printPlayerData();
		}

		private void printPlayerData()
		{
			hpLabel.Text = playerData.Hp.ToString();
			goldLabel.Text = playerData.Gold.ToString();
			currentLaneLabel.Text = (playerData.CurrentLane + 1).ToString();
			targetLabel.Text = (playerData.Target + 1).ToString();
			deadAliveText.Text = (playerData.TimeDied == 0) ? "Alive" : "Dead";
			medicsText.Text = ((playerData.ActiveAbilitiesBitfield & AbilitiesBitfield.Medics) == AbilitiesBitfield.Medics) ? "Cooldown Active" : "Available";
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
		}

		private JsonCallback resultGameDataDelegate;
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

				if (level != null) this.gameData.Level = Convert.ToInt32(level.Value, CultureInfo.InvariantCulture) + 1;
				if (timestamp != null) this.gameData.Timestamp = Convert.ToInt64(timestamp.Value, CultureInfo.InvariantCulture);
				if (timestampLevelStart != null) this.gameData.TimestampLevelStart = Convert.ToInt64(timestampLevelStart.Value, CultureInfo.InvariantCulture);
				
				if (lanes != null)
				{
					int i = 0;
					foreach (JSONNode lane in lanes.Childs)
					{
						JSONNode activePlayerAbilityGoldPerClick = lane["active_player_ability_gold_per_click"];
						JSONNode enemies = lane["enemies"];

						if (activePlayerAbilityGoldPerClick != null) this.gameData.Lanes[i].ActivePlayerAbilityGoldPerClick = Convert.ToDecimal(activePlayerAbilityGoldPerClick.Value, CultureInfo.InvariantCulture);

						if (enemies != null)
						{
							int j = 0;
							foreach (JSONNode enemy in enemies.Childs)
							{
								JSONNode hp = enemy["hp"];
								JSONNode type = enemy["type"];

								if (hp != null) this.gameData.Lanes[i].Enemies[j].Hp = Convert.ToDecimal(hp.Value, CultureInfo.InvariantCulture);
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

				if (numActivePlayers != null) this.stats.NumActivePlayers = Convert.ToInt32(numActivePlayers.Value, CultureInfo.InvariantCulture);
			}
			if (gameData != null || stats != null)
				printGameData();
		}

		private void printGameData()
		{
			levelText.Text = gameData.Level.ToString() + " (" + (gameData.Timestamp - gameData.TimestampLevelStart).ToString() + "s)";
			activePlayersText.Text = stats.NumActivePlayers.ToString();
			lane1Gold.Text = decimal.Round(gameData.Lanes[0].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(0) ? "" : " (no enemies)");
			lane2Gold.Text = decimal.Round(gameData.Lanes[1].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(1) ? "" : " (no enemies)");
			lane3Gold.Text = decimal.Round(gameData.Lanes[2].ActivePlayerAbilityGoldPerClick, 4).ToString() + (enemiesAliveInLane(2) ? "" : " (no enemies)");
		}

		private void getStateThread()
		{
			// Tech Tree: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetPlayerData/v0001/?gameid=XXXX&steamid=XXXXXXXXXXXXXXX&include_tech_tree=1
			// Player List: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetPlayerNames/v0001/?gameid=XXXX
			// Game Data: http://steamapi-a.akamaihd.net/ITowerAttackMiniGameService/GetGameData/v0001/?gameid=37559&include_stats=1

			WebClient wc = new WebClient();
			wc.Headers.Add("Accept-Charset", "utf-8");
			while (running)
			{
				int startTick = System.Environment.TickCount;
				try
				{
					if (getPlayerNames)
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
			if (!exiting) Invoke(endedThreadDelegate);
		}
	}
}
