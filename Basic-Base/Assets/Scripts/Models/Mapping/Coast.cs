using System;
using System.Linq;
using Assets.Scripts.Models.Structures;
using Assets.Scripts.Tools;

namespace Assets.Scripts.Models.Mapping
{
    public class Coast : Land
    {
        private int _sandThickness;

        public Coast(Map map, Tile tile) : base(map, tile)
        {
            _sandThickness = Config.Seed.Next(1, 3);
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            Random random = Config.Seed;
            
            bool isCoastEnd = tile.Type == Tile.TileType.COAST_END;
            
            Border[] sandZones = getCoastBorder(_sandThickness, tile.Orientation, isCoastEnd);

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    Tile.TileType icon = (random.Next(0, 10) == 0) ? Tile.TileType.ROCK : Tile.TileType.DEFAULT;

                    if (sandZones.Any(border => border.IsPositionWithinBorder(x, y)))
                        icon = Tile.TileType.SAND;

                    SetLandPiece(x, y, Tile.TileType.GRASS, icon);
                }
            }
        }

        protected sealed override void Smooth()
        {
            Func<Tile, bool> condition = (currentTile) => currentTile.Type == Tile.TileType.WATER;
            Tile replacement = new Tile() { Type = Tile.TileType.WATER, Icon = Tile.TileType.DEFAULT };

            CornerSmoother.SetCornerSmoother(ref tile, ref map, ref land);
            CornerSmoother.Smooth(condition, false, replacement);

            if (tile.Orientation == Map.Orientation.TOP)
            {
                land[LAND_WIDTH - 1, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 1, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 2, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                
                land[0, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;
                land[0, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                land[1, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;

            }
            else if (tile.Orientation == Map.Orientation.BOTTOM)
            {
                land[LAND_WIDTH - 1, 1].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 1, 2].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 2, 2].Icon = Tile.TileType.DEFAULT;

                land[0, 1].Icon = Tile.TileType.DEFAULT;
                land[0, 2].Icon = Tile.TileType.DEFAULT;
                land[1, 2].Icon = Tile.TileType.DEFAULT;
            }
            else if (tile.Orientation == Map.Orientation.LEFT)
            {
                land[1, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[2, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[2, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;

                land[1, 0].Icon = Tile.TileType.DEFAULT;
                land[2, 0].Icon = Tile.TileType.DEFAULT;
                land[2, 1].Icon = Tile.TileType.DEFAULT;
            }
            else if (tile.Orientation == Map.Orientation.RIGHT)
            {
                land[LAND_WIDTH - 2, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 3, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 3, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;

                land[LAND_WIDTH - 2, 0].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 3, 0].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 3, 1].Icon = Tile.TileType.DEFAULT;
            }
        }

        private Border[] getCoastBorder(int thickness, Map.Orientation orientation, bool isCoastEnd)
        {
            switch (orientation)
            {
                case Map.Orientation.TOP:
                    return getTopBorders(isCoastEnd, thickness);

                case Map.Orientation.BOTTOM:
                    return getBottomBorders(isCoastEnd, thickness);

                case Map.Orientation.LEFT:
                    return getLeftBorders(isCoastEnd, thickness);

                case Map.Orientation.RIGHT:
                    return getRightBorders(isCoastEnd, thickness);

                case Map.Orientation.TOP_LEFT:
                    return getBorders(thickness, Map.Orientation.TOP, Map.Orientation.LEFT);

                case Map.Orientation.TOP_RIGHT:
                    return getBorders(thickness, Map.Orientation.TOP, Map.Orientation.RIGHT);

                case Map.Orientation.BOTTOM_LEFT:
                    return getBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.LEFT);

                case Map.Orientation.BOTTOM_RIGHT:
                    return getBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.RIGHT);

                default:
                    return getBorders(thickness, Map.Orientation.DEFAULT);
            }
        }

        private Border[] getTopBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.TOP, Map.Orientation.RIGHT, Map.Orientation.LEFT)
                : getBorders(thickness, Map.Orientation.TOP);
        }

        private Border[] getBottomBorders(bool isCoastEnd, int thickness)
        {
             return isCoastEnd 
                ? getBorders(thickness, Map.Orientation.BOTTOM, Map.Orientation.RIGHT, Map.Orientation.LEFT)
                : getBorders(thickness, Map.Orientation.BOTTOM);
        }

        private Border[] getLeftBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.LEFT, Map.Orientation.TOP, Map.Orientation.BOTTOM)
                : getBorders(thickness, Map.Orientation.LEFT);
        }

        private Border[] getRightBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.RIGHT, Map.Orientation.TOP, Map.Orientation.BOTTOM)
                : getBorders(thickness, Map.Orientation.RIGHT);
        }

        private Border[] getBorders(int thickness, params Map.Orientation[] orientations)
        {
            Border[] borders = new Border[orientations.Length];

            for (int i = 0; i < orientations.Length; i++)
            {
                borders[i] = getBorder(thickness, orientations[i]);
            }

            return borders;
        }

        private Border getBorder(int thickness, Map.Orientation orientation)
        {
            switch (orientation)
            {
                case Map.Orientation.BOTTOM:
                    return new Border(thickness, 0, 0, LAND_WIDTH - 1);

                case Map.Orientation.TOP:
                    return new Border(LAND_HEIGHT - 1, LAND_HEIGHT - thickness - 1, 0, LAND_WIDTH - 1);

                case Map.Orientation.LEFT:
                    return new Border(LAND_HEIGHT - 1, 0, 0, thickness);

                case Map.Orientation.RIGHT:
                    return new Border(LAND_HEIGHT - 1, 0, LAND_WIDTH - thickness - 1, LAND_WIDTH - 1);

                default:
                    return null;
            }
        }
    }
}
