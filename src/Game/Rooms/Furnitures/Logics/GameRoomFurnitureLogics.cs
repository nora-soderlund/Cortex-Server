using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Game.Furnitures;
using Cortex.Server.Game.Users.Furnitures;

using Cortex.Server.Events;

namespace Cortex.Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureLogics {
        public static IGameRoomFurnitureLogic CreateLogic(GameRoomFurniture furniture) {
            switch(furniture.UserFurniture.Furniture.Logic) {
                case "furniture_multistate": return new GameRoomFurnitureMultistate() { Furniture = furniture };

                case "furniture_change_state_when_step_on": return new GameRoomFurnitureChangeStateWhenStepOn() { Furniture = furniture };

                case "furniture_queue_tile": return new GameRoomFurnitureQueueTile() { Furniture = furniture };

                case "furniture_dice": return new GameRoomFurnitureDice() { Furniture = furniture };

                case "furniture_gate": return new GameRoomFurnitureGate() { Furniture = furniture };

                case "furniture_video": return new GameRoomFurnitureVideo(furniture);

                case "furniture_banzai_tile": return new GameRoomFurnitureBanzaiTile() { Furniture = furniture };

                case "furniture_banzai_portal": return new GameRoomFurnitureBanzaiPortal(furniture) { Furniture = furniture };

                case "furniture_banzai_score": return new GameRoomFurnitureBanzaiScore(furniture) { Furniture = furniture };

                case "furniture_counter_clock": return new GameRoomFurnitureCounter(furniture) { Furniture = furniture };

                case "furniture_pushable": return new GameRoomFurniturePushable(furniture) { Furniture = furniture };
            }

            return new GameRoomFurnitureBasic() { Furniture = furniture };
        }
    }
}
