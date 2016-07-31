using UnityEngine;
using System.Collections;
using Orientation = Map.Orientation;
using TileType = Map.TileType;
using LandType = Land.LandType;

public class TileController : MonoBehaviour
{
    private Tile _tile;
    private GameObject _icon;
    private ViewController _controller;
    private int _xPosition;
    private int _yPosition;

    void Start()
    {
        _controller = transform.parent.gameObject.GetComponent<ViewController>();
    }

    void OnMouseDown()
    {
        if (_tile.TileType != TileType.DEFAULT) _controller.LoadTile(_tile);
    }
    
    void OnMouseEnter()
    {
        if(_controller == null) return;

        _controller.SelectTile(_xPosition, _yPosition);
    }

    public void SetPosition(int x, int y)
    {
        _xPosition = x;
        _yPosition = y;
    }

    public void SetTile(Tile tile)
    {
        _tile = tile;

        bool isTileWater = (Config.ViewMode == View.ViewMode.MAP)
            ? tile.TileType == TileType.WATER
            : tile.LandType == LandType.WATER;

        string groudSprite = (Config.ViewMode == View.ViewMode.MAP)
            ? TileType.PLAIN.ToString().ToLower()
            : LandType.GRASS.ToString().ToLower();

        string sprite = isTileWater ? TileType.WATER.ToString().ToLower() : groudSprite;

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Config.SpritesPath + sprite);

        SetIcon(tile);
    }

    private void SetIcon(Tile tile)
    {
        _tile = tile;

        bool isEmptyLandOrWater = (Config.ViewMode == View.ViewMode.MAP) 
            ? tile.TileType == TileType.PLAIN || tile.TileType == TileType.WATER
            : tile.LandType == LandType.GRASS || tile.LandType == LandType.WATER;

        string iconPath = isEmptyLandOrWater ? "" : (Config.ViewMode == View.ViewMode.MAP) 
            ? Config.SpritesPath + tile.TileType.ToString().ToLower() 
            : Config.SpritesPath + tile.LandType.ToString().ToLower();

        _icon = transform.GetChild(0).gameObject;
        _icon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        _icon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
        _icon.transform.localEulerAngles = OrientationToVector(tile.Orientation);
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
