using System;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Networking;

namespace TestClient
{
    static class RequestHandler
    {
        public const byte ErrorCode = 0xFF;
        public const byte SuccessCode = 0x40;

        public static void HandleRequest(Client client, byte[] msg)
        {
            ushort code = BitConverter.ToUInt16(msg, 0);
            switch (code)
            {
                case 0x0100 | SuccessCode: SignUpSuccess(client, msg); break;
                case 0x0100 | ErrorCode: SignUpError(client, msg); break;

                case 0x0200 | SuccessCode: GetWorldSuccess(client, msg); break;
                case 0x0200 | ErrorCode: Console.WriteLine("Could not get world"); break;
                case 0x0200 | SuccessCode + 1: UploadWorldSuccess(client, msg); break;
                case 0x0200 | ErrorCode - 1: Console.WriteLine("Could not upload world"); break;
            }
        }

        private static void SignUpSuccess(Client client, byte[] msg)
        {
            ushort id = BitConverter.ToUInt16(msg, 2);
            Console.WriteLine($"Success; ID: {id}");
        }

        private static void SignUpError(Client client, byte[] msg)
        {
            Console.WriteLine("Error");
        }

        private static void GetWorldSuccess(Client client, byte[] msg)
        {
            World world = World.LoadFromBytes(msg.Skip(2).ToArray());
            Console.WriteLine(world.Title);
        }

        private static void UploadWorldSuccess(Client client, byte[] msg)
        {
            Console.WriteLine($"World uploaded; ID: {BitConverter.ToUInt32(msg, 2)}");
        }
    }
}
