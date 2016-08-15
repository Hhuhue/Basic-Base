using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour
{
    public Entity Person { get; set; } 

    void Start()
    {
        Person = new Entity();
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        float speed = 1;

        if (Input.GetKey(KeyCode.LeftArrow)) move.x = -speed;
        if (Input.GetKey(KeyCode.RightArrow)) move.x = speed;
        if (Input.GetKey(KeyCode.DownArrow)) move.y = -speed;
        if (Input.GetKey(KeyCode.UpArrow)) move.y = speed;

        transform.position += move * Time.deltaTime;
    }

    public void SetSelected(bool state)
    {
        Person.Selected = state;
        string iconPath = !state ? "" : Config.SpritesPath + "circle";

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
    }


}
