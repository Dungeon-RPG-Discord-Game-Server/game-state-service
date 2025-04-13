using System.Text.Json.Serialization;

namespace GameStateService.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public int AttackPower { get; set; }

        public int ManaCost { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WeaponType Type { get; set; }

        public Skill Skill { get; set; }

        public void UpgradeWeapon(int upgradePower, int upgradeManaCost)
        {

            AttackPower += upgradePower;
            ManaCost += upgradeManaCost;
            Skill = WeaponFactory.CreateSkill(ManaCost, AttackPower * 2);
        }
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
                skillManaCost = manaCost * 2;
            }else{
                skillManaCost = 5;
            }

            return new Weapon
            {
                Name = name,
                AttackPower = attackPower,
                ManaCost = manaCost,
                Type = type,
                Skill = CreateSkill(skillManaCost, attackPower * 2)
            };
        }

        public static string UpgradeWeapon(Weapon weapon, int upgradePower, int upgradeManaCost)
        {
            if (weapon == null)
            {
                return "Weapon not found.";
            }

            weapon.AttackPower += upgradePower;
            weapon.ManaCost += upgradeManaCost;
            weapon.Skill = CreateSkill(weapon.ManaCost, weapon.AttackPower * 2);

            return $"{weapon.Name} has been upgraded! New Attack Power: {weapon.AttackPower} and Mana Cost: {weapon.ManaCost}.";
        }
    }
}