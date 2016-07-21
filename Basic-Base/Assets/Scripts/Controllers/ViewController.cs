using UnityEngine;
using System.Linq;
using TileType = Map.TileType;
using LandType = Land.LandType;
using Orientation = Map.Orientation;
using Border = Land.Border;

public class ViewController : MonoBehaviour
{
    public GameObject MapUI;
    public Vector2 RelativeBottomLeft;
    public SpriteRenderer Selector;
    public CameraController CameraController;

    private Map map;
    private GameObject[,] childs;
    private Border borders;
    private GameObject selectedChild;

    public void Initialize()
    {
        MapController mapGenerator = MapUI.GetComponent<MapController>();
        map = mapGenerator.GetMap();

        childs = new GameObject[Config.ViewWidth, Config.ViewHeight];

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
                controller.Map = MapUI;
                controller.SetPosition(x, y);

                childs[x, y] = landPiece;
            }
        }
    }

    public void DrawLand()
    {
        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Vector2 landPosition = new Vector2((RelativeBottomLeft.x + x) / 10, (RelativeBottomLeft.y + y) / 10);
                Vector2 landPiecePosition = new Vector2((RelativeBottomLeft.x + x) % 10, (RelativeBottomLeft.y + y) % 10);
                Land land = map.GetLand((int)landPosition.x, (int)landPosition.y);
                Tile landPiece = land.GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y);

                GameObject landPieceUI = childs[x, y];
                SpriteRenderer renderer = landPieceUI.GetComponent<SpriteRenderer>();
                landPieceUI.GetComponent<TileController>().enabled = false;

                LandType landType = landPiece.LandType;

                string path = landType != LandType.WATER
                    ? LandType.GRASS.ToString().ToLower()
                    : LandType.WATER.ToString().ToLower();

                renderer.sprite = Resources.Load<Sprite>("Sprites/" + path);

                path = (landType == LandType.GRASS || landType == LandType.WATER) ? "" : "Sprites/" + landType.ToString().ToLower();

                GameObject icon = landPieceUI.transform.GetChild(0).gameObject;
                icon.GetComponent<IconController>().SetSprite(path);
                icon.transform.localEulerAngles = TileController.OrientationToVector(landPiece.Orientation);
            }
        }
    }

    public void DrawMap()
    {
        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Vector2 landPosition = new Vector2(RelativeBottomLeft.x + x, RelativeBottomLeft.y + y);
                Tile tile = map.GetTile((int)landPosition.x, (int)landPosition.y);

                GameObject tileUI = childs[x, y];

                TileType type = tile.TileType;
                type = (type != TileType.WATER) ? TileType.PLAIN : TileType.WATER;

                TileController controller = tileUI.GetComponent<TileController>();
                controller.enabled = true;
                controller.Map = gameObject;
                controller.Tile = tile;
                controller.SetSprite(Config.TileIconPath + type.ToString().ToLower());
                controller.SetPosition(x, y);

                if (tile.TileType != TileType.WATER && tile.TileType != TileType.PLAIN)
                    controller.SetLandType(tile);
            }
        }
    }

    public void OnBorderReached(Orientation side)
    {
        Vector2 move = Vector2.zero;

        switch (side)
        {
            case Orientation.TOP:
                move.y = -(Config.ViewHeight - Camera.main.orthographicSize * 2);
                break;

            case Orientation.BOTTOM:
                move.y = Config.ViewHeight - Camera.main.orthographicSize * 2;
                break;

            case Orientation.LEFT:
                move.x = Config.ViewWidth - Camera.main.orthographicSize * 4;
                break;

            case Orientation.RIGHT:
                move.x = -(Config.ViewWidth - Camera.main.orthographicSize * 4);
                break;
        }

        float newX = RelativeBottomLeft.x + move.x;
        float newY = RelativeBottomLeft.y + move.y;

        if (newX >= 0 && newY >= 0 && map.IsPositionValid((int)(newX + Config.ViewWidth) / 10, (int)(newY + Config.ViewHeight) / 10))
        {
            RelativeBottomLeft += move;

            if (States.View == CameraController.CameraView.LAND)
                DrawLand();
            else
                DrawMap();
        }
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
        if (selectedChild == childs[x, y])
        {
            selectedChild = null;
            Selector.enabled = false;
        }
        else
        {
            selectedChild = childs[x, y];
            Selector.enabled = true;
            Selector.transform.position = new Vector3(x + 0.5f, y + 0.5f, -2);
        }
    }
}
