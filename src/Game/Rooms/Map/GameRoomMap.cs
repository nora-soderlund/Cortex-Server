using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using RoyT.AStar;

namespace Server.Game.Rooms.Map {
    class GameRoomMap {
        [JsonProperty("floor")]
        public string Floor;

        [JsonIgnore]
        public string[] FloorGrid;

        [JsonIgnore]
        public Grid Grid;

        public GameRoomMap(string floor) {
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
        }

        public Position[] GetPath(GameRoomPoint start, GameRoomPoint end) {
            return Grid.GetPath(new Position((int)start.Row, (int)start.Column), new Position((int)end.Row, (int)end.Column));
        }

        public double GetDepth(int row, int column) {
            if(Char.ToUpper(FloorGrid[row][column]) != Char.ToLower(FloorGrid[row][column]))
                return (double)((FloorGrid[row][column] - 97) - '0');

            return (double)(FloorGrid[row][column] - '0');
        }
    }
}