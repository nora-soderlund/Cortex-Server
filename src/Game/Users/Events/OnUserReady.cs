using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Users.Events {
    class OnUserReady : ISocketEvent {
        public string Event => "OnUserReady";

        public int Execute(SocketClient client, JToken data) {
            Dictionary<string, Dictionary<string, int>> furnitures = new Dictionary<string, Dictionary<string, int>>();

            foreach(GameUserFurniture furniture in client.User.Furnitures) {
                if(!furnitures.ContainsKey(furniture.Furniture.Id))
                    furnitures.Add(furniture.Furniture.Id, new Dictionary<string, int>());

                if(furniture.Room != 0) {
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
                id = client.User.Id,
                
                home = client.User.Home,

                furnitures = furnitures,

                figure = client.User.Figure
            }).Compose());

            if(client.User.Room == null && client.User.Home != null)
                GameRoomManager.AddUser((int)client.User.Home, client.User);

            return 1;
        }
    }
}
