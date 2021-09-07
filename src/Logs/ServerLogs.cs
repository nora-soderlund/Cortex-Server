using System;
using System.IO;
using System.Threading;

using Cortex.Server.Events;

namespace Cortex.Server {
    class Console  {
        private static string file = "logs/" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".log";
        
        private static string pendingLine = "";
        private static string pendingLog = "";

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
                string prefix = "[" + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + "." + DateTime.Now.Second.ToString("00") + "] ";

                string[] inputs = input.ToString().Split('\n');

                using(StreamWriter writer = File.AppendText(file)) {
                    foreach(string line in inputs) {
                        System.Console.WriteLine(prefix + line);

                        writer.WriteLine(prefix + line);
                    }
                    
                    if(pendingLine.Length != 0) {
                        foreach(string line in pendingLine.Split('\n')) {
                            System.Console.WriteLine(prefix + line);

                            writer.WriteLine(prefix + line);
                        }

                        pendingLine = "";
                    }
                }
            }
            catch(IOException) {
                pendingLine += input;
            }
        }

        public static void WriteLog(object input) {
            try {
                string prefix = "[" + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + "." + DateTime.Now.Second.ToString("00") + "] ";
                
                string[] inputs = input.ToString().Split('\n');

                using(StreamWriter writer = File.AppendText(file)) {
                    foreach(string line in inputs)
                        writer.WriteLine(prefix + line);
                
                    if(pendingLog.Length != 0) {                
                        foreach(string line in pendingLog.Split('\n'))
                            writer.WriteLine(prefix + line);

                        pendingLog = "";
                    }
                }
            }
            catch(IOException) {
                pendingLog += input;
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
