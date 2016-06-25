using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour
{

    void Start()
    {
    }

    public void SetSprite(string path)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>(path);
    }
}
