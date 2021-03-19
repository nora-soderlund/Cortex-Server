using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Furnitures;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Furnitures.Logics;

namespace Server.Game.Rooms.Map {
    class GameRoomMap {
        [JsonIgnore]
        public GameRoom Room;

        [JsonProperty("floor")]
        public string[] Floor;

        [JsonProperty("door")]
        public GameRoomPoint Door;

        [JsonIgnore]
        public int Rows;

        [JsonIgnore]
        public int Columns;

        public GameRoomMap(GameRoom room, string floor, GameRoomPoint door) {
            Room = room;

            Floor = floor.ToUpper().Split('|');

            Rows = Floor.Length;

            for(int row = 0; row < Rows; row++)
                if(Floor[row].Length > Columns)
                    Columns = Floor[row].Length;

            Door = door;
        }

        public double GetFloorDepth(int row, int column) {
            if(!IsValidFloor(row, column))
                return 'X';

            if(Char.ToUpper(Floor[row][column]) != Char.ToLower(Floor[row][column]))
                return (double)((Floor[row][column] - 97) - '0');

            return (double)(Floor[row][column] - '0');
        }
        public Position[] GetFloorPath(GameRoomPoint start, GameRoomPoint end) {
            Grid grid = new Grid(Rows, Columns);

            for(int row = 0; row < Rows; row++)
            for(int column = 0; column < Floor[row].Length; column++) {
                if(Floor[row][column] == 'X') {
                    grid.BlockCell(new Position(row, column));

                    continue;
                }

                if(GetFloorFurniture(row, column, null, GameFurnitureFlags.Sitable) != null) {
                    continue;
                }

                GameRoomFurniture furniture = GetFloorFurniture(row, column);

                if(furniture != null) {
                    if(furniture.Logic != null && !furniture.Logic.IsWalkable()) {
                        grid.BlockCell(new Position(row, column));

                        continue;
                    }
                }
            }

            return grid.GetPath(new Position(start.Row, start.Column), new Position(end.Row, end.Column));
        }

        public GameRoomFurniture GetFloorFurniture(int row, int column, Type logic = null, Enum flag = null) {
            GameRoomFurniture result = null;

            List<GameRoomFurniture> furnitures = new List<GameRoomFurniture>(Room.Furnitures);

            if(logic != null)
                furnitures.RemoveAll(x => (x.Logic == null) || (x.Logic.GetType() != logic));

            if(flag != null)
                furnitures.RemoveAll(x => !x.UserFurniture.Furniture.Flags.HasFlag(flag));

            foreach(GameRoomFurniture furniture in furnitures) {
                if(furniture.Position.Row > row)
                    continue;
                    
                if(furniture.Position.Column > column)
                    continue;

                GameRoomPoint dimensions = furniture.GetDimension();

                if(furniture.Position.Row + dimensions.Row <= row)
                    continue;

                if(furniture.Position.Column + dimensions.Column <= column)
                    continue;

                if(result != null && (furniture.Position.Depth + dimensions.Depth) < (result.Position.Depth + result.GetDimension().Depth))
                    continue;

                result = furniture;
            }

            return result;
        }

        public bool IsValidFloor(int row, int column) {
            if(row < 0 || row >= Floor.Length)
                return false;

            if(column < 0 || column >= Floor[row].Length)
                return false;

            if(Floor[row][column] == 'X')
                return false;

            return true;
        }

        public Dictionary<int, Dictionary<int, double>> GetStackableFloor() {
            Dictionary<int, Dictionary<int, double>> map = new Dictionary<int, Dictionary<int, double>>();

            for(int row = 0; row < Rows; row++) {
                map.Add(row, new Dictionary<int, double>());

                for(int column = 0; column < Floor[row].Length; column++) {
                    if(Floor[row][column] == 'X')
                        continue;

                    double depth = GetFloorDepth(row, column);

                    GameRoomFurniture roomFurniture = GetFloorFurniture(row, column);

                    if(roomFurniture != null) {
                        if(!roomFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                            continue;

                        depth = roomFurniture.Position.Depth + roomFurniture.GetDimension().Depth;
                    }

                    map[row].Add(column, depth);
                }
            }

            return map;
        }
    }
}
