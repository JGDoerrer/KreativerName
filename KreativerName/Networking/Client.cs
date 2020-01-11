using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KreativerName.Networking
{
    public delegate void PacketEvent(Client client, Packet packet);

    public class Client
    {
        public Client(TcpClient client)
        {
            tcp = client;
        }

        TcpClient tcp;
        Thread threadRecieve;
        bool recieve = false;


        public event PacketEvent PacketRecieved;
        public bool Connected => tcp.Connected;
        public uint UserID;
        public bool LoggedIn;

        public EndPoint LocalIP => tcp.Client.LocalEndPoint;
        public EndPoint RemoteIP => tcp.Client.RemoteEndPoint;

        public void Send(Packet packet)
        {
            if (Connected)
            {
                try
                {
                    NetworkStream stream = tcp.GetStream();

                    byte[] bytes = packet.ToBytes();
                    byte[] length = BitConverter.GetBytes(bytes.Length);

                    stream.Write(length, 0, 4);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Client]: Error while sending: {e.Message}");
                }
            }
        }

        public void StartRecieve()
        {
            recieve = true;
            threadRecieve = new Thread(Recieve);
            threadRecieve.Start();
        }

        public void StopRecieve()
        {
            recieve = false;
            threadRecieve.Abort();
        }

        private void Recieve()
        {
            while (recieve)
            {
                try
                {
                    NetworkStream stream = tcp.GetStream();
                    byte[] length = new byte[4];
                    stream.Read(length, 0, 4);
                    int size = BitConverter.ToInt32(length, 0);
                    byte[] data = new byte[size];
                    stream.Read(data, 0, size);

                    PacketRecieved?.Invoke(this, new Packet(data));
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        public void Disconnect()
        {
            if (tcp.Connected)
            {
                StopRecieve();
                tcp.Close();
            }
        }
    }
}
