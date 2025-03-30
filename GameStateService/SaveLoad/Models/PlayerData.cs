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

        /// <summary>
        /// 플레이어가 소유한 아이템 목록
        /// </summary>
        public List<GameItem> Inventory { get; set; }

        /// <summary>
        /// 플레이어가 위치한 맵 파일명
        /// </summary>
        public Map CurrentMap { get; set; }

        /// <summary>
        /// PlayerData 클래스의 기본 생성자입니다.
        /// </summary>
        public PlayerData()
        {
            Inventory = new List<GameItem>();
        }

        /// <summary>
        /// 플레이어 정보를 간단한 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"PlayerData: {UserName} (ID: {PlayerId}) - Level: {Level}, Exp: {Experience}, Health: {Health}, Mana: {Mana}";
        }
    }
}
