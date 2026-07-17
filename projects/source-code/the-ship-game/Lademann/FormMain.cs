using Lademann.Play;
using Lademann.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Lademann
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent(); 
        }
        #region zmienne globalne
        public  class MainGlobals
        {
            
          
            public static string volume = "";
           public static string nickname = "";
            public static string address = "";
            public static string theme = "";
            public static string resolution = "";
            public static string wins = File.ReadAllText("wins");
            public static string PlayedBattles = File.ReadAllText("PlayedBattles");
            public static string DestroyedShips = File.ReadAllText("DestroyedShips");
            public static string TimeInGame = File.ReadAllText("TimeInGame");
            public static bool runServer = false;
            public static int serverport = 0;
            public static string gamemode = "";
            public static bool runTask = true;

        }
        #endregion
        #region zamyka i otwiera kontrolki użytkownika
        public void zmienwidok(string zamknij, string otworz)
        {
            //ServerView1
            if (zamknij == "PlayView") { play1.Visible = false; }
            if (zamknij == "SettingsView") { settingsView1.Visible = false; }
            if (zamknij == "MainView") { mainView1.Visible = false; }
            if (zamknij == "ScoreboardView") { scoreboardView1.Visible = false; }
            if (zamknij == "CreditsView") { creditsView1.Visible = false; }
            if (zamknij == "GameLobbyView") { gameLobbyView1.Visible = false; }
            if (zamknij == "Bot") { bot2.Visible = false; }


            if (otworz == "PlayView") { play1.Visible = true; }
            if (otworz == "SettingsView") { settingsView1.Visible = true; }
            if (otworz == "MainView") { mainView1.Visible = true; }
            if (otworz == "CreditsView") { creditsView1.Visible = true; }
            if (otworz == "ScoreboardView") { scoreboardView1.Visible = true; }
            if (otworz == "GameLobbyView") { gameLobbyView1.Visible = true; }
            if (otworz == "Bot") { bot2.Visible = true; }

        }
        #endregion

        bool ismsglocked = false;
        public void MSG(string msg)
        {
            
            Thread thread = new Thread(() => {
                while (ismsglocked) { }
                
                MSGBOX(-100);
                MSGBOXCONTENT(msg);
                //  msgboxContent.Text = ;
                ismsglocked = true;
                MSGBOXVISIBLE(true);
                for (int i = -100;i <= 0; i+=4)
                {
                    MSGBOX(i);
                   Thread.Sleep(1);
                }
                Thread.Sleep(3000);
                for (int i = 0; i >= -100; i-=4)
                {
                    MSGBOX(i);
                   Thread.Sleep(1);
                }
                MSGBOXVISIBLE(false);
                ismsglocked = false;
            });
            try { thread.Start(); }
            catch(Exception e) { MessageBox.Show(e.Message); }
           
        }
        
        private void MSGBOXCONTENT(string msg)
        {
            if (msgboxContent.InvokeRequired)
            {
                msgboxContent.Invoke(new Action<string>(MSGBOXCONTENT), msg);
                return;
            }
            msgboxContent.Text = msg;
        }
        private void MSGBOX(int i)
        {
            if (msgbox.InvokeRequired)
            {
                try
                {
                    msgbox.Invoke(new Action<int>(MSGBOX), i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }
            msgbox.Location = new Point(390, i);
        }
        private void MSGBOXVISIBLE(bool i)
        {
            if (msgbox.InvokeRequired)
            {
                msgbox.Invoke(new Action<bool>(MSGBOXVISIBLE), i);
                return;
            }
            msgbox.Visible = i;
        }
        private void play1_Load(object sender, EventArgs e)
        {

        }

        private void scoreboardView1_Load(object sender, EventArgs e)
        {

        }

        private void creditsView1_Load(object sender, EventArgs e)
        {

        }

        private void gameLobbyView1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MSG("hello");
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            settingsView1.InitResolution();
        }

        private void bot2_Load(object sender, EventArgs e)
        {

        }
    }


}
