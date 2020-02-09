using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KreativerName.Networking;

namespace Server
{
    partial class Program
    {
        public static KreativerName.Version version = new KreativerName.Version(0, 2, 1, 0);
        
        public static List<Client> Clients = new List<Client>();
        public static List<Room> Rooms = new List<Room>();
        static TcpListener listener;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8875);
            
            new Thread(AcceptClients).Start();
            new Thread(RemoveClients).Start();

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine();

                CommandHandler.HandleCommand(command);
            }
        }

        static void AcceptClients()
        {
            listener.Start();
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                Client client = new Client(tcpClient);
                client.PacketRecieved += ClientBytesRecieved;
                client.StartRecieve();

                Clients.Add(client);
                Console.Beep(240, 500);
            }
        }

        static void RemoveClients()
        {
            while (true)
            {
                for (int i = Clients.Count - 1; i >= 0; i--)
                {
                    if (!Clients[i].Connected)
                    {
                        Clients.RemoveAt(i);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private static void ClientBytesRecieved(Client client, Packet packet)
        {
            RequestHandler.HandleRequest(client, packet);
        }
    }
}
