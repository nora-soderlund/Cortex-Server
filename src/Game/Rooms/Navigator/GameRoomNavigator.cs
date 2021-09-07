using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Cortex.Server.Events;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Game.Rooms.Navigator {
    class GameRoomNavigator {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("title")]
        public string Title;

        [JsonIgnore]
        public int Access;

        [JsonIgnore]
        public long User;

        [JsonIgnore]
        public GameRoom Room;

        public GameRoomNavigator(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Title = reader.GetString("title");

            User = reader.GetInt64("user");

            Access = reader.GetInt32("access");
        }

        public GameRoomNavigator(GameRoom room) {
            Room = room;
            
            Id = room.Id;

            Title = room.Title;

            User = room.User;

            Access = room.Access;
        }
    }

    class OnRoomNavigatorUpdate : ISocketEvent {
        public string Event => "OnRoomNavigatorUpdate";

        public int Execute(SocketClient client, JToken data) {
            switch(data.ToString()) {
                case "all_rooms": {
                    List<object> popular = new List<object>();

                    foreach(GameRoomNavigator navigator in GameRoomManager.Navigator.FindAll(x => x.Room != null && x.Room.Users.Count != 0)) {
                        if(navigator.Room == null) {
                            popular.Add(new {
                                id = navigator.Id,
                                title = navigator.Title,
                                user = navigator.User
                            });

                            continue;
                        }

                        popular.Add(new {
                            id = navigator.Id,
                            title = navigator.Title,
                            user = navigator.User,
                            
                            users = navigator.Room.Users.Count
                        });
                    }

                    client.Send(new SocketMessage("OnRoomNavigatorUpdate", new {
                        popular
                    }).Compose());

                    return 1;
                }

                case "my_rooms": {
                    List<object> owned = new List<object>();

                    foreach(GameRoomNavigator navigator in GameRoomManager.Navigator.FindAll(x => x.User == client.User.Id)) {
                        if(navigator.Room == null) {
                            owned.Add(new {
                                id = navigator.Id,
                                title = navigator.Title,
                                user = navigator.User
                            });

                            continue;
                        }

                        owned.Add(new {
                            id = navigator.Id,
                            title = navigator.Title,
                            user = navigator.User,
                            
                            users = navigator.Room.Users.Count
                        });
                    }

                    client.Send(new SocketMessage("OnRoomNavigatorUpdate", new {
                        owned
                    }).Compose());

                    return 1;
                }
            }

            return 0;
        }
    }
}
