using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models.Entities;

public class ActionBase
{
    public Vector2 Location { get; set; }

    public bool FirstPersonSpawned { get; set; }

    public List<Entity> Selection { get; set; }

    public EntityFactory EFactory { get; set; }
}
