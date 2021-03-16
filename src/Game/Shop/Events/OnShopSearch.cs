using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using Server.Game.Furnitures;
using Server.Game.Shop.Furnitures;

namespace Server.Game.Shop.Events {
    class OnShopSearch : ISocketEvent {
        public string Event => "OnShopSearch";

        public int Execute(SocketClient client, JToken data) {
            string input = data.ToString().ToLower();

            List<GameShopPage> pages = new List<GameShopPage>();

            foreach(GameShopPage page in Shop.GameShop.Pages.FindAll(x => (x.Title != null && x.Title.ToLower().Contains(input)) || (x.Description != null && x.Description.ToLower().Contains(input)))) {
                if(pages.Count > 32)
                    break;

                pages.Add(page);
            }

            List<GameShopFurniture> furnitures = new List<GameShopFurniture>();

            foreach(GameShopFurniture furniture in Shop.GameShop.Furnitures.FindAll(x => x.Furniture.Id.ToLower().Contains(input) || (x.Furniture.Title != null && x.Furniture.Title.ToLower().Contains(input)) || (x.Furniture.Description != null && x.Furniture.Description.ToLower().Contains(input)))) {
                if(furnitures.Count > 32)
                    break;

                furnitures.Add(furniture);
            }

            client.Send(new SocketMessage("OnShopSearch", new {
                pages, furnitures
            }).Compose());

            return 1;
        }
    }
}
