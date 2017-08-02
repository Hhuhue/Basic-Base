using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;

namespace Assets.Scripts.Tools
{
    public static class CornerSmoother
    {
        private static Tile _tile;
        private static Tile[,] _land;
        private static Map _map;

        public static void SetCornerSmoother(ref Tile tile, ref Map map, ref Tile[,] land)
        {
            _tile = tile;
            _land = land;
            _map = map;
        }

        public static void Smooth(Func<Tile, bool> condition, bool checkDiagonals, params Tile[] replacement)
        {
            if (_tile == null || _land == null || _map == null)
            {
                throw new InvalidOperationException("ConerSmoother improperly set.");
            }

            int xPosition = (int)_tile.Position.x;
            int yPosition = (int)_tile.Position.y;
            int index = 0;

            Map.Orientation[] orientations =
            {
                Map.Orientation.BOTTOM_LEFT,
                Map.Orientation.LEFT,
                Map.Orientation.TOP_LEFT,
                Map.Orientation.BOTTOM,
                Map.Orientation.TOP,
                Map.Orientation.BOTTOM_RIGHT,
                Map.Orientation.RIGHT,
                Map.Orientation.TOP_RIGHT
            };

            Dictionary<Map.Orientation, bool> doesRespectCondition = new Dictionary<Map.Orientation, bool>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Tile currentTile = _map.GetTile(xPosition + x, yPosition + y);
                    doesRespectCondition.Add(orientations[index], currentTile != null && condition(currentTile));

                    index++;
                }
            }

            bool trimTopLeft = doesRespectCondition[Map.Orientation.TOP] && doesRespectCondition[Map.Orientation.LEFT];
            trimTopLeft = (checkDiagonals) ? trimTopLeft && doesRespectCondition[Map.Orientation.TOP_LEFT] : trimTopLeft;

            bool trimBottomLeft = doesRespectCondition[Map.Orientation.BOTTOM] && doesRespectCondition[Map.Orientation.LEFT];
            trimBottomLeft = (checkDiagonals) ? trimBottomLeft && doesRespectCondition[Map.Orientation.BOTTOM_LEFT] : trimBottomLeft;

            bool trimTopRight = doesRespectCondition[Map.Orientation.TOP] && doesRespectCondition[Map.Orientation.RIGHT];
            trimTopRight = (checkDiagonals) ? trimTopRight && doesRespectCondition[Map.Orientation.TOP_RIGHT] : trimTopRight;

            bool trimBottomRight = doesRespectCondition[Map.Orientation.BOTTOM] && doesRespectCondition[Map.Orientation.RIGHT];
            trimBottomRight = (checkDiagonals) ? trimBottomRight && doesRespectCondition[Map.Orientation.BOTTOM_RIGHT] : trimBottomRight;

            if (trimTopLeft) trimCorner(Map.Orientation.TOP_LEFT, ref _land, replacement);

            if (trimTopRight) trimCorner(Map.Orientation.TOP_RIGHT, ref _land, replacement);

            if (trimBottomLeft) trimCorner(Map.Orientation.BOTTOM_LEFT, ref _land, replacement);

            if (trimBottomRight) trimCorner(Map.Orientation.BOTTOM_RIGHT, ref _land, replacement);

            _tile = null;
            _land = null;
            _map = null;
        }

        private static void trimCorner(Map.Orientation corner, ref Tile[,] land, Tile[] replacement)
        {
            Random random = Config.Seed;
            int replacementCount = replacement.Length;

            int xStart = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.TOP_LEFT) ? 0 : Land.LAND_WIDTH / 2;

            int xMax = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.TOP_LEFT) ? Land.LAND_WIDTH / 2 : Land.LAND_WIDTH;

            int yStart = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.BOTTOM_RIGHT) ? 0 : Land.LAND_HEIGHT / 2;

            int yMax = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.BOTTOM_RIGHT) ? Land.LAND_HEIGHT / 2 : Land.LAND_HEIGHT;

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    if (!isTileToChange(corner, x, y)) continue;
                    int index = random.Next(0, replacementCount - 1);

                    land[x, y].Type = replacement[index].Type;
                    land[x, y].Icon = replacement[index].Icon;
                }
            }
        }

        private static bool isTileToChange(Map.Orientation corner, int x, int y)
        {
            bool isOnSide = false;
            bool isInSide = false;

            if (corner == Map.Orientation.BOTTOM_LEFT)
            {
                isOnSide = (x == 0 && y < 6) || (y == 0 && x < 6);
                isInSide = (x < 2 && y < 3) || (x < 3 && y < 2);
            }

            if (corner == Map.Orientation.BOTTOM_RIGHT)
            {
                isOnSide = (x == Land.LAND_WIDTH - 1 && y < 6) || (y == 0 && Land.LAND_WIDTH - x - 1 < 6);
                isInSide = (Land.LAND_WIDTH - 1 - x < 2 && y < 3) || (Land.LAND_WIDTH - 1 - x < 3 && y < 2);
            }

            if (corner == Map.Orientation.TOP_LEFT)
            {
                isOnSide = (x == 0 && Land.LAND_HEIGHT - y - 1 < 6) || (y == Land.LAND_HEIGHT - 1 && x < 6);
                isInSide = (x < 2 && Land.LAND_HEIGHT - y - 1 < 3) || (x < 3 && Land.LAND_HEIGHT - y - 1 < 2);
            }

            if (corner == Map.Orientation.TOP_RIGHT)
            {
                isOnSide = (x == Land.LAND_WIDTH - 1 && Land.LAND_HEIGHT - y - 1 < 6) ||
                    (y == Land.LAND_HEIGHT - 1 && Land.LAND_WIDTH - x - 1 < 6);

                isInSide = (Land.LAND_WIDTH - x - 1 < 2 && Land.LAND_HEIGHT - y - 1 < 3) || 
                    (Land.LAND_WIDTH - x - 1 < 3 && Land.LAND_HEIGHT - y - 1 < 2);
            }

            return (isOnSide || isInSide);
        }
    }
}
