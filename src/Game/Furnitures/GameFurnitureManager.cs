using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Game.Furnitures.Videos;

using Cortex.Server.Game.Users.Furnitures;

using VideoLibrary;

namespace Cortex.Server.Game.Furnitures {
    class GameFurnitureManager {
        public static List<GameFurniture> Furnitures = new List<GameFurniture>();
        public static List<GameUserFurniture> UserFurnitures = new List<GameUserFurniture>();
        
        public static List<GameFurnitureVideo> Videos = new List<GameFurnitureVideo>();

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

        public static GameFurnitureVideo GetVideo(string id) {
            GameFurnitureVideo video = Videos.Find(x => x.Id == id);

            if(video != null)
                return video;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM furniture_videos WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if(!reader.Read())
                return DownloadVideo(id);

            video = new GameFurnitureVideo(reader);

            Videos.Add(video);

            return video;
        }

        public static GameFurnitureVideo DownloadVideo(string id) {
            YouTubeVideo video = YouTube.Default.GetVideo("https://www.youtube.com/watch?v=" + id);

            File.WriteAllBytes(Program.Config["youtube"]["path"].ToString() + "/" + id + ".mp4", video.GetBytes());

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("INSERT INTO furniture_videos (id, title, author, length) VALUES (@id, @title, @author, @length)", connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", video.Title);
            command.Parameters.AddWithValue("@author", video.Info.Author);
            command.Parameters.AddWithValue("@length", video.Info.LengthSeconds);

            command.ExecuteNonQuery();

            GameFurnitureVideo furnitureVideo = new GameFurnitureVideo() {
                Id = id,
                Title = video.Title,
                Author = video.Info.Author,
                Length = (int)video.Info.LengthSeconds
            };

            Videos.Add(furnitureVideo);

            return furnitureVideo;
        }
    }
}
