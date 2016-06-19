using UnityEngine;
using System.Collections;

public class LandController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Hi");
    }

    public void SetSprite(string sprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
    }
}
