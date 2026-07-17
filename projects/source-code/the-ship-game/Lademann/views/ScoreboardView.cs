using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lademann.views
{
    public partial class ScoreboardView : UserControl
    {
        public ScoreboardView()
        {
            InitializeComponent();
            wypisz_zmienne();
        }

   

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("ScoreboardView", "MainView");
        }
        public void wypisz_zmienne()
        {

            wins.Text = FormMain.MainGlobals.wins;
            PlayedBattles.Text = FormMain.MainGlobals.PlayedBattles;
            DestroyedShips.Text = FormMain.MainGlobals.DestroyedShips;
            //TimeInGame.Text = FormMain.MainGlobals.TimeInGame+"  secs";
        }
      
    }
}
