using UnityEngine;
using UnityEditor;

public abstract class Action {
    private string _description;

    public Action(string description)
    {
        _description = description;
    }

    public string GetDescription()
    {
        return _description;
    }

    public abstract void Perform();
}