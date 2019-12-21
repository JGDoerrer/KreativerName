﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KreativerName.Networking;

namespace Server
{
    static class CommandHandler
    {
        public static void HandleCommand(string command)
        {
            string[] args = command.Split(' ');

            switch (args[0])
            {
                case "clients":
                {
                    if (args.Length == 1)
                    {
                        PrintClients();
                        break;
                    }
                    break;
                }
                case "users":
                {
                    if (args.Length == 1)
                    {
                        PrintUsers();
                        break;
                    }
                    break;
                }
                case "worlds":
                {
                    if (args.Length == 1)
                    {
                        PrintWorlds();
                        break;
                    }
                    break;
                }
            }
        }

        static void PrintClients()
        {
            int count = 0;
            foreach (Client client in Program.clients)
            {
                Console.WriteLine($"Clients {++count}:");
                PrintObject(client);
            }
        }

        static void PrintUsers()
        {
            int count = 0;
            foreach (User user in DataBase.GetUsers())
            {
                Console.WriteLine($"User {++count}:");
                PrintObject(user);
            }
        }

        static void PrintWorlds()
        {
            int count = 0;
            foreach (uint id in DataBase.GetWorldIDs())
            {
                Console.WriteLine($"World {++count}:");
                PrintObject(DataBase.GetWorld(id));
            }
        }
               
        private static void PrintObject(object obj, int layer = 0)
        {
            Type type = obj.GetType();
            PropertyInfo[] fields = type.GetProperties();
            string padding = new string(' ', (layer + 1) * 2);

            if (fields.Length > 0)
            {
                int maxLength = fields.Max(x => x.Name.Length) + 1;

                foreach (var field in fields)
                {
                    object value = field.GetValue(obj);
                    switch (value)
                    {
                        case string _:
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)} {value as string}");
                            break;
                        case byte[] _:
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)} {BitConverter.ToString(value as byte[])}");
                            break;
                        case uint _:
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)} {(value as uint?)?.ToString("X")}");
                            break;
                        case int _:
                        case DateTime _:
                        case TimeSpan _:
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)} {value.ToString()}");
                            break;
                        case ICollection _:
                        {
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)}");

                            if ((value as ICollection).Count > 0)
                            {
                                int counter = 0;
                                foreach (var item in value as ICollection)
                                {
                                    Console.WriteLine($"{padding}  {counter++}:");
                                    PrintObject(item, layer + 2);
                                }
                            }
                            else
                                Console.WriteLine($"{padding}  No elements.");
                            break;
                        }

                        default:
                            Console.WriteLine($"{padding}{(field.Name + ":").PadRight(maxLength)}");
                            PrintObject(value, layer + 1);
                            //Console.WriteLine($"{padding}{(property.Name + ":").PadRight(maxLength)} {(value != null ? value.ToString() : "null")}");
                            break;
                    }
                }
            }
            else
                Console.WriteLine($"{padding}{obj.ToString()}");
        }
    }
}
