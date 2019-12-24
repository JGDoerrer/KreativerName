using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KreativerName.Networking
{
    public delegate void ByteEvent(Client client, byte[] bytes);

    public class Client
    {
        public Client(TcpClient client)
        {
            tcp = client;
        }

        TcpClient tcp;
        Thread threadRecieve;
        bool recieve = false;


        public event ByteEvent BytesRecieved;
        public bool Connected => tcp.Connected;
        public uint UserID;
        public bool LoggedIn;

        public EndPoint LocalIP => tcp.Client.LocalEndPoint;
        public EndPoint RemoteIP => tcp.Client.RemoteEndPoint;
        
        public void Send(byte[] bytes)
        {
            if (Connected)
            {
                try
                {
                    NetworkStream stream = tcp.GetStream();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                catch
                { }
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

                    BytesRecieved?.Invoke(this, data);
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
                Send(new byte[] { 0x00, 0xFF});
                tcp.Close();
            }
        }
    }
}
