using UnityEngine;
using System.Collections;
using TileType = TileController.LandType;

public class LandGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int resourceConcentration;

    private GameObject[,] landscape;
    private Tile[,] landTypes;

    public void GenerateLand(Tile[,] map, Vector2 position, string seed)
    {
        landscape = new GameObject[10, 10];
        landTypes = new Tile[10, 10];
        
        switch (map[(int)position.x, (int)position.y].TileType)
        {
            case TileType.PLAIN:
                GeneratePlain(seed, map[(int)position.x, (int)position.y]);
                break;

            case TileType.FOREST:
                GenerateForest(seed, map[(int)position.x, (int)position.y]);
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

    private void GenerateForest(string seed, Tile tile)
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = random.Next(0, 100);
                LandPieceType type = number < 2 ? LandPieceType.ROCK
                    : number < 50 ? LandPieceType.PINE
                    : number < 90 ? LandPieceType.TREE
                    : LandPieceType.GRASS;

                landTypes[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    private void GeneratePlain(string seed, Tile tile)
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int number = random.Next(0, 100);
                LandPieceType type = number < 2 ? LandPieceType.ROCK
                    : number < 4 ? LandPieceType.PINE
                    : LandPieceType.GRASS;

                landTypes[x, y] = new Tile()
                {
                    Position = new Vector2(tile.Position.x + x * 0.1f, tile.Position.y + y * 0.1f),
                    LandType = type
                };
            }
        }
    }

    public void DrawLand(Tile tile)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.position = new Vector3(tile.Position.x + 0.05f + x * 0.1f, tile.Position.y + 0.05f + y * 0.1f, 0);
                landPiece.transform.parent = transform;
                landPiece.transform.localScale = new Vector3(1, 1, 1);

                SpriteRenderer renderer = landPiece.GetComponent<SpriteRenderer>();
                string path = tile.LandType != LandPieceType.WATER 
                    ? LandPieceType.GRASS.ToString().ToLower()
                    : LandPieceType.WATER.ToString().ToLower();
                renderer.sprite = Resources.Load<Sprite>("Sprites/LandTiles/" + path);

                if (landTypes[x, y].LandType == LandPieceType.GRASS || landTypes[x, y].LandType == LandPieceType.WATER)
                    continue;

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPiece.transform;
                icon.transform.position = landPiece.transform.position + Vector3.back;
                icon.GetComponent<IconController>().SetSprite("Sprites/LandTiles/" + landTypes[x, y].LandType.ToString().ToLower());
            }
        }
    }

    public enum LandPieceType
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