using System;
using System.Linq;
using UnityEngine;
using TileType = Tile.TileType;
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
        switch (tile.GetGlobalType())
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
        System.Random random = Config.Seed;

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                int number = random.Next(0, 100);
                TileType icon = number < 2 ? TileType.ROCK
                    : number < 50 ? TileType.PINE
                    : number < 90 ? TileType.TREE
                    : TileType.DEFAULT;

                SetLandPiece(x, y, TileType.GRASS, icon);
            }
        }
    }

    private void GeneratePlain()
    {
        System.Random random = Config.Seed;

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                int number = random.Next(0, 100);
                TileType icon = number < 2 ? TileType.ROCK
                    : number < 4 ? TileType.PINE
                    : TileType.DEFAULT;

                SetLandPiece(x, y, TileType.GRASS, icon);
            }
        }
    }

    private void GenerateWater()
    {
        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                SetLandPiece(x, y, TileType.WATER, TileType.DEFAULT);
            }
        }
    }

    private void GenerateCoast()
    {
        System.Random random = Config.Seed;

        int waterThickness = random.Next(0, 2);
        int sandThickness = random.Next(1, 3);
        bool isCoastEnd = tile.Type == TileType.COAST_END;

        Border[] waterZones = GetCoastBorder(waterThickness, tile.Orientation, isCoastEnd);
        Border[] sandZones = GetCoastBorder(sandThickness + waterThickness, tile.Orientation, isCoastEnd);

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                TileType icon = waterZones.Any(border => border.IsPositionWithinBorder(x, y)) ? TileType.WATER
                    : sandZones.Any(border => border.IsPositionWithinBorder(x, y)) ? TileType.SAND
                    : TileType.DEFAULT;

                SetLandPiece(x, y, TileType.GRASS, icon);
            }
        }
    }

    private void GenerateMountain()
    {
        System.Random random = Config.Seed;

        Border[] mountainZones = GenerateMountainLayers();

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            for (int y = 0; y < LAND_HEIGHT; y++)
            {
                TileType type = mountainZones.Any(border => border.IsPositionOnBorder(x, y)) ? TileType.MOUNTAIN_FACE
                    : mountainZones.Any(border => border.IsPositionWithinBorder(x, y)) ? TileType.MOUNTAIN_TOP
                    : random.Next(0, 100) < 40 ? TileType.ROCK
                    : TileType.DEFAULT;

                SetLandPiece(x, y, TileType.GRASS, type);
            }
        }

        foreach (Border layer in mountainZones)
        {
            for (int x = (int)layer.Left; x <= layer.Right; x++)
            {
                for (int y = (int)layer.Bottom; y <= layer.Top; y++)
                {
                    Orientation orientation = GetMountainFaceOrientation(x, y, layer);
                    TileType type = map.IsOrientationCorner(orientation) ? TileType.MOUNTAIN_CORNER : land[x, y].Icon;

                    land[x, y].Icon = type;
                    land[x, y].Orientation = type == TileType.MOUNTAIN_TOP ? Orientation.DEFAULT : orientation;
                }
            }
        }
    }

    private void SetLandPiece(int x, int y, TileType type, TileType icon)
    {
        land[x, y] = new Tile()
        {
            Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
            Type = type,
            Icon = icon,
            IsCrossable = type == TileType.GRASS
        };
    }

    private void SmoothLand()
    {
        TileType type = tile.GetGlobalType();
        if (!(type == TileType.PLAIN || type == TileType.FOREST)) return;

        int xPosition = (int)tile.Position.x;
        int yPosition = (int)tile.Position.y;

        Tile topTile = map.GetTile(xPosition, yPosition + 1) ?? new Tile();
        Tile bottomTile = map.GetTile(xPosition, yPosition - 1) ?? new Tile();
        Tile rightTile = map.GetTile(xPosition + 1, yPosition) ?? new Tile();
        Tile leftTile = map.GetTile(xPosition - 1, yPosition) ?? new Tile();

        System.Random random = Config.Seed;

        for (int x = 0; x < LAND_WIDTH; x++)
        {
            TileType newType = land[x, LAND_HEIGHT - 1].Icon;
            Orientation newOrientation = land[x, LAND_HEIGHT - 1].Orientation;

            switch (topTile.GetGlobalType())
            {
                case TileType.WATER:
                    if (x == 0 && leftTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.TOP_LEFT;
                    }
                    else if (x == LAND_WIDTH - 1 && rightTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.TOP_RIGHT;
                    }
                    else
                    {
                        newType = TileType.CLIFF;
                        newOrientation = Orientation.TOP;
                    }
                    break;

                case TileType.FOREST:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 35) ? TileType.TREE
                        : (random.Next(0, 100) < 70) ? TileType.PINE
                        : newType;
                    break;

                case TileType.MOUNTAIN:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 70) ? TileType.ROCK : newType;
                    break;
            }

            land[x, LAND_HEIGHT - 1].Icon = newType;
            land[x, LAND_HEIGHT - 1].Orientation = newOrientation;
            
            newType = land[x, 0].Icon;
            newOrientation = land[x, 0].Orientation;

            switch (bottomTile.GetGlobalType())
            {
                case TileType.WATER:
                    if (x == 0 && leftTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.BOTTOM_LEFT;
                    }
                    else if (x == LAND_WIDTH - 1 && rightTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.BOTTOM_RIGHT;
                    }
                    else
                    {
                        newType = TileType.CLIFF;
                        newOrientation = Orientation.BOTTOM;
                    }
                    break;

                case TileType.FOREST:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 35) ? TileType.TREE
                        : (random.Next(0, 100) < 70) ? TileType.PINE
                        : newType;
                    break;

                case TileType.MOUNTAIN:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 70) ? TileType.ROCK : newType;
                    break;
            }

            land[x, 0].Icon = newType;
            land[x, 0].Orientation = newOrientation;
        }

        for (int y = 0; y < LAND_HEIGHT; y++)
        {
            TileType newType = land[0, y].Icon;
            Orientation newOrientation = land[0, y].Orientation;

            switch (leftTile.GetGlobalType())
            {
                case TileType.WATER:
                    if (y == 0 && bottomTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.BOTTOM_LEFT;
                    }
                    else if (y == LAND_HEIGHT - 1 && topTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.TOP_LEFT;
                    }
                    else
                    {
                        newType = TileType.CLIFF;
                        newOrientation = Orientation.LEFT;
                    }
                    break;

                case TileType.FOREST:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 35) ? TileType.TREE
                        : (random.Next(0, 100) < 70) ? TileType.PINE
                        : newType;
                    break;

                case TileType.MOUNTAIN:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 70) ? TileType.ROCK : newType;
                    break;
            }

            land[0, y].Icon = newType;
            land[0, y].Orientation = newOrientation;

            newType = land[LAND_WIDTH - 1, y].Icon;
            newOrientation = land[LAND_WIDTH - 1, y].Orientation;

            switch (rightTile.GetGlobalType())
            {
                case TileType.WATER:
                    if (y == 0 && bottomTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.BOTTOM_RIGHT;
                    }
                    else if (y == LAND_HEIGHT - 1 && topTile.Type == TileType.WATER)
                    {
                        newType = TileType.CLIFF_CORNER;
                        newOrientation = Orientation.TOP_RIGHT;
                    }
                    else
                    {
                        newType = TileType.CLIFF;
                        newOrientation = Orientation.RIGHT;
                    }
                    break;

                case TileType.FOREST:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 35) ? TileType.TREE
                        : (random.Next(0, 100) < 70) ? TileType.PINE
                        : newType;
                    break;

                case TileType.MOUNTAIN:
                    if (newType == TileType.CLIFF) break;
                    newType = (random.Next(0, 100) < 70) ? TileType.ROCK : newType;
                    break;
            }

            land[LAND_WIDTH - 1, y].Icon = newType;
            land[LAND_WIDTH - 1, y].Orientation = newOrientation;
        }
    }

    private Border[] GenerateMountainLayers()
    {
        System.Random random = Config.Seed;
        int layerCount = random.Next(1, 4);
        Border[] layers = new Border[layerCount];
        Border lastBorder = new Border(LAND_HEIGHT - 1, -1, -1, LAND_WIDTH - 1);

        for (int i = 0; i < layerCount; i++)
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

    private Orientation GetMountainFaceOrientation(int x, int y, Border border)
    {
        if (land[x, y].Icon == TileType.MOUNTAIN_TOP) return Orientation.DEFAULT;

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
                return (!isCoastEnd) ? new[] { new Border(9, 9 - thickness, 0, 9) }
                    : new[]
                        {
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                        };

            case Orientation.BOTTOM:
                return (!isCoastEnd) ? new[] { new Border(thickness, 0, 0, 9) }
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                        };

            case Orientation.LEFT:
                return (!isCoastEnd) ? new[] { new Border(9, 0, 0, thickness) }
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.LEFT, false)[0],
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0]
                        };

            case Orientation.RIGHT:
                return (!isCoastEnd) ? new[] { new Border(9, 0, 9 - thickness, 9) }
                : new[]
                        {
                            GetCoastBorder(thickness, Orientation.RIGHT, false)[0],
                            GetCoastBorder(thickness, Orientation.TOP, false)[0],
                            GetCoastBorder(thickness, Orientation.BOTTOM, false)[0]
                        };

            case Orientation.TOP_LEFT:
                return new[]
                {
                    GetCoastBorder(thickness, Orientation.TOP, false)[0],
                    GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                };

            case Orientation.TOP_RIGHT:
                return new[]
                {
                    GetCoastBorder(thickness, Orientation.TOP, false)[0],
                    GetCoastBorder(thickness, Orientation.RIGHT, false)[0]
                };

            case Orientation.BOTTOM_LEFT:
                return new[]
                {
                    GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                    GetCoastBorder(thickness, Orientation.LEFT, false)[0]
                };

            case Orientation.BOTTOM_RIGHT:
                return new[]
                {
                    GetCoastBorder(thickness, Orientation.BOTTOM, false)[0],
                    GetCoastBorder(thickness, Orientation.RIGHT, false)[0]
                };

            default:
                return new[] { new Border() };
        }
    }

    public struct Surrounding
    {
        public Tile Top { get; set; }
        public Tile Bottom { get; set; }
        public Tile Left { get; set; }
        public Tile Right { get; set; }
    }

    public struct Border
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }

        public Border(float top, float bottom, float left, float right) : this()
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
            return IsPositionWithinBorder(x, y) && (x == Left || x == Right || y == Bottom || y == Top);
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
}

