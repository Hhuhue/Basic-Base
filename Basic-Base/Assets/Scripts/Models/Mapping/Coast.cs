using System;
using System.Linq;
using Assets.Scripts.Models.Structures;

namespace Assets.Scripts.Models.Mapping
{
    public class Coast : Land
    {
        public Coast(Map map, Tile tile) : base(map, tile)
        {
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            Random random = Config.Seed;

            int waterThickness = random.Next(0, 2);
            int sandThickness = random.Next(1, 3);
            bool isCoastEnd = tile.Type == Tile.TileType.COAST_END;

            Border[] waterZones = GetCoastBorder(waterThickness, tile.Orientation, isCoastEnd);
            Border[] sandZones = GetCoastBorder(sandThickness + waterThickness, tile.Orientation, isCoastEnd);

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    Tile.TileType icon = Tile.TileType.DEFAULT;

                    if (waterZones.Any(border => border.IsPositionWithinBorder(x, y)))
                        icon = Tile.TileType.WATER;
                    else if (sandZones.Any(border => border.IsPositionWithinBorder(x, y)))
                        icon = Tile.TileType.SAND;

                    SetLandPiece(x, y, Tile.TileType.GRASS, icon);
                }
            }
        }

        protected sealed override void Smooth()
        {
            
        }

        private Border[] GetCoastBorder(int thickness, Map.Orientation orientation, bool isCoastEnd)
        {
            switch (orientation)
            {
                case Map.Orientation.TOP:
                    return GetTopBorders(isCoastEnd, thickness);

                case Map.Orientation.BOTTOM:
                    return GetBottomBorders(isCoastEnd, thickness);

                case Map.Orientation.LEFT:
                    return GetLeftBorders(isCoastEnd, thickness);

                case Map.Orientation.RIGHT:
                    return GetRightBorders(isCoastEnd, thickness);

                case Map.Orientation.TOP_LEFT:
                    return GetBorders(thickness, Map.Orientation.TOP, Map.Orientation.LEFT);

                case Map.Orientation.TOP_RIGHT:
                    return GetBorders(thickness, Map.Orientation.TOP, Map.Orientation.RIGHT);

                case Map.Orientation.BOTTOM_LEFT:
                    return GetBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.LEFT);

                case Map.Orientation.BOTTOM_RIGHT:
                    return GetBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.RIGHT);

                default:
                    return GetBorders(thickness, Map.Orientation.DEFAULT);
            }
        }

        private Border[] GetTopBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? GetBorders(thickness, Map.Orientation.TOP, Map.Orientation.RIGHT, Map.Orientation.LEFT)
                : GetBorders(thickness, Map.Orientation.TOP);
        }

        private Border[] GetBottomBorders(bool isCoastEnd, int thickness)
        {
             return isCoastEnd 
                ? GetBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.RIGHT, Map.Orientation.LEFT)
                : GetBorders(thickness, Map.Orientation.BOTTOM);
        }

        private Border[] GetLeftBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? GetBorders(thickness, Map.Orientation.LEFT, Map.Orientation.TOP, Map.Orientation.BOTTOM)
                : GetBorders(thickness, Map.Orientation.LEFT);
        }

        private Border[] GetRightBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? GetBorders(thickness, Map.Orientation.RIGHT, Map.Orientation.TOP, Map.Orientation.BOTTOM)
                : GetBorders(thickness, Map.Orientation.RIGHT);
        }

        private Border[] GetBorders(int thickness, params Map.Orientation[] orientations)
        {
            Border[] borders = new Border[orientations.Length];

            for (int i = 0; i < orientations.Length; i++)
            {
                borders[i] = GetBorder(thickness, orientations[i]);
            }

            return borders;
        }

        private Border GetBorder(int thickness, Map.Orientation orientation)
        {
            switch (orientation)
            {
                case Map.Orientation.BOTTOM:
                    return new Border(thickness, 0, 0, 9);

                case Map.Orientation.TOP:
                    return new Border(9, 9 - thickness, 0, 9);

                case Map.Orientation.LEFT:
                    return new Border(9, 0, 0, thickness);

                case Map.Orientation.RIGHT:
                    return new Border(9, 0, 9 - thickness, 9);

                default:
                    return null;
            }
        }
    }
}
