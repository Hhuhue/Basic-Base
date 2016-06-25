using UnityEngine;
using System.Collections;
using Orientation = Map.Orientation;

public class TileController : MonoBehaviour
{
    public Tile tile;
    private int xPosition;
    private int yPosition;
    private GameObject icon;

    void Start()
    {

    }

    void OnMouseDown()
    {
        transform.parent.GetComponent<MapController>().LoadTile(tile);
    }
    
    void OnMouseEnter()
    {
        transform.parent.GetComponent<MapController>().SelectTile(xPosition, yPosition);
    }

    public void SetPosition(int x, int y)
    {
        xPosition = x;
        yPosition = y;
    }

    public void SetLandType(Tile tile)
    {
        icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
        icon.transform.parent = transform;
        icon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        icon.GetComponent<IconController>().SetSprite("Sprites/" + tile.TileType.ToString().ToLower());
        icon.transform.localEulerAngles = OrientationToVector(tile.Orientation);
    }

    public void SetSprite(string sprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
    }

    Vector3 OrientationToVector(Orientation orientation)
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
