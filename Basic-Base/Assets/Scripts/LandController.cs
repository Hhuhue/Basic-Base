using UnityEngine;
using System.Linq;
using LandType = Land.LandType;
using Border = Land.Border;

public class LandController : MonoBehaviour
{
    public GameObject mapUI;
    public int Height = 30;
    public int Width = 50;

    private Map map;
    private GameObject[,] childs;
    private Border borders;
    private Vector2 relativeBottomLeft;

    void Start()
    {
        MapController mapGenerator = mapUI.GetComponent<MapController>();
        map = mapGenerator.GetMap();

        childs = new GameObject[Width, Height];
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
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

    void Update()
    {
        if (Input.anyKey && States.View == CameraController.CameraView.LAND)
        {
            float cameraSize = Camera.main.orthographicSize;
            float panSpeed = cameraSize * 0.1f;

            Vector2 move = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
            if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
            if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
            if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

            float newX = relativeBottomLeft.x + move.x;
            float newY = relativeBottomLeft.y + move.y;

            if(newX >= 0 && newY >=0 && map.IsPositionValid((int)(newX + Width) / 10, (int)(newY + Height) / 10))
            {
                relativeBottomLeft += move;
                DrawLand(relativeBottomLeft);
            }            
        }
    }

    public void DrawLand(Vector2 origin)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 landPosition = new Vector2((relativeBottomLeft.x + x) / 10, (relativeBottomLeft.y + y) / 10);
                Vector2 landPiecePosition = new Vector2((relativeBottomLeft.x + x) % 10, (relativeBottomLeft.y + y) % 10);
                Land land = map.GetLand((int)landPosition.x, (int)landPosition.y);
                Tile landPiece = land.GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y);

                if(landPiece == null) Debug.Log(landPiecePosition.ToString() + " " + landPosition.ToString());

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
}
