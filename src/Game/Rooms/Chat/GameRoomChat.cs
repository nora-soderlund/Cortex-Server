using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Events;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Users.Actions;
using Cortex.Server.Game.Rooms.Chat.Commands;

namespace Cortex.Server.Game.Rooms.Chat {
    class GameRoomChat : IInitializationEvent {
        public static List<IGameRoomChatCommand> Commands = new List<IGameRoomChatCommand>();

        public void OnInitialization() {
            foreach (var instance in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(a => a.GetConstructor(Type.EmptyTypes) != null).Select(Activator.CreateInstance).OfType<IGameRoomChatCommand>())
                Commands.Add(instance);

            Console.WriteLine("Loaded " + Commands.Count + " room chat commands...");
        }

        public static Dictionary<string, string> Actions = new Dictionary<string, string>() {
            { "o/", "Wave" },
            { ":D", "Laugh" },
            { ":)", "GestureSmile" },
            { ":(", "GestureSad" },
            { ":@", "GestureAngry" },
            { ":O", "GestureSurprised" }
        };

        public static void Call(GameRoomUser user, string input) {
            try {
                if(input[0] == ':' || input[0] == '/') {
                    string[] parameters = input.Substring(1).Split(' ');

                    IGameRoomChatCommand command = Commands.Find(x => x.Command == parameters[0]);

                    if(command != null)
                        if(command.Execute(user, parameters))
                            return;
                }

                user.User.Room.Send(new SocketMessage("OnRoomUserChat", new {
                    id = user.Id,

                    message = input
                }).Compose());

                foreach(KeyValuePair<string, string> action in Actions) {
                    if(input.Contains(action.Key)) {
                        user.User.Room.Actions.AddEntity(user.Id, new GameRoomUserAction(user, action.Value, 2000));
                        
                        break;
                    }
                }
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
