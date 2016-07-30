using UnityEngine;
using System.Linq;
using System.Reflection;
using TileType = Map.TileType;
using LandType = Land.LandType;
using Orientation = Map.Orientation;
using Border = Land.Border;

public class ViewController : MonoBehaviour
{
    public View ViewField { get; set; }

    private GameObject[,] _children;
    private Map _map;
    private SpriteRenderer _selectorRenderer;

    void Start()
    {
        SetSelector();

        _children = new GameObject[Config.ViewWidth, Config.ViewHeight];
        _map = GetComponent<MapController>().GetMap();

        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Tile tile = _map.GetTile(x, y);

                GameObject tileObject = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                tileObject.transform.parent = transform;
                tileObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 3);
                tileObject.transform.localScale = Vector3.one;

                GameObject icon = tileObject.transform.GetChild(0).gameObject;
                icon.transform.parent = tileObject.transform;

                tileObject.GetComponent<TileController>().SetTile(tile);

                _children[x, y] = tileObject;
            }
        }
    }

    void SetSelector()
    {
        GameObject selector = new GameObject("selector");
        selector.transform.parent = transform;
        selector.transform.position = new Vector3(0, 0, 1);
        _selectorRenderer = selector.AddComponent<SpriteRenderer>();
        _selectorRenderer.sprite = Resources.Load<Sprite>("Sprites/selection");
        _selectorRenderer.enabled = false;
    }

    public void LoadTile(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public void SelectTile(int xPosition, int yPosition)
    {
        Vector3 position = new Vector3(xPosition + 0.5f, yPosition + 0.5f, 1);

        _selectorRenderer.enabled = true;
        _selectorRenderer.transform.position = position;
    }
}
