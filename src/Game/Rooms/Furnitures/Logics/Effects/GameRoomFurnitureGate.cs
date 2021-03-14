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
    class GameRoomFurnitureGate : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public bool IsStandable() {
            if(Furniture.Animation == 0)
                return false;

            return true;
        }

        public bool IsWalkable() {
            if(Furniture.Animation == 0)
                return false;

            return true;
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            if(!user.HasRights())
                return;

            if(Furniture.Animation == 0) {
                Furniture.SetAnimation(101, false);
            }
            else {
                if(Furniture.Room.Users.Find(x => x.Position.Row == Furniture.Position.Row && x.Position.Column == Furniture.Position.Column) != null)
                    return;

                Furniture.SetAnimation(100, false);
            }
        }
    }
}
