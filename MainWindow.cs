using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
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

		public MainWindow()
		{
			InitializeComponent();
			endedThreadDelegate = new EmptyCallback(endedThread);
			enableDelegate = new EnableCallback(enable);
			getStateInit();
			postAbilitiesInit();
			postUpgradesInit();
			// splitContainer1.Enabled = false;
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
				if (exiting)
				{
					exiting = false;
					Close();
				}
			}
		}

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (exiting)
			{
				e.Cancel = true;
			}
			else
			{
				exiting = true;
				if (running)
				{
					running = false;
					this.Enabled = false;
					e.Cancel = true;
				}
			}
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{

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
			boostText.Value = 1;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			// minText.Value = 2147483647 / 2;
			// maxText.Value = 2147483647;
			minText.Value = 16;
			maxText.Value = 20;
			boostText.Value = 20;
		}

		private void goldLaneCheck_CheckedChanged(object sender, EventArgs e)
		{
			goldLaneSwitcherOn = goldLaneCheck.Checked;
		}

		private void boostText_ValueChanged(object sender, EventArgs e)
		{
			clickBoost = (int)boostText.Value;
		}

		void everythingEnable(bool status)
		{
			autoClickerCheck.Checked = status;
			laneSwitcherCheck.Checked = status;
			goldLaneCheck.Checked = status;
			respawnerCheck.Checked = status;
			supportAbilitiesCheck.Checked = status;
			bossLaneCheck.Checked = status;
			targetSpawnerCheck.Checked = status;
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
	}
}
