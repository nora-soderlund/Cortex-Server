using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Fleck;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

using Cortex.Server.Game.Users;

using VideoLibrary;

namespace Cortex.Server.Videos {
    class YouTubeManager {
        public static string Get(string id) {
            string link = "https://www.youtube.com/watch?v=" + id;

            YouTubeVideo video = YouTube.Default.GetVideo(link);

            File.WriteAllBytes(Program.Config["youtube"]["path"].ToString() + "/" + id + ".mp4", video.GetBytes());

            return id;
        }
    }
}
