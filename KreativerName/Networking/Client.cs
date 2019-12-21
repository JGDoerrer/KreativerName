﻿using System;
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

        public event ByteEvent BytesRecieved;
        public bool Connected => tcp.Connected;
        public uint UserID;
        public bool LoggedIn;

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
            threadRecieve = new Thread(Recieve);
            threadRecieve.Start();
        }

        public void StopRecieve()
        {
            threadRecieve.Abort();
        }

        private void Recieve()
        {
            while (true)
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
    }
}
