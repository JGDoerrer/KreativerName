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
        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x80;

        static Random random = new Random();

        public static void HandleRequest(Client client, byte[] msg)
        {
            ushort code = BitConverter.ToUInt16(msg, 0);
            switch (code)
            {
                case 0x0100: SignUp(client, msg); break;
                case 0x0110: LogIn(client, msg); break;
                case 0x0200: GetWorldByID(client, msg); break;
                case 0x0210: GetIDs(client, msg); break;
                case 0x0220: UploadWorld(client, msg); break;
                case 0x0300: UploadStats(client, msg); break;
            }
        }

        static void SignUp(Client client, byte[] msg)
        {
            try
            {
                int index = 2;

                int nameLength = BitConverter.ToInt32(msg, index);
                index += 4;
                string name = Encoding.UTF8.GetString(msg, index, nameLength);
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

                List<byte> bytes = new List<byte>() { 0x00, 0x01, SuccessCode };
                bytes.AddRange(BitConverter.GetBytes(id));
                bytes.AddRange(BitConverter.GetBytes(loginInfo));

                client.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x00, 0x01, ErrorCode });
            }
        }

        static void LogIn(Client client, byte[] msg)
        {
            try
            {
                uint id = BitConverter.ToUInt32(msg, 2);
                uint loginInfo = BitConverter.ToUInt32(msg, 6);

                if (DataBase.GetUser(id)?.LoginInfo == loginInfo)
                {
                    client.LoggedIn = true;
                    client.UserID = id;
                }

                client.Send(new byte[] { 0x10, 0x01, SuccessCode });
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x10, 0x01, ErrorCode });
            }
        }

        static void GetWorldByID(Client client, byte[] msg)
        {
            try
            {
                uint id = BitConverter.ToUInt32(msg, 2);

                World world = DataBase.GetWorld(id).Value;

                byte[] bytes = new byte[] { 0x00, 0x02, SuccessCode };
                bytes = bytes.Concat(world.ToCompressed()).ToArray();

                client.Send(bytes);
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x00, 0x02, ErrorCode });
            }
        }

        static void GetIDs(Client client, byte[] msg)
        {
            try
            {
                int count = BitConverter.ToInt32(msg, 2);

                // Get random IDs
                List<uint> ids = DataBase.GetWorldIDs().OrderBy(x => random.Next()).Take(count).ToList();

                List<byte> bytes = new List<byte>();
                bytes.AddRange(new byte[] { 0x10, 0x02, SuccessCode });
                bytes.AddRange(BitConverter.GetBytes(ids.Count));

                foreach (uint id in ids)
                {
                    bytes.AddRange(BitConverter.GetBytes(id));
                }

                client.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x10, 0x02, ErrorCode });
            }
        }

        static void UploadWorld(Client client, byte[] msg)
        {
            try
            {
                if (!client.LoggedIn)
                    throw new Exception();

                uint id;
                do
                {
                    id = (uint)random.Next(int.MinValue, int.MaxValue);
                }
                while (DataBase.ExistsWorld(id));

                World world = World.LoadFromBytes(msg.Skip(2).ToArray());
                world.AllCompleted = false;
                world.AllPerfect = false;
                world.ID = id;
                world.Uploader = client.UserID;
                world.UploadTime = DateTime.Now;

                DataBase.AddWorld(id, world);

                List<byte> bytes = new List<byte>() { 0x20, 0x02, SuccessCode };
                bytes.AddRange(BitConverter.GetBytes(id));

                client.Send(bytes.ToArray());
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x20, 0x02, ErrorCode });
            }
        }

        static void UploadStats(Client client, byte[] msg)
        {
            try
            {
                if (!client.Connected)
                    throw new Exception();

                Stats stats = new Stats();
                stats.FromBytes(msg, 2);

                User user = DataBase.GetUser(client.UserID).Value;
                user.Statistics = stats;
                DataBase.SaveUser(user);
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x00, 0x03, ErrorCode });
            }
        }
    }
}
