using UnityEngine;
using TileType = Map.TileType;

public class MapController : MonoBehaviour
{
    public int Width;
    public int Height;
    public int SmoothCount;
    public string Seed;
    public bool UseRandomSeed;

    [Range(0, 100)]
    public int FillPercentage = 50;

    [Range(0, 100)]
    public int ForestPercentage = 50;

    [Range(0, 100)]
    public int MountainPercentage = 5;

    [Range(0, 100)]
    public int CoastPercentage = 20;

    private SpriteRenderer _selectorRenderer;
    private ViewController _viewController;
    private CameraController _cameraController;

    private Map _map;

    void Start()
    {
        SetSelector();
        LoadMap();
        SetCamera();
        SetViewManager();
    }

    void SetViewManager()
    {
        ViewController controller = gameObject.AddComponent<ViewController>();
        controller.ViewField = new View(_map);
        controller.MapUi = gameObject;
        controller.RelativeBottomLeft = Vector2.zero;
        controller.CameraController = _cameraController;
        controller.Selector = _selectorRenderer;
        controller.Initialize();
        controller.DrawMap();

        _viewController = controller;
        _cameraController.Land = _viewController;
    }

    private void LoadMap()
    {
        if (UseRandomSeed) Seed = System.DateTime.UtcNow.ToString();

        Config.MapHeight = Height;
        Config.MapWidth = Width;
        Config.ViewHeight = Height / 2;
        Config.ViewWidth = Width / 2;
        Config.FillRatio = FillPercentage;
        Config.ForestRatio = ForestPercentage;
        Config.MountainRatio = MountainPercentage;
        Config.CoastRatio = CoastPercentage;
        Config.Seed = new System.Random(Seed.GetHashCode());
        Config.SmoothCount = SmoothCount;
        Config.TileIconPath = "Sprites/";

        _map = new Map();
    }

    void SetCamera()
    {
        Vector3 cameraPosition = new Vector3(Config.ViewWidth / 2 + 0.5f, Config.ViewHeight / 2 + 0.5f, -5);

        _cameraController = Camera.main.transform.GetComponent<CameraController>();
        _cameraController.SetPosition(cameraPosition);
    }

    void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selector.transform.position = new Vector3(0, 0, 1);
        _selectorRenderer = selector.AddComponent<SpriteRenderer>();
        _selectorRenderer.sprite = Resources.Load<Sprite>("Sprites/selection");
        _selectorRenderer.enabled = false;
    }

    public Map GetMap()
    {
        return _map;
    }
}