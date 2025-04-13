using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace GameStateService.Models
{
    public class PlayerData
    {
        public string id { get; set; }
        public string PlayerId { get; set; }
        public string UserName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public Weapon? Weapon {get; set;}
        public MapData? CurrentMapData { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameStateType CurrentGameState { get; set; }

        /// <summary>
        /// 플레이어 정보를 간단한 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"PlayerData: {UserName} (ID: {PlayerId}) - Level: {Level}, Exp: {Experience}, Health: {Health}, Mana: {Mana}, CurrentMap: {CurrentMapData?.MapName}, GameState: {CurrentGameState}";
        }

        public void ApplyReward(Reward reward)
        {
            if (reward == null) return;

            RecoverHealth(reward.Health);
            RecoverMana(reward.Mana);
            GainExperience(reward.Experience);
        }

        public void RecoverHealth(int amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
        }

        public void RecoverMana(int amount)
        {
            Mana = Math.Min(Mana + amount, MaxMana);
        }

        public void GainExperience(int amount)
        {
            Experience += amount;
            while (Experience >= GetRequiredExperience())
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Experience -= GetRequiredExperience();
            Level++;

            MaxHealth += 10;
            Health = MaxHealth;

            MaxMana += 5;
            Mana = MaxMana;

            Weapon.UpgradeWeapon(2, 0);
        }

        private int GetRequiredExperience()
        {
            return 50 + ((Level - 1) * 25);
        }
    }

    public static class PlayerFactory
    {
        public static PlayerData CreatePlayerWithWeapon(string playerId, string userName, Weapon weapon)
        {
            return new PlayerData
            {
                id = playerId,
                PlayerId = playerId,
                UserName = userName,
                Level = 1,
                Experience = 0,
                Health = 100,
                MaxHealth = 100,
                Mana = 50,
                MaxMana = 50,
                Weapon = weapon,
                CurrentMapData = null,
                CurrentGameState = GameStateType.MainMenuState
            };
        }
        public static PlayerData CreateNewPlayer(string playerId, string userName, WeaponType weaponType)
        {
            PlayerData playerData;

            switch (weaponType)
            {
                case WeaponType.Sword:
                    playerData = CreatePlayerWithWeapon(playerId, userName, WeaponFactory.CreateWeapon("Sword", 20, 0, weaponType));
                    playerData.Health = 120; // Sword users have higher health
                    playerData.Mana = 50; // Sword users have lower mana
                    break;
                case WeaponType.MagicWand:
                    playerData = CreatePlayerWithWeapon(playerId, userName, WeaponFactory.CreateWeapon("Magic Wand", 30, 5, weaponType));
                    playerData.Health = 80; // Magic wand users have lower health
                    playerData.Mana = 120; // Magic wand users have higher mana
                    break;
                default:
                    throw new ArgumentException("Invalid weapon type");
            }

            playerData.MaxHealth = playerData.Health;
            playerData.MaxMana = playerData.Mana;
            
            return playerData;
        }
        public static string GetPlayerString(PlayerData data){
            return $"PlayerData: {data.UserName} (ID: {data.PlayerId}) - Level: {data.Level}, Exp: {data.Experience}, Health: {data.Health}, Mana: {data.Mana}";
        }
    }
}
