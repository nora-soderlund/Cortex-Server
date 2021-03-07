using System;

using Newtonsoft.Json;

namespace Server.Game.Rooms {
    class GameRoomDirection {
        public static int FromPosition(GameRoomPoint from, GameRoomPoint to) {
            if((from.Row - 1 == to.Row) && (from.Column == to.Column))
                return 0;
            
            if((from.Row - 1 == to.Row) && (from.Column + 1 == to.Column))
                return 1;
            
            if((from.Row == to.Row) && (from.Column + 1 == to.Column))
                return 2;
            
            if((from.Row + 1 == to.Row) && (from.Column + 1 == to.Column))
                return 3;
            
            if((from.Row + 1 == to.Row) && (from.Column == to.Column))
                return 4;
            
            if((from.Row + 1 == to.Row) && (from.Column - 1 == to.Column))
                return 5;
            
            if((from.Row == to.Row) && (from.Column - 1 == to.Column))
                return 6;
            
            if((from.Row - 1 == to.Row) && (from.Column - 1 == to.Column))
                return 7;

            return 0;
        }
    }
}
