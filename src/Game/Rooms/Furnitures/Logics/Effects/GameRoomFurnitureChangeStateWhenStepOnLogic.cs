using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Furnitures;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Users;

using Server.Game.Rooms.Furnitures.Actions;

using Server.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureChangeStateWhenStepOnLogic : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public void OnUserUse(GameRoomUser user, JToken data) {
            
        }

        public void OnUserEnter(GameRoomUser user) {
            if(Furniture.Animation == 1)
                return;

            Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, 101));

            Furniture.Animation = 1;
        }

        public void OnUserLeave(GameRoomUser user) {
            if(Furniture.Animation == 0)
                return;

            Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, 100));

            Furniture.Animation = 0;
        }

        public void OnFurnitureEnter(GameRoomFurniture furniture) {
            
        }

        public void OnFurnitureLeave(GameRoomFurniture furniture) {
            
        }
    }
}
