using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using static Lademann.views.AI;
using System.Diagnostics;
using System.IO;

namespace Lademann.views
{
    public partial class Bot : UserControl
    {
        public Bot()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Wyłącz odtwarzanie dźwięku w trybie designu
                return;
            }
            initializePicBoxes();

            shipsLeftLabels = new Label[4] { label8, label9, label36, label37 };
        }
        bool[,] ClientArray = new bool[10, 10];
        public void Bot_Load(object a, EventArgs b)
        {
            
           
            //    //MessageBox.Show("botArray\n" + m);
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      
        #region gamelobby
      #region ship place
        enum ShipOrientation : byte
        {
            Vertical,
            Horizontal,
        }
        

        ShipOrientation shipOrientation = ShipOrientation.Horizontal;

        bool[,] shipCords = new bool[10, 10];

        int shipsPlaced = 0;

        int[] shipsOfNSizePlaced = new int[4];

        int currentShipSize = 1;

        Label[] shipsLeftLabels;

        Image[] images = new Image[100];

        string shipTextureFolderName()
        {
            switch (currentShipSize)
            {
                case 1: return "one";
                case 2: return "two";
                case 3: return "three";
                case 4: return "four";
            }
            return "Unreachable";
        }

        bool isInBounds(int i)
        {
            return i < 10 && i >= 0;
        }

        PictureBox getPicBox(int line, int row)
        {
            return this.Controls.Find(string.Format("pictureBox{0}", line * 10 + row + 6), true).First() as PictureBox;
        }

        void nextShip()
        {
            ++shipsPlaced;
            ++shipsOfNSizePlaced[currentShipSize - 1];
            var lbl = shipsLeftLabels[currentShipSize - 1];
            lbl.Text = (int.Parse(lbl.Text.Substring(0, 1)) - 1).ToString() + "x";
        }

        void resetShipCountLabels()
        {
            int i = 4;
            foreach (var lbl in shipsLeftLabels)
            {
                this.Invoke(new Action(() => { lbl.Text = String.Format("{0}x", i); }));

                --i;
            }
        }

        void initializePicBoxes()
        {
            // button2 - button101
            // ustawia taką samą funkcje onclick dla każdego buttona ale z innym argumentem (odpowiednimi koordynatami)
            // ponizszy kod zalezy od nazw w projektancie

            for (int i = 6; i <= 105; ++i)
            {
                var buf = this.Controls.Find(string.Format("pictureBox{0}", i), true).First() as PictureBox;
                int pos = i - 6; // 0 - 99
                int line = pos / 10;
                int row = pos % 10;

                buf.Click += (sender, EventArgs) => { placementPicBoxOnClick(sender, EventArgs, line, row); };
            }
        }

        bool userCanPlaceShipOfCurrentSize()
        {
            return shipsOfNSizePlaced[currentShipSize - 1] < (5 - currentShipSize);
        }


        void placementPicBoxOnClick(object sender, EventArgs e, int line, int row)
        {
            // gdy statek jest w pionie bedzie ustawiac go na kratke na ktora kliknieto i na pozostale kierując się w dół
            // gdy statek jest w poziomie bedzie ustawiac go na kratke na ktora kliknieto i na pozostale kierujac sie w prawo

            if (shipsPlaced == 10)
            {
                ((FormMain)Parent).MSG("You have already placed all of your ships!");
                return;
            }

            if (!userCanPlaceShipOfCurrentSize())
            {
                ((FormMain)Parent).MSG("You don't have any more ships of this size!");
                return;
            }

            Action earlyReturn = () =>
            {
                ((FormMain)Parent).MSG("You can't place the ship on this field!");
            };

            if (shipOrientation == ShipOrientation.Vertical)
            {
                for (int i = 0; i < currentShipSize; ++i)
                {
                    // najpierw sprawdz czy nie wyjdzie poza boundsy, potem czy nie koliduje z innym statkiem
                    int l = line + i;

                    if (!isInBounds(l) || shipCords[l, row])
                    {
                        earlyReturn();
                        return;
                    }

                    // sprawdz czy na polach dookola (sciany dluzsze) nie ma statku

                    int row_to_check = row + 1;
                    if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na prawo od statku
                    {
                        earlyReturn();
                        return;
                    }

                    row_to_check = row - 1;
                    if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na lewo od statku
                    {
                        earlyReturn();
                        return;
                    }
                }

                // sprawdz dwa pola - pod statkiem i nad

                int line_to_check = line + currentShipSize;
                if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole pod statkiem
                {
                    earlyReturn();
                    return;
                }

                line_to_check = line - 1;
                if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole nad statkiem
                {
                    earlyReturn();
                    return;
                }

                for (int i = 0; i < currentShipSize; ++i)
                {
                    int l = line + i;
                    shipCords[l, row] = true;

                    getPicBox(l, row).BackgroundImage =
                        Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\" + shipTextureFolderName() + @"\" + (i + 1).ToString() + "of" + currentShipSize.ToString() + "-v.png"));
                }
            }
            else if (shipOrientation == ShipOrientation.Horizontal)
            {
                for (int i = 0; i < currentShipSize; ++i)
                {
                    int r = row + i;

                    if (!isInBounds(r) || shipCords[line, r])
                    {
                        earlyReturn();
                        return;
                    }

                    int line_to_check = line + 1;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola pod statkiem
                    {
                        earlyReturn();
                        return;
                    }

                    line_to_check = line - 1;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola nad
                    {
                        earlyReturn();
                        return;
                    }
                }

                int row_to_check = row + currentShipSize;
                if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na prawo od statku
                {
                    earlyReturn();
                    return;
                }

                row_to_check = row - 1;
                if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na lewo
                {
                    earlyReturn();
                    return;
                }

                // zaktualizuj matrix i ustaw kolory buttonow
                for (int i = 0; i < currentShipSize; ++i)
                {

                    int r = row + i;
                    shipCords[line, r] = true;
                    shipCords[line, r] = true;
                    getPicBox(line, r).BackgroundImage =
                         Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\" + shipTextureFolderName() + @"\" + (i + 1).ToString() + "of" + currentShipSize.ToString() + "-h.png"));
                }
            }

            if (shipsPlaced == 9)
            {
                WhenAllShipsPlaced();
               // setupGame();
            }

            nextShip();
        }
        bool[,] myArr = new bool[10, 10];
        private void WhenAllShipsPlaced()
        {
            for (int i = 6; i <= 105; ++i)
            {
                var picb = this.Controls.Find(string.Format("pictureBox{0}", i), true).First() as PictureBox;
                if (picb.BackgroundImage != null)
                {
                    images[i - 6] = picb.BackgroundImage;
                }
            }

            if (FormMain.MainGlobals.gamemode == "Host")
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        HostArray[i, j] = shipCords[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ClientArray[i, j] = shipCords[i, j];
                    }
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                shipOrientation = ShipOrientation.Horizontal;
            }
            else
            {
                shipOrientation = ShipOrientation.Vertical;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                currentShipSize = 1;
                pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-v.png");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                currentShipSize = 2;
                pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\two\full-h.png");
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                currentShipSize = 3;
                pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\three\full-h.png");
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                currentShipSize = 4;
                pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\four\full-h.png");
            }
        }



        #endregion

        public PictureBox getMyPicbox(int i, int j)
        {
            return tableLayoutPanel12.GetControlFromPosition(j + 1, i + 1) as PictureBox;
        }

        public PictureBox getOpponentPicbox(int i, int j)
        {
            return tableLayoutPanel19.GetControlFromPosition(j + 1, i + 1) as PictureBox;
        }

        Image imgX = Image.FromFile(@"..\..\resources\textures\ui\x.png");
        Image imgMissed = Image.FromFile(@"..\..\resources\textures\ui\not_allowed.png"); // do zmiany
        Image imgUnknown = Image.FromFile(@"..\..\resources\textures\ui\unknown_white.png");
        byte shipsShot = 0;
        #endregion

        public static bool ItIsMyTurn = false;

        private void setgameback_Click(object sender, EventArgs e) { }

        public static bool HostReady = false;
        private void readybtn_Click(object sender, EventArgs e)
        {

            if (label8.Text == "0x" && label9.Text == "0x" && label37.Text == "0x" && label36.Text == "0x")//
            {
                LobbyUpdateOpponentNick("opponent: ai");
                LobbyUpdateOpponentStatusk("always connected and ready");
                if (HostReady)
                    {
                        HostReady = false;
                        LobbyUpdateMeStatus("connected but not ready");
                    
                       // readybtn.Text = "READY";
                    }
                    else
                    {
                        HostReady = true;
                       
                        //readybtn.Text = "NOT READY";
                        CheckIfBothReady();
                        //Czarek trzeba przepisac arr do HostArr
                    }
               
                }
            
            else
            {
                ((FormMain)Parent).MSG("some ships left");
            }
        }






        public static string HostNickname, ClientNickname = "";
      //  public static bool[,] ClientArray = new bool[10, 10];
        public static bool[,] HostArray = new bool[10, 10];
        private void GameLobbyView_VisibleChanged(object sender, EventArgs e)
        {
            if (FormMain.MainGlobals.nickname.Length < 3) return;
            if (this.Visible)
            {
                cleanupAfterGame();
                // 
                shipsShot = 0;
                GameNotify.Text = "";
                pictureBox279.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png");
                pictureBox278.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\two\full-h.png");
                pictureBox277.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\three\full-h.png");
                pictureBox276.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\four\full-h.png");

                pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png");
                pictureBox1.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png"));
                pictureBox4.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\two\full-h.png"));
                pictureBox2.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\three\full-h.png"));
                pictureBox3.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\four\full-h.png"));
               
              

                    SwitchTab(0);
               
              
               
              

            }
            else {
                czasGry.Stop();
                int wins = int.Parse(File.ReadAllText("wins"));
                int PlayedBattles = int.Parse(File.ReadAllText("PlayedBattles"));
                int DestroyedShips = int.Parse(File.ReadAllText("DestroyedShips"));
                int TimeInGame = int.Parse(File.ReadAllText("TimeInGame"));
                File.WriteAllText("wins", (wins + 0).ToString());
                File.WriteAllText("PlayedBattles", (PlayedBattles + 1).ToString());
                File.WriteAllText("DestroyedShips", (DestroyedShips + shipsShot).ToString());
                int czas = int.Parse(czasGry.ElapsedMilliseconds.ToString()) / 6000;
                File.WriteAllText("TimeInGame", (TimeInGame + czas).ToString());
                FormMain.MainGlobals.wins = (wins + 1).ToString();
                FormMain.MainGlobals.PlayedBattles = (PlayedBattles + 1).ToString();
                FormMain.MainGlobals.DestroyedShips = (DestroyedShips + shipsShot).ToString();
                FormMain.MainGlobals.TimeInGame = (TimeInGame + czas).ToString();
            }
            
        }

        void resetEnemyShipFields()
        {
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var picb = tableLayoutPanel19.GetControlFromPosition(j + 1, i + 1) as PictureBox;
                    picb.BackgroundImage = imgUnknown;
                }
            }
        }

       

        void askUserToPlayAgain()
        {
            this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));
            // DialogResult res = //MessageBox.Show("Do you want to play again?", "", MessageBoxButtons.YesNo);
            //  return res == DialogResult.Yes;
        }

      
        private void Play_label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 20);

        }
        private void Play_label_MouseHover(object sender, EventArgs e)
        {

            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 16F);

        }
        public void InitializeUI()
        {

                meNickname.Text = "me: " + FormMain.MainGlobals.nickname;
                meNicknameGame.Text = "me: " + FormMain.MainGlobals.nickname;
                lbmeready.Text = "connected but not ready";
               
                //   tabControl1.SelectedIndex = 0;
              
           

        }
      

        void setupGame()
        {
            czasGry.Start();
            czasGry.Reset();

            AI ai = new AI();
            ClientArray = ai.GenerateMatrix();
            string m = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (ClientArray[i, j])
                    {
                        m += "# ";
                    }
                    else
                    {
                        m += "- ";
                    }
                }
                m += "\n";
            }
            //MessageBox.Show("botArray\n" + m);
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var picb = tableLayoutPanel12.GetControlFromPosition(j + 1, i + 1) as PictureBox;
                    //if (images[i * 10 + j] != null) {
                    picb.BackgroundImage = images[i * 10 + j];
                    //}
                }
            }
        }

      
        private void SwitchTab(int index)
        {
            if (tabControl1.InvokeRequired)
            {
                tabControl1.Invoke(new Action<int>(SwitchTab), index);
                return;
            }
            tabControl1.SelectedIndex = index;
        }
        private void LobbyUpdateMeNick(string text)
        {
            if (meNickname.InvokeRequired)
            {
                meNickname.Invoke(new Action<string>(LobbyUpdateMeNick), text);
                return;
            }
            meNickname.Text = text;
        }
        private void GameUpdateOpponentNick(string text)
        {
            if (opponentNickGame.InvokeRequired)
            {
                opponentNickGame.Invoke(new Action<string>(GameUpdateOpponentNick), text);
                return;
            }
            opponentNickGame.Text = text;
        }
        private void LobbyUpdateOpponentNick(string text)
        {
            if (opponentNickLobby.InvokeRequired)
            {
                opponentNickLobby.Invoke(new Action<string>(LobbyUpdateOpponentNick), text);
                return;
            }
            opponentNickLobby.Text = text;
        }
        private void LobbyUpdateMeStatus(string text)
        {
            if (lbmeready.InvokeRequired)
            {
                lbmeready.Invoke(new Action<string>(LobbyUpdateMeStatus), text);
                return;
            }
            lbmeready.Text = text;
        }
        private void LobbyUpdateOpponentStatusk(string text)
        {
            if (opponentstatusLobby.InvokeRequired)
            {
                opponentstatusLobby.Invoke(new Action<string>(LobbyUpdateOpponentStatusk), text);
                return;
            }
            opponentstatusLobby.Text = text;
        }

        void cleanupAfterGame()
        {
            labeljeden.Text = "4";
            labeldwa.Text = "3";
            labeltrzy.Text = "2";
            labelcztery.Text = "1";
            clickedArr = new bool[10, 10];
            shipCords = new bool[10, 10];
            ClientArray = new bool[10, 10]; //Bot Arr
            HostArray = new bool[10, 10]; //moj arr
            shipsPlaced = 0;
            shipOrientation = ShipOrientation.Horizontal;
            shipsOfNSizePlaced = new int[4];
            currentShipSize = 1;
            images = new Image[100];
            HostReady = false;
           
            //listenerPort = 20000;
           


            //reset picboxow w lobby
            for (int i = 6; i <= 105; ++i)
            {
                var buf = this.Controls.Find(string.Format("pictureBox{0}", i), true).First() as PictureBox;
                buf.BackgroundImage = null;
            }

            resetShipCountLabels();
            resetEnemyShipFields();
        }

        void whenGameEndsSuccessfully(bool res)
        {
            czasGry.Stop();
            int wins = int.Parse(File.ReadAllText("wins"));
            int PlayedBattles = int.Parse(File.ReadAllText("PlayedBattles"));
            int DestroyedShips = int.Parse(File.ReadAllText("DestroyedShips"));
            int TimeInGame = int.Parse(File.ReadAllText("TimeInGame"));
            File.WriteAllText("wins", (wins + ((res) ? 1 : 0)).ToString());
            File.WriteAllText("PlayedBattles", (PlayedBattles + 1).ToString());
            File.WriteAllText("DestroyedShips", (DestroyedShips + shipsShot).ToString());
            int czas = int.Parse(czasGry.ElapsedMilliseconds.ToString()) / 6000;
            File.WriteAllText("TimeInGame", (TimeInGame + czas).ToString());
            FormMain.MainGlobals.wins = (wins + 1).ToString();
            FormMain.MainGlobals.PlayedBattles = (PlayedBattles + 1).ToString();
            FormMain.MainGlobals.DestroyedShips = (DestroyedShips + shipsShot).ToString();
            FormMain.MainGlobals.TimeInGame = (TimeInGame + czas).ToString();
            if (res)
            {
              
                    ((FormMain)Parent).MSG("new game started");

                    SwitchTab(0);
              
            }
            else
            {
                //MessageBox.Show("przeszlo");
                ((FormMain)Parent).MSG("game ended");
                FormMain myparent = (FormMain)Parent;
                myparent.zmienwidok("Bot", "PlayView");
                
            }
        }

      
       
      

        public void handleClientShot(int x, int y, bool res)
        {
            //getOpponentPicbox(x, y).BackgroundImage = res ? imgX : imgMissed;
            PictureBox pb = getOpponentPicbox(x, y);

            if (res)
            {
                pb.BackgroundImage = imgX;
                ++shipsShot;
                if (shipsShot >= 20)
                {
                    this.Invoke(new Action(() => { cleanupAfterGame(); }));
                    IWon();
                }
            }
            else
            {
                pb.BackgroundImage = imgMissed;
            }
            //czy aktualizowac hostarray?
            //HostArray[i, j] = false;
        }

      
        public void HostShootedYou(int i, int j, bool res)//client action
        {
            //Czarek ma sie zmienic minuaturka ze wzgl na res
            if (res)
            {
                getMyPicbox(i, j).BackgroundImage = imgX;
                // ClientArray[i, j] = false; // czy ma aktualizowac clientarray? czarek nie
            }
            else
            {
                getMyPicbox(i, j).BackgroundImage = imgMissed;
            }

            ItIsMyTurn = !ItIsMyTurn;
            if (ItIsMyTurn)
            {
                GameUpdateNotification("It is your turn");
            }
            else
            {
                GameUpdateNotification("It opponent turn");
            }
        }
        public void ShootResult(int i, int j, bool res)//client action Czarek to jest wysyłane do klient po wykonaniu strzału
        {
            if (FormMain.MainGlobals.gamemode == "Host")
            {
               // SendDataAsServer("youshooted;" + i + ";" + j + ";" + res);
            }
        }
        public void clientshootedyou(int i, int j)//host action
        {
            //Czarek ma sie zmienic minuaturka ze wzgl na res
            //bool res = false;
            bool res;
            res = HostArray[i, j];
            if (res)
            {
                getMyPicbox(i, j).BackgroundImage = imgX;
                // HostArray[i, j] = false;// czy ma sie tu updateowac? nie
            }
            else
            {
                getMyPicbox(i, j).BackgroundImage = imgMissed;
            }

            //ustal resut
            ShootResult(i, j, res);//wyslij wynik strzalu do klienta
            ItIsMyTurn = !ItIsMyTurn;
            if (ItIsMyTurn)
            {
                GameUpdateNotification("It is your turn");
            }
            else
            {
                GameUpdateNotification("It opponent turn");
            }
        }


        public void sett()
        {
            AI.Bot_Game_plan.shooted = false;
            AI.Bot_Game_plan.second_shooted = false;
            AI.Bot_Game_plan.left_checked = false;
            AI.Bot_Game_plan.right_checked = false;
            AI.Bot_Game_plan.up_checked = false;
            AI.Bot_Game_plan.up_up_checked = false;
            AI.Bot_Game_plan.up_down_checked = false;
            AI.Bot_Game_plan.down_down_checked = false;
            AI.Bot_Game_plan.down_up_checked = false;
            AI.Bot_Game_plan.down_checked = false;
            AI.Bot_Game_plan.left_left_checked = false;
            AI.Bot_Game_plan.right_right_checked = false;
            AI.Bot_Game_plan.left_right_checked = false;
            AI.Bot_Game_plan.right_left_checked = false;
            AI.Bot_Game_plan.right_shooted = false;
            AI.Bot_Game_plan.left_shooted = false;
            AI.Bot_Game_plan.up_shooted = false;
            AI.Bot_Game_plan.down_shooted = false;
            AI.Bot_Game_plan.third_shooted = false;
            AI.Bot_Game_plan.fourth_shot_right_right_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_right_right_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_right_left_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_left_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_left_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_right_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_up_up_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_up_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_down_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_down_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_down_up_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_down_up_checked = false;
        }
        List<String> lista = new List<String>();

        private void generate()
        {
            for (int i = 0; i <10; i++)
            {
                for (int j=0; j<10; j++)
                {
                    lista.Add(i.ToString() + ';' + j.ToString());
                }
            }
        }
        private int[] losuj()
        {
            Random random = new Random();
            int num = random.Next(0, lista.Count);
            string remember = lista[num];
            string[] a = remember.Split(';');
            int[] b = new int[a.Length];
            b[0] = int.Parse(a[0]);
            b[1] = int.Parse(a[1]);
            lista.RemoveAt(num);
            return b;
        }
        private int zwrocDlugosc()
        {
            return lista.Count;
        }
        public bool znajdzIWykasuj(string szukana)
        {
            int index = lista.IndexOf(szukana);
            if(index >= 0)
            {
                lista.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }
        private void GameUpdateNotification(string text)
        {
            if (GameNotify.InvokeRequired)
            {
                GameNotify.Invoke(new Action<string>(GameUpdateNotification), text);
                return;
            }
            GameNotify.Text = text;
        }
        private void OnHostHitOrMissClient(int x, int y, bool res)//host tutaj gdy host wykona strzal to od razu otrzymuje rezultat
        {
            //Czarek zaaktualizuj plansze
            if (res)
            {
                getOpponentPicbox(x, y).BackgroundImage = imgX;
                ++shipsShot;
                if (shipsShot >= 20)
                {
                    this.Invoke(new Action(() => { cleanupAfterGame(); }));
                    IWon();
                }
                // czy aktualizowac tu ClientArray?
                // ClientArray[i, j] = false;
            }
            else
            {
                getOpponentPicbox(x, y).BackgroundImage = imgMissed;
            }
        }
        bool[,] clickedArr = new bool[10, 10];


        private void usun(string szukana)
        {
            int index = lista.IndexOf(szukana);
            lista.RemoveAt(index);
        }

        private void Shoot(object sender, EventArgs z)
        { //client and host action
          //int x = e.Column, y = e.Row;


            // 
            PictureBox clickedButton = (PictureBox)sender;
            int y = tableLayoutPanel19.GetColumn(clickedButton) - 1;
            int x = tableLayoutPanel19.GetRow(clickedButton) - 1;



            if (clickedArr[x, y])
            {



                ((FormMain)Parent).MSG("pick another cell.");
                return;
            }


            clickedArr[x, y] = true;


            bool res;
            res = ClientArray[x, y];

            OnHostHitOrMissClient(x, y, res);//aktualizacja planszy 
                                             //bot

            string imagePath;
        poczatek:
          
            



            Random random = new Random();
            //int x_bot = random.Next(0, 9);
            // int y_bot = random.Next(0, 9);
            int index = random.Next(0,lista.Count);
            string pozycja = (string)lista[index];
            
            int x_bot = int.Parse(pozycja.Split(';')[0]);
            int y_bot = int.Parse(pozycja.Split(';')[1]);
           // string cos = x_bot.ToString() + ";" + y_bot.ToString();

            // //MessageBox.Show("Bot strzela w " + AI.Bot_Game_plan.last_good_shot_x.ToString() + " " + AI.Bot_Game_plan.last_good_shot_x.ToString());
            if (AI.Bot_Game_plan.total_amount_ships > 0) //sprawdza czy sa statki na plansy
            {
               // //MessageBox.Show("sa statki");
                if (AI.Bot_Game_plan.shooted) // sprawdza czy przedtem statek zostal trafiony
                {
                   
                    
                       // //MessageBox.Show(HostArray.GetLength(0).ToString());
                    
                    //MessageBox.Show("poprzedna czesc statku została trafiona");
                    if (AI.Bot_Game_plan.second_shooted) //sprawdza czy druga czesc statku zostala trafiona
                    {
                        //MessageBox.Show("3 shot");
                        if (AI.Bot_Game_plan.third_shooted)
                        {

                            //MessageBox.Show("4 shot");
                            if (AI.Bot_Game_plan.ships_size_four == 0)
                            {
                                //MessageBox.Show("nie ma już statkow 4 więc nie ma sensu sprawdzać");
                                AI.Bot_Game_plan.ships_size_three--;
                                sett();
                                goto poczatek;
                                
                            }
                            
                            if (AI.Bot_Game_plan.right_right_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_right = AI.Bot_Game_plan.fourth_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_left = AI.Bot_Game_plan.last_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_right++;
                                AI.Bot_Game_plan.fourth_good_shot_y_left--;
                                if (AI.Bot_Game_plan.fourth_shot_right_right_right_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x,AI.Bot_Game_plan.fourth_good_shot_y_right]==false )
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);






                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_right;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz (right right right) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //MessageBox.Show("bot nie trafił 4 strzału right right right" + AI.Bot_Game_plan.fourth_good_shot_y_right + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.fourth_shot_right_right_right_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right).BackgroundImage = imgMissed;


                                    }
                                }
                                else if (AI.Bot_Game_plan.fourth_shot_right_right_left_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_left;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz (right right right) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////
                                        sett();

                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //MessageBox.Show("bot nie trafił 4 strzału right right left " + AI.Bot_Game_plan.fourth_good_shot_y_left + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.fourth_shot_right_right_left_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left).BackgroundImage = imgMissed;


                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }
                               
                            }
                            else if (AI.Bot_Game_plan.right_left_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_left = AI.Bot_Game_plan.fourth_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_left--;
                              if (AI.Bot_Game_plan.fourth_shot_right_left_left_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_left;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz (right left left ) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //MessageBox.Show("nie trafiłeś 4 czesci right left left" + AI.Bot_Game_plan.fourth_good_shot_y_left + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.fourth_shot_right_left_left_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left).BackgroundImage = imgMissed;

                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }



                            }
                            else if (AI.Bot_Game_plan.left_left_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_left = AI.Bot_Game_plan.fourth_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_right = AI.Bot_Game_plan.last_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y_left--;
                                AI.Bot_Game_plan.fourth_good_shot_y_right++;
                                if (AI.Bot_Game_plan.fourth_shot_left_left_left_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_left;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz (left left left ) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y_left + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //MessageBox.Show("nie trafiłeś 4 strzału left left left" + AI.Bot_Game_plan.fourth_good_shot_y_left + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.fourth_shot_right_left_left_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_left).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.fourth_shot_left_left_right_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_right;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz left left right) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału left left right" + AI.Bot_Game_plan.fourth_good_shot_y_right + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.fourth_shot_left_left_right_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }

                            }
                            else if (AI.Bot_Game_plan.left_right_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_y_right = AI.Bot_Game_plan.third_good_shot_y;
                                if (AI.Bot_Game_plan.fourth_shot_left_right_right_checked == false && AI.Bot_Game_plan.fourth_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.fourth_good_shot_y_right;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz left right right) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x + ";" + AI.Bot_Game_plan.fourth_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału left left right" + AI.Bot_Game_plan.fourth_good_shot_y_right + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.fourth_shot_left_right_right_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y_right).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }
                            }
                            else if (AI.Bot_Game_plan.up_up_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_x_up = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_down = AI.Bot_Game_plan.last_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_up--;

                                AI.Bot_Game_plan.fourth_good_shot_x_down++;
                                
                                if (AI.Bot_Game_plan.fourth_shot_up_up_up_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_up >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_up;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz up up up) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //MessageBox.Show("nie trafiłeś 4 strzału up up up" + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_up);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_up_up_up_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.fourth_shot_up_up_down_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.ships_size_four--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_down;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz up up down) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x + "ostatni stał   " + AI.Bot_Game_plan.last_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału up up down" + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_down);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_up_up_down_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                 //  //MessageBox.Show( "ostatni strzał w dół  "+ AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_down);

                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }
                            }
                            else if (AI.Bot_Game_plan.up_down_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_x_down = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_down++;
                                if (AI.Bot_Game_plan.fourth_shot_up_down_down_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_down;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz up down down) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////
                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału up down down" + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_down);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_up_down_down_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }

                            }
                            else if (AI.Bot_Game_plan.down_down_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_x_up = AI.Bot_Game_plan.last_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_down = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_up--;
                                AI.Bot_Game_plan.fourth_good_shot_x_down++;
                                if (AI.Bot_Game_plan.fourth_shot_down_down_down_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_down;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz down down down) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_down + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału down down down" + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_down);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_down_down_down_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_down, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.fourth_shot_down_down_up_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_up >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_up;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz down down up) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////

                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału down down up" + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x_up);
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_down_down_up_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    goto poczatek;
                                }
                            }
                            else if (AI.Bot_Game_plan.down_up_shooted)
                            {
                                AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_y = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.fourth_good_shot_x_up = AI.Bot_Game_plan.third_good_shot_x;
                                AI.Bot_Game_plan.fourth_good_shot_x_up--;
                                if (AI.Bot_Game_plan.fourth_shot_down_up_up_checked == false && AI.Bot_Game_plan.fourth_good_shot_x_up >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] == false)
                                {
                                    if (HostArray[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y])
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.fourth_good_shot_x = AI.Bot_Game_plan.fourth_good_shot_x_up;
                                        //MessageBox.Show("Bot poprawnie trafił 4 raz down up up) i zatopił statek " + AI.Bot_Game_plan.fourth_good_shot_y + " " + AI.Bot_Game_plan.fourth_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgX;
                                        //////////////////////////////////////////////////////////////////
                                        sett();
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.fourth_good_shot_x_up + ";" + AI.Bot_Game_plan.fourth_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("nie trafiłeś 4 strzału down up up");
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y] = true;
                                        AI.Bot_Game_plan.fourth_shot_down_up_up_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.fourth_good_shot_x_up, AI.Bot_Game_plan.fourth_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("Zatopiono statek o wielkości 3");
                                    AI.Bot_Game_plan.ships_size_three--;
                                    sett();
                                    AI.Bot_Game_plan.ships_size_three--;
                                    goto poczatek;
                                }



                            }
                            /*
                            //MessageBox.Show("dochodzimy do 4 strzału (nie zrobione)");

                            AI.Bot_Game_plan.shooted = false;
                            AI.Bot_Game_plan.second_shooted = false;
                            AI.Bot_Game_plan.ships_sieze_two--;
                            AI.Bot_Game_plan.left_checked = false;
                            AI.Bot_Game_plan.right_checked = false;
                            AI.Bot_Game_plan.up_checked = false;
                            AI.Bot_Game_plan.up_up_checked = false;
                            AI.Bot_Game_plan.up_down_checked = false;
                            AI.Bot_Game_plan.down_down_checked = false;
                            AI.Bot_Game_plan.down_up_checked = false;
                            AI.Bot_Game_plan.down_checked = false;
                            AI.Bot_Game_plan.left_left_checked = false;
                            AI.Bot_Game_plan.right_right_checked = false;
                            AI.Bot_Game_plan.left_right_checked = false;
                            AI.Bot_Game_plan.right_left_checked = false;
                            AI.Bot_Game_plan.right_shooted = false;
                            AI.Bot_Game_plan.left_shooted = false;
                            AI.Bot_Game_plan.up_shooted = false;
                            AI.Bot_Game_plan.down_shooted = false;
                            AI.Bot_Game_plan.third_shooted = false;

                            goto poczatek;
                            */
                        }
                        else
                        {
                            if (AI.Bot_Game_plan.right_shooted)
                            {
                                
                                AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_right = AI.Bot_Game_plan.third_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_left = AI.Bot_Game_plan.last_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_left--;
                                AI.Bot_Game_plan.third_good_shot_y_right++;
                                //MessageBox.Show("działamy na right shooted koordynaty: " + AI.Bot_Game_plan.third_good_shot_y_right + " " + AI.Bot_Game_plan.third_good_shot_x);

                                if (AI.Bot_Game_plan.right_right_checked == false && AI.Bot_Game_plan.third_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x,AI.Bot_Game_plan.third_good_shot_y_right] == false)
                                {
                                    //MessageBox.Show("nie oddano jeszcze stału na prawo od ostatniego strzału (3shot)");
                                     if (HostArray[AI.Bot_Game_plan.third_good_shot_x,AI.Bot_Game_plan.third_good_shot_y_right] == true) //Wykonuje się w momencie kiedy na prawo od 2 dobrego strzału znajduje się 3 część statku
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.right_right_checked = true;
                                        AI.Bot_Game_plan.right_right_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.third_good_shot_y_right;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (right right) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        

                                    }
                                    else  //nie trafił na prawo od drugie strzału (
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (right right)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.right_right_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.right_left_checked == false && AI.Bot_Game_plan.third_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x,AI.Bot_Game_plan.third_good_shot_y_left] == false ) 
                                {
                                    //MessageBox.Show("nie oddano jeszcze strzału w lewo od ostatniego strzału");
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] == true) // sprawdza czy 3 część statku nie znajduje się na lewo od pierwszego strzału
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.right_left_checked = true;
                                        AI.Bot_Game_plan.right_left_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.third_good_shot_y_left;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (right left) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (right left)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.right_left_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("został zatopiony statek o wielkości 2");
                                    sett();
                                    AI.Bot_Game_plan.ships_sieze_two--;


                                    goto poczatek;
                                    
                                }
                                
                                
                            }

                            
                            else if (AI.Bot_Game_plan.left_shooted)
                            {
                                
                                AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_left = AI.Bot_Game_plan.second_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_right = AI.Bot_Game_plan.last_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_y_left--;
                                AI.Bot_Game_plan.third_good_shot_y_right++;
                                //MessageBox.Show("działamy na left shooted koordynaty (3shot) : " + AI.Bot_Game_plan.third_good_shot_y_left + " " + AI.Bot_Game_plan.third_good_shot_x);
                                if (AI.Bot_Game_plan.left_left_checked == false && AI.Bot_Game_plan.third_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x,AI.Bot_Game_plan.third_good_shot_y_left] == false )
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.left_left_checked = true;
                                        AI.Bot_Game_plan.left_left_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.third_good_shot_y_left;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (left left) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_left).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (left left)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left] = true;
                                        AI.Bot_Game_plan.left_left_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_left).BackgroundImage = imgMissed;
                                    }

                                }
                                else if (AI.Bot_Game_plan.left_right_checked == false && AI.Bot_Game_plan.third_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] == false )
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.left_right_checked = true;
                                        AI.Bot_Game_plan.left_right_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.third_good_shot_y_right;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (left right) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x + ";" + AI.Bot_Game_plan.third_good_shot_y_right).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (left right)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right] = true;
                                        AI.Bot_Game_plan.left_right_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y_right).BackgroundImage = imgMissed;
                                    }

                                }
                                else
                                {
                                    //MessageBox.Show("został zatopiony statek o wielkości 2");
                                    sett();
                                    AI.Bot_Game_plan.ships_sieze_two--;
                                    goto poczatek;
                                }


                            }
                            else if (AI.Bot_Game_plan.up_shooted)
                            {
                                AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_x_down = AI.Bot_Game_plan.last_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_x_up = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_x_up--;
                                AI.Bot_Game_plan.third_good_shot_x_down++;
                                //MessageBox.Show("działamy na up shooted koordynaty: " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x_up);

                                if (AI.Bot_Game_plan.up_up_checked == false && AI.Bot_Game_plan.third_good_shot_x_up >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up,AI.Bot_Game_plan.third_good_shot_y] == false ) //ZAD2 SPRAWDZIĆ CZEMU ŹLE CZYTA GÓRĘ (WGL PRZEANALIZOWAĆ CZY UP I DOWN CAŁE DOBRZE JEST ZROBIONE)
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] == true) //sprawdza czy komórka nad ostatnią trafioną zawiera 3 część statku
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_up + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.up_up_checked = true;
                                        AI.Bot_Game_plan.up_up_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.third_good_shot_x_up;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (up up) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else //bot nie trafił
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_up + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (up up)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.up_up_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.up_down_checked == false && AI.Bot_Game_plan.third_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] == false )
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] == true) //sprawdz czy komórka pod ostatnią trafioną zawiera 3 część statku
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_down + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.up_down_checked = true;
                                        AI.Bot_Game_plan.up_down_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.third_good_shot_x_down;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (up down) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_down + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (up down)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.up_down_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("został zatopiony statek o wielkości 2");
                                    sett();
                                    AI.Bot_Game_plan.ships_sieze_two--;
                                    goto poczatek;
                                }

                            }
                            else if (AI.Bot_Game_plan.down_shooted)
                            {
                                AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;
                                AI.Bot_Game_plan.third_good_shot_x_down = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_x_up = AI.Bot_Game_plan.last_good_shot_x;
                                AI.Bot_Game_plan.third_good_shot_x_up--;
                                AI.Bot_Game_plan.third_good_shot_x_down++;
                                if (AI.Bot_Game_plan.down_down_checked == false && AI.Bot_Game_plan.third_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] == false )
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] == true) //sprawdz czy komórka pod ostatnią trafioną zawiera 3 część statku
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_down + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.down_down_checked = true;
                                        AI.Bot_Game_plan.down_down_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.third_good_shot_x_down;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (down down) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_down + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (down down)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.down_down_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x_down, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else if (AI.Bot_Game_plan.down_up_checked == false && AI.Bot_Game_plan.third_good_shot_x_up >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] == false ) //ZAD3 NIE WYKONAŁ SIĘ
                                {
                                    if (HostArray[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] == true)
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_up + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        AI.Bot_Game_plan.third_shooted = true;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.total_amount_ships--;
                                        AI.Bot_Game_plan.down_up_checked = true;
                                        AI.Bot_Game_plan.down_up_shooted = true;
                                        AI.Bot_Game_plan.third_good_shot_x = AI.Bot_Game_plan.third_good_shot_x_up;
                                        //MessageBox.Show("Bot poprawnie trafił 3 raz (down up) " + AI.Bot_Game_plan.third_good_shot_y + " " + AI.Bot_Game_plan.third_good_shot_x);
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgX;
                                        
                                    }
                                    else
                                    {
                                        string szukana = (AI.Bot_Game_plan.third_good_shot_x_up + ";" + AI.Bot_Game_plan.third_good_shot_y).ToString();
                                        int indexx = lista.IndexOf(szukana);
                                        //MessageBox.Show("indeks : " + index.ToString() + " liczby " + szukana);
                                        if(indexx >= 0)lista.RemoveAt(indexx);
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        //MessageBox.Show("Bot nie trafił tym razem 3 strzał (down up)");
                                        AI.Bot_Game_plan.third_shooted = false;
                                        AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y] = true;
                                        AI.Bot_Game_plan.down_up_checked = true;
                                        AI.Bot_Game_plan.total_amount_shots++;
                                        getMyPicbox(AI.Bot_Game_plan.third_good_shot_x_up, AI.Bot_Game_plan.third_good_shot_y).BackgroundImage = imgMissed;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show("został zatopiony statek o wielkości 2");
                                    sett();
                                    AI.Bot_Game_plan.ships_sieze_two--;
                                    goto poczatek;

                                }

                            }
                            
                        }

                    }
                    else
                    {
                        AI.Bot_Game_plan.second_good_shot_x = AI.Bot_Game_plan.last_good_shot_x;
                        AI.Bot_Game_plan.second_good_shot_y = AI.Bot_Game_plan.last_good_shot_y;
                        AI.Bot_Game_plan.second_good_shot_y_right = AI.Bot_Game_plan.last_good_shot_y;
                        AI.Bot_Game_plan.second_good_shot_y_left = AI.Bot_Game_plan.last_good_shot_y;
                        AI.Bot_Game_plan.second_good_shot_x_up = AI.Bot_Game_plan.last_good_shot_x;
                        AI.Bot_Game_plan.second_good_shot_x_down = AI.Bot_Game_plan.last_good_shot_x;
                        AI.Bot_Game_plan.second_good_shot_x_down++;
                        AI.Bot_Game_plan.second_good_shot_x_up--;
                        AI.Bot_Game_plan.second_good_shot_y_right++;
                        AI.Bot_Game_plan.second_good_shot_y_left--;
                        //MessageBox.Show(AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);
                        if (AI.Bot_Game_plan.right_checked == false && AI.Bot_Game_plan.second_good_shot_y_right <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right] == false ) //sprawdza czy była już sprawdzana komórka na prawo od strzelonej i czy się mieści w przedziałach (ZAD1 SPRAWDZIĆ JAK WYGLĄDA BOT MOVES W TYM MIEJSCU)
                        {
                            
                          //  AI.Bot_Game_plan.second_good_shot_y++;
                            //MessageBox.Show( "Secxond shot right checked = false: " +AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);

                            if (HostArray[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right] == true) // sprawdza czy komórka na prawo od strzelonej również ma statek
                                {
                                string szukana =(AI.Bot_Game_plan.second_good_shot_x + ";" + AI.Bot_Game_plan.second_good_shot_y_right).ToString();
                                int indexx = lista.IndexOf(szukana);
                                //MessageBox.Show("indeks : " + indexx.ToString() + " liczby " + szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.second_shooted = true;
                                AI.Bot_Game_plan.total_amount_ships--;
                                AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right] = true;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right] = true;
                                AI.Bot_Game_plan.right_checked = true;
                                AI.Bot_Game_plan.right_shooted = true;
                                AI.Bot_Game_plan.second_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.second_good_shot_y = AI.Bot_Game_plan.second_good_shot_y_right;
                                //MessageBox.Show("bot trafił 2 czesc right " + AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);
                                AI.Bot_Game_plan.total_amount_shots++;
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right).BackgroundImage = imgX;
                              


                                }
                                else
                                {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x + ";" + AI.Bot_Game_plan.second_good_shot_y_right).ToString();
                                int indexx = lista.IndexOf(szukana);
                                //MessageBox.Show("indeks : " + indexx.ToString() + "liczby" + lista.IndexOf(szukana));
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right] = true;
                              
                                AI.Bot_Game_plan.right_checked = true;
                                //MessageBox.Show("nie trafiles 2 czesc right " + AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);
                                //MessageBox.Show("koordynaty na lewo (right)" + AI.Bot_Game_plan.second_good_shot_y_left + " " + AI.Bot_Game_plan.second_good_shot_x);
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_right).BackgroundImage = imgMissed;
                                }

                            }
                        else if (AI.Bot_Game_plan.left_checked == false && AI.Bot_Game_plan.second_good_shot_y_left >= 0 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left] == false )//sprawdza czy była już sprawdzana komórka na lewo od strzelonej i czy się mieści w przedziałach
                        {
                            //MessageBox.Show("Secxond shot left checked = false: " + AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);

                            // AI.Bot_Game_plan.second_good_shot_y--;
                            if (HostArray[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left] == true) // sprawdza czy komórka na lewo od strzelonej również ma statek
                            {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x + ";" + AI.Bot_Game_plan.second_good_shot_y_left).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.second_shooted = true;
                                AI.Bot_Game_plan.total_amount_ships--;
                                AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left] = true;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left] = true;
                                AI.Bot_Game_plan.left_checked = true;
                                //MessageBox.Show("bot trafił 2 czesc left " + AI.Bot_Game_plan.second_good_shot_y_left + " " + AI.Bot_Game_plan.second_good_shot_x);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.left_shooted = true;
                                AI.Bot_Game_plan.second_good_shot_x = AI.Bot_Game_plan.second_good_shot_x;
                                AI.Bot_Game_plan.second_good_shot_y = AI.Bot_Game_plan.second_good_shot_y_left;
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left).BackgroundImage = imgX;
                            

                                
                                }
                                else
                                {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x + ";" + AI.Bot_Game_plan.second_good_shot_y_left).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left] = true;
                                AI.Bot_Game_plan.left_checked = true;
                                //  imagePath = @"..\..\resources\textures\ui\not_allowed.png";
                                //MessageBox.Show("nie trafiles 2 czesc left " + AI.Bot_Game_plan.second_good_shot_x + " " + AI.Bot_Game_plan.second_good_shot_y_left);
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x, AI.Bot_Game_plan.second_good_shot_y_left).BackgroundImage = imgMissed;
                                }
                            }
                        else if (AI.Bot_Game_plan.up_checked == false && AI.Bot_Game_plan.second_good_shot_x_up >= 0 &&  AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y] == false )//sprawdza czy była już ustrzelona komórka nad trafioną albo czy się mieści w macierzy
                            {
                            //MessageBox.Show("Secxond shot up checked = false: " + AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);
                            // AI.Bot_Game_plan.second_good_shot_x++;
                            if (HostArray[AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y] == true ) //Sprawdza czy nad komórką strzeloną znajduje się statek ZAD4 DO NAPRAWY
                            {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x_up + ";" + AI.Bot_Game_plan.second_good_shot_y).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.second_shooted = true;
                                AI.Bot_Game_plan.total_amount_ships--;
                                AI.Bot_Game_plan.up_checked = true;
                                AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y] = true;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y] = true;
                                //MessageBox.Show("bot trafił 2 czesc up " + AI.Bot_Game_plan.second_good_shot_y + " " + AI.Bot_Game_plan.second_good_shot_x_up);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.up_shooted = true;
                                AI.Bot_Game_plan.second_good_shot_x = AI.Bot_Game_plan.second_good_shot_x_up;
                                AI.Bot_Game_plan.second_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;
                               

                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y).BackgroundImage = imgX;
                                }
                                else
                                {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x_up + ";" + AI.Bot_Game_plan.second_good_shot_y).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y] = true;
                                //  imagePath = @"..\..\resources\textures\ui\not_allowed.png";
                                AI.Bot_Game_plan.up_checked = true;
                                //MessageBox.Show("nie trafiles 2 czesc up " + AI.Bot_Game_plan.second_good_shot_y + " " + AI.Bot_Game_plan.second_good_shot_x_up);
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x_up, AI.Bot_Game_plan.second_good_shot_y).BackgroundImage = imgMissed;
                                }
                            }
                        else if (AI.Bot_Game_plan.down_checked == false && AI.Bot_Game_plan.second_good_shot_x_down <= 9 && AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y] == false  )//sprawdza czy była już ustrzelona komórka  trafioną albo czy się mieści w macierzy
                        {
                            //MessageBox.Show("Secxond shot down checked = false: " + AI.Bot_Game_plan.second_good_shot_y_right + " " + AI.Bot_Game_plan.second_good_shot_x);
                            //AI.Bot_Game_plan.second_good_shot_x--;
                            if (HostArray[AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y] == true)//Sprawdza czy pod komórką strzeloną znajduje się statek
                            {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x_down + ";" + AI.Bot_Game_plan.second_good_shot_y).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.second_shooted = true;
                                AI.Bot_Game_plan.total_amount_ships--;
                                AI.Bot_Game_plan.down_checked = true;
                                AI.Bot_Game_plan.bot_good_shots_position[AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y] = true;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y] = true;
                                //MessageBox.Show("bot trafił 2 czesc down " + AI.Bot_Game_plan.second_good_shot_y + " " + AI.Bot_Game_plan.second_good_shot_x_down);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.down_shooted = true;
                                AI.Bot_Game_plan.second_good_shot_x = AI.Bot_Game_plan.second_good_shot_x_down;
                                AI.Bot_Game_plan.second_good_shot_y = AI.Bot_Game_plan.second_good_shot_y;

                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y).BackgroundImage = imgX;
                                }
                                else
                                {
                                string szukana = (AI.Bot_Game_plan.second_good_shot_x_down + ";" + AI.Bot_Game_plan.second_good_shot_y).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                AI.Bot_Game_plan.total_amount_shots++;
                                AI.Bot_Game_plan.bot_moves[AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y] = true;
                                //  imagePath = @"..\..\resources\textures\ui\not_allowed.png";
                                AI.Bot_Game_plan.down_checked = true;
                                //MessageBox.Show("nie trafiles 2 czesc down " + AI.Bot_Game_plan.second_good_shot_y + " " + AI.Bot_Game_plan.second_good_shot_x_down);
                                getMyPicbox(AI.Bot_Game_plan.second_good_shot_x_down, AI.Bot_Game_plan.second_good_shot_y).BackgroundImage = imgMissed;
                                }
                         
                            

                            
                        }
                        else // został zatopiony statek o wielkości równej 1 
                        {
                            //MessageBox.Show("został zatopiony statek o wielkości 1");
                            sett();

                            goto poczatek;
                        }
                    }
                }
                else
                {
                    
                    ////MessageBox.Show(AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot + 1].ToString());
                   // //MessageBox.Show(AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot - 1].ToString());
                 //   //MessageBox.Show(AI.Bot_Game_plan.bot_good_shots_position[x_bot+1, y_bot].ToString());
                 //   //MessageBox.Show(AI.Bot_Game_plan.bot_good_shots_position[x_bot-1, y_bot].ToString());
                    if (AI.Bot_Game_plan.bot_moves[x_bot, y_bot] == false /*&& x_bot + 1 <= 10 && x_bot - 1 >= -1 && y_bot - 1 >= -1 && y_bot + 1 <= 10 && (AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot + 1] == false && AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot - 1] == false && AI.Bot_Game_plan.bot_good_shots_position[x_bot+1, y_bot] == false && AI.Bot_Game_plan.bot_good_shots_position[x_bot - 1, y_bot] == false )*/) // sprawdza czy pozycja nie została już wczesniej wylosowana
                    {

                        // //MessageBox.Show("pozycja jeszcze nie wylosowana");
                        if (x_bot != 0 && x_bot !=9 && y_bot != 0 && y_bot != 9)
                        {
                            if (AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot + 1] == true && AI.Bot_Game_plan.bot_moves[x_bot, y_bot] == false)
                            {
                               
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                //MessageBox.Show("nie postawi tutaj" + x_bot + y_bot);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot - 1] == true && AI.Bot_Game_plan.bot_moves[x_bot, y_bot] == false)
                            {
                                
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                //MessageBox.Show("nie postawi tutaj" + x_bot + y_bot);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot + 1, y_bot] == true && AI.Bot_Game_plan.bot_moves[x_bot, y_bot] == false)
                            {
                                
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                //MessageBox.Show("nie postawi tutaj" + x_bot + y_bot);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot - 1, y_bot] == true && AI.Bot_Game_plan.bot_moves[x_bot , y_bot] == false)
                            {
                                
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if(indexx >= 0)lista.RemoveAt(indexx);
                                //MessageBox.Show("nie postawi tutaj" + x_bot + y_bot);
                                goto poczatek;
                            }
                        }




                        if (HostArray[x_bot, y_bot] == true)
                        {
                            lista.RemoveAt(index);

                            AI.Bot_Game_plan.total_amount_shots++;
                            AI.Bot_Game_plan.amount_bot_good_shots++;
                            AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot] = true;
                            AI.Bot_Game_plan.bot_moves[x_bot, y_bot] = true;
                            AI.Bot_Game_plan.total_amount_ships--;
                            AI.Bot_Game_plan.last_good_shot_x = x_bot;
                            AI.Bot_Game_plan.last_good_shot_y = y_bot;
                            //MessageBox.Show("bot trafił 1 czesc " + y_bot + " " + x_bot);
                            AI.Bot_Game_plan.shooted = true;
                            getMyPicbox(x_bot, y_bot).BackgroundImage = imgX;

                        }
                        else
                        {
                            lista.RemoveAt(index);
                            AI.Bot_Game_plan.total_amount_shots++;
                            AI.Bot_Game_plan.bot_moves[x_bot, y_bot] = true;
                            //  imagePath = @"..\..\resources\textures\ui\not_allowed.png";
                            //MessageBox.Show("nie trafiles " + y_bot + " " + x_bot);
                            AI.Bot_Game_plan.shooted = false;
                            getMyPicbox(x_bot, y_bot).BackgroundImage = imgMissed;

                        }
                    }

                    else 
                    {
                        /*
                        if (y_bot + 1 <= 9 && y_bot -1 >= 0 && x_bot +1 >=0 && x_bot+1 <= 9)
                        {
                            if (AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot + 1] == true)
                            {
                                y_bot = y_bot + 1;
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if (indexx >= 0) lista.RemoveAt(indexx);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot - 1] == true)
                            {
                                y_bot = y_bot - 1;
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if (indexx >= 0) lista.RemoveAt(indexx);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot + 1, y_bot] == true)
                            {
                                x_bot = x_bot + 1;
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if (indexx >= 0) lista.RemoveAt(indexx);
                                goto poczatek;
                            }
                            else if (AI.Bot_Game_plan.bot_good_shots_position[x_bot - 1, y_bot] == true)
                            {
                                x_bot = x_bot - 1;
                                string szukana = (x_bot + ";" + y_bot).ToString();
                                int indexx = lista.IndexOf(szukana);
                                if (indexx >= 0) lista.RemoveAt(indexx);
                                goto poczatek;
                            }
                            // //MessageBox.Show("liczba sie powtorzyla");
                        
                        }
                        else
                        {
                            goto poczatek;
                        }
                        */
                    }
                    // Image image = Image.FromFile(imagePath);
                }
            }
            else
            {
                //MessageBox.Show("koniec gry
                sett();
                AI.Bot_Game_plan.ships_size_one = 4;
                AI.Bot_Game_plan.ships_sieze_two = 3;
                AI.Bot_Game_plan.ships_size_three = 2;
                AI.Bot_Game_plan.ships_size_four = 1;
                AI.Bot_Game_plan.total_amount_ships = 20;
                BotWon();
                cleanupAfterGame();
                resetEnemyShipFields();
                
                
            }

        }
       
        private void YESBTNC(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { whenGameEndsSuccessfully(true); }));
            this.Invoke(new Action(() => { tableLayoutPanel20.Visible = false; }));

        }

        private void NOBTNC(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { whenGameEndsSuccessfully(false); }));
            this.Invoke(new Action(() => { tableLayoutPanel20.Visible = false; }));
           
            ((FormMain)Parent).MSG("game ended");
           // FormMain myParent = (FormMain)this.Parent;
            //SwitchTab(0);
          //  myParent.zmienwidok("Bot", "PlayView");
        //    SwitchTab(0);


        }
        #region min plu labels
        private void label1min1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeljeden.Text);
            c--;
            if (c < 0 || c > 4) return;
            labeljeden.Text = c.ToString();
        }

        private void label1plu1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeljeden.Text);
            c++;
            if (c < 0 || c > 4) return;
            labeljeden.Text = c.ToString();
        }

        private void label2min1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeldwa.Text);
            c--;
            if (c < 0 || c > 3) return;
            labeldwa.Text = c.ToString();
        }

        private void label2plu1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeldwa.Text);
            c++;
            if (c < 0 || c > 3) return;
            labeldwa.Text = c.ToString();
        }

        private void label3min1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeltrzy.Text);
            c--;
            if (c < 0 || c > 2) return;
            labeltrzy.Text = c.ToString();
        }

        private void label3plu1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labeltrzy.Text);
            c++;
            if (c < 0 || c > 2) return;
            labeltrzy.Text = c.ToString();
        }

        private void label4min1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labelcztery.Text);
            c--;
            if (c < 0 || c > 1) return;
            labelcztery.Text = c.ToString();
        }

        private void label4plu1_Click(object sender, EventArgs e)
        {
            int c = int.Parse(labelcztery.Text);
            c++;
            if (c < 0 || c > 1) return;
            labelcztery.Text = c.ToString();
        }
        #endregion
        private void LobbyBackClicked(object sender, EventArgs e)
        {
            //FormMain myParent = (FormMain)Parent;
            reset();
            this.Invoke(new Action(() => {
                FormMain myParent = (FormMain)Parent;
                myParent.zmienwidok("Bot", "PlayView"); 
            }));
            // myParent.zmienwidok("Bot", "MainView");
        }
        Stopwatch czasGry = new Stopwatch();


        private void OpponentWon()
        {
            reset();
                this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));
                ((FormMain)Parent).MSG("Opponent won");
                GameNotify.Text = "opponent won";
             
                //runTasks = false;
                // FormMain myparent = (FormMain)Parent;
                // myparent.zmienwidok("GameLobbyView", "PlayView");
                //tak samo jak nizej
  

        }

        AI ai = new AI();
        private void reset()
        {
            AI.Bot_Game_plan.total_amount_ships = 20;
            AI.Bot_Game_plan.ships_destroyed = 0;
            AI.Bot_Game_plan.ships_destroyed_position = new int[10, 10];
            AI.Bot_Game_plan.bot_ships_position = new bool[10, 10];
            AI.Bot_Game_plan.ships_size_one = 4;
            AI.Bot_Game_plan.ships_sieze_two = 3;
            AI.Bot_Game_plan.ships_size_three = 2;
            AI.Bot_Game_plan.ships_size_four = 1;
            AI.Bot_Game_plan.up_checked = false;
            AI.Bot_Game_plan.right_checked = false;
            AI.Bot_Game_plan.right_right_checked = false;

            AI.Bot_Game_plan.right_left_checked = false;
            AI.Bot_Game_plan.left_left_checked = false;
            AI.Bot_Game_plan.left_right_checked = false;
            AI.Bot_Game_plan.up_up_checked = false;
            AI.Bot_Game_plan.up_down_checked = false;
            AI.Bot_Game_plan.down_checked = false;
            AI.Bot_Game_plan.down_down_checked = false;
            AI.Bot_Game_plan.down_up_checked = false;
            AI.Bot_Game_plan.left_checked = false;
            AI.Bot_Game_plan.bot_moves = new bool[100, 100];
            AI.Bot_Game_plan.bot_good_shots_position = new bool[100, 100];
            AI.Bot_Game_plan.amount_bot_good_shots = 0;
            AI.Bot_Game_plan.total_amount_shots = 0;
            AI.Bot_Game_plan.shooted = false;
            AI.Bot_Game_plan.second_shooted = false;
            AI.Bot_Game_plan.third_shooted = false;
            AI.Bot_Game_plan.fourth_shooted = false;
            AI.Bot_Game_plan.last_good_shot_x = 0;
            AI.Bot_Game_plan.last_good_shot_y = 0;
            AI.Bot_Game_plan.second_good_shot_y_right = 0;
            AI.Bot_Game_plan.second_good_shot_y_left = 0;
            AI.Bot_Game_plan.second_good_shot_y = 0;
            AI.Bot_Game_plan.second_good_shot_x = 0;
            AI.Bot_Game_plan.second_good_shot_x_down = 0;
            AI.Bot_Game_plan.second_good_shot_x_up = 0;
            AI.Bot_Game_plan.third_good_shot_x = 0;
            AI.Bot_Game_plan.third_good_shot_y = 0;
            AI.Bot_Game_plan.third_good_shot_x_down = 0;
            AI.Bot_Game_plan.third_good_shot_x_up = 0;
            AI.Bot_Game_plan.third_good_shot_y_right = 0;
            AI.Bot_Game_plan.right_right_shooted = false;
            AI.Bot_Game_plan.right_left_shooted = false;
            AI.Bot_Game_plan.left_right_shooted = false;
            AI.Bot_Game_plan.left_left_shooted = false;
            AI.Bot_Game_plan.up_up_shooted = false;
            AI.Bot_Game_plan.up_down_shooted = false;
            AI.Bot_Game_plan.down_down_shooted = false;
            AI.Bot_Game_plan.down_up_shooted = false;
            AI.Bot_Game_plan.third_good_shot_y_left = 0;
            AI.Bot_Game_plan.fourth_good_shot_x = 0;
            AI.Bot_Game_plan.fourth_good_shot_x_down = 0;
            AI.Bot_Game_plan.fourth_good_shot_x_up = 0;
            AI.Bot_Game_plan.fourth_good_shot_y = 0;
            AI.Bot_Game_plan.fourth_good_shot_y_right = 0;
            AI.Bot_Game_plan.fourth_good_shot_y_left = 0;
            AI.Bot_Game_plan.right_shooted = false;
            AI.Bot_Game_plan.left_shooted = false;
            AI.Bot_Game_plan.up_shooted = false;
            AI.Bot_Game_plan.down_shooted = false;
            AI.Bot_Game_plan.fourth_shot_right_right_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_right_right_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_right_left_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_left_left_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_left_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_left_right_right_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_up_up_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_up_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_up_down_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_down_up_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_down_down_checked = false;
            AI.Bot_Game_plan.fourth_shot_down_up_up_checked = false;
            shipsShot = 0;
    }
        
        private void IWon()//host action
        {
            reset();
                ((FormMain)Parent).MSG("You won");
                GameNotify.Text = "you won";
                this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));
           // AI.Bot_Game_plan.re
          //      Zmien AI na ai tylko musisz to zrobic 
          //    AI ai = new AI()
          //  pozniej
          // ai = new AI();
           




        }
        private void BotWon()
        {
            reset();
            ((FormMain)Parent).MSG("Bot won");
            GameNotify.Text = "Bot won";
            this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));
           
        }

        
        private void CheckIfBothReady()
        {


           // Random random = new Random();
           // int x_bot = random.Next(0, 9);
           // int y_bot = random.Next(0, 9);

            generate();
            Random rnd = new Random();
            int index = rnd.Next(lista.Count);
            string pozycja = (string)lista[index];
            lista.RemoveAt(index);
            int x_bot = int.Parse(pozycja.Split(';')[0]);
            int y_bot = int.Parse(pozycja.Split(';')[1]);
            

            SwitchTab(1);
                setupGame();
            
            
            HostArray = shipCords;

            bool ktozaczyna = extras.Draw();
            if (ktozaczyna)
            {
                ItIsMyTurn = true;
                
                //ja
            }
            else
            {
               
                //MessageBox.Show("Bot zaczyna" + "strzela w " + y_bot.ToString() + " " + x_bot.ToString());
               // Control cellControll = tableLayoutPanel12.GetControlFromPosition(x_bot, y_bot);
                string m = "";
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (HostArray[i, j])
                        {
                            m += "# ";
                        }
                        else
                        {
                            m += "- ";
                        }
                    }
                    m += "\n";
                }
                //MessageBox.Show("moj arrr\n" + m);
               
                
                  if (HostArray[x_bot,y_bot] == true)
                    {
                        AI.Bot_Game_plan.total_amount_shots++;
                    
                    AI.Bot_Game_plan.amount_bot_good_shots++;
                        AI.Bot_Game_plan.bot_good_shots_position[x_bot, y_bot] = true;
                        AI.Bot_Game_plan.bot_moves[x_bot, y_bot] = true;
                        //MessageBox.Show("trafiłes");
                        AI.Bot_Game_plan.shooted = true;
                        AI.Bot_Game_plan.last_good_shot_x = x_bot;
                        AI.Bot_Game_plan.last_good_shot_y = y_bot;
                        getMyPicbox(x_bot, y_bot).BackgroundImage = imgX;
                        AI.Bot_Game_plan.total_amount_ships--;

                    }
                    else
                    {
                        AI.Bot_Game_plan.total_amount_shots++;
                        AI.Bot_Game_plan.bot_moves[x_bot, y_bot] = true;
                        getMyPicbox(x_bot, y_bot).BackgroundImage = imgMissed;
                    }
                    
                
                
               
                
                


                //bot pierwszy strzal ma sie tutaj wykonac
            }
                           
        }
        private static int GenerateRandomNumber(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue + 1);
        }

        private void autoset_Click(object sender, EventArgs e)
        {
            if (shipsPlaced == 0)
            {
                AutoSet();
                //8 9 36 37
                label8.Text = "0x";
                label9.Text = "0x";
                label36.Text = "0x";
                label37.Text = "0x";
            }
            else
            {
                ((FormMain)Parent).MSG("you cannot use this option beacuse you placed some ships. " + shipsPlaced.ToString());
            }
        }
        private void AutoSet()
        {
            ShipOrientation shipOrientation = ShipOrientation.Horizontal;
            shipCords = new bool[10, 10];
            shipsOfNSizePlaced = new int[] { 4, 3, 2, 1 };
            int currentShipSize;
            string shipfolder = "";
            int GenerateRandomNumber(int minValue, int maxValue)
            {
                Random random = new Random(); return random.Next(minValue, maxValue + 1);
            }
            bool placeships(int line, int row)
            {
                if (shipOrientation == ShipOrientation.Vertical)
                {
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        // najpierw sprawdz czy nie wyjdzie poza boundsy, potem czy nie koliduje z innym statkiem
                        int l = line + i;

                        if (!isInBounds(l) || shipCords[l, row])
                        {
                            return false;
                        }

                        // sprawdz czy na polach dookola (sciany dluzsze) nie ma statku

                        int row_to_check = row + 1;
                        if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na prawo od statku
                        {
                            return false;
                        }

                        row_to_check = row - 1;
                        if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na lewo od statku
                        {
                            return false;
                        }
                    }

                    // sprawdz dwa pola - pod statkiem i nad

                    int line_to_check = line + currentShipSize;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole pod statkiem
                    {
                        return false;
                    }

                    line_to_check = line - 1;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole nad statkiem
                    {
                        return false;
                    }

                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int l = line + i;
                        shipCords[l, row] = true;
                        getPicBox(l, row).BackgroundImage =
                         Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\" + shipfolder + @"\" + (i + 1).ToString() + "of" + currentShipSize.ToString() + "-v.png"));
                    }
                }
                else if (shipOrientation == ShipOrientation.Horizontal)
                {
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int r = row + i;

                        if (!isInBounds(r) || shipCords[line, r])
                        {
                            return false;
                        }

                        int line_to_check = line + 1;
                        if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola pod statkiem
                        {
                            return false;
                        }

                        line_to_check = line - 1;
                        if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola nad
                        {
                            return false;
                        }
                    }

                    int row_to_check = row + currentShipSize;
                    if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na prawo od statku
                    {
                        return false;

                    }

                    row_to_check = row - 1;
                    if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na lewo
                    {

                        return false;
                    }

                    // zaktualizuj matrix i ustaw kolory buttonow
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int r = row + i;
                        shipCords[line, r] = true;

                        getPicBox(line, r).BackgroundImage =
                      Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\" + shipfolder + @"\" + (i + 1).ToString() + "of" + currentShipSize.ToString() + "-h.png"));
                    }
                }
                return true;
            }


            //  Console.WriteLine("hello\n");
            shipOrientation = ShipOrientation.Vertical;
            currentShipSize = 4;
            shipfolder = "four";
            while (true)
            {
                if (GenerateRandomNumber(0, 1) == 0)
                {
                    shipOrientation = ShipOrientation.Horizontal;
                }
                else
                {
                    shipOrientation = ShipOrientation.Vertical;
                }
                if (placeships(GenerateRandomNumber(0, 9), GenerateRandomNumber(0, 9)))
                {
                    shipsPlaced++;
                }

                if (shipsPlaced == 1)
                {
                    shipfolder = "three";
                    currentShipSize = 3;
                    Bot_GameLobbyView.ships_size_three--;
                    Bot_Game_plan.ships_size_three++;

                }
                if (shipsPlaced == 3)
                {
                    shipfolder = "two";
                    currentShipSize = 2;
                    Bot_GameLobbyView.ships_sieze_two--;
                    Bot_Game_plan.ships_sieze_two++;
                }
                if (shipsPlaced == 6)
                {
                    shipfolder = "one";
                    currentShipSize = 1;
                    Bot_GameLobbyView.ships_size_one--;
                    Bot_Game_plan.ships_size_one++;
                }

                if (shipsPlaced == 10) break;


            }
            HostArray = shipCords;
            WhenAllShipsPlaced();
            setupGame();
        }

        private void HostIsReady()//client action
        {
            if (HostReady)
            {
                HostReady = false;
                LobbyUpdateOpponentStatusk("connected but not ready");
            }
            else
            {

                HostReady = true;
                LobbyUpdateOpponentStatusk("connected and ready");
            }
        }
    }

}


