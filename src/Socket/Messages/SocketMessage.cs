using System;
using System.Linq;
using System.Collections.Generic;

using Fleck;

using Newtonsoft.Json;

namespace Server.Socket.Messages {
    class SocketMessage {
        private JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public SocketMessage() { }
        public SocketMessage(string header, object content = null) {
            Add(header, content);
        }

        public Dictionary<string, object> events = new Dictionary<string, object>();

        public void Add(string header, object content = null) {
            events[header] = content;
        }

        public string Compose() {
            return JsonConvert.SerializeObject(events, Formatting.None, settings);
        }
    }
}
