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
    class GameRoomFurnitureVideo : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public void OnUserUse(GameRoomUser user, JToken data) {
            if(Furniture.Animation == 0) {
                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                    id = Furniture.Id,

                    link = "http://api.project-cortex.net/YouTube/" + Furniture.Extra + ".mp4"
                }).Compose());

                Furniture.Animation = 1;
            }
            else {
                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStop", Furniture.Id).Compose()); 

                Furniture.Animation = 0;
            }
        }

        public void OnUserEnter(GameRoomUser user) {
            
        }

        public void OnUserLeave(GameRoomUser user) {
            
        }

        public void OnFurnitureEnter(GameRoomFurniture furniture) {
            
        }

        public void OnFurnitureLeave(GameRoomFurniture furniture) {
            
        }
    }
}
