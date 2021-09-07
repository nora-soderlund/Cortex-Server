using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Events;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

using Cortex.Server.Game.Badges;
using Cortex.Server.Game.Rooms.Users;

namespace Cortex.Server.Game.Rooms.Chat.Commands.User {
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
