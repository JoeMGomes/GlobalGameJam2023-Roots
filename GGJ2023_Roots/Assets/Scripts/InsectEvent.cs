using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InsectEvent : Event
{
    private float elapsedTime;
    private bool hoverEvent;

    private void Start()
    {
        OnStartEvent();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > this.lifetime)
        {
            OnEndEvent();
        }
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
