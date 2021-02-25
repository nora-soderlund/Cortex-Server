using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Events {
    class OnUserReady : ISocketEvent {
        public string Event => "OnUserReady";

        public int Execute(SocketClient client, JToken data) {
            Dictionary<string, Dictionary<string, int>> furnitures = new Dictionary<string, Dictionary<string, int>>();

            foreach(GameUserFurniture furniture in client.User.Furnitures) {
                if(!furnitures.ContainsKey(furniture.Furniture.Id))
                    furnitures.Add(furniture.Furniture.Id, new Dictionary<string, int>());

                if(furniture.Room == 0) {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("room"))
                        furnitures[furniture.Furniture.Id].Add("room", 0);

                    furnitures[furniture.Furniture.Id]["room"]++;
                }
                else {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("inventory"))
                        furnitures[furniture.Furniture.Id].Add("inventory", 0);

                    furnitures[furniture.Furniture.Id]["inventory"]++;
                }
            }

            client.Send(new SocketMessage("OnUserUpdate", new {
                home = client.User.Home,

                furnitures = furnitures
            }).Compose());

            if(client.User.Room == null && client.User.Home != null)
                GameRoomManager.AddUser((int)client.User.Home, client.User);

            return 1;
        }
    }
}
