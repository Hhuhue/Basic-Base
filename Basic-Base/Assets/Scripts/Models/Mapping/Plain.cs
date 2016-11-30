using System;

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

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    int number = random.Next(0, 100);

                    Tile.TileType icon = Tile.TileType.DEFAULT;

                    if (number < 2)
                        icon = Tile.TileType.ROCK;
                    else if (number < 4)
                        icon = Tile.TileType.PINE;

                    SetLandPiece(x, y, Tile.TileType.GRASS, icon);
                }
            }
        }

        protected sealed override void Smooth()
        {
            SmoothLand();
        }
    }
}
