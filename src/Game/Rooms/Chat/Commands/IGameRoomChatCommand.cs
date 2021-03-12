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

using Server.Game.Rooms.Users;

namespace Server.Game.Rooms.Chat.Commands {
    interface IGameRoomChatCommand {
        string Command { get; }

        bool Execute(GameRoomUser user, string[] parameters);
    }
}
