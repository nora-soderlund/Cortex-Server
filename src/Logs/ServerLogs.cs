using System;
using System.IO;

using Server.Events;

namespace Server {
    class Console  {
        private static string file = "logs/" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".log";

        public static void Start() {
            if(!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            UpdateTitle();
        }

        public static void UpdateTitle() {
            System.Console.Title = "Project Cortex Server";
        }

        public static void WriteLine(object input) {
            try {
                string prefix = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "." + DateTime.Now.Second + "] ";

                string[] inputs = input.ToString().Split('\n');

                using(StreamWriter writer = File.AppendText(file)) {
                    foreach(string line in inputs) {
                        System.Console.WriteLine(prefix + line);

                        writer.WriteLine(prefix + line);
                    }

                    writer.Close();
                }
            }
            catch(IOException) {
                WriteLine(input);
            }
        }

        public static void WriteLog(object input) {
            try {
                string prefix = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "." + DateTime.Now.Second + "] ";
                
                string[] inputs = input.ToString().Split('\n');

                using(StreamWriter writer = File.AppendText(file)) {
                    foreach(string line in inputs)
                        writer.WriteLine(prefix + line);
                
                    writer.Close();
                }
            }
            catch(IOException) {
                WriteLog(input);
            }
        }

        public static string ReadLine() {
            string input = System.Console.ReadLine();

            WriteLog(input);

            return input;
        }

        public static void Exception(Exception exception) {
            System.Console.ForegroundColor = ConsoleColor.DarkRed;

            System.Console.WriteLine("An unhandled but handled exception has occurred: " + exception.Message + Environment.NewLine + "Please review your server logs for a stack trace!");

            System.Console.ResetColor();

            WriteLog(exception.StackTrace);

            if(Program.Discord != null)
                Program.Discord.Exception(exception);
        }
    }
}
