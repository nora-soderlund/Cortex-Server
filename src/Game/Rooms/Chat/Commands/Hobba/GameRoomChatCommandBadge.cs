using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

using Server.Game.Badges;
using Server.Game.Rooms.Users;

namespace Server.Game.Rooms.Chat.Commands.User {
    class GameRoomChatCommandBadge : IGameRoomChatCommand {
        public string Command => "badge";

        public bool Execute(GameRoomUser user, string[] parameters) {
            if(parameters.Length < 2)
                return false;

            string badge = parameters[1];

            GameBadgeManager.AddBadge(user.User, badge, true);

            return true;
        }
    }
}
