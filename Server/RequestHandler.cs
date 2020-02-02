using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KreativerName;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    static class RequestHandler
    {
        static Random random = new Random();

        public static bool PrintPackets = false;

        public static void HandleRequest(Client client, Packet p)
        {
            if (PrintPackets)
                Console.WriteLine($"[Packet from {client.RemoteIP}]: {p.ToString()}");

            switch (p.Code)
            {
                case PacketCode.SignUp: SignUp(client, p); break;
                case PacketCode.LogIn: LogIn(client, p); break;
                case PacketCode.GetWorldByID: GetWorldByID(client, p); break;
                case PacketCode.GetIDs: GetIDs(client, p); break;
                case PacketCode.UploadWorld: UploadWorld(client, p); break;
                case PacketCode.GetWeeklyWorld: GetWeeklyWorld(client, p); break;
                case PacketCode.UploadStats: UploadStats(client, p); break;
                case PacketCode.SendNotification: Message(client, p); break;
                case PacketCode.CompareVersion: CompareVersion(client, p); break;
                case PacketCode.Disconnect: Disconnect(client, p); break;
            }
        }

        static void SignUp(Client client, Packet msg)
        {
            try
            {
                int index = 0;

                int nameLength = BitConverter.ToInt32(msg.Bytes, index);
                index += 4;
                string name = Encoding.UTF8.GetString(msg.Bytes, index, nameLength);
                index += nameLength;

                uint id;
                do
                {
                    id = (uint)random.Next(int.MinValue, int.MaxValue);
                }
                while (DataBase.ExistsUser(id));

                uint loginInfo = (uint)random.Next(int.MinValue, int.MaxValue);

                User user = new User(name, id, loginInfo);
                DataBase.SaveUser(user);

                client.LoggedIn = true;
                client.UserID = id;

                List<byte> bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(id));
                bytes.AddRange(BitConverter.GetBytes(loginInfo));

                client.Send(new Packet(PacketCode.SignUp, PacketInfo.Success, bytes.ToArray()));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.SignUp, PacketInfo.Error));
            }
        }

        static void LogIn(Client client, Packet msg)
        {
            try
            {
                uint id = BitConverter.ToUInt32(msg.Bytes, 0);
                uint loginInfo = BitConverter.ToUInt32(msg.Bytes, 4);

                if (DataBase.GetUser(id)?.LoginInfo == loginInfo)
                {
                    client.LoggedIn = true;
                    client.UserID = id;
                }

                client.Send(new Packet(PacketCode.LogIn, PacketInfo.Success));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.LogIn, PacketInfo.Error));
            }
        }

        static void GetWorldByID(Client client, Packet msg)
        {
            try
            {
                uint id = BitConverter.ToUInt32(msg.Bytes, 0);

                World world = DataBase.GetWorld(id).Value;
                
                client.Send(new Packet(PacketCode.GetWorldByID, PacketInfo.Success, world.ToCompressed()));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.GetWorldByID, PacketInfo.Error));
            }
        }

        static void GetIDs(Client client, Packet msg)
        {
            try
            {
                int count = BitConverter.ToInt32(msg.Bytes, 0);

                // Get random IDs
                List<uint> ids = DataBase.GetWorldIDs().OrderBy(x => random.Next()).Take(count).ToList();

                List<byte> bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(ids.Count));

                foreach (uint id in ids)
                {
                    bytes.AddRange(BitConverter.GetBytes(id));
                }

                client.Send(new Packet(PacketCode.GetIDs, PacketInfo.Success, bytes.ToArray()));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.GetIDs, PacketInfo.Error));
            }
        }

        static void UploadWorld(Client client, Packet msg)
        {
            try
            {
                if (!client.LoggedIn)
                {
                    client.Send(new Packet(PacketCode.UploadWorld, PacketInfo.NotLoggedIn));
                    return;
                }

                uint id;
                do
                {
                    id = (uint)random.Next(int.MinValue, int.MaxValue);
                }
                while (DataBase.ExistsWorld(id));

                World world = msg.World;
                world.AllCompleted = false;
                world.AllPerfect = false;
                world.ID = id;
                world.Uploader = client.UserID;
                world.UploadTime = DateTime.Now;

                DataBase.AddWorld(id, world);
                
                client.Send(new Packet(PacketCode.UploadWorld, PacketInfo.Success, BitConverter.GetBytes(id)));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.UploadWorld, PacketInfo.Error));
            }
        }

        static void GetWeeklyWorld(Client client, Packet msg)
        {
            try
            {
                int week = DateTime.Now.Subtract(new DateTime(2020, 1, 1, 0, 0, 0)).Days / 7;

                World world = DataBase.GetWeekly(week).Value;
                
                client.Send(new Packet(PacketCode.GetWeeklyWorld, PacketInfo.Success, world.ToCompressed()));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.GetWeeklyWorld, PacketInfo.Error));
            }
        }

        static void UploadStats(Client client, Packet msg)
        {
            try
            {
                if (!client.Connected)
                    throw new Exception();

                Stats stats = new Stats();
                stats.FromBytes(msg.Bytes, 0);

                User user = DataBase.GetUser(client.UserID).Value;

                // If new stats are newer, update
                if (stats.LastUpdated > user.Statistics.LastUpdated)
                    user.Statistics = stats;

                DataBase.SaveUser(user);
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.UploadStats, PacketInfo.Error));
            }
        }

        static void Message(Client client, Packet msg)
        {
            try
            {
                int length = BitConverter.ToInt32(msg.Bytes, 0);
                string s = Encoding.UTF8.GetString(msg.Bytes, 4, length);
                
                client.Send(new Packet(PacketCode.SendNotification, PacketInfo.Success));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.SendNotification, PacketInfo.Error));
            }
        }

        static void CompareVersion(Client client, Packet msg)
        {
            try
            {
                KreativerName.Version version = new KreativerName.Version();
                version.FromBytes(msg.Bytes, 0);

                if (Program.version.IsBiggerThan(version))
                    client.Send(new Packet(PacketCode.CompareVersion, PacketInfo.New));
                else
                    client.Send(new Packet(PacketCode.CompareVersion, PacketInfo.Success));
            }
            catch (Exception)
            {
                client.Send(new Packet(PacketCode.CompareVersion, PacketInfo.Error));
            }
        }

        static void Disconnect(Client client, Packet msg)
        {
            client.Disconnect();
            Program.clients.Remove(client);
        }
    }
}
