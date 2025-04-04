using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GameStateService.Models
{
    public class Map
    {
        public string MapName { get; set;}
    }
    public class MapData
    {
        [JsonPropertyName("map_name")]
        public string MapName { get; set; }

        [JsonPropertyName("current_room")]
        public int CurrentRoom { get; set; }

        [JsonPropertyName("rooms")]
        public List<Room> Rooms { get; set; }
    }

    public enum RoomType
    {
        Normal,
        Boss,
        Treasure,
        Door
    }

    public class Room
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("room_type")]
        public RoomType RoomType { get; set; }

        [JsonPropertyName("visited")]
        public bool Visited { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("neighbors")]
        public List<int> Neighbors { get; set; }

        [JsonPropertyName("monster")]
        public Monster Monster { get; set; }

        [JsonPropertyName("reward")]
        public Reward Reward { get; set; }

        /// <summary>
    }

    /// 몬스터 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Monster
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("level")]
        public int Level { get; set; }
        
        [JsonPropertyName("health")]
        public int Health { get; set; }
        
        [JsonPropertyName("attack")]
        public int Attack { get; set; }
    }

    /// <summary>
    /// 보상 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Reward
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("mana")]
        public int Mana { get; set; }

        [JsonPropertyName("experience")]
        public int Experience { get; set; }
    }
}
