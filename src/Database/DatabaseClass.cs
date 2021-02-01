using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Fleck;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

namespace Server.Database {
    class DatabaseClass {
        public readonly MySqlConnection Connection;

        public DatabaseClass(string hostname = "127.0.0.1", string username = "root", string database = "cortex", string password = "") {
            Program.WriteLine("Connecting to the database at " + username + "@" + hostname + "... ", ConsoleColor.Gray, true);

            Stopwatch watch = Stopwatch.StartNew();
            
            try {
                Connection = new MySqlConnection("server=" + hostname + ";uid=" + username + ";database=" + database);

                Connection.Open();

                Program.Write("connected after " + watch.ElapsedMilliseconds + "ms!" + Environment.NewLine, (watch.ElapsedMilliseconds > 500)?(ConsoleColor.DarkYellow):(ConsoleColor.DarkGreen));
            }
            catch (Exception exception) {
                Program.Write("failed after " + watch.ElapsedMilliseconds + "ms!" + Environment.NewLine, ConsoleColor.DarkRed);
            }
            finally {
                watch.Stop();
            }
        }
    }
}
