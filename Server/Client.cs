using System;
using System.Net.Sockets;

namespace Server
{
    public delegate void ByteEvent(byte[] bytes);

    public class Client
    {
        public Client(TcpClient client)
        {
            tcp = client;
        }

        TcpClient tcp;

        public event ByteEvent BytesRecieved;

        public bool Connected => tcp.Connected;

        public void Send(byte[] bytes)
        {
            if (Connected)
            {
                NetworkStream stream = tcp.GetStream();
                stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public void Recieve()
        {
            while (true)
            {
                NetworkStream stream = tcp.GetStream();
                byte[] length = new byte[4];
                stream.Read(length, 0, 4);
                int size = BitConverter.ToInt32(length, 0);
                byte[] data = new byte[size];
                stream.Read(data, 0, size);

                BytesRecieved?.Invoke(data);
            }
        }
    }
}
