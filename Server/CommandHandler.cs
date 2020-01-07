using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using KreativerName;
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
                    switch (args[1])
                    {
                        case "send":
                            SendClients();
                            break;
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
                    switch (args[1])
                    {
                        case "resave": DataBase.ReSaveUsers(); break;
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
                    switch (args[1])
                    {
                        case "set":
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

            for (string line = ""; (line = Console.ReadLine()) != "";)
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
                Console.WriteLine($"  ID:        {user.ID.ToID()}");
                Console.WriteLine($"  Name:      {user.Name}");
                Console.WriteLine($"  LoginInfo: {user.LoginInfo.ToString("X")}");
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
                Console.WriteLine($"World {++count}:");
            }
        }

        static void AnalyseStats()
        {
            List<User> users = DataBase.GetUsers();

            Console.WriteLine($"Total Statistics: {users.Count}");

            PropertyInfo[] properties = typeof(Stats).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                User highest = users[0];
                User lowest = users[0];

                foreach (User user in users)
                {
                    object v = property.GetValue(user.Statistics);
                    object vHighest = property.GetValue(highest.Statistics);
                    object vLowest = property.GetValue(lowest.Statistics);



                    if (Comparer<object>.Default.Compare(v, vHighest) > 0)
                        highest = user;
                    if (Comparer<object>.Default.Compare(v, vLowest) < 0)
                        lowest = user;
                }

                Console.WriteLine($"{property.Name}:");
                Console.WriteLine($"  Best:  {highest.ID.ToID()} with {property.GetValue(highest.Statistics)}");
                Console.WriteLine($"  Worst: {lowest.ID.ToID()} with {property.GetValue(lowest.Statistics)}");
            }
        }
    }
}
