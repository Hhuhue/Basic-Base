using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    private Map _map;

    void Start()
    {
        Camera.main.transform.position = new Vector3(25, 12.5f, -3);
        Camera.main.orthographicSize = 9;

        LoadMap();

        for (int x = 0; x < Config.MapWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = _map.GetTile(x, y);

                GameObject tileObject = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                tileObject.transform.parent = transform;
                tileObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 3);
                tileObject.transform.localScale = Vector3.one;

                TileController controller = tileObject.GetComponent<TileController>();
                controller.SetTile(tile);
                controller.SetPosition(x, y);
                controller.enabled = false;
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    private void LoadMap()
    {
        Config.MapHeight = Config.ViewHeight;
        Config.MapWidth = Config.ViewWidth;
        Config.FillRatio = 55;
        Config.ForestRatio = 50;
        Config.MountainRatio = 25;
        Config.CoastRatio = 50;
        Config.Seed = new System.Random("Hone".GetHashCode());
        Config.SmoothCount = 1;
        Config.SpritesPath = "Sprites/";

        _map = new Map();
    }
}
