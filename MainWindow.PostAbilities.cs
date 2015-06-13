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
		volatile bool autoClickerOn = false;
		volatile bool laneSwitcherOn = true;
		volatile bool goldLaneSwitcherOn = true;
		volatile bool respawnerOn = true;
		volatile bool healerOn = false;

		long clickCount = 0;
		volatile int addClicks = 0;
		int minClicks = 12;
		int maxClicks = 24;

		int laneSwitcherTime = 1;
		int laneSwitcherTimeCounter = 0;

		int laneRequested = 0;

		private void postAbilitiesInit()
		{
			resultPostAbilitiesDelegate = new JsonCallback(resultPostAbilities);
			autoClickerCheck.Checked = autoClickerOn;
			laneSwitcherCheck.Checked = laneSwitcherOn;
			goldLaneCheck.Checked = goldLaneSwitcherOn;
			respawnerCheck.Checked = respawnerOn;
			healerCheck.Checked = healerOn;
		}

		private void postAbilitiesGo()
		{
			clickCount = 0;
			clicksText.Text = "0";
		}

		private JsonCallback resultPostAbilitiesDelegate;
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

		private bool enemiesAliveInLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private void postAbilitiesThread()
		{
			// ChooseUpgrade(s?): {"gameid":"6059","upgrades":[4]} (NOTE: Double check for correct URL!!!)
			// UseAbilities: {"requested_abilities":[{"ability":2,"new_lane":0},{"ability":4,"new_target":0},{"ability":1,"num_clicks":1}],"gameid":"6059"}
			// 1: Click [num_clicks]
			// 2: Switch Lane [new_lane]
			// 3: Respawn
			// 4: Switch Target [new_target] (NOTE: Server automatically switches targets as well)

			Random random = new Random();
			WebClient wc = new WebClient();
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

					bool upgrades = false;
					string upgrades_json = "{\"gameid\":\"" + room + "\",\"upgrades\":[";

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
							++laneRequested;
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

					bool goldLane = false;
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
							goldLane = true;
						}
					}

					if (laneSwitcherOn || goldLane) // If any lane switching algorithm is enabled
					{
						if (laneRequested != playerData.CurrentLane)
						{
							// Switch lane if requested
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":2,\"new_lane\":" + laneRequested + "}";
							abilities = true;
						}
					}

					// After lane switched

					// TODO: Targeting (only useful for boss gold rain, otherwise not enough time in level)

					// After target switched

					if (healerOn)
					{
						if ((playerData.ActiveAbilitiesBitfield & AbilitiesBitfield.Medics) != AbilitiesBitfield.Medics)
						{
							// Medics
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":7}";
							abilities = true;
						}
					}

					if (autoClickerOn && maxClicks >= minClicks)
					{
						int nb = minClicks + random.Next(maxClicks - minClicks);
						addClicks = nb;
						if (abilities) abilties_json += ",";
						abilties_json += "{\"ability\":1,\"num_clicks\":" + nb + "}";
						abilities = true;
					}
					else
					{
						addClicks = 0;
					}

					if (abilities || !upgrades) // blank abilities to refresh player state in case no other post sent
					{
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

					if (upgrades)
					{
						// TODO
					}
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
			if (!exiting) Invoke(endedThreadDelegate);
		}
	}
}
