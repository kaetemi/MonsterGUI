namespace MonsterGUI
{
	partial class MainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.roomText = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.accessTokenText = new System.Windows.Forms.TextBox();
			this.go = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.clicksText = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.autoClickerCheck = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.minText = new System.Windows.Forms.NumericUpDown();
			this.maxText = new System.Windows.Forms.NumericUpDown();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.laneSwitcherCheck = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.laneSwitcherTimer = new System.Windows.Forms.NumericUpDown();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.targetLabel = new System.Windows.Forms.Label();
			this.hpLabel = new System.Windows.Forms.Label();
			this.goldLabel = new System.Windows.Forms.Label();
			this.currentLaneLabel = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.playerGroup = new System.Windows.Forms.GroupBox();
			this.playerListRefresh = new System.Windows.Forms.Button();
			this.playerList = new System.Windows.Forms.ListBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.getStateStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.postAbilitiesState = new System.Windows.Forms.ToolStripStatusLabel();
			this.respawnerGroup = new System.Windows.Forms.GroupBox();
			this.respawnerCheck = new System.Windows.Forms.CheckBox();
			this.deadAliveText = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.medicsText = new System.Windows.Forms.Label();
			this.healerCheck = new System.Windows.Forms.CheckBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.levelText = new System.Windows.Forms.Label();
			this.activePlayersText = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.roomText)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.minText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxText)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.laneSwitcherTimer)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.playerGroup.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.respawnerGroup.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.roomText);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.accessTokenText);
			this.groupBox1.Controls.Add(this.go);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(760, 71);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Authentication";
			// 
			// roomText
			// 
			this.roomText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.roomText.Location = new System.Drawing.Point(94, 45);
			this.roomText.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
			this.roomText.Name = "roomText";
			this.roomText.Size = new System.Drawing.Size(579, 20);
			this.roomText.TabIndex = 5;
			this.roomText.Value = new decimal(new int[] {
            37559,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Room:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Access Token: ";
			// 
			// accessTokenText
			// 
			this.accessTokenText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.accessTokenText.Location = new System.Drawing.Point(94, 19);
			this.accessTokenText.Name = "accessTokenText";
			this.accessTokenText.PasswordChar = '•';
			this.accessTokenText.Size = new System.Drawing.Size(579, 20);
			this.accessTokenText.TabIndex = 1;
			// 
			// go
			// 
			this.go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.go.Location = new System.Drawing.Point(679, 19);
			this.go.Name = "go";
			this.go.Size = new System.Drawing.Size(75, 46);
			this.go.TabIndex = 0;
			this.go.Text = "Go";
			this.go.UseVisualStyleBackColor = true;
			this.go.Click += new System.EventHandler(this.go_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.button2);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Controls.Add(this.clicksText);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.autoClickerCheck);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.minText);
			this.groupBox2.Controls.Add(this.maxText);
			this.groupBox2.Location = new System.Drawing.Point(209, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(388, 71);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Auto Clicker";
			// 
			// clicksText
			// 
			this.clicksText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clicksText.Location = new System.Drawing.Point(236, 47);
			this.clicksText.Name = "clicksText";
			this.clicksText.Size = new System.Drawing.Size(65, 13);
			this.clicksText.TabIndex = 6;
			this.clicksText.Text = "0";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(192, 47);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Clicks:";
			// 
			// postAbilitiesRunCheck
			// 
			this.autoClickerCheck.AutoSize = true;
			this.autoClickerCheck.Location = new System.Drawing.Point(192, 19);
			this.autoClickerCheck.Name = "postAbilitiesRunCheck";
			this.autoClickerCheck.Size = new System.Drawing.Size(66, 17);
			this.autoClickerCheck.TabIndex = 4;
			this.autoClickerCheck.Text = "Running";
			this.autoClickerCheck.UseVisualStyleBackColor = true;
			this.autoClickerCheck.CheckedChanged += new System.EventHandler(this.postAbilitiesRunCheck_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Minimum:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Maximum:";
			// 
			// minText
			// 
			this.minText.Location = new System.Drawing.Point(66, 19);
			this.minText.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.minText.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.minText.Name = "minText";
			this.minText.Size = new System.Drawing.Size(120, 20);
			this.minText.TabIndex = 1;
			this.minText.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.minText.ValueChanged += new System.EventHandler(this.minText_ValueChanged);
			// 
			// maxText
			// 
			this.maxText.Location = new System.Drawing.Point(66, 45);
			this.maxText.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.maxText.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.maxText.Name = "maxText";
			this.maxText.Size = new System.Drawing.Size(120, 20);
			this.maxText.TabIndex = 0;
			this.maxText.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.maxText.ValueChanged += new System.EventHandler(this.maxText_ValueChanged);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(12, 89);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBox6);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox5);
			this.splitContainer1.Panel1.Controls.Add(this.respawnerGroup);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.playerGroup);
			this.splitContainer1.Size = new System.Drawing.Size(760, 448);
			this.splitContainer1.SplitterDistance = 600;
			this.splitContainer1.TabIndex = 2;
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.laneSwitcherCheck);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.laneSwitcherTimer);
			this.groupBox4.Location = new System.Drawing.Point(209, 80);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(388, 45);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Timed Lane Switcher";
			// 
			// laneSwitcherCheck
			// 
			this.laneSwitcherCheck.AutoSize = true;
			this.laneSwitcherCheck.Location = new System.Drawing.Point(192, 19);
			this.laneSwitcherCheck.Name = "laneSwitcherCheck";
			this.laneSwitcherCheck.Size = new System.Drawing.Size(66, 17);
			this.laneSwitcherCheck.TabIndex = 2;
			this.laneSwitcherCheck.Text = "Running";
			this.laneSwitcherCheck.UseVisualStyleBackColor = true;
			this.laneSwitcherCheck.CheckedChanged += new System.EventHandler(this.laneSwitcherCheck_CheckedChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 21);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(53, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Timer (s): ";
			// 
			// laneSwitcherTimer
			// 
			this.laneSwitcherTimer.Location = new System.Drawing.Point(66, 19);
			this.laneSwitcherTimer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.laneSwitcherTimer.Name = "laneSwitcherTimer";
			this.laneSwitcherTimer.Size = new System.Drawing.Size(120, 20);
			this.laneSwitcherTimer.TabIndex = 0;
			this.laneSwitcherTimer.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.laneSwitcherTimer.ValueChanged += new System.EventHandler(this.laneSwitcherTimer_ValueChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.targetLabel);
			this.groupBox3.Controls.Add(this.hpLabel);
			this.groupBox3.Controls.Add(this.goldLabel);
			this.groupBox3.Controls.Add(this.currentLaneLabel);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Location = new System.Drawing.Point(3, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 71);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Player Data";
			// 
			// targetLabel
			// 
			this.targetLabel.AutoSize = true;
			this.targetLabel.Location = new System.Drawing.Point(83, 55);
			this.targetLabel.Name = "targetLabel";
			this.targetLabel.Size = new System.Drawing.Size(0, 13);
			this.targetLabel.TabIndex = 7;
			// 
			// hpLabel
			// 
			this.hpLabel.AutoSize = true;
			this.hpLabel.Location = new System.Drawing.Point(83, 16);
			this.hpLabel.Name = "hpLabel";
			this.hpLabel.Size = new System.Drawing.Size(0, 13);
			this.hpLabel.TabIndex = 6;
			// 
			// goldLabel
			// 
			this.goldLabel.AutoSize = true;
			this.goldLabel.Location = new System.Drawing.Point(83, 29);
			this.goldLabel.Name = "goldLabel";
			this.goldLabel.Size = new System.Drawing.Size(0, 13);
			this.goldLabel.TabIndex = 5;
			// 
			// currentLaneLabel
			// 
			this.currentLaneLabel.AutoSize = true;
			this.currentLaneLabel.Location = new System.Drawing.Point(83, 42);
			this.currentLaneLabel.Name = "currentLaneLabel";
			this.currentLaneLabel.Size = new System.Drawing.Size(0, 13);
			this.currentLaneLabel.TabIndex = 4;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(36, 55);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(41, 13);
			this.label9.TabIndex = 3;
			this.label9.Text = "Target:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 42);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(71, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "Current Lane:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(45, 29);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 13);
			this.label7.TabIndex = 1;
			this.label7.Text = "Gold:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(53, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Hp:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// playerGroup
			// 
			this.playerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.playerGroup.Controls.Add(this.playerListRefresh);
			this.playerGroup.Controls.Add(this.playerList);
			this.playerGroup.Location = new System.Drawing.Point(3, 3);
			this.playerGroup.Name = "playerGroup";
			this.playerGroup.Size = new System.Drawing.Size(149, 442);
			this.playerGroup.TabIndex = 0;
			this.playerGroup.TabStop = false;
			this.playerGroup.Text = "Players";
			// 
			// playerListRefresh
			// 
			this.playerListRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.playerListRefresh.Location = new System.Drawing.Point(68, 413);
			this.playerListRefresh.Name = "playerListRefresh";
			this.playerListRefresh.Size = new System.Drawing.Size(75, 23);
			this.playerListRefresh.TabIndex = 1;
			this.playerListRefresh.Text = "Refresh";
			this.playerListRefresh.UseVisualStyleBackColor = true;
			this.playerListRefresh.Click += new System.EventHandler(this.playerListRefresh_Click);
			// 
			// playerList
			// 
			this.playerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.playerList.FormattingEnabled = true;
			this.playerList.IntegralHeight = false;
			this.playerList.Location = new System.Drawing.Point(6, 19);
			this.playerList.Name = "playerList";
			this.playerList.Size = new System.Drawing.Size(137, 388);
			this.playerList.Sorted = true;
			this.playerList.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getStateStatus,
            this.postAbilitiesState});
			this.statusStrip1.Location = new System.Drawing.Point(0, 540);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(784, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip";
			// 
			// getStateStatus
			// 
			this.getStateStatus.Enabled = false;
			this.getStateStatus.Name = "getStateStatus";
			this.getStateStatus.Size = new System.Drawing.Size(17, 17);
			this.getStateStatus.Text = "▼";
			// 
			// postAbilitiesState
			// 
			this.postAbilitiesState.Enabled = false;
			this.postAbilitiesState.Name = "postAbilitiesState";
			this.postAbilitiesState.Size = new System.Drawing.Size(17, 17);
			this.postAbilitiesState.Text = "▲";
			// 
			// respawnerGroup
			// 
			this.respawnerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.respawnerGroup.Controls.Add(this.deadAliveText);
			this.respawnerGroup.Controls.Add(this.respawnerCheck);
			this.respawnerGroup.Location = new System.Drawing.Point(209, 131);
			this.respawnerGroup.Name = "respawnerGroup";
			this.respawnerGroup.Size = new System.Drawing.Size(388, 45);
			this.respawnerGroup.TabIndex = 4;
			this.respawnerGroup.TabStop = false;
			this.respawnerGroup.Text = "Respawner";
			// 
			// respawnerCheck
			// 
			this.respawnerCheck.AutoSize = true;
			this.respawnerCheck.Location = new System.Drawing.Point(192, 19);
			this.respawnerCheck.Name = "respawnerCheck";
			this.respawnerCheck.Size = new System.Drawing.Size(66, 17);
			this.respawnerCheck.TabIndex = 3;
			this.respawnerCheck.Text = "Running";
			this.respawnerCheck.UseVisualStyleBackColor = true;
			this.respawnerCheck.CheckedChanged += new System.EventHandler(this.respawnerCheck_CheckedChanged);
			// 
			// deadAliveText
			// 
			this.deadAliveText.AutoSize = true;
			this.deadAliveText.Location = new System.Drawing.Point(6, 21);
			this.deadAliveText.Name = "deadAliveText";
			this.deadAliveText.Size = new System.Drawing.Size(0, 13);
			this.deadAliveText.TabIndex = 4;
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.medicsText);
			this.groupBox5.Controls.Add(this.healerCheck);
			this.groupBox5.Location = new System.Drawing.Point(209, 182);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(388, 45);
			this.groupBox5.TabIndex = 5;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Healer";
			// 
			// medicsText
			// 
			this.medicsText.AutoSize = true;
			this.medicsText.Location = new System.Drawing.Point(6, 21);
			this.medicsText.Name = "medicsText";
			this.medicsText.Size = new System.Drawing.Size(0, 13);
			this.medicsText.TabIndex = 4;
			// 
			// healerCheck
			// 
			this.healerCheck.AutoSize = true;
			this.healerCheck.Location = new System.Drawing.Point(192, 19);
			this.healerCheck.Name = "healerCheck";
			this.healerCheck.Size = new System.Drawing.Size(66, 17);
			this.healerCheck.TabIndex = 3;
			this.healerCheck.Text = "Running";
			this.healerCheck.UseVisualStyleBackColor = true;
			this.healerCheck.CheckedChanged += new System.EventHandler(this.healerCheck_CheckedChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.levelText);
			this.groupBox6.Controls.Add(this.activePlayersText);
			this.groupBox6.Controls.Add(this.label18);
			this.groupBox6.Controls.Add(this.label19);
			this.groupBox6.Location = new System.Drawing.Point(3, 80);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(200, 45);
			this.groupBox6.TabIndex = 8;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Game Data";
			// 
			// levelText
			// 
			this.levelText.AutoSize = true;
			this.levelText.Location = new System.Drawing.Point(83, 16);
			this.levelText.Name = "levelText";
			this.levelText.Size = new System.Drawing.Size(0, 13);
			this.levelText.TabIndex = 6;
			// 
			// activePlayersText
			// 
			this.activePlayersText.AutoSize = true;
			this.activePlayersText.Location = new System.Drawing.Point(83, 29);
			this.activePlayersText.Name = "activePlayersText";
			this.activePlayersText.Size = new System.Drawing.Size(0, 13);
			this.activePlayersText.TabIndex = 5;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(37, 29);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(40, 13);
			this.label18.TabIndex = 1;
			this.label18.Text = "Active:";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(41, 16);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(36, 13);
			this.label19.TabIndex = 0;
			this.label19.Text = "Level:";
			this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(307, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Regular";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(307, 42);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Power";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.groupBox1);
			this.Name = "MainWindow";
			this.Text = "Monster Game";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			this.Load += new System.EventHandler(this.MainWindow_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.roomText)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.minText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxText)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.laneSwitcherTimer)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.playerGroup.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.respawnerGroup.ResumeLayout(false);
			this.respawnerGroup.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox accessTokenText;
		private System.Windows.Forms.Button go;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox playerGroup;
		private System.Windows.Forms.NumericUpDown roomText;
		private System.Windows.Forms.ListBox playerList;
		private System.Windows.Forms.Button playerListRefresh;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown minText;
		private System.Windows.Forms.NumericUpDown maxText;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel getStateStatus;
		private System.Windows.Forms.ToolStripStatusLabel postAbilitiesState;
		private System.Windows.Forms.Label clicksText;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox autoClickerCheck;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label targetLabel;
		private System.Windows.Forms.Label hpLabel;
		private System.Windows.Forms.Label goldLabel;
		private System.Windows.Forms.Label currentLaneLabel;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.NumericUpDown laneSwitcherTimer;
		private System.Windows.Forms.CheckBox laneSwitcherCheck;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox respawnerGroup;
		private System.Windows.Forms.CheckBox respawnerCheck;
		private System.Windows.Forms.Label deadAliveText;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label levelText;
		private System.Windows.Forms.Label activePlayersText;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label medicsText;
		private System.Windows.Forms.CheckBox healerCheck;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
	}
}

