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
    public partial class CreditsView : UserControl
    {
        public CreditsView()
        {
            InitializeComponent();
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            FormMain myParent = (FormMain)this.Parent;
            myParent.zmienwidok("CreditsView", "MainView");
        }
        private void Play_label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 36);

        }
        private void Play_label_MouseHover(object sender, EventArgs e)
        {

            ((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 31);

        }
    }
}
