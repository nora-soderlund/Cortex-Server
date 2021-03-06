using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Rooms.Furnitures.Logics;

using Server.Game.Furnitures;
using Server.Game.Users.Furnitures;

namespace Server.Game.Rooms.Furnitures {
    class GameRoomFurniture {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("position")]
        public GameRoomPoint Position;

        [JsonProperty("furniture")]
        public string Furniture;

        [JsonProperty("animation")]
        public int? Animation = null;

        [JsonIgnore]
        public GameRoom Room;

        [JsonIgnore]
        public GameUserFurniture UserFurniture;

        [JsonIgnore]
        public IGameRoomFurnitureLogic Logic;

        public GameRoomFurniture(GameRoom room, MySqlDataReader furniture) {
            Room = room;

            Id = furniture.GetInt32("id");

            UserFurniture = GameFurnitureManager.GetGameUserFurniture(furniture.GetInt32("furniture"));

            Furniture = UserFurniture.Furniture.Id;

            Position = new GameRoomPoint(furniture.GetInt32("row"), furniture.GetInt32("column"), furniture.GetDouble("depth"), furniture.GetInt32("direction"));
            
            Animation = (furniture.GetInt32("animation") != 0)?(furniture.GetInt32("animation")):(null);

            Logic = GameRoomFurnitureLogics.CreateLogic(this);
        }

        public GameRoomFurniture(GameRoom room, int id, int userFurniture, GameRoomPoint position) {
            Room = room;
            
            Id = id;

            UserFurniture = GameFurnitureManager.GetGameUserFurniture(userFurniture);

            Furniture = UserFurniture.Furniture.Id;

            Position = position;

            Logic = GameRoomFurnitureLogics.CreateLogic(this);
        }

        public GameRoomPoint GetDimension() {
            GameRoomPoint dimension = new GameRoomPoint(UserFurniture.Furniture.Dimension);

            if(Position.Direction == 0 || Position.Direction == 4) {
                int spare = dimension.Row;

                dimension.Row = dimension.Column;
                dimension.Column = spare;
            }

            return dimension;
        }

        public void SetAnimation(int animation) {
            Animation = animation;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET animation = @animation WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@animation", Animation);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
