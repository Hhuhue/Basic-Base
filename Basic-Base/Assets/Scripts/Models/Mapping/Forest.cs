using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Models.Structures;
using Orientation = Assets.Scripts.Models.Mapping.Map.Orientation;

namespace Assets.Scripts.Models.Mapping
{
    public class Forest : Land
    {
        public Forest(Map map, Tile tile) : base(map, tile)
        {
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            Random random = Config.Seed;

            const int ROCK_CHANCE = 2;
            const int PINE_CHANCE = 50;
            const int TREE_CHANCE = 90;

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
                    else if (number < TREE_CHANCE)
                        icon = Tile.TileType.TREE;

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

            if (!isForest[Orientation.TOP] && !isForest[Orientation.LEFT])
                TrimCorner(Orientation.TOP_LEFT);

            if (!isForest[Orientation.TOP] && !isForest[Orientation.RIGHT]) 
                TrimCorner(Orientation.TOP_RIGHT);

            if (!isForest[Orientation.BOTTOM] && !isForest[Orientation.LEFT])
                TrimCorner(Orientation.BOTTOM_LEFT);

            if (!isForest[Orientation.BOTTOM] && !isForest[Orientation.RIGHT])
                TrimCorner(Orientation.BOTTOM_RIGHT);

            if (isForest[Orientation.RIGHT] && isForest[Orientation.LEFT])
            {
                if(!isForest[Orientation.TOP]) TrimSide(Orientation.TOP);
                if(!isForest[Orientation.BOTTOM]) TrimSide(Orientation.BOTTOM);
            }

            if (isForest[Orientation.TOP] && isForest[Orientation.BOTTOM])
            {
                if (!isForest[Orientation.LEFT]) TrimSide(Orientation.LEFT);
                if (!isForest[Orientation.RIGHT]) TrimSide(Orientation.RIGHT);
            }
        }

        private void TrimCorner(Orientation corner)
        {
            int xStart = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.TOP_LEFT) ? 0 : LAND_WIDTH / 2;
            int xMax = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.TOP_LEFT) ? LAND_WIDTH / 2 : LAND_WIDTH;

            int yStart = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.BOTTOM_RIGHT) ? 0 : LAND_HEIGHT / 2;
            int yMax = (corner == Orientation.BOTTOM_LEFT || corner == Orientation.BOTTOM_RIGHT) ? LAND_HEIGHT / 2 : LAND_HEIGHT;

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    int number = Config.Seed.Next(0, 100);

                    if (GetTrimCornerChance(corner, x, y) <= number) land[x, y].Icon = Tile.TileType.DEFAULT;
                }
            }
        }

        private void TrimSide(Orientation side)
        {
            const int SIDE_TRIM_DEPTH = 2;

            int xStart;
            int xMax;

            int yStart;
            int yMax;

            if (side == Orientation.BOTTOM || side == Orientation.TOP)
            {
                xStart = 0;
                xMax = LAND_WIDTH;

                yStart = (side == Orientation.BOTTOM) ? 0 : LAND_HEIGHT - SIDE_TRIM_DEPTH;
                yMax = (side == Orientation.BOTTOM) ? SIDE_TRIM_DEPTH : LAND_HEIGHT;
            }
            else
            {
                yStart = 0;
                yMax = LAND_HEIGHT;

                xStart = (side == Orientation.LEFT) ? 0 : LAND_WIDTH - SIDE_TRIM_DEPTH;
                xMax = (side == Orientation.LEFT) ? SIDE_TRIM_DEPTH : LAND_WIDTH;
            }

            for (int x = xStart; x < xMax; x++)
            {
                for (int y = yStart; y < yMax; y++)
                {
                    int number = Config.Seed.Next(0, 100);

                    if (GetTrimSideChance(side, x, y) <= number) land[x, y].Icon = Tile.TileType.DEFAULT;
                }
            }
        }

        private int GetTrimCornerChance(Orientation corner, int x, int y)
        {
            if (corner == Orientation.BOTTOM_LEFT) return (x + y) * 10;

            if (corner == Orientation.BOTTOM_RIGHT) return (y + LAND_WIDTH - 1 - x) * 10;

            if (corner == Orientation.TOP_LEFT) return (LAND_HEIGHT - 1 - y + x) * 10;

            if (corner == Orientation.TOP_RIGHT) return (LAND_HEIGHT + LAND_WIDTH - 2 - y - x) * 10;

            return 100;
        }

        private int GetTrimSideChance(Orientation side, int x, int y)
        {
            if (side == Orientation.LEFT) return x * 33;

            if (side == Orientation.RIGHT) return (LAND_WIDTH - 1 - x) * 33;

            if (side == Orientation.BOTTOM) return y * 33;

            if (side == Orientation.TOP) return (LAND_HEIGHT - 1 - y) * 33;

            return 100;
        }

    }
}
