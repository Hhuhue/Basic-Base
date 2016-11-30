using UnityEngine;
using System.Collections;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using Orientation = Assets.Scripts.Models.Mapping.Map.Orientation;
using TileType = Assets.Scripts.Models.Mapping.Tile.TileType;

public class TileController : MonoBehaviour
{
    private Tile _tile;
    private GameObject _icon;
    private TextMesh _textMesh;
    private ViewController _viewController;
    private int _xPosition;
    private int _yPosition;

    void Start()
    {
        _viewController = transform.parent.gameObject.GetComponent<ViewController>();
    }

    void OnMouseDown()
    {
        if (_viewController == null) return;

        if (_tile.Type != TileType.DEFAULT) _viewController.LoadTile(_tile);
    }
    
    void OnMouseEnter()
    {
        if(_viewController == null) return;

        if (Config.ViewMode == View.ViewMode.LAND) DisplayPosition(true);

        _viewController.SelectTile(_xPosition, _yPosition);
    }

    void OnMouseExit()
    {
        if (Config.ViewMode == View.ViewMode.LAND) DisplayPosition(false);
    }

    public void SetPosition(int x, int y)
    {
        _xPosition = x;
        _yPosition = y;
    }

    public void SetTile(Tile tile)
    {
        _tile = tile;

        string sprite = tile.Type.ToString().ToLower();

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Config.SpritesPath + sprite);

        SetIcon(tile);
    }

    private void SetIcon(Tile tile)
    {
        string iconPath = tile.Icon == TileType.DEFAULT ? "" : tile.Icon.ToString().ToLower();

        _icon = transform.GetChild(0).gameObject;
        _icon.transform.position = new Vector3(transform.position.x, transform.position.y, _icon.transform.parent.position.z - 1);
        _icon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Config.SpritesPath + iconPath);
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

    private void DisplayPosition(bool display)
    {
        if (_textMesh == null)
        {
            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<MeshRenderer>();
            textObject.transform.parent = transform;
            textObject.transform.localPosition = Vector3.back;

            _textMesh = textObject.AddComponent<TextMesh>();
            _textMesh.fontSize = 20;
            _textMesh.characterSize = 0.1f;
        }

        _textMesh.text = (display) ? _tile.Position.ToString() : "";
    }   
}
