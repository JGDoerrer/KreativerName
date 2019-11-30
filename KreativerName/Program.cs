using System;

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
            Stats.Current = Stats.LoadFromFile("statistics");
            Settings.Current = Settings.LoadFromFile("settings");

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-f":
                        Settings.Current.Fullscreen = true;
                        break;
                }
            }

            new MainWindow().Run();

            Stats.Current.SaveToFile("statistics");
            Settings.Current.SaveToFile("settings");
        }
    }
}
