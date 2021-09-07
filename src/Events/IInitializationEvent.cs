using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json.Linq;

using Cortex.Server.Socket.Clients;

namespace Cortex.Server.Events {
    interface IInitializationEvent {
        void OnInitialization();   
    }
}
