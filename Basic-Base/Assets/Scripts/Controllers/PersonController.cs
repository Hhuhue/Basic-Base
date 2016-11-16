using System;
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

    public void GoToPosition(Vector2 destination)
    {
        Vector2 personPosition = new Vector2((float)Math.Truncate(Person.Position.x), (float)Math.Truncate(Person.Position.y));
        Vector2 destinationPosition = new Vector2((float)Math.Truncate(destination.x), (float)Math.Truncate(destination.y));

        Vector2[] path = PathFinder.GetPath(personPosition + ViewField.Origin, destinationPosition + ViewField.Origin);

        _targets.Clear();

        foreach (Vector2 step in path)
        {
            _targets.Push(step * 10 - ViewField.Origin + new Vector2(0.5f, 0.5f));
            Debug.Log(_targets.Peek().ToString());
        }

        _currentTarget = _targets.Pop();
    }
}
