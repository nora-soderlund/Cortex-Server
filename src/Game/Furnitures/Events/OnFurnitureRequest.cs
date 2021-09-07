using System;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Furnitures.Events {
    class OnFurnitureRequest : ISocketEvent {
        public string Event => "OnFurnitureRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnFurnitureRequest", GameFurnitureManager.Furnitures.Find(x => x.Id == id)).Compose());

            return 1;
        }
    }
}
