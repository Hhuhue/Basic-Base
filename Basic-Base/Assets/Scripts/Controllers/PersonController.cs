using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Entities;

public class PersonController : MonoBehaviour
{
    public Living Person { get; set; }

    public static View ViewField { get; set; }

    void Start()
    {
        Person = new Living((Vector2) transform.position + ViewField.Origin);
        Person.Selected = false;
    }

    void Update()
    {
        Person.Tick((Vector2)transform.position + ViewField.Origin);

        transform.position = Person.Position - (Vector3)ViewField.Origin;
    }

    public void SetPosition(Vector3 position)
    {
        Person.SetPosition((Vector2)position + ViewField.Origin);
    }

    public void Translate(Vector2 move)
    {
        transform.position += new Vector3(move.x, move.y, 0);
    }

    public void SetSelected(bool state)
    {
        Person.Selected = state;
        string iconPath = !state ? "" : Config.SpritesPath + "circle";

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
    }

    public void GoToPosition(Vector2 destination)
    {
        Person.GoToPosition(destination + ViewField.Origin);
    }
}
