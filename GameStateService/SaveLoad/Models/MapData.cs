using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GameStateService.Models
{
    public class Map
    {
        public string MapName { get; set;}
        public string RoomId { get; set; }
    }
    public class MapData
    {
        [JsonPropertyName("rooms")]
        public List<Room> Rooms { get; set; }
    }

    public class Room
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("neighbors")]
        public List<string> Neighbors { get; set; }

        [JsonPropertyName("monsters")]
        public List<Monster> Monsters { get; set; }

        [JsonPropertyName("rewards")]
        public List<Reward> Rewards { get; set; }

        /// <summary>
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
        
        [JsonPropertyName("defense")]
        public int Defense { get; set; }
    }

    /// <summary>
    /// 보상 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Reward
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }
    }
}
