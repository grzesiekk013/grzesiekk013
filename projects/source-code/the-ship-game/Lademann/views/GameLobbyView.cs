using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Lademann.views.AI;

namespace Lademann.views
{
	public partial class GameLobbyView : UserControl
	{
		public GameLobbyView()
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
		#region gamelobby
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
				setupGame();
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

		public static bool ItIsMyTurn = false;

		private void setgameback_Click(object sender, EventArgs e) { }
		private void setgamesubmit_Click(object sender, EventArgs e)
		{
			//connection needed to be checked
			//
			try
			{
				ServerAddress = setGameAddress.Text.Split(':')[0];
				ServerPort = setGameAddress.Text.Split(':')[1];
			}
			catch (Exception)
			{
				((FormMain)Parent).MSG("please enter ip address and port for e.g. localhost:10000"); return;
			}


			if (ServerAddress.Length < 3) { ((FormMain)Parent).MSG("ip address is too short"); return; }
			if (ServerPort.Length < 5) { ((FormMain)Parent).MSG("port is too short"); return; }

			try
			{
				if (conn(ServerAddress))
				{


					try
					{

						InitializeCommunication();


					


					}
					catch (Exception ex)
					{

						((FormMain)Parent).MSG("unable to establish connection " + ex.Message);
						return;
					}
				}
				else
				{
					((FormMain)Parent).MSG("host is down");
				}
			}
			catch (Exception)
			{

			}

			// tabControl1.SelectedIndex = 0;
		}
		public static bool HostReady = false, ClientReady = false;
		private void readybtn_Click(object sender, EventArgs e)
		{

			if (label8.Text == "0x" && label9.Text == "0x" && label37.Text == "0x" && label36.Text == "0x")//
			{
				if (FormMain.MainGlobals.gamemode == "Host")
				{
					if (HostReady)
					{
						HostReady = false;
						LobbyUpdateMeStatus("connected but not ready");
						SendDataAsServer("hostisready;");
						readybtn.Text = "READY";
					}
					else
					{
						HostReady = true;
						LobbyUpdateMeStatus("connected and ready");
						SendDataAsServer("hostisready;");
						readybtn.Text = "NOT READY";
						CheckIfBothReady();
						//Czarek trzeba przepisac arr do HostArr
					}
				}
				else
				{
					if (ClientReady)
					{
						ClientReady = false;
						LobbyUpdateMeStatus("connected but not ready");
						SendDataAsClinet("ready;");
						readybtn.Text = "READY";
					}
					else
					{
						ClientReady = true;
						LobbyUpdateMeStatus("connected and ready");
						SendDataAsClinet("ready;");
						readybtn.Text = "NOT READY";
						string tmp = "";
						string abc = "";
						for (int i = 0; i < 10; i++)//wiersze
						{
							for (int j = 0; j < 10; j++)//kolumny
							{
								if (shipCords[i, j])
								{
									tmp += "1";
									abc += "1";
								}
								else
								{
									tmp += "0";
									abc += "-";
								}

							}
							abc += "\n";
						}
						Console.WriteLine("mymatrix\n" + abc);
						//Czarek trzeba przepsiac arr do myArr i dodac to jako string do sendmatrix \/
						SendDataAsClinet("sendmatrix;" + tmp);
					}
				}
			}
			else
			{
				((FormMain)Parent).MSG("some ships left");
			}
		}

		/// <summary>
		/// //////////////////////////////////////////////////////////////////////////
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		//public static bool runTasks = false;

		public static int listenerPort = 30000;


		public static Ping pingSender = new Ping();
		public static PingOptions options = new PingOptions();
		public static string gamemode;

		public static string HostAddress, HostNickname, HostPort = "", ClientAddress = "", ClientNickname = "", ClientPort = "", ServerAddress = "", ServerPort = "";

