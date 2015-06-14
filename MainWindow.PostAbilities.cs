using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SimpleJSON;

namespace MonsterGUI
{
	public partial class MainWindow
	{
		// Switched by the callbacks from the checkboxes in the GUI
		volatile bool autoClickerOn = false;
		volatile bool laneSwitcherOn = true;
		volatile bool targetSpawnersOn = true;
		volatile bool goldLaneSwitcherOn = true;
		volatile bool bossLaneOn = true;
		volatile bool respawnerOn = true;
		volatile bool supportAbilitiesOn = false;
		volatile bool offensiveAbilitiesOn = false;
		volatile bool itemAbilitiesOn = false;

		// Auto clicker runtime info
		long clickCount = 0;
		long addClicks = 0;
		int minClicks = 16;
		int maxClicks = 20;
		int clickBoost = 1;

		// Timed Lane Switcher runtime info
		int laneSwitcherTime = 1;
		int laneSwitcherTimeCounter = 0;

		// Lane Switcher runtime info
		int laneRequested = 0;

		// Abilities info
		int lastGoldRainLevel = 0;

		/// <summary>
		/// App init
		/// </summary>
		private void postAbilitiesInit()
		{
			resultPostAbilitiesDelegate = new JsonCallback(resultPostAbilities);
			autoClickerCheck.Checked = autoClickerOn;
			laneSwitcherCheck.Checked = laneSwitcherOn;
			targetSpawnerCheck.Checked = targetSpawnersOn;
			goldLaneCheck.Checked = goldLaneSwitcherOn;
			bossLaneCheck.Checked = bossLaneOn;
			respawnerCheck.Checked = respawnerOn;
			supportAbilitiesCheck.Checked = supportAbilitiesOn;
			ovenzifCheck.Checked = offensiveAbilitiesOn;
			itemsCheck.Checked = itemAbilitiesOn;
		}

		/// <summary>
		/// User GO
		/// </summary>
		private void postAbilitiesGo()
		{
			clickCount = 0;
			clicksText.Text = "0";
			lastGoldRainLevel = 0;
		}

		private JsonCallback resultPostAbilitiesDelegate;
		/// <summary>
		/// Success response from server
		/// </summary>
		/// <param name="json"></param>
		private void resultPostAbilities(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode playerData = response["player_data"];
			if (playerData == null)
				return;
			clickCount += (long)addClicks;
			decodePlayerData(playerData);
			clicksText.Text = clickCount.ToString();
		}

