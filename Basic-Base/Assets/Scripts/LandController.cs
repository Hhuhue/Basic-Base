using UnityEngine;
using System.Linq;
using TileType = TileController.LandType;

public class LandController : MonoBehaviour
{
    public GameObject map;

    private Tile[,] resources;
    private GameObject[,] childs;

    void Start()
    {
        MapGenerator mapGenerator = map.GetComponent<MapGenerator>();
        resources = mapGenerator.GetMap();

        childs = new GameObject[mapGenerator.width, mapGenerator.height];

        int landCount = 0;
        for (int x = 0; x < mapGenerator.width; x++)
        {
            for (int y = 0; y < mapGenerator.height; y++)
            {
                GameObject land = new GameObject("Land-" + ++landCount);
                land.transform.parent = transform;
                land.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

                LandGenerator generator = land.AddComponent<LandGenerator>();
                generator.resourceConcentration = 90;
                generator.GenerateLand(resources, resources[x, y].Position, mapGenerator.seed);
                childs[x, y] = land;

                land.SetActive(false);
            }
        }
    }

    public void DrawLand(Tile tile)
    {
        GameObject land = childs[(int)tile.Position.x, (int)tile.Position.y];
        land.SetActive(true);

        LandGenerator generator = land.GetComponent<LandGenerator>();
        generator.DrawLand(tile);
    }
}
