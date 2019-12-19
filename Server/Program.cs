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
        public static List<Client> clients = new List<Client>();
        static TcpListener listener;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8875);

            DataBase.Init();

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
                client.BytesRecieved += ClientBytesRecieved;
                new Thread(client.StartRecieve).Start();

                clients.Add(client);
            }
        }

        static void RemoveClients()
        {
            while (true)
            {
                for (int i = clients.Count - 1; i >= 0; i--)
                {
                    if (!clients[i].Connected)
                    {
                        clients.RemoveAt(i);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private static void ClientBytesRecieved(Client client, byte[] bytes)
        {
            RequestHandler.HandleRequest(client, bytes);
        }
    }
}
