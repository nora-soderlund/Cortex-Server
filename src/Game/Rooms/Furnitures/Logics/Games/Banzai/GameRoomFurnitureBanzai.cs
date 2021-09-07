using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;

using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Users.Actions;

using Cortex.Server.Game.Rooms.Furnitures.Actions;

using Cortex.Server.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureBanzai {
        public static int GetUserTeam(GameRoomUser user) {
            if(user.Effect < 33 || user.Effect > 36)
                return 0;

            return user.Effect - 32;
        }

        public static void OnGameStart(GameRoomFurniture counter) {
            if(counter.UserFurniture.Furniture.Line != "battle_banzai")
                return;
        }

        public static void OnGameStop(GameRoomFurniture counter) {
            if(counter.UserFurniture.Furniture.Line != "battle_banzai")
                return;
                
            List<KeyValuePair<int, int>> teams = new List<KeyValuePair<int, int>>();

            foreach(GameRoomFurniture furniture in counter.Room.Furnitures.Where(x => x.Logic is GameRoomFurnitureBanzaiScore)) {
                GameRoomFurnitureBanzaiScore furnitureScore = furniture.Logic as GameRoomFurnitureBanzaiScore;

                teams.Add(new KeyValuePair<int, int>(furnitureScore.Team, furnitureScore.Score));
            }

            teams = teams.OrderByDescending(x => x.Value).ToList();

            List<int> winners = new List<int>();

            foreach(KeyValuePair<int, int> team in teams) {
                if(team.Value == teams[0].Value)
                    winners.Add(team.Key);
            }

            foreach(int team in winners) {
                foreach(GameRoomUser user in counter.Room.Users.Where(x => GetUserTeam(x) == team))
                    counter.Room.Actions.AddEntity(user.Id, new GameRoomUserAction(user, "Wave", 5000));
            }

            foreach(GameRoomFurniture furniture in counter.Room.Furnitures.Where(x => x.Logic is GameRoomFurnitureBanzaiTile))
                furniture.Animation = 0;

            int animation = (winners.Count == 1)?((winners[0] * 3) + 2):(1);
            
            counter.Room.Send(new SocketMessage("OnRoomFurnitureFlash", new { id = "bb_patch1", animation }).Compose());
        }
    }
}
