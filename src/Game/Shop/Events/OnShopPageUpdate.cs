using System;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Shop.Events {
    class OnShopPageUpdate : ISocketEvent {
        public string Event => "OnShopPageUpdate";

        public int Execute(SocketClient client, JToken data) {
            int id = data.ToObject<int>();

            GameShopPage page = GameShop.Pages.Find(x => x.Id == id);

            client.Send(new SocketMessage("OnShopPageUpdate", new {
                type = page.Type,
                
                description = page.Description,

                header = page.Header,
                teaser = page.Teaser,
                content = page.Content
            }).Compose());

            return 1;
        }
    }
}
