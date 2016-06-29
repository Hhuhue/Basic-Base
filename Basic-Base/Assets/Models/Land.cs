using System;
using System.Linq;
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

                SetLandPiece(x, y, type);
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

                SetLandPiece(x, y, type);
            }
        }
    }

    private void GenerateWater()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                SetLandPiece(x, y, LandType.WATER);
            }
        }
    }

    private void GenerateCoast()
    {
        System.Random random = map.GetConfiguration().Seed;

        int waterThickness = random.Next(0, 2);
        int sandThickness = random.Next(1, 3);
        bool isCoastEnd = tile.TileType == TileType.COAST_END;

        Border[] waterZones = GetCoastBorder(waterThickness, tile.Orientation, isCoastEnd);
        Border[] sandZones = GetCoastBorder(sandThickness + waterThickness, tile.Orientation, isCoastEnd);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                LandType type = waterZones.Any(border => border.IsPositionWithinBorder(x, y)) ? LandType.WATER
                    : sandZones.Any(border => border.IsPositionWithinBorder(x, y)) ? LandType.SAND
                    : LandType.GRASS;

                SetLandPiece(x, y, type);
            }
        }        
    }

    private void GenerateMountain()
    {
        System.Random random = map.GetConfiguration().Seed;

        Border mountainZone = new Border(9 - random.Next(0, 2), random.Next(0, 2), random.Next(0, 2), 9- random.Next(0, 2)); 

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                LandType type = mountainZone.IsPositionWithinBorder(x, y) ? LandType.MOUNTAIN 
                    : random.Next(0, 100) < 40 ? LandType.ROCK
                    : LandType.GRASS;
                
                SetLandPiece(x, y, type);
            }
        }
    }

    private void SetLandPiece(int x, int y, LandType type)
    {
        land[x, y] = new Tile()
        {
            Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
            LandType = type
        };
    }

    private void SmoothLand()
    {

    }

    private Border[] GetCoastBorder(int thickness, Orientation orientation, bool isCoastEnd)
    {
        switch (orientation)
        {
            case Orientation.TOP:
                return (!isCoastEnd) ? new [] { new Border(9, 9 - thickness, 0, 9)}
                    : new []
                        {
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                        };

            case Orientation.BOTTOM:
                return (!isCoastEnd) ? new [] { new Border(thickness, 0, 0, 9)}
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                        };

            case Orientation.LEFT:
                return (!isCoastEnd) ? new [] { new Border(9, 0, 0, thickness)}
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0],
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0]
                        };

            case Orientation.RIGHT:
                return (!isCoastEnd) ? new[] { new Border(9, 0, 9 - thickness, 9)}
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0]
                        };

            case Orientation.TOP_LEFT:
                return new []
                {
                    GetCoastBorder(thickness, Orientation.TOP, false)[0],
                    GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                };

            case Orientation.TOP_RIGHT:
                return new []
                {
                    GetCoastBorder(thickness, Orientation.TOP, false)[0],
                    GetCoastBorder(thickness, Orientation.RIGHT, false)[0]
                };

            case Orientation.BOTTOM_LEFT:
                return new [] 
                {
                    GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                    GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                };

            case Orientation.BOTTOM_RIGHT:
                return new []
                {
                    GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                    GetCoastBorder(thickness, Orientation.RIGHT, false)[0]
                };

            default:
                return new [] { new Border()};
        }
    }

    public struct Border
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Border(int top, int bottom, int left, int right) : this()
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

