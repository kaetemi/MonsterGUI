﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Drawing.Text;
using System.IO;
using SimpleJSON;

namespace MonsterGUI
{
	public delegate void EmptyCallback();
	public delegate void JsonCallback(JSONNode /* press X to */ json);
	public delegate void EnableCallback(ToolStripItem control, bool enabled);

	public partial class MainWindow : Form
	{
		string host = "steamapi-a.akamaihd.net/ITowerAttackMiniGameService/";

		Thread getState;
		Thread postAbilities;
		Thread postUpgrades;
		volatile int runCount = 0;
		volatile bool running = false;
		volatile bool exiting = false;

		string accessToken;
		int room;

		bool multiThreadWarp = false;

		PrivateFontCollection pfc;
		System.Net.WebClient pfcWc;
		string pfcFile;
		public MainWindow()
		{
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

			endedThreadDelegate = new EmptyCallback(endedThread);
			enableDelegate = new EnableCallback(enable);
			getStateInit();
			postAbilitiesInit();
			postUpgradesInit();
			// splitContainer1.Enabled = false;
			loadFontDelegate = new EmptyCallback(loadFont);

			multiThreadWarp = File.Exists("MonsterGUI.exe.config");
			w9check.Enabled = multiThreadWarp;
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			new Thread(new ThreadStart(downloadFontAsync)).Start();
		}

