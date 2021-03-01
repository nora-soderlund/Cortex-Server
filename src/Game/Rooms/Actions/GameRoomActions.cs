using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Actions {
    class GameRoomActions {
        public GameRoom Room;
        
        public GameRoomActions(GameRoom room) {
            Room = room;
        }

        public Dictionary<int, Timer> Timers = new Dictionary<int, Timer>();

        public Dictionary<int, Dictionary<string, Dictionary<string, List<IGameRoomAction>>>> Actions = new Dictionary<int, Dictionary<string, Dictionary<string, List<IGameRoomAction>>>>();

        public void Add(int interval, string header, string subHeader, IGameRoomAction action) {
            if(!Actions.ContainsKey(interval))
                Actions.Add(interval, new Dictionary<string, Dictionary<string, List<IGameRoomAction>>>());

            if(!Actions[interval].ContainsKey(header))
                Actions[interval].Add(header, new Dictionary<string, List<IGameRoomAction>>());

            if(!Actions[interval][header].ContainsKey(subHeader))
                Actions[interval][header].Add(subHeader, new List<IGameRoomAction>());
            else
                Actions[interval][header][subHeader].Clear();

            Actions[interval][header][subHeader].Add(action);

            Start(interval);
        }

        public void Start(int interval) {
            if(Timers.ContainsKey(interval)) {
                if(Timers[interval].Enabled == true)
                    return;

                Timers[interval].Start();

                return;
            }

            Timers[interval] = new Timer(interval) { Enabled = true };

            Timers[interval].Elapsed += (source, args) => {
                Elapse(interval);
            };

            Elapse(interval);
        }

        public void Elapse(int interval) {
            if(!Actions.ContainsKey(interval))
                return;

            SocketMessage message = new SocketMessage();

            foreach(KeyValuePair<string, Dictionary<string, List<IGameRoomAction>>> headers in Actions[interval]) {
                Dictionary<string, List<object>> messages = new Dictionary<string, List<object>>();

                foreach(KeyValuePair<string, List<IGameRoomAction>> subHeader in headers.Value) {
                    messages.Add(subHeader.Key, new List<object>());

                    for(int index = 0; index < subHeader.Value.Count; index++) {
                        int result = subHeader.Value[index].Execute();

                        if(result == 0) {
                            subHeader.Value.Remove(subHeader.Value[index]);

                            continue;
                        }

                        messages[subHeader.Key].Add(subHeader.Value[index].Result);

                        if(result == -1) {
                            subHeader.Value.Remove(subHeader.Value[index]);
                        }
                    }

                    if(messages[subHeader.Key].Count == 0)
                        messages.Remove(subHeader.Key);
                }

                if(messages.Count != 0)
                    message.Add(headers.Key, messages);

                if(headers.Value.Count == 0)
                    Actions[interval].Remove(headers.Key);
            }

            Room.Send(message.Compose());

            if(Actions[interval].Count == 0) {
                Actions.Remove(interval);
                
                Timers[interval].Stop();

                Timers.Remove(interval);
            }
        }
    }
}