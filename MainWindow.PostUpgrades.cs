using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SimpleJSON;

// This will be the home of automatic upgrades

namespace MonsterGUI
{
	public partial class MainWindow
	{
		volatile bool refreshUpgrades = false;

		private void postUpgradesInit()
		{
			resultPostUpgradesDelegate = new JsonCallback(resultPostUpgrades);
		}

		/// <summary>
		/// When user pushes GO
		/// </summary>
		private void postUpgradesGo()
		{
			
		}

		private JsonCallback resultPostUpgradesDelegate;
		private void resultPostUpgrades(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode techTree = response["tech_tree"];
			if (techTree == null)
				return;
			decodeTechTree(techTree);
		}

		/// <summary>
		/// Thread calling POST ChooseUpgrade
		/// </summary>
		private void postUpgradesThread()
		{
			// ChooseUpgrade: {"gameid":"6059","upgrades":[4,4,5,6]}

			WebClient wc = new TimeoutWebClient();
			const int notSentCountLimit = 60; // Send a blank upgrade request every 60 ticks to force update the player state
			int notSentCount = notSentCountLimit;
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

					bool upgrades = false;
					string upgrades_json = "{\"gameid\":\"" + room + "\",\"upgrades\":[";

					// Upgrade prices can be found under techTree
					// Example how to add an upgrade
					/*
						if (upgrades) upgrades_json += ",";
						upgrades_json += "4";
						upgrades = true;
					*/

					// Send the upgrade packet
					if (upgrades)
					{
						upgrades_json += "]}";
						StringBuilder url = new StringBuilder();
						url.Append("https://");
						url.Append(host);
						url.Append("ChooseUpgrade/v0001/");
						StringBuilder query = new StringBuilder();
						query.Append("input_json=");
						query.Append(WebUtilities.UrlEncode(upgrades_json));
						query.Append("&access_token=");
						query.Append(accessToken);
						query.Append("&format=json");
						wc.Headers[HttpRequestHeader.AcceptCharset] = "utf-8";
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						if (!exiting) Invoke(enableDelegate, postUpgradesState, true);
						Console.WriteLine(upgrades_json);
						string res = wc.UploadString(url.ToString(), query.ToString());
						Console.WriteLine(res);
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostUpgradesDelegate, json);
						notSentCount = 0;
						refreshUpgrades = false;
					}
					else if ((notSentCount >= notSentCountLimit || refreshUpgrades) && !string.IsNullOrEmpty(steamId))
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetPlayerData/v0001/?gameid=");
						url.Append(room);
						url.Append("&steamid=");
						url.Append(steamId);
						url.Append("&include_tech_tree=1&format=json");
						if (!exiting) Invoke(enableDelegate, postUpgradesState, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostUpgradesDelegate, json);
						notSentCount = 0;
						refreshUpgrades = false;
					}
					else
					{
						++notSentCount;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				if (!exiting) Invoke(enableDelegate, postUpgradesState, false);
				int endTick = System.Environment.TickCount;
				int toSleep = 1000 - (endTick - startTick);
				if (toSleep > 0) System.Threading.Thread.Sleep(toSleep);
			}
			wc.Dispose();
			Invoke(endedThreadDelegate);
		}
	}
}
