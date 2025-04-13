using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using GameStateService.Models;
using GameStateService.Utils;

public static class MapLoader
{
    public static MapData LoadNewMapAsync(string mapName, int mapLevel)
    {
        int mapCount = mapLevel * 10;
        MapData newMap = MapGenerator.GenerateMap(mapName, mapCount);

        return newMap;
    }
}