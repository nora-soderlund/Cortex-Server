using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Chat.Commands;

namespace Server.Game.Rooms.Chat {
    class GameRoomChat : IInitializationEvent {
        public static List<IGameRoomChatCommand> Commands = new List<IGameRoomChatCommand>();

        public void OnInitialization() {
            foreach (var instance in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(a => a.GetConstructor(Type.EmptyTypes) != null).Select(Activator.CreateInstance).OfType<IGameRoomChatCommand>())
                Commands.Add(instance);
        }

        public static void Call(GameRoomUser user, string input) {
            try {
                if(input[0] == ':') {
                    string[] parameters = input.Split(' ');

                    IGameRoomChatCommand command = Commands.Find(x => x.Command == parameters[0]);

                    if(command != null)
                        if(command.Execute(user, parameters))
                            return;
                }

                user.User.Room.Send(new SocketMessage("OnRoomUserChat", new {
                    id = user.Id,

                    message = input
                }).Compose());
            }
            catch(Exception exception) {
                Console.Exception(exception);

                user.User.Room.Send(new SocketMessage("OnRoomUserChat", new {
                    id = user.Id,

                    message = input
                }).Compose());
            }
        }
    }
}
