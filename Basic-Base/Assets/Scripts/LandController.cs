using UnityEngine;
using System.Linq;
using LandType = Land.LandType;

public class LandController : MonoBehaviour
{
    public GameObject map;

    private Map resources;
    private GameObject[,] childs;
    private CameraBorder borders;

    void Start()
    {
        MapController mapGenerator = map.GetComponent<MapController>();
        resources = mapGenerator.GetMap();

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

            CameraBorder border = new CameraBorder()
            {
                Top = (int)(cameraPosition.y + cameraSize) / 10,
                Bottom = (int)(cameraPosition.y - cameraSize - 5) / 10,
                Left = (int)(cameraPosition.x + cameraSize * 2 + 5) / 10,
                Right = (int)(cameraPosition.x - cameraSize * 2 - 5) / 10,
            };

            Debug.Log(borders.ToString() + " " + border.ToString());

            if (borders != null && borders == border) return;

            borders = border;

            for (int x = border.Left - 1; x < border.Right + 2; x++)
            {
                for (int y = border.Bottom - 1; y < border.Top + 2; y++)
                {
                    if (!resources.IsPositionValid(x, y) || childs[x, y].activeSelf) continue;

                    DrawLand(resources.GetTile(x, y));
                }
            }
        }
    }

    public void DrawLand(Tile tile)
    {
        //Debug.Log("Drawing " + tile.TileType + " at " + tile.Position.x + ", " + tile.Position.y );
        GameObject land = childs[(int)tile.Position.x, (int)tile.Position.y];
        land.SetActive(true);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 position = tile.Position;

                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.position = new Vector3(position.x * 10 + x + 0.5f, position.y * 10 + y + 0.5f, -2);
                landPiece.transform.parent = land.transform;
                landPiece.transform.localScale = new Vector3(1, 1, 1);

                SpriteRenderer renderer = landPiece.GetComponent<SpriteRenderer>();
                string path = tile.TileType != Map.TileType.WATER
                    ? LandType.GRASS.ToString().ToLower()
                    : LandType.WATER.ToString().ToLower();

                renderer.sprite = Resources.Load<Sprite>("Sprites/" + path);
                LandType landType = resources.GetLand((int)position.x, (int)position.y).GetLandPiece(x, y).LandType;

                if (landType == LandType.GRASS || landType == LandType.WATER) continue;

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPiece.transform;
                icon.transform.position = landPiece.transform.position + Vector3.back;
                icon.GetComponent<IconController>().SetSprite("Sprites/" + landType.ToString().ToLower());
            }
        }
    }

    public struct CameraBorder
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public static bool operator ==(CameraBorder border1, CameraBorder border2)
        {
            return border1.Top == border2.Top &&
                   border1.Bottom == border2.Bottom &&
                   border1.Left == border2.Left &&
                   border1.Right == border2.Right;
        }

        public static bool operator !=(CameraBorder border1, CameraBorder border2)
        {
            return !(border1 == border2);
        }

        public string ToString()
        {
            return "Top: " + Top + " Bottom: " + Bottom + " Left: " + Left + " Right: " + Right;
        }
    }
}
