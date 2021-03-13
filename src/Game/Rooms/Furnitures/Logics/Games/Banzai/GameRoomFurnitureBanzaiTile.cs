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
    class GameRoomFurnitureBanzaiTile : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public int Team = 0;
        public int Step = 0;

        public void OnUserEnter(GameRoomUser user) {
            int team = GameRoomFurnitureBanzai.GetUserTeam(user);

            if(team == 0)
                return;

            if(Team != team) {
                Team = team;

                Step = 0;
            }
            else if(Step < 2)
                Step++;
            else
                return;

            Furniture.Animation = (team * 3) + Step;

            Furniture.Room.Actions.AddEntityDelay(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }
    }
}
