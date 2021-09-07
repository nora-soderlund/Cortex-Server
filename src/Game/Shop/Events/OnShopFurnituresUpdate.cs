using System;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Shop.Events {
    class OnShopFurnituresUpdate : ISocketEvent {
        public string Event => "OnShopFurnituresUpdate";

        public int Execute(SocketClient client, JToken data) {
            int page = data.ToObject<int>();

            client.Send(new SocketMessage("OnShopFurnituresUpdate", GameShop.Furnitures.FindAll(x => x.Page == page)).Compose());

            return 1;
        }
    }
}
