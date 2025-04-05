using System;
using System.Collections.Generic;

namespace GameStateService.Models
{
    /// <summary>
    /// 플레이어의 기본 데이터를 나타내는 모델 클래스입니다.
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// 플레이어의 고유 식별자 (예: Discord ID, 사용자 ID 등)
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// 플레이어의 이름
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 플레이어의 현재 레벨
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 플레이어의 경험치
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// 플레이어의 현재 체력
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// 플레이어의 현재 마나
        /// </summary>
        public int Mana { get; set; }

        public Weapon? Weapon {get; set;}

        /// <summary>
        /// 플레이어가 위치한 맵 파일명
        /// </summary>
        public MapData? CurrentMapData { get; set; }
        public GameStateType CurrentGameState { get; set; }

        /// <summary>
        /// 플레이어 정보를 간단한 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            string weaponInfo = Weapon != null
                ? $"🗡️ Weapon: {Weapon.Name} (ATK: {Weapon.AttackPower}, Mana Cost: {Weapon.ManaCost})"
                : "❌ No weapon equipped";

            string mapInfo = CurrentMapData != null
                ? $"🗺️ Current Map: {CurrentMapData.MapName}"
                : "🌐 No map assigned";

            return
                $@"👤 **{UserName}**
                🏅 Level: {Level}    ✨ EXP: {Experience}
                ❤️ Health: {Health}    🔷 Mana: {Mana}
                {weaponInfo}
                {mapInfo}
                🎮 State: {CurrentGameState}";
        }
    }

    public static class PlayerFactory
    {
        public static PlayerData CreatePlayerWithWeapon(string playerId, string userName, Weapon weapon)
        {
            return new PlayerData
            {
                PlayerId = playerId,
                UserName = userName,
                Level = 1,
                Experience = 0,
                Health = 100,
                Mana = 50,
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
                    playerData = CreatePlayerWithWeapon(playerId, userName, WeaponFactory.CreateWeapon("Basic Sword", 10, 0, weaponType));
                    playerData.Health = 200; // Sword users have higher health
                    playerData.Mana = 30; // Sword users have lower mana
                    break;
                case WeaponType.MagicWand:
                    playerData = CreatePlayerWithWeapon(playerId, userName, WeaponFactory.CreateWeapon("Basic Magic Wand", 20, 5, weaponType));
                    playerData.Health = 100; // Magic wand users have lower health
                    playerData.Mana = 100; // Magic wand users have higher mana
                    break;
                default:
                    throw new ArgumentException("Invalid weapon type");
            }
            
            return playerData;
        }
        public static string GetPlayerString(PlayerData data){
            return $"PlayerData: {data.UserName} (ID: {data.PlayerId}) - Level: {data.Level}, Exp: {data.Experience}, Health: {data.Health}, Mana: {data.Mana}";
        }
    }
}
