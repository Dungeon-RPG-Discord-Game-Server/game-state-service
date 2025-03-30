namespace GameStateService.Models
{
    /// <summary>
    /// 게임에서 사용 가능한 아이템 종류를 나타내는 열거형입니다.
    /// </summary>
    public enum Item
    {
        None,          // 기본값, 아이템이 없는 경우
        Sword,         // 검
        Shield,        // 방패
        HealthPotion,  // 체력 회복 포션
        ManaPotion,    // 마나 회복 포션
        Armor,         // 갑옷
        Ring,          // 반지
        Bow,           // 활
        Arrow,         // 화살
        Helmet,        // 투구
        Boots          // 장화
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
