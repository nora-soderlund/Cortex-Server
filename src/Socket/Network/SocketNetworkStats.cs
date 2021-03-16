using System;
using System.Timers;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Fleck;

using Newtonsoft.Json;

using Server.Events;

using Server.Socket.Messages;
using Server.Socket.Clients;

namespace Server.Socket.Network {
    class SocketNetworkStats : IInitializationEvent {
        public static Timer Timer = new Timer(1000) {
            Enabled = true
        };

        public void OnInitialization() {
            Timer.Elapsed += OnTimer;
        }

        public static int Users = 0;

        public static DateTime Boot = DateTime.Now;

        public static string Uptime;

        public void OnTimer(Object source, System.Timers.ElapsedEventArgs e) {
            if(Program.Socket == null)
                return;

            bool changed = false;

            if(Users != Program.Socket.clients.Count) {
                Users = Program.Socket.clients.Count;

                changed = true;
            }

            TimeSpan upDuration = DateTime.Now - Boot;

            string newUptime = "";
            
            if(upDuration.Days != 0)
                newUptime += upDuration.Days + " day" + ((upDuration.Days == 1)?(""):("s")) + " ";
            
            if(upDuration.Hours != 0)
                newUptime += upDuration.Hours + " hour" + ((upDuration.Hours == 1)?(""):("s")) + " ";
            
            newUptime += upDuration.Minutes + " minute" + ((upDuration.Minutes == 1)?(""):("s"));

            if(Uptime != newUptime) {
                Uptime = newUptime;

                changed = true;
            }

            if(!changed) {
                foreach(SocketClient client in Program.Socket.clients) {
                    Send(client);
                }
            }
        }

        public void Send(SocketClient client) {
            client.Send(new SocketMessage("OnSocketUpdate", new {
                users = Users,
                uptime = Uptime
            }).Compose());
        }
    }
}
