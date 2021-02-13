using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Inventory.Events {
    class OnInventoryFurnituresUpdate : ISocketEvent {
        public string Event => "OnInventoryFurnituresUpdate";

        public int Execute(SocketClient client, JToken data) {
            Dictionary<string, int> furnitures = new Dictionary<string, int>();

            foreach(GameUserFurniture furniture in client.User.Furnitures.FindAll(x => x.Room == 0)) {
                if(!furnitures.ContainsKey(furniture.Furniture.Id))
                    furnitures.Add(furniture.Furniture.Id, 0);

                furnitures[furniture.Furniture.Id]++;
            }

            client.Send(new SocketMessage("OnInventoryFurnituresUpdate", furnitures).Compose());

            return 1;
        }
    }
}
