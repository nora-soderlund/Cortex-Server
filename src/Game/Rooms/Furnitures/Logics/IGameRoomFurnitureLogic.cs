using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Users;

using Server.Game.Furnitures;

using Server.Game.Users.Furnitures;

namespace Server.Game.Rooms.Furnitures.Logics {
    interface IGameRoomFurnitureLogic {
        GameRoomFurniture Furniture { get; set; }

        void OnUserUse(GameRoomUser user, JToken data);
        void OnUserEnter(GameRoomUser user);
        void OnUserLeave(GameRoomUser user);
    }
}
