using UnityEngine;
using System.Collections;

public class SpawnAction : Action
{
    private EntityFactory _factory;
    private Vector2 _position;

    public SpawnAction(EntityFactory factory, Vector2 position, string description) : base(description)
    {
        _factory = factory;
        _position = position;
    }

    public override void Perform()
    {
        _factory.CreatePerson(_position);
    }
}
