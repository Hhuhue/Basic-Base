using System;
using UnityEngine;
using TileType = Map.TileType;

public class Land
{
    private Tile[,] land;
    private Tile tile;
    private Map map;
    
    public Land(Map map, Tile tile)
    {
        this.map = map;
        this.tile = tile;

        land = new Tile[10, 10];

        GenerateLand();
    }

    public Tile GetLandPiece(int x, int y)
    {
        if (x < 0 || x > 9 || y < 0 || y > 9) return null;

        return land[x, y];
    }

    private void GenerateLand()
    {
        switch (tile.TileType)
        {
            case TileType.PLAIN:
                GeneratePlain();
                break;

            case TileType.FOREST:
                GenerateForest();
                break;

            case TileType.MOUNTAIN:

                break;

            case TileType.WATER:
                break;

            case TileType.COAST:
            case TileType.COAST_CORNER:
            case TileType.COAST_END:
                break;
        }

    }

    private void GenerateForest()
    {        
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = map.GetConfiguration().Seed.Next(0, 100);
                LandType type = number < 2 ? LandType.ROCK
                    : number < 50 ? LandType.PINE
                    : number < 90 ? LandType.TREE
                    : LandType.GRASS;

                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    private void GeneratePlain()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = map.GetConfiguration().Seed.Next(0, 100);
                LandType type = number < 2 ? LandType.ROCK
                    : number < 4 ? LandType.PINE
                    : LandType.GRASS;

                land[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    public enum LandType
    {
        TREE,
        PINE,
        ROCK,
        SAND,
        GRASS,
        WATER,
        CLIFF,
        MOUNTAIN
    }
}

