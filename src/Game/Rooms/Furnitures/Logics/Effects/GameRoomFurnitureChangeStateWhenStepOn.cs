using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;

using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Game.Rooms.Furnitures.Actions;

using Cortex.Server.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureChangeStateWhenStepOn : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public void OnUserEnter(GameRoomUser user) {
            if(Furniture.Animation == 1)
                return;

            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, 101));

            Furniture.Animation = 1;
        }

        public void OnUserLeave(GameRoomUser user) {
            if(Furniture.Animation == 0)
                return;

            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, 100));

            Furniture.Animation = 0;
        }
    }
}
