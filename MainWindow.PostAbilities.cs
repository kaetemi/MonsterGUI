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
		volatile bool goldLaneSwitcherOn = true;
		volatile bool respawnerOn = true;
		volatile bool healerOn = false;

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

		/// <summary>
		/// App init
		/// </summary>
		private void postAbilitiesInit()
		{
			resultPostAbilitiesDelegate = new JsonCallback(resultPostAbilities);
			autoClickerCheck.Checked = autoClickerOn;
			laneSwitcherCheck.Checked = laneSwitcherOn;
			goldLaneCheck.Checked = goldLaneSwitcherOn;
			respawnerCheck.Checked = respawnerOn;
			healerCheck.Checked = healerOn;
		}

		/// <summary>
		/// User GO
		/// </summary>
		private void postAbilitiesGo()
		{
			clickCount = 0;
			clicksText.Text = "0";
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
					// NOTE: Target switching is already done by the server so not entirely useful since the monsters die too fast

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
