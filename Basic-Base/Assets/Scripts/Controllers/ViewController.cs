using UnityEngine;

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
        int multiplier = (Config.ViewMode == View.ViewMode.MAP) ? 0 : 10;
        int modifier = (multiplier == 0) ? -1 : 1;

        Vector2 relativeOrigin = ViewField.Origin;
        Vector3 cameraMove = Vector3.zero;

        float originXLimit = Config.MapWidth * multiplier - Mathf.Round((float)Config.MapWidth / 2) * modifier;
        float originYLimit = Config.MapHeight * multiplier - Mathf.Round((float)Config.MapHeight / 2) * modifier;

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
    }

    void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selector.transform.position = new Vector3(0, 0, 1);
        _selectorRenderer = selector.AddComponent<SpriteRenderer>();
        _selectorRenderer.sprite = Resources.Load<Sprite>(Config.SpritesPath + "selection");
        _selectorRenderer.enabled = false;
    }

    public void LoadTile(Tile tile)
    {
        if (Config.ViewMode == View.ViewMode.LAND) return;
        _selectorRenderer.enabled = false;

        Config.ViewMode = View.ViewMode.LAND;
        ViewField.SetOrigin(new Vector2((int)tile.Position.x -2.5f, (int)tile.Position.y - 1.25f) * 10);
        Camera.main.transform.position = new Vector3(Config.ViewWidth / 2 + 5, Config.ViewHeight / 2 + 5, -5);

        GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/Person"));
        person.transform.position = new Vector3(Config.ViewWidth/2 + 5, Config.ViewHeight/2 + 5, 2.5f);
        person.transform.parent = transform;

        UpdateView();
    }

    public void SelectTile(int xPosition, int yPosition)
    {
        if (Config.ViewMode == View.ViewMode.LAND) return;
        Vector3 position = new Vector3(xPosition + 0.5f, yPosition + 0.5f, 1);

        _selectorRenderer.enabled = true;
        _selectorRenderer.transform.position = position;
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