		/// <summary>
		/// Check if there are any enemies alive in the specified lane (lane index 0, 1, 2)
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		private bool enemiesAliveInLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private decimal highestMonsterOnLane(int i)
		{
			decimal highest = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp > highest)
					highest = gameData.Lanes[i].Enemies[j].Hp;
			}
			return highest;
		}

		private int numberMonstersAliveOnLane(int i)
		{
			int count = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					++count;
			}
			return count;
		}

		private bool bossMonsterOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Boss && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private bool treasureMonsterOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Treasure && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private int findSpawnerOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Spawner && gameData.Lanes[i].Enemies[j].Hp != 0)
					return j;
			}
			return -1;
		}

		private int findTreasureOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Treasure && gameData.Lanes[i].Enemies[j].Hp != 0)
					return j;
			}
			return -1;
		}

		private decimal highestHpFactorOnLane(int i)
		{
			decimal highest = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].MaxHp != 0 && (gameData.Lanes[i].Enemies[j].Hp / gameData.Lanes[i].Enemies[j].MaxHp) > highest)
					return gameData.Lanes[i].Enemies[j].Hp / gameData.Lanes[i].Enemies[j].MaxHp;
			}
			return highest;
		}

		private bool farmingGoldOnLane(int i)
		{
			return ((gameData.Level < 1000 || ((gameData.Level % 200) == 0)) && bossMonsterOnLane(i));
		}

		private bool nukeOnLane(int i)
		{
			if (gameData.Level > 1000)
			{
				return ((gameData.Level % 200) != 0) && bossMonsterOnLane(laneRequested);
			}
			else
			{
				return findSpawnerOnLane(i) >= 0;
			}
		}

		/// <summary>
		/// Thread which posts abilities
		/// </summary>
		private void postAbilitiesThread()
		{
			// UseAbilities: {"requested_abilities":[{"ability":2,"new_lane":0},{"ability":4,"new_target":0},{"ability":1,"num_clicks":1}],"gameid":"6059"}
			// 1: Click [num_clicks]
			// 2: Switch Lane [new_lane]
			// 3: Respawn
			// 4: Switch Target [new_target] (NOTE: Server automatically switches targets as well)

			Random random = new Random();
			WebClient wc = new TimeoutWebClient();
			while (running)
			{
				int startTick = System.Environment.TickCount;
				try
				{
					if (string.IsNullOrEmpty(accessToken))
					{
						Console.WriteLine("Access token not set");
						break;
					}

					bool abilities = false;
					string abilties_json = "{\"gameid\":\"" + room + "\",\"requested_abilities\":[";

					if (respawnerOn)
					{
						if (playerData.TimeDied != 0)
						{
							// Respawn
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":3}";
							abilities = true;
						}
					}

					// Before lane switched

					if (laneSwitcherOn) // Timed lane switcher
					{
						laneSwitcherTimeCounter++;
						laneSwitcherTimeCounter %= laneSwitcherTime;

						if (laneSwitcherTimeCounter == 0)
						{
							// ++laneRequested;
							laneRequested += random.Next(1, 3); // incl min, excl max
							laneRequested %= 3;
						}

						for (int i = 0; i < 3; ++i)
						{
							if (!enemiesAliveInLane(laneRequested))
							{
								++laneRequested;
								laneRequested %= 3;
							}
						}
					}

					bool smartLane = false;
					if (goldLaneSwitcherOn)
					{
						int bestLane = -1;
						decimal bestGold = 0.0m;
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (gameData.Lanes[i].ActivePlayerAbilityGoldPerClick > bestGold
								&& enemiesAliveInLane(i))
							{
								bestLane = i;
								bestGold = gameData.Lanes[i].ActivePlayerAbilityGoldPerClick;
							}
						}
						if (bestLane >= 0)
						{
							laneRequested = bestLane;
							smartLane = true;
						}
					}

					if (bossLaneOn)
					{
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (treasureMonsterOnLane(i))
							{
								laneRequested = i;
								smartLane = true;
								break;
							}
						}
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (bossMonsterOnLane(i))
							{
								laneRequested = i;
								smartLane = true;
								break;
							}
						}
					}

					if (laneSwitcherOn || smartLane) // If any lane switching algorithm is enabled
					{
						if (laneRequested != playerData.CurrentLane)
						{
							// Switch lane if requested
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":2,\"new_lane\":" + laneRequested + "}";
							abilities = true;
						}
					}
					else
					{
						laneRequested = playerData.CurrentLane;
					}

					// After lane switched
					// NOTE: Target switching is already done by the server so not entirely useful since the monsters die too fast

					if (targetSpawnersOn) // Useful for nuking spawners at early levels and treasure
					{
						int preferTarget = findTreasureOnLane(laneRequested);
						if (preferTarget < 0)
							preferTarget = findSpawnerOnLane(laneRequested);
						if (preferTarget >= 0)
						{
							if (preferTarget != playerData.Target)
							{
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":2,\"new_target\":" + preferTarget + "}"; // Only using this for spawners
								abilities = true;
							}
						}
					}

					bool requestTreeRefresh = false;

					if (enemiesAliveInLane(laneRequested) && enemiesAliveInLane(playerData.CurrentLane))
					{
						if (supportAbilitiesOn)
						{
							if (hasPurchasedAbility(Abilities.Medics) && !isAbilityCoolingDown(Abilities.Medics))
							{
								// Medics, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.Medics + "}";
								abilities = true;
							}
							if (laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								if (!farmingGoldOnLane(laneRequested)) // Don't do extra damage when farming gold
								{
									if (hasPurchasedAbility(Abilities.MoraleBooster) && !isAbilityCoolingDown(Abilities.MoraleBooster))
									{
										if (highestMonsterOnLane(laneRequested) > 100000000 && numberMonstersAliveOnLane(laneRequested) >= 2)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.MoraleBooster + "}"; // More Damage
											abilities = true;
										}
									}
									if (hasPurchasedAbility(Abilities.GoodLuckCharm) && !isAbilityCoolingDown(Abilities.GoodLuckCharm))
									{
										if (abilities) abilties_json += ",";
										abilties_json += "{\"ability\":" + (int)Abilities.GoodLuckCharm + "}"; // IncreaseCritPercentage
										abilities = true;
									}
								}
								else // We're farming gold
								{
									if (hasPurchasedAbility(Abilities.MetalDetector) && !isAbilityCoolingDown(Abilities.MetalDetector))
									{
										if (highestHpFactorOnLane(laneRequested) > 0.8m)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.MetalDetector + "}"; // Increase Gold Drops
											abilities = true;
										}
									}
								}
							}
							// Cooldown: DecreaseCooldowns .. ?
						}

						if (offensiveAbilitiesOn)
						{
							if (laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								if (hasPurchasedAbility(Abilities.Nuke) && !isAbilityCoolingDown(Abilities.Nuke))
								{
									if (nukeOnLane(laneRequested))
									{
										if (highestHpFactorOnLane(laneRequested) > 0.65m)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.Nuke + "}"; // Nuke
											abilities = true;
										}
									}
								}
							}
						}

						if (itemAbilitiesOn)
						{
							if (hasPurchasedAbility(Abilities.IncreaseCritPercentagePermanently) && !isAbilityCoolingDown(Abilities.IncreaseCritPercentagePermanently))
							{
								// Permanent upgrades, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.IncreaseCritPercentagePermanently + "}";
								abilities = true;
								requestTreeRefresh = true;
							}
							if (hasPurchasedAbility(Abilities.IncreaseHPPermanently) && !isAbilityCoolingDown(Abilities.IncreaseHPPermanently))
							{
								// Permanent upgrades, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.IncreaseHPPermanently + "}";
								abilities = true;
								requestTreeRefresh = true;
							}
							if (laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								if (lastGoldRainLevel != gameData.Level && hasPurchasedAbility(Abilities.GoldRain) && !isAbilityCoolingDown(Abilities.GoldRain))
								{
									// Gold rain
									if (farmingGoldOnLane(laneRequested))
									{
										if (highestHpFactorOnLane(laneRequested) > 0.75m)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.GoldRain + "}";
											abilities = true;
											lastGoldRainLevel = gameData.Level;
											requestTreeRefresh = true;
										}
									}
								}
							}
						}
					}

					long ac = 0;
					if (autoClickerOn && maxClicks >= minClicks)
					{
						for (int i = 0; i < clickBoost; ++i) // Send clicks ability multiple times
						{
							int nb = minClicks + random.Next(maxClicks - minClicks); // Random clicks number
							ac += (long)nb;
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":1,\"num_clicks\":" + nb + "}";
							abilities = true;
						}
					}
					addClicks = ac;

					if (abilities)
					{
						// Send the UseAbilities POST request
						abilties_json += "]}";
						StringBuilder url = new StringBuilder();
						url.Append("https://");
						url.Append(host);
						url.Append("UseAbilities/v0001/");
						StringBuilder query = new StringBuilder();
						query.Append("input_json=");
						query.Append(WebUtilities.UrlEncode(abilties_json));
						query.Append("&access_token=");
						query.Append(accessToken);
						query.Append("&format=json");
						wc.Headers[HttpRequestHeader.AcceptCharset] = "utf-8";
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						if (!exiting) Invoke(enableDelegate, postAbilitiesState, true);
						Console.WriteLine(abilties_json);
						string res = wc.UploadString(url.ToString(), query.ToString());
						Console.WriteLine(res);
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostAbilitiesDelegate, json);
					}
					else if (!string.IsNullOrEmpty(steamId))
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetPlayerData/v0001/?gameid=");
						url.Append(room);
						url.Append("&steamid=");
						url.Append(steamId);
						url.Append("&include_tech_tree=0&format=json");
						if (!exiting) Invoke(enableDelegate, postAbilitiesState, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostAbilitiesDelegate, json);
					}

					if (requestTreeRefresh)
						refreshUpgrades = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				if (!exiting) Invoke(enableDelegate, postAbilitiesState, false);
				int endTick = System.Environment.TickCount;
				int toSleep = 1000 - (endTick - startTick);
				if (toSleep > 0) System.Threading.Thread.Sleep(toSleep);
			}
			wc.Dispose();
			Invoke(endedThreadDelegate);
		}
	}
}
