namespace Assets.Scripts.Models.Mapping
{
    public class Water : Land
    {
        public Water(Map map, Tile tile) : base(map, tile)
        {
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    SetLandPiece(x, y, Tile.TileType.WATER, Tile.TileType.DEFAULT);
                }
            }
        }

        protected sealed override void Smooth()
        {
            SmoothLand();
        }
    }
}
