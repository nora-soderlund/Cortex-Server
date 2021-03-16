using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Badges.Events {
    class OnBadgeRequest : ISocketEvent {
        public string Event => "OnBadgeRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnBadgeRequest", GameBadgeManager.GetBadge(id)).Compose());

            return 1;
        }
    }
}
