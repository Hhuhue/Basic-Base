using UnityEngine;
using System.Collections;

public class LandController : MonoBehaviour
{
    private int xPosition;
    private int yPosition;

    void Start()
    {
    }

    public void SetPosition(int x, int y)
    {
        xPosition = x;
        yPosition = y;
    }

    public void SetSprite(string sprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
    }
}
