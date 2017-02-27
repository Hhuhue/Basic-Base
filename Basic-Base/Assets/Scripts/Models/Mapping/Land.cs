using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Models.Mapping
{
    public abstract class Land
    {
        public const int LAND_HEIGHT = 10;
        public const int LAND_WIDTH = 10;

        protected Tile[,] land;
        protected Tile tile;
        protected Map map;

        protected Land(Map map, Tile tile)
        {
            this.map = map;
            this.tile = tile;

            land = new Tile[LAND_WIDTH, LAND_HEIGHT];
        }

        public Tile GetLandPiece(int x, int y)
        {
            if (x < 0 || x >= LAND_WIDTH || y < 0 || y >= LAND_HEIGHT) return null;

            return land[x, y];
        }

        protected abstract void Generate();

        protected abstract void Smooth();

        protected void SetLandPiece(int x, int y, Tile.TileType type, Tile.TileType icon)
        {
            land[x, y] = new Tile
            {
                Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                Type = type,
                Icon = icon
            };
        }
    }
}

