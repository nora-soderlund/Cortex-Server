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

                            Furniture.Animation = Users.Count;
                            Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
                        }
                    }
                }
                else {
                    Users.Remove(user);
                    
                    Furniture.Animation = Users.Count;
                    Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
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
            Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }

        public void OnUserStreamOut(GameRoomUser user) {
            if(!Users.Contains(user))
                return;

            Users.Remove(user);

            Furniture.Animation = Users.Count;
            Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
        }
    }
}
