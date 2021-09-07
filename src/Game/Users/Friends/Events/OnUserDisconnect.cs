using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Users.Friends.Events {
    class OnUserDisconnect : ISocketEvent {
        public string Event => "OnUserDisconnect";

        public int Execute(SocketClient client, JToken data) {
            foreach(GameUserFriend friend in client.User.Friends) {
                GameUser friendUser = Game.GetUser(friend.Friend);

                if(friendUser == null)
                    continue;

                GameUserFriend friendUserFriend = friendUser.Friends.Find(x => x.Friend == client.User.Id);

                if(friendUserFriend.Status != GameUserFriendStatus.Sent) {
                    friendUser.Client.Send(new SocketMessage("OnUserFriendRemove", client.User.Id).Compose());
                }
            }

            return 1;
        }
    }
}
