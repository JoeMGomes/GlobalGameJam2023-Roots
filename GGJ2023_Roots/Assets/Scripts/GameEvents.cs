using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private bool canInstantiate;
    private float instantiateDelay;
    public float minInstantiateDelay;
    public float maxInstantiateDelay;

    public List<GameObject> eventList;
    public List<Transform> positionsList;


    private void Start()
    {
        InstantiateEvent();
    }

    public void InstantiateEvent()
    {
        instantiateDelay = Random.Range(minInstantiateDelay, maxInstantiateDelay);
        StartCoroutine(CreateEvent(instantiateDelay));
    }

    IEnumerator CreateEvent(float duration)
    {
        yield return new WaitForSeconds(instantiateDelay);
        if(eventList.Count > 0)
        {
            int id = Random.Range(0, eventList.Count - 1);
            int id2 = Random.Range(0, positionsList.Count - 1);
            GameObject instance = Instantiate(eventList[id], positionsList[id2]);
            instance.GetComponent<Event>().InstantiateOnEnd = () => CanInstantiate();
        }
        yield break;
    }

    void CanInstantiate()
    {
        StartCoroutine(CreateEvent(instantiateDelay));
    }
}
