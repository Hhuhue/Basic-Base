using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Structures;
using Assets.Scripts.Tools;
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
            Func<Tile, bool> condition = (currentTile) => currentTile.Type == Tile.TileType.WATER;
            Tile replacement = new Tile() {Type = Tile.TileType.WATER, Icon = Tile.TileType.DEFAULT};

            CornerSmoother.SetCornerSmoother(ref tile, ref map, ref land);
            CornerSmoother.Smooth(condition, false, replacement);

            condition = (currentTile) => currentTile.Icon == Tile.TileType.FOREST;

            Tile[] replacements =
            {
                new Tile() { Type = Tile.TileType.GRASS, Icon = Tile.TileType.PINE },
                new Tile() { Type = Tile.TileType.GRASS, Icon = Tile.TileType.TREE },
                new Tile() { Type = Tile.TileType.GRASS, Icon = Tile.TileType.DEFAULT }

            };

            CornerSmoother.SetCornerSmoother(ref tile, ref map, ref land);
            CornerSmoother.Smooth(condition, true, replacements);
        }
    }
}
