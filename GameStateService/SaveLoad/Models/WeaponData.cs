namespace GameStateService.Models
{
    /// <summary>
    /// 게임 내 무기 정보를 나타내는 클래스입니다.
    /// </summary>
    public class Weapon
    {
        /// <summary>
        /// 무기의 고유 이름 또는 ID
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 공격력
        /// </summary>
        public int AttackPower { get; set; }

        public int ManaCost { get; set; }

        /// <summary>
        /// 해당 무기의 무기 타입 (예: Sword, Bow, Staff 등)
        /// </summary>
        public WeaponType Type { get; set; }

        public Skill? Skill { get; set; }
    }

    public class Skill
    {
        public string Name { get; set; }
        public int ManaCost { get; set; }
        public int Damage { get; set; }
    }
    /// 
    public enum WeaponType
    {
        Sword,
        MagicWand,
    }
}