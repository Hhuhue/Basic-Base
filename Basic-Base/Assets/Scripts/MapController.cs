﻿using UnityEngine;
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
        SetCamera();
        LoadMap();
        DrawMap();
        SetLandGenerator();
    }

    void SetLandGenerator()
    {
        GameObject lands = new GameObject("Lands");
        lands.transform.position = Vector3.zero;
        LandController controller = lands.AddComponent<LandController>();
        controller.map = gameObject;
        landController = controller;
    }

    private void LoadMap()
    {
        if (useRandomSeed) seed = System.DateTime.UtcNow.ToString();

        Config config = new Config()
        {
            Height = height,
            Width = width,
            FillRatio = fillPercentage,
            ForestRatio = forestPercentage,
            MountainRatio = mountainPercentage,
            CoastRatio = coastPercentage,
            Seed = new System.Random(seed.GetHashCode()),
            SmoothCount = smoothCount,
            TileIconPath = "Sprites/",
            LandIconPath = "Sprites/LandTiles/"
        };

        map = new Map(config);
    }

    void SetCamera()
    {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        cameraController.SetPosition(new Vector3(width / 2, height / 2, 0));
        cameraController.mapWidth = width;
        cameraController.mapHeight = height;
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
                    controller.tile = map.GetTile(x, y);
                    controller.SetSprite(map.GetConfiguration().TileIconPath + type.ToString().ToLower());
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

        for (int x = (int)tile.Position.x - 2; x < (int)tile.Position.x + 3; x++)
        {
            for (int y = (int)tile.Position.y - 1; y < (int)tile.Position.y + 2; y++)
            {
                if (map.IsPositionValid(x, y))
                {
                    landController.DrawLand(map.GetTile(x, y));
                }
            }
        }

        cameraController.ChangeView();
        cameraController.SetPosition(new Vector3(tile.Position.x + 0.5f, tile.Position.y + 0.5f, -6));
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