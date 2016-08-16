using UnityEngine;
using System.Collections.Generic;
using TileType = Tile.TileType;
using System;

public class Map
{
    private Tile[,] map;
    private Land[,] lands;

    public Map()
    {
        map = new Tile[Config.MapWidth, Config.MapHeight];
        lands = new Land[Config.MapWidth, Config.MapHeight];

        RandomFillMap();
        SmoothMap();
        GenerateResource(Config.ForestRatio, TileType.FOREST);
        GenerateResource(Config.MountainRatio, TileType.MOUNTAIN);
        GenerateResource(Config.CoastRatio, TileType.COAST);
        GenerateLands();
    }

    public bool IsPositionValid(int x, int y)
    {
        return y >= 0 && x >= 0 && y < Config.MapHeight && x < Config.MapWidth;
    }

    public Tile GetTile(int x, int y)
    {
        if (!IsPositionValid(x, y)) return null;

        return map[x, y];
    }

    public Land GetLand(int x, int y)
    {
        if (!IsPositionValid(x, y)) return null;

        return lands[x, y];
    }
    
    public bool IsOrientationCorner(Orientation orientation)
    {
        return orientation == Orientation.BOTTOM_LEFT ||
               orientation == Orientation.BOTTOM_RIGHT ||
               orientation == Orientation.TOP_LEFT ||
               orientation == Orientation.TOP_RIGHT;
    }

    public void SaveMap()
    {
        SaveManager.Save(map, Config.MapHeight, Config.MapWidth);
    }

    private void RandomFillMap()
    {
        for (int x = 0; x < Config.MapWidth; x++)
        {
            for (int y = 0; y < Config.MapHeight; y++)
            {
                map[x, y] = new Tile()
                {
                    Position = new Vector2(x, y),
                    Type = Config.Seed.Next(0, 100) < Config.FillRatio ? TileType.PLAIN : TileType.WATER,
                    Orientation = Orientation.DEFAULT
                };
            }
        }
    }

    private void SmoothMap()
    {
        for (int i = 0; i < Config.SmoothCount; i++)
        {
            for (int x = 0; x < Config.MapWidth; x++)
            {
                for (int y = 0; y < Config.MapHeight; y++)
                {
                    int surroundingLandCount = GetSurroundingLandCount(x, y);

                    if (surroundingLandCount > 4) map[x, y].Type = TileType.PLAIN;
                    if (surroundingLandCount < 4) map[x, y].Type = TileType.WATER;
                }
            }
        }
    }

    private int GetSurroundingLandCount(int xPosition, int yPosition)
    {
        int landCount = 0;

        for (int x = xPosition - 1; x < xPosition + 2; x++)
        {
            for (int y = yPosition - 1; y < yPosition + 2; y++)
            {
                if (IsPositionValid(x, y) && !(x == xPosition && y == yPosition))
                {
                    if (map[x, y].Type != TileType.WATER) landCount++;
                }
            }
        }

        return landCount;
    }

    void GenerateResource(int ratio, TileType resource)
    {
        System.Random random = Config.Seed;

        for (int x = 0; x < Config.MapWidth; x++)
        {
            for (int y = 0; y < Config.MapHeight; y++)
            {
                if (map[x, y].Type == TileType.WATER) continue;

                int number = random.Next(0, 100);

                if (resource == TileType.COAST)
                {
                    Orientation coastOrientation = GetCoastOrientation(x, y);
                    if (coastOrientation == Orientation.DEFAULT || number >= ratio) continue;

                    TileType coastType = IsCoastEnd(x, y) ? TileType.COAST_END
                        : IsOrientationCorner(coastOrientation) ? TileType.COAST_CORNER
                            : TileType.COAST;

                    map[x, y].Icon = coastType;
                    map[x, y].Orientation = coastOrientation;
                }
                else
                {
                    map[x, y].Icon = number < ratio ? resource : map[x, y].Icon;
                }
            }
        }
    }

    private void GenerateLands()
    {
        for (int x = 0; x < Config.MapWidth; x++)
        {
            for (int y = 0; y < Config.MapHeight; y++)
            {
                lands[x, y] = new Land(this, map[x, y]);
            }
        }
    }

    private Orientation GetCoastOrientation(int x, int y)
    {
        bool waterOnTop = y + 1 < Config.MapHeight && map[x, y + 1].Type == TileType.WATER;
        bool waterOnBottom = y - 1 >= 0 && map[x, y - 1].Type == TileType.WATER;
        bool waterOnLeft = x - 1 >= 0 && map[x - 1, y].Type == TileType.WATER;
        bool waterOnRight = x + 1 < Config.MapWidth && map[x + 1, y].Type == TileType.WATER;

        if (waterOnBottom && waterOnTop)
        {
            return waterOnRight ? Orientation.RIGHT : (waterOnLeft ? Orientation.LEFT : Orientation.DEFAULT);
        }

        if (waterOnRight && waterOnLeft)
        {
            return waterOnTop ? Orientation.TOP : (waterOnBottom ? Orientation.BOTTOM : Orientation.DEFAULT);
        }

        if (waterOnTop)
        {
            return waterOnRight
                ? Orientation.TOP_RIGHT
                : (waterOnLeft ? Orientation.TOP_LEFT : Orientation.TOP);
        }

        if (waterOnBottom)
        {
            return waterOnRight
               ? Orientation.BOTTOM_RIGHT
               : (waterOnLeft ? Orientation.BOTTOM_LEFT : Orientation.BOTTOM);
        }

        return waterOnLeft ? Orientation.LEFT : (waterOnRight ? Orientation.RIGHT : Orientation.DEFAULT);
    }

    private bool IsCoastEnd(int x, int y)
    {
        int trueConditionCount = 0;
        trueConditionCount += (y + 1 < Config.MapHeight && map[x, y + 1].Type == TileType.WATER) ? 1 : 0;
        trueConditionCount += (y - 1 >= 0 && map[x, y - 1].Type == TileType.WATER) ? 1 : 0;
        trueConditionCount += (x - 1 >= 0 && map[x - 1, y].Type == TileType.WATER) ? 1 : 0;
        trueConditionCount += (x + 1 < Config.MapWidth && map[x + 1, y].Type == TileType.WATER) ? 1 : 0;

        return trueConditionCount > 2;
    }

    public enum Orientation
    {
        DEFAULT,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
}
