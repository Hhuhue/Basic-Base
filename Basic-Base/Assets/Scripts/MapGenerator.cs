using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int fillPercentage;

    private GameObject[,] childs;
    private string[,] map;
    private const string GRASS = "Sprites/grass";
    private const string WATER = "Sprites/water";

    void Start()
    {
        GenerateMap();
        RandomFillMap();
        SmoothMap();
        DrawMap();
    }

    void GenerateMap()
    {
        map = new string[width, height];
        childs = new GameObject[width, height];
    }

    void RandomFillMap()
    {
        if (useRandomSeed) seed = Time.time.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = random.Next(0, 100) < fillPercentage ? GRASS : WATER;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingLandCount = GetSurroundingLandCount(x, y);

                map[x, y] = surroundingLandCount >= 4 ? GRASS : WATER;
            }
        }
    }

    int GetSurroundingLandCount(int xPosition, int yPosition)
    {
        int landCount = 0;

        for(int x = xPosition - 1; x < xPosition + 2; x++)
        {
            for (int y = yPosition - 1; y < yPosition + 2; x++)
            {
                if (IsPositionValid(x, y) && x != xPosition && y != yPosition)
                {
                    if (map[x, y] == GRASS) landCount++;
                }
                    
            }
        }

        return landCount;
    }

    void DrawMap()
    {
        if(map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tile = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Tile"));
                    Vector2 position = new Vector2(x + 0.5f, y + 0.5f);

                    tile.GetComponent<LandController>().SetSprite(map[x, y]);
                    tile.transform.position = position;
                    tile.transform.parent = transform;

                    childs[x, y] = tile;                   
                }
            }
        }
    }

    public bool IsPositionValid(int x, int y)
    {
        return y >= 0 && x >= 0 && y < height && x < width;
    }
}
