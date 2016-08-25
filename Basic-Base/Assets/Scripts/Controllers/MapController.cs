using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public bool UseMenuConfig;
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

    public GameObject EntityContainer;
    public GameObject MapButton;

    private ViewController _viewController;
    private CameraController _cameraController;
    private PersonController _personController;

    private Map _map;

    void Start()
    {
        LoadMap();
        SetCamera();
        SetViewManager();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _map.SaveMap();
        }
    }

    void SetViewManager()
    {
        ViewController controller = gameObject.AddComponent<ViewController>();
        controller.ViewField = new View(_map);
        controller.EntityContainer = EntityContainer;
        controller.MapButton = MapButton;

        _viewController = controller;
    }

    public Map GetMap()
    {
        return _map;
    }

    public void SelectPerson(PersonController person)
    {
        _personController = person;
    }

    private void LoadMap()
    {
        if (!UseMenuConfig)
        {
            if (UseRandomSeed) Seed = System.DateTime.UtcNow.ToString();

            Config.MapHeight = Height;
            Config.MapWidth = Width;
            Config.FillRatio = FillPercentage;
            Config.ForestRatio = ForestPercentage;
            Config.MountainRatio = MountainPercentage;
            Config.CoastRatio = CoastPercentage;
            Config.Seed = new System.Random(Seed.GetHashCode());
            Config.SmoothCount = SmoothCount;
            Config.SpritesPath = "Sprites/";
        }

        _map = new Map();
    }

    private void SetCamera()
    {
        Vector3 cameraPosition = new Vector3(Config.ViewWidth / 2 + 0.5f, Config.ViewHeight / 2 + 0.5f, -5);

        _cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraPosition;
    }
}