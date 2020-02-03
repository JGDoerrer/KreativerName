using System;
using System.Net.Sockets;
using System.Text;
using KreativerName.UI;

namespace KreativerName.Networking
{
    public static class ClientManager
    {
        static Client client;

        public static bool Connected => client?.Connected == true;

        /// <summary>
        /// Gets invoked when a packet from the server is recieved.
        /// </summary>
        public static event PacketEvent PacketRecieved;

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <returns>Returns true when successfully connected, false if not.</returns>
        public static bool Connect()
        {
            try
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect("Josuas-Pc", 8875);

                client = new Client(tcp);
                client.StartRecieve();

                client.PacketRecieved -= HandleRequest;
                client.PacketRecieved += HandleRequest;

                Console.WriteLine($"[Client]: Connected to server");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Client]: Error while connecting: {e.Message}");

                return false;
            }
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet">The packet to be sent.</param>
        public static void Send(Packet packet)
        {
            client?.Send(packet);
        }

        /// <summary>
        /// Logs in.
        /// </summary>
        public static void Login()
        {
            if (client == null)
                return;
            if (!Settings.Current.LoggedIn)
                return;

            byte[] msg = new byte[8];
            BitConverter.GetBytes(Settings.Current.UserID).CopyTo(msg, 0);
            BitConverter.GetBytes(Settings.Current.LoginInfo).CopyTo(msg, 4);

            void handle(Client c, Packet p)
            {
                if (p.Code == PacketCode.LogIn && p.Info == PacketInfo.Success)
                {
                    Notification.Show($"Eingeloggt unter {Settings.Current.UserName} ({Settings.Current.UserID.ToID()})");
                    client.PacketRecieved -= handle;
                }
            }
            client.PacketRecieved += handle;

            client.Send(new Packet(PacketCode.LogIn, PacketInfo.None, msg));
        }

        /// <summary>
        /// Compares the version of the server to the current one.
        /// </summary>
        public static void CompareVersion()
        {
            if (client == null)
                return;

            static void handle(Client client, Packet p)
            {
                if (p.Code == PacketCode.CompareVersion)
                {
                    if (p.Info == PacketInfo.New)
                        Notification.Show("Eine neue Version ist verfügbar!\nhttps://jgdoerrer.github.io/KreativerName/");
                    else if (p.Info == PacketInfo.Error)
                        Notification.Show("Fehler beim Überprüfen der Version");

                    client.PacketRecieved -= handle;
                }
            }

            client.PacketRecieved += handle;

            client.Send(new Packet(PacketCode.CompareVersion, PacketInfo.None, MainWindow.version.ToBytes()));
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public static void Disconnect()
        {
            if (client == null)
                return;

            client.Send(new Packet(PacketCode.Disconnect, PacketInfo.None));
            client.StopRecieve();
            client.Disconnect();
        }

        private static void HandleRequest(Client client, Packet msg)
        {
            PacketRecieved?.Invoke(client, msg);

            switch (msg.Code)
            {
                case PacketCode.RecieveNotification:
                    float size = BitConverter.ToSingle(msg.Bytes, 0);
                    int sLength = BitConverter.ToInt32(msg.Bytes, 4);
                    string s = Encoding.UTF8.GetString(msg.Bytes, 8, sLength);

                    Notification.Show(s, size);
                    break;
            }
        }
    }
}
