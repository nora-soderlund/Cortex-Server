using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Users.Furnitures;

namespace Server.Game.Furnitures {
    class GameFurnitureManager {
        public static List<GameFurniture> Furnitures = new List<GameFurniture>();
        public static List<GameUserFurniture> UserFurnitures = new List<GameUserFurniture>();

        public static GameFurniture GetGameFurniture(string id) {
            GameFurniture furniture = Furnitures.Find(x => x.Id == id);

            if(furniture != null)
                return furniture;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM furnitures WHERE id = @id", connection);

            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if(!reader.Read())
                return null;

            furniture = new GameFurniture(reader);

            Furnitures.Add(furniture);

            return furniture;
        }

        public static GameUserFurniture GetGameUserFurniture(int id) {
            GameUserFurniture furniture = UserFurnitures.Find(x => x.Id == id);

            if(furniture != null)
                return furniture;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM user_furnitures WHERE id = @id", connection);

            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if(!reader.Read())
                return null;

            furniture = new GameUserFurniture(reader);

            UserFurnitures.Add(furniture);

            return furniture;
        }
    }
}