		public static bool[,] ClientArray = new bool[10, 10];
		public static bool[,] HostArray = new bool[10, 10];
		private void GameLobbyView_VisibleChanged(object sender, EventArgs e)
		{
			if (FormMain.MainGlobals.nickname.Length < 3) return;
			if (this.Visible)
			{
				cleanupAfterGame();
				// 
				pictureBox279.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png");
				pictureBox278.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\two\full-h.png");
				pictureBox277.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\three\full-h.png");
				pictureBox276.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\four\full-h.png");

				pictureBox5.BackgroundImage = Image.FromFile(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png");
				pictureBox1.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\one\1of1-h.png"));
				pictureBox4.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\two\full-h.png"));
				pictureBox2.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\three\full-h.png"));
				pictureBox3.BackgroundImage = Image.FromFile(String.Format(@"..\..\resources\textures\" + FormMain.MainGlobals.theme + @"\four\full-h.png"));
				gamemode = FormMain.MainGlobals.gamemode;
				if (FormMain.MainGlobals.gamemode == "Host")
				{

					SwitchTab(0);
					((FormMain)Parent).MSG("you're hosting this game. share your ip address and port");
				}
				else
				{
					SwitchTab(2);
					// ((FormMain)Parent).MSG("you need to enter your friend's ip address and port e.g. localhost:1000");
				}
				if (FormMain.MainGlobals.gamemode == "Host")
				{
					InitializeCommunication();
				}


			}
			else
			{
                FormMain.MainGlobals.runTask = false;
                czasGry.Stop();
                int wins = int.Parse(File.ReadAllText("wins"));
                int PlayedBattles = int.Parse(File.ReadAllText("PlayedBattles"));
                int DestroyedShips = int.Parse(File.ReadAllText("DestroyedShips"));
                int TimeInGame = int.Parse(File.ReadAllText("TimeInGame"));
                File.WriteAllText("wins", (wins + 0).ToString());
                File.WriteAllText("PlayedBattles", (PlayedBattles + 1).ToString());
                File.WriteAllText("DestroyedShips", (DestroyedShips + shipsShot).ToString());
                int czas = int.Parse(czasGry.ElapsedMilliseconds.ToString()) / 1000;
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

		bool isOpponentAvailable()
		{
			return true;
		}

		void askUserToPlayAgain()
		{
			this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));
			// DialogResult res = ((FormMain)Parent).MSG("Do you want to play again?", "", MessageBoxButtons.YesNo);
			//  return res == DialogResult.Yes;
		}

		private void InitializeCommunication()
		{
			try
			{
                FormMain.MainGlobals.runTask = true;


				Thread InstanceCaller2 = new Thread(new ThreadStart(IncomingMessages)); //Thread InstanceCaller3 = new Thread(new ThreadStart(CheckOpponentConnection));
				InstanceCaller2.Start(); //InstanceCaller3.Start();
				InitializeUI();
				if (FormMain.MainGlobals.gamemode != "Host")
				{

					lbsrvaddr.Text = setGameAddress.Text;
					label107.Text = setGameAddress.Text;
				}
			}
			catch (Exception ex)
			{
				((FormMain)Parent).MSG("FATAL ERROR. Game needs to be stopped!" + ex.Message);
				return;
			}

		}
		private void Play_label_MouseLeave(object sender, EventArgs e) { ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 20); }
		private void Play_label_MouseHover(object sender, EventArgs e) { ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 16F); }
		public void InitializeUI()
		{
			if (gamemode == "Host")
			{
				meNickname.Text = "me: " + FormMain.MainGlobals.nickname; meNicknameGame.Text = "me: " + FormMain.MainGlobals.nickname; lbmeready.Text = "connected but not ready";
				opponentNickLobby.Text = "unknown"; opponentNickGame.Text = "unknown"; opponentstatusLobby.Text = "unknown";//   tabControl1.SelectedIndex = 0;
				lbsrvaddr.Text = extras.GetAllLocalIPv4(NetworkInterfaceType.Ethernet).FirstOrDefault() + "/" + extras.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault() + ":" + listenerPort.ToString();
				label107.Text = extras.GetAllLocalIPv4(NetworkInterfaceType.Ethernet).FirstOrDefault() + "/" + extras.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault() + ":" + listenerPort.ToString();
			}
			else
			{
				FormMain.MainGlobals.gamemode = "Client"; meNickname.Text = "me: " + FormMain.MainGlobals.nickname; meNicknameGame.Text = "me: " + FormMain.MainGlobals.nickname;
				lbmeready.Text = "connected but not ready"; opponentNickGame.Text = "unknown"; opponentNickLobby.Text = "unknown"; opponentstatusLobby.Text = "unknown";
				//   tabControl1.SelectedIndex = 2;
			}

		}
		public void SendDataAsServer(string data)
		{
			Thread thread = new Thread(() =>
			{
				try { string response = data; byte[] responseBytes = Encoding.UTF8.GetBytes(response); if(tcpclient!=null) tcpclient.GetStream().Write(responseBytes, 0, responseBytes.Length); Console.WriteLine("S:" + response); }
				catch (Exception) { ((FormMain)Parent).MSG("0x0005"); }
			}); thread.Start();

		}

		void setupGame()
		{
            czasGry.Start();
            czasGry.Reset();
            for (int i = 0; i < 10; ++i)
			{
				for (int j = 0; j < 10; ++j)
				{
					var picb = tableLayoutPanel12.GetControlFromPosition(j + 1, i + 1) as PictureBox;
					picb.BackgroundImage = images[i * 10 + j];
				}
			}
		}

		#region invoke required
		private void SwitchTab(int index) { if (tabControl1.InvokeRequired) { tabControl1.Invoke(new Action<int>(SwitchTab), index); return; } tabControl1.SelectedIndex = index; }
		private void LobbyUpdateMeNick(string text) { if (meNickname.InvokeRequired) { meNickname.Invoke(new Action<string>(LobbyUpdateMeNick), text); return; } meNickname.Text = text; }
		private void GameUpdateOpponentNick(string text)
		{
			if (opponentNickGame.InvokeRequired) { opponentNickGame.Invoke(new Action<string>(GameUpdateOpponentNick), text); return; }
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
		private void LobbyUpdateMeStatus(string text) { if (lbmeready.InvokeRequired) { lbmeready.Invoke(new Action<string>(LobbyUpdateMeStatus), text); return; } lbmeready.Text = text; }
		private void LobbyUpdateOpponentStatusk(string text)
		{
			if (opponentstatusLobby.InvokeRequired) { opponentstatusLobby.Invoke(new Action<string>(LobbyUpdateOpponentStatusk), text); return; }
			opponentstatusLobby.Text = text;
		}
		#endregion
		void cleanupAfterGame()
		{
			shipsShot = 0;
            this.Invoke(new Action(() => { GameNotify.Text = "it is opponent turn"; }));
            labeljeden.Text = "4"; labeldwa.Text = "3"; labeltrzy.Text = "2"; labelcztery.Text = "1";
			clickedArr = new bool[10, 10]; shipCords = new bool[10, 10]; ClientArray = new bool[10, 10]; HostArray = new bool[10, 10];
			shipsPlaced = 0; shipOrientation = ShipOrientation.Horizontal; shipsOfNSizePlaced = new int[4]; currentShipSize = 1;
			images = new Image[100]; HostReady = false; ClientReady = false; this.Invoke(new Action(() => { readybtn.Text = "READY"; }));
			//reset picboxow w lobby
			for (int i = 6; i <= 105; ++i) { var buf = this.Controls.Find(string.Format("pictureBox{0}", i), true).First() as PictureBox; buf.BackgroundImage = null; }
			resetShipCountLabels(); resetEnemyShipFields();
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
            int czas = int.Parse(czasGry.ElapsedMilliseconds.ToString()) / 1000;
            File.WriteAllText("TimeInGame", (TimeInGame + czas).ToString());
            FormMain.MainGlobals.wins = (wins + 1).ToString();
            FormMain.MainGlobals.PlayedBattles = (PlayedBattles + 1).ToString();
            FormMain.MainGlobals.DestroyedShips = (DestroyedShips + shipsShot).ToString();
            FormMain.MainGlobals.TimeInGame = (TimeInGame + czas).ToString();

            shipsShot = 0;
            this.Invoke(new Action(() => { tableLayoutPanel20.Visible = false; }));
            this.Invoke(new Action(() => { GameNotify.Text = "it is opponent turn";lbmeready.Text = "connected vut not ready";opponentstatusLobby.Text = "connected vut not ready";HostReady = false;ClientReady = false; }));

            if (res)
			{
				if (FormMain.MainGlobals.gamemode == "Host") { ((FormMain)Parent).MSG("new game started"); SwitchTab(0); }
				else
				{ ((FormMain)Parent).MSG("new game started"); lbsrvaddr.Text = setGameAddress.Text;  label107.Text = setGameAddress.Text; SwitchTab(0);
                }
			}
			else
			{ ((FormMain)Parent).MSG("game ended"); FormMain.MainGlobals.runTask = false; FormMain myparent = (FormMain)Parent; this.Invoke(new Action(() => { myparent.zmienwidok("GameLobbyView", "PlayView");  }));   }
		}
		TcpListener tcplistener; TcpClient tcpclient;
		public async void IncomingMessages()
		{
			int firstrun = 0;
			//init
			Console.WriteLine("Tasks Started"); if (gamemode != "Host") listenerPort = listenerPort + 50;
			while (true) { bool result = extras.chekIfPortAvaliable(listenerPort++); if (result) break; }


			if (FormMain.MainGlobals.gamemode == "Host")
			{
				try
				{
                    tcplistener = new TcpListener(IPAddress.Any, listenerPort); tcplistener.Start();
				}
				catch(Exception e)
				{
					((FormMain)Parent).MSG(e.Message);

                    return;
				}
				
				
				//tcpclient = tcplistener.AcceptTcpClient(); Console.WriteLine("Nowe połączenie od klienta: " + tcpclient.Client.RemoteEndPoint);
			}
			else
			{
				// ((FormMain)Parent).MSG(ServerAddress + listenerPort.ToString());
				//tcpclient = new TcpClient(); tcpclient.Connect(ServerAddress, int.Parse(ServerPort));
				//	tcpclient = new TcpClient(); tcpclient.Connect(ServerAddress, int.Parse(ServerPort));
				try
				{
                    tcpclient = new TcpClient(); tcpclient.Connect(ServerAddress, int.Parse(ServerPort));
                    SwitchTab(0);
                }
				catch(Exception ex) {
					((FormMain)Parent).MSG("unable to connect");
                    FormMain.MainGlobals.runTask = false;
					return;
				}
                Thread.Sleep(1000);


              
            }
			while (FormMain.MainGlobals.runTask)
			{
			
				if (FormMain.MainGlobals.gamemode == "Host")
				{
					while (true && FormMain.MainGlobals.runTask)

					{
                       
                        tcpclient = tcplistener.AcceptTcpClient(); Console.WriteLine("Nowe połączenie od klienta: " + tcpclient.Client.RemoteEndPoint);//
						while (tcpclient.Connected && FormMain.MainGlobals.runTask)
						{
                            Console.WriteLine(".");
                            try
							{   //read data
                                // if (client.Client.Poll(0, SelectMode.SelectRead))
                                
                                    byte[] buffer1 = new byte[1];
                                    if (tcpclient.Client.Receive(buffer1, SocketFlags.Peek) == 0)
                                    {
                                    ((FormMain)Parent).MSG("client disconnected");
                                        break;
                                    }
                                
                                byte[] buffer = new byte[1024]; int bytesRead = tcpclient.GetStream().Read(buffer, 0, buffer.Length);
								// Thread.Sleep();
								if (bytesRead == 0)
								{//conn closed
									Console.WriteLine(".");
									break;
								}
								//gen response
								string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
								if (request.Length != 0)
								{
									Console.WriteLine("S r: " + request);
									//proccess
									string[] requestArr = request.Split(new char[] { ';' });

									if (requestArr.Length > 1)
									{
										#region ify
										if (requestArr[0] == "join")//from client to server
										{
											ClientAddress = "localhost";//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
											ClientNickname = requestArr[1];
											ClientPort = requestArr[2];
											LobbyUpdateOpponentNick("opponent: " + ClientNickname);
											GameUpdateOpponentNick("opponent: " + ClientNickname);
											LobbyUpdateOpponentStatusk("connected but not ready");
										}
										else if (requestArr[0] == "gethostname")
										{//from client to server

											ClientPort = requestArr[1];
											ClientAddress = "localhost";//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
											SendDataAsServer("hostnickname;" + FormMain.MainGlobals.nickname + ";");
										}
										else if (requestArr[0] == "iwin")//from client to server
										{
                                            this.Invoke(new Action(() => { cleanupAfterGame(); }));
                                            this.Invoke(new Action(() => { OpponentWon(); }));

											
											this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));


										}
										else if (requestArr[0] == "iexit")//from client to server
										{
                                            this.Invoke(new Action(() => { ((FormMain)Parent).MSG("game ended"); }));
                                            this.Invoke(new Action(() => { OpponentExit(); }));
											// ();
											cleanupAfterGame();
											this.Invoke(new Action(() => { cleanupAfterGame(); }));

										}
										else if (requestArr[0] == "ready")//from client to server
										{
											OpponentIsReady();
										}
										else if (requestArr[0] == "hello")//from client to server
										{
											//error = "OK";
										}
										else if (requestArr[0] == "sendmatrix")//from client to server
										{
											// string[] arr = (requestArr[1]).Split(new char[] { '.' });
											int pos = 0, row = 0;
											string abc = "";
											foreach (char num in requestArr[1])
											{

												if (pos == 10)
												{
													pos = 0; row++;
												}
												ClientArray[row, pos] = (num == '1') ? true : false;
												pos++;
											}
											for (int i = 0; i < 10; i++)
											{
												for (int j = 0; j < 10; j++)
												{
													abc += (ClientArray[i, j]) ? "1" : "-";
												}
												abc += "\n";
											}



											// ((FormMain)Parent).MSG(request+"\n"+abc+"\n");

										}
										else if (requestArr[0] == "hostnickname")//from server to client
										{
											HostNickname = requestArr[1];

											FormMain.MainGlobals.nickname = requestArr[1];
											LobbyUpdateOpponentNick("opponent: " + HostNickname);
											GameUpdateOpponentNick("opponent: " + HostNickname);
											LobbyUpdateOpponentStatusk("connected but not ready");
										}
										else if (requestArr[0] == "youshooted")//from server to client
										{
											//Czarek zaaktualizuj plansze pov: ja strzeliłem <czekam na rezultat> <otrzymuje rezultat strzalu jako x,y,rezultat>
											// ((FormMain)Parent).MSG(requestArr[0]+" "+ requestArr[1]+" "+ requestArr[2]+" "+ requestArr[3]);
											handleClientShot(int.Parse(requestArr[1]), int.Parse(requestArr[2]), bool.Parse(requestArr[3]));
										}
										else if (requestArr[0] == "opponentwon")//from server to client
										{
                                            this.Invoke(new Action(() => { cleanupAfterGame(); }));
                                            this.Invoke(new Action(() => { OpponentWon(); }));
											//     cleanupAfterGame();
											
											//this.Invoke(new Action(() => { whenGameEndsSuccessfully(); }));
											//OpponentWon();
										}
										else if (requestArr[0] == "hostexit")//from server to client
										{
											HostExit();
											this.Invoke(new Action(() => { cleanupAfterGame(); }));
										}
										else if (requestArr[0] == "hostisready")
										{
											HostIsReady();
										}
										else if (requestArr[0] == "itisyourturn")
										{
											ItIsMyTurn = true;
											GameUpdateNotification("It is your turn");
										}
										else if (requestArr[0] == "itishostturn")
										{
											ItIsMyTurn = false;
											GameUpdateNotification("It is opponent turn");
										}
										else if (requestArr[0] == "gamestarted")
										{
											SwitchTab(1);
											setupGame();
										}
										else if (requestArr[0] == "HostShootedYou")//from server to client Czarek to jest akcja gdy host wykona strzał na clienta
										{
											HostShootedYou(int.Parse(requestArr[1]), int.Parse(requestArr[2]), bool.Parse(requestArr[3]));
										}
										else if (requestArr[0] == "clientshootedyou") //Czarek to jest akcja gdy client postrzeli hosta
										{
											clientshootedyou(int.Parse(requestArr[1]), int.Parse(requestArr[2]));
										}
										else
										{
											((FormMain)Parent).MSG("FATAL ERROR UNKNOWN COMMAND. Game needs to be stopped!");
										}
										#endregion }

									}
								}
								//write data
								// string response = request; byte[] responseBytes = Encoding.UTF8.GetBytes(response); tcpclient.GetStream().Write(responseBytes, 0, responseBytes.Length);

							}
							catch (Exception ex)
							{
								((FormMain)Parent).MSG("fatal error: " + ex.Message); break;
							}
						}
						tcpclient.Close(); ((FormMain)Parent).MSG("conn closed"); whenGameEndsSuccessfully(false);

                    }
				}
				else
				{
					while (tcpclient.Connected && FormMain.MainGlobals.runTask)
                    {
                        Console.WriteLine(".");
                        try
						{
							if (firstrun < 2)
							{
								// SendDataAsClinet("gethostname;");
								//SendDataAsClinet(");
								if (firstrun == 1)
								{
									byte[] nameBytes = Encoding.UTF8.GetBytes("join;" + FormMain.MainGlobals.nickname + ";");
									try
									{
										tcpclient.GetStream().Write(nameBytes, 0, nameBytes.Length);
									}
									catch (Exception ex)
									{
										((FormMain)Parent).MSG("0x1226. Game needs to be stopped! " + ex);
									}
								}
								if (firstrun == 0)
								{
									byte[] nameBytes = Encoding.UTF8.GetBytes("gethostname;");
									try
									{
										tcpclient.GetStream().Write(nameBytes, 0, nameBytes.Length);
									}
									catch (Exception ex)
									{
										((FormMain)Parent).MSG("0x1225. Game needs to be stopped! " + ex);
									}
								}



								firstrun++;
							}


							byte[] buffer = new byte[1024]; int bytesRead = tcpclient.GetStream().Read(buffer, 0, buffer.Length);
							if (bytesRead == 0)
							{
								Console.WriteLine(".");
								// Przerwij pę
								break;
							}
							string response = Encoding.UTF8.GetString(buffer, 0, bytesRead); Console.WriteLine("C r: " + response);
							string[] requestArr = response.Split(new char[] { ';' });
							if (requestArr.Length > 1)
							{
								#region ify
								if (requestArr[0] == "join")//from client to server
								{
									ClientAddress = "localhost";//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
									ClientNickname = requestArr[1];
									ClientPort = requestArr[2];
									LobbyUpdateOpponentNick("opponent: " + ClientNickname);
									GameUpdateOpponentNick("opponent: " + ClientNickname);
									LobbyUpdateOpponentStatusk("connected but not ready");
								}
								else if (requestArr[0] == "gethostname")
								{//from client to server

									ClientPort = requestArr[1];
									ClientAddress = "localhost";//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
									SendDataAsServer("hostnickname;" + FormMain.MainGlobals.nickname + ";");
								}
								else if (requestArr[0] == "iwin")//from client to server
								{
									this.Invoke(new Action(() => { OpponentWon(); }));

									this.Invoke(new Action(() => { cleanupAfterGame(); }));
									this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; }));


								}
								else if (requestArr[0] == "iexit")//from client to server
								{
                                    this.Invoke(new Action(() => { ((FormMain)Parent).MSG("game ended"); }));
                                    this.Invoke(new Action(() => { OpponentExit(); }));
									// ();
									cleanupAfterGame();
									this.Invoke(new Action(() => { cleanupAfterGame(); }));

								}
								else if (requestArr[0] == "ready")//from client to server
								{
									OpponentIsReady();
								}
								else if (requestArr[0] == "hello")//from client to server
								{
									//error = "OK";
								}
								else if (requestArr[0] == "sendmatrix")//from client to server
								{
									// string[] arr = (requestArr[1]).Split(new char[] { '.' });
									int pos = 0, row = 0;
									string abc = "";
									foreach (char num in requestArr[1])
									{

										if (pos == 10)
										{
											pos = 0; row++;
										}
										ClientArray[row, pos] = (num == '1') ? true : false;
										pos++;
									}
									for (int i = 0; i < 10; i++)
									{
										for (int j = 0; j < 10; j++)
										{
											abc += (ClientArray[i, j]) ? "1" : "-";
										}
										abc += "\n";
									}



									// ((FormMain)Parent).MSG(request+"\n"+abc+"\n");

								}
								else if (requestArr[0] == "hostnickname")//from server to client
								{
									HostNickname = requestArr[1];

									//FormMain.MainGlobals.nickname = requestArr[1];
									LobbyUpdateOpponentNick("opponent: " + HostNickname);
									GameUpdateOpponentNick("opponent: " + HostNickname);
									LobbyUpdateOpponentStatusk("connected but not ready");
								}
								else if (requestArr[0] == "youshooted")//from server to client
								{
									//Czarek zaaktualizuj plansze pov: ja strzeliłem <czekam na rezultat> <otrzymuje rezultat strzalu jako x,y,rezultat>
									// ((FormMain)Parent).MSG(requestArr[0]+" "+ requestArr[1]+" "+ requestArr[2]+" "+ requestArr[3]);
									handleClientShot(int.Parse(requestArr[1]), int.Parse(requestArr[2]), bool.Parse(requestArr[3]));
								}
								else if (requestArr[0] == "opponentwon")//from server to client
								{
									this.Invoke(new Action(() => { OpponentWon(); }));
									//     cleanupAfterGame();
									this.Invoke(new Action(() => { cleanupAfterGame(); }));
									//this.Invoke(new Action(() => { whenGameEndsSuccessfully(); }));
									//OpponentWon();
								}
								else if (requestArr[0] == "hostexit")//from server to client
								{
									HostExit();
									this.Invoke(new Action(() => { cleanupAfterGame(); }));
								}
								else if (requestArr[0] == "hostisready")
								{
									HostIsReady();
								}
								else if (requestArr[0] == "itisyourturn")
								{
									ItIsMyTurn = true;
									GameUpdateNotification("It is your turn");
								}
								else if (requestArr[0] == "itishostturn")
								{
									ItIsMyTurn = false;
									GameUpdateNotification("It is opponent turn");
								}
								else if (requestArr[0] == "gamestarted")
								{
									SwitchTab(1);
									setupGame();
								}
								else if (requestArr[0] == "HostShootedYou")//from server to client Czarek to jest akcja gdy host wykona strzał na clienta
								{
									HostShootedYou(int.Parse(requestArr[1]), int.Parse(requestArr[2]), bool.Parse(requestArr[3]));
								}
								else if (requestArr[0] == "clientshootedyou") //Czarek to jest akcja gdy client postrzeli hosta
								{
									clientshootedyou(int.Parse(requestArr[1]), int.Parse(requestArr[2]));
								}
								else
								{
									((FormMain)Parent).MSG("FATAL ERROR UNKNOWN COMMAND. Game needs to be stopped!");
								}
								#endregion }

							}

						}
						catch (Exception) { ((FormMain)Parent).MSG("broken pipe "); whenGameEndsSuccessfully(false); FormMain.MainGlobals.runTask = false; break;  }

					}
					tcpclient.Close();
					break;
				}
				//Thread.Sleep(300);
			}
		}

		public bool conn(string addr)
		{
			if (addr == null) return true;
			string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; byte[] buffer = Encoding.ASCII.GetBytes(data); options.DontFragment = true; if (addr.Length < 5) return true;
			try { PingReply reply = pingSender.Send(addr, 10000, buffer, options); if (reply.Status != IPStatus.Success) { return false; } else { return true; } } catch (Exception) { Console.WriteLine("0x0000"); return false; }
		}

		public void handleClientShot(int x, int y, bool res)
		{
			PictureBox pb = getOpponentPicbox(x, y);
			if (res)
			{
				pb.BackgroundImage = imgX;
				++shipsShot;
				if (shipsShot >= 20) // >= 20
				{
					this.Invoke(new Action(() => { cleanupAfterGame(); }));
					IWon();
				}
			}
			else
			{
				pb.BackgroundImage = imgMissed;
			}
		}

		public void SendDataAsClinet(string data)
		{
			if (ServerAddress.Length < 3) return;
			Console.WriteLine("C:" + data);
			byte[] nameBytes = Encoding.UTF8.GetBytes(data);
			try
			{

				tcpclient.GetStream().Write(nameBytes, 0, nameBytes.Length);
			}
			catch (Exception ex) { ((FormMain)Parent).MSG("0x1221. Game needs to be stopped! " + ex); }//});
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
				SendDataAsServer("youshooted;" + i + ";" + j + ";" + res);
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
				if (shipsShot >= 20) // >= 20
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
		private void Shoot(object sender, EventArgs z)
		{ //client and host action
		  //int x = e.Column, y = e.Row;

			// 
			PictureBox clickedButton = (PictureBox)sender;
			int y = tableLayoutPanel19.GetColumn(clickedButton) - 1;
			int x = tableLayoutPanel19.GetRow(clickedButton) - 1;

			if (ItIsMyTurn)
			{
				if (clickedArr[x, y])
				{
					((FormMain)Parent).MSG("pick another cell.");
					return;
				}
				clickedArr[x, y] = true;
				if (FormMain.MainGlobals.gamemode == "Host")//host strzela
				{
					//bool res = false;//<------------------------------------------------------------need to be edit
					//send host shooted

					//tutaj strzela host i od razu sprawdza i wysyla result strzalu <- czarek musisz to ustalić
					bool res;
					res = ClientArray[x, y];

					OnHostHitOrMissClient(x, y, res);//aktualizacja planszy
					SendDataAsServer("HostShootedYou;" + x + ";" + y + ";" + res.ToString());
				}
				else//klient strzela
				{
					SendDataAsClinet("clientshootedyou;" + x + ";" + y);
				}
				ItIsMyTurn = false;
				GameUpdateNotification("It opponent turn");
			}
			else
			{
				((FormMain)Parent).MSG("It is not your turn!");
			}

		}

		private void HostExit() //client action
		{
			//pause game
			//show message
			//runtasks=false
			//switch view
			((FormMain)Parent).MSG("Host closed the server");
            FormMain.MainGlobals.runTask = false;
			FormMain myparent = (FormMain)Parent;
			myparent.zmienwidok("GameLobbyView", "PlayView");
		}

		private void YESBTNC(object sender, EventArgs e)
		{
			this.Invoke(new Action(() => { whenGameEndsSuccessfully(true); }));
			this.Invoke(new Action(() => { tableLayoutPanel20.Visible = false; }));
          
                byte[] nameBytes = Encoding.UTF8.GetBytes("join;" + FormMain.MainGlobals.nickname + ";");
                try
                {
                    tcpclient.GetStream().Write(nameBytes, 0, nameBytes.Length);
                }
                catch (Exception ex)
                {
                    ((FormMain)Parent).MSG("0x1226. Game needs to be stopped! " + ex);
                }
            
            
                nameBytes = Encoding.UTF8.GetBytes("gethostname;");
                try
                {
                    tcpclient.GetStream().Write(nameBytes, 0, nameBytes.Length);
                }
                catch (Exception ex)
                {
                    ((FormMain)Parent).MSG("0x1225. Game needs to be stopped! " + ex);
                }
            
        }

		private void NOBTNC(object sender, EventArgs e)
		{
			this.Invoke(new Action(() => { whenGameEndsSuccessfully(false); }));
			this.Invoke(new Action(() => { tableLayoutPanel20.Visible = false; }));
            this.Invoke(new Action(() => { ((FormMain)Parent).MSG("game ended"); }));
            if (FormMain.MainGlobals.gamemode == "Host")
			{
				SendDataAsServer("iexit;");
			}
			else
			{
				SendDataAsClinet("iexit;");
			}
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

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

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
				((FormMain)Parent).MSG("you cannot use this option beacuse you placed some ships. "+shipsPlaced.ToString());
			}
			
        }

        private void LobbyBackClicked(object sender, EventArgs e)
		{
            FormMain.MainGlobals.runTask = false;
			if (FormMain.MainGlobals.gamemode == "Host")
			{
                this.Invoke(new Action(() => { ((FormMain)Parent).MSG("game ended"); }));
                SendDataAsServer("iexit;");
			}
			else
			{
                this.Invoke(new Action(() => { ((FormMain)Parent).MSG("game ended"); }));
                SendDataAsClinet("iexit;");
			}
			FormMain myParent = (FormMain)Parent;
			myParent.zmienwidok("GameLobbyView", "PlayView");
		}

		private void OpponentWon()
		{
			if (gamemode == "Host")
			{
				this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; GameNotify.Text = "opponent won"; }));
				((FormMain)Parent).MSG("Opponent won");
				GameNotify.Text = "opponent won";
                GameNotify.Text = "opponent won";
                ClientReady = false;
				this.Invoke(new Action(() => { opponentNickLobby.Text = "unknown"; }));
				this.Invoke(new Action(() => { opponentstatusLobby.Text = "unknown"; }));
				//runTasks = false;
				// FormMain myparent = (FormMain)Parent;
				// myparent.zmienwidok("GameLobbyView", "PlayView");
				//tak samo jak nizej
			}
			else
			{
				this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; GameNotify.Text = "opponent won"; }));
				GameNotify.Text = "opponent won";
                GameNotify.Text = "opponent won";
                ((FormMain)Parent).MSG("Opponent won");

				//runTasks = false;
				//FormMain myparent = (FormMain)Parent;
				//myparent.zmienwidok("GameLobbyView", "PlayView");
				//Kuba statystyki tutaj po tym
			}

		}
		private void OpponentExit() //host action
		{
			((FormMain)Parent).MSG("Opponent exit");
			FormMain.MainGlobals.runTask = false;
			FormMain myparent = (FormMain)Parent;
			myparent.zmienwidok("GameLobbyView", "PlayView");
		}
		private void IWon()//host action
		{
			if (FormMain.MainGlobals.gamemode == "Host")
			{
				((FormMain)Parent).MSG("You won");
				GameNotify.Text = "you won";
                GameNotify.Text = "you won";
                this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; GameNotify.Text = "you won"; }));
				this.Invoke(new Action(() => { opponentNickLobby.Text = "unknown"; }));
				this.Invoke(new Action(() => { opponentstatusLobby.Text = "unknown"; }));
				ClientReady = false;
				SendDataAsServer("iwin;");//Czarek !!!
										  // Kuba statystyki

			}
			else
			{
				((FormMain)Parent).MSG("You won");
				GameNotify.Text = "you won";
                GameNotify.Text = "you won";
                this.Invoke(new Action(() => { tableLayoutPanel20.Visible = true; GameNotify.Text = "you won"; }));
				//Kuba statystyki
				SendDataAsClinet("iwin;");//Czarek !!!

			}
		}


		private void CheckIfBothReady()
		{
			if (HostReady == true && ClientReady == true)
			{
				SendDataAsServer("gamestarted;");
				SwitchTab(1);
				setupGame();
				SendDataAsServer("gamestarted;");
				losujStrone();
				string hArrT = "", cArrT = "";

				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						if (shipCords[i, j])
						{
							hArrT += "1";
						}
						else
						{
							hArrT += "-";
						}
						if (ClientArray[i, j])
						{
							cArrT += "1";
						}
						else
						{
							cArrT += "-";
						}
					}
					cArrT += "\n";
					hArrT += "\n";

				}
				Console.WriteLine(hArrT);
				Console.WriteLine(cArrT);

			};
		}
		private void losujStrone()
		{
			Task.Delay(100);
			//itisyourturn  itishostturn
			if (extras.Draw())
			{
				ItIsMyTurn = true;
				SendDataAsServer("itishostturn;");
				SendDataAsServer("itishostturn;");
				GameUpdateNotification("It is your turn");

			}
			else
			{
				SendDataAsServer("itisyourturn;");
				SendDataAsServer("itisyourturn;");
				GameUpdateNotification("It is opponent turn");
			}
		}
		private void OpponentIsReady()//host action
		{
			if (ClientReady)
			{
				ClientReady = false;
				LobbyUpdateOpponentStatusk("connected but not ready");
			}
			else
			{
				ClientReady = true;
				LobbyUpdateOpponentStatusk("connected and ready");
			}

			CheckIfBothReady();
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
        Stopwatch czasGry = new Stopwatch();

        private void AutoSet()
		{
			ShipOrientation shipOrientation = ShipOrientation.Horizontal;
			shipCords = new bool[10, 10];
			shipsOfNSizePlaced = new int[] { 4,3,2,1};
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

               
       
    }
	}

