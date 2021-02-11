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
            int page = data.ToObject<int>();

            client.Send(new SocketMessage("OnShopFurnituresUpdate", GameShop.Furnitures.FindAll(x => x.Page == page)).Compose());

            return 1;
        }
    }
}
