using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class SaveManager
{
    public static void Save(Tile[,] map, int height, int width)
    {
        string[] lines = new string[height];

        for (int y = 0; y < height; y++)
        {
            StringBuilder line = new StringBuilder();

            for (int x = 0; x < width; x++) 
            {
                line.Append(map[x, y].Type + "," + map[x, y].Icon + "," + map[x, y].Orientation + " ");
            }

            lines[y] = line.Remove(line.Length - 1, 1).ToString();
        }

        System.IO.File.WriteAllLines(Application.dataPath + "/save.txt", lines);
    }

    public static Tile[,] Load()
    {
        string[] lines = System.IO.File.ReadAllLines(Application.dataPath + "/save.txt");
        int width = lines[0].Split(' ').Length;
        int height = lines.Length;

        Config.MapHeight = height;
        Config.MapWidth = width;

        Tile[,] map = new Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            string[] tiles = lines[y].Split(' ');

            for (int x = 0; x < width; x++)
            {
                map[x, y] = new Tile
                {
                    Type = (Tile.TileType) Enum.Parse(typeof(Tile.TileType), tiles[x].Split(',')[0]),
                    Icon = (Tile.TileType) Enum.Parse(typeof(Tile.TileType), tiles[x].Split(',')[1]),
                    Position = new Vector2(x, y),
                    Orientation = (Map.Orientation) Enum.Parse(typeof(Map.Orientation), tiles[x].Split(',')[2])
                };
            }
        }

        return map;
    }
}
