using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json.Linq;

using Server.Socket.Clients;

namespace Server.Events {
    interface IInitializationEvent {
        void OnInitialization();   
    }
}
