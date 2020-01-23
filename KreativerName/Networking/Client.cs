using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KreativerName.Networking
{
    public delegate void PacketEvent(Client client, Packet packet);

    /// <summary>
    /// A class for a connection between the game and the server.
    /// </summary>
    public class Client
    {
        public Client(TcpClient client)
        {
            tcp = client;
        }

        TcpClient tcp;
        Thread threadRecieve;
        bool recieve = false;

        /// <summary>
        /// A event that gets invoked when a packet got recieved.
        /// </summary>
        public event PacketEvent PacketRecieved;
        public bool Connected => tcp.Connected;
        public uint UserID;
        public bool LoggedIn;

        public EndPoint LocalIP => tcp.Client.LocalEndPoint;
        public EndPoint RemoteIP => tcp.Client.RemoteEndPoint;

        /// <summary>
        /// Sends a packet.
        /// </summary>
        /// <param name="packet">The packet to be sent.</param>
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

        /// <summary>
        /// Starts a thread for recieving packets.
        /// </summary>
        public void StartRecieve()
        {
            recieve = true;
            threadRecieve = new Thread(Recieve);
            threadRecieve.Start();
        }

        /// <summary>
        /// Stops the thread for recieving packets.
        /// </summary>
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

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Disconnect()
        {
            if (tcp.Connected)
            {
                //StopRecieve();
                tcp.Close();
            }
        }
    }
}
