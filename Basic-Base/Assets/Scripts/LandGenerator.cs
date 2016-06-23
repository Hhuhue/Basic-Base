using UnityEngine;
using System.Collections;
using TileType = TileController.LandType;

public class LandGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int resourceConcentration;

    private GameObject[,] landscape;
    private LandPieceType[,] landTypes;

    void Start()
    {
    }

    void Update()
    {

    }

    public void GenerateLand(TileType type, int xPosition, int yPosition, string seed)
    {
        landscape = new GameObject[10, 10];
        landTypes = new LandPieceType[10, 10];

        System.Random random = new System.Random(seed.GetHashCode());

        switch (type)
        {
            case TileType.FOREST:
                
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

    public void DrawLand(TileType type, int xPosition, int yPosition)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.position = new Vector3(xPosition + 0.05f + x * 0.1f, yPosition + 0.05f + y * 0.1f, 0);
                landPiece.transform.parent = transform;
            }
        }
    }

    public enum LandPieceType
    {
        TREE,
        ROCK,
        SAND,
        GRASS,
        WATER,
        CLIFF,
        MOUNTAIN
    }
}