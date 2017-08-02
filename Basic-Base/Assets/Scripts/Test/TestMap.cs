using UnityEngine;
using System.Collections;
using Assets.Scripts.Controllers;

public class TestMap : MonoBehaviour
{

    // Use this for initialization
    private int[,] map;
    private string[] sprites;
    private GameObject[,] childs;

    private int height = 30;
    private int width = 50;

    void Start()
    {
        map = new int[width, height];
        childs = new GameObject[width, height];
        sprites = new[] { "plain", "water", "sand", "mountain_top" };
        System.Random random = new System.Random();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 5);
                GameObject tile = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                childs[x, y] = tile;
                Destroy(tile.GetComponent<TileController>());

                tile.transform.position = position;
                tile.transform.parent = transform;
                map[x, y] = random.Next(0, 4);

                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>("Sprites/" + sprites[map[x, y]]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < width; x++)
        {
            int firstIndex = map[x, 0];
            for (int y = 0; y < height; y++)
            {
                map[x, y] = y + 1 < height ? map[x, y + 1] : firstIndex;
                childs[x,y].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + sprites[map[x, y]]);
            }
        }
    }
}
