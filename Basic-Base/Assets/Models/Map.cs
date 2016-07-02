using UnityEngine;
using System.Collections.Generic;
using System;

public class Map
{
    private Tile[,] map;
    private Land[,] lands;
    private Config config;

    public Map(Config configuration)
    {
        config = configuration;
        map = new Tile[config.Width, config.Height];
        lands = new Land[config.Width, config.Height];

        RandomFillMap();
        SmoothMap();
        GenerateResource(config.ForestRatio, TileType.FOREST);
        GenerateResource(config.MountainRatio, TileType.MOUNTAIN);
        GenerateResource(config.CoastRatio, TileType.COAST);
        GenerateLands();
    }

    public bool IsPositionValid(int x, int y)
    {
        return y >= 0 && x >= 0 && y < config.Height && x < config.Width;
    }

    public Tile GetTile(int x, int y)
    {
        if (!IsPositionValid(x, y)) return null;

        return map[x, y];
    }

    public Config GetConfiguration()
    {
        return config;
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

    private void RandomFillMap()
    {
        for (int x = 0; x < config.Width; x++)
        {
            for (int y = 0; y < config.Height; y++)
            {
                map[x, y] = new Tile()
                {
                    Position = new Vector2(x, y),
                    Icons = new List<string>(),
                    TileType = config.Seed.Next(0, 100) < config.FillRatio ? TileType.PLAIN : TileType.WATER,
                    Orientation = Orientation.DEFAULT
                };
            }
        }
    }

    private void SmoothMap()
    {
        for (int i = 0; i < config.SmoothCount; i++)
        {
            for (int x = 0; x < config.Width; x++)
            {
                for (int y = 0; y < config.Height; y++)
                {
                    int surroundingLandCount = GetSurroundingLandCount(x, y);

                    if (surroundingLandCount > 4) map[x, y].TileType = TileType.PLAIN;
                    if (surroundingLandCount < 4) map[x, y].TileType = TileType.WATER;
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
                    if (map[x, y].TileType != TileType.WATER) landCount++;
                }
            }
        }

        return landCount;
    }

    void GenerateResource(int ratio, TileType resource)
    {
        System.Random random = config.Seed;

        for (int x = 0; x < config.Width; x++)
        {
            for (int y = 0; y < config.Height; y++)
            {
                if (map[x, y].TileType != TileType.WATER)
                {
                    int number = random.Next(0, 100);

                    if (resource == TileType.COAST)
                    {
                        Orientation coastOrientation = GetCoastOrientation(x, y);
                        if (coastOrientation != Orientation.DEFAULT && number < ratio)
                        {
                            TileType coastType = IsCoastEnd(x, y) ? TileType.COAST_END
                                : IsOrientationCorner(coastOrientation) ? TileType.COAST_CORNER
                                : TileType.COAST;

                            map[x, y].TileType = coastType;
                            map[x, y].Orientation = coastOrientation;
                            map[x, y].Icons.Add(config.TileIconPath + coastType.ToString().ToLower());
                        }
                    }
                    else
                    {
                        map[x, y].TileType = number < ratio ? resource : map[x, y].TileType;
                        map[x, y].Icons.Add(config.TileIconPath + map[x, y].TileType.ToString().ToLower());
                    }
                }
            }
        }
    }

    private void GenerateLands()
    {
        for (int x = 0; x < config.Width; x++)
        {
            for (int y = 0; y < config.Height; y++)
            {
                lands[x, y] = new Land(this, map[x, y]);
            }
        }
    }

    private Orientation GetCoastOrientation(int x, int y)
    {
        bool waterOnTop = y + 1 < config.Height && map[x, y + 1].TileType == TileType.WATER;
        bool waterOnBottom = y - 1 >= 0 && map[x, y - 1].TileType == TileType.WATER;
        bool waterOnLeft = x - 1 >= 0 && map[x - 1, y].TileType == TileType.WATER;
        bool waterOnRight = x + 1 < config.Width && map[x + 1, y].TileType == TileType.WATER;

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
        trueConditionCount += (y + 1 < config.Height && map[x, y + 1].TileType == TileType.WATER) ? 1 : 0;
        trueConditionCount += (y - 1 >= 0 && map[x, y - 1].TileType == TileType.WATER) ? 1 : 0;
        trueConditionCount += (x - 1 >= 0 && map[x - 1, y].TileType == TileType.WATER) ? 1 : 0;
        trueConditionCount += (x + 1 < config.Width && map[x + 1, y].TileType == TileType.WATER) ? 1 : 0;

        return trueConditionCount > 2;
    }

    public enum TileType
    {
        DEFAULT,
        WATER,
        PLAIN,
        FOREST,
        MOUNTAIN,
        COAST,
        COAST_CORNER,
        COAST_END
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
