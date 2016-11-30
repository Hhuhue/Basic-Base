using UnityEngine;
using TileType = Assets.Scripts.Models.Mapping.Tile.TileType;
using Orientation = Assets.Scripts.Models.Mapping.Map.Orientation;

namespace Assets.Scripts.Models.Mapping
{
    public abstract class Land
    {
        protected Tile[,] land;
        protected Tile tile;
        protected Map map;

        protected const int LAND_HEIGHT = 10;
        protected const int LAND_WIDTH = 10;

        protected Land(Map map, Tile tile)
        {
            this.map = map;
            this.tile = tile;

            land = new Tile[LAND_WIDTH, LAND_HEIGHT];
        }

        public Tile GetLandPiece(int x, int y)
        {
            if (x < 0 || x >= LAND_WIDTH || y < 0 || y >= LAND_HEIGHT) return null;

            return land[x, y];
        }

        protected abstract void Generate();

        protected abstract void Smooth();

        protected void SetLandPiece(int x, int y, TileType type, TileType icon)
        {
            land[x, y] = new Tile()
            {
                Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                Type = type,
                Icon = icon,
                IsCrossable = type == TileType.GRASS
            };
        }

        protected void SmoothLand()
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
}

