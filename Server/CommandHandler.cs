using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            }
        }

        static void SendClients()
        {
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
                List<byte> bytes = new List<byte>() { 0x00, 0x04 };

                bytes.AddRange(BitConverter.GetBytes(msg.Length));
                bytes.AddRange(msg);

                client.Send(bytes.ToArray());
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
                Console.WriteLine( "  TcpClient:");
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
    }
}
