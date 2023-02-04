using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    //Event parameters
    public float lifetime;
    public float health;

    public virtual void OnStartEvent()
    {

    }

    public virtual void OnEndEvent()
    {
        Destroy(this);
    }

    public virtual void TakeDamage()
    {
        health -= (Time.deltaTime * GameManager.instance.playerDamageOverTime);
        Debug.Log(health);
    }
}
