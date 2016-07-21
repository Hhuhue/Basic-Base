using UnityEngine;
using System.Collections;
using Orientation = Map.Orientation;

public class TileController : MonoBehaviour
{
    public Tile Tile;
    public GameObject Map;

    private int _xPosition;
    private int _yPosition;
    private GameObject _icon;

    void OnMouseDown()
    {
        if(Tile.TileType != global::Map.TileType.DEFAULT)
            Map.GetComponent<ViewController>().LoadTile(Tile);
    }
    
    void OnMouseEnter()
    {
        Map.GetComponent<ViewController>().SelectTile(_xPosition, _yPosition);
    }

    public void SetPosition(int x, int y)
    {
        _xPosition = x;
        _yPosition = y;
    }

    public void SetLandType(Tile tile)
    {
        _icon = transform.GetChild(0).gameObject;
        _icon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        _icon.GetComponent<IconController>().SetSprite(Config.TileIconPath + tile.TileType.ToString().ToLower());
        _icon.transform.localEulerAngles = OrientationToVector(tile.Orientation);
    }

    public void SetSprite(string sprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
    }

    public static Vector3 OrientationToVector(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.BOTTOM:
            case Orientation.BOTTOM_LEFT:
                return new Vector3(0, 0, 90);

            case Orientation.RIGHT:
            case Orientation.BOTTOM_RIGHT:
                return new Vector3(0, 0, 180);

            case Orientation.TOP:
            case Orientation.TOP_RIGHT:
                return new Vector3(0, 0, -90);

            default:
                return new Vector3(0, 0, 0);
        }
    }   
}
