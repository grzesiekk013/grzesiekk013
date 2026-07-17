using Lademann.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lademann.Play
{
    public partial class PlayView : UserControl
    {
        public PlayView()
        {
            InitializeComponent();
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("PlayView", "MainView");
        }

        private void btnPlayWithFriend_Click(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("PlayView", "GameLobbyView");
            FormMain.MainGlobals.gamemode = "Client";
        }

        private void btnPlayAsHost(object sender, EventArgs e)
        {
            FormMain.MainGlobals.gamemode = "Host";
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("", "ServerView1");
            myParent.zmienwidok("PlayView", "GameLobbyView");
           
        }
        private void Play_label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 36);

        }
        private void Play_label_MouseHover(object sender, EventArgs e)
        {

            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 31);

        }

        private void PlayWithBot_Click(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("PlayView", "Bot");
        }
    }
}
