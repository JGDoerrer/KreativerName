using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server;

namespace TestClient
{
    class Program
    {
        static Client client;

        static void Main(string[] args)
        {
            TcpClient tcp = new TcpClient();
            tcp.Connect("Josuas-Pc", 8875);

            client = new Client(tcp);
            client.BytesRecieved += BytesRecieved;
            new Thread(client.Recieve).Start();

            Console.Beep();
        }

        private static void BytesRecieved(byte[] bytes)
        {
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
        }
    }
}
