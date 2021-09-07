using System;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Badges.Events {
    class OnBadgeRequest : ISocketEvent {
        public string Event => "OnBadgeRequest";

        public int Execute(SocketClient client, JToken data) {
            string id = data.ToString();

            client.Send(new SocketMessage("OnBadgeRequest", GameBadgeManager.GetBadge(id)).Compose());

            return 1;
        }
    }
}
