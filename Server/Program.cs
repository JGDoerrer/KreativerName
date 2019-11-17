using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static List<Client> clients = new List<Client>();
        static TcpListener listener;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8875);

            new Thread(AcceptClients).Start();

            while (true)
            {
                if (clients.Count > 0)
                {
                    Console.Write("> ");
                    string msg = Console.ReadLine();
                    clients.ForEach(x => x.Send(Encoding.UTF8.GetBytes(msg)));
                }

                for (int i = clients.Count - 1; i >= 0; i--)
                {
                    if (!clients[i].Connected)
                    {
                        clients.RemoveAt(i);
                    }
                }
            }
        }

        static void AcceptClients()
        {
            listener.Start();
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                Client client = new Client(tcpClient);
                
                clients.Add(client);
            }
        }
    }
}
