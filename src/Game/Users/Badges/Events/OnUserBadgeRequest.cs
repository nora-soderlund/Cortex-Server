using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Badges.Events {
    class OnUserBadgeRequest : ISocketEvent {
        public string Event => "OnUserBadgeRequest";

        public int Execute(SocketClient client, JToken data) {
            int id = data.ToObject<int>();

            GameUser user = Game.GetUser(id);

            if(user != null) {
                client.Send(new SocketMessage("OnUserBadgeRequest", user.Badges.Where(x => x.Equipped)).Compose());

                return 1;
            }

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("SELECT * FROM user_badges WHERE user = @user AND equipped = true", connection)) {
                    command.Parameters.AddWithValue("@user", id);

                    List<GameUserBadge> badges = new List<GameUserBadge>();

                    using MySqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                        badges.Add(new GameUserBadge(reader));

                    client.Send(new SocketMessage("OnUserBadgeRequest", badges).Compose());
                }
            }

            return 1;
        }
    }
}
