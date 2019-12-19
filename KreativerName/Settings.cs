using System;
using System.Collections.Generic;
using System.IO;

namespace KreativerName
{
    struct Settings : IBytes
    {
        public bool ShowMoves;
        public bool Fullscreen;
        public bool ShowFps;
        public bool LoggedIn;

        public ushort UserID;
        public uint LoginInfo;

        public static Settings Current;
        public static Settings New => new Settings()
        {
            ShowMoves = true,
            Fullscreen = false,
            ShowFps = false,
            LoggedIn = false,
            UserID = 0,
            LoginInfo = 0,
        };


        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            ShowMoves = (bytes[startIndex + count] & (1 << 0)) > 0;
            Fullscreen = (bytes[startIndex + count] & (1 << 1)) > 0;
            ShowFps = (bytes[startIndex + count] & (1 << 2)) > 0;
            LoggedIn = (bytes[startIndex + count] & (1 << 3)) > 0;

            count++;

            UserID = BitConverter.ToUInt16(bytes, startIndex + count);
            count += 2;

            LoginInfo = BitConverter.ToUInt32(bytes, startIndex + count);
            count += 4;

            return count;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            byte b1 = (byte)((ShowMoves ? 1 : 0) << 0 | (Fullscreen ? 1 : 0) << 1 | (ShowFps ? 1 : 0) << 2 | (LoggedIn ? 1 : 0) << 3);
            bytes.Add(b1);

            bytes.AddRange(BitConverter.GetBytes(UserID));
            bytes.AddRange(BitConverter.GetBytes(LoginInfo));

            return bytes.ToArray();
        }

        public void SaveToFile(string name)
        {
            string path = $@"Resources\{name}.set";

            byte[] bytes = ToBytes();
            File.WriteAllBytes(path, bytes);
        }

        public static Settings LoadFromFile(string name)
        {
            Settings settings = New;
            string path = $@"Resources\{name}.set";

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                settings.FromBytes(bytes, 0);
            }

            return settings;
        }
    }
}
