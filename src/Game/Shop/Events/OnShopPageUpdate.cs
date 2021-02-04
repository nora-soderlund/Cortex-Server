using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Shop.Events {
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
