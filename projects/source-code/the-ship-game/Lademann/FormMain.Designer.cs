namespace Lademann
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.msgbox = new System.Windows.Forms.TableLayoutPanel();
            this.msgboxContent = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gameLobbyView1 = new Lademann.views.GameLobbyView();
            this.scoreboardView1 = new Lademann.views.ScoreboardView();
            this.play1 = new Lademann.Play.PlayView();
            this.settingsView1 = new Lademann.views.SettingsView();
            this.mainView1 = new Lademann.views.MainView();
            this.bot2 = new Lademann.views.Bot();
            this.creditsView1 = new Lademann.views.CreditsView();
            this.msgbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // msgbox
            // 
            this.msgbox.BackColor = System.Drawing.SystemColors.ControlText;
            this.msgbox.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetPartial;
            this.msgbox.ColumnCount = 1;
            this.msgbox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.msgbox.Controls.Add(this.msgboxContent, 0, 1);
            this.msgbox.Controls.Add(this.label1, 0, 0);
            this.msgbox.Location = new System.Drawing.Point(391, 0);
            this.msgbox.Name = "msgbox";
            this.msgbox.RowCount = 2;
            this.msgbox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.83505F));
            this.msgbox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 72.16495F));
            this.msgbox.Size = new System.Drawing.Size(500, 83);
            this.msgbox.TabIndex = 6;
            this.msgbox.Visible = false;
            // 
            // msgboxContent
            // 
            this.msgboxContent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.msgboxContent.AutoSize = true;
            this.msgboxContent.Font = new System.Drawing.Font("Mario Kart DS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msgboxContent.ForeColor = System.Drawing.Color.LightSeaGreen;
            this.msgboxContent.Location = new System.Drawing.Point(208, 45);
            this.msgboxContent.Name = "msgboxContent";
            this.msgboxContent.Size = new System.Drawing.Size(83, 15);
            this.msgboxContent.TabIndex = 1;
            this.msgboxContent.Text = "message";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Mario Kart DS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(188, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "NOTIFICATION";
            // 
            // gameLobbyView1
            // 
            this.gameLobbyView1.BackColor = System.Drawing.Color.Black;
            this.gameLobbyView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameLobbyView1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gameLobbyView1.Location = new System.Drawing.Point(0, 0);
            this.gameLobbyView1.Margin = new System.Windows.Forms.Padding(4);
            this.gameLobbyView1.Name = "gameLobbyView1";
            this.gameLobbyView1.Size = new System.Drawing.Size(1264, 761);
            this.gameLobbyView1.TabIndex = 5;
            this.gameLobbyView1.Visible = false;
            // 
            // scoreboardView1
            // 
            this.scoreboardView1.BackColor = System.Drawing.Color.Transparent;
            this.scoreboardView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scoreboardView1.Location = new System.Drawing.Point(0, 0);
            this.scoreboardView1.Margin = new System.Windows.Forms.Padding(4);
            this.scoreboardView1.Name = "scoreboardView1";
            this.scoreboardView1.Size = new System.Drawing.Size(1264, 761);
            this.scoreboardView1.TabIndex = 3;
            this.scoreboardView1.Visible = false;
            this.scoreboardView1.Load += new System.EventHandler(this.scoreboardView1_Load);
            // 
            // play1
            // 
            this.play1.BackColor = System.Drawing.Color.Transparent;
            this.play1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("play1.BackgroundImage")));
            this.play1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.play1.Location = new System.Drawing.Point(0, 0);
            this.play1.Margin = new System.Windows.Forms.Padding(4);
            this.play1.Name = "play1";
            this.play1.Size = new System.Drawing.Size(1264, 761);
            this.play1.TabIndex = 2;
            this.play1.Visible = false;
            this.play1.Load += new System.EventHandler(this.play1_Load);
            // 
            // settingsView1
            // 
            this.settingsView1.BackColor = System.Drawing.Color.Black;
            this.settingsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsView1.Location = new System.Drawing.Point(0, 0);
            this.settingsView1.Margin = new System.Windows.Forms.Padding(4);
            this.settingsView1.Name = "settingsView1";
            this.settingsView1.Size = new System.Drawing.Size(1264, 761);
            this.settingsView1.TabIndex = 1;
            this.settingsView1.Visible = false;
            // 
            // mainView1
            // 
            this.mainView1.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.mainView1.BackColor = System.Drawing.Color.Transparent;
            this.mainView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainView1.Location = new System.Drawing.Point(0, 0);
            this.mainView1.Margin = new System.Windows.Forms.Padding(4);
            this.mainView1.Name = "mainView1";
            this.mainView1.Size = new System.Drawing.Size(1264, 761);
            this.mainView1.TabIndex = 0;
            // 
            // bot2
            // 
            this.bot2.BackColor = System.Drawing.Color.Black;
            this.bot2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bot2.Location = new System.Drawing.Point(0, 0);
            this.bot2.Name = "bot2";
            this.bot2.Size = new System.Drawing.Size(1264, 761);
            this.bot2.TabIndex = 8;
            this.bot2.Visible = false;
            this.bot2.Load += new System.EventHandler(this.bot2_Load);
            // 
            // creditsView1
            // 
            this.creditsView1.BackColor = System.Drawing.Color.Transparent;
            this.creditsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.creditsView1.Location = new System.Drawing.Point(0, 0);
            this.creditsView1.Margin = new System.Windows.Forms.Padding(4);
            this.creditsView1.Name = "creditsView1";
            this.creditsView1.Size = new System.Drawing.Size(1264, 761);
            this.creditsView1.TabIndex = 4;
            this.creditsView1.Visible = false;
            this.creditsView1.Load += new System.EventHandler(this.creditsView1_Load);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.msgbox);
            this.Controls.Add(this.scoreboardView1);
            this.Controls.Add(this.play1);
            this.Controls.Add(this.settingsView1);
            this.Controls.Add(this.mainView1);
            this.Controls.Add(this.bot2);
            this.Controls.Add(this.creditsView1);
            this.Controls.Add(this.gameLobbyView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "THE SHIP GAME";
            this.VisibleChanged += new System.EventHandler(this.FormMain_VisibleChanged);
            this.msgbox.ResumeLayout(false);
            this.msgbox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private views.MainView mainView1;
        private views.SettingsView settingsView1;
        private Play.PlayView play1;
        private views.ScoreboardView scoreboardView1;
        private views.CreditsView creditsView1;
        private views.GameLobbyView gameLobbyView1;
        private System.Windows.Forms.Label msgboxContent;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TableLayoutPanel msgbox;
        private views.Bot bot2;
        //  private views.GameView gameView1;
    }
}