using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    private Tile[,] _map;

    void Start()
    {
        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = _map[x, y];

                GameObject tileObject = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                tileObject.transform.parent = transform;
                tileObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 3);
                tileObject.transform.localScale = Vector3.one;

                GameObject icon = tileObject.transform.GetChild(0).gameObject;
                icon.transform.parent = tileObject.transform;

                tileObject.GetComponent<TileController>().SetTile(tile);
                tileObject.GetComponent<TileController>().SetPosition(x, y);
            }
        }
    }

    private void SetHomeMap()
    {
        _map = new Tile[50, 25];
        
         
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
}