		void downloadFontAsync()
		{
			try
			{
				string source = "https://github.com/MorbZ/OpenSansEmoji/raw/master/OpenSansEmoji.ttf";
				pfcFile = Path.GetFullPath(Path.GetTempPath() + "/OpenSansEmoji.ttf");
				if (!File.Exists(pfcFile))
				{
					pfcWc = new System.Net.WebClient();
					Console.WriteLine("Downloading {0} to {1}", source, pfcFile);
					pfcWc.DownloadFileCompleted += wc_DownloadFileCompleted;
					pfcWc.DownloadFileAsync(new Uri(source), pfcFile);
				}
				else
				{
					Invoke(loadFontDelegate);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Invoke(loadFontDelegate);
			try
			{
				pfcWc.Dispose();
				pfcWc = null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		EmptyCallback loadFontDelegate;
		void loadFont()
		{
			try
			{
				Console.WriteLine("Using font {0}", pfcFile);
				pfc = new PrivateFontCollection();
				pfc.AddFontFile(pfcFile);
				if (pfc.Families.Length > 0)
					elementText.Font = new Font(pfc.Families[0], elementText.Font.Size);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		EnableCallback enableDelegate;
		private void enable(ToolStripItem control, bool enabled)
		{
			control.Enabled = enabled;
		}

		private void go_Click(object sender, EventArgs e)
		{
			if (running)
			{
				go.Enabled = false;
				running = false;
				go.Text = "...";
				// splitContainer1.Enabled = false;
			}
			else
			{
				runCount = 3;
				running = true;

				accessToken = accessTokenText.Text;
				room = (int)roomText.Value; // Convert.ToInt32(roomText.Text, CultureInfo.InvariantCulture);
				accessTokenText.Enabled = false;
				roomText.Enabled = false;

				getStateGo();
				postAbilitiesGo();
				postUpgradesGo();

				getState = new Thread(new ThreadStart(getStateThread));
				postAbilities = new Thread(new ThreadStart(postAbilitiesThread));
				postUpgrades = new Thread(new ThreadStart(postUpgradesThread));
				getState.Start();
				postAbilities.Start();
				postUpgrades.Start();
				go.Text = "Stop";
				// splitContainer1.Enabled = true;
			}
		}

		EmptyCallback endedThreadDelegate;
		private void endedThread()
		{
			--runCount;
			if (runCount == 0)
			{
				go.Enabled = true;
				go.Text = "Go";
				accessTokenText.Enabled = true;
				roomText.Enabled = true;
				/*if (exiting)
				{
					exiting = false;
					Close();
				}*/
			}
		}

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (running || go.Enabled == false)
			{
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}

			/*if (exiting)
			{
				e.Cancel = true;
			}
			else
			{
				exiting = true;
				if (running || go.Enabled == false)
				{
					running = false;
					this.Enabled = false;
					e.Cancel = true;
				}
			}*/
		}

		private void postAbilitiesRunCheck_CheckedChanged(object sender, EventArgs e)
		{
			autoClickerOn = autoClickerCheck.Checked;
		}

		private void laneSwitcherCheck_CheckedChanged(object sender, EventArgs e)
		{
			laneSwitcherOn = laneSwitcherCheck.Checked;
		}

		private void minText_ValueChanged(object sender, EventArgs e)
		{
			minClicks = (int)minText.Value;
		}

		private void maxText_ValueChanged(object sender, EventArgs e)
		{
			maxClicks = (int)maxText.Value;
		}

		private void laneSwitcherTimer_ValueChanged(object sender, EventArgs e)
		{
			laneSwitcherTime = (int)laneSwitcherTimer.Value;
		}

		private void playerListRefresh_Click(object sender, EventArgs e)
		{
			getPlayerNames = true;
		}

		private void respawnerCheck_CheckedChanged(object sender, EventArgs e)
		{
			respawnerOn = respawnerCheck.Checked;
		}

		private void healerCheck_CheckedChanged(object sender, EventArgs e)
		{
			supportAbilitiesOn = supportAbilitiesCheck.Checked;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			minText.Value = 16;
			maxText.Value = 20;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			// minText.Value = 2147483647 / 2;
			// maxText.Value = 2147483647;
			minText.Value = 16;
			maxText.Value = 20;
		}

		private void goldLaneCheck_CheckedChanged(object sender, EventArgs e)
		{
			goldLaneSwitcherOn = goldLaneCheck.Checked;
		}

		void everythingEnable(bool status)
		{
			autoClickerCheck.Checked = status;
			laneSwitcherCheck.Checked = status;
			elementSwitcherCheck.Checked = status;
			goldLaneCheck.Checked = status;
			respawnerCheck.Checked = status;
			supportAbilitiesCheck.Checked = status;
			bossLaneCheck.Checked = status;
			targetSpawnerCheck.Checked = status;
			ovenzifCheck.Checked = status;
			itemsCheck.Checked = status;
			autoUpgradesCheck.Checked = status;
			badgesCheck.Checked = status;
		}

		private void presetEverythingOff_Click(object sender, EventArgs e)
		{
			everythingEnable(false);
		}

		private void presetEverythingON_Click(object sender, EventArgs e)
		{
			everythingEnable(true);
		}

		private void bossLaneCheck_CheckedChanged(object sender, EventArgs e)
		{
			bossLaneOn = bossLaneCheck.Checked;
		}

		private void targetSpawnerCheck_CheckedChanged(object sender, EventArgs e)
		{
			targetSpawnersOn = targetSpawnerCheck.Checked;
		}

		private void ovenzifCheck_CheckedChanged(object sender, EventArgs e)
		{
			offensiveAbilitiesOn = ovenzifCheck.Checked;
		}

		private void itemsCheck_CheckedChanged(object sender, EventArgs e)
		{
			itemAbilitiesOn = itemsCheck.Checked;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			refreshUpgrades = true;
		}

		private void fireImmediatelyCheck_CheckedChanged(object sender, EventArgs e)
		{
			triggerHappy = fireImmediatelyCheck.Checked;
		}

		private void elementSwitcherCheck_CheckedChanged(object sender, EventArgs e)
		{
			elementSwitcherOn = elementSwitcherCheck.Checked;
		}

		bool showResponsesOn = true;
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			showResponsesOn = checkBox1.Checked;
		}

		private void upgrMaxHP_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxHP = (double)upgrMaxHP.Value;
		}

		private void upgrMaxDPS_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxDPS = (double)upgrMaxDPS.Value;
		}

		private void upgrMaxDmg_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxDamage = (double)upgrMaxDmg.Value;
		}

		private void upgrMaxCrit_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxCrit = (double)upgrMaxCrit.Value;
		}

		private void upgrMaxLoot_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxLoot = (double)upgrMaxLoot.Value;
		}

