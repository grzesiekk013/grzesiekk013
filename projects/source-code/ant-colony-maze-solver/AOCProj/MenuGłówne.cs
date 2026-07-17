using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AOCProj
{
    public partial class MenuGłówne : Form
    {

        static int czasŻyciaMrówki = 1000; //Ticks
        static int ulatnianie = 0;
        static int feromon = 0;
        static int ilośćMrówek = 100;
        static int wysokość = 21;
        static int szerokość = 21;
        static float pd = 0.3f;
        static float pdDlaTrafionych = 0.01f;
        public int krok = 0;
        public bool trybAuto = false;
        public bool pętlaGłówna = false;
        public static int szybkośćPętli = 100;
        public Bitmap originMaze;
        public static Color[] ColorMapAnts =   {
                                Color.LightYellow, Color.LemonChiffon, Color.LightGoldenrodYellow, Color.PaleGoldenrod,
                                Color.Khaki, Color.Gold, Color.Yellow, Color.Orange,
                                Color.DarkOrange, Color.OrangeRed, Color.Tomato, Color.Coral,
                                Color.Salmon, Color.DarkSalmon, Color.LightCoral, Color.IndianRed,
                                Color.Crimson, Color.Red, Color.Firebrick, Color.DarkRed
                                };

        public static Color[] ColorMapFeromon = {
        Color.LightBlue, Color.PowderBlue, Color.SkyBlue, Color.DeepSkyBlue, 
        Color.DodgerBlue, Color.CornflowerBlue, Color.RoyalBlue, Color.MediumSlateBlue,
        Color.SlateBlue, Color.MediumPurple, Color.BlueViolet, Color.Indigo,
        Color.DarkViolet, Color.DarkOrchid, Color.MediumOrchid, Color.Orchid,
        Color.Violet, Color.Purple, Color.DarkMagenta, Color.MediumVioletRed
     };

    List<Mrówka> mrówki = new List<Mrówka>();
        Wierzchołek[,] wierzchołki;
        List<Krawędź> krawędzie;

        public enum EtapProgramu
        {
            BrakLabiryntu,
            GotowyDoPracy,
            wTrakciePracy,
            wTrakciePracyManualnej
        }

        bool[,] mapa = new bool[szerokość, wysokość];

        int[,] mapaG = new int[szerokość, wysokość];
        float[,] mapaF = new float[szerokość, wysokość];
        float[,] mapaM = new float[szerokość, wysokość];
        public byte[] labiryntPodstawowy;
        EtapProgramu etap = EtapProgramu.BrakLabiryntu;


        static Random rand = new Random();
        public void imgToPb(byte[] mrowki, byte[] feromon)
        {
          

            using (MemoryStream ms = new MemoryStream(mrowki))
            {
                this.pictureMrowki.Image = Image.FromStream(ms);     
            }
            using (MemoryStream ms = new MemoryStream(feromon))
            {
                this.pictureFeromon.Image = Image.FromStream(ms);
            }

            
            this.pictureMrowki.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureFeromon.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void drawFeromon(byte[] feromonF)
        {



            using (MemoryStream ms = new MemoryStream(feromonF))
            {
                this.pictureFeromon.Image = Image.FromStream(ms);
            }
            this.pictureFeromon.SizeMode = PictureBoxSizeMode.Zoom;
        }


        public void imgToPbStart(byte[] mrowki)
        {
            this.Invoke((MethodInvoker)delegate {
                
           
            using (MemoryStream ms = new MemoryStream(mrowki))
            {
                this.pictureMrowki.Image = Image.FromStream (ms);
            }
            this.pictureMrowki.SizeMode = PictureBoxSizeMode.Zoom;
            });
        }
        public byte[] MazeToIMG(bool[,] mapa, int szerokosc, int wysokosc)
        {
            int newWidth = szerokosc * 4;
            int newHeight = wysokosc * 4;

            using (Bitmap bitmap = new Bitmap(newWidth, newHeight))
            {
                for (int y = 0; y < wysokosc; y++)
                {
                    for (int x = 0; x < szerokosc; x++)
                    {
                        Color color = mapa[x, y] ? Color.Black : Color.DarkGray;

                        // Ustawienie koloru 3x3 pikseli
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                bitmap.SetPixel(x * 4 + i, y * 4 + j, color);
                            }
                        }
                    }
                }

                originMaze = (Bitmap)bitmap.Clone();

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
        public Color getAntColor(int AntsCount)
        {
            Color color = new Color();
            
            double ratio = (double)AntsCount / (double)ilośćMrówek;

            for (int i = 0; i< 20; i++)
            {

                if (
                    (ratio > (double)(i * 0.05)) 
                    && 
                    (ratio <= (double)((i + 1) * 0.05))
                    ) 
                { color = ColorMapAnts[i]; break; }
            

            }
           // log(color.ToString());
            return color;

        }

        public Color getFeromonColor(double Feromon, double threshold)
        {
            if (Feromon >= threshold)
            {
                return Color.MediumVioletRed; // Ustalony kolor dla wartości przekraczających próg
            }

            // Normalizacja wartości feromonu do zakresu [0, 1]
            double ratio = Feromon / threshold;

            // Ustalanie odpowiedniego indeksu koloru na podstawie znormalizowanej wartości
            int colorIndex = (int)(ratio * (ColorMapFeromon.Length - 1));

            // Zwracanie odpowiedniego koloru z palety
            return ColorMapFeromon[colorIndex];
        }

        public byte[] MazeWithAntsToImg(bool[,] mapa, int szerokosc, int wysokosc, List<Mrówka> mrówki)
        {
            int newWidth = szerokosc * 4;
            int newHeight = wysokosc * 4;

            using (Bitmap bitmap = (Bitmap)originMaze.Clone())
            {
              

                int[,] antsCountInSingleNode = new int[szerokosc, wysokosc];

              
                foreach (var mrowka in mrówki)
                {
                    antsCountInSingleNode[mrowka.lokalizacja.x, mrowka.lokalizacja.y] += 1;
                }

                String msg = "";
                for (int y = 1; y < wysokosc - 1; y++)
                {
                    for (int x = 1; x < szerokosc - 1; x++)
                    {
                        msg += antsCountInSingleNode[x, y].ToString() + "_";
                    }
                    msg += "\n";
                }
               // log(msg);

                for (int y = 1; y < wysokosc-1; y++)
                {
                    for (int x = 1; x < szerokosc-1; x++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if(antsCountInSingleNode[x, y] != 0)
                                {
                                    bitmap.SetPixel(x*4 + i, y * 4 + j, getAntColor(antsCountInSingleNode[x, y]));
                                    //log(getAntColor(antsCountInSingleNode[x, y]).ToString());
                                }
                                
                            }
                        }
                    }
                }
                

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public byte[] MazeWithFeromon(bool[,] mapa, int szerokosc, int wysokosc, List<Krawędź> krawędźie)
        {
            int newWidth = szerokosc * 4;
            int newHeight = wysokosc * 4;
            int Total = 0;

            using (Bitmap bitmap = (Bitmap)originMaze.Clone())
            {


                double[,] feromoneInSigneNode = new double[szerokosc, wysokosc];

           //    log("liczba krawedzi " + krawędzie.Count);
                foreach (var krawedz in krawędzie)
                {
                    feromoneInSigneNode[krawedz.a.x, krawedz.a.y] = krawedz.feromon;
                    feromoneInSigneNode[krawedz.b.x, krawedz.b.y] = krawedz.feromon;
                    
                }
                Total = feromon * (szerokosc + wysokosc) / 2;
       

                for (int y = 1; y < wysokosc - 1; y++)
                {
                    for (int x = 1; x < szerokosc - 1; x++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (feromoneInSigneNode[x, y] != 0)
                                {
                                    bitmap.SetPixel(x * 4 + i, y * 4 + j, getFeromonColor(feromoneInSigneNode[x, y], Total));
                                    //log(getAntColor(antsCountInSingleNode[x, y]).ToString());
                                }

                            }
                        }
                    }
                }

             //   log("total feromon " + Total);
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public int krokPlusPlus()
        {

            krok++;
            this.Invoke((MethodInvoker)delegate {
                
            
            lbKroki.Text = krok.ToString();
            });
            return krok;
           
        }
        public void generujKrawędzieWierzchołki(bool[,] mapa, int szerokość, int wysokość)
        {
            int liczbaWolnych = 0;
            for (int x = 0; x < szerokość; x++)
            {
                for (int y = 0; y < wysokość; y++)
                {
                    if (!mapa[x, y]) liczbaWolnych++; // nie ma ściany
                }
            }

            bool[,] wykorzystane = new bool[szerokość, wysokość];
            wierzchołki = new Wierzchołek[szerokość, wysokość];//przyp do zm glob


           

            int X, Y;

            for (X = 1; X < (szerokość - 1); X++)
            {
                for (Y = 1; Y < (wysokość - 1); Y++)
                {
                    //gora
                    if ((!mapa[X, Y - 1]) && (wykorzystane[X, Y - 1] == false))
                    {
                        wierzchołki[X, Y - 1] = new Wierzchołek(X, Y - 1);
                        wykorzystane[X, Y - 1] = true;
                    }
                    //dół
                    if ((!mapa[X, Y + 1]) && (wykorzystane[X, Y + 1] == false))
                    {
                        wierzchołki[X, Y + 1] = new Wierzchołek(X, Y + 1);
                        wykorzystane[X, Y + 1] = true;
                    }
                    //lewo
                    if ((!mapa[X - 1, Y]) && (wykorzystane[X - 1, Y] == false))
                    {
                        wierzchołki[X - 1, Y] = new Wierzchołek(X - 1, Y);
                        wykorzystane[X - 1, Y] = true;
                    }
                    //prawo
                    if ((!mapa[X + 1, Y]) && (wykorzystane[X + 1, Y] == false))
                    {
                        wierzchołki[X + 1, Y] = new Wierzchołek(X + 1, Y);
                        wykorzystane[X + 1, Y] = true;
                    }
                }
            }

            //generowanie krawędzi

            krawędzie = new List<Krawędź>();

            for (X = 1; X < (szerokość - 1); X += 2)
            {
                for (Y = 1; Y < (wysokość - 1); Y += 2)
                {
                    //gora
                    if ((!mapa[X, Y - 1]))
                    {
                        if ((wierzchołki[X, Y] != null) && (wierzchołki[X, Y - 1]) != null)
                        {
                            Krawędź temp = new Krawędź(wierzchołki[X, Y], wierzchołki[X, Y - 1]);
                            krawędzie.Add(temp);
                            wierzchołki[X, Y].krawędzie.Add(temp);
                            wierzchołki[X, Y - 1].krawędzie.Add(temp);
                        }
                    }

                    //dol
                    if ((!mapa[X, Y + 1]))
                    {
                        if ((wierzchołki[X, Y] != null) && (wierzchołki[X, Y + 1]) != null)
                        {
                            Krawędź temp = new Krawędź(wierzchołki[X, Y], wierzchołki[X, Y + 1]);
                            krawędzie.Add(temp);
                            wierzchołki[X, Y].krawędzie.Add(temp);
                            wierzchołki[X, Y + 1].krawędzie.Add(temp);
                        }
                    }

                    //lewo
                    if ((!mapa[X - 1, Y]))
                    {
                        if ((wierzchołki[X, Y] != null) && (wierzchołki[X - 1, Y]) != null)
                        {
                            Krawędź temp = new Krawędź(wierzchołki[X, Y], wierzchołki[X - 1, Y]);
                            krawędzie.Add(temp);
                            wierzchołki[X, Y].krawędzie.Add(temp);
                            wierzchołki[X - 1, Y].krawędzie.Add(temp);
                        }
                    }

                    //prawo
                    if ((!mapa[X + 1, Y]))
                    {
                        if ((wierzchołki[X, Y] != null) && (wierzchołki[X + 1, Y]) != null)
                        {
                            Krawędź temp = new Krawędź(wierzchołki[X, Y], wierzchołki[X + 1, Y]);
                            krawędzie.Add(temp);
                            wierzchołki[X, Y].krawędzie.Add(temp);
                            wierzchołki[X + 1, Y].krawędzie.Add(temp);
                        }
                    }



                }
            }


            //ustawienie startu i konca
            wierzchołki[1, 1].isHome = true;
            wierzchołki[szerokość - 2, wysokość - 2].isEnd = true; // ustawienie końca

            //logi do wyjebania
            String msg = "";
            for (Y = 0; Y < (wysokość ); Y += 1)
               
            {
                for (X = 0; X < (szerokość ); X += 1)
                {
                    if (mapa[X, Y]) msg += "1_";
                    else msg += "0_";

                }
                msg += "\n";
            }
            //log(msg);

            msg = "";
            for (Y = 0; Y < (wysokość); Y += 1)
                
            {
                for (X = 0; X < (szerokość); X += 1)
                {
                    if (mapa[X, Y]) msg += "$_"; // Ściana
                    else if (wierzchołki[X, Y] != null) msg += wierzchołki[X, Y].krawędzie.Count().ToString() + "_"; 
                    else msg += "0_"; 
                                      
                }
                msg += "\n";
        }

            //log(msg);

        }
        public void drawLegend()
        {
            for(int i = 0; i< tableLegendaMrowki.ColumnCount; i++)
            {
                ((Panel)tableLegendaMrowki.GetControlFromPosition(i, 0)).BackColor = (Color)ColorMapAnts[i];
                ((Label)tableLegendaMrowki.GetControlFromPosition(i, 1)).Text = (i*0.05).ToString()+ "\n -\n"+((i+1)*0.05);
            }

            for (int i = 0; i < tableLegendaFeromon.ColumnCount; i++)
            {
                ((Panel)tableLegendaFeromon.GetControlFromPosition(i, 0)).BackColor = (Color)ColorMapFeromon[i];
                ((Label)tableLegendaFeromon.GetControlFromPosition(i, 1)).Text = (i * 0.05).ToString() + "\n -\n" + ((i + 1) * 0.05);
            }
        }
        public void main()
        {
            lbStatus.Text = etap.ToString();
            enableControlls();
            drawLegend();
         }

        public static bool[,] GenerateMaze(int width, int height)
        {
            bool[,] maze = new bool[width, height];

            // Inicjalizacja labiryntu - wszystkie komórki są ścianami
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    maze[x, y] = true;

            // Startowanie generowania labiryntu od komórki (1, 1)
            CarvePassages(1, 1, maze);

            // Ustawienie punktu startowego i końcowego
            maze[1, 1] = false; // początek
            maze[width - 2, height - 2] = false; // koniec

            return maze;
        }

        private static void CarvePassages(int cx, int cy, bool[,] maze)
        {
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };
            int[] directions = { 0, 1, 2, 3 };

            // Tasowanie kierunków, aby uzyskać losowe rozwiązanie
            Shuffle(directions);

            for (int i = 0; i < directions.Length; i++)
            {
                int nx = cx + dx[directions[i]] * 2;
                int ny = cy + dy[directions[i]] * 2;

                if (IsInBounds(nx, ny, maze) && maze[nx, ny])
                {
                    maze[cx + dx[directions[i]], cy + dy[directions[i]]] = false;
                    maze[nx, ny] = false;
                    CarvePassages(nx, ny, maze);
                }
            }
        }

        private static bool IsInBounds(int x, int y, bool[,] maze)
        {
            return x > 0 && x < maze.GetLength(0) - 1 && y > 0 && y < maze.GetLength(1) - 1;
        }

        private static void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }


        public MenuGłówne()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            main();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerujLabirynt_Click(object sender, EventArgs e)
        {
            try
            {
                 szerokość = int.Parse(tbSzerokoscPlanszy.Text);
                 wysokość = int.Parse(tbWysokoscPlanszy.Text);
                if (szerokość < 20)
                {
                    log("Minimalna szerokość wynosi 20 jednostek");
                    return;
                }
                if (szerokość > 200)
                {
                    log("Maksymalna szerokość wynosi 200 jednostek");
                    return;
                }
                if (wysokość < 20)
                {
                    log("Minimalna wysokość wynosi 20 jednostek");
                    return;
                }
                if (wysokość > 200)
                {
                 log("Maksymalna wysokość wynosi 200 jednostek");
                    return;
                }
                if (wysokość < 5) wysokość += 5;
                if (szerokość % 2 == 0) szerokość += 1;
                if (wysokość % 2 == 0) wysokość += 1;
                bool[,] maze = GenerateMaze(szerokość, wysokość);
                mapa = maze;

                szerokość = mapa.GetLength(0);
                wysokość = mapa.GetLength(1);

                int szerokosc = mapa.GetLength(0);
                int wysokosc = mapa.GetLength(1);

                
                byte[] png = MazeToIMG(mapa, szerokosc, wysokosc);

                imgToPb(png, png);
                log("Wygenerowano labirynt o szerokości: " + szerokość + ", wysokości: " + wysokość+".");
                etap = EtapProgramu.GotowyDoPracy;
                lbStatus.Text = etap.ToString();
            }
            catch (Exception)
            {
                log("Nie można odczytać poprawnej szerokości/wysokości planszy.");
                return;
            }
            
        }
        public void startAlgorytmu()
        {
            try
            {
                szybkośćPętli = int.Parse(tbPredkosc.Text);
            }
            catch (Exception)
            {
                log("Wystąpił błąd podczas próby odczytu pola szybkości pętli automatycznej");
                return;
            }

            try
            {
                ilośćMrówek = int.Parse(tbIlośćMrówek.Text);
            }catch (Exception)
            {
                log("Wystąpił błąd podczas próby odczytu pola ilość mrówek.");
                return;
            }
            try
            {
                ulatnianie = int.Parse(tbWspUlatniaia.Text);
            }
            catch (Exception)
            {
                log("Wystąpił błąd podczas próby odczytu pola wsp. ulatniania.");
                return;
            }
            try
            {
                feromon = int.Parse(tbWspFeromonu.Text);
            }
            catch (Exception)
            {
                log("Wystąpił błąd podczas próby odczytu pola wsp. feromonu.");
                return;
            }
            try
            {
                czasŻyciaMrówki = int.Parse(tbCzasŻyciaKolonii.Text);
            }
            catch (Exception)
            {
                log("Wystąpił błąd podczas próby odczytu pola czas życia koloni.");
                return;
            }
            

            
            generujKrawędzieWierzchołki(mapa, szerokość, wysokość);

            if (trybAuto) algorytmAuto();
            else algorytmManual();



        }
        public void algorytmAuto()
        {
            disableControlls();
            log("Wystartowano algorytm mrówkowy w trybie " + ((trybAuto) ? "automatycznym" : "manualnym"));
            etap = EtapProgramu.wTrakciePracy;
            lbStatus.Text = etap.ToString();
            mrówki.Clear();
            for (int i = 0; i < ilośćMrówek; i++)
            {

                mrówki.Add(new Mrówka(wierzchołki[1, 1], czasŻyciaMrówki, pd));

                
            }
            pętlaGłówna = true;
            Console.WriteLine("algorytm auto start");
            Thread myThread = new Thread(new ThreadStart(threadLoop));
            myThread.Start();


        } 
        public void threadLoop()
        {

            while (pętlaGłówna)
            {
                Thread.Sleep(szybkośćPętli);
                przejscieMrowekManual();
            }
        }
        public void algorytmManual() {
            disableControlls();
            log("Wystartowano algorytm mrówkowy w trybie " + ((trybAuto) ? "automatycznym" : "manualnym"));
            log("Kliknij przycisk dalej, aby kontynuować działanie programu");
            etap = EtapProgramu.wTrakciePracyManualnej;
            lbStatus.Text = etap.ToString();
            if (krok == 0)
            {
                mrówki.Clear();
                for (int i = 0; i < ilośćMrówek; i++)
                {
                    mrówki.Add(new Mrówka(wierzchołki[1, 1], czasŻyciaMrówki, pd));
                }
              
                byte[] png1 = MazeWithAntsToImg(mapa, szerokość, wysokość, mrówki);
                imgToPbStart(png1);
            }
            
        }
            

        private void log(string msg)
        {
            
            this.Invoke((MethodInvoker)delegate {
                rtbLogs.AppendText(msg + "\n");
               
            });
        }
        private void logAuto(string msg)
        {
            rtbLogs.AppendText(msg + "\n");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (etap != EtapProgramu.GotowyDoPracy)
            {
                log("Nie można uruchomić akcji z powodu: " + etap);
                return;
            }

            trybAuto = rdAuto.Checked;

            btnGenerujLabirynt.Enabled = false;

            lbKroki.Text = "0";
            krok = 0;



            startAlgorytmu();
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (etap == EtapProgramu.wTrakciePracy || etap == EtapProgramu.wTrakciePracyManualnej)
            {
                enableControlls();
                log("Zatrzymano algorytm mrówkowy.");
                etap = EtapProgramu.GotowyDoPracy;
                lbStatus.Text = etap.ToString();
                btnGenerujLabirynt.Enabled = true;
                pętlaGłówna = false;
                return;
            }
            log("Nie można uruchomić akcji z powodu: " + etap);
            

        }
       
        public void disableControlls()
        {
            rdAuto.Enabled = false;
            rdManual.Enabled = false;
            tbCzasŻyciaKolonii.Enabled = false;
            tbPredkosc.Enabled = false;
            tbIlośćMrówek.Enabled = false;
            tbSzerokoscPlanszy.Enabled = false;
            tbWspFeromonu.Enabled = false;
            tbWspUlatniaia.Enabled = false;
            tbWysokoscPlanszy.Enabled   =false;
        }
        public void enableControlls()
        {
            rdAuto.Enabled = true;
            rdManual.Enabled = true;
            tbPredkosc.Enabled = true;
            tbCzasŻyciaKolonii.Enabled = true;
            tbIlośćMrówek.Enabled = true;
            tbSzerokoscPlanszy.Enabled = true;
            tbWspFeromonu.Enabled = true;
            tbWspUlatniaia.Enabled = true;
            tbWysokoscPlanszy.Enabled = true;
        }
        public void przejscieMrowekManual()
        {
            int zabite = 0;
            foreach (var krawedz in krawędzie) //  ulatnianie feromonu 
            {
                // ulatnianie
                if (krawedz.feromon != 0) krawedz.feromon -= ulatnianie;
                if (krawedz.feromon < 0) krawedz.feromon = 0;
            }
            List<Krawędź> krawedzieToBeUpdate = new List<Krawędź>();
            for (int i = 0; i < mrówki.Count; i++)

            {


                if (mrówki[i].życie <= 0)
                {
                    zabite++;
                    mrówki[i].zabij(czasŻyciaMrówki, pd);
                    log("Zabito mrówke z indexem : " + i);

                }
               
                mrówki[i].życie--;
                
                Mrówka mrówka = mrówki[i];
                Wierzchołek aktualnaPozycja = mrówka.lokalizacja;
                var dostepneKrawedzie = aktualnaPozycja.krawędzie;

                List<Krawędź> wybory = new List<Krawędź>();
                List<float> prawdopodobienstwa = new List<float>();
                float sumaFeromonow = 0;

                foreach (var krawedz in dostepneKrawedzie)  //dodajemy do wyborów możliwe krawędzie do wyboru + sumujemy feromon na każdej krawędzi i dodajemy feromon do listy Prawdopodobieństwa
                {
                    Wierzchołek cel = (krawedz.a == aktualnaPozycja) ? krawedz.b : krawedz.a;
                    if (cel != mrówka.lokalizacjaOstatni)
                    {
                        wybory.Add(krawedz);
                        sumaFeromonow += krawedz.feromon + 1; 
                        prawdopodobienstwa.Add(krawedz.feromon + 1);
                    }
                }

                if (wybory.Count > 0)
                {
                    float wylosowanaWartosc = (float)rand.NextDouble() * sumaFeromonow; //losujemy liczbe randomową z przedziału sumy prawd
                    float akumulacja = 0;
                    float wylosowana2Wartosc = (float)rand.NextDouble();
                    Krawędź wybranaKrawedz = null;
                    for (int j = 0; j < wybory.Count; j++)
                    {
                        akumulacja += prawdopodobienstwa[j];
                        if (wylosowana2Wartosc > pd)
                        {
                            if (wylosowanaWartosc <= akumulacja)
                            {
                                wybranaKrawedz = wybory[j];
                                break;
                            }
                        }
                        else
                        {
                            Random rand = new Random();
                            wybranaKrawedz = wybory[rand.Next(wybory.Count)];
                           // log("random 1");
                        }

                    }

                    if (wybranaKrawedz != null)
                    {
                        Wierzchołek nowaLokalizacja = (wybranaKrawedz.a == aktualnaPozycja) ? wybranaKrawedz.b : wybranaKrawedz.a;
                        if (!mrówka.wiekszyFeromon)
                        {
                            mrówka.pamięć.Add(aktualnaPozycja);
                            mrówka.lokalizacjaOstatni = aktualnaPozycja;
                          //  wybranaKrawedz.feromon += feromon;
                            krawedzieToBeUpdate.Add(wybranaKrawedz);

                            mrówka.AktualizowanaLokalizacja(nowaLokalizacja); //aktualizujemy lokalizacje naszej mrówki
                        }
                        else
                        {
                            mrówka.pamięć.Add(aktualnaPozycja);
                            mrówka.lokalizacjaOstatni = aktualnaPozycja;
                            mrówka.życie = czasŻyciaMrówki;
                            wybranaKrawedz.feromon += feromon * 2;

                            mrówka.AktualizowanaLokalizacja(nowaLokalizacja); //aktualizujemy lokalizacje naszej mrówki
                        }
                        

                        // Sprawdzamy, czy mrówka dotarła do celu
                        if (nowaLokalizacja.isEnd)
                        {
                            mrówka.wiekszyFeromon = true;
                            mrówka.prawd = pdDlaTrafionych;
                            mrówka.ZmienKierunek();//zmieniamy flagę boolowską
                            if (mrówka.pamięć.Count > 1) //cofamy mrówkę o jedno pole
                            {
                                mrówka.lokalizacjaOstatni = mrówka.lokalizacja;
                                mrówka.lokalizacja = mrówka.pamięć[mrówka.pamięć.Count - 1];
                                mrówka.pamięć.RemoveAt(mrówka.pamięć.Count - 1);
                                aktualnaPozycja = mrówka.lokalizacja;
                                dostepneKrawedzie = aktualnaPozycja.krawędzie;
                                

                                

                            }
                        }
                        // Sprawdź, czy mrówka dotarła do startu
                        else if (nowaLokalizacja.isHome && mrówka.wracaDoStartu)
                        {
                            mrówka.ZmienKierunek(); //zmieniamy flagę boolowską
                            if (mrówka.pamięć.Count > 1)//cofamy mrówkę o jedno pole
                            {
                                mrówka.lokalizacjaOstatni = mrówka.lokalizacja;
                                mrówka.lokalizacja = mrówka.pamięć[mrówka.pamięć.Count - 1];
                                mrówka.pamięć.RemoveAt(mrówka.pamięć.Count - 1);
                                aktualnaPozycja = mrówka.lokalizacja;
                                dostepneKrawedzie = aktualnaPozycja.krawędzie;

                                
                            }
                        }
                    }
                }
                else //jeżeli mrówka trafia do miejsca z którego nie może wyjśc wywołuje się else
                {
                    if (mrówka.pamięć.Count > 0)
                    {
                        mrówka.lokalizacjaOstatni = mrówka.lokalizacja;
                        mrówka.lokalizacja = mrówka.pamięć[mrówka.pamięć.Count - 1];
                        mrówka.pamięć.RemoveAt(mrówka.pamięć.Count - 1);

                        aktualnaPozycja = mrówka.lokalizacja;
                        dostepneKrawedzie = aktualnaPozycja.krawędzie; //cofamy mrówke o jedno pole

                        wybory.Clear();
                        prawdopodobienstwa.Clear();
                        sumaFeromonow = 0;

                        foreach (var krawedz in dostepneKrawedzie) //wybieramy nową krawędź dla mrówki
                        {
                            Wierzchołek cel = (krawedz.a == aktualnaPozycja) ? krawedz.b : krawedz.a;
                            if (cel != mrówka.lokalizacjaOstatni)
                            {
                                wybory.Add(krawedz);
                                sumaFeromonow += krawedz.feromon + 1;
                                prawdopodobienstwa.Add(krawedz.feromon + 1);
                            }
                        }

                        if (wybory.Count > 0)
                        {
                            float wylosowanaWartosc = (float)rand.NextDouble() * sumaFeromonow;
                            float wylosowana2Wartosc = (float)rand.NextDouble();
                            float akumulacja = 0;

                            Krawędź wybranaKrawedz = null;
                            for (int j = 0; j < wybory.Count; j++)
                            {
                                akumulacja += prawdopodobienstwa[j];
                                if (wylosowana2Wartosc > pd)
                                {
                                    if (wylosowanaWartosc <= akumulacja)
                                    {
                                        wybranaKrawedz = wybory[j];
                                        break;
                                    }
                                }
                                else
                                {
                                    Random rand = new Random();
                                    wybranaKrawedz = wybory[rand.Next(wybory.Count)];
                                   // log("random");
                                }

                            }

                            if (wybranaKrawedz != null)
                            {
                                Wierzchołek nowaLokalizacja = (wybranaKrawedz.a == aktualnaPozycja) ? wybranaKrawedz.b : wybranaKrawedz.a;
                                if (!mrówka.wiekszyFeromon)
                                {
                                    mrówka.pamięć.Add(aktualnaPozycja);
                                    mrówka.lokalizacjaOstatni = aktualnaPozycja;
                                    wybranaKrawedz.feromon += feromon;
                                    krawedzieToBeUpdate.Add(wybranaKrawedz);
                                   // mrówka.AktualizowanaLokalizacja(nowaLokalizacja);
                                }
                                else
                                {
                                    mrówka.pamięć.Add(aktualnaPozycja);
                                    mrówka.lokalizacjaOstatni = aktualnaPozycja;
                                    mrówka.życie = czasŻyciaMrówki;
                                    wybranaKrawedz.feromon += feromon *2;


                                    mrówka.AktualizowanaLokalizacja(nowaLokalizacja);
                                }

                                

                                // Sprawdź, czy mrówka dotarła do celu
                                if (nowaLokalizacja.isEnd)
                                {
                                    mrówka.wiekszyFeromon = true;
                                    mrówka.ZmienKierunek();
                                    mrówka.prawd = pdDlaTrafionych;
                                    if (mrówka.pamięć.Count > 1)
                                    {
                                        mrówka.lokalizacjaOstatni = mrówka.lokalizacja;
                                        mrówka.lokalizacja = mrówka.pamięć[mrówka.pamięć.Count - 1];
                                        mrówka.pamięć.RemoveAt(mrówka.pamięć.Count - 1);
                                        aktualnaPozycja = mrówka.lokalizacja;
                                        dostepneKrawedzie = aktualnaPozycja.krawędzie;
                                    }
                                }
                                // Sprawdź, czy mrówka dotarła do startu
                                else if (nowaLokalizacja.isHome && mrówka.wracaDoStartu)
                                {
                                    mrówka.ZmienKierunek();
                                    if (mrówka.pamięć.Count > 1)
                                    {
                                        mrówka.lokalizacjaOstatni = mrówka.lokalizacja;
                                        mrówka.lokalizacja = mrówka.pamięć[mrówka.pamięć.Count - 1];
                                        mrówka.pamięć.RemoveAt(mrówka.pamięć.Count - 1);
                                        aktualnaPozycja = mrówka.lokalizacja;
                                        dostepneKrawedzie = aktualnaPozycja.krawędzie;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach(var kraw in krawedzieToBeUpdate)
            {
                kraw.feromon += feromon;
            }
            if(zabite == mrówki.Count)
            {
                log("odświezenie planszy");
                foreach (var krawedz in krawędzie)
                {
                    krawedz.feromon = 0;
                }
            }
            byte[] png1 = MazeWithAntsToImg(mapa, szerokość, wysokość, mrówki);
            imgToPbStart(png1);
            krokPlusPlus();
            drawFeromon(MazeWithFeromon(mapa, szerokość, wysokość, krawędzie));
        }

       
        private void btnDalejClick(object sender, EventArgs e)
        {
            if (etap != EtapProgramu.wTrakciePracyManualnej)
            {
                log("Nie można uruchomić akcji z powodu: "+ etap);
                return;
            }

            
            
                przejscieMrowekManual();
            

           // log("Pomyślnie zwiększono krok programu do: "+ krokPlusPlus() + "");
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void rtbClear(object sender, EventArgs e)
        {
            rtbLogs.Clear();
        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
    public class Wierzchołek
    {
        public bool isHome = false;
        public bool isEnd = false;
        public String ind;
        public List<Krawędź> krawędzie = new List<Krawędź> ();
        public int x { get; set; }
        public int y { get; set; }
        public Wierzchołek(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.ind = x.ToString() + "," + y.ToString();
        }
    }
    public class Krawędź
    {
        public float feromon = 0;
        public float prawdopodobieństwo = 0.000f;
        public float prawdopodobieństwoOryginalne = 0.000f;

        public Wierzchołek a;
        public Wierzchołek b;

        public Krawędź(Wierzchołek a, Wierzchołek b) { 
            this.a = a;
            this.b = b;
        }
    }

    public class Mrówka
    {
        private Wierzchołek dom;
        public int życie;
        public Wierzchołek lokalizacja;
        public Wierzchołek lokalizacjaOstatni;
        public List<Wierzchołek> pamięć;
        public bool wracaDoStartu;
        public bool wiekszyFeromon;
        public float prawd;
        public Mrówka(Wierzchołek lokalizacja, int życie, float pd) {
            this.dom = lokalizacja;
            this.lokalizacja = lokalizacja;
            this.życie = życie;
            this.pamięć = new List<Wierzchołek>();
            this.pamięć.Add(lokalizacja);
            this.wracaDoStartu = false;
            this.wiekszyFeromon = false;
            this.prawd = pd;

        }
        public void zabij(int noweŻycie, float pd)
        {
            
            this.lokalizacja = this.dom;
            this.pamięć.Clear();
            this.wracaDoStartu = false;
            this.wiekszyFeromon = false;
            this.życie = noweŻycie;
            this.prawd = pd;
        }
        
        public void AktualizowanaLokalizacja(Wierzchołek nowaLokalizacja)
        {
            this.lokalizacjaOstatni = this.lokalizacja;
            this.lokalizacja = nowaLokalizacja;
            this.pamięć.Add(nowaLokalizacja);

        }

        public void ZmienKierunek()
        {
            this.wracaDoStartu = !this.wracaDoStartu;
        }


    }
    
}
