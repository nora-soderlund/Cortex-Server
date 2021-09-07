using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Events;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Game.Rooms.Settings {
    class GameRoomSettings {
        class OnRoomSettingsUpdate : ISocketEvent {
            public string Event => "OnRoomSettingsUpdate";

            public int Execute(SocketClient client, JToken data) {
                if(client.User.Room == null)
                    return 0;
                    
                if(client.User.Id != client.User.Room.User)
                    return 0;

                if(data["map"] != null) {
                    client.User.Room.Map = new Map.GameRoomMap(client.User.Room, data["map"]["floor"].ToString(), client.User.Room.Map.Door);   

                    client.User.Room.Send(new SocketMessage("OnRoomSettingsUpdate", new { map = client.User.Room.Map }).Compose());

                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        using(MySqlCommand command = new MySqlCommand("UPDATE rooms SET map = @map WHERE id = @id", connection)) {
                            command.Parameters.AddWithValue("@id", client.User.Room.Id);
                            command.Parameters.AddWithValue("@map", data["map"]["floor"].ToString());

                            command.ExecuteNonQuery();
                        }
                    }
                }

                if(data["title"] != null) {
                    string title = data["title"].ToString();

                    if(title.Length == 0)
                        title = "No room title...";

                    client.User.Room.SetTitle(title);
                }

                if(data["description"] != null) {
                    string description = data["description"].ToString();

                    if(description.Length == 0)
                        description = "No room description...";

                    client.User.Room.SetDescription(description);
                }

                if(data["floor"] != null) {
                    if(data["floor"]["material"] != null)
                        client.User.Room.FloorMaterial = data["floor"]["material"].ToString();

                    if(data["floor"]["thickness"] != null)
                        client.User.Room.FloorThickness = data["floor"]["thickness"].ToObject<int>();

                    client.User.Room.Send(new SocketMessage("OnRoomSettingsUpdate", new { floor_material = client.User.Room.FloorMaterial, floor_thickness = client.User.Room.FloorThickness }).Compose());

                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        using(MySqlCommand command = new MySqlCommand("UPDATE rooms SET floor_material = @floor_material, floor_thickness = @floor_thickness WHERE id = @id", connection)) {
                            command.Parameters.AddWithValue("@id", client.User.Room.Id);

                            command.Parameters.AddWithValue("@floor_material", client.User.Room.FloorMaterial);
                            command.Parameters.AddWithValue("@floor_thickness", client.User.Room.FloorThickness);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                if(data["wall"] != null) {
                    if(data["wall"]["material"] != null)
                        client.User.Room.WallMaterial = data["wall"]["material"].ToString();

                    if(data["wall"]["thickness"] != null)
                        client.User.Room.WallThickness = data["wall"]["thickness"].ToObject<int>();

                    client.User.Room.Send(new SocketMessage("OnRoomSettingsUpdate", new { wall_material = client.User.Room.WallMaterial, wall_thickness = client.User.Room.WallThickness }).Compose());

                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        using(MySqlCommand command = new MySqlCommand("UPDATE rooms SET wall_material = @wall_material, wall_thickness = @wall_thickness WHERE id = @id", connection)) {
                            command.Parameters.AddWithValue("@id", client.User.Room.Id);
                            
                            command.Parameters.AddWithValue("@wall_material", client.User.Room.WallMaterial);
                            command.Parameters.AddWithValue("@wall_thickness", client.User.Room.WallThickness);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return 1;
            }
        }
    }
}
