using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Furnitures;
using Server.Game.Users.Furnitures;

using Server.Events;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureLogics {
        public static IGameRoomFurnitureLogic CreateLogic(GameRoomFurniture furniture) {
            switch(furniture.UserFurniture.Furniture.Logic) {
                case "furniture_multistate": return new GameRoomFurnitureMultistateLogic() { Furniture = furniture };
                case "furniture_change_state_when_step_on": return new GameRoomFurnitureChangeStateWhenStepOnLogic() { Furniture = furniture };
            }

            return null;
        }
    }
}
