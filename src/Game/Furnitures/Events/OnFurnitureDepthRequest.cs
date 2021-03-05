using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Furnitures.Events {
    class OnFurnitureDepthRequest : ISocketEvent {
        public string Event => " ";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnFurnitureDepthRequest", GameFurnitureManager.Furnitures.Find(x => x.Id == id).Dimension.Depth).Compose());

            return 1;
        }
    }
}
