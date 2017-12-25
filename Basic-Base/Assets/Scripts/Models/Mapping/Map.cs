using Assets.Scripts.Tools;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Models.Mapping
{
    /// <summary>
    /// The world map
    /// </summary>
    public class Map
    {
        private Tile[,] _map;
        private Land[,] _lands;
        private int _mapHeight;
        private int _mapWidth;

        /// <summary>
        /// Creates a new instance of map
        /// </summary>
        /// <param name="configuration">The generation configuration. </param>
        public Map(Config configuration)
        {
            _mapHeight = configuration.MapHeight;
            _mapWidth = configuration.MapWidth;
            _map = new Tile[_mapWidth, _mapHeight];
            _lands = new Land[_mapWidth, _mapHeight];
            
            randomFillMap(configuration);
            smoothMap(configuration);
            generateResource(configuration.Seed, configuration.ForestRatio, Tile.TileType.FOREST);
            generateResource(configuration.Seed, configuration.MountainRatio, Tile.TileType.MOUNTAIN);
            generateResource(configuration.Seed, configuration.CoastRatio, Tile.TileType.COAST);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        public void GenerateLands(Random seed)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    switch (_map[x, y].GetGlobalType())
                    {
                        case Tile.TileType.PLAIN:
                            _lands[x, y] = new Plain(this, _map[x, y], seed);
                            break;

                        case Tile.TileType.WATER:
                            _lands[x, y] = new Water(this, _map[x, y], seed);
                            break;

                        case Tile.TileType.FOREST:
                            _lands[x, y] = new Forest(this, _map[x, y], seed);
                            break;

                        case Tile.TileType.MOUNTAIN:
                            _lands[x, y] = new Mountain(this, _map[x, y], seed);
                            break;

                        default:
                            _lands[x, y] = new Coast(this, _map[x, y], seed);
                            break;
                    }
                    _lands[x, y].Smooth();
                }
            }
        }

        /// <summary>
        /// Check if the coordinates are within the map
        /// </summary>
        /// <param name="x">The x coordinates</param>
        /// <param name="y">The y coordinates</param>
        /// <returns></returns>
        public bool IsPositionValid(int x, int y)
        {
            return y >= 0 && x >= 0 && y < _mapHeight && x < _mapWidth;
        }

        /// <summary>
        /// Gets the tile at the given position
        /// </summary>
        /// <param name="x">The x coordinates</param>
        /// <param name="y">The y coordinates</param>
        /// <returns></returns>
        public Tile GetTile(int x, int y)
        {
            if (!IsPositionValid(x, y)) return null;

            return _map[x, y];
        }

        /// <summary>
        /// Gets the land at the given position.
        /// </summary>
        /// <param name="x">The x coordinates</param>
        /// <param name="y">The y coordinates</param>
        /// <returns></returns>
        public Land GetLand(int x, int y)
        {
            if (!IsPositionValid(x, y)) return null;

            return _lands[x, y];
        }
        
        /// <summary>
        /// Checks if the given orientation value defines a corners.
        /// </summary>
        /// <param name="orientation">The orientation value. </param>
        /// <returns>Whether </returns>
        public bool IsOrientationCorner(Orientation orientation)
        {
            return orientation == Orientation.BottomLeft ||
                   orientation == Orientation.BottomRight ||
                   orientation == Orientation.TopLeft ||
                   orientation == Orientation.TopRight;
        }

        //Todo: Transfert to game class 
        public void SaveMap()
        {
            SaveManager.Save(_map, _mapHeight, _mapWidth);
        }

        private void randomFillMap(Config configuration)
        {
            for (int x = 0; x < configuration.MapWidth; x++)
            {
                for (int y = 0; y < configuration.MapHeight; y++)
                {
                    _map[x, y] = new Tile
                    {
                        Position = new Vector2(x, y),
                        Type = configuration.Seed.Next(0, 100) < configuration.FillRatio ? Tile.TileType.PLAIN : Tile.TileType.WATER,
                        Orientation = Orientation.Default
                    };
                }
            }
        }

        private void smoothMap(Config configuration)
        {
            for (int i = 0; i < configuration.SmoothCount; i++)
            {
                for (int x = 0; x < configuration.MapWidth; x++)
                {
                    for (int y = 0; y < configuration.MapHeight; y++)
                    {
                        int surroundingLandCount = getSurroundingLandCount(x, y);

                        if (surroundingLandCount > 4) _map[x, y].Type = Tile.TileType.PLAIN;
                        if (surroundingLandCount < 4) _map[x, y].Type = Tile.TileType.WATER;
                    }
                }
            }
        }

        private int getSurroundingLandCount(int xPosition, int yPosition)
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

        void generateResource(Random random, int ratio, Tile.TileType resource)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    if (_map[x, y].Type == Tile.TileType.WATER) continue;

                    int number = random.Next(0, 100);

                    if (resource == Tile.TileType.COAST)
                    {
                        Orientation coastOrientation = getCoastOrientation(x, y);
                        if (coastOrientation == Orientation.Default || number >= ratio) continue;

                        Tile.TileType coastType = isCoastEnd(x, y) 
                            ? Tile.TileType.COAST_END : IsOrientationCorner(coastOrientation) 
                            ? Tile.TileType.COAST_CORNER : Tile.TileType.COAST;

                        _map[x, y].Icon = coastType;
                        _map[x, y].Orientation = coastOrientation;
                    }
                    else
                    {
                        _map[x, y].Icon = (number < ratio) ? resource : _map[x, y].Icon;
                    }
                }
            }
        }

        private Orientation getCoastOrientation(int x, int y)
        {
            bool waterOnTop = y + 1 < _mapHeight && _map[x, y + 1].Type == Tile.TileType.WATER;
            bool waterOnBottom = y - 1 >= 0 && _map[x, y - 1].Type == Tile.TileType.WATER;
            bool waterOnLeft = x - 1 >= 0 && _map[x - 1, y].Type == Tile.TileType.WATER;
            bool waterOnRight = x + 1 < _mapWidth && _map[x + 1, y].Type == Tile.TileType.WATER;

            if (waterOnBottom && waterOnTop)
            {
                return waterOnRight ? Orientation.Right : (waterOnLeft ? Orientation.Left : Orientation.Default);
            }

            if (waterOnRight && waterOnLeft)
            {
                return waterOnTop ? Orientation.Top : (waterOnBottom ? Orientation.Bottom : Orientation.Default);
            }

            if (waterOnTop)
            {
                return waterOnRight
                    ? Orientation.TopRight
                    : (waterOnLeft ? Orientation.TopLeft : Orientation.Top);
            }

            if (waterOnBottom)
            {
                return waterOnRight
                    ? Orientation.BottomRight
                    : (waterOnLeft ? Orientation.BottomLeft : Orientation.Bottom);
            }

            return waterOnLeft ? Orientation.Left : (waterOnRight ? Orientation.Right : Orientation.Default);
        }

        private bool isCoastEnd(int x, int y)
        {
            int trueConditionCount = 0;
            trueConditionCount += (y + 1 < _mapHeight && _map[x, y + 1].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (y - 1 >= 0 && _map[x, y - 1].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (x - 1 >= 0 && _map[x - 1, y].Type == Tile.TileType.WATER) ? 1 : 0;
            trueConditionCount += (x + 1 < _mapWidth && _map[x + 1, y].Type == Tile.TileType.WATER) ? 1 : 0;

            return trueConditionCount > 2;
        }

        public enum Orientation
        {
            Default,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}
