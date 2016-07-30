using UnityEngine;
using System.Linq;
using System.Reflection;
using TileType = Map.TileType;
using LandType = Land.LandType;
using Orientation = Map.Orientation;
using Border = Land.Border;

public class ViewController : MonoBehaviour
{
    public GameObject MapUi;
    public Vector2 RelativeBottomLeft;
    public SpriteRenderer Selector;
    public CameraController CameraController;
    public View ViewField;
    
    private GameObject[,] _childs;
    private GameObject _selectedChild;

    public void Initialize()
    {
        _childs = new GameObject[Config.ViewWidth, Config.ViewHeight];

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                landPiece.transform.parent = transform;
                landPiece.transform.position = new Vector3(x + 0.5f, y + 0.5f, 1);
                landPiece.transform.localScale = new Vector3(1, 1, 1);

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPiece.transform;
                icon.transform.position = landPiece.transform.position + Vector3.back;

                TileController controller = landPiece.GetComponent<TileController>();
                controller.Map = MapUi;
                controller.SetPosition(x, y);

                _childs[x, y] = landPiece;
            }
        }
    }

    public void DrawLand()
    {
        Tile[,] view = ViewField.GetView();

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = view[x, y];

                GameObject landPieceUi = _childs[x, y];
                SpriteRenderer renderer = landPieceUi.GetComponent<SpriteRenderer>();
                landPieceUi.GetComponent<TileController>().enabled = false;

                LandType landType = tile.LandType;

                string path = landType != LandType.WATER
                    ? LandType.GRASS.ToString().ToLower()
                    : LandType.WATER.ToString().ToLower();

                renderer.sprite = Resources.Load<Sprite>("Sprites/" + path);

                path = (landType == LandType.GRASS || landType == LandType.WATER) ? "" : "Sprites/" + landType.ToString().ToLower();

                GameObject icon = landPieceUi.transform.GetChild(0).gameObject;
                icon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);
                icon.transform.localEulerAngles = TileController.OrientationToVector(tile.Orientation);
            }
        }
    }

    public void DrawMap()
    {
        Tile[,] view = ViewField.GetView();

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = view[x, y];

                GameObject tileUi = _childs[x, y];

                TileType type = tile.TileType;
                type = (type != TileType.WATER) ? TileType.PLAIN : TileType.WATER;

                TileController controller = tileUi.GetComponent<TileController>();
                controller.enabled = true;
                controller.Map = gameObject;
                controller.Tile = tile;
                controller.SetSprite(Config.TileIconPath + type.ToString().ToLower());
                controller.SetPosition(x, y);
                controller.SetIcon(tile);
            }
        }
    }

    public void OnBorderReached(Orientation side)
    {
        ViewField.MoveView(side, Camera.main.orthographicSize);

        if (States.View == View.ViewMode.LAND)
            DrawLand();
        else
            DrawMap();
    }

    public void LoadTile(Tile tile)
    {
        if (Camera.main.transform.position.z > 0) return;

        RelativeBottomLeft = (tile.Position + new Vector2(-2, -1)) * 10;
        DrawLand();

        CameraController.ChangeView();
        Camera.main.orthographicSize = 5;
        CameraController.SetPosition(new Vector3((float)Config.ViewWidth / 2 + 0.5f, (float)Config.ViewHeight / 2 + 0.5f, -6));
    }

    public void SelectTile(int x, int y)
    {
        if (_selectedChild == _childs[x, y])
        {
            _selectedChild = null;
            Selector.enabled = false;
        }
        else
        {
            _selectedChild = _childs[x, y];
            Selector.enabled = true;
            Selector.transform.position = new Vector3(x + 0.5f, y + 0.5f, -2);
        }
    }
}
