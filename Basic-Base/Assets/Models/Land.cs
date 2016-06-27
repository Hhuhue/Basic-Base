using System;
using UnityEngine;
using TileType = Map.TileType;
using Orientation = Map.Orientation;

public class Land
{
    private Tile[,] land;
    private Tile tile;
    private Map map;

    public Land(Map map, Tile tile)
    {
        this.map = map;
        this.tile = tile;

        land = new Tile[10, 10];

        GenerateLand();
    }

    public Tile GetLandPiece(int x, int y)
    {
        if (x < 0 || x > 9 || y < 0 || y > 9) return null;

        return land[x, y];
    }

    private void GenerateLand()
    {
        switch (tile.TileType)
        {
            case TileType.PLAIN:
                GeneratePlain();
                break;

            case TileType.FOREST:
                GenerateForest();
                break;

            case TileType.MOUNTAIN:
                GenerateMountain();
                break;

            case TileType.WATER:
                GenerateWater();
                break;

            case TileType.COAST:
            case TileType.COAST_CORNER:
            case TileType.COAST_END:
                GenerateCoast();
                break;
        }

    }

    private void GenerateForest()
    {
        System.Random random = map.GetConfiguration().Seed;

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = random.Next(0, 100);
                LandType type = number < 2 ? LandType.ROCK
                    : number < 50 ? LandType.PINE
                    : number < 90 ? LandType.TREE
                    : LandType.GRASS;

                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    private void GeneratePlain()
    {
        System.Random random = map.GetConfiguration().Seed;

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = random.Next(0, 100);
                LandType type = number < 2 ? LandType.ROCK
                    : number < 4 ? LandType.PINE
                    : LandType.GRASS;

                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    private void GenerateWater()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = LandType.WATER
                };
            }
        }
    }

    private void GenerateCoast()
    {
        System.Random random = map.GetConfiguration().Seed;

        int waterThickness = random.Next(0, 2);
        int sandThickness = random.Next(1, 3);

        Border water = GetCoastBorder(waterThickness);
        Border sand = GetCoastBorder(sandThickness + waterThickness);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                LandType type = water.IsPositionWithinBorder(x, y) ? LandType.WATER
                    : sand.IsPositionWithinBorder(x, y) ? LandType.SAND
                    : LandType.GRASS;

                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }        
    }

    private string ToString()
    {
        string s = "";
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                s += "[" + x + ", " + y + "] = " + land[x, y].LandType + " | ";
            }
        }
        return s;
    }

    private void GenerateMountain()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = LandType.ROCK
                };
            }
        }
    }

    private void SmoothLand()
    {

    }

    private Border GetCoastBorder(int thickness)
    {
        switch (tile.Orientation)
        {
            case Orientation.TOP:
                return new Border(9, 9 - thickness, 0, 9);

            case Orientation.BOTTOM:
                return new Border(thickness, 0, 0, 9);

            case Orientation.LEFT:
                return new Border(9, 0, 0, thickness);

            case Orientation.RIGHT:
                return new Border(9, 0, 9 - thickness, 9);

            default:
                return new Border();
        }
    }

    public struct Border
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Border(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public bool IsPositionWithinBorder(int x, int y)
        {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }

        public static bool operator ==(Border border1, Border border2)
        {
            return border1.Top == border2.Top &&
                   border1.Bottom == border2.Bottom &&
                   border1.Left == border2.Left &&
                   border1.Right == border2.Right;
        }

        public static bool operator !=(Border border1, Border border2)
        {
            return !(border1 == border2);
        }

        public string ToString()
        {
            return "Top: " + Top + " Bottom: " + Bottom + " Left: " + Left + " Right: " + Right;
        }
    }

    public enum LandType
    {
        TREE,
        PINE,
        ROCK,
        SAND,
        GRASS,
        WATER,
        CLIFF,
        MOUNTAIN
    }
}

