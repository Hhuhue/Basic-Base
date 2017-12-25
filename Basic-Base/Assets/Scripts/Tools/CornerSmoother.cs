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
        private static Game _game;
        private static Random _random;

        public static void Initialize(Game game, Random seed)
        {
            _game = game;
            _random = seed;
        }

        public static void Smooth(Tile tile, Func<Tile, bool> condition, bool checkDiagonals, params Tile[] replacement)
        {
            if (tile == null || _game == null)
            {
                throw new InvalidOperationException("Smoother not initialized or tile is null");
            }

            int xPosition = (int)tile.Position.x;
            int yPosition = (int)tile.Position.y;
            int index = 0;

            Map.Orientation[] orientations =
            {
                Map.Orientation.BottomLeft,
                Map.Orientation.Left,
                Map.Orientation.TopLeft,
                Map.Orientation.Bottom,
                Map.Orientation.Top,
                Map.Orientation.BottomRight,
                Map.Orientation.Right,
                Map.Orientation.TopRight
            };

            Dictionary<Map.Orientation, bool> doesRespectCondition = new Dictionary<Map.Orientation, bool>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Tile currentTile = _game.GetMapTile(xPosition + x, yPosition + y);
                    doesRespectCondition.Add(orientations[index], currentTile != null && condition(currentTile));

                    index++;
                }
            }

            bool trimTopLeft = doesRespectCondition[Map.Orientation.Top] && doesRespectCondition[Map.Orientation.Left];
            trimTopLeft = (checkDiagonals) ? trimTopLeft && doesRespectCondition[Map.Orientation.TopLeft] : trimTopLeft;

            bool trimBottomLeft = doesRespectCondition[Map.Orientation.Bottom] && doesRespectCondition[Map.Orientation.Left];
            trimBottomLeft = (checkDiagonals) ? trimBottomLeft && doesRespectCondition[Map.Orientation.BottomLeft] : trimBottomLeft;

            bool trimTopRight = doesRespectCondition[Map.Orientation.Top] && doesRespectCondition[Map.Orientation.Right];
            trimTopRight = (checkDiagonals) ? trimTopRight && doesRespectCondition[Map.Orientation.TopRight] : trimTopRight;

            bool trimBottomRight = doesRespectCondition[Map.Orientation.Bottom] && doesRespectCondition[Map.Orientation.Right];
            trimBottomRight = (checkDiagonals) ? trimBottomRight && doesRespectCondition[Map.Orientation.BottomRight] : trimBottomRight;

            if (trimTopLeft) trimCorner(Map.Orientation.TopLeft, replacement, xPosition, yPosition);

            if (trimTopRight) trimCorner(Map.Orientation.TopRight, replacement, xPosition, yPosition);

            if (trimBottomLeft) trimCorner(Map.Orientation.BottomLeft, replacement, xPosition, yPosition);

            if (trimBottomRight) trimCorner(Map.Orientation.BottomRight, replacement, xPosition, yPosition);
        }

        private static void trimCorner(Map.Orientation corner, Tile[] replacement, int landX, int landY)
        {
            int replacementCount = replacement.Length;

            int xStart = (corner == Map.Orientation.BottomLeft || corner == Map.Orientation.TopLeft) ? 0 : Land.LAND_WIDTH / 2;

            int xMax = (corner == Map.Orientation.BottomLeft || corner == Map.Orientation.TopLeft) ? Land.LAND_WIDTH / 2 : Land.LAND_WIDTH;

            int yStart = (corner == Map.Orientation.BottomLeft || corner == Map.Orientation.BottomRight) ? 0 : Land.LAND_HEIGHT / 2;

            int yMax = (corner == Map.Orientation.BottomLeft || corner == Map.Orientation.BottomRight) ? Land.LAND_HEIGHT / 2 : Land.LAND_HEIGHT;

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    if (!isTileToChange(corner, x, y)) continue;
                    int index = _random.Next(0, replacementCount - 1);

                    _game.SetMapLandTile(landX, landY, x, y, replacement[index].Type, replacement[index].Icon);
                }
            }
        }

        private static bool isTileToChange(Map.Orientation corner, int x, int y)
        {
            bool isOnSide = false;
            bool isInSide = false;

            if (corner == Map.Orientation.BottomLeft)
            {
                isOnSide = (x == 0 && y < 6) || (y == 0 && x < 6);
                isInSide = (x < 2 && y < 3) || (x < 3 && y < 2);
            }

            if (corner == Map.Orientation.BottomRight)
            {
                isOnSide = (x == Land.LAND_WIDTH - 1 && y < 6) || (y == 0 && Land.LAND_WIDTH - x - 1 < 6);
                isInSide = (Land.LAND_WIDTH - 1 - x < 2 && y < 3) || (Land.LAND_WIDTH - 1 - x < 3 && y < 2);
            }

            if (corner == Map.Orientation.TopLeft)
            {
                isOnSide = (x == 0 && Land.LAND_HEIGHT - y - 1 < 6) || (y == Land.LAND_HEIGHT - 1 && x < 6);
                isInSide = (x < 2 && Land.LAND_HEIGHT - y - 1 < 3) || (x < 3 && Land.LAND_HEIGHT - y - 1 < 2);
            }

            if (corner == Map.Orientation.TopRight)
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
