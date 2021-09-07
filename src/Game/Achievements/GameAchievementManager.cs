using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Achievements;

using Cortex.Server.Events;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Achievements {
    class GameAchievements {
        public static string BattleBallTilesLocked = "BattleBallTilesLocked";
    }

    class GameAchievementManager : IInitializationEvent {
        public static List<GameAchievement> Achievements = new List<GameAchievement>();

        public void OnInitialization() {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM achievements", connection);

            using MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
                Achievements.Add(new GameAchievement(reader));
        }

        public static GameUserAchievement CreateScore(GameUser user, string achievement) {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("INSERT INTO user_achievements (user, achievement) VALUES (@user, @achievement)", connection);
            command.Parameters.AddWithValue("@user", user.Id);
            command.Parameters.AddWithValue("@achievement", achievement);

            command.ExecuteNonQuery();

            GameUserAchievement userAchievement = new GameUserAchievement() {
                Id = (int)command.LastInsertedId,
                Achievement = achievement,
                Score = 0
            };

            user.Achievements.Add(userAchievement);

            return userAchievement;
        }

        public static void AddScore(GameUser user, string id, int score) {
            GameUserAchievement userAchievement = user.Achievements.Find(x => x.Achievement == id);

            if(userAchievement == null)
                userAchievement = CreateScore(user, id);

            userAchievement.Score += score;

            GameAchievement achievement = Achievements.Find(x => x.Id == id);

            if(userAchievement.Level != achievement.LevelCount) {
                GameAchievementLevel achievementLevel = achievement.Levels.Find(x => x.Level == userAchievement.Level + 1);

                if(achievementLevel != null) {
                    if(userAchievement.Score >= achievementLevel.Score) {
                        user.Client.Send(new SocketMessage("OnAchievementUnlocked", new { achievement, level = achievementLevel }).Compose());

                        userAchievement.Level = achievementLevel.Level;
                    }
                }
            }

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("UPDATE user_achievements SET score = @score, level = @level WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", userAchievement.Id);
            command.Parameters.AddWithValue("@score", userAchievement.Score);
            command.Parameters.AddWithValue("@level", userAchievement.Level);
            
            command.ExecuteNonQuery();
        }
    }
}
