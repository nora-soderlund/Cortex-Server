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

            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurniturePosition(Furniture, position, 500));
        }

        public GameRoomPoint GetNextPoint(GameRoomPoint _position, int direction) {
			GameRoomPoint position = new GameRoomPoint(_position);

			position.FromDirection(direction);
            
            if(!Furniture.Room.Map.IsValidFloor(position.Row, position.Column))
                return null;
            
            GameRoomFurniture nextFurniture = Furniture.Room.Map.GetFloorFurniture(position.Row, position.Column);

			if(nextFurniture != null) {
                if(!nextFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Walkable))
                    return null;

                position.Depth = nextFurniture.Position.Depth;
            }

			return position;
		}
        
        public int GetInvertedPoint(GameRoomPoint position, int direction) {
            switch(direction) {
                case (int)GameRoomPointDirections.NorthWest: {
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.SouthEast) != null)
                        return (int)GameRoomPointDirections.SouthEast;
                        
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.South) != null)
                        return (int)GameRoomPointDirections.South;
                        
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.East) != null)
                        return (int)GameRoomPointDirections.East;

                    break;
                }
                    
                case 0: direction = (int)GameRoomPointDirections.South;
                    break;
                    
                case 1: {
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.East) != null)
                        return (int)GameRoomPointDirections.East;

                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.West) != null)
                        return (int)GameRoomPointDirections.West;

                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.SouthWest) != null)
                        return (int)GameRoomPointDirections.SouthWest;
                    
                    break;
                }
                    
                case 2: direction = 6;
                    break;
                    
                case 3: {
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.West) != null)
                        return (int)GameRoomPointDirections.West;

                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.North) != null)
                        return (int)GameRoomPointDirections.North;

                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.NorthWest) != null)
                        return (int)GameRoomPointDirections.NorthWest;
                    
                    break;
                }
                    
                case 4: direction = 0;
                    break;
                    
                case 5: {
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.NorthEast) != null)
                        return (int)GameRoomPointDirections.NorthEast;
                        
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.SouthEast) != null)
                        return (int)GameRoomPointDirections.SouthEast;
                        
                    if(GetNextPoint(Furniture.Position, (int)GameRoomPointDirections.NorthWest) != null)
                        return (int)GameRoomPointDirections.NorthWest;
                        
                    break;
                }
                    
                case 6: direction = 2;
                    break;
            }

            return direction;
        }
    }
}
