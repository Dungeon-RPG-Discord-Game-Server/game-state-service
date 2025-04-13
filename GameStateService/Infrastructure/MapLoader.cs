using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using GameStateService.Models;
using GameStateService.Utils;

public static class MapLoader
{
    private static int GetRoomCountByMapLevel(int level)
    {
        return level switch
        {
            1 => RandomProvider.Next(6, 9),
            2 or 3 => RandomProvider.Next(8, 13),
            4 or 5 => RandomProvider.Next(12, 17),
            6 or 7 => RandomProvider.Next(16, 21),
            8 or 9 => RandomProvider.Next(20, 25),
            _ => RandomProvider.Next(25, 31)
        };
    }
    public static MapData LoadNewMapAsync(int mapLevel, int playerLevel)
    {
        MapData newMap = MapGenerator.GenerateMap(GetRoomCountByMapLevel(mapLevel), mapLevel, playerLevel);

        return newMap;
    }
}