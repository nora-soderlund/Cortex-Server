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

namespace Server.Game.Rooms {
    class GameRoomEvents {
        public GameRoom Room;

        public Timer Timer;

        public Dictionary<GameRoomUser, List<IGameRoomUserAction>> Users = new Dictionary<GameRoomUser, List<IGameRoomUserAction>>();

        public void AddUser(GameRoomUser user, IGameRoomUserAction action) {
            if(Users.ContainsKey(user)) {
                IGameRoomUserAction duplicate = Users[user].Find(x => x.Key == action.Key);

                if(duplicate != null)
                    Users[user].Remove(duplicate);
            }
            else
                Users.Add(user, new List<IGameRoomUserAction>());

            Users[user].Add(action);

            Start();
        }

        public Dictionary<GameRoomFurniture, List<IGameRoomFurnitureAction>> Furnitures = new Dictionary<GameRoomFurniture, List<IGameRoomFurnitureAction>>();

        public void AddFurniture(GameRoomFurniture furniture, IGameRoomFurnitureAction action) {
            if(Furnitures.ContainsKey(furniture)) {
                IGameRoomFurnitureAction duplicate = Furnitures[furniture].Find(x => x.Key == action.Key);

                if(duplicate != null)
                    Furnitures[furniture].Remove(duplicate);
            }
            else
                Furnitures.Add(furniture, new List<IGameRoomFurnitureAction>());

            Furnitures[furniture].Add(action);

            Start();
        }
        
        public GameRoomEvents(GameRoom room) {
            Room = room;

            Timer = new Timer(500);

            Timer.Elapsed += OnTimerElapse;
        }

        public void Start() {
            if(Timer.Enabled)
                return;

            Timer.Start();

            OnTimerElapse();
        }

        public void OnTimerElapse(Object source = null, ElapsedEventArgs eventArgs = null) {
            SocketMessage message = new SocketMessage();
            
            Dictionary<int, object> messageUsers = new Dictionary<int, object>();

            foreach(KeyValuePair<GameRoomUser, List<IGameRoomUserAction>> user in Users) {
                Dictionary<string, object> changes = new Dictionary<string, object>();
                
                for(int index = 0; index < user.Value.Count; index++) {
                    int value = user.Value[index].Execute();

                    if(value == 0) {
                        user.Value.Remove(user.Value[index]);

                        continue;
                    }

                    changes.TryAdd(user.Value[index].Key, user.Value[index].Value);

                    if(value == -1)
                        user.Value.Remove(user.Value[index]);
                }

                if(changes.Count != 0)
                    messageUsers.Add(user.Key.Id, changes);
                
                if(user.Value.Count == 0)
                    Users.Remove(user.Key);
            }

            Dictionary<int, object> messageFurnitures = new Dictionary<int, object>();

            foreach(KeyValuePair<GameRoomFurniture, List<IGameRoomFurnitureAction>> furniture in Furnitures) {
                Dictionary<string, object> changes = new Dictionary<string, object>();
                
                for(int index = 0; index < furniture.Value.Count; index++) {
                    int value = furniture.Value[index].Execute();

                    if(value == 0) {
                        furniture.Value.Remove(furniture.Value[index]);

                        continue;
                    }

                    changes.TryAdd(furniture.Value[index].Key, furniture.Value[index].Value);

                    if(value == -1)
                        furniture.Value.Remove(furniture.Value[index]);
                }

                if(changes.Count != 0)
                    messageFurnitures.Add(furniture.Key.Id, changes);
                
                if(furniture.Value.Count == 0)
                    Furnitures.Remove(furniture.Key);
            }

            if(messageUsers.Count != 0 || messageFurnitures.Count != 0)
                message.Add("OnRoomEntityUpdate", new { users = messageUsers, furnitures = messageFurnitures });

            Room.Send(message.Compose());

            if(Users.Count == 0 && Furnitures.Count == 0)
                Timer.Stop();
        }
    }
}