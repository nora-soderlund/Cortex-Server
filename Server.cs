using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Server.Socket;
using Server.Events;
using Server.Socket.Events;

namespace Server {
    class Program {
        static private SocketClass socket;
        
        public static string Connection = "server=127.0.0.1;uid=root;database=cortex";

        static void Main(string[] args) {
            Console.Title = "Project Cortex Server";

            Console.WriteLine(  @"  _____           _           _      _____           _            " + Environment.NewLine +
                                @" |  __ \         (_)         | |    / ____|         | |           " + Environment.NewLine +
                                @" | |__) | __ ___  _  ___  ___| |_  | |     ___  _ __| |_ _____  __" + Environment.NewLine +
                                @" |  ___/ '__/ _ \| |/ _ \/ __| __| | |    / _ \| '__| __/ _ \ \/ /" + Environment.NewLine +
                                @" | |   | | | (_) | |  __/ (__| |_  | |___| (_) | |  | ||  __/>  < " + Environment.NewLine +
                                @" |_|   |_|  \___/| |\___|\___|\__|  \_____\___/|_|   \__\___/_/\_\" + Environment.NewLine +
                                @"                _/ |                                              " + Environment.NewLine +
                                @"               |__/                                               " + Environment.NewLine);
            
            Console.WriteLine("Project Cortex Server by Cake\t\thttps://project-cortex.net/" + Environment.NewLine);

            Console.ResetColor();

            socket = new SocketClass();

            foreach (var instance in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(a => a.GetConstructor(Type.EmptyTypes) != null).Select(Activator.CreateInstance).OfType<ISocketEvent>()) {
                if(instance.Event.Length == 0)
                    continue;
                
                if(!socket.events.ContainsKey(instance.Event))
                    socket.events[instance.Event] = new List<ISocketEvent>();

                socket.events[instance.Event].Add(instance);
            }

            socket.Open();

            foreach (var instance in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(a => a.GetConstructor(Type.EmptyTypes) != null).Select(Activator.CreateInstance).OfType<IInitializationEvent>()) {
                instance.OnInitialization();
            }

            while(true) {
                string input = Console.ReadLine();

                if(input.StartsWith("query")) {
                    int start = input.IndexOf(' ');

                    if(start == -1) {
                        Console.WriteLine("Query command must be followed by the exact database query!");

                        continue;
                    }

                    string query = input.Substring(start);

                    using MySqlConnection connection = new MySqlConnection(Connection);

                    connection.Open();

                    using MySqlCommand command = new MySqlCommand(query, connection);

                    int affected = command.ExecuteNonQuery();

                    Console.WriteLine("Query executed and affected " + affected + " rows!");
                }
            }
        }

        public static void Write(string value, ConsoleColor color = ConsoleColor.Gray) {
            Console.ForegroundColor = color;

            Console.Write(value);

            Console.ResetColor();
        }

        public static void WriteLine(string value, ConsoleColor color = ConsoleColor.Gray, bool skipLine = false) {
            Console.Write("[00:00.00] ");

            Console.ForegroundColor = color;

            Console.Write(value + ((skipLine)?(""):(Environment.NewLine)));

            Console.ResetColor();
        }
    }
}
