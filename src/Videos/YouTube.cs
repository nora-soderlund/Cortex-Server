using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Fleck;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using Server.Game.Users;

using VideoLibrary;

namespace Server.Videos {
    class YouTubeManager {
        public static string Get(string id) {
            string link = "https://www.youtube.com/watch?v=" + id;

            YouTubeVideo video = YouTube.Default.GetVideo(link);

            File.WriteAllBytes(Program.Config["youtube"]["path"].ToString() + "/" + id + ".mp4", video.GetBytes());

            return id;
        }
    }
}
