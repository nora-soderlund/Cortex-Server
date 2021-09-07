using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Actions {
    class GameRoomActions {
        public GameRoom Room;

        public Dictionary<int, Dictionary<string, IGameRoomEntityAction>> EntityActions = new Dictionary<int, Dictionary<string, IGameRoomEntityAction>>();
        
        public GameRoomActions(GameRoom room) {
            Room = room;
        }

        public void AddEntity(int id, IGameRoomEntityAction action) {
            if(!EntityActions.ContainsKey(id))
                EntityActions.Add(id, new Dictionary<string, IGameRoomEntityAction>());

            if(EntityActions[id].ContainsKey(action.Property))
                EntityActions[id].Remove(action.Property);

            EntityActions[id].Add(action.Property, action);
        }

        public void Elapse() {
            Dictionary<string, Dictionary<int, Dictionary<string, object>>> entities = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

            Dictionary<int, Dictionary<string, IGameRoomEntityAction>> actions = new Dictionary<int, Dictionary<string, IGameRoomEntityAction>>(EntityActions);

            for(int entityActionIndex = 0; entityActionIndex < actions.Count; entityActionIndex++) {
                KeyValuePair<int, Dictionary<string, IGameRoomEntityAction>> entityAction = actions.ElementAt(entityActionIndex);

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
                    EntityActions.Remove(entityAction.Key);
            }

            if(entities.Count != 0)
                Room.Send(new SocketMessage("OnRoomEntityUpdate", entities).Compose());
        }
    }
}