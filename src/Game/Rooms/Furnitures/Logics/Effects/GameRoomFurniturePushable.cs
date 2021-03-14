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
    class GameRoomFurniturePushable : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public int Distance;

        public GameRoomFurniturePushable(GameRoomFurniture furniture) {
            string[] parameters = furniture.UserFurniture.Furniture.Parameters.Split(',');

            Distance = Int32.Parse(parameters[0]);
        }

        public bool IsWalkable() {
            return true;
        }

        public void OnUserEnter(GameRoomUser user) {
            GameRoomPoint position = GetNextPoint(Furniture.Position, user.Position.Direction);

            if(position == null) {
                position = GetNextPoint(Furniture.Position, GetInvertedPoint(Furniture.Position, user.Position.Direction));

                if(position == null)
                    return;
            }

            Furniture.Room.Actions.AddEntity(Furniture.Id, 500, new GameRoomFurniturePosition(Furniture, position, 500));
        }

        public GameRoomPoint GetNextPoint(GameRoomPoint _position, int direction) {
			GameRoomPoint position = new GameRoomPoint(_position);

			position.FromDirection(direction);
            
            if(!Furniture.Room.Map.IsValidFloor(position.Row, position.Column))
                return null;
            
            GameRoomFurniture nextFurniture = Furniture.Room.Map.GetFloorFurniture(position.Row, position.Column);

			if(nextFurniture != null) {
                if(!nextFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                    return null;

                position.Depth = nextFurniture.Position.Depth;
            }

			return position;
		}
        
        public int GetInvertedPoint(GameRoomPoint position, int direction) {
            switch(direction) {
                case (int)GameRoomPointDirections.NorthWest: {
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.North) == null) {
                        if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.SouthWest) == null) {
                            direction = (int)GameRoomPointDirections.SouthEast;
                        }
                        else
                            direction = (int)GameRoomPointDirections.SouthWest;
                    }
                    else
                        direction = (int)GameRoomPointDirections.North;

                    break;
                }
                    
                case 0: direction = 4;
                    break;
                    
                case 1: direction = 3;
                    break;
                    
                case 2: direction = 6;
                    break;
                    
                case 3: direction = 7;
                    break;
                    
                case 4: direction = 0;
                    break;
                    
                case 5: direction = 3;
                    break;
                    
                case 6: direction = 2;
                    break;
            }

            return direction;
        }
    }
}
