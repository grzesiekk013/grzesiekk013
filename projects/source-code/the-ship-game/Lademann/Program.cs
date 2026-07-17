using Lademann.views;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lademann
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new FormMain());
            FormMain.MainGlobals.runTask = false;
            if (FormMain.MainGlobals.gamemode == "Host")
            {
                var x = new GameLobbyView();
                x.SendDataAsServer("iexit;");
                x = null;
            }
            else
            {
                var x = new GameLobbyView();
                
                x.SendDataAsClinet("iexit;");
                x = null;
            }
          
        }
    }
}
