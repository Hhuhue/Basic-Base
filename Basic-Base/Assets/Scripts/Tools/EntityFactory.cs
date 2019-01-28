using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Controllers;

public class EntityFactory
{
    private List<IFactoryObserver> _observers;

    public EntityFactory()
    {
        _observers = new List<IFactoryObserver>();
    }

    public void AddObserver(IFactoryObserver observer)
    {
        _observers.Add(observer);
    }

    public void CreatePerson(Vector2 position)
    {
        Vector3 position3 = new Vector3(position.x, position.y, 2.5f);
        Living newPerson = new Living(position3);
        newPerson.Name = "Person 1";
        fireOnEntityCreated(newPerson);
        fireOnPersonCreated(newPerson);
    }

    private void fireOnPersonCreated(Entity createdEntity)
    {
        foreach (IFactoryObserver observer in _observers)
        {
            observer.OnPersonCreated(createdEntity);
        }
    }

    private void fireOnEntityCreated(Entity createdEntity)
    {
        foreach (IFactoryObserver observer in _observers)
        {
            observer.OnEntityCreated(createdEntity);
        }
    }
}
