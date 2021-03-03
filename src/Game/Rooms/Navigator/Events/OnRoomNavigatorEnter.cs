using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

using Server.Game.Rooms.Navigator;

namespace Server.Game.Rooms.Navigator.Events {
    class OnRoomNavigatorEnter : ISocketEvent {
        public string Event => "OnRoomNavigatorEnter";

        public int Execute(SocketClient client, JToken data) {
            int id = Int32.Parse(data.ToString());

            GameRoomNavigator roomNavigator = GameRoomManager.Navigator.FirstOrDefault(x => x.Id == id);

            if(roomNavigator == null)
                return 0;

            switch((NavigatorRoomAccess)roomNavigator.Access) {
                case NavigatorRoomAccess.Public: {
                    GameRoomManager.AddUser(id, client.User);

                    break;
                }

                case NavigatorRoomAccess.Private: {

                    break;
                }

                case NavigatorRoomAccess.Password: {

                    break;
                }
            }

            return 1;
        }
    }
}
