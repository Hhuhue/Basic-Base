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
    
    private GameObject selectedChild;
    private SpriteRenderer selectorRenderer;
    private ViewController landController;
    private CameraController cameraController;

    private Map map;

    void Start()
    {
        SetSelector();
        LoadMap();
        SetCamera();
        SetLandGenerator();
    }

    void SetLandGenerator()
    {
        ViewController controller = gameObject.AddComponent<ViewController>();
        controller.MapUI = gameObject;
        controller.RelativeBottomLeft = Vector2.zero;
        controller.CameraController = cameraController;
        controller.Selector = selectorRenderer;
        controller.Initialize();
        controller.DrawMap();

        landController = controller;
        cameraController.Land = landController;
    }

    private void LoadMap()
    {
        if (useRandomSeed) seed = System.DateTime.UtcNow.ToString();

        Config.MapHeight = height;
        Config.MapWidth = width;
        Config.ViewHeight = height / 2;
        Config.ViewWidth = width / 2;
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
        Vector3 cameraPosition = new Vector3(Config.ViewWidth/2 + 0.5f, Config.ViewHeight/2 + 0.5f, -5);

        cameraController = Camera.main.transform.GetComponent<CameraController>();
        cameraController.SetPosition(cameraPosition);
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

    public Map GetMap()
    {
        return map;
    }
}