using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using VideoLibrary;

namespace Server.Game.Users.Events {
    class OnYouTubeRequest : ISocketEvent {
        public string Event => "OnYouTubeRequest";

        public int Execute(SocketClient client, JToken data) {
            if(data == null)
                return 0;

            string id = data.ToString();

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using(MySqlCommand command = new MySqlCommand("SELECT * FROM youtube WHERE id = @id", connection)) {
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader reader = command.ExecuteReader();

                if(reader.Read()) {
                    client.Send(new SocketMessage("OnYouTubeRequest", true).Compose());

                    return 1;
                }
            }

            YouTubeVideo video = YouTube.Default.GetVideo("https://www.youtube.com/watch?v=" + id);

            File.WriteAllBytes(Program.Config["youtube"]["path"].ToString() + "/" + id + ".mp4", video.GetBytes());

            using(MySqlCommand command = new MySqlCommand("INSERT INTO youtube (id, title) VALUES (@id, @title)", connection)) {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", video.Title);

                command.ExecuteNonQuery();

                client.Send(new SocketMessage("OnYouTubeRequest", true).Compose());
            }

            return 1;
        }
    }
}
