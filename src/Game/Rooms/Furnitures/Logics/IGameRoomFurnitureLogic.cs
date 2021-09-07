using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Rooms.Furnitures.Logics {
    interface IGameRoomFurnitureLogic {
        GameRoomFurniture Furniture { get; set; }

        bool IsWalkable() =>
            Furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Walkable);

        bool IsStandable() =>
            Furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Standable);

        void OnUserUse(GameRoomUser user, JToken data) { }
        void OnUserEnter(GameRoomUser user) { }
        void OnUserLeave(GameRoomUser user) { }
        void OnUserStreamIn(GameRoomUser user) { }
        void OnUserStreamOut(GameRoomUser user) { }
        
        void OnFurnitureEnter(GameRoomFurniture furniture) { }
        void OnFurnitureLeave(GameRoomFurniture furniture) { }
        
        void OnGameStart() { }
        void OnGameStop() { }
    }
}
