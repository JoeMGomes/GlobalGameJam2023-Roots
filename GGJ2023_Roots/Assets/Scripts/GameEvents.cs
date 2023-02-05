using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private float instantiateDelay;
    public float minInstantiateDelay;
    public float maxInstantiateDelay;

    public List<GameObject> eventList;
    public List<Transform> positionsList;

    public void InstantiateEvent()
    {
        GameManager.GetGameManagerInstance.instanced = true;
        instantiateDelay = Random.Range(minInstantiateDelay, maxInstantiateDelay);
        StartCoroutine(CreateEvent(instantiateDelay));
    }

    IEnumerator CreateEvent(float duration)
    {
        yield return new WaitForSeconds(instantiateDelay);
        if(GameManager.GetGameManagerInstance.tree.stateMachine.GetState() != GameManager.GetGameManagerInstance.tree.transition)
        {
            if (eventList.Count > 0)
            {
                int id = Random.Range(0, eventList.Count - 1);
                int id2 = Random.Range(0, positionsList.Count - 1);
                GameObject instance;
                if (positionsList.Count > 0)
                {
                    instance = Instantiate(eventList[id], positionsList[id2]);
                }
                else
                {
                    instance = Instantiate(eventList[id], GameManager.GetGameManagerInstance.tree.transform);
                }
                instance.GetComponent<Event>().tree = GameManager.GetGameManagerInstance.tree;
                GameManager.GetGameManagerInstance.instanced = false;
                instance.GetComponent<Event>().InstantiateOnEnd = () => CanInstantiate();
            }
        }
        yield break;
    }

    void CanInstantiate()
    {
        StartCoroutine(CreateEvent(instantiateDelay));
    }
}
