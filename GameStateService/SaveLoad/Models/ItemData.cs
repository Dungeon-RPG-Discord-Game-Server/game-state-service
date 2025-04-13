namespace GameStateService.Models
{
    public enum Item
    {
        None,
        Sword,
        Shield,
        HealthPotion,
        ManaPotion,
        Armor,
        Ring,
        Bow,
        Arrow,
        Helmet,
        Boots
    }

    public class GameItem
    {
        public Item Type { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int RecoveryCoefficient { get; set; }
        public string? Description { get; set; }
    }
}
