using System;
using System.Linq;
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

        public Dictionary<int, Dictionary<int, Dictionary<string, IGameRoomEntityAction>>> EntityActions = new Dictionary<int, Dictionary<int, Dictionary<string, IGameRoomEntityAction>>>();

        public void AddEntity(int id, int interval, IGameRoomEntityAction action) {
            if(!EntityActions.ContainsKey(interval))
                EntityActions.Add(interval, new Dictionary<int, Dictionary<string, IGameRoomEntityAction>>());

            if(!EntityActions[interval].ContainsKey(id))
                EntityActions[interval].Add(id, new Dictionary<string, IGameRoomEntityAction>());

            if(EntityActions[interval][id].ContainsKey(action.Property))
                EntityActions[interval][id].Remove(action.Property);

            EntityActions[interval][id].Add(action.Property, action);

            Start(interval);
        }

        public void AddEntityDelay(int id, int interval, IGameRoomEntityAction action) {
            Timer timer = new Timer(interval);

            timer.Elapsed += (a, b) => {
                if(!EntityActions.ContainsKey(0))
                    EntityActions.Add(0, new Dictionary<int, Dictionary<string, IGameRoomEntityAction>>());

                if(!EntityActions[0].ContainsKey(id))
                    EntityActions[0].Add(id, new Dictionary<string, IGameRoomEntityAction>());

                if(EntityActions[0][id].ContainsKey(action.Property))
                    EntityActions[0][id].Remove(action.Property);

                EntityActions[0][id].Add(action.Property, action);

                Start(0);

                timer.Stop();
            };

            timer.Start();
        }

        public void Start(int interval) {
            if(Timers.ContainsKey(interval)) {
                if(Timers[interval].Enabled == true)
                    return;

                Timers[interval].Start();

                return;
            }

            if(interval != 0) {
                Timers[interval] = new Timer(interval) { Enabled = true };

                Timers[interval].Elapsed += (source, args) => {
                    Elapse(interval);
                };
            }

            Elapse(interval);
        }

        public void Elapse(int interval) {
            Dictionary<string, Dictionary<int, Dictionary<string, object>>> entities = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

            for(int entityActionIndex = 0; entityActionIndex < EntityActions[interval].Count; entityActionIndex++) {
                KeyValuePair<int, Dictionary<string, IGameRoomEntityAction>> entityAction = EntityActions[interval].ElementAt(entityActionIndex);

                for(int index = 0; index < entityAction.Value.Count; index++) {
                    KeyValuePair<string, IGameRoomEntityAction> property = entityAction.Value.ElementAt(index);

                    int result = property.Value.Execute();

                    if(result == 0) {
                        entityAction.Value.Remove(property.Key);

                        continue;
                    }

                    if(!entities.ContainsKey(property.Value.Entity))
                        entities.Add(property.Value.Entity, new Dictionary<int, Dictionary<string, object>>());

                    if(!entities[property.Value.Entity].ContainsKey(entityAction.Key))
                        entities[property.Value.Entity].Add(entityAction.Key, new Dictionary<string, object>());

                    entities[property.Value.Entity][entityAction.Key][property.Value.Property] = property.Value.Result;

                    if(result == -1) {
                        entityAction.Value.Remove(property.Key);
                    }
                }

                if(entityAction.Value.Count == 0)
                    EntityActions[interval].Remove(entityAction.Key);
            }

            if(entities.Count != 0)
                Room.Send(new SocketMessage("OnRoomEntityUpdate", entities).Compose());

            if(EntityActions[interval].Count == 0) {
                EntityActions.Remove(interval);
                
                if(Timers.ContainsKey(interval)) {
                    Timers[interval].Stop();

                    Timers.Remove(interval);
                }
            }
        }
    }
}