using System;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Shop.Events {
    class OnShopUpdate : ISocketEvent {
        public string Event => "OnShopUpdate";

        public int Execute(SocketClient client, JToken data) {
            client.Send(new SocketMessage("OnShopUpdate", GameShop.Pages).Compose());

            return 1;
        }
    }
}
