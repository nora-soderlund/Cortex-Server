using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Furnitures.Events {
    class OnFurnitureDepthRequest : ISocketEvent {
        public string Event => "OnFurnitureDepthRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnFurnitureDepthRequest", GameFurnitureManager.Furnitures.Find(x => x.Id == id).Dimension.Depth).Compose());

            return 1;
        }
    }
    
    class OnFurnitureLogicRequest : ISocketEvent {
        public string Event => "OnFurnitureLogicRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnFurnitureLogicRequest", GameFurnitureManager.Furnitures.Find(x => x.Id == id).Logic).Compose());

            return 1;
        }
    }
    
    class OnFurnitureFlagsRequest : ISocketEvent {
        public string Event => "OnFurnitureFlagsRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnFurnitureFlagsRequest", GameFurnitureManager.Furnitures.Find(x => x.Id == id).Flags).Compose());

            return 1;
        }
    }
}
