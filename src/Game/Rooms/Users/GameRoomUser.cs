using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

namespace Server.Game.Rooms.Users {
    class GameRoomUser {
        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        public GameUser User;

        [JsonProperty("position")]
        public GameRoomPoint Position;

        [JsonProperty("figure")]
        public string Figure;

        [JsonProperty("actions")]
        public List<string> Actions = new List<string>();

        public int AddAction(string action) {
            if(Actions.Contains(action))
                return 0;

            //User.Room.Events.AddUser(this, new GameRoomUserAction(this, action, GameRoomUserActionType.Add));

            return 1;
        }

        public GameRoomUser(GameUser user) {
            Id = user.Id;
            
            User = user;

            Position = new GameRoomPoint();

            Figure = user.Figure;

            //User.Room.Events.AddUser(this, new GameRoomUserAction(this, "GestureAngry", GameRoomUserActionType.Add));
        }
    }
}
