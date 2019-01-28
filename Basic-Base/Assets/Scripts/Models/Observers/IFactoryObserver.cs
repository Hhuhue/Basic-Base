using UnityEngine;
using System.Collections;
using Assets.Scripts.Models.Entities;

public interface IFactoryObserver
{
    void OnEntityCreated(Entity createdEntity);

    void OnPersonCreated(Entity createdEntity);
}
