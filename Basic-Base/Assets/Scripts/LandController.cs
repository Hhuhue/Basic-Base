using UnityEngine;
using System.Linq;
using LandType = Land.LandType;
using Border = Land.Border;

public class LandController : MonoBehaviour
{
    public GameObject mapUI;

    private Map map;
    private GameObject[,] childs;
    private Border borders;

    void Start()
    {
        MapController mapGenerator = mapUI.GetComponent<MapController>();
        map = mapGenerator.GetMap();

        childs = new GameObject[mapGenerator.width, mapGenerator.height];

        int landCount = 0;
        for (int x = 0; x < mapGenerator.width; x++)
        {
            for (int y = 0; y < mapGenerator.height; y++)
            {
                GameObject land = new GameObject("Land-" + ++landCount);
                land.transform.parent = transform;
                land.transform.position = new Vector3(x * 10 + 5, y * 10 + 5, -1);
                land.SetActive(false);
                
                childs[x, y] = land;
            }
        }
    }

    void Update()
    {
        if(States.View == CameraController.CameraView.LAND && CameraController.isPanning)
        {
            float cameraSize = Camera.main.orthographicSize;
            Vector3 cameraPosition = Camera.main.transform.position;

            Border border = new Border()
            {
                Top = (int)(cameraPosition.y + cameraSize) / 10,
                Bottom = (int)(cameraPosition.y - cameraSize - 5) / 10,
                Right = (int)(cameraPosition.x + cameraSize * 2 + 5) / 10,
                Left = (int)(cameraPosition.x - cameraSize * 2 - 5) / 10,
            };
            
            if (borders != null && borders == border) return;

            int yMove = borders.Top < border.Top ? 1 : (borders.Top > border.Top) ? -1 : 0;
            int xMove = borders.Right < border.Right ? 1 : (borders.Right > border.Right) ? -1 : 0;

            Vector2 move = new Vector2(xMove, yMove);
            MoveLandView(move, border);        
            
            borders = border;
        }
    }

    public void DrawLand(Tile tile)
    {
        GameObject child = childs[(int)tile.Position.x, (int)tile.Position.y];
        child.SetActive(true);
        Land land = map.GetLand((int)tile.Position.x, (int)tile.Position.y);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 position = tile.Position;
                Tile landPiece = map.GetLand((int)position.x, (int)position.y).GetLandPiece(x, y);

                GameObject landPieceUI = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPieceUI.transform.position = new Vector3(position.x * 10 + x + 0.5f, position.y * 10 + y + 0.5f, -2);
                landPieceUI.transform.parent = child.transform;
                landPieceUI.transform.localScale = new Vector3(1, 1, 1);

                SpriteRenderer renderer = landPieceUI.GetComponent<SpriteRenderer>();
                LandType landType = landPiece.LandType;

                string path = landType != LandType.WATER
                    ? LandType.GRASS.ToString().ToLower()
                    : LandType.WATER.ToString().ToLower();

                renderer.sprite = Resources.Load<Sprite>("Sprites/" + path);

                if (landType == LandType.GRASS || landType == LandType.WATER) continue;

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPieceUI.transform;
                icon.transform.position = landPieceUI.transform.position + Vector3.back;
                icon.GetComponent<IconController>().SetSprite("Sprites/" + landType.ToString().ToLower());
                icon.transform.localEulerAngles = TileController.OrientationToVector(landPiece.Orientation);
            }
        }
    }

    private void MoveLandView(Vector2 move, Border border)
    {
        if(move.x > 0)
        {
            for (int i = 0; i < 3; i++) ActivateLand(border.Right, border.Bottom + i);
            for (int i = 0; i < 3; i++) DesactivateLand(border.Left - 1, border.Bottom + i);
        }

        if (move.x < 0)
        {
            for (int i = 0; i < 3; i++) ActivateLand(border.Left, border.Bottom + i);
            for (int i = 0; i < 3; i++) DesactivateLand(border.Right + 1, border.Bottom + i);
        }

        if (move.y > 0)
        {
            for (int i = 0; i < 4; i++) ActivateLand(border.Left + i, border.Top);
            for (int i = 0; i < 4; i++) DesactivateLand(border.Left + i, border.Bottom - 1);
        }

        if (move.y < 0)
        {
            for (int i = 0; i < 4; i++) ActivateLand(border.Left + i, border.Bottom);
            for (int i = 0; i < 4; i++) DesactivateLand(border.Left + i, border.Top + 1);
        }
    }

    private void ActivateLand(int x, int y)
    {
        if (!map.IsPositionValid(x, y)) return;

        if (childs[x, y].transform.childCount < 1)
            DrawLand(map.GetTile(x, y));
        else
            childs[x, y].SetActive(true);
    }    

    private void DesactivateLand(int x, int y)
    {
        if (!map.IsPositionValid(x, y)) return;

        childs[x, y].SetActive(false);
    }
}
