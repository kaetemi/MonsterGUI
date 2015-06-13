using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using SimpleJSON;

namespace MonsterGUI
{
	enum AbilitiesBitfield
	{
		Medics = 0x80,
		MetalDetector = 0x100,
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
	}

	struct GameData
	{
		public int Level;

		// public Lane[] Lanes = new Lane[3];

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
		}

		private void getStateGo()
		{
			getPlayerNames = true;

			playerData = new PlayerData();
			gameData = new GameData();
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
			currentLaneLabel.Text = playerData.CurrentLane.ToString();
			targetLabel.Text = playerData.Target.ToString();
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

				if (level != null) this.gameData.Level = Convert.ToInt32(level.Value, CultureInfo.InvariantCulture) + 1;
				if (timestamp != null) this.gameData.Timestamp = Convert.ToInt64(timestamp.Value, CultureInfo.InvariantCulture);
				if (timestampLevelStart != null) this.gameData.TimestampLevelStart = Convert.ToInt64(timestampLevelStart.Value, CultureInfo.InvariantCulture);
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
