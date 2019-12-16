using System;
using KreativerName.Grid;

namespace KreativerName
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            new MainWindow().Run();

            Stats.Current.SaveToFile("statistics");
            Settings.Current.SaveToFile("settings");
        }
    }
}
