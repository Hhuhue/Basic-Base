using System;
using System.Linq;
using Assets.Scripts.Models.Structures;
using Assets.Scripts.Tools;

namespace Assets.Scripts.Models.Mapping
{
    public class Coast : Land
    {
        private int _sandThickness;

        public Coast(Map map, Tile tile, Random seed) : base(map, tile)
        {
            _sandThickness = seed.Next(1, 3);
            Generate(seed);
            //Smooth();
        }

        protected sealed override void Generate(Random random)
        {
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

        public sealed override void Smooth()
        {
            Func<Tile, bool> condition = (currentTile) => currentTile.Type == Tile.TileType.WATER;
            Tile replacement = new Tile() { Type = Tile.TileType.WATER, Icon = Tile.TileType.DEFAULT };
            
            CornerSmoother.Smooth(tile, condition, false, replacement);

            if (tile.Orientation == Map.Orientation.Top)
            {
                land[LAND_WIDTH - 1, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 1, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 2, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                
                land[0, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;
                land[0, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;
                land[1, LAND_HEIGHT - 3].Icon = Tile.TileType.DEFAULT;

            }
            else if (tile.Orientation == Map.Orientation.Bottom)
            {
                land[LAND_WIDTH - 1, 1].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 1, 2].Icon = Tile.TileType.DEFAULT;
                land[LAND_WIDTH - 2, 2].Icon = Tile.TileType.DEFAULT;

                land[0, 1].Icon = Tile.TileType.DEFAULT;
                land[0, 2].Icon = Tile.TileType.DEFAULT;
                land[1, 2].Icon = Tile.TileType.DEFAULT;
            }
            else if (tile.Orientation == Map.Orientation.Left)
            {
                land[1, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[2, LAND_HEIGHT - 1].Icon = Tile.TileType.DEFAULT;
                land[2, LAND_HEIGHT - 2].Icon = Tile.TileType.DEFAULT;

                land[1, 0].Icon = Tile.TileType.DEFAULT;
                land[2, 0].Icon = Tile.TileType.DEFAULT;
                land[2, 1].Icon = Tile.TileType.DEFAULT;
            }
            else if (tile.Orientation == Map.Orientation.Right)
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
                case Map.Orientation.Top:
                    return getTopBorders(isCoastEnd, thickness);

                case Map.Orientation.Bottom:
                    return getBottomBorders(isCoastEnd, thickness);

                case Map.Orientation.Left:
                    return getLeftBorders(isCoastEnd, thickness);

                case Map.Orientation.Right:
                    return getRightBorders(isCoastEnd, thickness);

                case Map.Orientation.TopLeft:
                    return getBorders(thickness, Map.Orientation.Top, Map.Orientation.Left);

                case Map.Orientation.TopRight:
                    return getBorders(thickness, Map.Orientation.Top, Map.Orientation.Right);

                case Map.Orientation.BottomLeft:
                    return getBorders(thickness, Map.Orientation.Bottom, Map.Orientation.Left);

                case Map.Orientation.BottomRight:
                    return getBorders(thickness, Map.Orientation.Bottom, Map.Orientation.Right);

                default:
                    return getBorders(thickness, Map.Orientation.Default);
            }
        }

        private Border[] getTopBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.Top, Map.Orientation.Right, Map.Orientation.Left)
                : getBorders(thickness, Map.Orientation.Top);
        }

        private Border[] getBottomBorders(bool isCoastEnd, int thickness)
        {
             return isCoastEnd 
                ? getBorders(thickness, Map.Orientation.Bottom, Map.Orientation.Right, Map.Orientation.Left)
                : getBorders(thickness, Map.Orientation.Bottom);
        }

        private Border[] getLeftBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.Left, Map.Orientation.Top, Map.Orientation.Bottom)
                : getBorders(thickness, Map.Orientation.Left);
        }

        private Border[] getRightBorders(bool isCoastEnd, int thickness)
        {
            return isCoastEnd
                ? getBorders(thickness, Map.Orientation.Right, Map.Orientation.Top, Map.Orientation.Bottom)
                : getBorders(thickness, Map.Orientation.Right);
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
                case Map.Orientation.Bottom:
                    return new Border(thickness, 0, 0, LAND_WIDTH - 1);

                case Map.Orientation.Top:
                    return new Border(LAND_HEIGHT - 1, LAND_HEIGHT - thickness - 1, 0, LAND_WIDTH - 1);

                case Map.Orientation.Left:
                    return new Border(LAND_HEIGHT - 1, 0, 0, thickness);

                case Map.Orientation.Right:
                    return new Border(LAND_HEIGHT - 1, 0, LAND_WIDTH - thickness - 1, LAND_WIDTH - 1);

                default:
                    return null;
            }
        }
    }
}
