using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Shop.Events {
    class OnShopFurnituresUpdate : ISocketEvent {
        public string Event => "OnShopFurnituresUpdate";

        public int Execute(SocketClient client, JToken data) {
            int id = data.ToObject<int>();

            client.Send(new SocketMessage("OnShopFurnituresUpdate", GameShop.Pages.Find(x => x.Id == id).Furnitures).Compose());

            return 1;
        }
    }
}
