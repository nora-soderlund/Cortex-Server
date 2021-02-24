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
    }
}
