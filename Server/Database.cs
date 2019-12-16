using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KreativerName;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    public static class DataBase
    {
        public static void Init()
        {
            Users = new Dictionary<ushort, User>();

            LoadUsers();
        }

        private const string userPath = "DataBase/users.dat";
        public static Dictionary<ushort, User> Users;

        #region Worlds

        public static World? GetWorld(uint id)
        {
            string path = $"DataBase/Worlds/{id}.wld";

            if (File.Exists(path))
            {
                return World.LoadFromFile(path, false);
            }

            return null;
        }
               
        public static bool AddWorld(uint id, World world)
        {
            string path = $"DataBase/Worlds/{id}.wld";

            if (File.Exists(path))
                return false;

            world.SaveToFile(path, false);
            return true;
        }

        public static bool ExistsWorld(uint id)
            => File.Exists($"DataBase/Worlds/{id}.wld");

        #endregion

        #region Users

        public static void SaveUsers()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Users.Count.ToBytes());

            foreach (KeyValuePair<ushort, User> user in Users)
            {
                bytes.AddRange(user.Value.ToBytes());
            }

            File.WriteAllBytes(userPath, bytes.ToArray());
        }

        public static void LoadUsers()
        {
            byte[] bytes = File.ReadAllBytes(userPath);
            int index = 0;

            int count = BitConverter.ToInt32(bytes, index);
            index += 4;

            for (int i = 0; i < count; i++)
            {
                User user = new User();
                index += user.FromBytes(bytes, index);

                Users.Add(user.ID, user);
            }
        }

        #endregion
    }
}
