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

using Cortex.Server.Game.Rooms.Users;

namespace Cortex.Server.Game.Rooms.Chat.Commands {
    interface IGameRoomChatCommand {
        string Command { get; }

        bool Execute(GameRoomUser user, string[] parameters);
    }
}
