using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using GameStateService.Models;
using GameStateService.Utils;

public class MapLoader
{
    /// <summary>
        /// 지정된 경로에서 맵 데이터를 로드합니다.
        /// </summary>
        /// <param name="filePath">JSON 파일 경로</param>
        /// <returns>비동기적으로 MapData 객체를 반환</returns>
        public async Task<MapData> LoadNewMapAsync(string mapName, int mapLevel)
        {
            int mapCount = mapLevel * 2;
            MapData newMap = MapGenerator.GenerateMap(mapName, mapCount);
            string visualizedMap = MapGenerator.VisualizeMap(newMap.Rooms);
            Console.WriteLine($"Generated Map:\n {visualizedMap}");

            return newMap;
        }
}