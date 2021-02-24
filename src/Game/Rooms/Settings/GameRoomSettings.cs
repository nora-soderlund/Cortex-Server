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

                if(data["map"] != null) {
                    if(client.User.Id != client.User.Room.User)
                        return 0;

                    client.User.Room.Map = new Map.GameRoomMap(client.User.Room, data["map"]["floor"].ToString(), client.User.Room.Map.Door);

                    client.Send(new SocketMessage("OnRoomSettingsUpdate", true).Compose());

                    client.User.Room.Send(new SocketMessage("OnRoomMapUpdate", client.User.Room.Map).Compose());
                }

                return 1;
            }
        }
    }
}
