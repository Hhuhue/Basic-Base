﻿using System;
using System.Linq;

namespace Assets.Scripts.Models.Mapping
{
    public class Mountain : Land
    {
        public Mountain(Map map, Tile tile) : base(map, tile)
        {
            Generate();
            Smooth();
        }

        protected sealed override void Generate()
        {
            System.Random random = Config.Seed;

            Border[] mountainZones = GenerateMountainLayers();

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
                        Map.Orientation orientation = GetMountainFaceOrientation(x, y, layer);
                        Tile.TileType type = map.IsOrientationCorner(orientation) ? Tile.TileType.MOUNTAIN_CORNER : land[x, y].Icon;

                        land[x, y].Icon = type;
                        land[x, y].Orientation = type == Tile.TileType.MOUNTAIN_TOP ? Map.Orientation.DEFAULT : orientation;
                    }
                }
            }
        }

        protected sealed override void Smooth()
        {
            SmoothLand();
        }

        private Border[] GenerateMountainLayers()
        {
            Random random = Config.Seed;
            int layerCount = random.Next(1, 4);
            Border[] layers = new Border[layerCount];
            Border lastBorder = new Border(LAND_HEIGHT - 1, -1, -1, LAND_WIDTH - 1);

            for (int i = 0; i < layerCount; i++)
            {
                Border mountainZone = new Border()
                {
                    Top = lastBorder.Top - random.Next(1, 3),
                    Bottom = lastBorder.Bottom + random.Next(1, 3),
                    Left = lastBorder.Left + random.Next(1, 3),
                    Right = lastBorder.Right - random.Next(1, 3)
                };
                layers[i] = mountainZone;
                lastBorder = mountainZone;
            }

            return layers;
        }

        private Map.Orientation GetMountainFaceOrientation(int x, int y, Border border)
        {
            if (land[x, y].Icon == Tile.TileType.MOUNTAIN_TOP) return Map.Orientation.DEFAULT;

            bool borderOnTop = y + 1 <= border.Top && border.IsPositionOnBorder(x, y + 1);
            bool borderOnBottom = y - 1 >= border.Bottom && border.IsPositionOnBorder(x, y - 1);
            bool borderOnLeft = x - 1 >= border.Left && border.IsPositionOnBorder(x - 1, y);
            bool borderOnRight = x + 1 <= border.Right && border.IsPositionOnBorder(x + 1, y);

            if (borderOnTop && borderOnBottom)
                return (x - 1 < border.Left) ? Map.Orientation.LEFT : Map.Orientation.RIGHT;

            if (borderOnLeft && borderOnRight)
                return (y - 1 < border.Bottom) ? Map.Orientation.BOTTOM : Map.Orientation.TOP;

            if (borderOnBottom && borderOnRight) return Map.Orientation.TOP_LEFT;

            if (borderOnBottom && borderOnLeft) return Map.Orientation.TOP_RIGHT;

            if (borderOnTop && borderOnRight) return Map.Orientation.BOTTOM_LEFT;

            if (borderOnTop && borderOnLeft) return Map.Orientation.BOTTOM_RIGHT;

            return Map.Orientation.DEFAULT;
        }
    }
}
