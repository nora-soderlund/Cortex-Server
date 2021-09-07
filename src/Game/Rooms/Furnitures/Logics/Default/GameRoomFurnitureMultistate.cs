using System;
using System.Linq;
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
    class GameRoomFurnitureMultistate : IGameRoomFurnitureLogic {
        public GameRoomFurniture Furniture { get; set; }

        public void OnUserUse(GameRoomUser user, JToken data) {
            if(!user.HasRights())
                return;
                
            int animation = data["animation"].ToObject<int>();
            
            user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", Furniture.Id).Compose());

            Furniture.SetAnimation(animation, true);
        }
    }
}
