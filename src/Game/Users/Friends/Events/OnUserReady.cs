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
    class OnUserReady : ISocketEvent {
        public string Event => "OnUserReady";

        public int Execute(SocketClient client, JToken data) {
            List<object> friends = new List<object>();

            foreach(GameUserFriend friend in client.User.Friends) {
                if(friend.Status == GameUserFriendStatus.Sent)
                    continue;

                GameUser friendUser = Game.GetUser(friend.Friend);

                if(friendUser == null)
                    continue;

                friends.Add(new {
                    id = friend.Friend,

                    status = friend.Status
                });

                GameUserFriend friendUserFriend = friendUser.Friends.Find(x => x.Friend == client.User.Id);

                if(friendUserFriend.Status != GameUserFriendStatus.Sent) {
                    friendUser.Client.Send(new SocketMessage("OnUserFriendUpdate", new {
                        id = client.User.Id,

                        status = friendUserFriend.Status
                    }).Compose());
                }
            }

            client.Send(new SocketMessage("OnUserFriendUpdate", friends).Compose());

            return 1;
        }
    }
}
