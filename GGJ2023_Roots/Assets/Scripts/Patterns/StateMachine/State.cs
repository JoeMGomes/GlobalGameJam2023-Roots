using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class State
{
    public string name;
    public bool isActive;
    public Action onEnter;
    public Action onExit;

    public State(string name)
    {
        this.name = name;
        isActive = false;
    }
}
