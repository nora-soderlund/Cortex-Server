using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Fleck;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

using Cortex.Server.Game.Users;

namespace Cortex.Server.Socket {
    class SocketClass {
        public WebSocketServer server;

        public List<SocketClient> clients = new List<SocketClient>();

        public Dictionary<string, List<ISocketEvent>> events = new Dictionary<string, List<ISocketEvent>>();

        public SocketClass() {
            FleckLog.Level = LogLevel.Error;
        }

        public void Open() {
            server = new WebSocketServer("ws" + (((bool)Program.Config["socket"]["ssl"])?("s"):("")) + "://" + Program.Config["socket"]["address"].ToString() + ":" + Program.Config["socket"]["port"].ToString());

            if((bool)Program.Config["socket"]["ssl"]) {
                X509Certificate2 certificate = new X509Certificate2((string)Program.Config["socket"]["certificate"]["path"], (string)Program.Config["socket"]["certificate"]["password"]);

                server.Certificate = certificate;
                server.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            }

            server.Start(socket => {
                socket.OnOpen += () => {
                    ThreadPool.QueueUserWorkItem(state => onOpen(socket));
                };

                socket.OnClose += () => {
                    ThreadPool.QueueUserWorkItem(state => onClose(socket));
                };

                socket.OnMessage += (message) => {
                    ThreadPool.QueueUserWorkItem(state => onMessage(socket, message));
                };  
            });
            
            Console.WriteLine(Environment.NewLine + "Listening to connections to " + server.Location + "..." + Environment.NewLine);
        }
    
        private void onOpen(IWebSocketConnection socket) {
            try {
                string key = socket.ConnectionInfo.Path.Substring(1);

                Console.WriteLine("Receiving connection from client at " + socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort + " with key " + key + "...");

                using MySqlConnection connection = new MySqlConnection(Program.Connection);

                connection.Open();

                using MySqlCommand keyCommand = new MySqlCommand("SELECT * FROM user_keys WHERE `key` = @key", connection);

                keyCommand.Parameters.AddWithValue("@key", key);

                using MySqlDataReader keyReader = keyCommand.ExecuteReader();

                if(!keyReader.Read()) {
                    socket.Send(new SocketMessage("OnSocketClose", "USER_KEY_INVALID").Compose());

                    socket.Close();

                    return;
                }

                string address = keyReader.GetString("address");

                /*if(address != "Discord" && address != socket.ConnectionInfo.ClientIpAddress) {
                    socket.Send(new SocketMessage("OnSocketClose", "USER_KEY_UNAUTHORIZED").Compose());

                    socket.Close();

                    return;
                }*/

                long id = keyReader.GetInt64("user");

                keyReader.Close();

                using MySqlCommand userCommand = new MySqlCommand("SELECT * FROM users WHERE id = @id", connection);

                userCommand.Parameters.AddWithValue("@id", id);

                using MySqlDataReader userReader = userCommand.ExecuteReader();

                if(!userReader.Read()) {
                    socket.Send(new SocketMessage("OnSocketClose", "USER_INVALID").Compose());

                    socket.Close();

                    return;
                }

                SocketClient previousClient = clients.Find(x => x.User.Id == id);

                if(previousClient != null) {
                    previousClient.Connection.Close();

                    socket.Send(new SocketMessage("OnSocketClose", "USER_DUPLICATE").Compose());

                    socket.Close();

                    return;
                }

                GameUser user = new GameUser(userReader);

                SocketClient client = new SocketClient(socket, user);

                user.Client = client;

                clients.Add(client);
                
                client.Send(new SocketMessage("OnSocketAuthenticate", true).Compose());

                if(Program.Discord != null)
                    Program.Discord.Client.SetGameAsync("Project Cortex with " + clients.Count + " other" + ((clients.Count == 1)?(""):("s")) + "!");
            }
            catch(Exception exception) {
                Console.Exception(exception);
            }
        }

        private void onMessage(IWebSocketConnection socket, string message) {
            try {
                SocketClient client = clients.Find(x => x.Connection == socket);

                if(client == null) {
                    Console.WriteLog("Received message from unrecognized client at " + socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort + ": " + message);

                    return;
                }

                client.Received++;

                Console.WriteLog("Received message from " + client.GetAddressPort() + ": " + message);

                JObject items = JObject.Parse(message);

                foreach(KeyValuePair<string, JToken> item in items) {
                    if(!events.ContainsKey(item.Key)) {
                        Console.WriteLine("Event " + item.Key + " has no handlers on the server!!");

                        socket.Send(new SocketMessage(item.Key).Compose());

                        continue;
                    }

                    foreach(ISocketEvent socketEvent in events[item.Key]) {
                        if(socketEvent.Execute(client, item.Value) == 0) {
                            Console.WriteLine("Event " + item.Key + " didn't execute properly!!");

                            socket.Send(new SocketMessage(item.Key).Compose());
                        }
                    }
                }
            }
            catch(Exception exception) {
                Console.Exception(exception);
            }
        }

        private void onClose(IWebSocketConnection socket) {
            try {
                SocketClient client = clients.Find(x => x.Connection == socket);

                if(client == null) {
                    Console.WriteLine("Lost connection with unrecognized client at " + socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort + "!");

                    return;
                }

                foreach(ISocketEvent socketEvent in events["OnSocketClose"])
                    socketEvent.Execute(client, null);

                foreach(ISocketEvent socketEvent in events["OnUserDisconnect"])
                    socketEvent.Execute(client, null);

                Console.WriteLine("Lost connection with client at " + client.GetAddressPort() + "!");

                clients.Remove(client);

                if(Program.Discord != null)
                    Program.Discord.Client.SetGameAsync("Project Cortex with " + clients.Count + " other" + ((clients.Count == 1)?(""):("s")) + "!");
            }
            catch(Exception exception) {
                Console.Exception(exception);
            }
        }
    }
}
