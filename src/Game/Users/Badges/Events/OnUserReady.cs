using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Server.Game.Badges;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Badges.Events {
    class OnUserReady : ISocketEvent {
        public string Event => "OnUserReady";

        public int Execute(SocketClient client, JToken data) {
            GameBadgeManager.AddBadge(client.User, "Cortex/BETA", true);

            return 1;
        }
    }
}