		private void upgrMaxFire_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxFire = (double)upgrMaxFire.Value;
		}

		private void upgrMaxWater_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxWater = (double)upgrMaxWater.Value;
		}

		private void upgrMaxEarth_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxEarth = (double)upgrMaxEarth.Value;
		}

		private void upgrMaxAir_ValueChanged(object sender, EventArgs e)
		{
			upgradeMaxAir = (double)upgrMaxAir.Value;
		}

		private void autoUpgradesCheck_CheckedChanged(object sender, EventArgs e)
		{
			autoUpgradesOn = autoUpgradesCheck.Checked;
		}

		private void badgesCheck_CheckedChanged(object sender, EventArgs e)
		{
			autoBadgesOn = badgesCheck.Checked;
		}

		private void toolStripStatusLabel1_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://steamcommunity.com/groups/monstergui");
		}

		private void fasterWormhole_CheckedChanged(object sender, EventArgs e)
		{
			superWormholeOn = fasterWormhole.Checked;
		}

		string warpFile;
		System.Net.WebClient warpWc;
		void downloadPicardAsync()
		{
			try
			{
				string source = "http://cdn.meme.am/instances/250x250/61211811.jpg";
				warpFile = Path.GetFullPath(Path.GetTempPath() + "/250x250Warp9.jpg");
				if (!File.Exists(warpFile))
				{
					warpWc = new System.Net.WebClient();
					Console.WriteLine("Downloading {0} to {1}", source, warpFile);
					warpWc.DownloadFileCompleted += wc_DownloadFileCompletedWarp;
					warpWc.DownloadFileAsync(new Uri(source), warpFile);
				}
				else
				{
					Invoke(new EmptyCallback(loadWarp));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		void wc_DownloadFileCompletedWarp(object sender, AsyncCompletedEventArgs e)
		{
			Invoke(new EmptyCallback(loadWarp));
			try
			{
				warpWc.Dispose();
				warpWc = null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		void loadWarp()
		{
			try
			{
				Console.WriteLine("Using image {0}", warpFile);
				Image warp = Image.FromFile(warpFile);
				warpBox.Image = warp;
				warpBox.Visible = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			everythingEnable(true);
			fasterWormhole.Checked = true;
			minText.Value = 20;
			maxText.Value = 20;
			upgrMaxDPS.Value = 1m;
			upgrMaxHP.Value = 1000000000000000m;
			fireImmediatelyCheck.Checked = true;
			if (w9check.Checked)
				warp9();
			if (multiThreadWarp)
				w9check.Checked = true;
			multiWhCheck.Checked = true;
			ovenzifCheck.Checked = false;

			if (!multiThreadWarp)
				MessageBox.Show("Missing MonsterGUI.exe.config, unable to use multi-threaded worm holes");
		}

		private void multiWhCheck_CheckedChanged(object sender, EventArgs e)
		{
			multiWormholeOn = multiWhCheck.Checked;
		}
		
		void warp9()
		{
			if (!warpBox.Visible)
				new Thread(new ThreadStart(downloadPicardAsync)).Start();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox2.Checked)
			{
				string source = "http://i.imgur.com/9R0436k.gif";
				string file = Path.GetFullPath(Path.GetTempPath() + "/PraiseGoldHelmSwag.gif");
				if (!File.Exists(file))
				{
					System.Net.WebClient wc = new System.Net.WebClient();
					Console.WriteLine("Downloading {0} to {1}", source, file);
					wc.DownloadFile(new Uri(source), file);
				}
				Image i = Image.FromFile(file);
				BackgroundImage = i;
				foreach (Control c in Controls)
				{
					c.BackColor = Color.Transparent;
				}
			}
			else
			{
				BackgroundImage = null;
				
			}
		}

		private void warpBox_Click(object sender, EventArgs e)
		{
			warpBox.Visible = false;
		}

		bool w9on = false;
		private void w9check_CheckedChanged(object sender, EventArgs e)
		{
			w9on = w9check.Checked;
			if (w9check.Checked)
			{
				warp9();
			}
			else
			{
				warpBox.Visible = false;
			}
		}
	}
}
