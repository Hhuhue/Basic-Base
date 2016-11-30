using System;

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

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    int number = random.Next(0, 100);

                    Tile.TileType icon = Tile.TileType.DEFAULT;

                    if (number < 2) 
                        icon = Tile.TileType.ROCK;
                    else if (number < 50)
                        icon = Tile.TileType.PINE;
                    else if (number < 90)
                        icon = Tile.TileType.TREE;

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
