using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameStateService.Models;

namespace GameStateService.Utils
{
    public static class MonsterGenerator
    {
        private static readonly string[] BossTypes = new[]
        {
            "Boss", "Abyssal", "Legendary", "Champion", "Elite", "Boss", "Ancient", "Mythic", "Titan", "Overlord"
        };
        private static readonly string[] Adjectives = new[]
        {
            "Feral", "Dark", "Vicious", "Rotten", "Shadow", "Burning", "Wicked", "Savage", "Corrupted", "Venomous"
        };

        private static readonly string[] Nouns = new[]
        {
            "Wolf", "Goblin", "Wraith", "Slime", "Spider", "Golem", "Bat", "Reaper", "Troll", "Serpent"
        };

        public static Monster Generate(int playerLevel, int mapLevel, bool isBoss = false)
        {
            string name = isBoss
                ? $"{BossTypes[RandomProvider.Next(BossTypes.Length)]} {Adjectives[RandomProvider.Next(Adjectives.Length)]} {Nouns[RandomProvider.Next(Nouns.Length)]}"
                : $"{Adjectives[RandomProvider.Next(Adjectives.Length)]} {Nouns[RandomProvider.Next(Nouns.Length)]}";

            int level = isBoss
                ? Math.Max(playerLevel, mapLevel) + 2
                : Math.Max(playerLevel, mapLevel);

            int health = isBoss
                ? RandomProvider.Next(100, 150) + level * 10
                : RandomProvider.Next(20, 40) + level * 5;

            int attack = isBoss
                ? RandomProvider.Next(15, 25) + level * 2
                : RandomProvider.Next(5, 10) + level;

            return new Monster
            {
                Name = name,
                Level = level,
                MaxHealth = health,
                Health = health,
                Attack = attack
            };
        }
    }

    public static class RewardGenerator
    {
        private static readonly string[] RewardNames = new[]
        {
            "Elixir of Power", "Mystic Tome", "Ancient Relic", "Crystal Heart", "Phoenix Feather"
        };

        private static readonly string[] RewardDescriptions = new[]
        {
            "Greatly boosts your stats temporarily.",
            "A tome filled with forgotten knowledge.",
            "Glows with ancient magical energy.",
            "Revives the spirit within.",
            "Burns with eternal life energy."
        };

        public static Reward Generate(Monster monster, bool isBoss = false)
        {
            int healthRestore = isBoss ? RandomProvider.Next(25, 50) : RandomProvider.Next(5, 11);
            int manaRestore = isBoss ? RandomProvider.Next(20, 40) : RandomProvider.Next(3, 8);
            int expGained = isBoss
                ? monster.Level * RandomProvider.Next(30, 50)
                : monster.Level * RandomProvider.Next(8, 15);

            string name = isBoss
                ? RewardNames[RandomProvider.Next(RewardNames.Length)]
                : "Small Potion";

            string desc = isBoss
                ? RewardDescriptions[RandomProvider.Next(RewardDescriptions.Length)]
                : "Restores a bit of HP and MP.";

            return new Reward
            {
                Name = name,
                Description = desc,
                Health = healthRestore,
                Mana = manaRestore,
                Experience = expGained
            };
        }
    }

    public static class MapGenerator
    {
        private static readonly List<(int dx, int dy)> directions = new()
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0)
        };

        private static readonly string[] Adjectives = new[]
        {
            "Forgotten", "Ancient", "Cursed", "Haunted", "Shadowy", "Burning", "Frozen", "Echoing", "Twisted", "Abyssal"
        };

        private static readonly string[] Nouns = new[]
        {
            "Catacombs", "Sanctum", "Keep", "Crypt", "Ruins", "Depths", "Abyss", "Vault", "Labyrinth", "Stronghold"
        };

        public static string GenerateMapName(int mapLevel)
        {
            var adjective = Adjectives[RandomProvider.Next(0, Adjectives.Length)];
            var noun = Nouns[RandomProvider.Next(0, Nouns.Length)];
            return $"{adjective} {noun} (level {mapLevel})";
        }

        public static MapData GenerateMap(int roomCount, int mapLevel = 1, int playerLevel = 1)
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
                var shuffled = directions.OrderBy(_ => RandomProvider.Next()).ToList();

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
                if (RandomProvider.NextDouble() < 0.3 && room.Id != 0)
                {
                    room.Monster = MonsterGenerator.Generate(playerLevel, mapLevel);
                    room.Reward = RewardGenerator.Generate(room.Monster);
                }
                else if (RandomProvider.NextDouble() < 0.2)
                {
                    // room.Reward = new Reward
                    // {
                    //     Name = "Small Potion",
                    //     Description = "Restores a bit of HP and MP.",
                    //     Health = 10,
                    //     Mana = 5,
                    //     Experience = 10
                    // };
                    continue;
                }
            }

            // 마지막 방은 보스룸으로
            var lastRoom = roomList.Last();
            lastRoom.RoomType = RoomType.Boss;
            lastRoom.Monster = MonsterGenerator.Generate(playerLevel, mapLevel, true);
            lastRoom.Reward = RewardGenerator.Generate(lastRoom.Monster, true);

            return new MapData
            {
                MapName = GenerateMapName(mapLevel),
                CurrentRoom = 0,
                Rooms = roomList,
                MapLevel = mapLevel
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
