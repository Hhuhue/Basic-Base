using UnityEngine;
using TileType = Map.TileType;

public class MapController : MonoBehaviour
{
    public int width;
    public int height;
    public int smoothCount;
    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int fillPercentage = 50;

    [Range(0, 100)]
    public int forestPercentage = 50;

    [Range(0, 100)]
    public int mountainPercentage = 5;

    [Range(0, 100)]
    public int coastPercentage = 20;

    private GameObject[,] childs;
    private GameObject selectedChild;
    private SpriteRenderer selectorRenderer;
    private LandController landController;
    private CameraController cameraController;

    private Map map;

    void Start()
    {
        childs = new GameObject[width, height];

        SetSelector();
        LoadMap();
        DrawMap();
        SetLandGenerator();
        SetCamera();
    }

    void SetLandGenerator()
    {
        GameObject lands = new GameObject("Lands");
        lands.transform.position = Vector3.zero;
        LandController controller = lands.AddComponent<LandController>();
        controller.MapUI = gameObject;
        landController = controller;
    }

    private void LoadMap()
    {
        if (useRandomSeed) seed = System.DateTime.UtcNow.ToString();

        Config.MapHeight = height;
        Config.MapWidth = width;
        Config.LandHeight = height / 2;
        Config.LandWidth = width / 2;
        Config.FillRatio = fillPercentage;
        Config.ForestRatio = forestPercentage;
        Config.MountainRatio = mountainPercentage;
        Config.CoastRatio = coastPercentage;
        Config.Seed = new System.Random(seed.GetHashCode());
        Config.SmoothCount = smoothCount;
        Config.TileIconPath = "Sprites/";

        map = new Map();
    }

    void SetCamera()
    {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        cameraController.SetPosition(new Vector3(width / 2 + 0.5f, height / 2 + 0.5f, 0));
        cameraController.Land = landController;
    }

    void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selector.transform.position = new Vector3(0, 0, 1);
        selectorRenderer = selector.AddComponent<SpriteRenderer>();
        selectorRenderer.sprite = Resources.Load<Sprite>("Sprites/selection");
        selectorRenderer.enabled = false;
    }

    void GenerateMap()
    {
        childs = new GameObject[width, height];
    }

    void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 5);
                    GameObject tile = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                    tile.transform.position = position;
                    tile.transform.parent = transform;
                    childs[x, y] = tile;

                    TileType type = map.GetTile(x, y).TileType;
                    type = (type != TileType.WATER) ? TileType.PLAIN : TileType.WATER;

                    TileController controller = tile.GetComponent<TileController>();
                    controller.map = gameObject;
                    controller.tile = map.GetTile(x, y);
                    controller.SetSprite(Config.TileIconPath + type.ToString().ToLower());
                    controller.SetPosition(x, y);

                    type = map.GetTile(x, y).TileType;
                    if (type != TileType.WATER && type != TileType.PLAIN)
                        controller.SetLandType(map.GetTile(x, y));
                }
            }
        }
    }    

    public Map GetMap()
    {
        return map;
    }

    public void LoadTile(Tile tile)
    {
        if (Camera.main.transform.position.z > 0) return;

        landController.RelativeBottomLeft = (tile.Position + new Vector2(-2, -1)) * 10;
        landController.DrawLand();

        cameraController.ChangeView();
        Camera.main.orthographicSize = 5;
        cameraController.SetPosition(new Vector3((float)Config.LandWidth / 2 + 0.5f, (float)Config.LandHeight / 2 + 0.5f, -6));
    }

    public void SelectTile(int x, int y)
    {
        if (selectedChild == childs[x, y])
        {
            selectedChild = null;
            selectorRenderer.enabled = false;
        }
        else
        {
            selectedChild = childs[x, y];
            selectorRenderer.enabled = true;
            selectorRenderer.transform.position = new Vector3(x + 0.5f, y + 0.5f, 1);
        }
    }
}