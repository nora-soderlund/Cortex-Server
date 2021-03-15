using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Socket.Clients;

using Server.Game.Rooms;
using Server.Game.Furnitures;
using Server.Game.Users.Friends;
using Server.Game.Users.Furnitures;
using Server.Game.Users.Badges;
using Server.Game.Users.Achievements;

namespace Server.Game.Users {
    class GameUser {
        [JsonIgnore]
        public int Id;

        [JsonIgnore]
        public SocketClient Client;
        
        [JsonIgnore]
        public string Name;

        [JsonProperty("home")]
        public int? Home;

        [JsonProperty("figure")]
        public string Figure;

        [JsonIgnore]
        public GameRoom Room = null;

        [JsonIgnore]
        public List<GameUserFurniture> Furnitures = new List<GameUserFurniture>();

        [JsonProperty("friends")]
        public List<GameUserFriend> Friends = new List<GameUserFriend>();

        [JsonIgnore]
        public List<GameUserBadge> Badges = new List<GameUserBadge>();

        [JsonIgnore]
        public List<GameUserAchievement> Achievements = new List<GameUserAchievement>();

        public GameUser(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Figure = reader.GetString("figure");

            Name = reader.GetString("name");

            if((Home = reader.GetInt32("home")) == 0)
                Home = null;

            GetFurnitures();

            Friends = GetFriends();

            Badges = GetBadges();

            Achievements = GetAchievements();
        }

        public List<GameUserFriend> GetFriends() {
            List<GameUserFriend> friends = new List<GameUserFriend>();

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("SELECT * FROM user_friends WHERE user = @user", connection)) {
                    command.Parameters.AddWithValue("@user", Id);

                    MySqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                        friends.Add(new GameUserFriend(reader));
                }
            }

            return friends;
        }

        public void GetFurnitures() {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM user_furnitures WHERE user = @user", connection);
            command.Parameters.AddWithValue("@user", Id);

            MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read()) {
                Furnitures.Add(GameFurnitureManager.GetGameUserFurniture(reader.GetInt32("id")));
            }
        }

        public Dictionary<string, Dictionary<string, int>> GetFurnitureMessages(string id = "") {
            Dictionary<string, Dictionary<string, int>> furnitures = new Dictionary<string, Dictionary<string, int>>();

            foreach(GameUserFurniture furniture in (id.Length == 0)?(Furnitures):(Furnitures.Where(x => x.Furniture.Id == id))) {
                if(!furnitures.ContainsKey(furniture.Furniture.Id))
                    furnitures.Add(furniture.Furniture.Id, new Dictionary<string, int>());

                if(furniture.Room != 0) {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("room"))
                        furnitures[furniture.Furniture.Id].Add("room", 0);

                    furnitures[furniture.Furniture.Id]["room"]++;
                }
                else {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("inventory"))
                        furnitures[furniture.Furniture.Id].Add("inventory", 0);

                    furnitures[furniture.Furniture.Id]["inventory"]++;
                }
            }

            return furnitures;
        }
    
        public List<GameUserBadge> GetBadges() {
            List<GameUserBadge> badges = new List<GameUserBadge>();

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM user_badges WHERE user = @user", connection);
            command.Parameters.AddWithValue("@user", Id);

            using MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
                badges.Add(new GameUserBadge(reader));

            return badges;
        }

        public List<GameUserAchievement> GetAchievements() {
            List<GameUserAchievement> achievements = new List<GameUserAchievement>();

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM user_achievements WHERE user = @user", connection);
            command.Parameters.AddWithValue("@user", Id);

            using MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
                achievements.Add(new GameUserAchievement(reader));

            return achievements;
        }
    }
}
