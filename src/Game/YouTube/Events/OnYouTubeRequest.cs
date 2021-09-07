using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

using VideoLibrary;

namespace Cortex.Server.Game.Users.Events {
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

                reader.Close();
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
