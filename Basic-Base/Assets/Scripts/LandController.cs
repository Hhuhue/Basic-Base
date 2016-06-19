using UnityEngine;
using System.Collections;

public class LandController : MonoBehaviour
{
    private int xPosition;
    private int yPosition;
    private LandType type;
    private GameObject icon;

    void Start()
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

    public void SetLandType(LandType type)
    {
        this.type = type;
        icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
        icon.transform.parent = transform;
        icon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        icon.GetComponent<IconController>().SetSprite("Sprites/" + type.ToString().ToLower());
    }

    public void SetSprite(string sprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
    }

    public enum LandType
    {
        WATER,
        PLAIN,
        FOREST,
        MOUNTAIN,
        COAST
    }
}
