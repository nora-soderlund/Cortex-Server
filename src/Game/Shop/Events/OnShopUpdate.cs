using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Shop.Events {
    class OnShopUpdate : ISocketEvent {
        public string Event => "OnShopUpdate";

        public int Execute(SocketClient client, JToken data) {
            client.Send(new SocketMessage("OnShopUpdate", GameShop.Pages).Compose());

            return 1;
        }
    }
}
