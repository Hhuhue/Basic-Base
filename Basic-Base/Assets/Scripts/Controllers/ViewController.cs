using System;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    public View ViewField { get; set; }
    public GameObject EntityContainer { get; set; }
    public GameObject MapButton { get; set; }

    private GameObject[,] _children;
    private Map _map;
    private SpriteRenderer _selectorRenderer;

    void Start()
    {
        SetSelector();

        MapButton.GetComponent<Button>().onClick.AddListener(LoadMap);
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
                tileObject.GetComponent<TileController>().SetTile(tile);
                tileObject.GetComponent<TileController>().SetPosition(x, y);

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

        bool originChanged = false;
        int viewModeScale = (Config.ViewMode == View.ViewMode.MAP) ? 1 : 10;

        Vector2 relativeOrigin = ViewField.Origin;
        Vector3 cameraMove = Vector3.zero;

        float originXLimit = Config.MapWidth * viewModeScale - Config.ViewWidth;
        float originYLimit = Config.MapHeight * viewModeScale - Config.ViewHeight;

        CameraEndState endState = GetCameraEndState(borderState, relativeOrigin, originXLimit, originYLimit);

        float jumpSizeX = Config.ViewWidth - 1 - cameraSize * 4;
        if (Input.GetKey(KeyCode.A) && borderState.LeftBorderReached && !endState.LeftEndReached)
        {
            originChanged = true;
            if (relativeOrigin.x - jumpSizeX < 0)
                jumpSizeX = relativeOrigin.x;

            cameraMove.x = jumpSizeX;
            relativeOrigin.x -= jumpSizeX;
        }
        else if (Input.GetKey(KeyCode.D) && borderState.RightBorderReached && !endState.RightEndReached)
        {
            originChanged = true;
            if (relativeOrigin.x + jumpSizeX > originXLimit)
                jumpSizeX = originXLimit - relativeOrigin.x;

            cameraMove.x = -jumpSizeX;
            relativeOrigin.x += jumpSizeX;
        }

        float jumpSizeY = Config.ViewHeight - 1 - cameraSize * 2;
        if (Input.GetKey(KeyCode.S) && borderState.BottomBorderReached && !endState.BottomEndReached)
        {
            originChanged = true;
            if (relativeOrigin.y - jumpSizeY < 0)
                jumpSizeY = relativeOrigin.y;

            cameraMove.y = jumpSizeY;
            relativeOrigin.y -= jumpSizeY;
        }
        else if (Input.GetKey(KeyCode.W) && borderState.TopBorderReached && !endState.TopEndReached)
        {
            originChanged = true;
            if (relativeOrigin.y + cameraSize + jumpSizeY > originYLimit)
                jumpSizeY = originYLimit - relativeOrigin.y;

            cameraMove.y = -jumpSizeY;
            relativeOrigin.y += jumpSizeY;
        } 

        if (!originChanged) return;
        mainCamera.transform.position += cameraMove;
        ViewField.SetOrigin(relativeOrigin);
        UpdateView();
        UpdateEntities(cameraMove);
    }

    public void LoadTile(Tile tile)
    {
        if (Config.ViewMode == View.ViewMode.LAND) return;

        _selectorRenderer.enabled = false;

        Config.ViewMode = View.ViewMode.LAND;

        Vector2 viewCenter = new Vector2((float)Math.Truncate((float)Config.ViewWidth / 2), (float)Math.Truncate((float)Config.ViewHeight / 2));
        Vector2 newOrigin = new Vector2((int) tile.Position.x, (int) tile.Position.y) * 10 - viewCenter;
        ViewField.SetOrigin(newOrigin);

        Camera.main.transform.position = new Vector3(Config.ViewWidth / 2 + 5, Config.ViewHeight / 2 + 5, -5);

        Vector3 position = new Vector3(Config.ViewWidth / 2 + 5, Config.ViewHeight / 2 + 5, 2.5f);
        GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/Person"));
        person.transform.position = position;
        person.transform.parent = EntityContainer.transform;

        EntityContainer.SetActive(true);

        UpdateView();
    }

    public void SelectTile(int xPosition, int yPosition)
    {
        if (Config.ViewMode == View.ViewMode.LAND) return;
        Vector3 position = new Vector3(xPosition + 0.5f, yPosition + 0.5f, 1);

        _selectorRenderer.enabled = true;
        _selectorRenderer.transform.position = position;
    }

    public void LoadMap()
    {
        if (Config.ViewMode == View.ViewMode.MAP) return;
        _selectorRenderer.enabled = true;

        int maxViewXPosition = Config.MapWidth - Config.ViewWidth;
        int maxViewYPosition = Config.MapHeight - Config.ViewHeight;

        Config.ViewMode = View.ViewMode.MAP;
        Vector2 viewCenter = new Vector2(Config.ViewWidth / 2, Config.ViewHeight / 2);

        Vector2 focusedLandPiecePosition = ViewField.Origin + viewCenter;
        Vector2 scaledPosition = focusedLandPiecePosition / 10;
        Vector2 scaledOrigin = scaledPosition - viewCenter;        

        scaledOrigin.x = (scaledOrigin.x > maxViewXPosition) ? maxViewXPosition : (scaledOrigin.x < 0) ? 0 : scaledOrigin.x; 
        scaledOrigin.y = (scaledOrigin.y > maxViewYPosition) ? maxViewYPosition : (scaledOrigin.y < 0) ? 0 : scaledOrigin.y;

        Debug.Log("Focus: " + focusedLandPiecePosition.ToString() + ", Scaled focus: " + scaledPosition.ToString() + ", Scaled origin: " + scaledOrigin.ToString());

        ViewField.SetOrigin(scaledOrigin);
        Camera.main.transform.position = new Vector3(Config.ViewWidth / 2 + 5, Config.ViewHeight / 2 + 5, -5);
        
        EntityContainer.SetActive(false);

        UpdateView();
    }

    private void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selector.transform.position = new Vector3(0, 0, 1);
        _selectorRenderer = selector.AddComponent<SpriteRenderer>();
        _selectorRenderer.sprite = Resources.Load<Sprite>(Config.SpritesPath + "selection");
        _selectorRenderer.enabled = false;
    }

    private void UpdateView()
    {
        Tile[,] view = ViewField.GetView();

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                TileController tile = _children[x, y].GetComponent<TileController>();
                tile.SetTile(view[x, y]);
            }
        }
    }

    private void UpdateEntities(Vector2 move)
    {
        for (int i = 0; i < EntityContainer.transform.childCount; i++)
        {
            Transform entity = EntityContainer.transform.GetChild(i);
            entity.GetComponent<PersonController>().Translate(move);
        }
    }

    private CameraBorderState GetCameraBorderState(Vector3 cameraPosition, float cameraSize)
    {
        return new CameraBorderState
        {
            LeftBorderReached = cameraPosition.x - cameraSize * 2 <= 1,
            RightBorderReached = cameraPosition.x + cameraSize * 2 >= Config.ViewWidth,
            BottomBorderReached = cameraPosition.y - cameraSize <= 1,
            TopBorderReached = cameraPosition.y + cameraSize >= Config.ViewHeight
        };
    }

    private CameraEndState GetCameraEndState(CameraBorderState borderState, Vector2 relativeOrigin, float xLimit, float yLimit)
    {
        return new CameraEndState
        {
            LeftEndReached = borderState.LeftBorderReached && relativeOrigin.x <= 0,
            RightEndReached = borderState.RightBorderReached && relativeOrigin.x >= xLimit,
            BottomEndReached = borderState.BottomBorderReached && relativeOrigin.y <= 0,
            TopEndReached = borderState.TopBorderReached && relativeOrigin.y >= yLimit
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
