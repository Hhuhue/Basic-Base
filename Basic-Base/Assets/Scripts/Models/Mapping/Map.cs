using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Models.Mapping
{
    public class Map
    {
        private Tile[,] _map;
        private Land[,] _lands;

        public Map()
        {
            _map = new Tile[Config.MapWidth, Config.MapHeight];
            _lands = new Land[Config.MapWidth, Config.MapHeight];

            RandomFillMap();
            SmoothMap();
            GenerateResource(Config.ForestRatio, Tile.TileType.FOREST);
            GenerateResource(Config.MountainRatio, Tile.TileType.MOUNTAIN);
            GenerateResource(Config.CoastRatio, Tile.TileType.COAST);
            GenerateLands();
        }

        public bool IsPositionValid(int x, int y)
        {
            return y >= 0 && x >= 0 && y < Config.MapHeight && x < Config.MapWidth;
        }

        public Tile GetTile(int x, int y)
        {
            if (!IsPositionValid(x, y)) return null;

            return _map[x, y];
        }

        public Land GetLand(int x, int y)
        {
            if (!IsPositionValid(x, y)) return null;

            return _lands[x, y];
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
            SaveManager.Save(_map, Config.MapHeight, Config.MapWidth);
        }

        private void RandomFillMap()
        {
            for (int x = 0; x < Config.MapWidth; x++)
            {
                for (int y = 0; y < Config.MapHeight; y++)
                {
                    _map[x, y] = new Tile
                    {
                        Position = new Vector2(x, y),
                        Type = Config.Seed.Next(0, 100) < Config.FillRatio ? Tile.TileType.PLAIN : Tile.TileType.WATER,
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

                        if (surroundingLandCount > 4) _map[x, y].Type = Tile.TileType.PLAIN;
                        if (surroundingLandCount < 4) _map[x, y].Type = Tile.TileType.WATER;
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
                        if (_map[x, y].Type != Tile.TileType.WATER) landCount++;
                    }
                }
            }

            return landCount;
        }

        void GenerateResource(int ratio, Tile.TileType resource)
        {
            Random random = Config.Seed;

            for (int x = 0; x < Config.MapWidth; x++)
            {
                for (int y = 0; y < Config.MapHeight; y++)
                {
                    if (_map[x, y].Type == Tile.TileType.WATER) continue;

                    int number = random.Next(0, 100);

                    if (resource == Tile.TileType.COAST)
                    {
                        Orientation coastOrientation = GetCoastOrientation(x, y);
                        if (coastOrientation == Orientation.DEFAULT || number >= ratio) continue;

                        Tile.TileType coastType = IsCoastEnd(x, y) ? Tile.TileType.COAST_END
                            : IsOrientationCorner(coastOrientation) ? Tile.TileType.COAST_CORNER
                                : Tile.TileType.COAST;

                        _map[x, y].Icon = coastType;
                        _map[x, y].Orientation = coastOrientation;
                    }
                    else
                    {
                        _map[x, y].Icon = number < ratio ? resource : _map[x, y].Icon;
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
                    switch (_map[x,y].GetGlobalType())
                    {
                        case Tile.TileType.PLAIN:
                            _lands[x, y] = new Plain(this, _map[x, y]);
                            break;

                        case Tile.TileType.WATER:
                            _lands[x, y] = new Water(this, _map[x, y]);
                            break;
                        
                        case Tile.TileType.FOREST:
                            _lands[x, y] = new Forest(this, _map[x, y]);
                            break;
                        
                        case Tile.TileType.MOUNTAIN:
                            _lands[x, y] = new Mountain(this, _map[x, y]);
                            break;

                        default:
                            _lands[x, y] = new Coast(this, _map[x, y]);
                            break;
                    }
                    
                }
            }
        }

        private Orientation GetCoastOrientation(int x, int y)
        {
            bool waterOnTop = y + 1 < Config.MapHeight && _map[x, y + 1].Type == Tile.TileType.WATER;
            bool waterOnBottom = y - 1 >= 0 && _map[x, y - 1].Type == Tile.TileType.WATER;
            bool waterOnLeft = x - 1 >= 0 && _map[x - 1, y].Type == Tile.TileType.WATER;
            bool waterOnRight = x + 1 < Config.MapWidth && _map[x + 1, y].Type == Tile.TileType.WATER;

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
            trueConditionCount += (y + 1 < Config.MapHeight && _map[x, y + 1].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (y - 1 >= 0 && _map[x, y - 1].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (x - 1 >= 0 && _map[x - 1, y].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (x + 1 < Config.MapWidth && _map[x + 1, y].Type == Tile.TileType.WATER) ? 1 : 0;

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
}
