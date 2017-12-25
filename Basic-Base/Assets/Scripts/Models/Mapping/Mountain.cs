using System;
using System.Linq;
using Assets.Scripts.Models.Structures;

namespace Assets.Scripts.Models.Mapping
{
    public class Mountain : Land
    {
        public Mountain(Map map, Tile tile, Random seed) : base(map, tile)
        {
            Generate(seed);
            //Smooth();
        }

        public sealed override void Smooth()
        {

        }

        protected sealed override void Generate(Random random)
        {
            Border[] mountainZones = generateMountainLayers(random);

            for (int x = 0; x < LAND_WIDTH; x++)
            {
                for (int y = 0; y < LAND_HEIGHT; y++)
                {
                    Tile.TileType type = Tile.TileType.DEFAULT;

                    if(mountainZones.Any(border => border.IsPositionOnBorder(x, y)))
                        type = Tile.TileType.MOUNTAIN_FACE;
                    else if(mountainZones.Any(border => border.IsPositionWithinBorder(x, y)))
                        type = Tile.TileType.MOUNTAIN_TOP;
                    else if (random.Next(0, 100) < 40)
                        type = Tile.TileType.ROCK; 

                    SetLandPiece(x, y, Tile.TileType.GRASS, type);
                }
            }

            foreach (Border layer in mountainZones)
            {
                for (int x = (int)layer.Left; x <= layer.Right; x++)
                {
                    for (int y = (int)layer.Bottom; y <= layer.Top; y++)
                    {
                        Map.Orientation orientation = getMountainFaceOrientation(x, y, layer);
                        Tile.TileType type = map.IsOrientationCorner(orientation) ? Tile.TileType.MOUNTAIN_CORNER : land[x, y].Icon;

                        land[x, y].Icon = type;
                        land[x, y].Orientation = type == Tile.TileType.MOUNTAIN_TOP ? Map.Orientation.Default : orientation;
                    }
                }
            }
        }

        private Border[] generateMountainLayers(Random random)
        {
            int layerCount = random.Next(1, 4);
            Border[] layers = new Border[layerCount];
            Border lastBorder = new Border(LAND_HEIGHT - 1, -1, -1, LAND_WIDTH - 1);

            for (int i = 0; i < layerCount; i++)
            {
                Border mountainZone = new Border(lastBorder.Top - random.Next(1, 3), lastBorder.Bottom + random.Next(1, 3), lastBorder.Left + random.Next(1, 3), lastBorder.Right - random.Next(1, 3));
                layers[i] = mountainZone;
                lastBorder = mountainZone;
            }

            return layers;
        }

        private Map.Orientation getMountainFaceOrientation(int x, int y, Border border)
        {
            if (land[x, y].Icon == Tile.TileType.MOUNTAIN_TOP) return Map.Orientation.Default;

            bool borderOnTop = y + 1 <= border.Top && border.IsPositionOnBorder(x, y + 1);
            bool borderOnBottom = y - 1 >= border.Bottom && border.IsPositionOnBorder(x, y - 1);
            bool borderOnLeft = x - 1 >= border.Left && border.IsPositionOnBorder(x - 1, y);
            bool borderOnRight = x + 1 <= border.Right && border.IsPositionOnBorder(x + 1, y);

            if (borderOnTop && borderOnBottom)
                return (x - 1 < border.Left) ? Map.Orientation.Left : Map.Orientation.Right;

            if (borderOnLeft && borderOnRight)
                return (y - 1 < border.Bottom) ? Map.Orientation.Bottom : Map.Orientation.Top;

            if (borderOnBottom && borderOnRight) return Map.Orientation.TopLeft;

            if (borderOnBottom && borderOnLeft) return Map.Orientation.TopRight;

            if (borderOnTop && borderOnRight) return Map.Orientation.BottomLeft;

            if (borderOnTop && borderOnLeft) return Map.Orientation.BottomRight;

            return Map.Orientation.Default;
        }
    }
}
