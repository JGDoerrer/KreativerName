using System;
using System.Collections.Generic;
using System.Linq;
using KreativerName.Grid;

namespace KreativerName.Networking
{
    public struct Packet : IBytes
    {
        public Packet(byte[] bytes) : this()
        {
            FromBytes(bytes, 0);
        }

        public Packet(PacketCode code, PacketInfo info) : this()
        {
            Code = code;
            Info = info;
        }

        public Packet(PacketCode code, PacketInfo info, byte[] bytes)
        {
            Code = code;
            Info = info;
            Bytes = bytes;
        }

        public Packet(PacketCode code, byte[] bytes)
        {
            Code = code;
            Info = PacketInfo.None;
            Bytes = bytes;
        }

        public PacketCode Code;
        public PacketInfo Info;
        public byte[] Bytes;

        public World World => new World(Bytes);

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes((ushort)Code));
            bytes.Add((byte)Info);

            bytes.AddRange(BitConverter.GetBytes(Bytes?.Length ?? 0));
            bytes.AddRange(Bytes ?? new byte[0]);

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            Code = (PacketCode)BitConverter.ToUInt16(bytes, startIndex + count);
            count += 2;
            Info = (PacketInfo)bytes[startIndex + count];
            count += 1;

            int byteCount = BitConverter.ToInt32(bytes, startIndex + count);
            count += 4;
            Bytes = bytes.Skip(startIndex + count).Take(byteCount).ToArray();
            count += byteCount;

            return count;
        }
    }

    public enum PacketCode : ushort
    {
        NoOp = 0x0000,

        SignUp = 0x0100,
        LogIn = 0x0110,

        GetWorldByID = 0x0200,
        GetIDs = 0x0210,
        UploadWorld = 0x0220,
        GetWeeklyWorld = 0x0230,

        UploadStats = 0x0300,

        RecieveNotification = 0x0400,
        SendNotification = 0x0410,

        CompareVersion = 0x0500,

        Disconnect = 0xFF00,
    }

    public enum PacketInfo : byte
    {
        None = 0x00,
        New = 0x40,
        Success = 0x80,
        NotLoggedIn = 0xF0,
        Error = 0xFF,
    }
}
