using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using Server.Game.Users.Badges;

namespace Server.Game.Users.Inventory.Events {
    class OnUserInventoryBadges : ISocketEvent {
        public string Event => "OnUserInventoryBadges";

        public int Execute(SocketClient client, JToken data) {
            JToken token = data.SelectToken("id");

            if(token == null) {
                client.Send(new SocketMessage("OnUserInventoryBadges", client.User.Badges).Compose());
                
                return 1;
            }

            string badge = data["id"].ToString();

            GameUserBadge userBadge = client.User.Badges.Find(x => x.Badge == badge);

            if(userBadge == null)
                return 0;

            userBadge.Equipped = !userBadge.Equipped;

            client.Send(new SocketMessage("OnUserInventoryBadges", true).Compose());

            return 1;
        }
    }
}
