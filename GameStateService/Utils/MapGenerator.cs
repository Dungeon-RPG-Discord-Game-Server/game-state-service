using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameStateService.Models;

namespace GameStateService.Utils
{
    public static class MapGenerator
    {
        private static readonly Random _random = new();

        private static readonly List<(int dx, int dy)> directions = new()
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0)
        };

        public static MapData GenerateMap(string mapName, int roomCount)
        {
            var map = new Dictionary<(int, int), Room>();
            var roomList = new List<Room>();

            var startRoom = new Room
            {
                Id = 0,
                X = 0,
                Y = 0,
                Visited = true,
                RoomType = RoomType.Normal,
                Neighbors = new List<int>()
            };

            map[(0, 0)] = startRoom;
            roomList.Add(startRoom);

            var frontier = new Queue<Room>();
            frontier.Enqueue(startRoom);

            while (roomList.Count < roomCount && frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                var shuffled = directions.OrderBy(_ => _random.Next()).ToList();

                foreach (var (dx, dy) in shuffled)
                {
                    int nx = current.X + dx;
                    int ny = current.Y + dy;
                    var newCoord = (nx, ny);

                    if (map.ContainsKey(newCoord)) continue;

                    var newRoomId = roomList.Count;
                    var newRoom = new Room
                    {
                        Id = newRoomId,
                        X = nx,
                        Y = ny,
                        Visited = false,
                        RoomType = RoomType.Normal,
                        Neighbors = new List<int>()
                    };

                    current.Neighbors.Add(newRoom.Id);
                    newRoom.Neighbors.Add(current.Id);

                    map[newCoord] = newRoom;
                    roomList.Add(newRoom);
                    frontier.Enqueue(newRoom);

                    if (roomList.Count >= roomCount)
                        break;
                }
            }

            foreach (var room in roomList)
            {
                foreach (var (dx, dy) in directions)
                {
                    int nx = room.X + dx;
                    int ny = room.Y + dy;
                    var neighborCoord = (nx, ny);

                    if (map.TryGetValue(neighborCoord, out var neighborRoom))
                    {
                        if (!room.Neighbors.Contains(neighborRoom.Id))
                            room.Neighbors.Add(neighborRoom.Id);
                        if (!neighborRoom.Neighbors.Contains(room.Id))
                            neighborRoom.Neighbors.Add(room.Id);
                    }
                }
            }

            foreach (var room in roomList)
            {
                if (_random.NextDouble() < 0.3 && room.Id != 0)
                {
                    room.Monster = new Monster
                    {
                        Name = "Slime",
                        Level = 1,
                        Health = 10,
                        MaxHealth = 20,
                        Attack = 5
                    };
                    room.Reward = new Reward
                    {
                        Name = "Small Potion",
                        Description = "Restores a bit of HP and MP.",
                        Health = 10,
                        Mana = 5,
                        Experience = 10
                    };
                }
                else if (_random.NextDouble() < 0.2)
                {
                    room.Reward = new Reward
                    {
                        Name = "Small Potion",
                        Description = "Restores a bit of HP and MP.",
                        Health = 10,
                        Mana = 5,
                        Experience = 10
                    };
                }
            }

            // 마지막 방은 보스룸으로
            var lastRoom = roomList.Last();
            lastRoom.RoomType = RoomType.Boss;
            lastRoom.Monster = new Monster
            {
                Name = "Boss Goblin",
                Level = 5,
                Health = 10,
                MaxHealth = 100,
                Attack = 20
            };
            lastRoom.Reward = new Reward
                    {
                        Name = "Small Potion",
                        Description = "Restores a bit of HP and MP.",
                        Health = 10,
                        Mana = 5,
                        Experience = 10
                    };

            return new MapData
            {
                MapName = mapName,
                CurrentRoom = 0,
                Rooms = roomList
            };
        }

        public static string VisualizeMap(List<Room> rooms, int? currentRoomId = null)
        {
            var mapDict = rooms.ToDictionary(r => (r.X, r.Y), r => r);
            int minX = rooms.Min(r => r.X);
            int maxX = rooms.Max(r => r.X);
            int minY = rooms.Min(r => r.Y);
            int maxY = rooms.Max(r => r.Y);

            var sb = new StringBuilder();
            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (mapDict.TryGetValue((x, y), out var room))
                    {
                        if (currentRoomId.HasValue && room.Id == currentRoomId.Value)
                        {
                            sb.Append("📍");
                        }
                        else
                        {
                            string symbol = room.RoomType switch
                            {
                                RoomType.Normal => room.Visited
                                    ? (room.Monster != null ? "🐉" : "🟩")
                                    : "⬜",
                                RoomType.Boss => "🟥",
                                RoomType.Treasure => "💰",
                                RoomType.Door => "🚪",
                                _ => "❓"
                            };
                            sb.Append(symbol);
                        }
                    }
                    else
                    {
                        sb.Append("⬛");
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
