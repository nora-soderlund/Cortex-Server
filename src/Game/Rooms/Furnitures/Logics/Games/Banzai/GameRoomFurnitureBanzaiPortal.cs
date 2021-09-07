using System;
using System.Linq;
using System.Timers;
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
    class GameRoomFurnitureBanzaiPortal : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public int Team;
        public int Effect;


        public List<GameRoomUser> Users = new List<GameRoomUser>();

        public GameRoomFurnitureBanzaiPortal(GameRoomFurniture furniture) {
            string[] parameters = furniture.UserFurniture.Furniture.Parameters.Split(',');

            Team = Int32.Parse(parameters[0]);
            Effect = Int32.Parse(parameters[1]);
        }

        public bool IsWalkable() => !(Users.Count >= 5);
        
        public void OnUserEnter(GameRoomUser user) {
            if(GameRoomFurnitureBanzai.GetUserTeam(user) != 0) {
                if(!Users.Contains(user)) {
                    GameRoomFurniture furniture = Furniture.Room.Furnitures.Find(x => x.Logic is GameRoomFurnitureBanzaiPortal && (x.Logic as GameRoomFurnitureBanzaiPortal).Users.Contains(user));
                    
                    if(furniture != null) {
                        GameRoomFurnitureBanzaiPortal portal = furniture.Logic as GameRoomFurnitureBanzaiPortal;

                        if(portal.Users.Contains(user)) {
                            portal.Users.Remove(user);

                            furniture.Animation = Users.Count;
                            furniture.Room.Actions.AddEntity(furniture.Id, new GameRoomFurnitureAnimation(furniture, furniture.Animation));
                        }
                    }
                }
                else {
                    Users.Remove(user);
                    
                    Furniture.Animation = Users.Count;
                    Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
                }
                    
                user.SetEffect(0);

                return;
            }

            if(Users.Count >= 5)
                return;

            if(GameRoomFurnitureBanzai.GetUserTeam(user) == 0)

            Users.Add(user);

            user.SetEffect(Effect);

            Furniture.Animation = Users.Count;
            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }

        public void OnUserStreamOut(GameRoomUser user) {
            if(!Users.Contains(user))
                return;

            Users.Remove(user);

            Furniture.Animation = Users.Count;
            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }
    }
}
