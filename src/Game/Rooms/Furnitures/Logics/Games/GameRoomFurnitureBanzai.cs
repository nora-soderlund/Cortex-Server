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
    class GameRoomFurnitureBanzai : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public int Team = 0;
        public int Step = 0;

        public void OnUserLeave(GameRoomUser user) {
            int team = GetUserTeam(user);

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

            Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }

        public int GetUserTeam(GameRoomUser user) {
            switch(user.Effect) {
                case 33: return 1; // purple
                case 34: return 2; // green
                case 35: return 3; // blue
                case 36: return 4; // yellow
            }

            return 0;
        }
    }
}
