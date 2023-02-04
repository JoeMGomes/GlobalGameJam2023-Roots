using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public float instantiateDelay;
    public List<GameObject> eventList;
    public List<Transform> positionsList;

    private void Start()
    {
        StartCoroutine(InstantiateEvent(instantiateDelay));
    }

    IEnumerator InstantiateEvent(float duration)
    {
        yield return new WaitForSeconds(instantiateDelay);
        int id = Random.Range(0, eventList.Count - 1);
        int id2 = Random.Range(0, positionsList.Count - 1);
        Instantiate(eventList[id], positionsList[id2]);
        yield break;
    }
}

public class Event : MonoBehaviour
{
    //Event parameters
    public float lifetime;

    public virtual void OnStartEvent()
    {

    }

    public virtual void OnEndEvent()
    {
        Destroy(this);
    }
}
