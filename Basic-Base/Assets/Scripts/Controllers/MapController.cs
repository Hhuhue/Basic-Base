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
    
    private ViewController _viewController;
    private CameraController _cameraController;

    private Map _map;

    void Start()
    {
        LoadMap();
        SetCamera();
        SetViewManager();
    }

    void SetViewManager()
    {
        ViewController controller = gameObject.AddComponent<ViewController>();
        controller.ViewField = new View(_map);

        _viewController = controller;
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
        Config.SpritesPath = "Sprites/";

        _map = new Map();
    }

    void SetCamera()
    {
        Vector3 cameraPosition = new Vector3(Config.ViewWidth / 2 + 0.5f, Config.ViewHeight / 2 + 0.5f, -5);

        _cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraPosition;
    }

    public Map GetMap()
    {
        return _map;
    }
}