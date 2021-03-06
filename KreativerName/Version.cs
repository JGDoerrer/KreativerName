﻿using System;
using System.Collections.Generic;

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
        {
            if (Major != version.Major)
                return Major > version.Major;

            if (Minor != version.Minor)
                return Minor > version.Minor;

            if (Build != version.Build)
                return Build > version.Build;

            if (Revision != version.Revision)
                return Revision > version.Revision;

            return false;
        }

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
