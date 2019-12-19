using System;
using System.Collections.Generic;
using System.Text;

namespace KreativerName.Networking
{
    public struct User : IBytes
    {
        public User(string name, ushort id)
        {
            Name = name;
            ID = id;
            LoginInfo = 0;
        }
        public User(string name, ushort id, uint loginInfo)
        {
            Name = name;
            ID = id;
            LoginInfo = loginInfo;
        }

        public string Name;
        public ushort ID;
        public uint LoginInfo;

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            byte[] name = Encoding.UTF8.GetBytes(Name);
            bytes.AddRange(name.Length.ToBytes());
            bytes.AddRange(name);

            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(LoginInfo));

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            int nameLength = 0;
            count += nameLength.FromBytes(bytes, startIndex + count);
            Name = Encoding.UTF8.GetString(bytes, startIndex + count, nameLength);
            count += nameLength;

            ID = BitConverter.ToUInt16(bytes, startIndex + count);
            count += 2;

            LoginInfo = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;

            return count;
        }

        public override string ToString() => $"Name: {Name}; ID: {ID}";
    }
}
