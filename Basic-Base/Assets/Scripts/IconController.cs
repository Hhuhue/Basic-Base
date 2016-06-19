using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour
{
    private SpriteRenderer renderer;

    void Start()
    {
    }

    public void SetSprite(string path)
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>(path);
        renderer.enabled = false;
        renderer.enabled = true;
    }
}
