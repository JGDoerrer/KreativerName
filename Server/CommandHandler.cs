using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KreativerName;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    static class CommandHandler
    {
        public static void HandleCommand(string command)
        {
            string[] args = command.Split(' ');

            switch (args[0])
            {
                case "clients":
                {
                    if (args.Length == 1)
                    {
                        PrintClients();
                        break;
                    }
                    if (args[1] == "send")
                    {
                        SendClients();
                    }
                    break;
                }
                case "users":
                {
                    if (args.Length == 1)
                    {
                        PrintUsers();
                        break;
                    }
                    if (args[1] == "resave")
                    {
                        DataBase.ReSaveUsers();
                    }
                    break;
                }
                case "worlds":
                {
                    if (args.Length == 1)
                    {
                        PrintWorlds();
                        break;
                    }
                    break;
                }
                case "version":
                {
                    if (args.Length == 1)
                    {
                        Console.WriteLine($"Current Version: {Program.version}");
                        break;
                    }
                    if (args[1] == "set")
                    {
                        SetVersion();
                        break;
                    }
                    break;
                }
                case "stats":
                {
                    if (args.Length == 1)
                    {
                        AnalyseStats();
                        break;
                    }
                    break;
                }
                case "packets":
                {
                    RequestHandler.PrintPackets = !RequestHandler.PrintPackets;
                    Console.WriteLine($"{(RequestHandler.PrintPackets ? "Showing" : "Not showing")} packets.");
                    break;
                }

                default:
                    Console.WriteLine("Command not found.");
                    break;
            }
        }

        static void SendClients()
        {
            Console.Write("Size: ");
            float size;

            while (!float.TryParse(Console.ReadLine(), out size)) ;

            Console.WriteLine("Message: ");
            string s = "";

            for (string line; (line = Console.ReadLine()) != "";)
            {
                s += line + "\n";
            }

            s = s.TrimEnd();


            byte[] msg = Encoding.UTF8.GetBytes(s);

            foreach (var client in Program.clients)
            {
                List<byte> bytes = new List<byte>();

                bytes.AddRange(BitConverter.GetBytes(size));
                bytes.AddRange(BitConverter.GetBytes(msg.Length));
                bytes.AddRange(msg);

                client.Send(new Packet(PacketCode.RecieveNotification, bytes.ToArray()));
            }
        }

        static void SetVersion()
        {
            try
            {
                Console.Write("Major: ");
                uint mayor = uint.Parse(Console.ReadLine());
                Console.Write("Minor: ");
                uint minor = uint.Parse(Console.ReadLine());
                Console.Write("Build: ");
                uint build = uint.Parse(Console.ReadLine());
                Console.Write("Revision: ");
                uint revision = uint.Parse(Console.ReadLine());

                Program.version = new KreativerName.Version(mayor, minor, build, revision);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }

        static void PrintClients()
        {
            int count = 0;
            foreach (Client client in Program.clients)
            {
                Console.WriteLine($"Client {++count}:");

                Console.WriteLine($"  ID:        {client.UserID}");
                Console.WriteLine($"  LoggedIn:  {client.LoggedIn}");
                Console.WriteLine($"  Connected: {client.Connected}");
                Console.WriteLine("  TcpClient:");
                Console.WriteLine($"    LocalIP:  {client.LocalIP}");
                Console.WriteLine($"    RemoteIP: {client.RemoteIP}");
            }
        }

        static void PrintUsers()
        {
            int count = 0;
            foreach (User user in DataBase.GetUsers())
            {
                Console.WriteLine($"User {++count}:");
                Console.WriteLine($"  ID:        {user.ID.ToID().ToUpper()}");
                Console.WriteLine($"  Name:      {user.Name}");
                Console.WriteLine($"  LoginInfo: {user.LoginInfo.ToID().ToUpper()}");
                Console.WriteLine("  Statistics:");

                foreach (var property in user.Statistics.GetType().GetProperties())
                {
                    string value = property.GetValue(user.Statistics).ToString();
                    value = value.PadLeft(30 - property.Name.Length + value.Length);
                    Console.WriteLine($"    {property.Name}: {value}");
                }
            }
        }

        static void PrintWorlds()
        {
            int count = 0;
            foreach (uint id in DataBase.GetWorldIDs())
            {
                World world = DataBase.GetWorld(id) ?? new World();

                Console.WriteLine($"World {++count}:");
                Console.WriteLine($"  ID:         {world.ID.ToID().ToUpper()}");
                Console.WriteLine($"  Uploader:   {world.Uploader.ToID().ToUpper()}");
                Console.WriteLine($"  UploadTime: {world.UploadTime}");
                Console.WriteLine($"  Levels:     {world.Levels.Count}");
                Console.WriteLine($"  Title:      {world.Title}");
            }
        }

        static void AnalyseStats()
        {
            List<User> users = DataBase.GetUsers();

            Console.WriteLine($"Total Statistics: {users.Count}");

            PropertyInfo[] properties = typeof(Stats).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object max = users.Max(x => property.GetValue(x.Statistics));
                object min = users.Min(x => property.GetValue(x.Statistics));

                Console.WriteLine($"{property.Name}:");
                Console.WriteLine($"  Highest: {max}");

                if (property.PropertyType == typeof(uint))
                {
                    object avg = users.Average(x => (uint)property.GetValue(x.Statistics));
                    Console.WriteLine($"  Average: {avg}");
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    double avg = users.Average(x => ((DateTime)property.GetValue(x.Statistics)).Ticks);
                    Console.WriteLine($"  Average: {new DateTime((long)avg)}");
                }
                else if (property.PropertyType == typeof(TimeSpan))
                {
                    double avg = users.Average(x => ((TimeSpan)property.GetValue(x.Statistics)).Ticks);
                    Console.WriteLine($"  Average: {new TimeSpan((long)avg)}");
                }

                Console.WriteLine($"  Lowest:  {min}");
            }
        }
    }
}
