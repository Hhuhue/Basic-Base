using System;
using System.Collections.Generic;
using Orientation = Assets.Scripts.Models.Mapping.Map.Orientation;

namespace Assets.Scripts.Models.Mapping
{
    public class Plain : Land
    {
        public Plain(Map map, Tile tile) : base(map, tile)
        {
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            Random random = Config.Seed;

            const int ROCK_CHANCE = 2;
            const int PINE_CHANCE = 4;

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    int number = random.Next(0, 100);

                    Tile.TileType icon = Tile.TileType.DEFAULT;

                    if (number < ROCK_CHANCE)
                        icon = Tile.TileType.ROCK;
                    else if (number < PINE_CHANCE)
                        icon = Tile.TileType.PINE;

                    SetLandPiece(x, y, Tile.TileType.GRASS, icon);
                }
            }
        }

        protected sealed override void Smooth()
        {
            int xPosition = (int)tile.Position.x;
            int yPosition = (int)tile.Position.y;
            int index = 0;

            Orientation[] orientations =
            {
                Orientation.BOTTOM_LEFT,
                Orientation.LEFT,
                Orientation.TOP_LEFT,
                Orientation.BOTTOM,
                Orientation.TOP,
                Orientation.BOTTOM_RIGHT,
                Orientation.RIGHT,
                Orientation.TOP_RIGHT
            };

            Dictionary<Orientation, bool> isForest = new Dictionary<Orientation, bool>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Tile currentTile = map.GetTile(xPosition + x, yPosition + y);
                    isForest.Add(orientations[index], currentTile != null && currentTile.Icon == Tile.TileType.FOREST);

                    index++;
                }
            }

            if (isForest[Orientation.TOP] && isForest[Orientation.LEFT])
                FillForestCorner(Orientation.TOP_LEFT);

            if (isForest[Orientation.TOP] && isForest[Orientation.RIGHT])
                FillForestCorner(Orientation.TOP_RIGHT);

            if (isForest[Orientation.BOTTOM] && isForest[Orientation.LEFT])
                FillForestCorner(Orientation.BOTTOM_LEFT);

            if (isForest[Orientation.BOTTOM] && isForest[Orientation.RIGHT])
                FillForestCorner(Orientation.BOTTOM_RIGHT);
        }

        private void FillForestCorner(Orientation corner)
        {
            const int PINE_CHANCE = 50;

            int xStart = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.TOP_LEFT) ? 0 : LAND_WIDTH / 2;
            int xMax = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.TOP_LEFT) ? LAND_WIDTH / 2 : LAND_WIDTH;

            int yStart = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.BOTTOM_RIGHT) ? 0 : LAND_HEIGHT / 2;
            int yMax = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.BOTTOM_RIGHT) ? LAND_HEIGHT / 2 : LAND_HEIGHT;

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    int number = Config.Seed.Next(0, 100);

                    Tile.TileType type = (number < PINE_CHANCE) ? Tile.TileType.PINE : Tile.TileType.TREE;

                    if (GetFillForestCornerChance(corner, x, y) <= number)
                        land[x, y].Icon = type;
                }
            }
        }

        private int GetFillForestCornerChance(Orientation corner, int x, int y)
        {
            if (corner == Orientation.BOTTOM_LEFT) return (x + y) * 10;

            if (corner == Orientation.BOTTOM_RIGHT) return (y + LAND_WIDTH - 1 - x) * 10;

            if (corner == Orientation.TOP_LEFT) return (LAND_HEIGHT - 1 - y + x) * 10;

            if (corner == Orientation.TOP_RIGHT) return (LAND_HEIGHT + LAND_WIDTH - 2 - y - x) * 10;

            return 100;
        }
    }
}
