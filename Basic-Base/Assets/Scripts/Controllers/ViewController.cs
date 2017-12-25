using System;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    /// <summary>
    /// Class managing the view
    /// </summary>
    public class ViewController : MonoBehaviour
    {
        public GameObject EntityContainer { get; set; }
        public GameObject MapButton { get; set; }

        private GameObject[,] _children;
        private SpriteRenderer _selectorRenderer;
        private Game _game;
        private int _mapHeight;
        private int _mapWidth;
        
        private bool _updateView;

        void Start()
        {
            _game = GameProvider.Game();

            Dictionary<string, int> dimentions = _game.GetMapDimentions();
            _mapHeight = dimentions["Height"];
            _mapWidth = dimentions["Width"];

            //Initialize the tile selector
            setSelector();

            MapButton.GetComponent<Button>().onClick.AddListener(LoadMap);
            _children = new GameObject[View.VIEW_WIDTH, View.VIEW_HEIGHT];

            //Create and place the view tiles
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
            //Check if the user tries to move
            if (!Input.anyKey) return;

            //Get the camera state
            Camera mainCamera = Camera.main;
            Vector3 cameraPosition = mainCamera.transform.position;
            float cameraSize = mainCamera.orthographicSize;

            //Get the borders of the camera
            BorderState viewState = getViewBorderState(cameraPosition, cameraSize);

            //Check if the view could have to change
            if (viewState.AnyBorderReached())
            {
                //Assume that the view don't need to change
                _updateView = false;

                //Scale the map limits according to the view mode
                int viewModeScale = (Game.ViewMode == View.ViewMode.MAP) ? 1 : 10;

                Vector2 viewRelativeOrigin = _game.GetViewOrigin();
                Vector3 cameraMove = Vector3.zero;

                //Define the view's origin coordinates max values
                float originXLimit = _mapWidth * viewModeScale - View.VIEW_WIDTH;
                float originYLimit = _mapHeight * viewModeScale - View.VIEW_HEIGHT;

                //Check if any map border is reached
                BorderState mapState = getMapBorderState(viewState, viewRelativeOrigin, originXLimit, originYLimit);

                //Set the camera position modification in X
                float jumpSize = View.VIEW_WIDTH - 1 - cameraSize * 4;
                float jumpValue = getViewModifications(KeyCode.A, viewState, mapState, -jumpSize, viewRelativeOrigin.x, 0);

                if (jumpValue == 0)
                {
                    jumpValue = getViewModifications(KeyCode.D, viewState, mapState, jumpSize, viewRelativeOrigin.x, originXLimit);
                }

                if (jumpValue != 0)
                {
                    cameraMove.x = -jumpValue;
                    viewRelativeOrigin.x += jumpValue;
                }

                jumpSize = View.VIEW_HEIGHT - 1 - cameraSize * 2;
                jumpValue = getViewModifications(KeyCode.S, viewState, mapState, -jumpSize, viewRelativeOrigin.y, 0);

                if (jumpValue == 0)
                {
                    jumpValue = getViewModifications(KeyCode.W, viewState, mapState, jumpSize, viewRelativeOrigin.y, originYLimit);
                }

                if (jumpValue != 0)
                {
                    cameraMove.y = -jumpValue;
                    viewRelativeOrigin.y += jumpValue;
                }

                if (!_updateView) return;

                mainCamera.transform.position += cameraMove;
                _game.SetViewOrigin(viewRelativeOrigin);

                updateView();
                updateEntities(cameraMove);
            }
        }

        public void LoadTile(Tile tile)
        {
            if (Game.ViewMode == View.ViewMode.LAND) return;

            _selectorRenderer.enabled = false;

            Game.ViewMode = View.ViewMode.LAND;

            Vector2 viewCenter = new Vector2((float)Math.Truncate((float)View.VIEW_WIDTH / 2), (float)Math.Truncate((float)View.VIEW_HEIGHT / 2));
            Vector2 newOrigin = new Vector2((int)tile.Position.x, (int)tile.Position.y) * 10 - viewCenter;
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

        private BorderState getViewBorderState(Vector3 cameraPosition, float cameraSize)
        {
            return new BorderState
            {
                LeftBorderReached = cameraPosition.x - cameraSize * 2 <= 1,
                RightBorderReached = cameraPosition.x + cameraSize * 2 >= View.VIEW_WIDTH,
                BottomBorderReached = cameraPosition.y - cameraSize <= 1,
                TopBorderReached = cameraPosition.y + cameraSize >= View.VIEW_HEIGHT
            };
        }

        private BorderState getMapBorderState(BorderState borderState, Vector2 viewRelativeOrigin, float xLimit, float yLimit)
        {
            return new BorderState
            {
                LeftBorderReached = borderState.LeftBorderReached && viewRelativeOrigin.x <= 0,
                RightBorderReached = borderState.RightBorderReached && viewRelativeOrigin.x >= xLimit,
                BottomBorderReached = borderState.BottomBorderReached && viewRelativeOrigin.y <= 0,
                TopBorderReached = borderState.TopBorderReached && viewRelativeOrigin.y >= yLimit
            };
        }

        private float getViewModifications(KeyCode key, BorderState viewState, BorderState mapState, float moveValue, float relativeOriginCoordinate, float maxValue)
        {
            //If the camera reached a side of the view and if the end of the map of the same side isn't reached
            if (Input.GetKey(key) && viewState.BorderStateFromKey(key) && !mapState.BorderStateFromKey(key))
            {
                //The origin has to be updated
                _updateView = true;

                int direction = (moveValue < 0) ? -1 : 1;

                //Adjust the modification if we are reaching the an end of the map 
                if (relativeOriginCoordinate * direction + moveValue * direction > maxValue)
                {
                    moveValue = (maxValue - relativeOriginCoordinate * direction) * direction;
                }

                return moveValue;
            }

            return 0;
        }

        /// <summary>
        /// Structure telling which view border is reached by the camera
        /// </summary>
        private struct BorderState
        {
            public bool LeftBorderReached { get; set; }

            public bool RightBorderReached { get; set; }

            public bool BottomBorderReached { get; set; }

            public bool TopBorderReached { get; set; }

            public bool AnyBorderReached()
            {
                return LeftBorderReached || RightBorderReached || BottomBorderReached || TopBorderReached;
            }

            public bool BorderStateFromKey(KeyCode key)
            {
                switch (key)
                {
                    case KeyCode.A: return LeftBorderReached;

                    case KeyCode.D: return RightBorderReached;

                    case KeyCode.W: return TopBorderReached;

                    case KeyCode.S: return BottomBorderReached;

                    default: return false;
                }
            }
        }
    }
}
