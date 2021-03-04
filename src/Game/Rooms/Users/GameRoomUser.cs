using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Server.Game.Users;
using Server.Game.Rooms.Users.Actions;
using Server.Game.Rooms.Actions;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Users {
    class GameRoomUser {
        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        public GameUser User;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("position")]
        public GameRoomPoint Position;

        [JsonProperty("figure")]
        public string Figure;

        [JsonProperty("actions")]
        public List<string> Actions = new List<string>();

        public GameRoomUser(GameUser user) {
            Id = user.Id;
            
            User = user;

            Name = user.Name;

            Position = new GameRoomPoint();

            Figure = user.Figure;

            //User.Room.Events.AddUser(this, new GameRoomUserAction(this, "GestureAngry", GameRoomUserActionType.Add));
        }
    }
}
