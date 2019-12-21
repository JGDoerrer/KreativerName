using System.Collections.Generic;
using System.IO;
using System.Linq;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    public static class DataBase
    {
        const string userPath = "DataBase/Users";
        const string worldPath = "DataBase/Worlds";

        #region Worlds

        public static World? GetWorld(uint id)
        {
            string path = $"{worldPath}/{id.ToString("X")}.wld";

            if (File.Exists(path))
            {
                return World.LoadFromFile(path, false);
            }

            return null;
        }

        public static bool AddWorld(uint id, World world)
        {
            string path = $"{worldPath}/{id.ToString("X")}.wld";

            if (File.Exists(path))
                return false;

            world.SaveToFile(path, false);
            return true;
        }

        public static List<uint> GetWorldIDs()
        {
            string[] files = Directory.GetFiles(worldPath);

            return files.Select(x => uint.Parse(Path.GetFileNameWithoutExtension(x), System.Globalization.NumberStyles.HexNumber)).ToList();
        }

        public static List<World> GetWorlds()
        {
            string[] files = Directory.GetFiles(worldPath);

            return files.Select(x => World.LoadFromFile(x)).ToList();
        }

        public static bool ExistsWorld(uint id)
            => File.Exists($"{worldPath}/{id.ToString("X")}.wld");

        #endregion

        #region Users

        public static void SaveUser(User user)
        {
            string path = $"{userPath}/{user.ID.ToString("X")}.user";

            File.WriteAllBytes(path, user.ToBytes());
        }

        public static User? GetUser(uint id)
        {
            string path = $"{userPath}/{id.ToString("X")}.user";

            if (File.Exists(path))
            {
                User user = new User();
                user.FromBytes(File.ReadAllBytes(path), 0);
                return user;
            }

            return null;
        }

        public static List<User> GetUsers()
        {
            string[] files = Directory.GetFiles(userPath);

            return files.Select(x =>
            {
                User user = new User(); 
                user.FromBytes(File.ReadAllBytes(x), 0);
                return user;
            }).ToList();
        }

        public static bool ExistsUser(uint id)
            => File.Exists($"{userPath}/{id.ToString("X")}.user");

        #endregion
    }
}
