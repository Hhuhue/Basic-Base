using UnityEngine;
using System.Linq;
using TileType = TileController.LandType;

public class LandController : MonoBehaviour
{
    public GameObject map;

    private TileType[,] resources;

    void Start()
    {
        MapGenerator mapGenerator = map.GetComponent<MapGenerator>();
        resources = mapGenerator.GetMap();

        int landCount = 0;
        for (int x = 0; x < mapGenerator.width; x++)
        {
            for (int y = 0; y < mapGenerator.height; y++)
            {
                GameObject land = new GameObject("Land-" + ++landCount);
                land.transform.parent = transform;
                land.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                LandGenerator generator = land.AddComponent<LandGenerator>();
                generator.GenerateLand(resources[x,y], x, y);
                land.SetActive(false);
            }
        }
    }
}
