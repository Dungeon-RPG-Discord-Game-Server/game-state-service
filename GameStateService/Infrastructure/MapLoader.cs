using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GameStateService.Models;

public class MapLoader
{
    /// <summary>
        /// 지정된 경로에서 맵 데이터를 로드합니다.
        /// </summary>
        /// <param name="filePath">JSON 파일 경로</param>
        /// <returns>비동기적으로 MapData 객체를 반환</returns>
        public async Task<MapData> LoadMapAsync(string filePath)
        {
            try
            {
                // JSON 파일의 전체 내용을 비동기적으로 읽어옵니다.
                string jsonString = await File.ReadAllTextAsync(filePath);

                // JSON 파일을 MapData 객체로 파싱합니다.
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                MapData mapData = JsonSerializer.Deserialize<MapData>(jsonString, options);

                return mapData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("맵 데이터를 로드하는 중 오류 발생: " + ex.Message);
                throw;
            }
        }
}