using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    private int xPosition;
    private int yPosition;
    public LandType type;
    private GameObject icon;

    void Start()
    {
    }

    void OnMouseDown()
    {

    }

    void OnMouseEnter()
    {
        transform.parent.GetComponent<MapGenerator>().SelectTile(xPosition, yPosition);
    }

    public void SetPosition(int x, int y)
    {
        xPosition = x;
        yPosition = y;
    }

    public void SetLandType(LandType type, Orientation orientation = Orientation.DEFAULT)
    {
        this.type = type;
        icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
        icon.transform.parent = transform;
        icon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        icon.GetComponent<IconController>().SetSprite("Sprites/" + type.ToString().ToLower());
        icon.transform.localEulerAngles = OrientationToVector(orientation);
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
    
    public enum LandType
    {
        DEFAULT,
        WATER,
        PLAIN,
        FOREST,
        MOUNTAIN,
        COAST,
        COAST_CORNER,
        COAST_END
    }

    public enum Orientation
    {
        DEFAULT,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
}
