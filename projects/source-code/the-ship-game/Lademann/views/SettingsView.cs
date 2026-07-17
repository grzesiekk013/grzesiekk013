using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Lademann.views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
         //   MessageBox.Show(LicenseManager.UsageMode.ToString());
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Wyłącz odtwarzanie dźwięku w trybie designu
                return;
            }
            string[] arr = new string[4];
            //READ file config
            if (!File.Exists(Path.Combine("config.conf")))
            {
                try
                {
                    File.Create(Path.Combine("config.conf")).Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //volume,nickname,theme,resolution
                try
                {
                    arr[0] = "volume=3";
                    arr[1] = "nickname=player1";
                    arr[2] = "theme=space";
                    arr[3] = "resolution=1366x853";
                    File.WriteAllLines(Path.Combine("config.conf"), arr);
                    File.ReadAllLines(Path.Combine("config.conf"));
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].StartsWith("volume="))
                        {
                            FormMain.MainGlobals.volume = arr[i].Substring(arr[i].IndexOf("=") + 1, (arr[i].Length) - arr[i].IndexOf("=") + 1);
                        }
                        else if (arr[i].StartsWith("nickname="))
                        {
                            FormMain.MainGlobals.nickname = arr[i].Substring(arr[i].IndexOf("=") + 1, (arr[i].Length) - arr[i].IndexOf("=") + 1);
                            textBoxNickname.Text = FormMain.MainGlobals.nickname;
                        }
                        else if (arr[i].StartsWith("theme="))
                        {
                            FormMain.MainGlobals.theme = arr[i].Substring(arr[i].IndexOf("=") + 1, (arr[i].Length) - arr[i].IndexOf("=") + 1);
                        }
                        else if (arr[i].StartsWith("resolution="))
                        {
                            FormMain.MainGlobals.resolution = arr[i].Substring(arr[i].IndexOf("=") + 1, (arr[i].Length) - arr[i].IndexOf("=") + 1);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                string[] data = File.ReadAllLines(Path.Combine("config.conf"));
                for(int i = 0; i<data.Length; i++)
                {
                    if (data[i].StartsWith("volume="))
                    {
                        int ind,len;
                        ind = data[i].IndexOf("=") + 1;
                        len = data[i].Length  - ind;
                        FormMain.MainGlobals.volume = data[i].Substring(ind,len);
                    }else if(data[i].StartsWith("nickname="))
                    {
                        int ind, len;
                        ind = data[i].IndexOf("=") + 1;
                        len = data[i].Length  - ind;
                        FormMain.MainGlobals.nickname = data[i].Substring(ind, len);
                        textBoxNickname.Text = FormMain.MainGlobals.nickname;
                    }
                    else if (data[i].StartsWith("theme="))
                    {
                        int ind, len;
                        ind = data[i].IndexOf("=") + 1;
                        len = data[i].Length - ind;
                        FormMain.MainGlobals.theme = data[i].Substring(ind, len);
                    }
                    else if (data[i].StartsWith("resolution=")){
                        int ind, len;
                        ind = data[i].IndexOf("=") + 1;
                        len = data[i].Length - ind;
                        FormMain.MainGlobals.resolution = data[i].Substring(ind, len);
                        
                    }
                }
            }
            ThemeCarUnderline.Visible = false;
            ThemeHousesUnderline.Visible = false;
            ThemeSpaceUnderline.Visible = false;

            Resolution1366x853Underline.Visible = false;
            Resolution1600x918Underline.Visible = false;
            Resolution1280x800Underline.Visible = false;

            Speaker0Underline.Visible = false;
            Speaker1Underline.Visible = false;
            Speaker2Underline.Visible = false;
            Speaker3Underline.Visible = false;
            //MessageBox.Show(FormMain.MainGlobals.resolution + " " + FormMain.MainGlobals.volume + " " + FormMain.MainGlobals.theme + " ");
            // Ustawienie głośności na 50%
     
            setAll();
            waveOut.Init(audioFile);

            // Ustawianie głośności odtwarzania na 50%


            // Rozpoczęcie odtwarzania dźwięku

            waveOut.Play();
          //  waveOut.PlaybackStopped += (sender, eventArgs) => waveOut.Play();
       
           
        }
      
         private void Play_label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 36);

        }
        private void Play_label_MouseHover(object sender, EventArgs e)
        {

            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 31);

        }

        // ...

        // Tworzenie obiektu SoundPlayer i przypisanie ścieżki dźwięku

        AudioFileReader audioFile = new AudioFileReader("../../resources/musik.mp3");

        // Tworzenie obiektu WaveOut, który będzie odpowiadał za odtwarzanie dźwięku
        WaveOut waveOut = new WaveOut();


        public void InitResolution()
        {
            if(FormMain.MainGlobals.resolution== "1280x800")
            {
                ((FormMain)Parent).Size = new System.Drawing.Size(1280, 800);
            }
            else if(FormMain.MainGlobals.resolution == "1600x918")
            {
                ((FormMain)Parent).Size = new System.Drawing.Size(1680, 1280);
            }else if(FormMain.MainGlobals.resolution == "1366x853")
            {
                ((FormMain)Parent).Size = new System.Drawing.Size(1366, 853);
            }
        }
        private void btnBack_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("SettingsView", "MainView");
        }

        private void setAll()
        {
            if (FormMain.MainGlobals.resolution == "1366x853")
            {
                Resolution1366x853Underline.Visible = true;
                Resolution1600x918Underline.Visible = false;
                Resolution1280x800Underline.Visible = false;
            }
            else if (FormMain.MainGlobals.resolution == "1280x800")
            {
                Resolution1366x853Underline.Visible = false;
                Resolution1600x918Underline.Visible = false;
                Resolution1280x800Underline.Visible = true;
            }
            else if (FormMain.MainGlobals.resolution == "1600x918")
            {
                Resolution1366x853Underline.Visible = false;
                Resolution1600x918Underline.Visible = true;
                Resolution1280x800Underline.Visible = false;
            }
            if (FormMain.MainGlobals.volume == "0")
            {
                Speaker0Underline.Visible = true;
                Speaker1Underline.Visible = false;
                Speaker2Underline.Visible = false;
                Speaker3Underline.Visible = false;
                audioFile.Volume = 0.0f;
            }
            else if (FormMain.MainGlobals.volume == "1")
            {
                Speaker0Underline.Visible = false;
                Speaker1Underline.Visible = true;
                Speaker2Underline.Visible = false;
                Speaker3Underline.Visible = false;
                audioFile.Volume = 0.33f;
            }
            else if (FormMain.MainGlobals.volume == "2")
            {
                Speaker0Underline.Visible = false;
                Speaker1Underline.Visible = false;
                Speaker2Underline.Visible = true;
                Speaker3Underline.Visible = false;
                audioFile.Volume = 0.66f;
            }
            else if (FormMain.MainGlobals.volume == "3")
            {
                Speaker0Underline.Visible = false;
                Speaker1Underline.Visible = false;
                Speaker2Underline.Visible = false;
                Speaker3Underline.Visible = true;
                audioFile.Volume = 0.99f;
            }
            if (FormMain.MainGlobals.theme == "car")
            {
                ThemeCarUnderline.Visible = true;
                ThemeHousesUnderline.Visible = false;
                ThemeSpaceUnderline.Visible = false;
            }
            else if (FormMain.MainGlobals.theme == "space")
            {
                ThemeCarUnderline.Visible = false;
                ThemeHousesUnderline.Visible = false;
                ThemeSpaceUnderline.Visible = true;
            }
            else if (FormMain.MainGlobals.theme == "houses")
            {
                ThemeCarUnderline.Visible = false;
                ThemeHousesUnderline.Visible = true;
                ThemeSpaceUnderline.Visible = false;
            }
            UpdateConfigFile();
            FormMain.MainGlobals.nickname = textBoxNickname.Text;
        }
        private void SettingsVisible_Changed(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                setAll();
            }
        }

        private void themeSpaceItem_Clicked(object sender, EventArgs e)
        {
            ThemeCarUnderline.Visible = false;
            ThemeHousesUnderline.Visible = false;
            ThemeSpaceUnderline.Visible = true;
            FormMain.MainGlobals.theme = "space";
            UpdateConfigFile();
        }

        private void themeCarItem_Clicked(object sender, EventArgs e)
        {
            ThemeCarUnderline.Visible = true;
            ThemeHousesUnderline.Visible = false;
            ThemeSpaceUnderline.Visible = false;
            FormMain.MainGlobals.theme = "car";
            UpdateConfigFile();
        }

        private void themeHouseItem_Clicked(object sender, EventArgs e)
        {
            ThemeCarUnderline.Visible = false;
            ThemeHousesUnderline.Visible = true;
            ThemeSpaceUnderline.Visible = false;
            FormMain.MainGlobals.theme = "houses";
            UpdateConfigFile();
        }

        private void Resolution1366x853_Click(object sender, EventArgs e)
        {
            Resolution1366x853Underline.Visible = true;
            Resolution1600x918Underline.Visible = false;
            Resolution1280x800Underline.Visible = false;
            FormMain.MainGlobals.resolution = "1366x853";
            UpdateConfigFile();
            ((FormMain)Parent).Size = new System.Drawing.Size(1366, 853);
        }

        private void Resolution1280x800_Click(object sender, EventArgs e)
        {
            Resolution1366x853Underline.Visible = false;
            Resolution1600x918Underline.Visible = false;
            Resolution1280x800Underline.Visible = true;
            FormMain.MainGlobals.resolution = "1280x800";
            UpdateConfigFile();
            ((FormMain)Parent).Size = new System.Drawing.Size(1280, 800);
        }

        private void Resolution1600x918_Click(object sender, EventArgs e)
        {
            Resolution1366x853Underline.Visible = false;
            Resolution1600x918Underline.Visible = true;
            Resolution1280x800Underline.Visible = false;
            FormMain.MainGlobals.resolution = "1600x918";
            UpdateConfigFile();
            ((FormMain)Parent).Size = new System.Drawing.Size(1680, 1280);
        }

        private void Speaker0_Click(object sender, EventArgs e)
        {
            Speaker0Underline.Visible = true;
            Speaker1Underline.Visible = false;
            Speaker2Underline.Visible = false;
            Speaker3Underline.Visible = false;
            FormMain.MainGlobals.volume = "0";
            UpdateConfigFile();
            audioFile.Volume = 0.0f;
        }

        private void Speaker1_Click(object sender, EventArgs e)
        {
            Speaker0Underline.Visible = false;
            Speaker1Underline.Visible = true;
            Speaker2Underline.Visible = false;
            Speaker3Underline.Visible = false;
            FormMain.MainGlobals.volume = "1";
            UpdateConfigFile();
            audioFile.Volume = 0.33f;
        }

        private void Speaker2_Click(object sender, EventArgs e)
        {
            Speaker0Underline.Visible = false;
            Speaker1Underline.Visible = false;
            Speaker2Underline.Visible = true;
            Speaker3Underline.Visible = false;
            FormMain.MainGlobals.volume = "2";
            UpdateConfigFile();
            audioFile.Volume = 0.66f;
        }

        private void Speaker3_Click(object sender, EventArgs e)
        {
            Speaker0Underline.Visible = false;
            Speaker1Underline.Visible = false;
            Speaker2Underline.Visible = false;
            Speaker3Underline.Visible = true;
            FormMain.MainGlobals.volume = "3";
            audioFile.Volume = 0.99f;
            UpdateConfigFile();
        }
        private void UpdateConfigFile()
        {
            string[] arr = new string[4];
            arr[0] = "volume="+FormMain.MainGlobals.volume;
            arr[1] = "nickname=" + FormMain.MainGlobals.nickname;
            arr[2] = "theme=" + FormMain.MainGlobals.theme;
            arr[3] = "resolution=" + FormMain.MainGlobals.resolution;
            File.WriteAllLines(Path.Combine("config.conf"), arr);
        }
        private void textBoxNickname_TextChanged(object sender, EventArgs e)
        {
            textBoxNickname.Text =textBoxNickname.Text.Trim();
            textBoxNickname.Text = textBoxNickname.Text.Replace(";", "");
            FormMain.MainGlobals.nickname = textBoxNickname.Text;
            UpdateConfigFile();
        }

        private  void Back_Clicked(object sender, EventArgs e)
        {
            FormMain myparent = (FormMain)this.Parent;
            myparent.zmienwidok("SettingsView", "MainView");
            myparent.MSG("settings have been saved");
        }
    }
}
