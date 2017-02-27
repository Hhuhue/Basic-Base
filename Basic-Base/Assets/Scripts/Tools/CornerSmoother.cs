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
        public static void Smooth(ref Tile tile, ref Map map, ref Tile[,] land, Func<Tile, bool> condition, params Tile[] replacement)
        {
            int xPosition = (int)tile.Position.x;
            int yPosition = (int)tile.Position.y;
            int index = 0;

            Map.Orientation[] orientations =
            {
                Map.Orientation.LEFT,
                Map.Orientation.BOTTOM,
                Map.Orientation.TOP,
                Map.Orientation.RIGHT
            };

            Dictionary<Map.Orientation, bool> isWater = new Dictionary<Map.Orientation, bool>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (!(x == 0 ^ y == 0)) continue;

                    Tile currentTile = map.GetTile(xPosition + x, yPosition + y);
                    isWater.Add(orientations[index], currentTile != null && condition(currentTile));

                    index++;
                }
            }

            if (isWater[Map.Orientation.TOP] && isWater[Map.Orientation.LEFT])
                TrimCorner(Map.Orientation.TOP_LEFT, ref land, replacement);

            if (isWater[Map.Orientation.TOP] && isWater[Map.Orientation.RIGHT])
                TrimCorner(Map.Orientation.TOP_RIGHT, ref land, replacement);

            if (isWater[Map.Orientation.BOTTOM] && isWater[Map.Orientation.LEFT])
                TrimCorner(Map.Orientation.BOTTOM_LEFT, ref land, replacement);

            if (isWater[Map.Orientation.BOTTOM] && isWater[Map.Orientation.RIGHT])
                TrimCorner(Map.Orientation.BOTTOM_RIGHT, ref land, replacement);


        }

        private static void TrimCorner(Map.Orientation corner, ref Tile[,] land, Tile[] replacement)
        {
            Random random = Config.Seed;
            int replacementCount = replacement.Length;

            int xStart = (corner == Map.Orientation.BOTTOM_LEFT || 
                corner == Map.Orientation.TOP_LEFT) ? 0 : Land.LAND_WIDTH / 2;

            int xMax = (corner == Map.Orientation.BOTTOM_LEFT || 
                corner == Map.Orientation.TOP_LEFT) ? Land.LAND_WIDTH / 2 : Land.LAND_WIDTH;

            int yStart = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.BOTTOM_RIGHT) 
                ? 0 : Land.LAND_HEIGHT / 2;

            int yMax = (corner == Map.Orientation.BOTTOM_LEFT || corner == Map.Orientation.BOTTOM_RIGHT) 
                ? Land.LAND_HEIGHT / 2 : Land.LAND_HEIGHT;

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    if (!DoChangeTile(corner, x, y)) continue;
                    int index = random.Next(0, replacementCount - 1);

                    land[x, y].Type = replacement[index].Type;
                    land[x, y].Icon = replacement[index].Icon;
                }
            }
        }

        private static bool DoChangeTile(Map.Orientation corner, int x, int y)
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
