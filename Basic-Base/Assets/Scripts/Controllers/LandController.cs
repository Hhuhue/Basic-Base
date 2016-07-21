using UnityEngine;
using System.Linq;
using LandType = Land.LandType;
using Orientation = Map.Orientation;
using Border = Land.Border;

public class LandController : MonoBehaviour
{
    public GameObject MapUI;
    public Vector2 RelativeBottomLeft;

    private Map map;
    private GameObject[,] childs;
    private Border borders;

    void Start()
    {
        MapController mapGenerator = MapUI.GetComponent<MapController>();
        map = mapGenerator.GetMap();

        childs = new GameObject[Config.LandWidth, Config.LandHeight];

        for (int x = 0; x < Config.LandWidth; x++)
        {
            for (int y = 0; y < Config.LandHeight; y++)
            {
                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.parent = transform;
                landPiece.transform.position = new Vector3(x + 0.5f, y + 0.5f, -2);
                landPiece.transform.localScale = new Vector3(1, 1, 1);

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPiece.transform;
                icon.transform.position = landPiece.transform.position + Vector3.back;

                childs[x, y] = landPiece;
            }
        }
    }

    public void DrawLand()
    {
        for (int x = 0; x < Config.LandWidth; x++)
        {
            for (int y = 0; y < Config.LandHeight; y++)
            {
                Vector2 landPosition = new Vector2((RelativeBottomLeft.x + x) / 10, (RelativeBottomLeft.y + y) / 10);
                Vector2 landPiecePosition = new Vector2((RelativeBottomLeft.x + x) % 10, (RelativeBottomLeft.y + y) % 10);
                Land land = map.GetLand((int)landPosition.x, (int)landPosition.y);
                Tile landPiece = land.GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y);

                if (landPiece == null) Debug.Log(landPiecePosition + " " + landPosition);

                GameObject landPieceUI = childs[x, y];
                SpriteRenderer renderer = landPieceUI.GetComponent<SpriteRenderer>();
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

    public void OnBorderReached(Orientation side)
    {
        Vector2 move = Vector2.zero;

        switch (side)
        {
            case Orientation.TOP:
                move.y = -(Config.LandHeight - Camera.main.orthographicSize * 2);
                break;

            case Orientation.BOTTOM:
                move.y = Config.LandHeight - Camera.main.orthographicSize * 2;
                break;

            case Orientation.LEFT:
                move.x = Config.LandWidth - Camera.main.orthographicSize * 4;
                break;
                
            case Orientation.RIGHT:
                move.x = -(Config.LandWidth - Camera.main.orthographicSize * 4);
                break;
        }

        float newX = RelativeBottomLeft.x + move.x;
        float newY = RelativeBottomLeft.y + move.y;

        if (newX >= 0 && newY >= 0 && map.IsPositionValid((int)(newX + Config.LandWidth) / 10, (int)(newY + Config.LandHeight) / 10))
        {
            RelativeBottomLeft += move;
            DrawLand();
        }

    }
}
