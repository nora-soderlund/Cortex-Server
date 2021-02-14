using System;

using Newtonsoft.Json;

namespace Server.Game.Rooms {
    class GameRoomPoint {
        [JsonProperty("row")]
        public double? Row;

        [JsonProperty("column")]
        public double? Column;

        [JsonProperty("depth")]
        public double? Depth;

        [JsonProperty("direction")]
        public int? Direction;

        public GameRoomPoint(double? row = null, double? column = null, double? depth = null, int? direction = null) {
            Row = row;
            Column = column;
            Depth = depth;
            Direction = direction;
        }

        public GameRoomPoint(int row, int column, double? depth, int direction) {
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
