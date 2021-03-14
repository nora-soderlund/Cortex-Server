using System;

using Newtonsoft.Json;

namespace Server.Game.Rooms {
    class GameRoomPoint {
        [JsonProperty("row")]
        public int Row;

        [JsonProperty("column")]
        public int Column;

        [JsonProperty("depth")]
        public double Depth;

        [JsonProperty("direction")]
        public int Direction;

        public GameRoomPoint(int row = 0, int column = 0, double depth = 0.0, int direction = 0) {
            Row = row;
            Column = column;
            Depth = depth;
            Direction = direction;
        }

        public GameRoomPoint(GameRoomPoint point) {
            Row = point.Row;
            Column = point.Column;
            Depth = point.Depth;
            Direction = point.Direction;
        }

        public int GetDistance(GameRoomPoint point) {
            return (int)Math.Sqrt(Math.Pow(((double)point.Row - (double)Row), 2) + Math.Pow(((double)point.Column - (double)Column), 2));
        }

        public void FromDirection(int direction) {
            switch(direction) {
                case 0: {
                    Row--;

                    break;
                }
                
                case 1: {
                    Row--;
                    Column++;

                    break;
                }

                case 2: {
                    Column++;

                    break;
                }
                
                case 3: {
                    Row++;
                    Column++;

                    break;
                }
                
                case 4: {
                    Row++;

                    break;
                }
                
                case 5: {
                    Row++;
                    Column--;

                    break;
                }
                
                case 6: {
                    Column--;

                    break;
                }
                
                case 7: {
                    Row--;
                    Column--;

                    break;
                }
            }
        }
    }

    enum GameRoomPointDirections {
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    }
}
