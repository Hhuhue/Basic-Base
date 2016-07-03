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

    private const int LAND_HEIGHT = 10;
    private const int LAND_WIDTH = 10;

    public Land(Map map, Tile tile)
    {
        this.map = map;
        this.tile = tile;

        land = new Tile[LAND_WIDTH, LAND_HEIGHT];

        GenerateLand();
        SmoothLand();
    }

    public Tile GetLandPiece(int x, int y)
    {
        if (x < 0 || x >= LAND_WIDTH || y < 0 || y >= LAND_HEIGHT) return null;

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

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
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

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
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
        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
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

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
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

        Border[] mountainZones = GenerateMountainLayers(); 

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                LandType type = mountainZones.Any(border => border.IsPositionOnBorder(x, y)) ? LandType.MOUNTAIN_FACE 
                    : mountainZones.Any(border => border.IsPositionWithinBorder(x, y)) ? LandType.MOUNTAIN_TOP
                    : random.Next(0, 100) < 40 ? LandType.ROCK
                    : LandType.GRASS;

                SetLandPiece(x, y, type);
            }
        }

        foreach(Border layer in mountainZones)
        {
            for (int x = layer.Left; x <= layer.Right; x++)
            {
                for (int y = layer.Bottom; y <= layer.Top; y++)
                {
                    Orientation orientation = GetMountainFaceOrientation(x, y, layer);
                    LandType type = map.IsOrientationCorner(orientation) ? LandType.MOUNTAIN_CORNER : land[x, y].LandType;

                    land[x, y].LandType = type;
                    land[x, y].Orientation = type == LandType.MOUNTAIN_TOP ? Orientation.DEFAULT : orientation;
                }
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
        if (!(tile.TileType == TileType.PLAIN || tile.TileType == TileType.FOREST)) return;

        int xPosition = (int)tile.Position.x;
        int yPosition = (int)tile.Position.y;

        Tile topTile = map.GetTile(xPosition, yPosition + 1) ?? new Tile();
        Tile bottomTile = map.GetTile(xPosition, yPosition - 1) ?? new Tile();
        Tile rightTile = map.GetTile(xPosition + 1, yPosition) ?? new Tile();
        Tile leftTile = map.GetTile(xPosition - 1, yPosition) ?? new Tile();

        System.Random random = map.GetConfiguration().Seed;

        LandType newType = LandType.DEFAULT;
        Orientation newOrientation = Orientation.DEFAULT;

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            switch (topTile.TileType)
            {
                case TileType.WATER:
                    newType = (x == 0 && leftTile.TileType == TileType.WATER) || (x == LAND_WIDTH - 1 && rightTile.TileType == TileType.WATER) ?
                        LandType.CLIFF_CORNER : LandType.CLIFF;
                    newOrientation = (x == 0) ? Orientation.TOP_LEFT : (x == LAND_WIDTH - 1) ? Orientation.TOP_RIGHT : Orientation.TOP;
                    break;

                case TileType.FOREST:
                    newType = (random.Next(0, 100) < 70) ? LandType.TREE : land[x, LAND_HEIGHT - 1].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;

                case TileType.MOUNTAIN:
                    newType = (random.Next(0, 100) < 70) ? LandType.ROCK : land[x, LAND_HEIGHT - 1].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;

                default:
                    newType = land[x, LAND_HEIGHT - 1].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;
            }

            land[x, LAND_HEIGHT - 1].LandType = newType;
            land[x, LAND_HEIGHT - 1].Orientation = newOrientation;

            switch (bottomTile.TileType)
            {
                case TileType.WATER:
                    newType = (x == 0 && leftTile.TileType == TileType.WATER) || (x == LAND_WIDTH - 1 && rightTile.TileType == TileType.WATER) ?
                        LandType.CLIFF_CORNER : LandType.CLIFF;
                    newOrientation = (x == 0) ? Orientation.BOTTOM_LEFT : (x == LAND_WIDTH - 1) ? Orientation.BOTTOM_RIGHT : Orientation.BOTTOM;
                    break;

                case TileType.FOREST:
                    newType = (random.Next(0, 100) < 35) ? LandType.TREE 
                        : (random.Next(0, 100) < 70) ? LandType.PINE 
                        : land[x, 0].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;

                case TileType.MOUNTAIN:
                    newType = (random.Next(0, 100) < 70) ? LandType.ROCK : land[x, 0].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;

                default:
                    newType = land[x, LAND_HEIGHT - 1].LandType;
                    newOrientation = Orientation.DEFAULT;
                    break;
            }

            land[x, 0].LandType = newType;
            land[x, 0].Orientation = newOrientation;
        }
    }

    private Border[] GenerateMountainLayers()
    {
        System.Random random = map.GetConfiguration().Seed;
        int layerCount = random.Next(1, 4);
        Border[] layers = new Border[layerCount];
        Border lastBorder = new Border(LAND_HEIGHT - 1, -1, -1, LAND_WIDTH - 1);

        for(int i = 0; i < layerCount; i++)
        {
            Border mountainZone = new Border()
            {
                Top = lastBorder.Top - random.Next(1, 3),
                Bottom = lastBorder.Bottom + random.Next(1, 3),
                Left = lastBorder.Left + random.Next(1, 3),
                Right = lastBorder.Right - random.Next(1, 3)
            };
            layers[i] = mountainZone;
            lastBorder = mountainZone;
        }

        return layers;
    }

    private Orientation GetMountainFaceOrientation(int x ,int y, Border border)
    {
        if (land[x, y].LandType == LandType.MOUNTAIN_TOP) return Orientation.DEFAULT;

        bool borderOnTop = y + 1 <= border.Top && border.IsPositionOnBorder(x, y + 1);
        bool borderOnBottom = y - 1 >= border.Bottom && border.IsPositionOnBorder(x, y - 1);
        bool borderOnLeft = x - 1 >= border.Left && border.IsPositionOnBorder(x - 1, y);
        bool borderOnRight = x + 1 <= border.Right && border.IsPositionOnBorder(x + 1, y);

        if (borderOnTop && borderOnBottom)
            return (x - 1 < border.Left) ? Orientation.LEFT : Orientation.RIGHT;

        if (borderOnLeft && borderOnRight)
            return (y - 1 < border.Bottom) ? Orientation.BOTTOM : Orientation.TOP;

        if (borderOnBottom && borderOnRight) return Orientation.TOP_LEFT;

        if (borderOnBottom && borderOnLeft) return Orientation.TOP_RIGHT;

        if (borderOnTop && borderOnRight) return Orientation.BOTTOM_LEFT;

        if (borderOnTop && borderOnLeft) return Orientation.BOTTOM_RIGHT;

        return Orientation.DEFAULT;
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

        public bool IsPositionOnBorder(int x, int y)
        {
            return IsPositionWithinBorder(x, y) && (x == Left  || x == Right || y == Bottom || y == Top);
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
        DEFAULT,
        TREE,
        PINE,
        ROCK,
        SAND,
        GRASS,
        WATER,
        CLIFF,
        CLIFF_CORNER,
        MOUNTAIN_FACE,
        MOUNTAIN_CORNER,
        MOUNTAIN_TOP,
    }
}

