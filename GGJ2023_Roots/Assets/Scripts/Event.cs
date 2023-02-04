using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    //Event parameters
    public float lifetime;
    public float health;
    protected bool hover;
    public Action InstantiateOnEnd;

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            OnEndEvent();
        }
    }
    public virtual void OnStartEvent()
    {

    }

    public virtual void OnEndEvent()
    {
        InstantiateOnEnd.Invoke();
        Destroy(gameObject);
    }

    public virtual void TakeDamage()
    {
        health -= (Time.deltaTime * GameManager.GetGameManagerInstance().playerDamageOverTime);

        if(health < 0)
        {
            OnEndEvent();
        }
    }

    private void OnMouseEnter()
    {
        hover = true;
    }

    private void OnMouseExit()
    {
        hover = false;
    }

    private void OnMouseDrag()
    {
        if (hover)
        {
            TakeDamage();
        }
    }
}
