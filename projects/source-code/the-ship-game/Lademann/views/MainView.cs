using Lademann.Play;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lademann.views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
           
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Application.Exit();
        }

      

        private void btnPlay_clicked(object sender, EventArgs e)
        {
            
        }

        private void btnScoreboard_clicked(object sender, EventArgs e)
        {
            Console.WriteLine("sb clicked");
        }

        private void btnSettings_clicked(object sender, EventArgs e)
        {

        }

        private void btnCredits_clicked(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

    
 

        private void btnSettings_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("MainView", "SettingsView");
        }

        private void btnScoreboard_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("MainView", "ScoreboardView");
        }



        private void Play_label_MouseDown(object sender, MouseEventArgs e)
        {
            Play_label.ForeColor = Color.Green;
            Play_label.Font= new Font(Play_label.Font.FontFamily, 35);

        }

        private void Play_label_MouseUp(object sender, MouseEventArgs e)
        {
            Play_label.ForeColor = Color.Lime;
            Play_label.Font = new Font(Play_label.Font.FontFamily, 40);
        }

        private void Scoreboard_Label_MouseDown(object sender, MouseEventArgs e)
        {
            Scoreboard_Label.ForeColor= Color.Gold;
            Scoreboard_Label.Font = new Font(Scoreboard_Label.Font.FontFamily, 35);
        }

        private void Scoreboard_Label_MouseUp(object sender, MouseEventArgs e)
        {
            Scoreboard_Label.ForeColor = Color.Yellow;
            Scoreboard_Label.Font = new Font(Scoreboard_Label.Font.FontFamily, 40);
        }

        private void Credits_Label_MouseDown(object sender, MouseEventArgs e)
        {
            Credits_Label.ForeColor = Color.Purple;
            Credits_Label.Font = new Font(Credits_Label.Font.FontFamily, 35);
        }

        private void Credits_Label_MouseUp(object sender, MouseEventArgs e)
        {
            Credits_Label.ForeColor = Color.Violet;
            Credits_Label.Font = new Font(Credits_Label.Font.FontFamily, 40);
        }

        private void Settings_Label_MouseDown(object sender, MouseEventArgs e)
        {
            Settings_Label.ForeColor = Color.LightSteelBlue;
            Settings_Label.Font = new Font(Credits_Label.Font.FontFamily, 35);
        }

        private void Settings_Label_MouseUp(object sender, MouseEventArgs e)
        {
            Settings_Label.ForeColor = Color.LightSteelBlue;
            Settings_Label.Font = new Font(Credits_Label.Font.FontFamily, 40);
        }

        private void Exit_Label_MouseDown(object sender, MouseEventArgs e)
        {
            Exit_Label.ForeColor = Color.Maroon;
            Exit_Label.Font = new Font(Credits_Label.Font.FontFamily, 35);
        }

        private void Exit_Label_MouseUp(object sender, MouseEventArgs e)
        {
            Exit_Label.ForeColor = Color.Crimson;
            Exit_Label.Font = new Font(Credits_Label.Font.FontFamily, 40);
        }

        private void btnCredits_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("MainView", "CreditsView");
        }

        private void Play_label_MouseHover(object sender, EventArgs e)
        {
            
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 35);

        }

        private void Play_label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 40);

        }

        private void Play_label_Click(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("MainView", "PlayView");
        }
    }
}
