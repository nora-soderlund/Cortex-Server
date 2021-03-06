using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Furnitures;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Users;

using Server.Game.Rooms.Furnitures.Actions;

using Server.Events;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureHaloLogic : IGameRoomFurnitureLogic {
        public string Logic => "furniture_halo";

        public GameRoomFurniture Furniture { get; set; }

        public void OnUserEnter(GameRoomUser user) {
            Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, 101));

            Furniture.Animation = 1;
        }

        public void OnUserLeave(GameRoomUser user) {
            Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, 100));

            Furniture.Animation = 0;
        }
    }
}
