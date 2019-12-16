using System;
using System.Linq;
using System.Text;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    static class RequestHandler
    {
        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x40;

        static Random random = new Random();

        public static void HandleRequest(Client client, byte[] msg)
        {
            ushort code = BitConverter.ToUInt16(msg, 0);
            switch (code)
            {
                case 0x0100: SignUp(client, msg); break;
                case 0x0200: GetWorldByID(client, msg); break;
                case 0x0210: UploadWorld(client, msg); break;
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

                User user = new User(name, id);
                DataBase.Users.Add(id, user);

                Console.WriteLine($"New User: {user}");

                byte[] bytes = new byte[] { SuccessCode, 0x01 };
                bytes = bytes.Concat(BitConverter.GetBytes(id)).ToArray();
                client.Send(bytes);
            }
            catch (Exception)
            {
                client.Send(new byte[] { ErrorCode, 0x01 });
            }
        }

        private static void GetWorldByID(Client client, byte[] msg)
        {
            try
            {
                uint id = BitConverter.ToUInt32(msg, 2);

                World world = DataBase.GetWorld(id).Value;

                byte[] bytes = new byte[] { SuccessCode, 0x02 };
                bytes = bytes.Concat(world.ToCompressed()).ToArray();

                client.Send(bytes);
            }
            catch (Exception)
            {
                client.Send(new byte[] { ErrorCode, 0x02 });
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

                byte[] bytes = new byte[] { SuccessCode + 1, 0x02 };
                bytes = bytes.Concat(BitConverter.GetBytes(id)).ToArray();
                client.Send(bytes);
            }
            catch (Exception)
            {
                client.Send(new byte[] { ErrorCode - 1, 0x02 });
            }
        }
    }
}
