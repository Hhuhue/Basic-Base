using System;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using Orientation = Assets.Scripts.Models.Mapping.Map.Orientation;

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
            Func<Tile, bool> condition = (currentTile) => 
                currentTile.Type == Tile.TileType.PLAIN && (
                currentTile.Icon == Tile.TileType.FOREST ||
                currentTile.Icon == Tile.TileType.MOUNTAIN ||
                currentTile.Icon == Tile.TileType.DEFAULT);

            Tile replacement = new Tile() {Type = Tile.TileType.GRASS, Icon = Tile.TileType.DEFAULT};

            CornerSmoother.SetCornerSmoother(ref tile, ref map, ref land);
            CornerSmoother.Smooth(condition, false, replacement);
        }
    }
}
