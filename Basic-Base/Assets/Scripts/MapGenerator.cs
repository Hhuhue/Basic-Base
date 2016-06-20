using UnityEngine;
using System.Collections;
using TileType = LandController.LandType;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int smoothCount;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int fillPercentage;

    private GameObject[,] childs;
    private GameObject selectedChild;
    private SpriteRenderer selectorRenderer;

    private TileType[,] map;
    private TileType[,] resources;

    void Start()
    {
        SetSelector();
        GenerateMap();
        RandomFillMap();

        for (int i = 0; i < smoothCount; i++)
        {
            SmoothMap();
        }

        DrawMap();
        resources = map;
        GenerateForest();
        GenerateMountain();
        GenerateCoast();
        DrawResources();
    }

    void Update()
    {
        
    }

    void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selectorRenderer = selector.AddComponent<SpriteRenderer>();
        selectorRenderer.sprite = Resources.Load<Sprite>("Sprites/selection");
        selectorRenderer.enabled = false;
    }

    void GenerateMap()
    {
        map = new LandController.LandType[width, height];
        childs = new GameObject[width, height];
    }

    void RandomFillMap()
    {
        if (useRandomSeed) seed = System.DateTime.UtcNow.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = random.Next(0, 100) < fillPercentage ? TileType.PLAIN : TileType.WATER;
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

                if (surroundingLandCount > 4) map[x, y] = TileType.PLAIN;
                if (surroundingLandCount < 4) map[x, y] = TileType.WATER;
            }
        }
    }
    
    void GenerateForest()
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x,y] == TileType.PLAIN)
                {
                    resources[x, y] = random.Next(0, 100) < 50 ? TileType.FOREST : TileType.PLAIN;                     
                }
            }
        }
    }

    void GenerateMountain()
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != TileType.WATER)
                {
                    resources[x, y] = random.Next(0, 100) > 95 ? TileType.MOUNTAIN : resources[x, y];
                }
            }
        }
    }

    void GenerateCoast()
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != TileType.WATER)
                {
                    bool waterOnTop = (y + 1 < height) && map[x, y + 1] == TileType.WATER;
                    bool waterOnBottom = (y - 1 >= 0) && map[x, y - 1] == TileType.WATER;
                    bool waterOnLeft = (x - 1 >= 0) && map[x - 1, y] == TileType.WATER;
                    bool waterOnRight = (x + 1 < width) && map[x + 1, y] == TileType.WATER;

                    if(waterOnRight || waterOnBottom || waterOnLeft || waterOnTop)
                        resources[x, y] = random.Next(0, 100) > 70 ? TileType.COAST : resources[x, y];
                }
            }
        }
    }

    int GetSurroundingLandCount(int xPosition, int yPosition)
    {
        int landCount = 0;

        for (int x = xPosition - 1; x < xPosition + 2; x++)
        {
            for (int y = yPosition - 1; y < yPosition + 2; y++)
            {
                if (IsPositionValid(x, y) && !(x == xPosition && y == yPosition))
                {
                    if (map[x, y] != TileType.WATER) landCount++;
                }
            }
        }

        return landCount;
    }

    void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tile = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                    Vector2 position = new Vector2(x + 0.5f, y + 0.5f);

                    tile.transform.position = position;
                    tile.transform.parent = transform;
                    LandController controller = tile.GetComponent<LandController>();
                    controller.SetSprite("Sprites/" + map[x, y].ToString().ToLower());
                    controller.SetPosition(x, y);

                    childs[x, y] = tile;
                }
            }
        }
    }

    void DrawResources()
    {
        if (resources != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (resources[x, y] != TileType.PLAIN && resources[x, y] != TileType.WATER)
                    {
                        childs[x, y].GetComponent<LandController>().SetLandType(resources[x, y]);
                    }
                }
            }
        }
    }

    public bool IsPositionValid(int x, int y)
    {
        return y >= 0 && x >= 0 && y < height && x < width;
    }

    public void SelectTile(int x, int y)
    {
        if (selectedChild == childs[x,y])
        {
            selectedChild = null;
            selectorRenderer.enabled = false;
        }
        else
        {
            selectedChild = childs[x, y];
            selectorRenderer.enabled = true;
            selectorRenderer.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        }
    }
}