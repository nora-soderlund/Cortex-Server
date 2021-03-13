using System;
using System.Linq;
using System.Timers;
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
    class GameRoomFurnitureBanzaiScore : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public int Team;

        public int Score;

        public GameRoomFurnitureBanzaiScore(GameRoomFurniture furniture) {
            Team = Int32.Parse(furniture.UserFurniture.Furniture.Parameters);
        }

        public void UpdateScore() {
            Score = 0;

            foreach(GameRoomFurniture furniture in Furniture.Room.Furnitures.Where(x => x.Logic is GameRoomFurnitureBanzaiTile)) {
                GameRoomFurnitureBanzaiTile tile = furniture.Logic as GameRoomFurnitureBanzaiTile;

                if(tile.Team != Team)
                    continue;

                if(tile.Step != 2)
                    continue;

                Score++;
            }

            Furniture.Animation = Score;

            Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }
    }
}
