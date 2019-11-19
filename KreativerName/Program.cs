using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KreativerName.Rendering;

namespace KreativerName
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            mainWindow = new MainWindow();
            mainWindow.Run();
        }

        static MainWindow mainWindow;
    }
}
