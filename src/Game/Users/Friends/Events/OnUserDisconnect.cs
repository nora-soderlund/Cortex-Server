using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Friends.Events {
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
