﻿using UnityEngine;
using System.Linq;
using System.Reflection;
using TileType = Map.TileType;
using LandType = Land.LandType;
using Orientation = Map.Orientation;
using Border = Land.Border;

public class ViewController : MonoBehaviour
{
    public View ViewField { get; set; }

    private GameObject[,] _children;
    private Map _map;
    private SpriteRenderer _selectorRenderer;

    void Start()
    {
        SetSelector();

        _children = new GameObject[Config.ViewWidth, Config.ViewHeight];
        _map = GetComponent<MapController>().GetMap();

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = _map.GetTile(x, y);

                GameObject tileObject = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                tileObject.transform.parent = transform;
                tileObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 3);
                tileObject.transform.localScale = Vector3.one;

                GameObject icon = tileObject.transform.GetChild(0).gameObject;
                icon.transform.parent = tileObject.transform;

                tileObject.GetComponent<TileController>().SetTile(tile);

                _children[x, y] = tileObject;
            }
        }
    }

    void Update()
    {
        if (!Input.anyKey) return;

        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraSize = mainCamera.orthographicSize;

        CameraBorderState borderState = GetCameraBorderState(cameraPosition, cameraSize);

        if (!borderState.AnyBorderReached) return;

        Vector2 relativeOrigin = ViewField.Origin;
        Vector3 cameraMove = Vector3.zero;

        CameraEndState endState = GetCameraEndState(borderState, relativeOrigin);

        if (Input.GetKey(KeyCode.A) && borderState.LeftBorderReached && !endState.LeftEndReached)
        {
            float jumpSize = Config.ViewWidth - 1 - cameraSize * 4;
            cameraMove.x = jumpSize;
            relativeOrigin.x -= jumpSize;
        }
        else if (Input.GetKey(KeyCode.D) && borderState.RightBorderReached && !endState.RightEndReached)
        {
            float jumpSize = -(cameraPosition.x + 1 - cameraSize * 2);
            cameraMove.x = jumpSize;
            relativeOrigin.x -= jumpSize;
        }

        if (Input.GetKey(KeyCode.S) && borderState.BottomBorderReached && !endState.BottomEndReached)
        {
        }
        else if (Input.GetKey(KeyCode.W) && borderState.TopBorderReached && !endState.TopEndReached)
        {
        }

        //if(Vector3.zero != cameraMove)
        //    mainCamera.transform.position += cameraMove;
        ViewField.SetOrigin(relativeOrigin);
        Debug.Log(relativeOrigin.ToString());
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

    public void LoadTile(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public void SelectTile(int xPosition, int yPosition)
    {
        Vector3 position = new Vector3(xPosition + 0.5f, yPosition + 0.5f, 1);

        _selectorRenderer.enabled = true;
        _selectorRenderer.transform.position = position;
    }

    private CameraBorderState GetCameraBorderState(Vector3 cameraPosition, float cameraSize)
    {
        return new CameraBorderState
        {
            LeftBorderReached = cameraPosition.x - cameraSize * 2 <= 1,
            RightBorderReached = cameraPosition.x + cameraSize * 2 >= Config.ViewWidth - 1,
            BottomBorderReached = cameraPosition.y - cameraSize <= 1,
            TopBorderReached = cameraPosition.y + cameraSize >= Config.ViewHeight - 1
        };
    }

    private CameraEndState GetCameraEndState(CameraBorderState borderState, Vector2 relativeOrigin)
    {
        return new CameraEndState
        {
            LeftEndReached = borderState.LeftBorderReached && relativeOrigin.x <= 0,
            RightEndReached = borderState.RightBorderReached && relativeOrigin.x >= Config.MapWidth,
            BottomEndReached = borderState.BottomBorderReached && relativeOrigin.y <= 0,
            TopEndReached = borderState.TopBorderReached && relativeOrigin.y >= Config.MapHeight
        };
    }

    private struct CameraBorderState
    {
        public bool LeftBorderReached { get; set; }

        public bool RightBorderReached { get; set; }

        public bool BottomBorderReached { get; set; }

        public bool TopBorderReached { get; set; }

        public bool AnyBorderReached
        {
            get
            {
                return LeftBorderReached || RightBorderReached
                    || BottomBorderReached || TopBorderReached;
            }
        }
    }

    private struct CameraEndState
    {
        public bool LeftEndReached { get; set; }

        public bool RightEndReached { get; set; }

        public bool BottomEndReached { get; set; }

        public bool TopEndReached { get; set; }
    }
}
