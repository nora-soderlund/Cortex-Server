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

        bool IsWalkable() =>
            Furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Walkable);

        bool IsStandable() =>
            Furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Standable);

        void OnUserUse(GameRoomUser user, JToken data) { }
        void OnUserEnter(GameRoomUser user) { }
        void OnUserLeave(GameRoomUser user) { }
        
        void OnFurnitureEnter(GameRoomFurniture furniture) { }
        void OnFurnitureLeave(GameRoomFurniture furniture) { }
    }
}
