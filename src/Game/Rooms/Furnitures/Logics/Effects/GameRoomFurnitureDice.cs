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
    class GameRoomFurnitureDice : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public void OnUserUse(GameRoomUser user, JToken data) {
            if(Furniture.Position.GetDistance(user.Position) > 1)
                return;

            string tag = data["tag"].ToString();

            if(tag == "activate") {
                if(Furniture.Animation == -1)
                    return;

                Furniture.Animation = -1;

                Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, -1));

                Timer timer = new Timer(1000);

                timer.Elapsed += (a, b) => {
                    int dice = new Random().Next(1, 7);

                    Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, dice));

                    Furniture.SetAnimation(dice);

                    timer.Stop();
                };

                timer.Start();
            }
            else if(tag == "deactivate") {
                if(Furniture.Animation == 0)
                    return;

                Furniture.Room.Actions.AddEntity(Furniture.Id, 0, new GameRoomFurnitureAnimation(Furniture, 0));

                Furniture.SetAnimation(0);
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
