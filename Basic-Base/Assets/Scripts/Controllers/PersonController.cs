using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour
{
    public Entity Person { get; set; }

    private Vector2 _destination;
    private Vector2 _target;

    void Start()
    {
        Person = new Entity();
    }

    void Update()
    {
        if (_destination != _target)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, Time.deltaTime * 2);
        }
    }

    public void SetSelected(bool state)
    {
        Person.Selected = state;
        string iconPath = !state ? "" : Config.SpritesPath + "circle";

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
    }

    public void GoToPosition(Vector2 position)
    {
        _target = position;
    }
}
