using UnityEngine;
using System.Collections;
using Assets.Scripts.Models.Entities;

public class MoveAction : Action
{
    private Living _living;
    private Vector2 _destination;

    public MoveAction(Living living, Vector2 destination, string description): base(description)
    {
        _living = living;
        _destination = destination;
    }

    public override void Perform()
    {
        _living.GoToPosition(_destination);
    }
}
