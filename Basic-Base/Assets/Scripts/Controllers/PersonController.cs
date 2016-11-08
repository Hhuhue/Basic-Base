using UnityEngine;
using System.Collections;
using System.Linq;

public class PersonController : MonoBehaviour
{
    public Entity Person { get; set; }

    public static View ViewField { get; set; }
    
    private Vector3 _target;

    void Start()
    {
        Person = new Entity()
        {
            Position = transform.position,
            Selected = false
        };

        _target = transform.position;
    }

    void Update()
    {
        if (Person.Position != _target)
        {
            Person.Position = Vector3.MoveTowards(transform.position, _target, Time.deltaTime * 2);
            transform.position = Person.Position;
        }
    }

    public void SetPosition(Vector3 position)
    {
        Person.Position = position;
    }

    public void Translate(Vector2 move)
    {
        Person.Position += new Vector3(move.x, move.y, 0);
        _target += new Vector3(move.x, move.y, 0); ;

        transform.position = Person.Position;
    }

    public void SetSelected(bool state)
    {
        Person.Selected = state;
        string iconPath = !state ? "" : Config.SpritesPath + "circle";

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
    }

    public void GoToPosition(Vector2 position)
    {
        Vector2[] path = PathFinder.GetPath((Vector2)Person.Position + ViewField.Origin, position + ViewField.Origin);

        string pathString = path.Aggregate("[", (current, step) => current + step.ToString() + ", ");
        pathString += "]";

        Debug.Log(pathString);

        _target = position;
    }
}
