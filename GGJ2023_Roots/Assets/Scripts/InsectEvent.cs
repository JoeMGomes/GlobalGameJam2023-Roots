using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InsectEvent : Event
{
    private void Start()
    {
        OnStartEvent();
    }

    public override void OnStartEvent()
    {
        base.OnStartEvent();
    }

    public override void OnEndEvent()
    {
        // Behaviour code here before destroy

        base.OnEndEvent();
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
    }
}
