using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Networking;

namespace Server
{
    static class CommandHandler
    {
        public static void HandleCommand(string command)
        {
            switch (command)
            {
                case "clients": PrintClients(); break;
                case "users": PrintUsers(); break;
                case "users save": DataBase.SaveUsers(); break;
            }
        }

        static void PrintClients()
        {
            int count = 0;
            foreach (Client client in Program.clients)
            {
                Console.WriteLine($"{++count}: Logged in: {client.LoggedIn}; ID: {client.UserID.ToString("X")}; Login Info: {DataBase.Users[client.UserID].LoginInfo.ToString("X")}");
            }
        }

        static void PrintUsers()
        {
            int count = 0;
            foreach (KeyValuePair<ushort, User> user in DataBase.Users)
            {
                Console.WriteLine($"{++count}: {user.Value.Name}; ID: {user.Value.ID.ToString("X")}; LoginInfo: {user.Value.LoginInfo.ToString("X")}");
            }
        }
    }
}
