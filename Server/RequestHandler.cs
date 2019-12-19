using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                case 0x0210: GetWorldByID(client, msg); break;
                case 0x0220: UploadWorld(client, msg); break;
            }
        }

        private static void SignUp(Client client, byte[] msg)
        {
            try
            {
                int index = 2;

                int nameLength = BitConverter.ToInt32(msg, index);
                index += 4;
                string name = Encoding.UTF8.GetString(msg, index, nameLength);
                index += nameLength;

                ushort id;
                do
                {
                    id = (ushort)random.Next(ushort.MinValue, ushort.MaxValue + 1);
                }
                while (DataBase.Users.ContainsKey(id));

                uint loginInfo = (uint)random.Next(int.MinValue, int.MaxValue);

                User user = new User(name, id, loginInfo);
                DataBase.Users.Add(id, user);

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

        private static void LogIn(Client client, byte[] msg)
        {
            try
            {
                ushort id = BitConverter.ToUInt16(msg, 2);
                uint loginInfo = BitConverter.ToUInt32(msg, 4);

                if (DataBase.Users[id].LoginInfo == loginInfo)
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

        private static void GetWorldByID(Client client, byte[] msg)
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

        private static void UploadWorld(Client client, byte[] msg)
        {
            try
            {
                uint id;
                do
                {
                    id = (uint)random.Next(int.MinValue, int.MaxValue);
                }
                while (DataBase.ExistsWorld(id));

                World world = World.LoadFromBytes(msg.Skip(2).ToArray());

                DataBase.AddWorld(id, world);

                byte[] bytes = new byte[] { 0x10, 0x02, SuccessCode };
                bytes = bytes.Concat(BitConverter.GetBytes(id)).ToArray();
                client.Send(bytes);
            }
            catch (Exception)
            {
                client.Send(new byte[] { 0x10, 0x02, ErrorCode });
            }
        }
    }
}
