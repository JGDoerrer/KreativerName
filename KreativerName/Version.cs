using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public struct Version : IBytes
    {
        public Version(uint major)
        {
            Major = major;
            Minor = 0;
            Build = 0;
            Revision = 0;
        }
        public Version(uint major, uint minor)
        {
            Major = major;
            Minor = minor;
            Build = 0;
            Revision = 0;
        }
        public Version(uint major, uint minor, uint build)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = 0;
        }
        public Version(uint major, uint minor, uint build, uint revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public uint Major { get; set; }
        public uint Minor { get; set; }
        public uint Build { get; set; }
        public uint Revision { get; set; }

        public bool IsBiggerThan(Version version)
            => Major >= version.Major && Minor >= version.Minor && Build >= version.Build && Revision > version.Revision;
        
        public override string ToString() => $"{Major}.{Minor}.{Build}.{Revision}";

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            Major = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;
            Minor = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;
            Build = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;
            Revision = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(Major));
            bytes.AddRange(BitConverter.GetBytes(Minor));
            bytes.AddRange(BitConverter.GetBytes(Build));
            bytes.AddRange(BitConverter.GetBytes(Revision));

            return bytes.ToArray();
        }
    }
}
