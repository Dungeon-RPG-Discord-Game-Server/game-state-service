using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WeaponType Type { get; set; }

        public Skill Skill { get; set; }
    }

    public class Skill
    {
        public int ManaCost { get; set; }
        public int Damage { get; set; }
    }
    /// 
    public enum WeaponType
    {
        Sword,
        MagicWand,
    }

    public static class WeaponFactory
    {
        public static Skill CreateSkill(int manaCost, int damage)
        {
            return new Skill
            {
                ManaCost = manaCost,
                Damage = damage
            };
        }
        public static Weapon CreateWeapon(string name, int attackPower, int manaCost, WeaponType type)
        {
            int skillManaCost;
            if (manaCost > 0)
            {
                skillManaCost = manaCost * 2; // 예시: 스킬의 마나 비용은 무기 마나 비용의 절반
            }else{
                skillManaCost = 5;
            }

            return new Weapon
            {
                Name = name,
                AttackPower = attackPower,
                ManaCost = manaCost,
                Type = type,
                Skill = CreateSkill(skillManaCost, attackPower * 2) // 예시: 스킬 데미지는 공격력의 두 배
            };
        }

        public static string UpgradeWeapon(Weapon weapon, int upgradePower, int upgradeManaCost)
        {
            if (weapon == null)
            {
                return "Weapon not found.";
            }

            // 업그레이드 성공
            weapon.AttackPower += upgradePower;
            weapon.ManaCost += upgradeManaCost;
            weapon.Skill = CreateSkill(weapon.ManaCost * 2, weapon.AttackPower * 2); // 스킬도 업그레이드

            return $"{weapon.Name} has been upgraded! New Attack Power: {weapon.AttackPower} and Mana Cost: {weapon.ManaCost}.";
        }
    }
}