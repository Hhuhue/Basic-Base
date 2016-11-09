using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PersonController : MonoBehaviour
{
    public Entity Person { get; set; }

    public static View ViewField { get; set; }
    
    private Stack<Vector2> _targets;
    private Vector2 _currentTarget;

    void Start()
    {
        Person = new Entity()
        {
            Position = transform.position,
            Selected = false
        };

        _targets = new Stack<Vector2>();
        _currentTarget = transform.position;
    }

    void Update()
    {
        if ((Vector2)Person.Position != _currentTarget)
        {
            Person.Position = Vector3.MoveTowards(transform.position, _currentTarget, Time.deltaTime * 2);
            transform.position = Person.Position;
        }
        else if (_targets.Count != 0)
        {
            _currentTarget = _targets.Pop();
        }
    }

    public void SetPosition(Vector3 position)
    {
        Person.Position = position;
    }

    public void Translate(Vector2 move)
    {
        Person.Position += new Vector3(move.x, move.y, 0);
        _currentTarget += new Vector2(move.x, move.y); ;

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

        _targets.Clear();

        foreach (Vector2 step in path)
        {
            _targets.Push(step);
        }

        Debug.Log(pathString);

        _currentTarget = _targets.Pop();
    }
}
