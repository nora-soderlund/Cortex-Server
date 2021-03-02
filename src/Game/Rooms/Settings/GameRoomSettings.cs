using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

namespace Server.Game.Rooms.Settings {
    class GameRoomSettings {
        class OnRoomSettingsUpdate : ISocketEvent {
            public string Event => "OnRoomSettingsUpdate";

            public int Execute(SocketClient client, JToken data) {
                if(client.User.Room == null)
                    return 0;
                    
                if(client.User.Id != client.User.Room.User)
                    return 0;

                if(data["map"] != null) {
                    client.User.Room.Map = new Map.GameRoomMap(client.User.Room, data["map"]["floor"].ToString(), client.User.Room.Map.Door);   

                    client.User.Room.Send(new SocketMessage("OnRoomSettingsUpdate", new { map = client.User.Room.Map }).Compose());
                }

                if(data["title"] != null) {
                    string title = data["title"].ToString();

                    if(title.Length == 0)
                        title = "No room title...";

                    client.User.Room.SetTitle(title);
                }

                if(data["description"] != null) {
                    string description = data["description"].ToString();

                    if(description.Length == 0)
                        description = "No room description...";

                    client.User.Room.SetDescription(description);
                }

                return 1;
            }
        }
    }
}
