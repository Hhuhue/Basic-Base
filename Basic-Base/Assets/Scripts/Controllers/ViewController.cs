using System;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class ViewController : MonoBehaviour
    {
        public GameObject EntityContainer { get; set; }
        public GameObject MapButton { get; set; }

        private GameObject[,] _children;
        private SpriteRenderer _selectorRenderer;
        private Game _game;
        private int _mapHeight;
        private int _mapWidth;

        void Start()
        {
            _game = GameProvider.Game();

            var dimentions = _game.GetMapDimentions();
            _mapHeight = dimentions["Height"];
            _mapWidth = dimentions["Width"];

            setSelector();

            MapButton.GetComponent<Button>().onClick.AddListener(LoadMap);
            _children = new GameObject[View.VIEW_WIDTH, View.VIEW_HEIGHT];

            for (int x = 0; x < View.VIEW_WIDTH; x++)
            {
                for (int y = 0; y < View.VIEW_HEIGHT; y++)
                {
                    Tile tile = _game.GetViewTile(x, y);

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

            CameraBorderState borderState = getCameraBorderState(cameraPosition, cameraSize);

            if (!borderState.AnyBorderReached) return;

            bool originChanged = false;
            int viewModeScale = (Game.ViewMode == View.ViewMode.MAP) ? 1 : 10;

            Vector2 relativeOrigin = _game.GetViewOrigin();
            Vector3 cameraMove = Vector3.zero;

            float originXLimit = _mapWidth * viewModeScale - View.VIEW_WIDTH;
            float originYLimit = _mapHeight * viewModeScale - View.VIEW_HEIGHT;

            CameraEndState endState = getCameraEndState(borderState, relativeOrigin, originXLimit, originYLimit);

            float jumpSizeX = View.VIEW_WIDTH - 1 - cameraSize * 4;
            if (Input.GetKey(KeyCode.A) && borderState.LeftBorderReached && !endState.LeftEndReached)
            {
                originChanged = true;
                if (relativeOrigin.x - jumpSizeX < 0) jumpSizeX = relativeOrigin.x;

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

            float jumpSizeY = View.VIEW_HEIGHT - 1 - cameraSize * 2;
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
            _game.SetViewOrigin(relativeOrigin);
            updateView();
            updateEntities(cameraMove);
        }

        public void LoadTile(Tile tile)
        {
            if (Game.ViewMode == View.ViewMode.LAND) return;

            _selectorRenderer.enabled = false;

            Game.ViewMode = View.ViewMode.LAND;

            Vector2 viewCenter = new Vector2((float)Math.Truncate((float)View.VIEW_WIDTH / 2), (float)Math.Truncate((float)View.VIEW_HEIGHT / 2));
            Vector2 newOrigin = new Vector2((int) tile.Position.x, (int) tile.Position.y) * 10 - viewCenter;
            _game.SetViewOrigin(newOrigin);

            Camera.main.transform.position = new Vector3(View.VIEW_WIDTH / 2 + 5, View.VIEW_HEIGHT / 2 + 5, -5);

            //Todo: to remove
            Vector3 position = new Vector3(View.VIEW_WIDTH / 2 + 5, View.VIEW_HEIGHT / 2 + 5, 2.5f);
            GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/Person"));
            person.transform.position = position;
            person.transform.parent = EntityContainer.transform;

            EntityContainer.SetActive(true);

            updateView();
        }

        public void SelectTile(int xPosition, int yPosition)
        {
            if (Game.ViewMode == View.ViewMode.LAND) return;
            Vector3 position = new Vector3(xPosition + 0.5f, yPosition + 0.5f, 1);

            _selectorRenderer.enabled = true;
            _selectorRenderer.transform.position = position;
        }

        public void LoadMap()
        {
            if (Game.ViewMode == View.ViewMode.MAP) return;
            _selectorRenderer.enabled = true;

            int maxViewXPosition = _mapWidth - View.VIEW_WIDTH;
            int maxViewYPosition = _mapHeight - View.VIEW_HEIGHT;

            Game.ViewMode = View.ViewMode.MAP;
            Vector2 viewCenter = new Vector2(View.VIEW_WIDTH / 2, View.VIEW_HEIGHT / 2);

            Vector2 focusedLandPiecePosition = _game.GetViewOrigin() + viewCenter;
            Vector2 scaledPosition = focusedLandPiecePosition / 10;
            Vector2 scaledOrigin = scaledPosition - viewCenter;        

            scaledOrigin.x = (scaledOrigin.x > maxViewXPosition) ? maxViewXPosition : (scaledOrigin.x < 0) ? 0 : scaledOrigin.x; 
            scaledOrigin.y = (scaledOrigin.y > maxViewYPosition) ? maxViewYPosition : (scaledOrigin.y < 0) ? 0 : scaledOrigin.y;

            Debug.Log("Focus: " + focusedLandPiecePosition.ToString() + ", Scaled focus: " + scaledPosition.ToString() + ", Scaled origin: " + scaledOrigin.ToString());

            _game.SetViewOrigin(scaledOrigin);
            Camera.main.transform.position = new Vector3(View.VIEW_WIDTH / 2 + 5, View.VIEW_HEIGHT / 2 + 5, -5);
        
            EntityContainer.SetActive(false);

            updateView();
        }

        private void setSelector()
        {
            GameObject selector = new GameObject("selector");
            selector.transform.parent = transform;
            selector.transform.position = new Vector3(0, 0, 1);
            _selectorRenderer = selector.AddComponent<SpriteRenderer>();
            _selectorRenderer.sprite = Resources.Load<Sprite>(Game.SpritesPath + "selection");
            _selectorRenderer.enabled = false;
        }

        private void updateView()
        {
            for (int x = 0; x < View.VIEW_WIDTH; x++)
            {
                for (int y = 0; y < View.VIEW_HEIGHT; y++)
                {
                    TileController tile = _children[x, y].GetComponent<TileController>();
                    tile.SetTile(_game.GetViewTile(x, y));
                }
            }
        }

        private void updateEntities(Vector2 move)
        {
            for (int i = 0; i < EntityContainer.transform.childCount; i++)
            {
                Transform entity = EntityContainer.transform.GetChild(i);
                entity.GetComponent<PersonController>().Translate(move);
            }
        }

        private CameraBorderState getCameraBorderState(Vector3 cameraPosition, float cameraSize)
        {
            return new CameraBorderState
            {
                LeftBorderReached = cameraPosition.x - cameraSize * 2 <= 1,
                RightBorderReached = cameraPosition.x + cameraSize * 2 >= View.VIEW_WIDTH,
                BottomBorderReached = cameraPosition.y - cameraSize <= 1,
                TopBorderReached = cameraPosition.y + cameraSize >= View.VIEW_HEIGHT
            };
        }

        private CameraEndState getCameraEndState(CameraBorderState borderState, Vector2 relativeOrigin, float xLimit, float yLimit)
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
}
