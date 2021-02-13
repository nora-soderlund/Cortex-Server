using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Furnitures;
using Server.Game.Rooms.Furnitures;

namespace Server.Game.Rooms.Map {
    class GameRoomMap {
        [JsonProperty("floor")]
        public string Floor;

        [JsonProperty("height")]
        public Dictionary<int, Dictionary<int, double?>> Height;

        [JsonIgnore]
        public string[] FloorGrid;

        [JsonIgnore]
        public Grid Grid;

        [JsonIgnore]
        public GameRoom Room;

        public GameRoomMap(GameRoom room, string floor) {
            Room = room;

            Floor = floor;

            FloorGrid = floor.Split('|');

            Grid = new Grid(FloorGrid.Length, FloorGrid[0].Length, 1.0f);

            for(int row = 0; row < FloorGrid.Length; row++) {
                for(int column = 0; column < FloorGrid[row].Length; column++) {
                    if(FloorGrid[row][column] != 'X')
                        continue;
                    
                    Grid.BlockCell(new Position(row, column));
                }
            }

            UpdateHeight();
        }

        public Position[] GetPath(GameRoomPoint start, GameRoomPoint end) {
            return Grid.GetPath(new Position((int)start.Row, (int)start.Column), new Position((int)end.Row, (int)end.Column));
        }

        public GameRoomFurniture GetFurniture(int row, int column) {
            return Room.Furnitures.OrderByDescending(x => x.Position.Depth).FirstOrDefault(x => x.Position.Row == row && x.Position.Column == column);
        }

        public double? GetDepth(int row, int column) {
            GameRoomFurniture furniture = GetFurniture(row, column);

            if(furniture != null)
                return furniture.Position.Depth + furniture.Furniture.Dimension.Depth;

            if(Char.ToUpper(FloorGrid[row][column]) != Char.ToLower(FloorGrid[row][column]))
                return (double)((FloorGrid[row][column] - 97) - '0');

            return (double)(FloorGrid[row][column] - '0');
        }

        public void UpdateHeight() {
            Dictionary<int, Dictionary<int, double?>> height = new Dictionary<int, Dictionary<int, double?>>();

            for(int row = 0; row < FloorGrid.Length; row++) {
                height.Add(row, new Dictionary<int, double?>());

                for(int column = 0; column < FloorGrid[row].Length; column++) {
                    if(FloorGrid[row][column] == 'X')
                        continue;
                    
                    height[row].Add(column, GetDepth(row, column));
                }
            }

            Height = height;
        }

        public Dictionary<int, Dictionary<int, double?>> GetStackMap() {
            Dictionary<int, Dictionary<int, double?>> map = new Dictionary<int, Dictionary<int, double?>>();

            for(int row = 0; row < FloorGrid.Length; row++) {
                map.Add(row, new Dictionary<int, double?>());

                for(int column = 0; column < FloorGrid[row].Length; column++) {
                    if(FloorGrid[row][column] == 'X')
                        continue;

                    GameRoomFurniture roomFurniture = GetFurniture(row, column);

                    if(roomFurniture != null && !roomFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                        continue;

                    map[row].Add(column, GetDepth(row, column));
                }
            }

            return map;
        }
    }
}