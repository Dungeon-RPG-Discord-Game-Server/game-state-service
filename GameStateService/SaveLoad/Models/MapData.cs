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
        public string MapName { get; set; }
        public int CurrentRoom { get; set; }
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
        public int Id { get; set; }
        public RoomType RoomType { get; set; }
        public bool Visited { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> Neighbors { get; set; }
        public Monster Monster { get; set; }
        public Reward Reward { get; set; }

        /// <summary>
    }

    /// 몬스터 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Monster
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
    }

    /// <summary>
    /// 보상 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Reward
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Experience { get; set; }
    }
}
